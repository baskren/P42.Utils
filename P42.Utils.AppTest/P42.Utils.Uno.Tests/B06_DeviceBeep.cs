using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
internal class B06_DeviceBeep
{
    [TestMethod]
    public void A01_CanBeep()
    {
        P42.Utils.Uno.DeviceBeep.CanBeep.ShouldBeTrue();
    }

    [TestMethod]
    public async Task A02_BeepDefault()
    {
        await P42.Utils.Uno.DeviceBeep.PlayAsync();
    }

    [TestMethod]
    public async Task A03_LowBeep()
    {
        await P42.Utils.Uno.DeviceBeep.PlayAsync(800, 600);
    }

    [TestMethod]
    public async Task A03_HiBeep()
    {
        await P42.Utils.Uno.DeviceBeep.PlayAsync(6000, 300);
    }
}
