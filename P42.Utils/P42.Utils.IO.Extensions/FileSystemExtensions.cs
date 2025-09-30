using System;
using System.IO;
using System.Linq;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// Extensions for working with FileInfo 
/// </summary>
public static class FileSystemExtensions
{
    #region File Permissions
    /// <summary>
    /// Checks if the FileSystemItem has write permission
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    public static bool HasWritePermission(this FileSystemInfo fileSystemInfo)
        => OperatingSystem.IsWindows()
            ? fileSystemInfo.HasWindowsWriteAccessControl()
            : fileSystemInfo.HasUnixWritePermission();
    
    /// <summary>
    /// Checks if the FileSystemItem has read permission
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    public static bool HasReadPermission(this FileSystemInfo fileSystemInfo)
        => OperatingSystem.IsWindows()
            ? fileSystemInfo.HasWindowsReadAccessControl()
            : fileSystemInfo.HasUnixReadPermission();
    
    /// <summary>
    /// Checks if the FileSystemItem has execute permission
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    public static bool HasExecutePermission(this FileSystemInfo fileSystemInfo)
        => OperatingSystem.IsWindows()
            ? fileSystemInfo.HasWindowsExecuteAccessControl()
            : fileSystemInfo.HasUnixExecutePermission();

    #region Unix Permissions
    private static bool HasUnixWritePermission(this FileSystemInfo fileSystemInfo, bool checkParent = true)
    {
        if (!fileSystemInfo.Exists)
        {
            if (checkParent && fileSystemInfo.Parent() is { } parent)
                return parent.HasUnixWritePermission(false);
            return false;
        }        
        var mode = fileSystemInfo.UnixFileMode;
        // Check write permissions for owner, group, and others
        return (mode & UnixFileMode.UserWrite) == UnixFileMode.UserWrite ||
               (mode & UnixFileMode.GroupWrite) == UnixFileMode.GroupWrite ||
               (mode & UnixFileMode.OtherWrite) == UnixFileMode.OtherWrite;
    }

    private static bool HasUnixReadPermission(this FileSystemInfo fileSystemInfo, bool checkParent = true)
    {
        if (!fileSystemInfo.Exists)
        {
            if (checkParent && fileSystemInfo.Parent() is { } parent)
                return parent.HasUnixReadPermission(false);
            return false;
        }        
        var mode = fileSystemInfo.UnixFileMode;
        // Check read permissions for owner, group, and others
        return (mode & UnixFileMode.UserRead) == UnixFileMode.UserRead ||
               (mode & UnixFileMode.GroupRead) == UnixFileMode.GroupRead ||
               (mode & UnixFileMode.OtherRead) == UnixFileMode.OtherRead;
    }
    
    private static bool HasUnixExecutePermission(this FileSystemInfo fileSystemInfo, bool checkParent = true)
    {
        if (!fileSystemInfo.Exists)
        {
            if (checkParent && fileSystemInfo.Parent() is { } parent)
                return parent.HasUnixExecutePermission(false);
            return false;
        }        
        var mode = fileSystemInfo.UnixFileMode;
        // Check execute permissions for owner, group, and others
        return (mode & UnixFileMode.UserExecute) == UnixFileMode.UserExecute ||
               (mode & UnixFileMode.GroupExecute) == UnixFileMode.GroupExecute ||
               (mode & UnixFileMode.OtherExecute) == UnixFileMode.OtherExecute;
    }
    #endregion

    #region Windows Permissions
    #pragma warning disable CA1416
    private static bool HasWindowsWriteAccessControl(this FileSystemInfo fileSystemInfo, bool checkParent = true)
    {
        if (!fileSystemInfo.Exists)
        {
            if (checkParent && fileSystemInfo.Parent() is { } parent)
                return parent.HasWindowsWriteAccessControl(false);
            return false;
        }
        
        return fileSystemInfo switch
        {
            FileInfo fileInfo => fileInfo
                .GetAccessControl()
                .GetAccessRules(
                    true, 
                    true, 
                    typeof(System.Security.Principal.SecurityIdentifier)
                    )
                .OfType<System.Security.AccessControl.FileSystemAccessRule>()
                .Any(rule => rule.FileSystemRights.HasFlag(System.Security.AccessControl.FileSystemRights.WriteData)),
            DirectoryInfo directoryInfo => directoryInfo
                .GetAccessControl()
                .GetAccessRules(
                    true, 
                    true, 
                    typeof(System.Security.Principal.SecurityIdentifier)
                    )
                .OfType<System.Security.AccessControl.FileSystemAccessRule>()
                .Any(rule => rule.FileSystemRights.HasFlag(System.Security.AccessControl.FileSystemRights.WriteData)),
            _ => false
        };
    }

