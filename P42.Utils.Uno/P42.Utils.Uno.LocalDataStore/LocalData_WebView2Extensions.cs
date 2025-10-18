
namespace P42.Utils;
// ReSharper disable once UnusedType.Global
public static class LocalData_WebView2Extensions
{
    /// <summary>
    /// Presents the Item as a WebView2.Source Uri
    /// NOTE: If Item is a .zip, .tar, .tag.gz, or .tgz file, it will unpackage the file and return the default HTML file 
    /// </summary>
    /// <param name="searchPatterns">files names, searched in package, to be html source.  Default: ["index.html", "default.html", "index.htm", "default.htm", "*.html", "*.htm"]</param>
    /// <returns></returns>
    public static async Task SetSourceAsync(this WebView2 webView, LocalData.AsynchronousSourcedItem item, params string[] searchPatterns)
    {
#if ANDROID
        if (VisualTreeHelper.GetChildren<Android.Webkit.WebView>(webView).FirstOrDefault() is { } droidWebView)
        {
            droidWebView.Settings.AllowContentAccess = true;
            droidWebView.Settings.AllowFileAccessFromFileURLs = true;
            droidWebView.Settings.AllowUniversalAccessFromFileURLs = true;
            droidWebView.Settings.AllowFileAccess = true;
        }
#endif
        await item.TryAssurePulledAsync();
        webView.Source = await item.AsWebViewSourceAsync(searchPatterns);
    }


}
