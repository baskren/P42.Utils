using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P42.Utils;

namespace P42.Utils.AppTest;

[TestClass]
internal class B07_DeviceDisk
{
    [TestMethod]
    public async Task A01_Free()
    {
        Console.WriteLine($"Free: {(await DiskSpace.FreeAsync()).HumanReadableBytes()}");
    }

    [TestMethod]
    public async Task A02_Size()
    {
        Console.WriteLine($"Size: {(await DiskSpace.SizeAsync()).HumanReadableBytes()}");
    }

    [TestMethod]
    public async Task A01_Used()
    {
        Console.WriteLine($"Free: {(await DiskSpace.SizeAsync()).HumanReadableBytes ()}");
    }

}
