#if BROWSERWASM
using System.Runtime.InteropServices.JavaScript;

namespace P42.Utils.Uno;

public static partial class WasmNative
{
    [JSImport("globalThis.P42_Utils_Uno_Beep")]
    internal static partial void NativeBeep(int frequency, int duration);

    [JSImport("globalThis.P42_Utils_Uno_StorageEstimateJsonAsync")]
    internal static partial Task<string> StorageEstimateJsonAsync();
    
    [JSImport("globalThis.P42_Utils_Uno_BrowserWasmUserAgent")]
    internal static partial string GetUserAgent();
    
    [JSImport("globalThis.P42_Utils_UnoPlatform__GetPageUrl")]
    internal static partial string GetPageUrl();
    
    [JSImport("globalThis.P42_Utils_Uno_Platform_SetPageUrl")]
    internal static partial void SetPageUrl(string url);
    
    [JSImport("globalThis.P42_Utils_Uno_Platform_GetLoadedResources")]
    private static partial string GetLoadedResourcesString();
    
    public static string[] GetLoadedFiles()
    {
        try
        {
            var text = GetLoadedResourcesString().Trim('"');
#pragma warning disable IL2026
            var files = System.Text.Json.JsonSerializer.Deserialize<string[]>(text);
#pragma warning restore IL2026
            return files ?? [];
        }
        catch (Exception e)
        {
            Serilog.QuickLog.QLog.Error(e);
            return [];
        }
    }

    
    public static async Task<DateTime> GetWasmAsmDateTimeAsync(System.Reflection.Assembly assembly)
    {
        try
        {
            var name = assembly.GetName().Name;
            var files = GetLoadedFiles();
            if (files.FirstOrDefault(f => f.Contains($"/_framework/{name}")) is not { } file ||
                string.IsNullOrWhiteSpace(file))
                return default;

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Head, file);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.LastModified.HasValue)
                {
                    var lastModified = response.Content.Headers.LastModified.Value;
                    Console.WriteLine($"Last modified date-time of remote file: {lastModified}");
                    return lastModified.DateTime;
                }
                
                Console.WriteLine("Last-Modified header not found in the response.");
            }
            else
            {
                Console.WriteLine($"Error retrieving file information: {response.StatusCode}");
            }
        }
        catch (Exception e)
        {
            Serilog.QuickLog.QLog.Error(e);
        }
        
        return default;
    }
    
    }
#endif
