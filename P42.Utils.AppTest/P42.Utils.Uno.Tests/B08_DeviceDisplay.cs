using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.Utils.AppTest;

[TestClass]
internal class B08_DeviceDisplay
{
    [TestMethod]
    public void A01_GetInfo()
    {
        Console.WriteLine($"[{P42.Utils.Uno.DeviceDisplay.QueryMainDisplayInfo()}]");
    }
}
