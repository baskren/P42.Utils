using System;
using System.Threading.Tasks;


namespace P42.Utils.UWP
{
	public class DiskSpace : IDiskSpace
	{
		public ulong Free
		{
			get
			{
				var task = GetFreeSpace();
				var result = task.WaitAndUnwrapException();
				return result;
			}
		}


		public async Task<UInt64> GetFreeSpace()
		{
			StorageFolder local = ApplicationData.Current.LocalFolder;
			var retrivedProperties = await local.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace" }).ConfigureAwait(false);
			return (UInt64)retrivedProperties["System.FreeSpace"];
		}


	}
}