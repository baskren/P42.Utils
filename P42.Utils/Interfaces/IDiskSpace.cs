using System.Threading.Tasks;

namespace P42.Utils.Interfaces;

public interface IDiskSpace
{
    ulong Free { get; }

    ulong Size { get; }

    ulong Used { get; }
        
    Task<ulong> GetFreeSpaceAsync();
        
    Task<ulong> GetCapacityAsync();
}
