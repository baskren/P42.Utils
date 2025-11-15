using Microsoft.UI.Xaml.Media.Imaging;
using static P42.Utils.LocalData;

namespace P42.Utils.Uno;


// ReSharper disable once UnusedType.Global
// ReSharper disable once InconsistentNaming
public static class  LocalData_ImageSource_Extensions
{
    /// <summary>
    /// Get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ImageSource? RecallImageSource(this Item item)
        => GetItemImageSource(item);

    /// <summary>
    /// Get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="source"></param>
    /// <returns>true upon success</returns>
    public static bool TryRecallImageSource(this Item item, out ImageSource? source)
    {
        try
        {
            source = GetItemImageSource(item);
            return source != null;
        }
        catch (Exception)
        {
            source = null;
            return false; 
        }
    }

    /// <summary>
    /// Get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<ImageSource?> AssureExistsImageSourceAsync(this AsynchronousSourcedItem item)
    {
        await item.AssureExistsAsync();
        return await item.GetImageSourceAsync();
    }

    /// <summary>
    /// Get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ImageSource? AssureExistsImageSource(this SynchronousSourcedItem item)
    {
        item.AssureExists();
        return item.GetImageSource();
    }


    /// <summary>
    /// Try get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns>null on fail</returns>
    public static async Task<(bool success, ImageSource? imageSource)> TryAssureExistsImageSourceAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            var imageSource = await item.AssureExistsImageSourceAsync();
            return (imageSource != null, imageSource);
        }
        catch (Exception)
        {
            // ignored
        }

        return (false, null);
    }

    /// <summary>
    /// Try get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns>null on fail</returns>
    public static (bool success, ImageSource? imageSource) TryAssureExistsImageSource(this SynchronousSourcedItem item)
    {
        try
        {
            var imageSource = item.AssureExistsImageSource();
            return (imageSource != null, imageSource);
        }
        catch (Exception)
        {
            // ignored
        }

        return (false, null);
    }


    public static ImageSource? GetImageSource(this Item item)
        => GetItemImageSource(item);


    private static async Task<ImageSource?> GetImageSourceAsync(this Item item)
    {
        if (item.FullPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            item.FullPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
            item.FullPath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
        {
            await using var bitmapStream = item.Stream(FileMode.Open);
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(bitmapStream.AsRandomAccessStream());
            return bitmapImage;
        }

        if (!item.FullPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            return null;

        await using var stream = item.Stream(FileMode.Open);
        var svgImageSource = new SvgImageSource();
        await svgImageSource.SetSourceAsync(stream.AsRandomAccessStream());
        return svgImageSource;

    }
    
    private static ImageSource? GetItemImageSource(Item item)
        => MainThread.Invoke(async () => await GetImageSourceAsync(item));
   
}
