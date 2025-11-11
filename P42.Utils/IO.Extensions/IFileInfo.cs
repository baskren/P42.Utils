
namespace P42.Utils;

[Obsolete("Please contact developer is this method is used.  To be removed in the future")]
public interface IFileInfo : IFileSystemInfo
{
    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IDirectoryInfo Directory { get; }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    string DirectoryName { get; }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    bool IsReadOnly { get; set; }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    long Length { get; }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    IFileInfo CopyTo ( string destFileName, bool overwrite=false );

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    void MoveTo ( string destFileName );


}
