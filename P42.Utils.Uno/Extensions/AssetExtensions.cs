using System;

namespace P42.Utils.Uno;

public static class AssetExtensions
{
    /// <summary>
    /// Fixes inconsistencies in Asset references if calls are made as if assets in Application project are
    /// referenced "ms-appx:///Assets/..." and assets in libraries are referenced "ms-appx:///my-library/Assets/..."
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
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
        if (projectPath.IndexOf(prefix2, StringComparison.Ordinal) is var index and > -1)
        {
            if (index > 0)
                projectName = projectPath[..index].Trim('/');
            projectPath = projectPath[(index + prefix2.Length)..];
        }

        Console.WriteLine($"projectName:projectPath = {projectName}:{projectPath}");

        var argument = string.Empty;
        if (projectPath.IndexOf('#') is var argIndex and > -1)
        {
            argument = projectPath[(argIndex + 1)..];
            projectPath = projectPath[..argIndex];
        }

        Console.WriteLine($"projectName:projectPath#argument = {projectName}:{projectPath}#{argument}");
        
#if ANDROID
        const bool  useProjectPath = true;
#elif !RELEASE && BROWSERWASM
        const bool  useProjectPath = true;
#elif !RELEASE && DESKTOP
        const bool  useProjectPath = true;
#elif IOS
        const bool  useProjectPath = true;
#elif MACCATALYST
        const bool  useProjectPath = true;
#elif !RELEASE && WINDOWS
        const bool  useProjectPath = true;
#else
        const bool useProjectPath = false;
#endif

        var assetPath = useProjectPath && !string.IsNullOrWhiteSpace(projectName)
            ? $"/{projectName}/Assets/{projectPath}"
            : $"/Assets/{projectPath}";

        //var fullPath = Windows.ApplicationModel.Package.Current.InstalledPath + assetPath;
        var assetUrl = $"ms-appx://{assetPath}" + (string.IsNullOrEmpty(argument)
            ? string.Empty
            : "#" + argument);
        Console.WriteLine($"assetPath: {assetUrl}");
        return assetUrl;

    }
}
