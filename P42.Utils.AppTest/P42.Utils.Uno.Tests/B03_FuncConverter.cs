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
internal class B03_FuncConverter
{
    [TestMethod]
    public void A01_Convert()
    {
        var converter = new FuncConverter<int, string>(
                i => i.ToString(),
                s => int.Parse(s));

        var result = converter.Convert(55, typeof(string), null, string.Empty);
        result.ShouldBe("55");
    }

    [TestMethod]
    public void A02_ConvertBack()
    {
        var converter = new FuncConverter<int, string>(
                i => i.ToString(),
                s => int.Parse(s));

        var result = converter.ConvertBack("255", typeof(string), null, string.Empty);
        result.ShouldBe(255);
    }

    [TestMethod]
    public void A03_ConvertWithParam()
    {
        Func<int, int, string?> convert = (int source, int param) 
            => (source * param).ToString();
        
        Func<string?, int, int> convertBack = (string? dest, int param) 
            => int.Parse(dest ?? "0") / param;
        

        var converter = new FuncConverter<int, string, int>(convert, convertBack);

        var result = converter.Convert(100, typeof(string), 25, string.Empty);
        result.ShouldBe("2500");
    }

    [TestMethod]
    public void A04_ConvertBackWithParam()
    {
        Func<int, int, string?> convert = (int source, int param)
            => (source * param).ToString();

        Func<string?, int, int> convertBack = (string? dest, int param)
            => int.Parse(dest ?? "0") / param;


        var converter = new FuncConverter<int, string, int>(convert, convertBack);

        var result = converter.ConvertBack("2500", typeof(string), 25, string.Empty);
        result.ShouldBe(100);
    }

    [TestMethod]
    public void A05_ConvertWithParamLamda()
    {
        var converter = new FuncConverter<int, string, int>(
            (int source, int param) => (source * param).ToString(),
            (string? dest, int param) => int.Parse(dest ?? "0") / param);

        var result = converter.Convert(100, typeof(string), 25, string.Empty);
        result.ShouldBe("2500");
    }



}
