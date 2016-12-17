using Newtonsoft.Json;
namespace PCL.Utils
{
	public interface IJsonReadable
	{
		void ValueFrom(JsonReader reader);
	}

	public interface IJsonReadable<T> : IJsonReadable, ICopiable<T>
	{
	}

}
