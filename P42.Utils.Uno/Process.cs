using System;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.System.Diagnostics;

namespace P42.Utils.Uno;

public class Process : IProcess
{

    /// <summary>
    /// Dumps current memory consumption info to Console and Debug output
    /// </summary>
    /// <param name="path"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public ulong Memory([CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
    {
        var process = ProcessDiagnosticInfo.GetForCurrentProcess();
        var memoryReport = process.MemoryUsage.GetReport();

        var builder = new StringBuilder();
        
        builder.Append($"{nameof(Memory)} {path}:{line}");
        builder.Append($"\t NonPagedPoolSizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.NonPagedPoolSizeInBytes)}");
        builder.Append($"\t PagedPoolSizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.PagedPoolSizeInBytes)}");
        builder.Append($"\t PageFaultCount: {StringExtensions.HumanReadableBytes(memoryReport.PageFaultCount)}");
        builder.Append($"\t PageFileSizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.PageFileSizeInBytes)}");
        builder.Append($"\n PeakNonPagedPoolSizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.PeakNonPagedPoolSizeInBytes)}");
        builder.Append($"\n PeakPagedPoolSizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.PeakPagedPoolSizeInBytes)}");
        builder.Append($"\n PeakPageFileSizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.PeakPageFileSizeInBytes)}"); 
        builder.Append($"\n PeakVirtualMemorySizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.PeakVirtualMemorySizeInBytes)}");
        builder.Append($"\n PeakWorkingSetSizeInBytes: {StringExtensions.HumanReadableBytes(memoryReport.PeakWorkingSetSizeInBytes)}");
        builder.Append($"\n PrivatePageCount: {StringExtensions.HumanReadableBytes(memoryReport.PrivatePageCount)}\n\n");
        
        System.Diagnostics.Debug.WriteLine(builder.ToString());
        Console.WriteLine(builder.ToString());
        
        return memoryReport.PrivatePageCount;
    }

}
