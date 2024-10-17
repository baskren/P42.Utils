using System;
using Windows.UI;

namespace P42.Utils.Uno;

internal class MetaFont(Microsoft.UI.Xaml.Media.FontFamily family, double size, bool bold = false, bool italic = false)
{
    #region Properties

    public Microsoft.UI.Xaml.Media.FontFamily Family { get; } = family;

    public double Size { get; set; } = size;

    public bool Italic { get; set; } = italic;

    public bool Bold { get; set; } = bold;

    public FontBaseline Baseline { get; set; } = FontBaseline.Normal;

    public MetaFontAction? Action { get; set; }

    public Color TextColor { get; set; }

    public Color BackgroundColor { get; set; }

    public bool Underline { get; set; }

    public bool Strikethrough { get; set; }

    #endregion


    #region Construction

    public MetaFont(MetaFont f) : this (f.Family, f.Size, f.Bold, f.Italic)
    {
        Baseline = f.Baseline;
        Action = f.Action?.Copy();
        BackgroundColor = f.BackgroundColor;
        TextColor = f.TextColor;
        Underline = f.Underline;
        Strikethrough = f.Strikethrough;
    }
    public MetaFont(Microsoft.UI.Xaml.Media.FontFamily family, double size, bool bold = false, bool italic = false, string? id = null, string? href = null, Color textColor = default, Color backgroundColor = default, bool underline = false, bool strikethrough = false) : this(family, size, bold, italic)
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

    public override bool Equals(object? obj)
    {
        if (obj is not MetaFont other)
            return false;
        if (Math.Abs(Size - other.Size) > 0.01)
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
        return Strikethrough == other.Strikethrough;
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

internal class MetaFontAction(string? href, string? id)
{
    public string? Id { get;  } = id;

    public string? Href { get;  } = href;

    public bool IsEmpty()
        => string.IsNullOrWhiteSpace(Href);
    

    public MetaFontAction? Copy()
    {
        return IsEmpty() 
            ? null 
            : new MetaFontAction(Href, Id);
    }

    public static bool operator ==(MetaFontAction? a1, MetaFontAction? a2) => Equals(a1, a2);

    public static bool operator !=(MetaFontAction? a1, MetaFontAction? a2) => !Equals(a1, a2);

    public static bool Equals(MetaFontAction? a1, MetaFontAction? a2)
    {
        if (a1?.Id != a2?.Id)
            return false;
        return a1?.Href == a2?.Href;
    }

    public override bool Equals(object? obj) => Equals(this, obj as MetaFontAction);
        

    public override int GetHashCode()
    {
        var hash = 7;
        if (Id != null)
            hash = hash * 17 + Id.GetHashCode();

        if (Href != null)
            hash = hash * 17 + Href.GetHashCode();

        return hash;
    }

    public MetaFontAction(HyperlinkSpan actionSpan) : this(actionSpan.Href, actionSpan.Id)
    {
    }
}
