using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage.Streams;

namespace P42.Utils.Uno
{ 
    static class ImageSourceExtensions
    {
        public static async Task<ImageSource> GetImageSourceFromEmbeddedResource(string resourceId, Assembly assembly = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resourceId))
                    return null;

                assembly = assembly ?? EmbeddedResourceExtensions.FindAssemblyForResourceId(resourceId, assembly);
                if (assembly == null)
                    return null;

                using (var resourceStream = EmbeddedResourceExtensions.FindStreamForResourceId(resourceId, assembly))
                {
                    if (resourceStream is null)
                        return null;

                    if (resourceId.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        resourceId.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        resourceId.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var stream = resourceStream.AsRandomAccessStream())
                        {
                            var bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(stream);
                            return bitmapImage;
                        }
                    }
                    else if (resourceId.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var stream = resourceStream.AsRandomAccessStream())
                        {
                            var svgImage = new SvgImageSource();
                            await svgImage.SetSourceAsync(stream);
                            return svgImage;
                        }
                    }
                    else
                        throw new ArgumentException($"Invalid file type [{resourceId}]");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ImageSourceExtensions.GetImageSourceFromEmbeddedResource : EXCEPTION {ex}");
            }
            return null;
        }
    }
}
