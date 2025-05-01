using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace P42.UnoTestRunner;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Bindable]
public sealed partial class TestControlPage : Page
{
    #region Properties

    static TestControlPage? _instance;
    public static TestControlPage Instance 
    {
        get => _instance ?? throw new Exception("TestConrolPage.Instance called before being set"); 
        private set
        {
            if (_instance != null)
                throw new Exception("TestControlPage.Instance is a singleton and cannot be reset");
            _instance = value;
        }
    }

    internal static (UIElement Control, Func<UIElement?> GetContent, Action<UIElement?> SetContent) EmbeddedTestRoot 
        => (Instance, () => Instance.TestContent, (e) => Instance.TestContent = e);


    #region TestContent Property
    public static readonly DependencyProperty TestContentProperty = DependencyProperty.Register(
        nameof(TestContent),
        typeof(UIElement),
        typeof(TestControlPage),
        new PropertyMetadata(default(UIElement), (d,e) => ((TestControlPage)d).OnTestContentChanged(e))
    );

    private void OnTestContentChanged(DependencyPropertyChangedEventArgs args)
    {
        if (args.OldValue is UIElement oldTestContent)
            grid.Children.Remove(oldTestContent);
        if (args.NewValue is UIElement newTestContent)
            grid.Children.Add(newTestContent);
    }

    public UIElement? TestContent
    {
        get => (UIElement?)GetValue(TestContentProperty);
        set => SetValue(TestContentProperty, value);
    }
    #endregion TestContent Property

    #endregion


    #region Fields

    TestRun? TestRun;

    #endregion


    public TestControlPage()
    {
        Instance = this;
        this.InitializeComponent();

        TestApplication.Instance.ConsoleOutputRedirector.ContentChanged += OnConsoleContentChanged;

        var testTree = TestRunner.GetTestTree();

        if (testTree.Keys.Count == 0)
            return;

        var nodeTree = testTree.CreateTreeNode();
        foreach (var rootNode in nodeTree.Children)
            testsTreeView.RootNodes.Add(rootNode);

        testsTreeView.SelectionChanged += TestsTreeView_SelectionChanged;
    }

    private void TestsTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
        => stopStartTestButton.IsEnabled = testsTreeView.SelectedNodes.Any();
    

    private void OnConsoleContentChanged(object? sender, string e)
        => consoleTextBlock.Text = e;
    

    GridLength GridLengthZero = new GridLength(0);
    private void Page_SizeChanged(object sender, SizeChangedEventArgs args)
    {
        if (args.NewSize.Height <= 0)
            return;

        var newAspect = args.NewSize.Width / args.NewSize.Height;
        /*
        if (args.PreviousSize.Height > 0)
        {
            var oldAspect = args.PreviousSize.Width / args.PreviousSize.Height;
            if ((newAspect > 1) == (oldAspect > 1))
                return;
        }
        */

        if (newAspect > 1)
        {
            Grid.SetRow(resultsGrid, 0);
            Grid.SetColumn(resultsGrid, 1);
            Grid.SetRow(testsTreeGrid, 0);
            Grid.SetColumn(testsTreeGrid, 2);

            resultsGrid.MaxWidth = args.NewSize.Width / 2;
            col1.MinWidth = 300;
            col2.MinWidth = 300;
            col1.Width = default;
            col2.Width = default;

            row1.MaxHeight = default;
            row1.MinHeight = 0;
            row2.MinHeight = 0;
            row1.Height = GridLengthZero;
            row2.Height = GridLengthZero;
        }
        else
        {
            Grid.SetRow(resultsGrid, 1);
            Grid.SetColumn(resultsGrid, 0);
            Grid.SetRow(testsTreeGrid, 2);
            Grid.SetColumn(testsTreeGrid, 0);

            resultsGrid.MaxWidth = double.NaN;
            col1.MinWidth = 0;
            col2.MinWidth = 0;
            col1.Width = GridLengthZero;
            col2.Width = GridLengthZero;

            row1.MaxHeight = args.NewSize.Height/2;
            row1.MinHeight = 200;
            row2.MinHeight = 200;
            row1.Height = default;
            row2.Height = default;
        }

        System.Diagnostics.Debug.WriteLine($"col1.MaxWidth:[{col1.MaxWidth}]");
    }

    private async void stopStartTestButton_Click(object sender, RoutedEventArgs e)
    {
        stopStartTestButton.IsEnabled = false;
        if (TestRun is not TestRun run || run.State != TestRunState.Running)
            RunTest();
        else 
            await run.StopAsync();

        stopStartTestButton.IsEnabled = true;
    }

    async Task RunTest()
    {
        try
        {
            var selectedNodes = testsTreeView.SelectedNodes;
            var selectedTestNodes = selectedNodes.Where(n => !n.HasChildren).Select(n => n.Content);
            var selectedTests = selectedTestNodes.Cast<UnitTestMethodInfo>();

            TestRun = new TestRun();
            TestRun.PropertyChanged += OnTestRun_PropertyChanged;
            await TestRun.ExecuteAsync(selectedTests);
            TestRun.PropertyChanged -= OnTestRun_PropertyChanged;
        }
        catch (Exception ex)
        {
            var dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Title = "TEST RUN EXCEPTION";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.Content = new ScrollViewer
            {
                Content = new TextBlock { Text = ex.ToString() },
            };

            await dialog.ShowAsync();
        }
    }

    private void OnTestRun_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TestRun.ResultLog))
            resultsTextBlock.Text = TestRun?.ResultLog;
        else if (e.PropertyName == nameof(TestRun.State))
        {
            stopStartTestButton.Content = TestRun?.State == TestRunState.Running
                ? "CANCEL"
                : "RUN ⏵";
        }
    }

    private void ResultsCopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(resultsTextBlock.Text)) 
            return;
        DataPackage dataPackage = new DataPackage();
        dataPackage.SetText(resultsTextBlock.Text);
        Clipboard.SetContent(dataPackage);
    }
}


