using Windows.UI.Text;

namespace P42.Utils.Uno;

public static class AvailableFonts
{

    
    private static string[] _systemFonts = [];
    private const string AndroidFontsFolder = "/system/fonts";
    
    public static string[] GetSystemFontNames()
    {
        if (_systemFonts.Length > 0)
            return _systemFonts;
        
        #if __ANDROID__
        if (!Directory.Exists(AndroidFontsFolder))
            return _systemFonts;

        var fontFiles = Directory.GetFiles(AndroidFontsFolder, "*.ttf");
        _systemFonts = fontFiles.Select(System.IO.Path.GetFileNameWithoutExtension).ToArray()!;
        #elif __IOS__
        _systemFonts = UIKit.UIFont.FamilyNames;
        #elif DESKTOP
        if (OperatingSystem.IsWindows())
            _systemFonts = GetWindowsSystemFonts();
        else if (OperatingSystem.IsMacOS())
            _systemFonts = GetMacOsSystemFonts();
        else if (OperatingSystem.IsLinux())
            _systemFonts = GetLinuxSystemFonts();
        #elif WINDOWS
        _systemFonts = GetWindowsSystemFonts();
        #elif BROWSERWASM
        _systemFonts = [ 
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
        #endif
        
        Array.Sort(_systemFonts);
        return _systemFonts;
    }

    
    #if __ANDROID__ || __IOS__
    private static bool _tryingToGetPrivateFontRegistrationItems;
    private static bool _haveTriedGetPrivateFontRegistrationItems;
    private static Type? _fontEntryType;
    private static System.Reflection.MethodInfo? _fontDictionaryAddMethod;
    private static object? _fontCacheDict;
    private static System.Reflection.MethodInfo? _fontDictionaryContainsKeyMethod;
    
    private static async Task<bool> GetPrivateFontRegistrationItemsAsync()
    {
        while (_tryingToGetPrivateFontRegistrationItems)
            await Task.Delay(50);
        
        if (_haveTriedGetPrivateFontRegistrationItems)
            return _fontDictionaryAddMethod is not null;

        try
        {
            _tryingToGetPrivateFontRegistrationItems = true;
            _haveTriedGetPrivateFontRegistrationItems = true;

            var unoAsm = typeof(TextBlock).Assembly;
            _fontEntryType = unoAsm.GetType("Microsoft.UI.Xaml.Documents.TextFormatting.FontDetailsCache+FontEntry");
            var fontDetailsCacheType = unoAsm.GetType("Microsoft.UI.Xaml.Documents.TextFormatting.FontDetailsCache");
            if (_fontEntryType is null || fontDetailsCacheType is null)
            {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in loadedAssemblies)
                foreach (var t in asm.GetTypes())
                {
                    System.Diagnostics.Debug.WriteLine(t.Name);
                    if (t.Name.Contains("FontEntry") &&
                        t.FullName == "Microsoft.UI.Xaml.Documents.TextFormatting.FontDetailsCache+FontEntry")
                        _fontEntryType = t;
                    if (t.Name.Contains("FontDetailsCache") &&
                        t.FullName == "Microsoft.UI.Xaml.Documents.TextFormatting.FontDetailsCache")
                        fontDetailsCacheType = t;
                    if (_fontEntryType is not null && fontDetailsCacheType is not null)
                        break;
                }
            }

            if (_fontEntryType is null || fontDetailsCacheType is null)
                return false;

            if (fontDetailsCacheType.GetField("_fontCache", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic) is not
                { } fontCacheDictField)
                return false;

            _fontCacheDict = fontCacheDictField.GetValue(null);
            if (_fontCacheDict is null)
                return false;

            var fontCacheDictType = _fontCacheDict.GetType();
            _fontDictionaryContainsKeyMethod = fontCacheDictType.GetMethod("ContainsKey");
            _fontDictionaryAddMethod = fontCacheDictType.GetMethod("Add");
            return _fontDictionaryAddMethod is not null;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _tryingToGetPrivateFontRegistrationItems = false;
        }
    }
    #endif
    
    public static Task<FontFamily> GetSystemFontAsync(string systemFontName, TextBox textBox)
        => GetSystemFontAsync(systemFontName, textBox.FontWeight, textBox.FontStretch, textBox.FontStyle);
    
    public static Task<FontFamily> GetSystemFontAsync(string systemFontName, TextBlock textBlock)
        => GetSystemFontAsync(systemFontName, textBlock.FontWeight, textBlock.FontStretch, textBlock.FontStyle);

