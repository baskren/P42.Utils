using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace P42.Utils;

public static class HttpResponseExtensions
{
    /// <summary>
    /// Is the response expired?
    /// </summary>
    /// <param name="response"></param>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    public static bool IsExpiredResponse(this HttpResponseMessage response, FileSystemInfo fileSystemInfo)
    {
        if (!fileSystemInfo.Exists)
            return true;
        
        if (response.Content.Headers.LastModified.HasValue)
        {
            var lastSourceModified = response.Content.Headers.LastModified.Value;
            return lastSourceModified > fileSystemInfo.LastWriteTime;
        }
        
        if (response.Headers.CacheControl?.MaxAge.HasValue ?? false)
        {
            var maxAge = response.Headers.CacheControl.MaxAge.Value;
            var expiration = fileSystemInfo.LastWriteTime.Add(maxAge);
            return expiration < DateTime.Now;
        }
        
        return true;
    }
}
