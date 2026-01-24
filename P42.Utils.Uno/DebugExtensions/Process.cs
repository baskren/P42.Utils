using System.Runtime.CompilerServices;

namespace P42.Utils.Uno;

public class Process : IProcess
{
    public ulong Memory( [CallerMemberName] string caller = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        #if WINDOWS
        var process = Windows.System.Diagnostics.ProcessDiagnosticInfo.GetForCurrentProcess();
        var memoryReport = process.MemoryUsage.GetReport();
        System.Diagnostics.Debug.WriteLine(
            $"""
             {caller}:{callerFile}:{callerLineNumber} 
             NonPagedPoolSizeInBytes: {memoryReport.NonPagedPoolSizeInBytes.HumanReadableBytes()} 
             PagedPoolSizeInBytes: {memoryReport.PagedPoolSizeInBytes.HumanReadableBytes()} 
             PageFaultCount: {StringExtensions.HumanReadableBytes(memoryReport.PageFaultCount)} 
             PageFileSizeInBytes: {memoryReport.PageFileSizeInBytes.HumanReadableBytes()} 
             PeakNonPagedPoolSizeInBytes: {memoryReport.PeakNonPagedPoolSizeInBytes.HumanReadableBytes()} 
             PeakPagedPoolSizeInBytes: {memoryReport.PeakPagedPoolSizeInBytes.HumanReadableBytes()}
             PeakPageFileSizeInBytes: {memoryReport.PeakPageFileSizeInBytes.HumanReadableBytes()}
             PeakVirtualMemorySizeInBytes: {memoryReport.PeakVirtualMemorySizeInBytes.HumanReadableBytes()}
             PeakWorkingSetSizeInBytes: {memoryReport.PeakWorkingSetSizeInBytes.HumanReadableBytes()}
             PrivatePageCount: {memoryReport.PrivatePageCount.HumanReadableBytes()}
             """); 
        return memoryReport.PrivatePageCount;
        #else
        return P42.Utils.Process.Memory(caller, callerFile, callerLineNumber);
        #endif
        
 
        
    }
}
