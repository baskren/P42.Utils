
namespace P42.Utils;

public static class FileExtensions
{
    
    
    internal static async Task<string> UncompressGzAsync(string sourcePath, string? destPath = null, bool overwrite = true)
    {
        if (string.IsNullOrEmpty(destPath))
        {
            if (sourcePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                destPath = sourcePath[..^".gz".Length];
            else
                destPath = sourcePath + ".uncompressed";
        }        
        
        if (File.Exists(destPath))
        {
            if (overwrite)
                File.Delete(destPath);
            else
                throw new IOException($"destination [{destPath}] already exists and overwrite set to false");
        }

        await using var fileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
        await using var gzipStream = new System.IO.Compression.GZipStream(fileStream, System.IO.Compression.CompressionMode.Decompress);
        await using var fileStreamOut = new FileStream(destPath, FileMode.Create, FileAccess.Write);
        await gzipStream.CopyToAsync(fileStreamOut);
        return destPath;
    }

    /// <summary>
    /// Is the file binary.  Uses Git's "consecutive nulls" check
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="requiredConsecutiveNul"></param>
    /// <returns>true if binary</returns>
    public static bool IsBinary(this FileInfo fileInfo, int requiredConsecutiveNul = 1)
        => IsBinary(fileInfo.FullName, requiredConsecutiveNul);

    /// <summary>
    /// Is the file binary.  Uses Git's "consecutive nulls" check
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="requiredConsecutiveNul"></param>
    /// <returns>true if binary</returns>
    public static bool IsBinary(string filePath, int requiredConsecutiveNul = 1)
    {
        const int charsToCheck = 8000;
        const char nulChar = '\0';

        var nulCount = 0;

        using var streamReader = new StreamReader(filePath);
        for (var i = 0; i < charsToCheck; i++)
        {
            if (streamReader.EndOfStream)
                return false;

            if ((char) streamReader.Read() == nulChar)
            {
                nulCount++;

                if (nulCount >= requiredConsecutiveNul)
                    return true;
            }
            else
                nulCount = 0;
        }

        return false;
    }

    
}
