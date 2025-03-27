
namespace P42.Utils.Uno;

/// <summary>
/// Wrapped around WinUI3's FontFamily to:
/// 1) Enable use of fonts stored as Assets in libraries (addresses issue with iOS inconsistency?).  This may no longer be necessary.
/// 2) Enable use of fonts stored as EmbeddedResources.  THIS REQUIRES new FontFamily() SUPPORT OF "ms-appdata://", WHICH HAS NOT YET BEEN ENABLED IN UNO.
/// </summary>
/// <param name="familyName"></param>
public class FontFamily(string familyName) : Microsoft.UI.Xaml.Media.FontFamily(AssetExtensions.AssetPath(familyName));
