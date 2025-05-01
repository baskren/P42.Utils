#if __WASM__
using Uno.Foundation;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace P42.Utils.Tests;

public static class AudioHelper
{
    private static string? _audioContextId;
    private static string? _sineNodeId;

    public static async Task CreateAudioContextAndModule()
    {
        string script = @"
            return new Promise(resolve => {
                const audioContext = new (window.AudioContext || window.webkitAudioContext)();
                audioContext.audioWorklet.addModule('./sine-processor.js').then(() => {
                    resolve(Uno.Interop.getObjectId(audioContext));
                }).catch(error => {
                    console.error('Error adding AudioWorklet module:', error);
                    resolve(null);
                });
            });
        ";

        string resultId = await WebAssemblyRuntime.InvokeAsync(script);
        if (!string.IsNullOrEmpty(resultId) && resultId != "null")
        {
            _audioContextId = resultId;
        }
        else
        {
            _audioContextId = null;
            Console.WriteLine("Failed to create AudioContext and add module.");
        }
    }

    public static async Task StartSineWave()
    {
        if (_audioContextId == null)
        {
            Console.WriteLine("AudioContext not initialized.");
            return;
        }

        string script = $@"
            return new Promise(resolve => {{
                const audioContext = Uno.Interop.getObject({_audioContextId});
                if (audioContext) {{
                    const sineNode = audioContext.createWorkletNode('sine-processor');
                    const destination = audioContext.destination;
                    sineNode.connect(destination);
                    resolve(Uno.Interop.getObjectId(sineNode));
                }} else {{
                    resolve(null);
                }}
            }});
        ";

        string resultId = await WebAssemblyRuntime.InvokeAsync(script);
        if (!string.IsNullOrEmpty(resultId) && resultId != "null")
        {
            _sineNodeId = resultId;
        }
        else
        {
            _sineNodeId = null;
            Console.WriteLine("Failed to create or connect SineProcessor node.");
        }
    }

    public static async Task StopSineWave()
    {
        if (_sineNodeId != null)
        {
            string script = $@"
                const sineNode = Uno.Interop.getObject({_sineNodeId});
                if (sineNode) {{ sineNode.disconnect(); }}
            ";
            await WebAssemblyRuntime.InvokeAsync(script); // InvokeAsync returns a string, but we don't need it here
            string releaseScript = $"Uno.Interop.releaseObject({_sineNodeId});";
            await WebAssemblyRuntime.InvokeAsync(releaseScript);
            _sineNodeId = null;
        }
    }

    public static async Task PlaySineWaveForDuration(int durationMilliseconds)
    {
        await StartSineWave();
        await Task.Delay(durationMilliseconds);
        await StopSineWave();
    }
}
#endif
