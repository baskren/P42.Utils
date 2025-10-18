namespace P42.Utils.Uno;

public static class LinuxNative
{
    public static async Task BeepAsync(int freq, int duration)
    {
        var cmdLineText = $"-t sine -f {freq} -l {duration} ";
        var process = new System.Diagnostics.Process();
        process.StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            FileName = "speaker-test",
            Arguments = cmdLineText
        };
        process.Start();
        await  Task.Delay(duration);
    }       
}
