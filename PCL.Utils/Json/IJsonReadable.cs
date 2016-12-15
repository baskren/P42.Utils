using Newtonsoft.Json;
namespace PCL.Utils
{
	public interface IJsonReadable 
	{
		void ValueFrom(JsonReader sr);
	}

}
