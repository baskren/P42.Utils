using Android.Media;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{

    private static bool PlatformCanBeep() => true;

    // More accurate sine wave generation using AudioTrack (requires more setup)
    private static async Task PlatformBeepAsync(int frequencyHz, int durationMs)
    {
        const int sampleRate = 44100;
        var numSamples = durationMs * sampleRate / 1000;
        var samples = new double[numSamples];
        var generatedSnd = new short[numSamples];
        const double twoPi = 2 * Math.PI;
        var phase = 0.0;

        for (var i = 0; i < numSamples; i++)
        {
            samples[i] = Math.Sin(phase);
            generatedSnd[i] = (short)(samples[i] * short.MaxValue);
            phase += twoPi * frequencyHz / sampleRate;
        }

        AudioTrack? audioTrack = null;
        try
        {
            switch ((int)Android.OS.Build.VERSION.SdkInt)
            {
                case < 26:
#pragma warning disable CA1422 // Validate platform compatibility
                    audioTrack = new AudioTrack(
                        Android.Media.Stream.Music,
                        sampleRate,
                        ChannelOut.Mono,
                        Android.Media.Encoding.Pcm16bit,
                        numSamples * 2, // Two bytes per sample for PCM16BIT
                        AudioTrackMode.Static);
#pragma warning restore CA1422 // Validate platform compatibility
                    break;
                case >= 23:
                {
                    var bufferSize = AudioTrack.GetMinBufferSize(sampleRate, ChannelOut.Mono, Encoding.Pcm16bit);

                    var audioAttributes = new AudioAttributes.Builder()
                        .SetUsage(AudioUsageKind.Media)!
                        .SetContentType(AudioContentType.Music)!
                        .Build()!;

                    var format = new AudioFormat.Builder()
                        .SetSampleRate(sampleRate)!
                        .SetChannelMask(ChannelOut.Mono)
                        .SetEncoding(Encoding.Pcm16bit)!
                        .Build()!;

#pragma warning disable CA1416
                    audioTrack = new AudioTrack.Builder()
                        .SetAudioAttributes(audioAttributes)
                        .SetAudioFormat(format)
                        .SetBufferSizeInBytes(bufferSize)
                        .SetTransferMode(AudioTrackMode.Static)
                        .Build();
#pragma warning restore CA1416
                    break;
                }
            }
            if (audioTrack.State != AudioTrackState.Initialized)
            {
                Console.WriteLine("Error initializing AudioTrack.");
                return;
            }

            await audioTrack.WriteAsync(generatedSnd, 0, numSamples);
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