    private static bool HasWindowsReadAccessControl(this FileSystemInfo fileSystemInfo, bool checkParent = true)
    {
        if (!fileSystemInfo.Exists)
        {
            if (checkParent && fileSystemInfo.Parent() is { } parent)
                return parent.HasWindowsReadAccessControl(false);
            return false;
        }
        
        return fileSystemInfo switch
        {
            FileInfo fileInfo => fileInfo
                .GetAccessControl()
                .GetAccessRules(
                    true, 
                    true, 
                    typeof(System.Security.Principal.SecurityIdentifier)
                    )
                .OfType<System.Security.AccessControl.FileSystemAccessRule>()
                .Any(rule => rule.FileSystemRights.HasFlag(System.Security.AccessControl.FileSystemRights.ReadData)),
            DirectoryInfo directoryInfo => directoryInfo
                .GetAccessControl()
                .GetAccessRules(
                    true, 
                    true, 
                    typeof(System.Security.Principal.SecurityIdentifier)
                    )
                .OfType<System.Security.AccessControl.FileSystemAccessRule>()
                .Any(rule => rule.FileSystemRights.HasFlag(System.Security.AccessControl.FileSystemRights.ReadData)),
            _ => false
        };
    }
    
    private static bool HasWindowsExecuteAccessControl(this FileSystemInfo fileSystemInfo, bool checkParent = true)
    {
        if (!fileSystemInfo.Exists)
        {
            if (checkParent && fileSystemInfo.Parent() is { } parent)
                return parent.HasWindowsExecuteAccessControl(false);
            return false;
        }
        
        return fileSystemInfo switch
        {
            FileInfo fileInfo => fileInfo
                .GetAccessControl()
                .GetAccessRules(
                    true, 
                    true, 
                    typeof(System.Security.Principal.SecurityIdentifier)
                    )
                .OfType<System.Security.AccessControl.FileSystemAccessRule>()
                .Any(rule => rule.FileSystemRights.HasFlag(System.Security.AccessControl.FileSystemRights.ExecuteFile)),
            DirectoryInfo directoryInfo => directoryInfo
                .GetAccessControl()
                .GetAccessRules(
                    true, 
                    true, 
                    typeof(System.Security.Principal.SecurityIdentifier)
                    )
                .OfType<System.Security.AccessControl.FileSystemAccessRule>()
                .Any(rule => rule.FileSystemRights.HasFlag(System.Security.AccessControl.FileSystemRights.ExecuteFile)),
            _ => false
        };
    }
    #pragma warning restore CA1416
    #endregion
    
    #endregion
    
    
    /// <summary>
    /// Gets parent of FileSystemItem
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    public static FileSystemInfo? Parent(this FileSystemInfo fileSystemInfo)
        => fileSystemInfo switch
        {
            FileInfo fileInfo => fileInfo.Directory,
            DirectoryInfo directoryInfo => directoryInfo.Parent,
            _ => null
        };
    
    /// <summary>
    /// Is FileSystemItem potentially writable
    /// </summary>
    /// <param name="file"></param>
    /// <param name="tryForceOverwrite"></param>
    /// <returns>false if exists and tryForceOverwrite is false</returns>
    public static bool WritePossible(this FileInfo file, bool tryForceOverwrite)
    {
        try
        {
            var dir = new DirectoryInfo(file.FullName);
            if (dir.Exists) 
                return false;

            if (file.Exists)
            {
                if (file.Attributes.HasFlag(FileAttributes.ReadOnly))
                    return false;

                return tryForceOverwrite && file.HasWritePermission();
            }

            if (file.Parent() is  DirectoryInfo parentDir) 
                return WritePossible(parentDir, tryForceOverwrite);

        }
        catch (Exception e)
        {
            QLog.Error(e);
        }
        
        return false;
    }

    public static bool WritePossible(this DirectoryInfo dir, bool tryForceOverwrite)
    {
        try
        {
            var file = new FileInfo(dir.FullName);
            if (file.Exists)
                return false;

            if (dir.Exists)
            {
                if (dir.Attributes.HasFlag(FileAttributes.ReadOnly))
                    return false;

                return tryForceOverwrite && dir.HasWritePermission();
            }

            if (dir.Parent() is DirectoryInfo parentDir)
                return WritePossible(parentDir, tryForceOverwrite);

        }
        catch (Exception e)
        {
            QLog.Error(e);
        }

        return false;
    }

    #region File Name Legality
    private static readonly string[] IllegalWindowsNames =
    [
        ".", "..", "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    ];

    /// <summary>
    /// Tests if a string is a legal file name in Windows, Mac, iOS, Android, and Linux
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool IsLegalFileName(this string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;
        
        foreach (var c in fileName)
        {
            if (char.IsLetterOrDigit(c) || 
                c == '.' ||
                c == '_' ||
                c == '-' ||
                c == ' ')
                continue;
            return false;
        }
        
        if (fileName.EndsWith('.') || fileName.EndsWith(' '))
            return false;

        fileName = fileName.ToUpper().Split('.').First();
        return IllegalWindowsNames.All(name => fileName != name);
    }
    #endregion

}
