using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using Uno.Extensions;
using System.Collections.Specialized;
using Windows.UI.Xaml.Media;
using System.Linq;

namespace P42.Utils.Uno
{
    /// <summary>
    /// P42.Utils.Uno HTML markup string.
    /// </summary>
    [DesignTimeVisible(true)]
    class HtmlSpans : List<Span>
    {
        //internal readonly List<Span> _spans = new List<Span>();

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="P42.Utils.Uno.F9PFormattedString"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="P42.Utils.Uno.F9PFormattedString"/>.</returns>
        public override string ToString()
            => Text;
        

        internal bool ContainsActionSpan
        {
            get
            {
                foreach (var span in this)
                    if (span is HyperlinkSpan)
                        return true;
                return false;
            }
        }

        string _text = string.Empty;
        /// <summary>
        /// Gets or sets source text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                ProcessHTML();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlSpans"/> class.
        /// </summary>
        public HtmlSpans()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlSpans"/> class.
        /// </summary>
        /// <param name="s">S.</param>
        public HtmlSpans(string s) : this()
        {
            Text = s;
        }


        /// <param name="formatted">Formatted.</param>
        public static explicit operator string(HtmlSpans formatted)
        {
            return formatted?.Text;
        }

        class Attribute
        {
            public string Name;
            public string Value;
        }

        class Tag
        {
            public string Name;
            public int Start;
            public List<Attribute> Attributes = new List<Attribute> { Capacity = 10 };
        }

        readonly StringBuilder _unmarkedText = new StringBuilder();
        /// <summary>
        /// Gets the unmarked text.
        /// </summary>
        /// <value>The unmarked text.</value>
        public string UnmarkedText => _unmarkedText.ToString();

