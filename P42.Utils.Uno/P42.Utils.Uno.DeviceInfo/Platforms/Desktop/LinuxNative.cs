namespace P42.Utils.Uno;

public static class LinuxNative
{
    public static async Task BeepAsync(int freq, int duration)
    {
        Shell.ExecuteCommand("speaker-test", $"-t sine -f {freq} -l {duration} ", out _, out _);
        await  Task.Delay(duration);
    }       
}
