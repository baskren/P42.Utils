using P42.Utils.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace P42.Utils.UWP
{
    public class DiskSpace : IDiskSpace
    {
        public ulong Free
        {
            get
            {
                var task = GetFreeSpace();
                var waiter = task.GetAwaiter();
                var result = waiter.GetResult();
                return result;
            }
        }

        public ulong Size
        {
            get
            {
                var task = GetCapacity();
                var waiter = task.GetAwaiter();
                var result = waiter.GetResult();
                return result;
            }
        }

        public ulong Used
        {
            get
            {
                return Size - Free;
            }
        }

        async Task<UInt64> Get(string property)
        {
            StorageFolder local = ApplicationData.Current.LocalFolder;
            var task = local.Properties.RetrievePropertiesAsync(new string[] { property });
            var retrivedProperties = await task.AsTask().ConfigureAwait(false);
            return (UInt64)retrivedProperties[property];
        }

        async Task<UInt64> GetFreeSpace()
            => await Get("System.FreeSpace").ConfigureAwait(false);

        async Task<UInt64> GetCapacity()
            => await Get("System.Capacity").ConfigureAwait(false);



    }
}