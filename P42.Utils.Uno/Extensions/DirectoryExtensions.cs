using System.Linq;

namespace P42.Utils.Uno;

public static class DirectoryExtensions
{
    /// <summary>
    /// Generate folder tree a current path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public static string FolderTree(string path, int depth = 1)
    {
        var result = string.Empty;
        var dirs = System.IO.Directory.GetDirectories(path);
        foreach (var dir in dirs)
        {
            result += dir + " : \n";
            result += FolderTree(dir, depth + 1); 
        }
        var files = System.IO.Directory.GetFiles(path);
        return files.Aggregate(result, (current, file) => current + file + "\n");
    }
}
