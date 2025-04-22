using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using static P42.Utils.LocalData;

namespace P42.Utils.Uno;


public static class LocalData_ImageSource_Extensions
{
    /// <summary>
    /// Get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ImageSource RecallImageSource(this Item item)
        => GetItemImageSource(item);

    public static bool TryRecallImageSource(this Item item, [MaybeNullWhen(false)] out ImageSource source)
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
    public static async Task<ImageSource> AssureSourcedImageSourceAsync(this AsynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return item.RecallImageSource();

        await item.ForcePullAsync();
        return item.RecallImageSource();
    }

    /// <summary>
    /// Get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ImageSource AssureSourcedImageSource(this SynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return item.RecallImageSource();

        item.AssureSourcedText();
        return item.RecallImageSource();
    }


    /// <summary>
    /// Try get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns>null on fail</returns>
    public static async Task<ImageSource?> TryAssureSourcedImageSourceAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            return await item.AssureSourcedImageSourceAsync();
        }
        catch (Exception) { }

        return null;
    }

    /// <summary>
    /// Try get ImageSource from item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns>null on fail</returns>
    public static ImageSource? TryAssureSourcedImageSource(this SynchronousSourcedItem item)
    {
        try
        {
            return item.AssureSourcedImageSource();
        }
        catch (Exception) { }

        return null;
    }





    private static ImageSource GetItemImageSource(Item item)
    {
        
        using var stream = item.Stream(FileMode.Open);

        try
        {
            if (item.FullPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                item.FullPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                item.FullPath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream.AsRandomAccessStream());
                return bitmapImage;
            }

            if (item.FullPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                return MainThread.Invoke(async () =>
                {
                    var svgImageSource = new SvgImageSource();
                    await svgImageSource.SetSourceAsync(stream.AsRandomAccessStream());
                    return svgImageSource;
                });
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            stream.Dispose();
        }

        throw new ArgumentException($"Invalid ItemKey ItemSource [{item}] for ImageSource");
    }

}
