namespace P42.Utils;

[Obsolete("Please contact developer is this method is used.  To be removed in the future")]
public interface IDirectoryInfo : IFileSystemInfo
{
    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IDirectoryInfo Get(string path);

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IDirectoryInfo Parent {
        get;
    }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IDirectoryInfo Root {
        get;
    }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IDirectoryInfo CreateSubdirectory(string path);

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    void Delete(bool recursive);

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IDirectoryInfo[] GetDirectories ( string searchPattern="");

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IFileInfo[] GetFiles ( string searchPattern="");

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    void MoveTo ( string destDirName );
}
