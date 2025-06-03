using System;
using Windows.UI;

namespace P42.Utils.Uno;

internal class MetaFont(Microsoft.UI.Xaml.Media.FontFamily? family, double size, short fontWeight = 400, bool italic = false)
{
    #region Properties

    public Microsoft.UI.Xaml.Media.FontFamily? Family = family;

    public double Size = size;

    public bool Italic = italic;

    //public bool Bold = bold;
    public short FontWeight = fontWeight;

    public FontBaseline Baseline = FontBaseline.Normal;

    public MetaFontAction? Action;

    public Color TextColor;

    public Color BackgroundColor;

    public bool Underline;

    public bool Strikethrough;

    #endregion


    #region Construction

    public MetaFont(MetaFont f) : this (f.Family, f.Size, f.FontWeight, f.Italic)
    {
        Baseline = f.Baseline;
        Action = f.Action?.Copy();
        BackgroundColor = f.BackgroundColor;
        TextColor = f.TextColor;
        Underline = f.Underline;
        Strikethrough = f.Strikethrough;
    }
    public MetaFont(Microsoft.UI.Xaml.Media.FontFamily family, double size, short fontWeight = 400, bool italic = false, string id = "", string href = "", Color textColor = default, Color backgroundColor = default, bool underline = false, bool strikethrough = false) : this(family, size, fontWeight, italic)
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
        if (FontWeight != other.FontWeight)
            return false;
        if (Italic != other.Italic)
            return false;
        if (Family != other.Family)
            return false;
        if (Baseline != other.Baseline)
            return false;
        if (ReferenceEquals(Action, other.Action))
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
        var hash = HashCode.Combine(Family, Size, FontWeight, Italic, Baseline, Action, TextColor, BackgroundColor);
        return HashCode.Combine(hash, Underline, Strikethrough);
    }
    #endregion


    public bool IsActiveAction
        => Action is not null && !Action.IsEmpty();
}

internal class MetaFontAction(string href, string id)
{
    public readonly string Id = id;

    public readonly string Href = href;

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Href);
    }

    public MetaFontAction? Copy()
        => IsEmpty() 
            ? null 
            : new MetaFontAction(Href, Id);
    

    public static bool operator ==(MetaFontAction a1, MetaFontAction a2) => Equals(a1, a2);

    public static bool operator !=(MetaFontAction a1, MetaFontAction a2) => !Equals(a1, a2);

    public static bool Equals(MetaFontAction a1, MetaFontAction a2)
    {
        if (a1.Id != a2.Id)
            return false;
        return a1.Href == a2.Href;
    }

    public override bool Equals(object? obj)
        => obj is MetaFontAction other && Equals(this, other);
    


    public override int GetHashCode()
        => HashCode.Combine(Href, Id);

    public MetaFontAction(HyperlinkSpan actionSpan) : this(actionSpan.Href, actionSpan.Id)
    {
    }
}
