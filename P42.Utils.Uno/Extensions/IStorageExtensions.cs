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
    public static string FolderTree(IStorageFolder folder)
        => DirectoryExtensions.FolderTree(folder.Path);
    
}
