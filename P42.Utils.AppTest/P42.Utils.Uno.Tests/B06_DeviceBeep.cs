using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class B06_DeviceBeep
{
    [TestMethod]
    public void A01_CanBeep()
    {
        Uno.DeviceBeep.CanBeep.ShouldBeTrue();
    }

    [TestMethod]
    public async Task A02_BeepDefault()
    {
        await Uno.DeviceBeep.PlayAsync();
    }

    [TestMethod]
    public async Task A03_LowBeep()
    {
        await Uno.DeviceBeep.PlayAsync(800, 600);
    }

    [TestMethod]
    public async Task A03_HiBeep()
    {
        await Uno.DeviceBeep.PlayAsync(6000, 300);
    }
}
