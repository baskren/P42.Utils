using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace P42.Utils
{
	public class WeakEventManager
	{
		static readonly object SyncObj = new object();
		static readonly Dictionary<WeakReference<object>, WeakEventManager> WeakEventManagers = new Dictionary<WeakReference<object>, WeakEventManager>();

		public static WeakEventManager GetWeakEventManager(object source)
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

				var manager = new WeakEventManager();
				WeakEventManagers.Add(new WeakReference<object>(source), manager);

				return manager;
			}
		}

		readonly object _syncObj = new object();
		readonly Dictionary<string, List<Tuple<WeakReference, MethodInfo>>> _eventHandlers = new Dictionary<string, List<Tuple<WeakReference, MethodInfo>>>();

		private WeakEventManager()
		{
		}

		public void AddEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
			where TEventArgs : EventArgs
		{
			BuildEventHandler(eventName, value.Target, value.GetMethodInfo());
		}

		public void AddEventHandler(string eventName, EventHandler value)
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
				var weakRef = new WeakReference(handlerTarget);
				var tuple = Tuple.Create(weakRef, methodInfo);
				target.Add(tuple);
			}
		}

		public bool EventExists(string eventName)
		{
			List<Tuple<WeakReference, MethodInfo>> target;
			return _eventHandlers.TryGetValue(eventName, out target);
		}

		public void RaiseEvent(object sender, object args, string eventName)
		{
			var toRaise = new List<Tuple<object, MethodInfo>>();

			lock (_syncObj)
			{
				List<Tuple<WeakReference, MethodInfo>> targets;
				if (_eventHandlers.TryGetValue(eventName, out targets))
				{
					foreach (var tuple in targets.ToList())
					{
						var o = tuple.Item1.Target;

						if (o == null && !tuple.Item2.Name.Contains("<"))
							targets.Remove(tuple);
						else						
							toRaise.Add(Tuple.Create(o, tuple.Item2));
					}
				}
				if (targets != null && targets.Count == 0)
				{
					// remove event
					_eventHandlers.Remove(eventName);
				}
			}

			foreach (var tuple in toRaise)
				tuple.Item2.Invoke(tuple.Item1, new[] { sender, args });
		}

		public void RemoveEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
			where TEventArgs : EventArgs
		{
			RemoveEventHandlerImpl(eventName, value.Target, value.GetMethodInfo());
		}

		public void RemoveEventHandler(string eventName, EventHandler value)
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
					foreach (var tuple in target.Where(
						t => t.Item1.Target == handlerTarget &&
						t.Item2.Name == methodInfo.Name
					).ToList())
						target.Remove(tuple);
				}
			}
		}
	}
}

