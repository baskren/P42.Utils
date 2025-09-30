using System;
using System.Threading.Tasks;

#if NET7_0_OR_GREATER
using System.IO.IsolatedStorage;
using System.IO;

namespace P42.Utils;

internal static class IsolatedStorageExtensions

{
    // https://stackoverflow.com/questions/7116329/does-anyone-use-nets-system-io-isolatedstorage

    [Obsolete("Use P42.Utils.LocalData instead", true)]
    public static Task<string> ReadAllTextAsync(this IsolatedStorageFile storage, string path)
        => throw new NotImplementedException();
    
    [Obsolete("Use P42.Utils.LocalData instead", true)]
    public static Task WriteAllTextAsync(this IsolatedStorageFile storage, string path, string contents)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData instead", true)]
    public static string ReadAllText(this IsolatedStorageFile storage, string path)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData instead", true)]
    public static void WriteAllText(this IsolatedStorageFile storage, string path, string contents)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData instead", true)]
    public static StreamReader StreamReader(this IsolatedStorageFile storage, string path)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData instead", true)]
    public static StreamWriter StreamWriter(this IsolatedStorageFile storage, string path)
        => throw new NotImplementedException();
    
}
#endif
