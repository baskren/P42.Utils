using System;
namespace PCL.Utils
{
	public interface ICopiable<T>
	{
		void PropertiesFrom(T other);
	}
}
