using System;

namespace P42.Utils;

[Obsolete("Please contact developer is this method is used.  To be removed in the future")]
public interface IFileSystemInfo
{
    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    bool Exists {
        get;
    }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    string Extension {
        get;
    }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    string FullName {
        get;
    }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    string Name {
        get;
    }

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    void Delete();

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    int GetHashCode();

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    string ToString();

    [Obsolete("Please contact developer is this method is used.  To be removed in the future")]
    bool Equals(object obj);
    
}
