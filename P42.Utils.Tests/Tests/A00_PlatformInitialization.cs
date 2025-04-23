using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P42.Utils.Uno;
using Shouldly;
using Uno.UI.RuntimeTests;

namespace P42.Utils.Tests;

[TestClass]
internal class A00_PlatformInitialization
{
    [TestMethod]
    public async Task A00_TestUninitialized()
    {
        // un comment before pushing!
        // Assert.ThrowsException<P42.Utils.Uno.NotInitializedException>(() => _ = P42.Utils.Uno.Platform.Application);


    }

    [TestMethod]
    [RunsOnUIThread]
    public void A01_TestInitialize()
    {
        Assert.IsNotNull(App.Instance?.MainWindow);
        P42.Utils.Uno.Platform.Init(App.Instance!, App.Instance!.MainWindow!);
    }

    [TestMethod]
    [RunsOnUIThread]
    public void A02_P42_Utils_Uno_Platform_Properties()
    {
        P42.Utils.Uno.Platform.Application.ShouldBe(App.Instance!);
        P42.Utils.Uno.Platform.MainWindow.ShouldBe(App.Instance!.MainWindow);
        Thread.CurrentThread.ShouldBe(P42.Utils.Uno.Platform.MainThread);
        Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().ShouldBe(P42.Utils.Uno.Platform.MainThreadDispatchQueue);
    }

    /*
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
            VerticalAlignment = VerticalAlignment.Stretch,
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
    */

}
