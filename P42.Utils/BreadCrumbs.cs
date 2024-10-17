using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class BreadCrumbs
{
    private const string BreadCrumbFolderName = "P42.Utils.BreadCrumbs";

    private static string? _folderPath;
    private static string FolderPath
    {
        get
        {
            if (_folderPath != null)
                return _folderPath;

            DirectoryExtensions.GetOrCreateDirectory(Environment.ApplicationCachePath);
            var folderPath = Path.Combine(Environment.ApplicationCachePath, BreadCrumbFolderName);
            DirectoryExtensions.GetOrCreateDirectory(folderPath);
            _folderPath = folderPath;
            return _folderPath;
        }
    }

    public static bool IsEnabled
    {
        get => _streamWriter != null;
        set
        {
            if (IsEnabled == value)
                return;

            if (_streamWriter != null)
            {
                _streamWriter.Flush();
                _streamWriter.Close();
                _streamWriter.Dispose();
                _streamWriter = null;
            }
            else
            {
                var fileName = DateTime.Now.ToString("yyyyMMdd'T'HHmmss") + ".txt";
                var filePath = Path.Combine(FolderPath, fileName);
                _streamWriter = File.CreateText(filePath);
            }
        }
    }

    public static Action<string, string?, string?, int, string?>? EventAction { get; set; }
    public static Action<Exception, string, string?, string?, int, string?>? ExceptionAction  { get; set; }

    private static StreamWriter? _streamWriter;


    public static void Add(Type type, object? crumb = null, [CallerMemberName] string? method = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string? path = null)
        => Add(type.ToString(), crumb?.ToString(), method, lineNumber, path);

    public static void Add(string className, string? crumb = null, [CallerMemberName] string? method = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string? path = null)
    {
        if (IsEnabled)
        {
            _streamWriter?.WriteLine($"[{DateTime.Now:o}] [{className}.{method}:{lineNumber}] [{crumb}] ");
            _streamWriter?.Flush();
        }
        EventAction?.Invoke(className, crumb, method, lineNumber, path);
    }

    public static void AddException(Exception e, Type type, object? crumb = null, [CallerMemberName] string? method = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string? path = null)
        => AddException(e, type.ToString(), crumb?.ToString(), method, lineNumber, path);

    public static void AddException(Exception e, string className, string? crumb = null, [CallerMemberName] string? method = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string? path = null)
    {
        if (IsEnabled)
        {
            _streamWriter?.WriteLine($"[{DateTime.Now:o}] [{className}.{method}:{lineNumber}] [{crumb}] ");
            _streamWriter?.Flush();
        }
        ExceptionAction?.Invoke(e, className, crumb, method, lineNumber, path);
    }

    public static string[] List()
    {
        try
        {
            var files = Directory.GetFiles(FolderPath);
            Array.Sort(files);
            return files;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        return [];
    }

    public static bool Clear()
        => Clear(DateTime.MinValue.AddYears(1));

    public static bool Clear(TimeSpan timeSpan)
        => Clear(DateTime.Now - timeSpan);

    public static bool Clear(DateTime dateTime)
    {
        // complete clear
        if (!Directory.Exists(FolderPath))
            return false;

        var filesRemaining = false;
        foreach (var file in Directory.EnumerateFiles(FolderPath))
        {
            if (!File.Exists(file))
                continue;

            if (File.GetLastWriteTime(file) < dateTime)
                File.Delete(file);
            else
                filesRemaining = true;
        }
        return filesRemaining;
    }


    public static void SplitProperties(Dictionary<string, string> breadcrumbs, int max = 125)
    {
        var keys = breadcrumbs.Keys.ToArray();
        foreach (var key in keys)
        {
            if (!(breadcrumbs?.TryGetValue(key, out var property) ?? false))
                continue;

            //if (!string.IsNullOrWhiteSpace(RedundantPath) && property.StartsWith(RedundantPath))
            //    property = property.Substring(_redundantPathLength);
            if (string.IsNullOrWhiteSpace(property))
                continue;

            if (property.Length > max)
            {
                breadcrumbs[key] = property[..max];
                for (var i = 1; i * max < property.Length; i++)
                    breadcrumbs[key + "." + i] = property.Substring(i * max, Math.Min(max, property.Length - i * max));
            }
            else
                breadcrumbs[key] = property;
        }
    }

}
