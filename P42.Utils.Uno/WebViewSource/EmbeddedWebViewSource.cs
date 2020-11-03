using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace P42.Utils.Uno
{
    public class EmbeddedWebViewSource
    {
        public string Html { get; private set; }

        public string Path { get; private set; }

        public Assembly Assembly { get; private set; }
        public string FolderId { get; private set; }
        public string StartPageId { get; private set; }

        public EmbeddedWebViewSource(Assembly assembly, string folderId, string startPageId = null)
        {
            if (assembly is null)
                throw new ArgumentException("Assembly cannot be null");
            Assembly = assembly;

            var ids = Assembly.GetManifestResourceNames();
            if (!ids.Any())
                throw new ArgumentException("There are no embedded resources in assembly [" + Assembly + "]");

            if (folderId?.EndsWith(".") ?? false)
                folderId.Trim('.');
            FolderId = folderId;

            if (!string.IsNullOrEmpty(folderId))
            {
                folderId += ".";
                if (startPageId?.StartsWith(folderId) ?? false)
                    startPageId = startPageId.Substring(folderId.Length);
            }
            if (string.IsNullOrWhiteSpace(startPageId))
            {
                foreach (var id in ids)
                {
                    if ((string.IsNullOrEmpty(folderId) || id.StartsWith(folderId)) 
                        && (id.EqualsWildcard("*.html") || id.EqualsWildcard("*.htm")))
                    {
                        if (string.IsNullOrWhiteSpace(startPageId))
                            startPageId = id.Substring(folderId.Length);
                        else
                            throw new ArgumentException("No startPageId given and there are multiple .html files in folder [" + Assembly + "][" + FolderId + "]");
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(startPageId))
                throw new ArgumentException("No startPageId given for folder [" + Assembly + "][" + FolderId + "]");
            StartPageId = startPageId;
        }

        public async Task Initialize()
        {
            var folderId = (string.IsNullOrEmpty(FolderId) ? null : FolderId + ".");
            var path = await P42.Utils.EmbeddedResourceCache.LocalStorageFullPathForEmbeddedResourceAsync(folderId + StartPageId, Assembly, FolderId);
            var html = File.ReadAllText(path);
            var resourceNames = Assembly.GetManifestResourceNames();
            foreach (var resourceId in resourceNames)
            {
                if (resourceId.StartsWith(folderId, StringComparison.Ordinal))
                {
                    var resourcePath = resourceId.Split('.');
                    var suffix = resourcePath.LastOrDefault()?.ToLower();
                    if (suffix == "png"
                        || suffix == "jpg" || suffix == "jpeg"
                        || suffix == "svg"
                        || suffix == "gif"
                        || suffix == "tif" || suffix == "tiff"
                        || suffix == "pdf"
                        || suffix == "bmp"
                        || suffix == "ico")
                    {
                        var relativeSource = '"' + resourceId.Substring(folderId.Length) + '"';

                        if (html.Contains(relativeSource))
                        {
                            try
                            {
                                byte[] bytes;
                                using (var resourceStream = Assembly.GetManifestResourceStream(resourceId))
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        resourceStream.CopyTo(memoryStream);
                                        bytes = memoryStream.ToArray();
                                    }
                                    string base64 = '"' + "data:" + (suffix == "pdf" ? "application/" : "image/") + suffix + ";base64," + Convert.ToBase64String(bytes) + '"';
                                    html = html.Replace(relativeSource, base64);
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
            Html = html;

        }


        /*
        async Task<string> HtmlResourceConvert(string id, string folderId)
        {
            var path = await P42.Utils.EmbeddedResourceCache.LocalStorageFullPathForEmbeddedResourceAsync(id, Assembly, folderId);
            var html = File.ReadAllText(path);
            var resourceNames = Assembly.GetManifestResourceNames();
            foreach (var resourceId in resourceNames)
            {
                if (resourceId.StartsWith(folderId, StringComparison.Ordinal))
                {
                    var resourcePath = resourceId.Split('.');
                    var suffix = resourcePath.LastOrDefault()?.ToLower();
                    if (suffix == "png"
                        || suffix == "jpg" || suffix == "jpeg"
                        || suffix == "svg"
                        || suffix == "gif"
                        || suffix == "tif" || suffix == "tiff"
                        || suffix == "pdf"
                        || suffix == "bmp"
                        || suffix == "ico")
                    {
                        var relativeSource = '"' + resourceId.Substring(folderId.Length + 1) + '"';

                        if (html.Contains(relativeSource))
                        {
                            try
                            {
                                byte[] bytes;
                                using (var resourceStream = Assembly.GetManifestResourceStream(resourceId))
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        resourceStream.CopyTo(memoryStream);
                                        bytes = memoryStream.ToArray();
                                    }
                                    string base64 = '"' + "data:" + (suffix == "pdf" ? "application/" : "image/") + suffix + ";base64," + Convert.ToBase64String(bytes) + '"';
                                    html = html.Replace(relativeSource, base64);
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
            Html = html;

        }
        */
    }
}
