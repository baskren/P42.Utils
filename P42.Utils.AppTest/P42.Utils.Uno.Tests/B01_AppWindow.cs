using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
// ReSharper disable once InconsistentNaming
public class B01_AppWindow
{
    [TestMethod]
    public void A01_Size()
    {
        var size = Uno.AppWindow.Size();
        size.Width.ShouldBeGreaterThan(0);
        size.Height.ShouldBeGreaterThan(0);
        // ReSharper disable once LocalizableElement
        Console.WriteLine($"AppWindow.Size:[{size}]");
    }

    [TestMethod]
    public void A02_CurrentPage()
    {
        var page = Uno.AppWindow.CurrentPage;
        page.ShouldNotBeNull();
        page.GetType().ShouldBe(typeof(UnoTestRunner.TestControlPage));
    }

    [TestMethod]
    public void A03_CurrentFrame()
    {
        //var x = ushort.MaxValue;
        var frame = Uno.AppWindow.CurrentFrame;
        frame.ShouldNotBeNull();
        frame.Content.GetType().ShouldBe(typeof(UnoTestRunner.TestControlPage));
    }

}
