namespace P42.Utils.Uno
{
	/// <summary>
	/// Italics span.
	/// </summary>
	class ItalicsSpan : Span, ICopiable<ItalicsSpan>
	{
		internal const string SpanKey = "Italics";

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.ItalicsSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public ItalicsSpan (int start, int end) : base (start, end) {
			Key = SpanKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.ItalicsSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public ItalicsSpan(ItalicsSpan span) : base (span.Start, span.End) {
		}

		public void PropertiesFrom(ItalicsSpan source)
		{
			base.PropertiesFrom(source);
		}

		public override Span Copy()
		{
			return new ItalicsSpan(Start, End);
		}
	}	
}

