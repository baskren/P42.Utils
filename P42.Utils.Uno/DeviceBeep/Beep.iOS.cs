using AVFoundation;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{

    static bool PlatformCanBeep() => true;


    private static readonly AVAudioEngine AudioEngine;
    private static readonly AVAudioPlayerNode PlayerNode;
    private static readonly AVAudioFormat AudioFormat;

    static DeviceBeep()
    {
        AudioEngine = new AVAudioEngine();
        PlayerNode = new AVAudioPlayerNode();
        AudioFormat = new AVAudioFormat(44100, 1); // Standard format
        AudioEngine.AttachNode(PlayerNode);
        AudioEngine.Connect(PlayerNode, AudioEngine.OutputNode, AudioFormat);

        try
        {
            AudioEngine.Prepare();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AudioEngine Start Error: {ex.Message}");
        }
    }

    public static async Task PlatformBeepAsync(double frequency, int durationMs)
    {
        if (AudioEngine.Running)
            AudioEngine.Stop();
        AudioEngine.Reset();

        // Generate sine wave data
        var sampleRate = (int)AudioFormat.SampleRate;
        var frameCount = sampleRate * durationMs / 1000;
        var buffer = new AVAudioPcmBuffer(AudioFormat, (uint)frameCount);
        {
            unsafe
            {
                var channels = (nint*)buffer.FloatChannelData.ToPointer();
                var data = (float*)channels[0].ToPointer();
                for (var i = 0; i < frameCount; i++)
                {
                    try
                    {
                        var time = (double)i / sampleRate;
                        data[i] = (float)(1.0 * Math.Sin(2 * Math.PI * frequency * time)); // Adjust volume (0.2) as needed
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"EXCEPTION: i=[{ex}]");
                    }
                }
            }

            buffer.FrameLength = (uint)frameCount;
            PlayerNode.ScheduleBuffer(buffer, null);

            try
            {
                if (AudioEngine.StartAndReturnError(out var error))
                {
                    QLog.Error($"AVAudioEngine Start Error: {error.LocalizedDescription}");
                    AudioEngine.Stop();
                    return;
                }
                
                PlayerNode.Play();
                await Task.Delay(durationMs);
                PlayerNode.Stop();
                AudioEngine.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing tone: {ex}");
            }
        }
        
    }


}

