using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Uno.UI.RuntimeTests;

namespace P42.Utils.Tests.Tests;

[TestClass]
internal class A00_PlatformInitialization
{
    [TestMethod]
    public void A00_TestUninitialized()
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


}
