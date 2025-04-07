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
    private static readonly LocalData.UriItem UriItem = LocalData.UriItem.Get(SourceUri, RootUri);
    private static readonly LocalData.UriItem AltUriItem1 = LocalData.UriItem.Get(SourceUri, new Uri("https://raw.githubusercontent.com/cbracco/html5-test-page/refs/heads"));
    private static readonly LocalData.UriItem AltUriItem2 = LocalData.UriItem.Get(SourceUri, RootUri, "AltUris");
    private static readonly LocalData.UriItem AltUriItem3 = LocalData.UriItem.Get(SourceUri, RootUri, assembly: typeof(P42.Utils.Uno.Platform).GetAssembly());


    [TestMethod]
    public void A01_KeyEquality()
    {
        var item = LocalData.UriItem.Get(SourceUri, RootUri);
        item.FullPath.ShouldBe(UriItem.FullPath);
        item.ShouldBe(UriItem);

        item.FullPath.ShouldNotBe(AltUriItem1.FullPath);
        item.ShouldNotBe(AltUriItem1);
        item.FullPath.ShouldNotBe(AltUriItem2.FullPath);
        item.ShouldNotBe(AltUriItem2);
        item.FullPath.ShouldNotBe(AltUriItem3.FullPath);
        item.ShouldNotBe(AltUriItem3);
    }

    [TestMethod]
    public void A02_ClearAndPull()
    {
        UriItem.Clear();
        UriItem.Exists.ShouldBeFalse();

    }
}
