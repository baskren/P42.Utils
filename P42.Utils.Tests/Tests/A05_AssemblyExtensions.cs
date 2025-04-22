using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.Tests;

[TestClass]
public class A05_AssemblyExtensions
{
    Assembly? Assembly { get; set; }

    [TestMethod]
    public void A00_Init()
    {
        Assembly = Assembly.GetExecutingAssembly();
        Assembly.ShouldBe(GetType().Assembly);
    }

    [TestMethod]
    public void A01_Name()
    {
        Assembly.Name().ShouldBe("P42.Utils.Tests");
    }

    [TestMethod]
    public void A02_GetAssemblies()
    {
        var assemblies = AssemblyExtensions.GetAssemblies();
        assemblies.ShouldContain(Assembly);
        assemblies.ShouldContain(typeof(P42.Utils.LocalData).Assembly);
        assemblies.ShouldContain(typeof(Microsoft.UI.Xaml.Application).Assembly);
#if HAS_UNO
        assemblies.ShouldContain(typeof(global::Uno.UI.Toolkit.UIElementExtensions).Assembly);
#endif
    }

    [TestMethod]
    public void A03_GetAssemblyByName()
    {
        var assembly = AssemblyExtensions.GetAssemblyByName("P42.Utils.Tests");
        assembly.ShouldBe(Assembly);
    }

    [TestMethod]
    public void A04_GetBuildTime()
    {

#if BROWSERWASM
        Assembly.TryGetBuildTime(out var time).ShouldBeFalse();
#else
        Assembly.TryGetBuildTime(out var time).ShouldBeTrue();
#endif
        time.ShouldNotBe(DateTime.MinValue);
        time.ShouldNotBe(DateTime.MaxValue);
        time.ShouldNotBe(DateTime.Now);
        time.ShouldNotBe(DateTime.Today);
        time.ShouldNotBe(DateTime.UtcNow);
    }
}
