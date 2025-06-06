using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P42.UnoTestRunner;

namespace P42.Utils.AppTest;

[TestClass]
[OnlyExplicitlySelectable]
internal class A02_LocalDataReset
{
    [TestMethod]
    public void A00_LocalDataReset()
    {
        P42.Utils.LocalData.Clear();
    }
}
