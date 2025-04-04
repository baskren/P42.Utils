using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

namespace P42.Utils.Uno;

public class LocalDataImageSource : LocalData<ImageSource>
{
    internal static LocalDataImageSource Instance { get; } = new();
    
    /// <summary>
    /// Tries to get item out of local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>false if item is not already in local data store</returns>
    public override bool TryRecallItem(out ImageSource? item, ItemKey key)
    {
        item = null;
        if (!Stream.TryRecallItem(out var stream, key) || stream is null)
            return false;

        try
        {
            item = GetItemSource(stream, key);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            stream.Dispose();
        }
        
    }

    /// <summary>
    /// Gets or downloads item to LocalData store and returns a ImageSource pointing to it 
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null if not available</returns>
    public override async Task<ImageSource?> RecallOrPullItemAsync(ItemKey key)
    => await Stream.RecallOrPullItemAsync(key) is { } stream
            ? GetItemSource(stream, key)
            : null;
    

    private ImageSource GetItemSource(Stream stream, ItemKey key)
    {
        if (key.FullPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            key.FullPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
            key.FullPath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream.AsRandomAccessStream());
            return bitmapImage;
        }

        if (key.FullPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            return MainThread.Invoke(async() =>
            {
                var svgImageSource = new SvgImageSource();
                await svgImageSource.SetSourceAsync(stream.AsRandomAccessStream());
                return svgImageSource; 
            });
        }

        throw new ArgumentException($"Invalid ItemKey ItemSource [{key}] for ImageSource");
    }
    
    [Obsolete("METHOD DOES NOT MAKE SENSE IN THE CASE OF ImageSource", true)]
    public override void StoreItem(ImageSource? sourceItem, ItemKey key, bool wipeOld = true)
    {
        throw new NotImplementedException();
    }

    [Obsolete("METHOD DOES NOT MAKE SENSE IN THE CASE OF ImageSource", true)]
    public override Task StoreItemAsync(ImageSource? sourceItem, ItemKey key, bool wipeOld = true)
    {
        throw new NotImplementedException();
    }
}
