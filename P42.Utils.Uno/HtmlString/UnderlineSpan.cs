using P42.Utils;

namespace P42.Utils.Uno
{
	/// <summary>
	/// P42.Utils.Uno Underline span.
	/// </summary>
	class UnderlineSpan : Span, ICopiable<UnderlineSpan>
	{
		internal const string SpanKey = "Underline";

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.UnderlineSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public UnderlineSpan (int start, int end) : base (start, end) {
			//Color = color;
			//Style = style;
			Key = SpanKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.UnderlineSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public UnderlineSpan (UnderlineSpan span) : this (span.Start, span.End) {
		}

		public void PropertiesFrom(UnderlineSpan source)
		{
			base.PropertiesFrom(source);
		}

		public override Span Copy()
		{
			return new UnderlineSpan(Start, End);
		}
	}
}

