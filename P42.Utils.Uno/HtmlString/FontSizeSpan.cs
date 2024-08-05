namespace P42.Utils.Uno
{
	/// <summary>
	/// Font size span.
	/// </summary>
	class FontSizeSpan : Span, ICopiable<FontSizeSpan>
	{
#pragma warning disable CC0021 // Use nameof
        internal const string SpanKey = "Size";
#pragma warning restore CC0021 // Use nameof

		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		public float Size { get; set; } = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.FontSizeSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		/// <param name="size">Size.</param>
		public FontSizeSpan(int start, int end, float size) : base (start, end) {
			Key = SpanKey;
			Size = size;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.FontSizeSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public FontSizeSpan(FontSizeSpan span) : this(span.Start, span.End, span.Size) { }

		public void PropertiesFrom(FontSizeSpan source)
		{
			base.PropertiesFrom(source);
			Size = source.Size;
		}

		public override Span Copy()
		{
			return new FontSizeSpan(Start, End, Size);
		}
	}
}

