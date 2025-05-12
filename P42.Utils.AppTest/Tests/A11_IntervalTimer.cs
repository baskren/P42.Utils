using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class A11_Timers
{
    [TestMethod]
    public async Task A00_Interval()
    {
        Console.WriteLine(nameof(A00_Interval));
        var timeSpan = TimeSpan.FromMilliseconds(1000);
        var count = 0;
        var iters = 5;
        var tcs = new TaskCompletionSource<bool>();

        var first = true;
        var lastDateTime = DateTime.Now;
        P42.Utils.IntervalTimer.StartTimer(timeSpan, async () =>
        {
            var now = DateTime.Now;

            if (first)
            {
                lastDateTime = now;
                await Task.Delay(250);
                first = false;
                return true;
            }

            var elapsed = now - lastDateTime;
            lastDateTime = now;

            await Task.Delay(250);

            elapsed.ShouldBeGreaterThanOrEqualTo(timeSpan + TimeSpan.FromMilliseconds(250));
            Console.WriteLine(elapsed.ToString());

            count++;
            if (count >= iters)
                tcs.SetResult(true);
            return count < iters;
        });

        await tcs.Task;
    }
    
    [TestMethod]
    public async Task A01_Periodic()
    {
        Console.WriteLine(nameof(A01_Periodic));
        var timeSpan = TimeSpan.FromMilliseconds(1000);
        var count = 0;
        var iters = 5;
        var tcs = new TaskCompletionSource<bool>();

        var first = true;
        var lastDateTime = DateTime.Now;
        P42.Utils.PeriodicTimer.StartTimer(timeSpan, async () =>
        {
            var now = DateTime.Now;

            if (first)
            {
                lastDateTime = now;
                await Task.Delay(250);
                first = false;
                return true;
            }

            var elapsed = now - lastDateTime;
            lastDateTime = now;

            await Task.Delay(250);

            elapsed.ShouldBeLessThanOrEqualTo(timeSpan + TimeSpan.FromMilliseconds(50));
            Console.WriteLine(elapsed.ToString());

            count++;
            if (count >= iters)
                tcs.SetResult(true);
            return count < iters;
        });

        await tcs.Task;
    }

}
