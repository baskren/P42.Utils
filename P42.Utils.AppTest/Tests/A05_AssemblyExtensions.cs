using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class A05_AssemblyExtensions
{
    [TestMethod]
    public void A00_Init()
    {
        var asm = Assembly.GetExecutingAssembly();
        asm.ShouldBe(GetType().Assembly);
    }

    [TestMethod]
    public void A01_Name()
    {
        var asm = Assembly.GetExecutingAssembly();
        asm.Name().ShouldBe("P42.Utils.AppTest");
    }

    [TestMethod]
    public void A02_GetAssemblies()
    {
        var asm = Assembly.GetExecutingAssembly();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        assemblies.ShouldContain(asm);
        assemblies.ShouldContain(typeof(P42.Utils.LocalData).Assembly);
        assemblies.ShouldContain(typeof(Microsoft.UI.Xaml.Application).Assembly);
#if HAS_UNO
        assemblies.ShouldContain(typeof(global::Uno.UI.Toolkit.UIElementExtensions).Assembly);
#endif
    }

    [TestMethod]
    public void A03_GetAssemblyByName()
    {
        var asm = Assembly.GetExecutingAssembly();
        var assembly = AssemblyExtensions.GetAssemblyByName("P42.Utils.AppTest");
        assembly.ShouldBe(asm);
    }

    [TestMethod]
    public void A04_GetBuildTime()
    {
        var asm = Assembly.GetExecutingAssembly();

#if BROWSERWASM
        asm.TryGetBuildTime(out var time).ShouldBeFalse();
#else
        asm.TryGetBuildTime(out var time).ShouldBeTrue();
#endif
        time.ShouldNotBe(DateTime.MinValue);
        time.ShouldNotBe(DateTime.MaxValue);
        time.ShouldNotBe(DateTime.Now);
        time.ShouldNotBe(DateTime.Today);
        time.ShouldNotBe(DateTime.UtcNow);
    }
}
