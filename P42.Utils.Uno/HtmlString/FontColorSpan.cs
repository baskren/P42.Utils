using P42.Utils;
using Windows.UI;

namespace P42.Utils.Uno
{
	/// <summary>
	/// P42.Utils.Uno Font color span.
	/// </summary>
	class FontColorSpan : Span, ICopiable<FontColorSpan>
	{
		internal const string SpanKey = "FontColor";

		/// <summary>
		/// Gets or sets the font foreground color.
		/// </summary>
		/// <value>The color.</value>
		public Color Color { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.FontColorSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		/// <param name="color">Color.</param>
		public FontColorSpan (int start, int end, Color color) : base (start, end) {
			Key = SpanKey;
			Color = color;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.FontColorSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public FontColorSpan(FontColorSpan span) : this (span.Start, span.End, span.Color) {
		}

		public void PropertiesFrom(FontColorSpan source)
		{
			base.PropertiesFrom(source);
			Color = source.Color;
		}

		public override Span Copy()
		{
			return new FontColorSpan(Start, End, Color);
		}
	}
}

