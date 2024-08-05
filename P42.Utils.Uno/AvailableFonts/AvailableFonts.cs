using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace P42.Utils.Uno
{
    public static class AvailableFonts
    {
#if __ANDROID__
        const string AssetPrefix = "ms-appx:///Assets/";
        static Dictionary<string, Microsoft.UI.Xaml.Media.FontFamily> FontFamiliesByName = new Dictionary<string, Microsoft.UI.Xaml.Media.FontFamily> { {"default", null } };

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
                    FontFamiliesByName.Add(fileName, new Microsoft.UI.Xaml.Media.FontFamily(file));
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
#elif !HAS_UNO

                Guid CLSID_DWriteFactory = new Guid("B859EE5A-D838-4B5B-A2E8-1ADC7D93DB48");
                Guid CLSID_DWriteFactory7 = new Guid("35D0E0B3-9076-4D2E-A016-A91B568A06B4");
                IntPtr pDWriteFactoryPtr = IntPtr.Zero;
                HRESULT hr = DWriteCoreCreateFactory(DWRITE_FACTORY_TYPE.DWRITE_FACTORY_TYPE_SHARED, ref CLSID_DWriteFactory7, out pDWriteFactoryPtr);
                //IDWriteFactory m_pDWriteFactory = null;
                IDWriteFactory7 m_pDWriteFactory7 = null;

                var fonts = new List<string>(); //new System.Collections.ObjectModel.ObservableCollection<string>();

                if (hr == HRESULT.S_OK)
                {
                    // m_pDWriteFactory = Marshal.GetObjectForIUnknown(pDWriteFactoryPtr) as IDWriteFactory;
                    m_pDWriteFactory7 = Marshal.GetObjectForIUnknown(pDWriteFactoryPtr) as IDWriteFactory7;
                    //IDWriteFontCollection pFontCollection;
                    //hr = m_pDWriteFactory.GetSystemFontCollection(out pFontCollection);
                    IDWriteFontCollection3 pFontCollection;
                    hr = m_pDWriteFactory7.GetSystemFontCollection7(false, DWRITE_FONT_FAMILY_MODEL.DWRITE_FONT_FAMILY_MODEL_TYPOGRAPHIC, out pFontCollection);
                    if (hr == HRESULT.S_OK)
                    {
                        uint nFamilyCount = pFontCollection.GetFontFamilyCount();
                        for (uint i = 0; i < nFamilyCount; i++)
                        {
                            IDWriteFontFamily pFontFamily;
                            hr = pFontCollection.GetFontFamily(i, out pFontFamily);
                            IDWriteLocalizedStrings pFamilyNames;
                            pFontFamily.GetFamilyNames(out pFamilyNames);
                            // https://docs.microsoft.com/en-us/windows/win32/api/dwrite/nf-dwrite-idwritelocalizedstrings-findlocalename
                            uint nIndex = 0;
                            bool bExists = false;
                            StringBuilder sbLocaleName = new StringBuilder(LOCALE_NAME_MAX_LENGTH);
                            int nDefaultLocaleSuccess = GetUserDefaultLocaleName(sbLocaleName, LOCALE_NAME_MAX_LENGTH);
                            if (nDefaultLocaleSuccess > 0)
                            {
                                hr = pFamilyNames.FindLocaleName(sbLocaleName.ToString(), out nIndex, out bExists);
                            }
                            if (hr == HRESULT.S_OK && !bExists)
                            {
                                hr = pFamilyNames.FindLocaleName("en-us", out nIndex, out bExists);
                            }
                            if (!bExists)
                                nIndex = 0;
                            hr = pFamilyNames.GetString(nIndex, sbLocaleName, LOCALE_NAME_MAX_LENGTH);
                            string sName = sbLocaleName.ToString();
                            //Console.WriteLine("Font : {0}", sName);
                            //System.Diagnostics.Debug.WriteLine("Font : " + sName);
                            fonts.Add(sName);
                            Marshal.ReleaseComObject(pFamilyNames);
                            Marshal.ReleaseComObject(pFontFamily);
                        }
                        Marshal.ReleaseComObject(pFontCollection);
                    }
                    //Marshal.ReleaseComObject(m_pDWriteFactory);
                    Marshal.ReleaseComObject(m_pDWriteFactory7);
                }
                //return Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();

                return fonts.ToArray();
#else
            throw new NotImplementedException();
#endif
            }
        }

        public static Microsoft.UI.Xaml.Media.FontFamily FontFamily(string name)
        {
#if __ANDROID__
            if (FontFamiliesByName.TryGetValue(name, out var family))
                return family;
            return null;
#else
            return new Microsoft.UI.Xaml.Media.FontFamily(name);
#endif
        }

#if !HAS_UNO                           

        [DllImport("DWriteCore.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT DWriteCoreCreateFactory(DWRITE_FACTORY_TYPE factoryType, ref Guid iid, out IntPtr factory);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetUserDefaultLocaleName(StringBuilder lpLocaleName, int cchLocaleName);
        public const int LOCALE_NAME_MAX_LENGTH = 85;
#endif
    }
}
