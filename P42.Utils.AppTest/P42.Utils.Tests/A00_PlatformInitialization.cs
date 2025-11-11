using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P42.UnoTestRunner;
using P42.Utils.Uno;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
[SelectedByDefault]
[OnlyExplicitlyUnselectable]
// ReSharper disable once InconsistentNaming
public class A00_PlatformInitialization
{
    private static bool _hasBeenRunBefore;

    [TestMethod]
    public void A00_TestUninitialized()
    {
        // un comment before pushing!
        if (!_hasBeenRunBefore)
            Assert.ThrowsException<NotInitializedException>(() => _ = P42.Utils.Uno.Platform.Application);
        else
            Assert.IsNotNull(P42.Utils.Uno.Platform.Application);
        _hasBeenRunBefore = true;
    }

    [TestMethod]
    [RunsOnUIThread]
    public void A01_TestInitialize()
    {
        Assert.IsNotNull(TestApplication.MainWindow);
        P42.Utils.Uno.Platform.Init(Application.Current, TestApplication.MainWindow);
    }

    [TestMethod]
    [RunsOnUIThread]
    public void A02_P42_Utils_Uno_Platform_Properties()
    {
        P42.Utils.Uno.Platform.Application.ShouldBe(Application.Current);
        P42.Utils.Uno.Platform.MainWindow.ShouldBe(TestApplication.MainWindow);
        Thread.CurrentThread.ShouldBe(Uno.MainThread.Current);
        Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().ShouldBe(Uno.MainThread.DispatchQueue);
        Platform.ApplicationLocalFolderPath.ShouldNotBeNull();
        Platform.ApplicationLocalCacheFolderPath.ShouldNotBeNull();
        Platform.ApplicationTemporaryFolderPath.ShouldNotBeNull();
    }


    
    [TestMethod]
    [RunsOnUIThread]
    public async Task A03_WebView2_Working()
    {
        var rect = new Rectangle
        {
            Fill = Microsoft.UI.Colors.Pink.ToBrush(),
            Width = 500, Height = 500
        };
        Grid.SetRow(rect, 1);
        var wv2 = new WebView2
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        Grid.SetRow(wv2, 1);

        var button = new Button
        {
            Content = "THIS IS A BUTTON"
        };


        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Pixel) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.Children.Add(rect);
        grid.Children.Add(button);
        grid.Children.Add(wv2);
        UnitTestsUIContentHelper.Content = grid;

        await wv2.EnsureCoreWebView2Async();

        await UnitTestsUIContentHelper.WaitForIdle();

        wv2.NavigateToString("THIS IS A STRING SOURCE");


    }
    

}
