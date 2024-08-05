using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace P42.Utils.Uno
{ 
    public static class ImageSourceExtensions
    {
        public static ImageSource GetImageSourceFromEmbeddedResource(string resourceId, Assembly assembly = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resourceId))
                    return null;

                assembly = assembly ?? EmbeddedResourceExtensions.FindAssemblyForResourceId(resourceId, assembly);
                if (assembly == null)
                    return null;

                using var resourceStream = EmbeddedResourceExtensions.FindStreamForResourceId(resourceId, assembly);
                if (resourceStream is null)
                    return null;

                if (resourceId.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    resourceId.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    resourceId.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = resourceStream.AsRandomAccessStream();
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(stream);
                    return bitmapImage;
                }
                else if (resourceId.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = resourceStream.AsRandomAccessStream();
                    var svgImageSource = new SvgImageSource();
                    Task.Run(() =>
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await svgImageSource.SetSourceAsync(stream);
                        });
                    });
                    return svgImageSource;
                }
                else
                    throw new ArgumentException($"Invalid file type [{resourceId}]");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ImageSourceExtensions.GetImageSourceFromEmbeddedResource : EXCEPTION {ex}");
            }
            return null;
        }
    }
}
