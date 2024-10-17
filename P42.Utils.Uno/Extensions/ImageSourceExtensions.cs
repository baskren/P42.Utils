using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace P42.Utils.Uno;

public static class ImageSourceExtensions
{
    /// <summary>
    /// Creates ImageSource from EmbeddedResource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <returns>null if fail</returns>
    /// <exception cref="ArgumentException"></exception>
    public static ImageSource? GetImageSourceFromEmbeddedResource(string resourceId, Assembly? assembly = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                return null;

            assembly ??= EmbeddedResourceExtensions.FindAssemblyForResourceId(resourceId, assembly);
            if (assembly == null)
                return null;

            using var resourceStream = EmbeddedResourceExtensions.FindStreamForResourceId(resourceId, assembly);
            if (resourceStream is null)
                return null;

            if (resourceId.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                resourceId.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                resourceId.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                using var imageStream = resourceStream.AsRandomAccessStream();
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(imageStream);
                return bitmapImage;
            }

            if (!resourceId.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid file type [{resourceId}]");

            var stream = resourceStream.AsRandomAccessStream();
            var svgImageSource = new SvgImageSource();
            MainThread.InvokeOnMainThread(async () =>
            {
                await svgImageSource.SetSourceAsync(stream);
                stream.Dispose();
            });
            return svgImageSource;

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ImageSourceExtensions.GetImageSourceFromEmbeddedResource : EXCEPTION {ex}");
        }
        
        return null;
    }
    
    /// <summary>
    /// Asynchronously Creates ImageSource from EmbeddedResource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly">optional</param>
    /// <returns>null if fail</returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<ImageSource?> GetImageSourceFromEmbeddedResourceAsync(string resourceId, Assembly? assembly = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                return null;

            assembly ??= EmbeddedResourceExtensions.FindAssemblyForResourceId(resourceId, assembly);
            if (assembly == null)
                return null;

            await using var resourceStream = EmbeddedResourceExtensions.FindStreamForResourceId(resourceId, assembly);
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

            if (!resourceId.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid file type [{resourceId}]");

            var svgImageSource = new SvgImageSource();
            await MainThread.InvokeOnMainThreadAsync(CreateSvgImageSource(resourceStream));
            return svgImageSource;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ImageSourceExtensions.GetImageSourceFromEmbeddedResource : EXCEPTION {ex}");
        }
        
        return null;
        
        async Task<SvgImageSource> CreateSvgImageSource(Stream resourceStream)
        {
            using var stream = resourceStream.AsRandomAccessStream();
            var svgImageSource = new SvgImageSource();
            await svgImageSource.SetSourceAsync(stream);
            return svgImageSource;
        }
        
    }

}
