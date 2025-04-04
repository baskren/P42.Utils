using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.Tests.Tests;

[TestClass]
internal class A02_P42_Utils_CacheableNotifiablePropertyObject
{
    public class MyTestCacheableNotifiablePropertyObjectClass : P42.Utils.CacheableNotifiablePropertyObject
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

    const string MyName = "rumpelstiltskin";

#if !BROWSERWASM  // IntersessionCaching fails in WASM
    [TestMethod]
    public void A00_IntersessionCaching()
    {
        // Test if values are stored between app sessions
        var cacheable = new MyTestCacheableNotifiablePropertyObjectClass($"{nameof(MyTestCacheableNotifiablePropertyObjectClass)}.000");
        if (cacheable.MyName != MyName)
            throw new Exception("Should ONLY fail upon first run of test on a given platform.  Except WASM, if you see two times in a row, there is an error.");
    }
#endif

    [TestMethod]
    public void A01_SetGet()
    {
        var cacheable0 = new MyTestCacheableNotifiablePropertyObjectClass($"{nameof(MyTestCacheableNotifiablePropertyObjectClass)}.000");
        cacheable0.MyName = MyName;
        cacheable0.MyName.ShouldBe(MyName);
    }

    [TestMethod]
    public void A02_InstanceSeparation()
    {
        var cacheable1 = new MyTestCacheableNotifiablePropertyObjectClass($"{nameof(MyTestCacheableNotifiablePropertyObjectClass)}.001");
        cacheable1.MyName.ShouldNotBe(MyName);
        var guid = Guid.NewGuid().ToString();
        cacheable1.MyName = guid;
        cacheable1.MyName.ShouldBe(guid);

        var cacheable0 = new MyTestCacheableNotifiablePropertyObjectClass($"{nameof(MyTestCacheableNotifiablePropertyObjectClass)}.000");
        cacheable0.MyName.ShouldBe(MyName);
    }
}