        static Dictionary<string, int> xmlEscapes = new Dictionary<string, int>
        {
            { "quot", 0x0022},
            { "amp", 0x0026},
            { "apos", 0x0027},
            { "lt", 0x003C},
            { "gt", 0x003E},
            { "nbsp", 0x00A0},
            { "iexcl", 0x00A1},
            { "cent", 0x00A2},
            { "pound", 0x00A3},
            { "curren", 0x00A4},
            { "yen", 0x00A5},
            { "brvbar", 0x00A6},
            { "sect", 0x00A7},
            { "uml", 0x00A8},
            { "copy", 0x00A9},
            { "ordf", 0x00AA},
            { "laquo", 0x00AB},
            { "not", 0x00AC},
            { "shy", 0x00AD},
            { "reg", 0x00AE},
            { "macr", 0x00AF},
            { "deg", 0x00B0},
            { "plusmn", 0x00B1},
            { "sup2", 0x00B2},
            { "sup3", 0x00B3},
            { "acute", 0x00B4},
            { "micro", 0x00B5},
            { "para", 0x00B6},
            { "middot", 0x00B7},
            { "cedil", 0x00B8},
            { "sup1", 0x00B9},
            { "ordm", 0x00BA},
            { "raquo", 0x00BB},
            { "frac14", 0x00BC},
            { "frac12", 0x00BD},
            { "frac34", 0x00BE},
            { "iquest", 0x00BF},
            { "Agrave", 0x00C0},
            { "Aacute", 0x00C1},
            { "Acirc", 0x00C2},
            { "Atilde", 0x00C3},
            { "Auml", 0x00C4},
            { "Aring", 0x00C5},
            { "AElig", 0x00C6},
            { "Ccedil", 0x00C7},
            { "Egrave", 0x00C8},
            { "Eacute", 0x00C9},
            { "Ecirc", 0x00CA},
            { "Euml", 0x00CB},
            { "Igrave", 0x00CC},
            { "Iacute", 0x00CD},
            { "Icirc", 0x00CE},
            { "Iuml", 0x00CF},
            { "ETH", 0x00D0},
            { "Ntilde", 0x00D1},
            { "Ograve", 0x00D2},
            { "Oacute", 0x00D3},
            { "Ocirc", 0x00D4},
            { "Otilde", 0x00D5},
            { "Ouml", 0x00D6},
            { "times", 0x00D7},
            { "Oslash", 0x00D8},
            { "Ugrave", 0x00D9},
            { "Uacute", 0x00DA},
            { "Ucirc", 0x00DB},
            { "Uuml", 0x00DC},
            { "Yacute", 0x00DD},
            { "THORN", 0x00DE},
            { "szlig", 0x00DF},
            { "agrave", 0x00E0},
            { "aacute", 0x00E1},
            { "acirc", 0x00E2},
            { "atilde", 0x00E3},
            { "auml", 0x00E4},
            { "aring", 0x00E5},
            { "aelig", 0x00E6},
            { "ccedil", 0x00E7},
            { "egrave", 0x00E8},
            { "eacute", 0x00E9},
            { "ecirc", 0x00EA},
            { "euml", 0x00EB},
            { "igrave", 0x00EC},
            { "iacute", 0x00ED},
            { "icirc", 0x00EE},
            { "iuml", 0x00EF},
            { "eth", 0x00F0},
            { "ntilde", 0x00F1},
            { "ograve", 0x00F2},
            { "oacute", 0x00F3},
            { "ocirc", 0x00F4},
            { "otilde", 0x00F5},
            { "ouml", 0x00F6},
            { "divide", 0x00F7},
            { "oslash", 0x00F8},
            { "ugrave", 0x00F9},
            { "uacute", 0x00FA},
            { "ucirc", 0x00FB},
            { "uuml", 0x00FC},
            { "yacute", 0x00FD},
            { "thorn", 0x00FE},
            { "yuml", 0x00FF},
            { "OElig", 0x0152},
            { "oelig", 0x0153},
            { "Scaron", 0x0160},
            { "scaron", 0x0161},
            { "Yuml", 0x0178},
            { "fnof", 0x0192},
            { "circ", 0x02C6},
            { "tilde", 0x02DC},
            { "Alpha", 0x0391},
            { "Beta", 0x0392},
            { "Gamma", 0x0393},
            { "Delta", 0x0394},
            { "Epsilon", 0x0395},
            { "Zeta", 0x0396},
            { "Eta", 0x0397},
            { "Theta", 0x0398},
            { "Iota", 0x0399},
            { "Kappa", 0x039A},
            { "Lambda", 0x039B},
            { "Mu", 0x039C},
            { "Nu", 0x039D},
            { "Xi", 0x039E},
            { "Omicron", 0x039F},
            { "Pi", 0x03A0},
            { "Rho", 0x03A1},
            { "Sigma", 0x03A3},
            { "Tau", 0x03A4},
            { "Upsilon", 0x03A5},
            { "Phi", 0x03A6},
            { "Chi", 0x03A7},
            { "Psi", 0x03A8},
            { "Omega", 0x03A9},
            { "alpha", 0x03B1},
            { "beta", 0x03B2},
            { "gamma", 0x03B3},
            { "delta", 0x03B4},
            { "epsilon", 0x03B5},
            { "zeta", 0x03B6},
            { "eta", 0x03B7},
            { "theta", 0x03B8},
            { "iota", 0x03B9},
            { "kappa", 0x03BA},
            { "lambda", 0x03BB},
            { "mu", 0x03BC},
            { "nu", 0x03BD},
            { "xi", 0x03BE},
            { "omicron", 0x03BF},
            { "pi", 0x03C0},
            { "rho", 0x03C1},
            { "sigmaf", 0x03C2},
            { "sigma", 0x03C3},
            { "tau", 0x03C4},
            { "upsilon", 0x03C5},
            { "phi", 0x03C6},
            { "chi", 0x03C7},
            { "psi", 0x03C8},
            { "omega", 0x03C9},
            { "thetasym", 0x03D1},
            { "upsih", 0x03D2},
            { "piv", 0x03D6},
            { "ensp", 0x2002},
            { "emsp", 0x2003},
            { "thinsp", 0x2009},
            { "zwnj", 0x200C},
            { "zwj", 0x200D},
            { "lrm", 0x200E},
            { "rlm", 0x200F},
            { "ndash", 0x2013},
            { "mdash", 0x2014},
            { "lsquo", 0x2018},
            { "rsquo", 0x2019},
            { "sbquo", 0x201A},
            { "ldquo", 0x201C},
            { "rdquo", 0x201D},
            { "bdquo", 0x201E},
            { "dagger", 0x2020},
            { "Dagger", 0x2021},
            { "bull", 0x2022},
            { "hellip", 0x2026},
            { "permil", 0x2030},
            { "prime", 0x2032},
            { "Prime", 0x2033},
            { "lsaquo", 0x2039},
            { "rsaquo", 0x203A},
            { "oline", 0x203E},
            { "frasl", 0x2044},
            { "euro", 0x20AC},
            { "image", 0x2111},
            { "weierp", 0x2118},
            { "real", 0x211C},
            { "trade", 0x2122},
            { "alefsym", 0x2135},
            { "larr", 0x2190},
            { "uarr", 0x2191},
            { "rarr", 0x2192},
            { "darr", 0x2193},
            { "harr", 0x2194},
            { "crarr", 0x21B5},
            { "lArr", 0x21D0},
            { "uArr", 0x21D1},
            { "rArr", 0x21D2},
            { "dArr", 0x21D3},
            { "hArr", 0x21D4},
            { "forall", 0x2200},
            { "part", 0x2202},
            { "exist", 0x2203},
            { "empty", 0x2205},
            { "nabla", 0x2207},
            { "isin", 0x2208},
            { "notin", 0x2209},
            { "ni", 0x220B},
            { "prod", 0x220F},
            { "sum", 0x2211},
            { "minus", 0x2212},
            { "lowast", 0x2217},
            { "radic", 0x221A},
            { "prop", 0x221D},
            { "infin", 0x221E},
            { "ang", 0x2220},
            { "and", 0x2227},
            { "or", 0x2228},
            { "cap", 0x2229},
            { "cup", 0x222A},
            { "int", 0x222B},
            { "there4", 0x2234},
            { "sim", 0x223C},
            { "cong", 0x2245},
            { "asymp", 0x2248},
            { "ne", 0x2260},
            { "equiv", 0x2261},
            { "le", 0x2264},
            { "ge", 0x2265},
            { "sub", 0x2282},
            { "sup", 0x2283},
            { "nsub", 0x2284},
            { "sube", 0x2286},
            { "supe", 0x2287},
            { "oplus", 0x2295},
            { "otimes", 0x2297},
            { "perp", 0x22A5},
            { "sdot", 0x22C5},
            { "lceil", 0x2308},
            { "rceil", 0x2309},
            { "lfloor", 0x230A},
            { "rfloor", 0x230B},
            { "lang", 0x2329},
            { "rang", 0x232A},
            { "loz", 0x25CA},
            { "spades", 0x2660},
            { "clubs", 0x2663},
            { "hearts", 0x2665},
            { "diams", 0x2666},
        };

