using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class A03_LocalData
{
    public class MyTestCacheableNotifiablePropertyObjectClass : global::P42.Utils.CacheableNotifiablePropertyObject
    {
        private readonly string _instanceName;

        public string MyName
        {
            get => GetCachedValue(_instanceName, string.Empty) ?? string.Empty;
            set => SetCachedValue(_instanceName, value);
        }

        public MyTestCacheableNotifiablePropertyObjectClass(string instanceName)
            => _instanceName = instanceName;
    }

    const string TestString = "rumpelstiltskin";

//#if !BROWSERWASM  // IntersessionCaching fails in WASM unless the following property is added to the Build Project:       <WasmShellEnableIDBFS>true</WasmShellEnableIDBFS>
    [TestMethod]
    public void A00_IntersessionCaching()
    {
        // Test if values are stored between app sessions
        var cacheable = new MyTestCacheableNotifiablePropertyObjectClass("Instance.000");
        if (cacheable.MyName != TestString)
            throw new Exception("Should ONLY fail upon first run of test on a given platform.  If you see two times in a row, there is an error OR `<WasmShellEnableIDBFS>true</WasmShellEnableIDBFS>` was not added to Project's .csproj.");
    }
//#endif

    [TestMethod]
    public void A01_SetGet()
    {
        var cacheable0 = new MyTestCacheableNotifiablePropertyObjectClass("Instance.000");
        cacheable0.MyName = TestString;
        cacheable0.MyName.ShouldBe(TestString);
    }

    [TestMethod]
    public void A02_InstanceSeparation()
    {
        var cacheable1 = new MyTestCacheableNotifiablePropertyObjectClass("Instance.001");
        cacheable1.MyName.ShouldNotBe(TestString);
        var guid = Guid.NewGuid().ToString();
        cacheable1.MyName = guid;
        cacheable1.MyName.ShouldBe(guid);

        var cacheable0 = new MyTestCacheableNotifiablePropertyObjectClass("Instance.000");
        cacheable0.MyName.ShouldBe(TestString);
    }
}
