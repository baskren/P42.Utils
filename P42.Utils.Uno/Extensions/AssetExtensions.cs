using System;

namespace P42.Utils.Uno;

public static class AssetExtensions
{

    // TODO: Has the reason for this method been fixed in UNO?
    
    /// <summary>
    /// Edit path to asset (particularly library assets) so that it works across all platforms
    /// </summary>
    /// <param name="path"></param>
    /// <returns>Asset path for current platform</returns>
    public static string AssetPath(string path)
    {

        //return path;

        var projectPath = path;
        const string prefix1 = "ms-appx://";
        if (projectPath.StartsWith(prefix1))
            projectPath = projectPath[prefix1.Length..];
        else
            return path;

        projectPath = projectPath.Replace('\\', '/').Replace("//", "/");

        var projectName = string.Empty;
        const string prefix2 = "/Assets/";
        if (projectPath.IndexOf(prefix2, StringComparison.InvariantCulture) is var index and > -1)
        {
            if (index > 0)
                projectName = projectPath[..index].Trim('/');
            projectPath = projectPath[(index + prefix2.Length)..];
        }

        // Console.WriteLine($"projectName:projectPath = {projectName}:{projectPath}");

        var argument = string.Empty;
        if (projectPath.IndexOf('#') is var argIndex and > -1)
        {
            argument = projectPath[(argIndex + 1)..];
            projectPath = projectPath[..argIndex];
        }

        Console.WriteLine($"projectName:projectPath#argument = {projectName}:{projectPath}#{argument}");

        var useProjectPath = false;

#if ANDROID
        useProjectPath = true;
#elif BROWSERWASM
        useProjectPath = true;
#elif !RELEASE && DESKTOP
        useProjectPath = true;
#elif IOS
        useProjectPath = true;
#elif MACCATALYST
        useProjectPath = true;
#elif !RELEASE && WINDOWS
        useProjectPath = true;
#endif

        var assetPath = useProjectPath && !string.IsNullOrWhiteSpace(projectName)
            ? $"/{projectName}/Assets/{projectPath}"
            : $"/Assets/{projectPath}";

        // var fullPath = Windows.ApplicationModel.Package.Current.InstalledPath + assetPath;
        var assetUrl = $"ms-appx://{assetPath}{(string.IsNullOrEmpty(argument)
            ? string.Empty
            : $"#{argument}")}";
        
        Console.WriteLine($"assetPath: {assetUrl}");
        return assetUrl;

    }
}
