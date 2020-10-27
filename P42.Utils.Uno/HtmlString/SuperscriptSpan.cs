using P42.Utils;

namespace P42.Utils.Uno
{
	/// <summary>
	/// P42.Utils.Uno Superscript span.
	/// </summary>
	class SuperscriptSpan : Span, ICopiable<SuperscriptSpan>
	{
		internal const string SpanKey = "Superscript";

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.SuperscriptSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public SuperscriptSpan (int start, int end) : base(start, end) {
			Key = SpanKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.SuperscriptSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public SuperscriptSpan (SuperscriptSpan span) : this(span.Start, span.End) {
		}

		public void PropertiesFrom(SuperscriptSpan source)
		{
			base.PropertiesFrom(source);
		}

		public override Span Copy()
		{
			return new SuperscriptSpan(Start, End);
		}
	}
}

