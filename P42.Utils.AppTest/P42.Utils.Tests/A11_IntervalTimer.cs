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
        var results = new List<TimeSpan>();
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
            var error = elapsed - timeSpan;
            results.Add(error);
            lastDateTime = now;

            await Task.Delay(250);

            //elapsed.ShouldBeGreaterThanOrEqualTo(timeSpan + TimeSpan.FromMilliseconds(250));
            Console.WriteLine(error);

            count++;
            if (count >= iters)
                tcs.SetResult(true);
            return count < iters;
        });

        await tcs.Task;

        Assert.IsFalse(results.Any(e => e < TimeSpan.FromMilliseconds(250)));
    }
    
    [TestMethod]
    public async Task A01_Periodic()
    {
        Console.WriteLine(nameof(A01_Periodic));
        var results = new List<TimeSpan>();
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
            var error = elapsed - timeSpan;
            results.Add(error);
            lastDateTime = now;

            await Task.Delay(250);

            //elapsed.ShouldBeLessThanOrEqualTo(timeSpan + TimeSpan.FromMilliseconds(100));
            Console.WriteLine(error);

            count++;
            if (count >= iters)
                tcs.SetResult(true);
            return count < iters;
        });

        await tcs.Task;

        Assert.IsFalse(results.Any(e => e > TimeSpan.FromMilliseconds(100)));
    }

}
