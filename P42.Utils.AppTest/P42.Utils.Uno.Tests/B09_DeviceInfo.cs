using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.Utils.AppTest;

[TestClass]
internal class B09_DeviceInfo
{
    [TestMethod]
    public void A01_Make()
    {
        Console.WriteLine($"Platform:[{P42.Utils.Uno.DeviceInfo.Make}]");
    }

    [TestMethod]
    public void A02_Model()
    {
        Console.WriteLine($"Model:[{P42.Utils.Uno.DeviceInfo.Model}]");
    }

    [TestMethod]
    public void A03_DeviceName()
    {
        Console.WriteLine($"Name:[{P42.Utils.Uno.DeviceInfo.DeviceName}]");
    }

    [TestMethod]
    public void A03_DeviceId()
    {
        Console.WriteLine($"Name:[{P42.Utils.Uno.DeviceInfo.DeviceId}]");
    }

    [TestMethod]
    public void A05_DeviceForm()
    {
        Console.WriteLine($"DeviceForm:[{P42.Utils.Uno.DeviceInfo.DeviceForm}]");
    }

    [TestMethod]
    public void A06_Os()
    {
        Console.WriteLine($"OsVersion:[{P42.Utils.Uno.DeviceInfo.Os}]");
    }

    [TestMethod]
    public void A07_OsVersion()
    {
        Console.WriteLine($"OsVersion:[{P42.Utils.Uno.DeviceInfo.OsVersion}]");
    }

    [TestMethod]
    public void A08_OsDescription()
    {
        Console.WriteLine($"OsVersion:[{P42.Utils.Uno.DeviceInfo.OsDescription}]");
    }

    [TestMethod]
    public void A09_IsEmulator()
    {
        Console.WriteLine($"IsEmulator:[{P42.Utils.Uno.DeviceInfo.IsEmulator}]");
    }

    [TestMethod]
    public void A10_RuntimeIdentifier()
    {
        Console.WriteLine($"OsVersion:[ {P42.Utils.Uno.DeviceInfo.RuntimeIdentifier}]");
    }

    [TestMethod]
    public void A11_FrameworkDescription()
    {
        Console.WriteLine($"OsVersion:[ {P42.Utils.Uno.DeviceInfo.FrameworkDescription}]");
    }
}
