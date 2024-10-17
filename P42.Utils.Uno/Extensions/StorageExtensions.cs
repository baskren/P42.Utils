using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace P42.Utils.Uno;

/// <summary>
/// Windows.Storage.IStorageFolder extensions
/// </summary>
public static class StorageExtensions
{
    /// <summary>
    /// Generates tree of content found in IStorageFolder
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="depth">tree depth</param>
    /// <returns>tree</returns>
    public static async Task<string> FolderTreeAsync(this IStorageFolder folder, int depth = 1)
    {
        var result = new StringBuilder();
        var subfolders = await folder.GetFoldersAsync();
        foreach (var subFolder in subfolders)
        {
            result.Append(new string('\t', depth) + subFolder.Name + "/\n");
            result.Append(await FolderTreeAsync(subFolder, depth + 1));
        }
        var files = await folder.GetFilesAsync();
        return files.Aggregate(result.ToString(), (current, file) => current + new string('\t', depth) + file.Name + "\n");
    }

}
