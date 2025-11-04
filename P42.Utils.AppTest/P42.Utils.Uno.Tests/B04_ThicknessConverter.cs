using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
[SuppressMessage("Usage", "MSTEST0003:Test methods should have valid layout")]
// ReSharper disable once InconsistentNaming
#pragma warning disable MSTEST0002
internal class B04_ThicknessConverter
#pragma warning restore MSTEST0002
{
    [TestMethod]
    public void A01_ConvertString()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert("1,2,3,4", typeof(Thickness), string.Empty, string.Empty);
        result.ShouldBe(new Thickness(1,2, 3, 4));
    }

    [TestMethod]
    public void A02_ConvertBackString()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.ConvertBack(new Thickness(4,3,2,1), typeof(string), string.Empty, string.Empty);
#if WINDOWS
        result.ShouldBe("4,3,2,1");
#else
        result.ShouldBe("[Thickness: 4-3-2-1]");
#endif
    }

    [TestMethod]
    public void A03_ConvertDouble()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert(1.234, typeof(Thickness), string.Empty, string.Empty);
        result.ShouldBe(new Thickness(1.234));
    }

    [TestMethod]
    public void A04_ConvertBackDouble()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.ConvertBack(new Thickness(4, 3, 2, 1), typeof(double), string.Empty, string.Empty);
        result.ShouldBe(2.5);
    }

    [TestMethod]
    public void A05_ConvertTrue()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert(true, typeof(Thickness), new Thickness(10), string.Empty);
        result.ShouldBe(new Thickness(10));
    }

    [TestMethod]
    public void A06_ConvertFalse()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert(false, typeof(Thickness), new Thickness(10), string.Empty);
        result.ShouldBe(new Thickness(0));
    }

    [TestMethod]
    public void A07_ConvertBackBoolTrue()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.ConvertBack(new Thickness(4, 3, 2, 0), typeof(bool), new Thickness(4,3,2,1), string.Empty);
        result.ShouldBe(true);
    }

    [TestMethod]
    public void A08_ConvertBackBoolFalse()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.ConvertBack(new Thickness(4, 3, 2, 1), typeof(bool), new Thickness(4, 3, 2, 1), string.Empty);
        result.ShouldBe(false);
    }

    [TestMethod]
    public void A09_ConvertInt()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert(1234, typeof(Thickness), string.Empty, string.Empty);
        result.ShouldBe(new Thickness(1234));
    }

    [TestMethod]
    public void A10_ConvertBackInt()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.ConvertBack(new Thickness(4, 2, 4, 2), typeof(int), string.Empty, string.Empty);
        result.ShouldBe(3);
    }

    [TestMethod]
    public void A11_ConvertSystemSize()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert(new System.Drawing.Size(10,20), typeof(Thickness), string.Empty, string.Empty);
        result.ShouldBe(new Thickness(10,20,10,20));
    }

    [TestMethod]
    public void A12_ConvertBackSystemSize()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.ConvertBack(new Thickness(10,20,10,20), typeof(System.Drawing.Size), string.Empty, string.Empty);
        result.ShouldBe(new System.Drawing.Size(10, 20));
    }

    [TestMethod]
    public void A13_ConvertWinSize()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert(new Windows.Foundation.Size(10, 20), typeof(Thickness), string.Empty, string.Empty);
        result.ShouldBe(new Thickness(10, 20, 10, 20));
    }

    [TestMethod]
    public void A14_ConvertBackWinSize()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.ConvertBack(new Thickness(10, 20, 10, 20), typeof(Windows.Foundation.Size), string.Empty, string.Empty);
        result.ShouldBe(new Windows.Foundation.Size(10, 20));
    }

    [TestMethod]
    public void A15_ConvertUnknown()
    {
        var converter = Uno.ThicknessConverter.Instance;
        var result = converter.Convert(new List<int>(), typeof(Thickness), new Thickness(50), string.Empty);
        result.ShouldBe(new Thickness(50));
    }

    [TestMethod]
    public void A16_ConvertBackUnknown()
    {
        var converter = Uno.ThicknessConverter.Instance;
        Assert.Throws<ArgumentException>(() => converter.ConvertBack(new Thickness(80), typeof(List<int>), new Thickness(50), string.Empty)
        , "Cannot convert Thickness:[80, 80, 80, 80] to List<int>");
    }

}
