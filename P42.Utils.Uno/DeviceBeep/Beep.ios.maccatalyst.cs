#if __IOS__ || __MACCATALYST__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AudioToolbox;
using AVFoundation;
using Foundation;
using P42.Serilog.QuickLog;
using Windows.Storage.Streams;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{

    static bool PlatformCanBeep() => true;


    private static readonly AVAudioEngine _audioEngine;
    private static readonly AVAudioPlayerNode _playerNode;
    //private static AVAudioPcmBuffer _buffer;
    private static readonly AVAudioFormat _audioFormat;

    static DeviceBeep()
    {
        _audioEngine = new AVAudioEngine();
        _playerNode = new AVAudioPlayerNode();
        _audioFormat = new AVAudioFormat(44100, 1); // Standard format
        _audioEngine.AttachNode(_playerNode);
        _audioEngine.Connect(_playerNode, _audioEngine.OutputNode, _audioFormat);

        try
        {
            _audioEngine.Prepare();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AudioEngine Start Error: {ex.Message}");
        }
    }

    public static async Task PlatformBeepAsync(double frequency, int durationMs)
    {
        if (_audioEngine.Running)
            _audioEngine.Stop();
        _audioEngine.Reset();

        // Generate sine wave data
        var sampleRate = (int)_audioFormat.SampleRate;
        var frameCount = (int)(sampleRate * durationMs / 1000);
        var buffer = new AVAudioPcmBuffer(_audioFormat, (uint)frameCount);
        if (buffer?.FloatChannelData != null)
        {
            unsafe
            {
                nint* channels = (nint*)buffer.FloatChannelData.ToPointer();
                float* data = (float*)channels[0].ToPointer();
                for (int i = 0; i < frameCount; i++)
                {
                    try
                    {
                        double time = (double)i / sampleRate;
                        data[i] = (float)(1.0 * Math.Sin(2 * Math.PI * frequency * time)); // Adjust volume (0.2) as needed
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"EXECPTION: i=[{ex}]");
                    }
                }
            }

            buffer.FrameLength = (uint)frameCount;
            _playerNode.ScheduleBuffer(buffer, null);

            try
            {
                _audioEngine.StartAndReturnError(out var error);
                if (error != null)
                {
                    QLog.Error($"AVAudioEngine Start Error: {error}");
                    _audioEngine.Stop();
                    return;
                }
                
                _playerNode.Play();
                await Task.Delay(durationMs);
                _playerNode.Stop();
                _audioEngine.Stop();
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
        Console.WriteLine("SCHEULE COMPLETED");
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
#endif
