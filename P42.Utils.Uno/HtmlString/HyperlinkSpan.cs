// /*******************************************************************
//  *
//  * ActionSpan.cs copyright 2017 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/
using System;
using System.Windows.Input;
using P42.Utils;

namespace P42.Utils.Uno
{
	class HyperlinkSpan : Span, ICopiable<HyperlinkSpan>
	{
		internal const string SpanKey = "Hyperlink";

        internal const string NullId = "HyperlinkSpanNullId";

		public string Href { get; set; }

		public string Id { get; set; }


		public HyperlinkSpan(int start, int end, string href=null, string id=null) : base (start, end) {
			Key = SpanKey;
			Href = href;
			Id = id;
		}

		public HyperlinkSpan(HyperlinkSpan span) : this (span.Start, span.End, span.Href, span.Id) {
		}

		public void PropertiesFrom(HyperlinkSpan source)
		{
			base.PropertiesFrom(source);
			Href = source.Href;
			Id = source.Id;
		}

		public override Span Copy()
		{
			return new HyperlinkSpan(Start, End, Href, Id);
		}

        public bool IsEmpty() => string.IsNullOrWhiteSpace(Href) && string.IsNullOrWhiteSpace(Id);
	}
}
