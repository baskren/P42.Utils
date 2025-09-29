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
using AsyncAwaitBestPractices;
using Uno.Extensions;

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
    ConsoleOutputRedirector? _consoleOutputRedirector;
    public ConsoleOutputRedirector ConsoleOutputRedirector => _consoleOutputRedirector ??= new();
    List<TreeViewNode> LastSelectedNodes = [];
    #endregion


    public TestControlPage()
    {
        Instance = this;
        this.InitializeComponent();

        var testTree = TestRunner.GetTestTree();

        if (testTree.Count == 0)
            return;

        /*
        var nodeTree = testTree.Count == 1
            ? testTree[0].CreateTreeNode()
            : testTree.CreateTreeNode();
        */

        var nodeTree = testTree.CreateTreeNode();

        foreach (var rootNode in nodeTree.Children)
            testsTreeView.RootNodes.Add(rootNode);

        LastSelectedNodes = nodeTree.SelectedByDefault();
        testsTreeView.SelectedNodes.AddRange(LastSelectedNodes);

        testsTreeView.SelectionChanged += TestsTreeView_SelectionChanged;

        Loaded += TestControlPage_Loaded;
    }

    private void TestControlPage_Loaded(object sender, RoutedEventArgs e)
    {
        ConsoleOutputRedirector.ContentChanged += OnConsoleContentChanged;
        ConsoleOutputRedirector.Start();
        stopStartTestButton.IsEnabled = testsTreeView.SelectedNodes.Any();
    }

    private void TestsTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        List<TreeViewNode> lastSelectedNodes = [..LastSelectedNodes];

        //System.Diagnostics.Debug.WriteLine($"Added:[{string.Join(", ", args.AddedItems.Select(i => ((TreeViewNode)i).Content))}]  Removed:[{string.Join(", ", args.RemovedItems.Select(i => ((TreeViewNode)i).Content))}]  SelectedNode:[{sender.SelectedNode.Content}]");
        stopStartTestButton.IsEnabled = testsTreeView.SelectedNodes.Any();

        List<TreeViewNode> toRemove = [];
        if (args.AddedItems.Count > 1)
        {
            var selectedNode = args.AddedItems[0] as TreeViewNode;
            for (int i = 1; i < args.AddedItems.Count; i++)
            {
                var item = args.AddedItems[i];
                if (item is not TreeViewNode node)
                    continue;
                if (node.Content is UnitTestMethodInfo { Method: MethodInfo method } &&
                    method.HasAttribute(typeof(OnlyExplicitlySelectableAttribute))
                    )
                    toRemove.Add(node);
                if (node.Content is UnitTestClassInfo c &&
                    c.Type.HasAttribute(typeof(OnlyExplicitlySelectableAttribute)) )
                {
                    var deletes = node.Children.Where(node => node != selectedNode && !lastSelectedNodes.Contains(node));
                    toRemove.AddRange( deletes );
                }
            }
        }

        List<TreeViewNode> toReturn = [];
        if (args.RemovedItems.Count > 1)
        {
            var deselectedNode = args.RemovedItems[0] as TreeViewNode;
            for (int i = 1; i < args.RemovedItems.Count; i++)
            {
                var item = args.RemovedItems[i];
                if (item is not TreeViewNode node)
                    continue;
                if (node.Content is UnitTestMethodInfo { Method: MethodInfo method } &&
                    method.HasAttribute(typeof(OnlyExplicitlyUnselectableAttribute))
                    )
                    toReturn.Add(node);
                if (node.Content is UnitTestClassInfo c &&
                    c.Type.HasAttribute(typeof(OnlyExplicitlyUnselectableAttribute)))
                {
                    var adds = node.Children.Where(node => node != deselectedNode && lastSelectedNodes.Contains(node));
                    toReturn.AddRange(adds);
                }
            }
        }

        foreach (var node in toRemove)
            sender.SelectedNodes.Remove(node);
        foreach (var node in toReturn)
            sender.SelectedNodes.Add(node);

        LastSelectedNodes = [.. sender.SelectedNodes];
    }
    

    private void OnConsoleContentChanged(object? sender, string e)
        => MainThread.Invoke(() =>
        {
            consoleTextBlock.Text = e;
            consoleScrollViewer.ChangeView(null, consoleScrollViewer.ScrollableHeight, null);
        });
    

    GridLength GridLengthZero = new GridLength(0);
    GridLength GridLengthStar = new GridLength(1, GridUnitType.Star);
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
            resultsGridCol0.Width = GridLengthStar;
            resultsGridCol1.Width = GridLengthZero;
            resultsGridRow0.Height = new GridLength(3 * args.NewSize.Height / 4);
            resultsGridRow1.Height = new GridLength(args.NewSize.Height / 4);
            col1.MinWidth = 300;
            col2.MinWidth = 300;
            col1.Width = default;
            col2.Width = default;

            resultsGrid.MaxHeight = double.MaxValue;
            row1.MinHeight = 0;
            row2.MinHeight = 0;
            row1.Height = GridLengthZero;
            row2.Height = GridLengthZero;

            consoleRowSplitter.Visibility = Visibility.Visible;
            consoleColSplitter.Visibility = Visibility.Collapsed;

            Grid.SetRow(consoleOutputGrid, 1);
            Grid.SetColumn(consoleOutputGrid, 0);

            resultsGridRow0.MinHeight = 100;
            resultsGridRow1.MinHeight = 100;
            resultsGridCol0.MinWidth = 0;
            resultsGridCol1.MinWidth = 0;
        }
        else
        {
            Grid.SetRow(resultsGrid, 1);
            Grid.SetColumn(resultsGrid, 0);
            Grid.SetRow(testsTreeGrid, 2);
            Grid.SetColumn(testsTreeGrid, 0);

            resultsGrid.MaxWidth = double.MaxValue;
            resultsGridCol0.Width = new GridLength(args.NewSize.Width / 2);
            resultsGridCol1.Width = new GridLength(args.NewSize.Width / 2);
            resultsGridRow0.Height = GridLengthStar;
            resultsGridRow1.Height = GridLengthZero;
            col1.MinWidth = 0;
            col2.MinWidth = 0;
            col1.Width = GridLengthZero;
            col2.Width = GridLengthZero;

            resultsGrid.MaxHeight = args.NewSize.Height/2;
            row1.MinHeight = 200;
            row2.MinHeight = 200;
            row1.Height = default;
            row2.Height = default;

            consoleRowSplitter.Visibility = Visibility.Collapsed;
            consoleColSplitter.Visibility = Visibility.Visible;

            Grid.SetRow(consoleOutputGrid, 0);
            Grid.SetColumn(consoleOutputGrid, 1);

            resultsGridRow0.MinHeight = 0;
            resultsGridRow1.MinHeight = 0;
            resultsGridCol0.MinWidth = 100;
            resultsGridCol1.MinWidth = 100;

        }

        System.Diagnostics.Debug.WriteLine($"col1.MaxWidth:[{col1.MaxWidth}]");
    }

    private async void stopStartTestButton_Click(object sender, RoutedEventArgs e)
    {
        stopStartTestButton.IsEnabled = false;
        if (TestRun is not TestRun run || run.State != TestRunState.Running)
            RunTest().SafeFireAndForget(ex => ShowException(ex));
        else
        {
            ProgressRingShowCancelling();
            await run.StopAsync();
        }

        stopStartTestButton.IsEnabled = true;
    }

    void ProgressRingStart()
    {
        busyOverlay.Visibility = Visibility.Visible;
        progressRing.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Blue);
        progressRing.IsActive = true;
    }

    void ProgressRingStop()
    {
        busyOverlay.Visibility = Visibility.Collapsed;
        progressRing.IsActive = false;
    }

    void ProgressRingShowCancelling()
    {
        progressRing.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
    }

    async Task RunTest()
    {
        try
        {
            ConsoleOutputRedirector.Reset();
            ProgressRingStart();
            var selectedNodes = testsTreeView.SelectedNodes;
            var selectedTestNodes = selectedNodes.Where(n => !n.HasChildren).Select(n => n.Content);
            var selectedTests = selectedTestNodes.Cast<UnitTestMethodInfo>();

            TestRun = new TestRun();
            TestRun.PropertyChanged += OnTestRun_PropertyChanged;
            await TestRun.ExecuteAsync(selectedTests);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
        finally
        {
            if (TestRun is not null)
            {
                TestRun.PropertyChanged -= OnTestRun_PropertyChanged;
                TestRun.State = TestRunState.Completed;
            }
            ProgressRingStop();
        }
    }

    private void OnTestRun_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TestRun.ResultLog))
            MainThread.Invoke(() =>
            { 
                resultsTextBlock.Text = TestRun?.ResultLog;
                resultsScrollViewer.ChangeView(null, resultsScrollViewer.ScrollableHeight, null);
            });
        else if (e.PropertyName == nameof(TestRun.State))
        {
            MainThread.Invoke(() => 
            stopStartTestButton.Content = TestRun?.State == TestRunState.Running
                ? "CANCEL"
                : "RUN ⏵");

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

    private void copyConsoleButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(consoleTextBlock.Text))
            return;
        DataPackage dataPackage = new DataPackage();
        dataPackage.SetText(consoleTextBlock.Text);
        Clipboard.SetContent(dataPackage);

    }

    public void ShowException(Exception ex)
    {
        MainThread.Invoke(async () =>
        {
            var dialog = new ContentDialog();
            var text = ex.ToString();
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Title = "TEST RUN EXCEPTION";
            dialog.PrimaryButtonText = string.IsNullOrWhiteSpace(text) ? "" : "COPY";
            dialog.CloseButtonText = "CANCEL";
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.Content = new ScrollViewer
            {
                Content = new TextBlock { Text = text },
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;
                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);

            }
        });
    }
}


