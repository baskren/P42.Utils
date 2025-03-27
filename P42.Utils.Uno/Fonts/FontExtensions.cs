using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

internal static class FontExtensions
{
    private static bool _xamlAutoFontFamilyPresent;
    private static bool _xamlAutoFontFamilyPresentSet;
    public static bool XamlAutoFontFamilyPresent
    {
        get
        {
            if (_xamlAutoFontFamilyPresentSet)
                return _xamlAutoFontFamilyPresent;

            _xamlAutoFontFamilyPresent = Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Microsoft.UI.Xaml.Media.FontFamily", nameof(Microsoft.UI.Xaml.Media.FontFamily.XamlAutoFontFamily));
            _xamlAutoFontFamilyPresentSet = true;
            return _xamlAutoFontFamilyPresent;
        }
    }

    public static readonly Microsoft.UI.Xaml.Media.FontFamily DefaultSystemFontFamily = Platform.SansSerifFontFamily;

    public static readonly Dictionary<string, string> EmbeddedFontSources = new();
    
    private const string InvalidAssemblyName = "invalid_assembly_name";
    private const string Resources = "Resources";
    private const string FontCache = "P42.Utils.Uno.FontCache";
    
    /// <summary>
    /// Get a FontFamily from a string identifier 
    /// </summary>
    /// <param name="fontFamilyIdentifier"></param>
    /// <returns></returns>
    public static Microsoft.UI.Xaml.Media.FontFamily GetFontFamily(string? fontFamilyIdentifier)
    {
        if (string.IsNullOrWhiteSpace(fontFamilyIdentifier))
            return DefaultSystemFontFamily;

        string? localStoragePath = null;
        string? uri = null;

        switch (fontFamilyIdentifier.ToLower())
        {
            case "monospace":
                return Platform.MonoSpaceFontFamily;
            case "serif":
                return Platform.SerifFontFamily;
            case "sans-serif":
                return Platform.SansSerifFontFamily;
            case "stixgeneral":
            case "math":
                return Platform.MathFontFamily;
        }

        if (EmbeddedFontSources.TryGetValue(fontFamilyIdentifier, out var source))
            return new Microsoft.UI.Xaml.Media.FontFamily(source);

        var idParts = fontFamilyIdentifier.Split('#');
        var resourceId = idParts[0];
        var fontFamilyName = idParts.Length < 1 ? idParts.Last() : string.Empty;
        var appAssembly = Application.Current.GetType().Assembly;

        if (localStoragePath == null)
        {
            var targetAsmNameA = InvalidAssemblyName;

            // we've got to go hunting for this ... and UWP doesn't give us much help
            // first, try the main assembly!
            var targetParts = fontFamilyIdentifier.Split('.');
            if (targetParts.Contains(Resources))
            {
                targetAsmNameA = targetParts.TakeWhile(part => part != Resources).Aggregate("", (current, part) => current + $"{part}.");
                if (targetAsmNameA.Length > 0)
                    targetAsmNameA = targetAsmNameA[..^1];
            } 
            var targetAsmNameB = targetParts.First();

            var appAsmName = appAssembly.GetName().Name;
            if (targetAsmNameA == appAsmName || targetAsmNameB == appAsmName)
            {
                var item = P42.Utils.LocalData.ResourceItemKey.Get(resourceId, FontCache, appAssembly);
                if (item.TryRecallOrPullItem())
                {
                    localStoragePath = item.Path;
                    uri = item.AppDataUrl;
                }
            }

            // if that doesn't work, look through all known assemblies
            if (localStoragePath == null)
            {
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    var asmName = asm.GetName().Name;
                    if (asmName != targetAsmNameA && asmName != targetAsmNameB)
                        continue;

                    var item =  P42.Utils.LocalData.ResourceItemKey.Get(resourceId, FontCache, asm);
                    if (!item.TryRecallOrPullItem())
                        continue;

                    localStoragePath = item.Path;
                    uri = item.AppDataUrl;
                    break;
                }
            }
        }

        if (localStoragePath == null)
            return new Microsoft.UI.Xaml.Media.FontFamily(fontFamilyIdentifier);

        if (string.IsNullOrWhiteSpace(fontFamilyName))
            fontFamilyName = TtfAnalyzer.FontFamily(localStoragePath);
        //var uwpFontFamily = "ms-appdata:///local/" + localStorageFileName.Replace('\\','/') + (string.IsNullOrWhiteSpace(fontName) ? null : "#" + fontName);
        var uwpFontFamily = uri + (string.IsNullOrWhiteSpace(fontFamilyName) ? null : $"#{fontFamilyName}");
        EmbeddedFontSources.Add(fontFamilyIdentifier, uwpFontFamily);
        return new Microsoft.UI.Xaml.Media.FontFamily(uwpFontFamily);
    }

    /*
    public static double LineHeight(string fontFamily, double fontSize, FontStyle fontStyle)
    {
        var dxFont = FontExtensions.GetDxFont(fontFamily, FontWeight.Normal, FontStretch.Normal, fontAttributes.ToDxFontStyle());
        //return FontExtensions.LineHeightForFontSize(fontSize);
        return dxFont.Metrics.HeightForLinesAtFontSize(1, fontSize);
    }

    public static double LineSpace(string fontFamily, double fontSize, FontAttributes fontAttributes)
    {
        return 0;
    }
    */

}
