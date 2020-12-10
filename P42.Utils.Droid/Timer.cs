using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Droid
{
    public class Timer : IPlatformTimer
    {
		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			var handler = new Handler(Looper.MainLooper);
			handler.PostDelayed(() =>
			{
				if (callback?.Invoke() ?? false)
					StartTimer(interval, callback);

				handler.Dispose();
				handler = null;
			}, (long)interval.TotalMilliseconds);
		}

		public void StartTimer(TimeSpan interval, Func<Task<bool>> callback)
		{
			var handler = new Handler(Looper.MainLooper);
			handler.PostDelayed(async() =>
			{
				if (await callback.Invoke())
					StartTimer(interval, callback);

				handler.Dispose();
				handler = null;
			}, (long)interval.TotalMilliseconds);
		}


	}
}