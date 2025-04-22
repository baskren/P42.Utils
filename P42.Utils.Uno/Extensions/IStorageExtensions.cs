using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace P42.Utils.Uno;

public static class IStorageExtensions
{
    /// <summary>
    /// Human-readable folder tree
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string FolderTree(this IStorageFolder folder)
        => DirectoryExtensions.FolderTree(folder.Path);
    
    /// <summary>
    /// Clear the contents of an IStorageFolder
    /// </summary>
    /// <param name="parentFolder"></param>
    /// <returns></returns>
    public static async Task DeleteChildrenAsync(this IStorageFolder parentFolder)
    {
        var items = await parentFolder.GetItemsAsync();
        foreach (var child in items)
            await child.DeleteAsync();
    }
}
