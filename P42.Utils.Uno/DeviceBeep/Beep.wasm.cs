#if BROWSERWASM
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Foundation;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{
    static bool PlatformCanBeep()
        => true;

    static async Task PlatformBeepAsync(int freq, int duration)
    {
        //WebAssemblyRuntime.InvokeJS($"p42_utils_uno_beep({freq}, {duration});");
        /*
                    var snd = new Audio(""Assets/beep.mp3"");  
                    console.log(""BEEP 1"");
                    audio.controls = true;
                    console.log(""BEEP 2"");
                    document.body.appendChild(snd);
                    console.log(""BEEP 3"");
                    snd.play();
                    console.log(""BEEP 4"");
                    document.body.removeChild(snd);
        */

        var js = $@"
            console.log(""BEEP ENTER"");
            var context = new (window.AudioContext || window.webkitAudioContext)();
            var osc = context.createOscillator(); // instantiate an oscillator
            osc.type = 'sine'; // this is the default - also square, sawtooth, triangle
            osc.frequency.value = {freq}; // Hz
            osc.connect(context.destination); // connect it to the destination
            osc.start(); // start the oscillator
            osc.stop(context.currentTime + {duration/1000.0}); 
            console.log(""BEEP EXIT"");
            ";
        Console.WriteLine($"js=[{js}]");
        WebAssemblyRuntime.InvokeJS(js);
        await Task.CompletedTask;
    }
}
#endif
