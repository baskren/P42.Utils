using P42.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    static class FontExtensions
    {
        static bool _xamlAutoFontFamilyPresent;
        static bool _xamlAutoFontFamilyPresentSet;
        public static bool XamlAutoFontFamilyPresent
        {
            get
            {
                if (!_xamlAutoFontFamilyPresentSet)
                {
                    _xamlAutoFontFamilyPresent = Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Microsoft.UI.Xaml.Media.FontFamily", "XamlAutoFontFamily");
                    _xamlAutoFontFamilyPresentSet = true;
                }
                return _xamlAutoFontFamilyPresent;
            }
        }

        static FontFamily _defaultFontFamily;
        public static FontFamily DefaultSystemFontFamily
        {
            get
            {
                if (_defaultFontFamily == null)
                {
#if NET6_0_WINDOWS10_0_19041_0
                    if (XamlAutoFontFamilyPresent)
                        _defaultFontFamily = FontFamily.XamlAutoFontFamily;
                    else
#endif
                    //_defaultFontFamily = new FontFamily("ms-appx:///Assets/Fonts/segoeui.ttf#Segoe UI"); //new FontFamily("Segoe UI#Regular");
                    _defaultFontFamily = new FontFamily("Segoe UI#Regular");
                }
                return _defaultFontFamily;
            }
        }


        public static Dictionary<string, string> EmbeddedFontSources = new Dictionary<string, string>();

        public static FontFamily GetFontFamily(string fontFamilyName)
        {
            if (fontFamilyName == null)
                return (FontFamily)Application.Current.Resources["ContentControlThemeFontFamily"];

            if (string.IsNullOrWhiteSpace(fontFamilyName))
                return DefaultSystemFontFamily;

            string localStorageFileName = null;
            Assembly localStorageAssemby = null;
            string uri = null;
            string resourceId;

            switch (fontFamilyName.ToLower())
            {
                case "monospace":
                    return new FontFamily("Consolas");
                case "serif":
                    return new FontFamily("Cambria");
                case "sans-serif":
                    //return new FontFamily("ms-appx:///Assets/Fonts/segoeui.ttf#Segoe UI");
                    return new FontFamily("Segoe UI#Regular");
                case "stixgeneral":
                    return new FontFamily("ms-appx:///Assets/Fonts/STIXGeneral-Regular#STIXGeneral");
                    //resourceId = "Forms9Patch.Resources.Fonts.STIXGeneral.otf";
                    //fontFamilyName = resourceId + "#" + "STIXGeneral";
                    //break;
            }

            if (EmbeddedFontSources.ContainsKey(fontFamilyName))
                return new FontFamily(EmbeddedFontSources[fontFamilyName]);

            var idParts = fontFamilyName.Split('#');
            resourceId = idParts[0];

            if (localStorageFileName == null)
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
#pragma warning disable CC0039 // Don't concatenate strings in loops
                        targetAsmNameA += part + ".";
#pragma warning restore CC0039 // Don't concatenate strings in loops
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
                    localStorageAssemby = appAssembly;
                    if (localStorageFileName != null)
                        uri = EmbeddedResourceCache.ApplicationUri(resourceId, appAssembly);
                }

                // if that doesn't work, look through all known assemblies
                if (localStorageFileName == null)
                {
                    foreach (var asm in Platform.AssembliesToInclude)
                    {
                        var asmName = asm.GetName().Name;
                        if (targetAsmNameA == asmName || targetAsmNameB == asmName)
                        {
                            localStorageFileName = EmbeddedResourceCache.LocalStorageSubPathForEmbeddedResource(resourceId, asm);
                            localStorageAssemby = asm;
                            uri = EmbeddedResourceCache.ApplicationUri(resourceId, asm);
                            break;
                        }
                    }
                }
            }

            if (localStorageFileName == null)
                return new FontFamily(fontFamilyName);

            string fontName;
            if (idParts.Count() > 1)
                fontName = idParts.Last();
            else
            {
                //var cachedFilePath = System.IO.Path.Combine(P42.Utils.Environment.ApplicationDataPath, localStorageFileName);
                var cachedFilePath = System.IO.Path.Combine(P42.Utils.EmbeddedResourceCache.FolderPath(localStorageAssemby), localStorageFileName);
                fontName = TTFAnalyzer.FontFamily(cachedFilePath);
            }
            //var uwpFontFamily = "ms-appdata:///local/" + localStorageFileName.Replace('\\','/') + (string.IsNullOrWhiteSpace(fontName) ? null : "#" + fontName);
            var uwpFontFamily = uri + (string.IsNullOrWhiteSpace(fontName) ? null : "#" + fontName);
            //var uwpFontFamily = "ms-appdata:///local/EmbeddedResourceCache/02fe60e0da81514d145d946ab9ad9b97#Pacifico";
            //foreach (var c in uwpFontFamily)
            //    System.Diagnostics.Debug.WriteLine("c=["+c+"]["+(int)c+"]");
            EmbeddedFontSources.Add(fontFamilyName, uwpFontFamily);
            return new FontFamily(uwpFontFamily);
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
}
