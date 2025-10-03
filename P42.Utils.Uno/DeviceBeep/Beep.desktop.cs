using System;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{
   private static bool PlatformCanBeep() 
       => System.OperatingSystem.IsMacOS() 
          || System.OperatingSystem.IsWindows() 
          || System.OperatingSystem.IsLinux();

   private static async Task PlatformBeepAsync(int frequency, int duration)
   {
       if (System.OperatingSystem.IsMacOS())
           MacNativeFunctions.PlatformBeep(frequency, duration);
       else if (System.OperatingSystem.IsWindows())
           Console.Beep(frequency, duration);
       else if (System.OperatingSystem.IsLinux())
       {
           //> speaker-test -t sine -f 800 -l 1 & sleep 0.5 && kill -9 $!
           var cmdLineText = $"speaker-test -t sine -f {(int)frequency} -l 1 & sleep {duration/1000.0} && kill -9 $!";
           var process = new System.Diagnostics.Process();
           process.StartInfo = new System.Diagnostics.ProcessStartInfo
           {
               WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
               FileName = "/bin/bash",
               Arguments = cmdLineText
           };
           process.Start();        
       }
       else
           throw new PlatformNotSupportedException("Platform not supported");

   }
}
