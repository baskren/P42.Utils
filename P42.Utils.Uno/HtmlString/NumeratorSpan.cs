// /*******************************************************************
//  *
//  * NumeratorSpan.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno
{
	class NumeratorSpan: Span, ICopiable<NumeratorSpan>
	{
		internal const string SpanKey = "Numerator";

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.NumeratorSpan"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public NumeratorSpan(int start, int end) : base(start, end) {
			Key = SpanKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="P42.Utils.Uno.NumeratorSpan"/> class.
		/// </summary>
		/// <param name="span">Span.</param>
		public NumeratorSpan(NumeratorSpan span) : this(span.Start, span.End) {
		}

		public void PropertiesFrom(NumeratorSpan source)
		{
			base.PropertiesFrom(source);
		}

		public override Span Copy()
		{
			return new NumeratorSpan(Start, End);
		}
	}
}
