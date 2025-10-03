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
        if (buffer.FloatChannelData != null)
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
        // Play the generated buffer
        //_buffer.FrameLength = (uint)frameCount;
        //_playerNode.ScheduleBuffer(_buffer, null, false);
        //_playerNode.Play();
    }

    private static void OnComplete()
    {
        Console.WriteLine("SCHEDULE COMPLETED");
    }

    /*
    const double SampleRate = 44100.0; // Standard audio sample rate
    static double _frequency; // Frequency of the tone
    static double _theta; // Current phase of the wave
    static bool _isPlaying; // Playback state
    static AudioStreamBasicDescription audioFormat = new ()
    {
        SampleRate = SampleRate,
        Format = AudioFormatType.LinearPCM,
        FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsPacked,
        ChannelsPerFrame = 1,
        FramesPerPacket = 1,
        BitsPerChannel = 16,
        BytesPerFrame = 2,
        BytesPerPacket = 2
    };


    static async Task PlatformBeepAsync(double frequency, double duration)
    {
        if (_isPlaying)
            return;

        _isPlaying = true;

        // Audio format description

        var audioQueue = new OutputAudioQueue(audioFormat);

        // Allocate and enqueue audio buffers
        for (int i = 0; i < 3; i++)
        {
            IntPtr buffer;
            audioQueue.AllocateBuffer(2048, out buffer);
            FillAudioBuffer(buffer, 2048);
            audioQueue.EnqueueBuffer(buffer, 2048, new AudioStreamPacketDescription[] { });
        }

        var tcs = new TaskCompletionSource<bool>();

        EventHandler<BufferCompletedEventArgs> callback = (object? sender, BufferCompletedEventArgs e) =>
        {
            tcs.TrySetResult(true);
        };

        audioQueue.BufferCompleted += callback;
        audioQueue.Start();

        // Stop after the specified duration
        NSTimer.CreateScheduledTimer(duration, _ =>
        {
            audioQueue.Stop(true);
        });

        await tcs.Task;
        audioQueue.BufferCompleted -= callback;
        audioQueue.Dispose();
    }

    static void FillAudioBuffer(IntPtr buffer, int bufferSize)
    {
        short[] samples = new short[bufferSize / 2];
        double thetaIncrement = 2.0 * Math.PI * _frequency / SampleRate;

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = (short)(Math.Sin(_theta) * short.MaxValue);
            _theta += thetaIncrement;

            if (_theta > 2.0 * Math.PI)
                _theta -= 2.0 * Math.PI;
        }

        Marshal.Copy(samples, 0, buffer, samples.Length);
    }
    */

}

