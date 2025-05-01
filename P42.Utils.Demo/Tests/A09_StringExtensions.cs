using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.Tests;

[TestClass]
public class A09_StringExtensions
{
    const string text = "This is my text:\n  n•n•n = n³";

    [TestMethod]
    public void A00_UnicodeToHtmlEscapes()
    {
        var unicode = text.UnicodeToHtmlEscapes();
        unicode.ShouldNotBe(text);
    }

    [TestMethod]
    public void A01_RemoveWhiteSpace()
    {
        var spaceless = text.RemoveWhitespace();
        spaceless.ShouldNotBe(text);
        spaceless.ShouldBe("Thisismytext:n•n•n=n³");
    }

    [TestMethod]
    public void A02_IsNumeric()
    {
        text.IsNumeric().ShouldBeFalse();
        "3.45".IsNumeric().ShouldBeTrue();
    }

    [TestMethod]
    public void A03_RemoveLast()
    {
        var alt = text.RemoveLast();
        alt.ShouldBe(text[..^1]);

        alt = text.RemoveLast(18);
        alt.ShouldBe("This is my ");
        alt = alt.RemoveLast(20);
        alt.ShouldBe("");
    }

    [TestMethod]
    public void A04_SubstringLast()
    {
        var alt = text.SubstringLast(20);
        alt.ShouldBe(text[^20..]);

        alt = alt.SubstringLast(50);
        alt.ShouldBe(text[^20..]);
    }

    [TestMethod]
    public void A05_HasIllegalCharacter()
    {
        "`1234567890-=".HasIllegalCharacter().ShouldBeFalse();
        "~!@#$%^&*()_+".HasIllegalCharacter().ShouldBeFalse();
        "qwertyuiop\\".HasIllegalCharacter().ShouldBeFalse();
        "QWERTYUIOP|".HasIllegalCharacter().ShouldBeFalse();
        "asdfghjkl;'".HasIllegalCharacter().ShouldBeFalse();
        "ASDFGHJKL:\"".HasIllegalCharacter().ShouldBeFalse();
        "zxcvbnm,./".HasIllegalCharacter().ShouldBeFalse();
        "ZXCVBNM<>?".HasIllegalCharacter().ShouldBeFalse();

        "12{ab".HasIllegalCharacter().ShouldBeTrue();
        "34}cd".HasIllegalCharacter().ShouldBeTrue();
        "56[ef".HasIllegalCharacter().ShouldBeTrue();
        "78]gh".HasIllegalCharacter().ShouldBeTrue();
    }

    [TestMethod]
    public void A06_ReplaceIllegalCharacters()
    {
        "12{a}bc[D]h;`'".ReplaceIllegalCharacters().ShouldBe("12｛a｝bc［D］h;`'");
    }

    [TestMethod]
    public void A07_ReplaceSafeCharacters()
    {
        "12｛a｝bc［D］h;`'".ReplaceSafeCharacters().ShouldBe("12{a}bc[D]h;`'");
    }

    [TestMethod]
    public void A08_ToHex_char()
    {
        '0'.ToHex().ShouldBe<uint>(0);
        '1'.ToHex().ShouldBe<uint>(1);
        '2'.ToHex().ShouldBe<uint>(2);
        '3'.ToHex().ShouldBe<uint>(3);
        '4'.ToHex().ShouldBe<uint>(4);
        '5'.ToHex().ShouldBe<uint>(5);
        '6'.ToHex().ShouldBe<uint>(6);
        '7'.ToHex().ShouldBe<uint>(7);
        '8'.ToHex().ShouldBe<uint>(8);
        '9'.ToHex().ShouldBe<uint>(9);
        'A'.ToHex().ShouldBe<uint>(0xa);
        'B'.ToHex().ShouldBe<uint>(0xb);
        'C'.ToHex().ShouldBe<uint>(0xc);
        'D'.ToHex().ShouldBe<uint>(0xd);
        'E'.ToHex().ShouldBe<uint>(0xe);
        'F'.ToHex().ShouldBe<uint>(0xf);
        'a'.ToHex().ShouldBe<uint>(0xa);
        'b'.ToHex().ShouldBe<uint>(0xb);
        'c'.ToHex().ShouldBe<uint>(0xc);
        'd'.ToHex().ShouldBe<uint>(0xd);
        'e'.ToHex().ShouldBe<uint>(0xe);
        'f'.ToHex().ShouldBe<uint>(0xf);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => '/'.ToHex().ShouldBe<uint>(0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ':'.ToHex().ShouldBe<uint>(0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => '@'.ToHex().ShouldBe<uint>(0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => '['.ToHex().ShouldBe<uint>(0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => '`'.ToHex().ShouldBe<uint>(0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => '{'.ToHex().ShouldBe<uint>(0));
    }

    [TestMethod]
    public void A09_HumanReadableBytes()
    {
        StringExtensions.HumanReadableBytes(256).ShouldBe("256 Bi");
        StringExtensions.HumanReadableBytes(1024).ShouldBe("1 KBi");
        StringExtensions.HumanReadableBytes(1024, si: true).ShouldBe("1.02 KB");
        StringExtensions.HumanReadableBytes(1024, 3, true).ShouldBe("1.024 KB");
        StringExtensions.HumanReadableBytes(35543233).ShouldBe("33.9 MBi");
        StringExtensions.HumanReadableBytes(35543233, 4).ShouldBe("33.8967 MBi");
        StringExtensions.HumanReadableBytes(35543233, 4, true).ShouldBe("35.5432 MB");
    }


}
