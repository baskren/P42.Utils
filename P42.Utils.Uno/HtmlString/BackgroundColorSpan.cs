using System;
using P42.Utils;
using Windows.UI;

namespace P42.Utils.Uno
{
	/// <summary>
	/// P42.Utils.Uno Background color span.
	/// </summary>
	class BackgroundColorSpan : Span, ICopiable<BackgroundColorSpan>
	{
		internal const string SpanKey = "BackgroundColor";

		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		/// <value>The color.</value>
		public Color Color { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.BackgroundColorSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		/// <param name="color">Color.</param>
		public BackgroundColorSpan (int start, int end, Color color) : base (start, end)
		{
			Key = SpanKey;
			Color = color;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FoP42.Utils.Unorms9Patch.BackgroundColorSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public BackgroundColorSpan (BackgroundColorSpan span) : this (span.Start, span.End, span.Color) {
		}

		public void PropertiesFrom(BackgroundColorSpan source)
		{
			base.PropertiesFrom(source);
			Color = source.Color;
		}

		public override Span Copy()
		{
			return new BackgroundColorSpan(Start, End, Color);
		}

	}
}

