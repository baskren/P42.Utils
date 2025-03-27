using System;
using System.IO;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// Stream extensions
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Write stream to a file at destinationFilePath
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="destinationFilePath"></param>
    /// <param name="mode"></param>
    /// <param name="access"></param>
    /// <param name="share"></param>
    public static void CopyToPath(this Stream stream, string destinationFilePath, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
    {
        try
        {
            using var destinationFileStream = new FileStream(destinationFilePath, mode, access, share);
            stream.CopyTo(destinationFileStream);
        }
        catch (Exception e)
        {
            QLog.Error(e, $"Unable to write stream to file {destinationFilePath}");
            throw;
        }
    }

}
