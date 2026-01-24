using Microsoft.UI.Xaml.Media.Imaging;
using static P42.Utils.LocalData;

namespace P42.Utils.Uno;


// ReSharper disable once UnusedType.Global
// ReSharper disable once InconsistentNaming
public static class  LocalData_ImageSource_Extensions
{
    /// <param name="item"></param>
    extension(Item item)
    {
        /// <summary>
        /// Get ImageSource from item in local data store
        /// </summary>
        /// <returns></returns>
        public ImageSource? RecallImageSource()
            =>
                item.GetItemImageSource();

        /// <summary>
        /// Get ImageSource from item in local data store
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true upon success</returns>
        public bool TryRecallImageSource(out ImageSource? source)
        {
            try
            {
                source = item.GetItemImageSource();
                return source != null;
            }
            catch (Exception)
            {
                source = null;
                return false; 
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public ImageSource? GetImageSource()
            =>
                item.GetItemImageSource();

        private async Task<ImageSource?> GetImageSourceAsync()
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
        
        private ImageSource? GetItemImageSource()
            => MainThread.Invoke(async () => await item.GetImageSourceAsync());

    }


    /// <param name="item"></param>
    extension(AsynchronousSourcedItem item)
    {
        /// <summary>
        /// Get ImageSource from item in local data store
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<ImageSource?> AssureExistsImageSourceAsync()
        {
            await item.AssureExistsAsync();
            return await item.GetImageSourceAsync();
        }

        /// <summary>
        /// Try get ImageSource from item in local data store
        /// </summary>
        /// <returns>null on fail</returns>
        public async Task<(bool success, ImageSource? imageSource)> TryAssureExistsImageSourceAsync()
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
    }

    /// <param name="item"></param>
    extension(SynchronousSourcedItem item)
    {
        /// <summary>
        /// Get ImageSource from item in local data store
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public ImageSource? AssureExistsImageSource()
        {
            item.AssureExists();
            return item.GetImageSource();
        }

        /// <summary>
        /// Try get ImageSource from item in local data store
        /// </summary>
        /// <returns>null on fail</returns>
        public (bool success, ImageSource? imageSource) TryAssureExistsImageSource()
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
    }
}
