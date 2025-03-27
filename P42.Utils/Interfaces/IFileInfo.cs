
using System;

namespace P42.Utils;

[Obsolete("Please contact developer is this mthod is used.  To be reomved in the future")]
public interface IFileInfo : IFileSystemInfo
{
    [Obsolete("Please contact developer is this mthod is used.  To be reomved in the future")]
    IDirectoryInfo Directory { get; }

    [Obsolete("Please contact developer is this mthod is used.  To be reomved in the future")]
    string DirectoryName { get; }

    [Obsolete("Please contact developer is this mthod is used.  To be reomved in the future")]
    bool IsReadOnly { get; set; }

    [Obsolete("Please contact developer is this mthod is used.  To be reomved in the future")]
    long Length { get; }

    [Obsolete("Please contact developer is this mthod is used.  To be reomved in the future")]
    IFileInfo CopyTo ( string destFileName, bool overwrite=false );

    [Obsolete("Please contact developer is this mthod is used.  To be reomved in the future")]
    void MoveTo ( string destFileName );


}
