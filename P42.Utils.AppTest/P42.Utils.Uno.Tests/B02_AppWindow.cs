using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class B02_AppWindow
{
    [TestMethod]
    public void A01_Size()
    {
        var size = P42.Utils.Uno.AppWindow.Size();
        size.Width.ShouldBeGreaterThan(0);
        size.Height.ShouldBeGreaterThan(0);
        Console.WriteLine($"AppWindow.Size:[{size}]");
    }

    [TestMethod]
    public void A02_CurrentPage()
    {
        var page = P42.Utils.Uno.AppWindow.CurrentPage;
        page.GetType().ShouldBe(typeof(P42.UnoTestRunner.TestControlPage));
    }

    [TestMethod]
    public void A03_CurrentFrame()
    {
        var frame = Uno.AppWindow.CurrentFrame;
        frame.Content.GetType().ShouldBe(typeof(P42.UnoTestRunner.TestControlPage));
    }

}
