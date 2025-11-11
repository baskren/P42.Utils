using System.Runtime.CompilerServices;

namespace P42.Utils.Uno;

public static class Shell
{
    // ReSharper disable once UnusedMember.Local
    public static int ExecuteCommand(string command, string arguments, out string output, out string error)
    {
        if (OperatingSystem.IsIOS() || OperatingSystem.IsBrowser())
        {
            output = string.Empty;
            error = "Unsupported operating system";
            return -1;
        }        
        // Create a new process
        using var process = new System.Diagnostics.Process();
        // Configure the process
        if (OperatingSystem.IsWindows())
        {
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
        }
        else
        {
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
        }
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;

        // Start the process
        process.Start();

        // Capture output
        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();

        process.WaitForExit();
        
        return process.ExitCode;
    }
}
