using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.Utils.AppTest;

[TestClass]
public class B09_DeviceInfo
{
    [TestMethod]
    public void A01_Make()
    {
        Console.WriteLine($"Platform:[{Uno.DeviceInfo.Make}]");
    }

    [TestMethod]
    public void A02_Model()
    {
        Console.WriteLine($"Model:[{Uno.DeviceInfo.Model}]");
    }

    [TestMethod]
    public void A03_DeviceName()
    {
        Console.WriteLine($"Name:[{Uno.DeviceInfo.DeviceName}]");
    }

    [TestMethod]
    public void A03_DeviceId()
    {
        Console.WriteLine($"Name:[{Uno.DeviceInfo.DeviceId}]");
    }

    [TestMethod]
    public void A05_DeviceForm()
    {
        Console.WriteLine($"DeviceForm:[{Uno.DeviceInfo.DeviceForm}]");
    }

    [TestMethod]
    public void A06_Os()
    {
        Console.WriteLine($"OsVersion:[{Uno.DeviceInfo.Os}]");
    }

    [TestMethod]
    public void A07_OsVersion()
    {
        Console.WriteLine($"OsVersion:[{Uno.DeviceInfo.OsVersion}]");
    }

    [TestMethod]
    public void A08_OsDescription()
    {
        Console.WriteLine($"OsVersion:[{Uno.DeviceInfo.OsDescription}]");
    }

    [TestMethod]
    public void A09_IsEmulator()
    {
        Console.WriteLine($"IsEmulator:[{Uno.DeviceInfo.IsEmulator}]");
    }

    [TestMethod]
    public void A10_RuntimeIdentifier()
    {
        Console.WriteLine($"OsVersion:[ {Uno.DeviceInfo.RuntimeIdentifier}]");
    }

    [TestMethod]
    public void A11_FrameworkDescription()
    {
        Console.WriteLine($"OsVersion:[ {Uno.DeviceInfo.FrameworkDescription}]");
    }
}