    public static async Task<FontFamily> GetSystemFontAsync(string systemFontName, FontWeight fontWeight, FontStretch fontStretch, FontStyle fontStyle)
    {
        
        if (!_systemFonts.Contains(systemFontName))
            return FontFamily.XamlAutoFontFamily;
        
        #if __ANDROID__ || __IOS__
        if (!await GetPrivateFontRegistrationItemsAsync())
            return FontFamily.XamlAutoFontFamily;
            
        var (skWeight, skWidth, skSlant) =
            (fontWeight.ToSkiaWeight(), fontStretch.ToSkiaWidth(), fontStyle.ToSkiaSlant());
        var entry = Activator.CreateInstance(_fontEntryType!, systemFontName, skWeight, skWidth, skSlant);
        if (_fontDictionaryContainsKeyMethod!.Invoke(_fontCacheDict, [entry]) is not (bool and false))
            return new FontFamily(systemFontName);
        
        var task = OperatingSystem.IsAndroid()
            ? Task.Run(() => SkiaSharp.SKTypeface.FromFile(System.IO.Path.Combine(AndroidFontsFolder, systemFontName + ".ttf")))
            : Task.Run(() => SkiaSharp.SKTypeface.FromFamilyName(systemFontName));
        _fontDictionaryAddMethod!.Invoke(_fontCacheDict, [entry, task]);
        #else
        await Task.CompletedTask;
        #endif

        return new FontFamily(systemFontName);
    }

    
    private static string[] GetWindowsSystemFonts()
    {
        #if WindowsBaseOs
        using var ps = System.Management.Automation.PowerShell.Create();
        ps.AddScript("Add-Type -AssemblyName System.Drawing\r\n(New-Object System.Drawing.Text.InstalledFontCollection).Families | Select-Object -ExpandProperty Name");
        var psResults = ps.Invoke();
        if (ps.HadErrors)
            return results;

        foreach (var psResult in psResults)
            results.Add(psResult.ToString());

        return results.ToArray();
        #else
        return [];
        #endif
    }
    
    
    #if DESKTOP
    private static string[] GetMacOsSystemFonts()
    {
        var results = new List<string>();

        using var process = new System.Diagnostics.Process();
        process.StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            FileName = "atsutil",
            Arguments = "fonts -list ",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            var reachedFamilies = false;
            foreach (var line in output.Split(Environment.NewLine))
            {
                if (line.StartsWith("System Families:"))
                {
                    reachedFamilies = true;
                    continue;
                }
                if (!reachedFamilies)
                    continue;
                    
                results.Add(line.Trim());
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine(error);
            Console.WriteLine(error);
        }

        return results.ToArray();
    }

    private static string[] GetLinuxSystemFonts()
    {
        var results = new List<string>();
        
        using var process = new System.Diagnostics.Process();
        process.StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            FileName = "fc-list",
            Arguments = " : family | sort | uniq ",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            results.AddRange(output.Split(Environment.NewLine).Select(line => line.Trim()));
        }
        else
        {
            System.Diagnostics.Debug.WriteLine(error);
            Console.WriteLine(error);
        }
        
        return results.ToArray();
    }
    #endif
    
}


#if __ANDROID__ || __IOS__
internal static class SkiaFontAttributeExtensions
{
    public static SkiaSharp.SKFontStyleWeight ToSkiaWeight(this FontWeight fontWeight)
    {
        // Uno weight values are using the same system,
        // so we can convert directly to Skia system
        // without need for a mapping.
        return (SkiaSharp.SKFontStyleWeight)fontWeight.Weight;
    }
    public static SkiaSharp.SKFontStyleSlant ToSkiaSlant(this FontStyle style) =>
        style switch
        {
            FontStyle.Italic => SkiaSharp.SKFontStyleSlant.Italic,
            FontStyle.Normal => SkiaSharp.SKFontStyleSlant.Upright,
            FontStyle.Oblique => SkiaSharp.SKFontStyleSlant.Oblique,
            _ => SkiaSharp.SKFontStyleSlant.Upright
        };

    public static SkiaSharp.SKFontStyleWidth ToSkiaWidth(this FontStretch stretch) =>
        stretch switch
        {
            FontStretch.Undefined => SkiaSharp.SKFontStyleWidth.Normal,
            FontStretch.UltraCondensed => SkiaSharp.SKFontStyleWidth.UltraCondensed,
            FontStretch.ExtraCondensed => SkiaSharp.SKFontStyleWidth.ExtraCondensed,
            FontStretch.Condensed => SkiaSharp.SKFontStyleWidth.Condensed,
            FontStretch.SemiCondensed => SkiaSharp.SKFontStyleWidth.SemiCondensed,
            FontStretch.Normal => SkiaSharp.SKFontStyleWidth.Normal,
            FontStretch.SemiExpanded => SkiaSharp.SKFontStyleWidth.SemiExpanded,
            FontStretch.Expanded => SkiaSharp.SKFontStyleWidth.Expanded,
            FontStretch.ExtraExpanded => SkiaSharp.SKFontStyleWidth.ExtraExpanded,
            FontStretch.UltraExpanded => SkiaSharp.SKFontStyleWidth.UltraExpanded,
            _ => SkiaSharp.SKFontStyleWidth.Normal
        };
}
#endif
