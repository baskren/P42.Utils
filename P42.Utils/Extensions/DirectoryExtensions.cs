using System;
using System.IO;

namespace P42.Utils
{
    public static class DirectoryExtensions
    {
        public static DirectoryInfo AssureExists(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException(nameof(fullPath));
            DirectoryInfo info = Directory.Exists(fullPath)
                ? info = new DirectoryInfo(fullPath)
                : info = Directory.CreateDirectory(fullPath);
            if (!info.Exists)
                throw new Exception("Could not assure existence of directory [" + info + "]");
            return info;
        }
    }
}
