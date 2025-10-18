using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace P42.Utils.Uno;

[Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.ImageSource instead.")]
public static class ImageSourceExtensions
{
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.ImageSource instead.")]
    public static ImageSource? GetImageSourceFromEmbeddedResource(string resourceId, Assembly? assembly = null)
        => throw new NotImplementedException();
}
