using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uno.UI;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    public static class AvailableFonts
    {
#if __ANDROID__
        const string AssetPrefix = "ms-appx:///Assets/";
        static Dictionary<string, FontFamily> FontFamiliesByName = new Dictionary<string, FontFamily> { {"default", null } };

        static AvailableFonts()
        {

            var context = Android.App.Application.Context;
            Android.Content.Res.AssetManager assets = context.Assets;
            var files = assets.List("Fonts");
            foreach (var file in files)
            {
                var suffix = System.IO.Path.GetExtension(file).ToLower();
                if (suffix == ".ttf" || suffix == ".otf")
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName == "uno-fluentui-assets")
                        fileName = "Symbols";
                    FontFamiliesByName.Add(fileName, new FontFamily(file));
                }
            }
    }
#endif

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
            // https://cmsdk.com/css3/enumerate-fontface-urls-using-javascriptjquery.html
            throw new NotImplementedException();
#elif WINDOWS_UWP
                return Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
#else
            throw new NotImplementedException();
#endif
            }
        }

        public static FontFamily FontFamily(string name)
        {
#if __ANDROID__
            if (FontFamiliesByName.TryGetValue(name, out var family))
                return family;
            return null;
#else
            return new FontFamily(name);
#endif
        }
    }
}
