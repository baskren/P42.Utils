using P42.Utils;

namespace P42.Utils.Uno
{
	/// <summary>
	/// P42.Utils.Uno Subscript span.
	/// </summary>
	class SubscriptSpan : Span, ICopiable<SubscriptSpan>
	{
		internal const string SpanKey = "Subscript";

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.SubscriptSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public SubscriptSpan (int start, int end) : base (start, end) {
			Key = SpanKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.SubscriptSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public SubscriptSpan (SubscriptSpan span) : this (span.Start, span.End) {
		}

		public void PropertiesFrom(SubscriptSpan source)
		{
			base.PropertiesFrom(source);
		}

		public override Span Copy()
		{
			return new SubscriptSpan(Start, End);
		}
	}
}

