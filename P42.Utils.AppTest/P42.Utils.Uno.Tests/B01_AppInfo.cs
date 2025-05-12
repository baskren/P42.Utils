using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.Uno.AppTest;

[TestClass]
public class B01_AppInfo
{
    [TestMethod]
    public void A01_Identifier()
    {
        AppInfo.Identifier.ShouldBe("com.p42.utils.apptest");
   }


    [TestMethod]
    public void A02_Name()
        => AppInfo.Name.ShouldBe("P42.Utils.AppTest");

    [TestMethod]
    public void A03_Version()
        => AppInfo.Version.ShouldBe("1.2");

    [TestMethod]
    public void A04_Build()
        => AppInfo.Build.ShouldBe(3);
}
