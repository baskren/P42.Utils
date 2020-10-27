using System.Windows.Input;
using P42.Utils;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{
	/// <summary>
	/// Font family span.
	/// </summary>
	class FontFamilySpan : Span, ICopiable<FontFamilySpan>
	{
		internal const string SpanKey = "FontFamily";

		FontFamily _fontName;
		/// <summary>
		/// Gets or sets the name of the font family -OR- resource ID or embedded resource font.
		/// </summary>
		/// <value>The name of the font family.</value>
		public FontFamily FontFamily { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.FontFamilySpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		/// <param name="fontName">Font name.</param>
		public FontFamilySpan(int start, int end, FontFamily fontFamily =null) : base (start, end) {
			Key = SpanKey;
			FontFamily = fontFamily;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.FontFamilySpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public FontFamilySpan(FontFamilySpan span) : this (span.Start, span.End, span.FontFamily) {
		}

		public void PropertiesFrom(FontFamilySpan source)
		{
			base.PropertiesFrom(source);
			FontFamily = source.FontFamily;
		}

		public override Span Copy()
		{
			return new FontFamilySpan(Start, End, FontFamily);
		}
	}
}

