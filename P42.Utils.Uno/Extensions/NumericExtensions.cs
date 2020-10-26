using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Uno
{
	public static class NumericExtensions
	{
		public static T Clamp<T>(this T self, T min, T max) where T : IComparable
		{
			if (Comparer<T>.Default.Compare(max, min) < 0)
				return max;
			else if (Comparer<T>.Default.Compare(self, min) < 0)
				return min;
			else if (Comparer<T>.Default.Compare(self, max) > 0)
				return max;
			return self;
		}
	}
}
