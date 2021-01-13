using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace P42.Utils.iOS
{
    internal class Timer : IPlatformTimer
    {
		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			var timer = NSTimer.CreateRepeatingTimer(interval, t =>
			{
				if (!callback())
					t.Invalidate();
			});
			NSRunLoop.Main.AddTimer(timer, NSRunLoopMode.Common);
		}

		public void StartTimer(TimeSpan interval, Func<Task<bool>> callback)
		{
			var timer = NSTimer.CreateRepeatingTimer(interval, async t =>
			{
				if (!(await callback()))
					t.Invalidate();
			});
			NSRunLoop.Main.AddTimer(timer, NSRunLoopMode.Common);
		}
	}
}