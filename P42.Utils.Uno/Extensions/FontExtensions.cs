using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

internal static class FontExtensions
{
    private static bool _xamlAutoFontFamilyPresent;
    private static bool _xamlAutoFontFamilyPresentSet;
    
    /// <summary>
    /// Is the Microsoft.UI.Xaml.Media.FontFamily.XamlAutoFontFamily property supported by this platform?
    /// </summary>
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

    private static readonly Dictionary<string, string> EmbeddedFontSources = new ();

    /// <summary>
    /// More comprehensive way to get FontFamily
    /// </summary>
    /// <param name="fontFamilyName"></param>
    /// <returns></returns>
    public static Microsoft.UI.Xaml.Media.FontFamily GetFontFamily(string? fontFamilyName)
    {
        if (fontFamilyName == null)
            return (FontFamily)Application.Current.Resources["ContentControlThemeFontFamily"];

        if (string.IsNullOrWhiteSpace(fontFamilyName))
            return DefaultSystemFontFamily;

        if (fontFamilyName.StartsWith("ms-appx://"))
            return new FontFamily(fontFamilyName);
        
        var localStorageFileName = string.Empty;
        Assembly? localStorageAssembly = null;
        var uri = string.Empty;
        var resourceId = string.Empty;

        switch (fontFamilyName.ToLower())
        {
            case "monospace":
                return Platform.MonoSpaceFontFamily;
            case "serif":
                return new Microsoft.UI.Xaml.Media.FontFamily("Cambria");
            case "sans-serif":
                return Platform.SansSerifFontFamily;
            // ReSharper disable once StringLiteralTypo
            case "stixgeneral":
                return Platform.MathFontFamily;
            //resourceId = "Forms9Patch.Resources.Fonts.STIXGeneral.ttf";
            //fontFamilyName = resourceId + "#" + "STIXGeneral";
            //break;
        }

        if (EmbeddedFontSources.TryGetValue(fontFamilyName, out var source))
            return new Microsoft.UI.Xaml.Media.FontFamily(source);

        var idParts = fontFamilyName.Split('#');
        resourceId = idParts[0];

        if (string.IsNullOrEmpty(localStorageFileName))
        {
            // we've got to go hunting for this ... and UWP doesn't give us much help
            // first, try the main assembly!
            var targetParts = fontFamilyName.Split('.');
            var targetAsmNameA = "invalid_assembly_name";
            if (targetParts.Contains("Resources"))
            {
                targetAsmNameA = "";
                foreach (var part in targetParts)
                {
                    if (part == "Resources")
                        break;
                    targetAsmNameA += part + ".";
                }
                if (targetAsmNameA.Length > 0)
                    targetAsmNameA = targetAsmNameA.Substring(0, targetAsmNameA.Length - 1);
            }
            var targetAsmNameB = targetParts.First();

            var appAssembly = Application.Current.GetType().Assembly;
            var appAsmName = appAssembly.GetName().Name;
            if (targetAsmNameA == appAsmName || targetAsmNameB == appAsmName)
            {
                localStorageFileName = EmbeddedResourceCache.LocalStorageSubPathForEmbeddedResource(resourceId, appAssembly);
                localStorageAssembly = appAssembly;
                if (!string.IsNullOrWhiteSpace(localStorageFileName))
                    uri = EmbeddedResourceCache.ApplicationUri(resourceId, appAssembly);
            }

            // if that doesn't work, look through all known assemblies
            if (string.IsNullOrEmpty(localStorageFileName))
            {
                foreach (var asm in Platform.AssembliesToInclude)
                {
                    var asmName = asm.GetName().Name;
                    if (targetAsmNameA != asmName && targetAsmNameB != asmName)
                        continue;

                    localStorageFileName = EmbeddedResourceCache.LocalStorageSubPathForEmbeddedResource(resourceId, asm);
                    localStorageAssembly = asm;
                    uri = EmbeddedResourceCache.ApplicationUri(resourceId, asm);
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(localStorageFileName))
            return new Microsoft.UI.Xaml.Media.FontFamily(fontFamilyName);

        string fontName;
        if (idParts.Length > 1)
            fontName = idParts.Last();
        else
        {
            var cachedFilePath = System.IO.Path.Combine(EmbeddedResourceCache.FolderPath(localStorageAssembly), localStorageFileName);
            fontName = TTFAnalyzer.FontFamily(cachedFilePath);
        }
        //var uwpFontFamily = "ms-appdata:///local/" + localStorageFileName.Replace('\\','/') + (string.IsNullOrWhiteSpace(fontName) ? null : "#" + fontName);
        var uwpFontFamily = uri + (string.IsNullOrWhiteSpace(fontName) ? null : $"#{fontName}");
        EmbeddedFontSources.Add(fontFamilyName, uwpFontFamily);
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
