using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PCL.Utils
{
	public class WeakEventManager<TEventHandler> 
	{
		static readonly object SyncObj = new object();
		static readonly Dictionary<WeakReference<object>, WeakEventManager<TEventHandler>> WeakEventManagers = new Dictionary<WeakReference<object>, WeakEventManager<TEventHandler>>();

		public static WeakEventManager<TEventHandler> GetWeakEventManager(object source)
		{
			lock (SyncObj)
			{
				foreach (var kvp in WeakEventManagers.ToList())
				{
					object target;

					if (kvp.Key.TryGetTarget(out target))
					{
						if (ReferenceEquals(target, source))
							return kvp.Value;
					}
					else
						WeakEventManagers.Remove(kvp.Key);
				}

				var manager = new WeakEventManager<TEventHandler>();
				WeakEventManagers.Add(new WeakReference<object>(source), manager);

				return manager;
			}
		}

		readonly object _syncObj = new object();
		readonly Dictionary<string, List<Tuple<WeakReference, MethodInfo>>> _eventHandlers = new Dictionary<string, List<Tuple<WeakReference, MethodInfo>>>();

		private WeakEventManager()
		{
		}

		public void AddEventHandler(string eventName, TEventHandler value)
		{
			BuildEventHandler(eventName, value.Target, value.GetMethodInfo());
		}

		void BuildEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
		{
			lock (_syncObj)
			{
				List<Tuple<WeakReference, MethodInfo>> target;
				if (!_eventHandlers.TryGetValue(eventName, out target))
				{
					target = new List<Tuple<WeakReference, MethodInfo>>();
					_eventHandlers.Add(eventName, target);
				}

				target.Add(Tuple.Create(new WeakReference(handlerTarget), methodInfo));
			}
		}

		public void RaiseEvent(object sender, object args, string eventName)
		{
			var toRaise = new List<Tuple<object, MethodInfo>>();

			lock (_syncObj)
			{
				List<Tuple<WeakReference, MethodInfo>> target;
				if (_eventHandlers.TryGetValue(eventName, out target))
				{
					foreach (var tuple in target.ToList())
					{
						var o = tuple.Item1.Target;

						if (o == null)
							target.Remove(tuple);
						else
							toRaise.Add(Tuple.Create(o, tuple.Item2));
					}
				}
			}

			foreach (var tuple in toRaise)
				tuple.Item2.Invoke(tuple.Item1, new[] { sender, args });
		}

		public void RemoveEventHandler(string eventName, TEventHandler value)
		{
			RemoveEventHandlerImpl(eventName, value.Target, value.GetMethodInfo());
		}

		void RemoveEventHandlerImpl(string eventName, object handlerTarget, MemberInfo methodInfo)
		{
			lock (_syncObj)
			{
				List<Tuple<WeakReference, MethodInfo>> target;
				if (_eventHandlers.TryGetValue(eventName, out target))
				{
					foreach (var tuple in target.Where(t => t.Item1.Target == handlerTarget &&
						t.Item2.Name == methodInfo.Name).ToList())
						target.Remove(tuple);
				}
			}
		}
	}
}

