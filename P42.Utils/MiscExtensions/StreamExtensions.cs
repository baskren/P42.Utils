using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public static async Task<byte[]> ReadBytesAsync(this Stream stream)
    {
        var readBuffer = new byte[stream.CanSeek ? (stream.Length - stream.Position) : 4096];
        int totalBytesRead = 0;
        int num;
        while ((num = await stream.ReadAsync(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
        {
            totalBytesRead += num;
            if (totalBytesRead == readBuffer.Length)
            {
                var nextBytes = new byte[1];
                if (await stream.ReadAsync(nextBytes, 0, 1) == 1)
                {
                    var array = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, array, 0, readBuffer.Length);
                    Buffer.SetByte(array, totalBytesRead, nextBytes[0]);
                    readBuffer = array;
                    totalBytesRead++;
                }
            }
        }

        var array2 = readBuffer;
        if (readBuffer.Length != totalBytesRead)
        {
            array2 = new byte[totalBytesRead];
            Buffer.BlockCopy(readBuffer, 0, array2, 0, totalBytesRead);
        }

        return array2;
    }

    public static byte[] ReadBytes(this Stream stream)
    {
        if (stream.CanSeek && stream.Position == 0L && stream is MemoryStream memoryStream)
            return memoryStream.ToArray();

        var array = new byte[stream.CanSeek ? (stream.Length - stream.Position) : 4096];
        int num = 0;
        int num2;
        while ((num2 = stream.Read(array, num, array.Length - num)) > 0)
        {
            num += num2;
            if (num == array.Length)
            {
                int num3 = stream.ReadByte();
                if (num3 != -1)
                {
                    var array2 = new byte[array.Length * 2];
                    Buffer.BlockCopy(array, 0, array2, 0, array.Length);
                    Buffer.SetByte(array2, num, (byte)num3);
                    array = array2;
                    num++;
                }
            }
        }

        var array3 = array;
        if (array.Length != num)
        {
            array3 = new byte[num];
            Buffer.BlockCopy(array, 0, array3, 0, num);
        }

        return array3;
    }

    //
    // Summary:
    //     Reads the text container into the specified stream.
    //
    // Parameters:
    //   stream:
    //
    // Returns:
    //     The string using the default encoding.
    //
    // Remarks:
    //     The stream will be disposed when calling this method.
    public static string ReadToEnd(this Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    //
    // Summary:
    //     Reads the text container into the specified stream.
    //
    // Parameters:
    //   stream:
    //
    // Returns:
    //     The string using the default encoding.
    //
    // Remarks:
    //     The stream will be disposed when calling this method.
    public static string ReadToEnd(this Stream stream, Encoding encoding)
    {
        using var streamReader = new StreamReader(stream, encoding);
        return streamReader.ReadToEnd();
    }

    //
    // Summary:
    //     Warning, if stream cannot be seek, will read from current position! Warning,
    //     stream position will not been restored!
    //
    // Parameters:
    //   stream:
    //
    //   start:
    public static bool StartsWith(this Stream stream, byte[] start)
    {
        if (stream.CanSeek)
            stream.Position = 0L;

        var array = new byte[start.Length];
        stream.ReadExactly(array);
        return start.SequenceEqual(array);
    }

    //
    // Summary:
    //     Create a MemoryStream, copy source to it, and set position to 0.
    //
    // Parameters:
    //   source:
    //     Stream to copy
    //
    // Returns:
    //     Newly created memory stream, position set to 0
    public static MemoryStream ToMemoryStream(this Stream source)
    {
        var memoryStream = new MemoryStream();
        source.CopyTo(memoryStream);
        memoryStream.Position = 0L;
        return memoryStream;
    }

    //
    // Summary:
    //     Check if stream is seekable (CanSeek), if not copy it to a MemoryStream. WARNING:
    //     Some stream (like UnmanagedMemoryStream) return CanSeek = true but are not seekable.
    //     Prefer using ToMemoryStream() to be 100% safe.
    //
    // Parameters:
    //   stream:
    //     A stream
    //
    // Returns:
    //     A seekable stream (original if seekable, a MemoryStream copy of stream else)
    public static Stream ToSeekable(this Stream stream)
    {
        if (!stream.CanSeek)
            return stream.ToMemoryStream();

        return stream;
    }
}
