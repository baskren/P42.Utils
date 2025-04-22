#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Media;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{

    static bool PlatformCanBeep() => true;

    // More accurate sine wave generation using AudioTrack (requires more setup)
    static async Task PlatformBeepAsync(int frequencyHz, int durationMs)
    {
        int sampleRate = 44100;
        int numSamples = durationMs * sampleRate / 1000;
        double[] samples = new double[numSamples];
        short[] generatedSnd = new short[numSamples];
        double twopi = 2 * Math.PI;
        double phase = 0.0;

        for (int i = 0; i < numSamples; i++)
        {
            samples[i] = Math.Sin(phase);
            generatedSnd[i] = (short)(samples[i] * short.MaxValue);
            phase += twopi * frequencyHz / sampleRate;
        }

        AudioTrack? audioTrack = null;
        try
        {
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
            {
#pragma warning disable CA1422 // Validate platform compatibility
                audioTrack = new AudioTrack(
                    Android.Media.Stream.Music,
                    sampleRate,
                    ChannelOut.Mono,
                    Android.Media.Encoding.Pcm16bit,
                    numSamples * 2, // Two bytes per sample for PCM16BIT
                    AudioTrackMode.Static);
#pragma warning restore CA1422 // Validate platform compatibility
            }
            else
            {
                int bufferSize = AudioTrack.GetMinBufferSize(sampleRate, ChannelOut.Mono, Android.Media.Encoding.Pcm16bit);

                var audioAttributes = new AudioAttributes.Builder()
                    ?.SetUsage(AudioUsageKind.Media)
                    ?.SetContentType(AudioContentType.Music)
                    ?.Build();

                var format = new AudioFormat.Builder()
                    ?.SetSampleRate(sampleRate)
                    ?.SetChannelMask(ChannelOut.Mono)
                    ?.SetEncoding(Android.Media.Encoding.Pcm16bit)
                    ?.Build();

                audioTrack = new AudioTrack.Builder()
                    .SetAudioAttributes(audioAttributes)
                    .SetAudioFormat(format)
                    .SetBufferSizeInBytes(bufferSize)
                    .SetTransferMode(AudioTrackMode.Static)
                    .Build();

                if (audioTrack?.State != AudioTrackState.Initialized)
                {
                    Console.WriteLine("Error initializing AudioTrack.");
                    return;
                }
            }

            audioTrack.Write(generatedSnd, 0, numSamples);
            audioTrack.Play();

            // Wait for the duration
            await Task.Delay(durationMs);
            audioTrack.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing sine wave: {ex}");
        }
        finally
        {
            audioTrack?.Release();
        }
    }


}

#endif
