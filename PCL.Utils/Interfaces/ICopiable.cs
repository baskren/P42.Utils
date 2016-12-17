using System;
namespace PCL.Utils
{
	public interface ICopiable<T>
	{
		void ValueFrom(T other);
	}
}