        bool inPreSpan;
        void ProcessHTML()
        {
            _unmarkedText.Clear();
            if (string.IsNullOrWhiteSpace(_text))
                return;
            // remove previously Translated spans
            Clear();

            var tags = new List<Tag>();
            var index = 0;

            for (int i = 0; i < _text.Length; i++)
            {
                var leap = 0;
                string tagText = null;
                char c0 = _text[i];
                if (c0 == '<' && i + 2 < _text.Length)
                {
                    char c1 = _text[i + 1];
                    if (c1 == '_' || c1 == '/' || char.IsLetter(c1))
                    {
                        var restText = _text.Substring(i + 1);
                        var nextClose = restText.IndexOf('>');
                        if (nextClose > 0)
                        {
                            var nextOpen = restText.IndexOf('<');
                            if (nextOpen < 0 || nextOpen > nextClose)
                            {
                                tagText = restText.Substring(0, nextClose);
                                leap = nextClose + 1;
                            }

                        }

                    }
                }

                int unicodeInt = 0;
                if (string.IsNullOrWhiteSpace(tagText))
                {
                    if (c0 == '&' && i + 3 < _text.Length)
                    {
                        var restText = _text.Substring(i + 1);
                        var nextClose = restText.IndexOf(';');
                        if (nextClose > 0)
                        {
                            var nextOpen = restText.IndexOf('&');
                            if (nextOpen < 0 || nextOpen > nextClose)
                            {
                                var escText = restText.Substring(0, nextClose);
                                leap = nextClose + 1;

                                if (escText[1] == '#')
                                {
                                    var start = 2;
                                    var numBase = 10;
                                    if (char.ToLower(escText[2]) == 'x')
                                    {
                                        start = 3;
                                        numBase = 16;
                                    }
                                    unicodeInt = Convert.ToInt32(escText.Substring(start), numBase);
                                }
                                else if (xmlEscapes.TryGetValue(escText, out int value))
                                {
                                    unicodeInt = value;
                                }
                                
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(tagText))  
                {
                    if (tagText[0] == '/')
                    {
                        tagText = tagText.Substring(1).ToLower();
                        if (tags.LastOrDefault((Tag obj) => obj.Name == tagText) is Tag tag)
                        {
                            tags.Remove(tag);
                            ProcessTag(tag, index);
                        }
                    }
                    else if (tagText == "br/" || tagText == "br")
                    {
                        index++;
                        _unmarkedText.Append("\n");
                    }
                    else
                    {
                        var pattern = @" ([^>=]*)=[""']([^""']*)[""']";

                        var tagName = tagText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
                        var tag = new Tag { Name = tagName, Start = index };
                        foreach (Match m in Regex.Matches(tagText, pattern))
                        {
                            if (m.Groups.Count != 3)
                                throw new FormatException("Cannot parse attributes for tag [" + tagText + "]");
                            var attrName = m.Groups[1].Value;
                            var attrValue = m.Groups[2].Value;
                            var attribute = new Attribute { Name = attrName, Value = attrValue };
                            tag.Attributes.Add(attribute);
                        }
                        tags.Add(tag);
                        inPreSpan = inPreSpan || tagName == "pre";
                    }
                    i += leap;
                }
                else if (unicodeInt != 0)
                {
                    var unicodeString = char.ConvertFromUtf32(unicodeInt);
                    index += unicodeString.Length;
                    _unmarkedText.Append(unicodeString);
                    i += leap;
                }
                else  if (inPreSpan || !(char.IsWhiteSpace(c0) && i > 0 && char.IsWhiteSpace(_text, i - 1)))
                {
                    index++;
                    _unmarkedText.Append(_text[i]);
                }

            }

            if (tags.Count > 0)
                foreach (var tag in tags)
                    ProcessTag(tag, index);
        }

        void ProcessTag(Tag tag, int index)
        {
            if (tag.Start >= index)
                return;
            float size;
            inPreSpan &= tag.Name != "pre";
            Span span;
            switch (tag.Name)
            {
                case "strong":
                case "b":
                    span = new BoldSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "em":
                case "i":
                    span = new ItalicsSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "sub":
                    span = new SubscriptSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "sup":
                    span = new SuperscriptSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "num":
                    span = new NumeratorSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "den":
                    span = new DenominatorSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "u":
                case "ins":
                    span = new UnderlineSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "font":
                    foreach (var attr in tag.Attributes)
                    {
                        switch (attr.Name)
                        {
                            case "color":
                                span = new FontColorSpan(tag.Start, index - 1, attr.Value.ColorFromString());
                                Add(span);
                                break;
#pragma warning disable CC0021 // Use nameof
                            case "size":
#pragma warning restore CC0021 // Use nameof
                                size = (float)ToFontSize(attr.Value);
                                span = new FontSizeSpan(tag.Start, index - 1, size);
                                Add(span);
                                break;
                            case "face":
                                span = new FontFamilySpan(tag.Start, index - 1, FontExtensions.GetFontFamily(attr.Value));
                                Add(span);
                                break;
                        }
                    }
                    break;
                case "pre": // preformatted text
                case "tt": // teletype (monospaced)
                    span = new FontFamilySpan(tag.Start, index - 1, new FontFamily("Lucida Console"));
                    Add(span);
                    break;
                case "strike":
                case "s": // strikethrough 
                case "del":
                    span = new StrikethroughSpan(tag.Start, index - 1);
                    Add(span);
                    break;
                case "big":
                    size = (float)ToFontSize("x-large");
                    span = new FontSizeSpan(tag.Start, index - 1, size);
                    Add(span);
                    break;
                case "small":
                    size = (float)ToFontSize("x-small");
                    span = new FontSizeSpan(tag.Start, index - 1, size);
                    Add(span);
                    break;
                case "a":
                    string href = null;
                    foreach (var attr in tag.Attributes)
                    {
                        switch (attr.Name)
                        {
                            case nameof(href):
                                href = attr.Value;
                                break;
                                /*
                            case nameof(id):
                                id = attr.Value;
                                break;
                                */
                        }
                    }
                    span = new HyperlinkSpan(tag.Start, index - 1, href);
                    Add(span);
                    break;
            }
            // process  attributes
            foreach (var attr in tag.Attributes)
            {
                if (attr.Name == "style")
                {
                    var styleAttrs = attr.Value.Split(';');
                    foreach (var styleAttr in styleAttrs)
                    {
                        var strs = styleAttr.Split(':');
                        if (strs.Length == 2)
                        {
                            switch (strs[0].ToLower())
                            {
                                case "background-color":
                                    span = new BackgroundColorSpan(tag.Start, index - 1, strs[1].ColorFromString());
                                    Add(span);
                                    break;
                                case "color":
                                    span = new FontColorSpan(tag.Start, index - 1, strs[1].ColorFromString());
                                    Add(span);
                                    break;
                                case "font-family":
                                    span = new FontFamilySpan(tag.Start, index - 1, new FontFamily(strs[1]));
                                    Add(span);
                                    break;
                                case "font-size":
                                    size = (float)ToFontSize(strs[1]);
                                    span = new FontSizeSpan(tag.Start, index - 1, size);
                                    Add(span);
                                    break;
                                case "font-weight":
                                    if (strs[1].ToLower() == "bold")
                                    {
                                        span = new BoldSpan(tag.Start, index - 1);
                                        Add(span);
                                    }
                                    else
                                    {
                                        throw new FormatException("style=\"font-Weight: " + strs[1] + ";\" not supported");
                                    }
                                    break;
                                case "font-style":
                                    if (strs[1].ToLower() == "italic")
                                    {
                                        span = new ItalicsSpan(tag.Start, index - 1);
                                        Add(span);
                                    }
                                    else
                                    {
                                        throw new FormatException("style=\"font-style: " + strs[1] + ";\" not supported");
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        static double ToFontSize(string sizeString)
        {
            var s = sizeString;
            double size;
            //var element = new Xamarin.Forms.Label();
            if (s.EndsWith("px", StringComparison.Ordinal))
            {
                var subString = s.Substring(0, s.Length - 2);
                if (double.TryParse(subString, NumberStyles.Float, CultureInfo.InvariantCulture, out size))
                    return size;
                throw new FormatException("Cannot parse [" + s + "][" + subString + "] [" + size + "] px font size");
            }
            if (s.EndsWith("em", StringComparison.Ordinal))
            {
                var subString = s.Substring(0, s.Length - 2);
                if (double.TryParse(subString, NumberStyles.Float, CultureInfo.InvariantCulture, out size))
                    return -size;
                throw new FormatException("Cannot parse [" + s + "][" + subString + "] [" + size + "] em font size");
            }
            if (s.EndsWith("%", StringComparison.Ordinal))
            {
                var subString = s.Substring(0, s.Length - 1);
                if (double.TryParse(subString, NumberStyles.Float, CultureInfo.InvariantCulture, out size))
                    return -size / 100.0;
                throw new FormatException("Cannot parse [" + s + "][" + subString + "] [" + size + "] % font size");
            }
            switch (s.ToLower())
            {
                case "xx-small":
                    //return Device.GetNamedSize (NamedSize.Micro, element) - (Device.GetNamedSize (NamedSize.Small, element) - Device.GetNamedSize (NamedSize.Micro, element)) ;
                    return -10.0 / 17.0;
                case "x-small":
                case "1":
                case "-2":
                    //return Device.GetNamedSize (NamedSize.Micro, element);
                    return -12.0 / 17.0;
                case "small":
                case "2":
                case "-1":
                    //return Device.GetNamedSize (NamedSize.Small, element);
                    return -14.0 / 17.0;
                case "medium":
                case "3":
                    //return Device.GetNamedSize (NamedSize.Medium, element);
                    return -1.0;
                case "large":
                case "4":
                case "+1":
                    //return Device.GetNamedSize (NamedSize.Large, element);
                    return -22.0 / 17.0;
                case "x-large":
                case "5":
                case "+2":
                    //return Device.GetNamedSize (NamedSize.Large, element) + (Device.GetNamedSize (NamedSize.Large, element) - Device.GetNamedSize (NamedSize.Medium, element));
                    return -27.0 / 17.0;
                case "xx-large":
                case "6":
                case "+3":
                    //return Device.GetNamedSize (NamedSize.Large, element) + (Device.GetNamedSize (NamedSize.Large, element) - Device.GetNamedSize (NamedSize.Medium, element)) * 2.0;
                    return -32.0 / 17.0;
                case "7":
                case "+4":
                    //return Device.GetNamedSize (NamedSize.Large, element) + (Device.GetNamedSize (NamedSize.Large, element) - Device.GetNamedSize (NamedSize.Medium, element)) * 3.0;
                    return -37.0 / 17.0;
                case "initial":
                    return -1;
            }
            /*
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out size))
            {
                if (size < 1)
                    return Device.GetNamedSize(NamedSize.Micro, element);
                if (size > 7)
                    return Device.GetNamedSize(NamedSize.Large, element) + (Device.GetNamedSize(NamedSize.Large, element) - Device.GetNamedSize(NamedSize.Medium, element)) * 3.0;
            }
            */
            return 0;
        }

    }
}

