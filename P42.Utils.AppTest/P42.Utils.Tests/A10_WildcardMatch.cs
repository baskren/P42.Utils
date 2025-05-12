using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class A10_WildcardMatch
{

    [TestMethod]
    public void A00_EqualsWildcard()
    {
        "This is a test".EqualsWildcard("This * test").ShouldBeTrue();
        "That is a test".EqualsWildcard("This * test").ShouldBeFalse();
        "This is a test".EqualsWildcard("This*").ShouldBeTrue();
        "This is a test".EqualsWildcard("*test").ShouldBeTrue();
        "This is a test".EqualsWildcard("*").ShouldBeTrue();
        "This is a test".EqualsWildcard("This is ? test").ShouldBeTrue();
        "This is  test".EqualsWildcard("This is ? test").ShouldBeFalse();
    }

    [TestMethod]
    public void A00_EqualsWildcard_IgnoreCase()
    {
        "THIS is a TEST".EqualsWildcard("This * test", true).ShouldBeTrue();
        "THAT is a TEST".EqualsWildcard("This * test").ShouldBeFalse();
        "THIS is a TEST".EqualsWildcard("This*", true).ShouldBeTrue();
        "THIS is a TEST".EqualsWildcard("*test", true).ShouldBeTrue();
        "THIS is a TEST".EqualsWildcard("*", true).ShouldBeTrue();
        "THIS is a TEST".EqualsWildcard("This is ? test", true).ShouldBeTrue();
        "THIS is  TEST".EqualsWildcard("This is ? test").ShouldBeFalse();
    }

}
