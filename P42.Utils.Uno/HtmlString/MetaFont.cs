using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    class MetaFont 
    {
        #region Properties

        public FontFamily Family { get; set; }

        public double Size { get; set; }

        public bool Italic { get; set; }

        public bool Bold { get; set; }

        public FontBaseline Baseline { get; set; }

        public MetaFontAction Action { get; set; }

        public Color TextColor { get; set; }

        public Color BackgroundColor { get; set; }

        public bool Underline { get; set; }

        public bool Strikethrough { get; set; }

        #endregion


        #region Constrction
        public MetaFont(FontFamily family, double size, bool bold = false, bool italic = false)
        {
            Baseline = FontBaseline.Normal;
            Family = family;
            Size = size;
            Bold = bold;
            Italic = italic;
        }

        public MetaFont(MetaFont f) : this (f.Family, f.Size, f.Bold, f.Italic)
        {
            if (f is null)
                return;
            Baseline = f.Baseline;
            Action = f.Action?.Copy();
            BackgroundColor = f.BackgroundColor;
            TextColor = f.TextColor;
            Underline = f.Underline;
            Strikethrough = f.Strikethrough;
        }
        public MetaFont(FontFamily family, double size, bool bold = false, bool italic = false, string id = null, string href = null, Color textColor = default, Color backgroundColor = default, bool underline = false, bool strikethrough = false) : this(family, size, bold, italic)
        {
            if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(href))
                Action = new MetaFontAction(href, id);
            TextColor = textColor;
            BackgroundColor = backgroundColor;
            Underline = underline;
            Strikethrough = strikethrough;
        }

        #endregion


        #region Equality
        public static bool operator ==(MetaFont f1, MetaFont f2) => f1.Equals(f2);

        public static bool operator !=(MetaFont f1, MetaFont f2) => !f1.Equals(f2);

        public override bool Equals(object obj)
        {
            if (obj is MetaFont other)
            {
                if (Size != other.Size)
                    return false;
                if (Bold != other.Bold)
                    return false;
                if (Italic != other.Italic)
                    return false;
                if (Family != other.Family)
                    return false;
                if (Baseline != other.Baseline)
                    return false;
                if (Action != other.Action)
                    return false;
                if (TextColor != other.TextColor)
                    return false;
                if (BackgroundColor != other.BackgroundColor)
                    return false;
                if (Underline != other.Underline)
                    return false;
                if (Strikethrough != other.Strikethrough)
                    return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + Family.GetHashCode();
            hash = hash * 23 + Size.GetHashCode();
            hash = hash * 23 + Bold.GetHashCode();
            hash = hash * 23 + Italic.GetHashCode();
            hash = hash * 23 + Baseline.GetHashCode();
            if (Action != null)
                hash = hash * 23 + Action.GetHashCode();
            hash = hash * 23 + TextColor.GetHashCode();
            hash = hash * 23 + BackgroundColor.GetHashCode();
            hash = hash * 23 + Underline.GetHashCode();
            hash = hash * 23 + Strikethrough.GetHashCode();
            return hash;
        }
        #endregion


        public bool IsActiveAction
            => Action != null && !Action.IsEmpty();
    }

    class MetaFontAction
    {
        public string Id { get; set; }

        public string Href { get; set; }

        public MetaFontAction(string href, string id)
        {
            Id = id;
            Href = href;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Href);
        }

        public MetaFontAction Copy()
        {
            if (IsEmpty())
                return null;
            return new MetaFontAction(Href, Id);
        }

        public static bool operator ==(MetaFontAction a1, MetaFontAction a2) => Equals(a1, a2);

        public static bool operator !=(MetaFontAction a1, MetaFontAction a2) => !Equals(a1, a2);

        public static bool Equals(MetaFontAction a1, MetaFontAction a2)
        {
            if (a1?.Id != a2?.Id)
                return false;
            if (a1?.Href != a2?.Href)
                return false;
            
            return true;
        }

        public override bool Equals(object obj) => Equals(this, obj as MetaFontAction);
        

        public override int GetHashCode()
        {
            var hash = 7;
            hash = hash * 17 + Id.GetHashCode();
            hash = hash * 17 + Href.GetHashCode();
            return hash;
        }

        public MetaFontAction(HyperlinkSpan actionSpan)
        {
            Id = actionSpan.Id;
            Href = actionSpan.Href;
        }
    }
}
