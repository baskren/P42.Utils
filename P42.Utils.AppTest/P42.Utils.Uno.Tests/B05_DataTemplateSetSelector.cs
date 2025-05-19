using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P42.Utils.Uno;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
internal class B05_DataTemplateSetSelector
{
    P42.Utils.Uno.DataTemplateSetSelector Selector;

    [TestInitialize]
    public void A00_Initialize()
    {
        Selector = new Uno.DataTemplateSetSelector()
            .Add<int, Button>()
            .Add<double, TextBlock>();
        Selector.Count.ShouldBe(2);
        Selector.NullTemplateSet.ShouldBeOfType<DefaultNullDataTemplateSet>();
        Assert.IsNull(Selector.NoMatchTemplateSet);
    }

    [TestMethod]
    public void A01_Null()
    {
        Selector.GetUIElement(null).ShouldBeOfType<NullView>();
    }

    [TestMethod]
    public void A02_Int()
    {
        Selector.GetUIElement(10).ShouldBeOfType<Button>();
    }

    [TestMethod]
    public void A03_Double()
    {
        Selector.GetUIElement(10.1).ShouldBeOfType<TextBlock>();
    }

    [TestMethod]
    public void A04_NoMatch()
    {
        Assert.IsNull(Selector.GetUIElement("pizza"));
    }
}
