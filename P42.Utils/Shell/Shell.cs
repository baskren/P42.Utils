namespace P42.Utils;

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
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;

        // Start the process
        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            output = string.Empty;
            error = ex.Message;
            return -1;
        }

        // Capture output
        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();

        process.WaitForExit();
        
        return process.ExitCode;
    }


    public static async Task<(int code, string output, string error)> ExecuteCommandAsync(string command, string arguments, CancellationToken token = default)
    {
        var output = string.Empty;
        string error;
        if (OperatingSystem.IsIOS() || OperatingSystem.IsBrowser())
        {
            error = "Unsupported operating system";
            return (-1, output, error);
        }
        // Create a new process
        using var process = new System.Diagnostics.Process();
        // Configure the process
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;

        // Start the process
        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return (-1, output, error);
        }

        await Task.Run(() =>
        {
            process.WaitForExit();
        }, token);

        output = await process.StandardOutput.ReadToEndAsync(token);
        error = await process.StandardError.ReadToEndAsync(token);

        if (process.HasExited)
            return (process.ExitCode, output, error);

        return (-1, output, $"process was cancelled : [{error}]");
    }

}
