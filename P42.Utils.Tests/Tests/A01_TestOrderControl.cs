using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.Utils.Tests.Tests;

[TestClass]
internal class A01_TestOrderControl
{
    [TestMethod]
    public void A()
    {
        Assert.IsNotNull(App.Instance?.MainWindow);
        try
        {
            _ = P42.Utils.Uno.Platform.Application;
        }
        catch (P42.Utils.Uno.NotInitializedException)
        {
            throw new Exception($"TEST ORDER IS NOT AS EXPECTED.  THIS TEST SHOULD BE AFTER {nameof(A00_PlatformInitialization)}.{nameof(A00_PlatformInitialization.A00_TestUninitialized)}.");
        }
    }
}
