namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Bold span.
/// </summary>
internal class FontWeightSpan : Span, ICopiable<FontWeightSpan>
{
    internal const string SpanKey = "Weight";

    public short Weight { get; set; }

    public bool IsRelativeToParent { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontWeightSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="isRelativeToParent"></param>
    /// <param name="id">optional</param>
    /// <param name="weight"></param>
    public FontWeightSpan(int start, int end, short weight, bool isRelativeToParent = false, string id = "") : base(start, end, id)
    {
        Key = SpanKey;
        Weight = weight;
        IsRelativeToParent = isRelativeToParent;
    }

    public FontWeightSpan(int start, int end, Windows.UI.Text.FontWeight fontWeight, string id = "") : this (start, end, (short)fontWeight.Weight, false, id)
    {  }


    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontWeightSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public FontWeightSpan (FontWeightSpan span) : this (span.Start, span.End, span.Weight) { }

    /// <summary>
    /// Copies the properties from the specified source.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(FontWeightSpan source) 
        => base.PropertiesFrom(source);
    

    /// <summary>
    /// Make a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new FontWeightSpan(Start, End, Weight);
    
}
