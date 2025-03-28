using System.Runtime.CompilerServices;
using Windows.System.Diagnostics;

namespace P42.Utils.Uno;

internal class Process : P42.Utils.IProcess
{
    public ulong Memory([CallerMemberName] string caller = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        var process = ProcessDiagnosticInfo.GetForCurrentProcess();
        var memoryReport = process.MemoryUsage.GetReport();
        System.Diagnostics.Debug.WriteLine($"{caller}:{callerFile}:{callerLineNumber}: "
            + "\n NonPagedPoolSizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.NonPagedPoolSizeInBytes)
            + "\n PagedPoolSizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.PagedPoolSizeInBytes)
            + "\n PageFaultCount: " + StringExtensions.HumanReadableBytes(memoryReport.PageFaultCount)
            + "\n PageFileSizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.PageFileSizeInBytes)
            + "\n PeakNonPagedPoolSizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.PeakNonPagedPoolSizeInBytes)
            + "\n PeakPagedPoolSizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.PeakPagedPoolSizeInBytes)
            + "\n PeakPageFileSizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.PeakPageFileSizeInBytes)
            + "\n PeakVirtualMemorySizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.PeakVirtualMemorySizeInBytes)
            + "\n PeakWorkingSetSizeInBytes: " + StringExtensions.HumanReadableBytes(memoryReport.PeakWorkingSetSizeInBytes)
            + "\n PrivatePageCount: " + StringExtensions.HumanReadableBytes(memoryReport.PrivatePageCount)
            + "\n\n"); 
        return memoryReport.PrivatePageCount;
    }

}
