using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.Tests;

[TestClass]
public class A04_LocalData_UrlItem
{
    private static readonly Uri SourceUri = new Uri("https://raw.githubusercontent.com/cbracco/html5-test-page/refs/heads/master/index.html");
    private static readonly Uri RootUri = new Uri("https://raw.githubusercontent.com/cbracco/html5-test-page/refs/heads/master/");
    private static readonly LocalData.UriItemKey UriKey = LocalData.UriItemKey.Get(SourceUri, RootUri);
    private static readonly LocalData.UriItemKey AltUriKey1 = LocalData.UriItemKey.Get(SourceUri, new Uri("https://raw.githubusercontent.com/cbracco/html5-test-page/refs/heads"));
    private static readonly LocalData.UriItemKey AltUriKey2 = LocalData.UriItemKey.Get(SourceUri, RootUri, "AltUris");
    private static readonly LocalData.UriItemKey AltUriKey3 = LocalData.UriItemKey.Get(SourceUri, RootUri, assembly: typeof(P42.Utils.Uno.Platform).GetAssembly());


    [TestMethod]
    public void A01_KeyEquality()
    {
        var key = LocalData.UriItemKey.Get(SourceUri, RootUri);
        key.FullPath.ShouldBe(UriKey.FullPath);
        key.ShouldBe(UriKey);

        key.FullPath.ShouldNotBe(AltUriKey1.FullPath);
        key.ShouldNotBe(AltUriKey1);
        key.FullPath.ShouldNotBe(AltUriKey2.FullPath);
        key.ShouldNotBe(AltUriKey2);
        key.FullPath.ShouldNotBe(AltUriKey3.FullPath);
        key.ShouldNotBe(AltUriKey3);
    }

    [TestMethod]
    public void A02_ClearAndPull()
    {
        UriKey.Clear();
        UriKey.Exists.ShouldBeFalse();

    }
}
