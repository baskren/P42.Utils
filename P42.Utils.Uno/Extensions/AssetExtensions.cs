using System;

namespace P42.Utils.Uno;

public static class AssetExtensions
{

    public static string AssetPath(string path)
    {
        //return path;

        var projectPath = path;
        const string prefix1 = "ms-appx://";
        if (projectPath.StartsWith(prefix1))
            projectPath = projectPath.Substring(prefix1.Length);
        else
            return path;

        projectPath = projectPath.Replace('\\', '/').Replace("//", "/");

        var projectName = string.Empty;
        var prefix2 = $"/Assets/";
        if (projectPath.IndexOf(prefix2) is int index && index > -1)
        {
            if (index > 0)
                projectName = projectPath.Substring(0, index).Trim('/');
            projectPath = projectPath.Substring(index + prefix2.Length);
        }

        // Console.WriteLine($"projectName:projectPath = {projectName}:{projectPath}");

        var argument = string.Empty;
        if (projectPath.IndexOf('#') is int argIndex && argIndex > -1)
        {
            argument = projectPath.Substring(argIndex + 1);
            projectPath = projectPath.Substring(0, argIndex);
        }

        Console.WriteLine($"projectName:projectPath#argument = {projectName}:{projectPath}#{argument}");

        var useProjectPath = false;

#if ANDROID
        useProjectPath = true;
#elif !RELEASE && BROWSERWASM
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

        var assetPath = (useProjectPath && !string.IsNullOrWhiteSpace(projectName))
            ? $"/{projectName}/Assets/{projectPath}"
            : $"/Assets/{projectPath}";

        var fullPath = Windows.ApplicationModel.Package.Current.InstalledPath + assetPath;
        var assetUrl = $"ms-appx://{assetPath}" + (string.IsNullOrEmpty(argument)
            ? string.Empty
            : "#" + argument);
        Console.WriteLine($"assetPath: {assetUrl}");
        return assetUrl;

    }
}
