using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;

namespace P42.Utils;

/// <summary>
/// IsolatedStorage Extensions
/// </summary>
static class IsolatedStorageExtensions
{
    /// <summary>
    /// Read text in file in IsolatedStorage
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="fileName"></param>
    /// <returns>Task with contents of the file</returns>
    public static async Task<string> ReadTextAsync(this IsolatedStorageFile storage, string fileName)
    {
        using var reader = storage.StreamReader(fileName);
        if (reader is null)
            return string.Empty;
        
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Writes text to a file, overwriting any existing data
    /// </summary>
    /// <param name="storage">an IsolatedStorageFile</param>
    /// <param name="fileName">file name</param>
    /// <param name="text">text to write</param>
    /// <returns>A task which completes when the write operation finishes</returns>
    public static async Task WriteTextAsync(this IsolatedStorageFile storage, string fileName, string text)
    {
        await using var stream = storage.CreateFile(fileName);
        await using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        await memoryStream.CopyToAsync(stream);
    }

    /// <summary>
    /// Read text in file in IsolatedStorage
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="fileName"></param>
    /// <returns>text (string.Empty on fail)</returns>
    public static string ReadText(this IsolatedStorageFile storage, string fileName)
    {
        using var reader = storage.StreamReader(fileName);
        return reader is not null 
            ? reader.ReadToEnd() 
            : string.Empty;
    }

    /// <summary>
    /// Writes text to file in IsolatedStorage
    /// </summary>
    /// <param name="storage">an IsolatedStorageFile</param>
    /// <param name="fileName">file name</param>
    /// <param name="text">text to write</param>
    public static void WriteText(this IsolatedStorageFile storage, string fileName, string text)
    {
        using var stream = storage.CreateFile(fileName);
        using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        memoryStream.WriteTo(stream);
    }

    /// <summary>
    /// Creates a StreamReader for a file (fileName) in IsolatedStorage
    /// </summary>
    /// <param name="storage">an IsolatedStorage</param>
    /// <param name="fileName">file name</param>
    /// <returns>StreamReader or null on fail</returns>
    public static StreamReader? StreamReader(this IsolatedStorageFile storage, string fileName)
    {
        if (!storage.FileExists(fileName))
            return null;
            
        if (storage.OpenFile(fileName, FileMode.Open) is not { } stream)
            return null;

        if (stream.CanRead)
            return new StreamReader(stream);
            
        stream.Dispose();
        return null;
    }

    /// <summary>
    /// Creates a new file (fileName) in IsolatedStorage and returns StreamWriter for it 
    /// </summary>
    /// <param name="storage">an IsolatedStorageFile</param>
    /// <param name="fileName">file name</param>
    /// <returns>StreamWriter or null on fail</returns>
    public static StreamWriter? StreamWriter(this IsolatedStorageFile storage, string fileName)
    {
        if (storage.CreateFile(fileName) is not { } stream)
            return null;
            
        if (stream.CanWrite)
            return new StreamWriter(stream);
            
        stream.Dispose();
        return null;
    }
}
