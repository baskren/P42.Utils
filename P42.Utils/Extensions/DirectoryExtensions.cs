using System;
using System.IO;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class DirectoryExtensions
{
    /// <summary>
    /// Return Directory Info for path, creating if it doesn't exist
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static DirectoryInfo? GetOrCreateDirectory(string fullPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);

        var info = Directory.Exists(fullPath)
            ? new DirectoryInfo(fullPath)
            : Directory.CreateDirectory(fullPath);
    
        if (!info.Exists)
            throw new Exception("Could not assure existence of directory [" + info + "]");
        
        return info;
    }

    /// <summary>
    /// Attempt to get or create a directory
    /// </summary>
    /// <param name="fullPath">Path</param>
    /// <param name="directoryInfo">result</param>
    /// <returns>true on success</returns>
    public static bool TryGetOrCreateDirectory(string fullPath, out DirectoryInfo? directoryInfo)
    {
        directoryInfo = null;

        if (string.IsNullOrWhiteSpace(fullPath))
            return false;

        try
        {
            directoryInfo = Directory.Exists(fullPath)
                ? new DirectoryInfo(fullPath)
                : Directory.CreateDirectory(fullPath);
        }
        catch (Exception e)
        {
            QLog.Debug(e);
            return false;
        }
        
        return directoryInfo.Exists;
    }
}
