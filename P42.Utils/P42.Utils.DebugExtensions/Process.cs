using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class Process
{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    internal static IProcess? PlatformProcess;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    /// <summary>
    /// How much memory is consumed by this process / application
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    //public static ulong Memory => PlatformProcess?.Memory() ?? throw new IncompleteInitialization();
    public static ulong Memory( [CallerMemberName] string caller = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        /*
        var process = ProcessDiagnosticInfo.GetForCurrentProcess();
        var memoryReport = process.MemoryUsage.GetReport();
        Debug.WriteLine(
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
        */
        
        // HEAP
        
        var managedMemoryBytes = GC.GetTotalMemory(true); // true to force a garbage collection before calculation
        Console.WriteLine($"Managed memory usage: {managedMemoryBytes.HumanReadableBytes()}");
        
        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
        var privateMemoryBytes = currentProcess.PrivateMemorySize64; // Memory allocated to this process and not shared
        Console.WriteLine($"Private Memory Size: {privateMemoryBytes.HumanReadableBytes()}");
        Console.WriteLine($"Working Set: {currentProcess.WorkingSet64.HumanReadableBytes()}"); // Physical memory currently used by the process
        Console.WriteLine($"Virtual Memory Size: {currentProcess.VirtualMemorySize64.HumanReadableBytes()}"); // Total virtual memory used by the process

        return (ulong)privateMemoryBytes;
    }
}
