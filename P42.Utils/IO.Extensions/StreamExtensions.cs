using System.Text;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// Stream extensions
/// </summary>
public static class StreamExtensions
{
    /// <param name="stream"></param>
    extension(Stream stream)
    {
        /// <summary>
        /// Write stream to a file at destinationFilePath
        /// </summary>
        /// <param name="destinationFilePath"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        public void CopyToPath(string destinationFilePath, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
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

        /// <summary>
        /// Read bytes from stream
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> ReadBytesAsync()
        {
            var readBuffer = new byte[stream.CanSeek ? stream.Length - stream.Position : 4096];
            var totalBytesRead = 0;
            int num;
            while ((num = await stream.ReadAsync(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += num;
                if (totalBytesRead != readBuffer.Length)
                    continue;

                var nextBytes = new byte[1];
                if (await stream.ReadAsync(nextBytes, 0, 1) != 1)
                    continue;

                var array = new byte[readBuffer.Length * 2];
                Buffer.BlockCopy(readBuffer, 0, array, 0, readBuffer.Length);
                Buffer.SetByte(array, totalBytesRead, nextBytes[0]);
                readBuffer = array;
                totalBytesRead++;
            }

            var array2 = readBuffer;
            if (readBuffer.Length == totalBytesRead)
                return array2;

            array2 = new byte[totalBytesRead];
            Buffer.BlockCopy(readBuffer, 0, array2, 0, totalBytesRead);

            return array2;
        }

        /// <summary>
        /// Read bytes from stream
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            if (stream is { CanSeek: true, Position: 0L } and MemoryStream memoryStream)
                return memoryStream.ToArray();

            var array = new byte[stream.CanSeek ? stream.Length - stream.Position : 4096];
            var num = 0;
            int num2;
            while ((num2 = stream.Read(array, num, array.Length - num)) > 0)
            {
                num += num2;
                if (num != array.Length)
                    continue;

                var num3 = stream.ReadByte();
                if (num3 == -1)
                    continue;

                var array2 = new byte[array.Length * 2];
                Buffer.BlockCopy(array, 0, array2, 0, array.Length);
                Buffer.SetByte(array2, num, (byte)num3);
                array = array2;
                num++;
            }

            var array3 = array;
            if (array.Length == num)
                return array3;

            array3 = new byte[num];
            Buffer.BlockCopy(array, 0, array3, 0, num);

            return array3;
        }

        /// <summary>
        /// Read text from stream
        /// </summary>
        /// <returns></returns>
        public string ReadToEnd()
        {
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Read text from stream using encoding
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string ReadToEnd(Encoding encoding)
        {
            using var streamReader = new StreamReader(stream, encoding);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// does stream start with bytes?
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public bool StartsWith(byte[] start)
        {
            if (stream.CanSeek)
                stream.Position = 0L;

            var array = new byte[start.Length];
            stream.ReadExactly(array);
            return start.SequenceEqual(array);
        }

        /// <summary>
        /// Convert stream to MemoryStream
        /// </summary>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public MemoryStream ToMemoryStream()
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0L;
            return memoryStream;
        }

        /// <summary>
        /// If stream is not seekable, convert it to Memory Stream
        /// </summary>
        /// <returns></returns>
        public Stream ToSeekable()
            => !stream.CanSeek ? stream.ToMemoryStream() : stream;
    }

    //
}
