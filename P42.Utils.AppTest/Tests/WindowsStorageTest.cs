using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class WindowsStorageTest
{
    // https://github.com/unoplatform/uno.ui.runtimetests.engine

    [TestMethod]
    public void Test1()
    {
        var path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        path.ShouldNotBeNullOrWhiteSpace();


        // throw new Exception(path);  // use exception to see the valid produced during testing

    }

}
