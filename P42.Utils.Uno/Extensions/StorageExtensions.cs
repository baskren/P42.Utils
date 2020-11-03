using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Search;
using Windows.Storage;

namespace P42.Utils.Uno
{
    public static class StorageExtensions
    {
        public static async Task<string> FolderTree(IStorageFolder folder, int depth = 1)
        {
            string result = "";
            var subfolders = await folder.GetFoldersAsync();
            foreach (var subFolder in subfolders)
            {
                result += new string('\t', depth) + subFolder.Name + "/\n";
                result += await FolderTree(subFolder, depth + 1);
            }
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
                result += new string('\t', depth) + file.Name + "\n";
            return result;
        }

    }
}
