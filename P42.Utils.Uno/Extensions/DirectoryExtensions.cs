namespace P42.Utils.Uno
{
    public static class DirectoryExtensions
    {
        public static string FolderTree(string path, int depth = 1)
        {
            string result = "";
            var dirs = System.IO.Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                result += dir + " : \n";
                result += FolderTree(dir, depth + 1); 
            }
            var files = System.IO.Directory.GetFiles(path);
            foreach (var file in files)
                result += file + "\n";
            return result;
        }
    }
}
