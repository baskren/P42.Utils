//using DWrite;

//TODO: This needs attention in general.  Need to first figure out how to do "ms-appdata://" font in UNO

namespace P42.Utils.Uno;

[Obsolete("Has not been updated to be useful with modern versions of UNO")]
public static class AvailableFonts
{
#if __ANDROID__
    //private const string AssetPrefix = "ms-appx:///Assets/";
    private static readonly Dictionary<string, Microsoft.UI.Xaml.Media.FontFamily> FontFamiliesByName = new (){ {"default", new Microsoft.UI.Xaml.Media.FontFamily(null) } };

    static AvailableFonts()
    {

        if (Android.App.Application.Context.Assets is not{} assets)
            return;
    
        if (assets.List("Fonts") is not { } files)
            return;
        
        foreach (var file in files)
        {
            var suffix = Path.GetExtension(file).ToLower();
            if (suffix is not (".ttf" or ".otf"))
                continue;

            var fileName = Path.GetFileName(file);
            // ReSharper disable StringLiteralTypo
            if (fileName == "uno-fluentui-assets")
                fileName = "Symbols";
            // ReSharper restore StringLiteralTypo
            FontFamiliesByName.Add(fileName, new Microsoft.UI.Xaml.Media.FontFamily(file));
        }
    }
#endif

    [Obsolete("Has not been updated to be useful with modern versions of UNO")]
    public static string[] Names
    {
        get
        {
#if __ANDROID__
                return FontFamiliesByName.Keys.ToArray();
#elif __IOS__
            return UIKit.UIFont.FamilyNames;
#elif __MACOS__
                return AppKit.NSFontManager.SharedFontManager.AvailableFontFamilies;
#elif __WASM__
            // ReSharper disable StringLiteralTypo
            return [ 
                // Sans-Serif Fonts
                "Arial",
                "Helvetica",
                "Verdana",
                "Tahoma",
                "Trebuchet MS",
                "Gill Sans",

                // Serif Fonts
                "Times New Roman",
                "Georgia",
                "Garamond",
                "Palatino Linotype",
                "Palatino",
                "Book Antiqua",

                // Monospace Fonts
                "Courier New",
                "Lucida Console",
                "Consolas",
                "Monaco",
                "Andale Mono",

                // Cursive Fonts
                "Comic Sans MS",
                "Brush Script MT",
            ];
            // ReSharper restore StringLiteralTypo
#elif WINDOWS

            List<string> fontNames = new List<string>();

            // Use CanvasFontSet to get all installed fonts
            var fontSet = Microsoft.Graphics.Canvas.Text.CanvasFontSet.GetSystemFontSet();
            foreach (Microsoft.Graphics.Canvas.Text.CanvasFontFace font in fontSet.Fonts)
            {
                foreach (var fontName in font.FamilyNames)
                    if (!fontNames.Contains(fontName.Value))
                    fontNames.Add(fontName.Value);
            }

            return fontNames.ToArray();

#else
            throw new NotImplementedException();
#endif
        }
    }

    /// <summary>
    /// For OS pre-installed fonts, use the Font Name.  Examples:
    /// - iOS: Baskerville-Bold for the bold font rendition in the Baskerville font family
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [Obsolete("Has not been updated to be useful with modern versions of UNO", true)]
    public static Microsoft.UI.Xaml.Media.FontFamily FontFamily(string name)
    {
#if __ANDROID__
        return FontFamiliesByName.TryGetValue(name, out var family) ? family : new Microsoft.UI.Xaml.Media.FontFamily(name);
#else
        return new Microsoft.UI.Xaml.Media.FontFamily(name);
#endif
    }


}
