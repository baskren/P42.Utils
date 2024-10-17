using System;
using System.Collections.Generic;
using System.Reflection;

namespace P42.Utils;
/*
public class WeakEventManager
{
    private static readonly object SyncObj = new ();
    private static readonly Dictionary<WeakReference<object>, WeakEventManager> WeakEventManagers = new ();

    public static WeakEventManager GetWeakEventManager(object source)
    {
        lock (SyncObj)
        {
            var keys = new List<WeakReference<object>>(WeakEventManagers.Keys);
            foreach (var key in keys)
            {
                if (key.TryGetTarget(out var target))
                {
                    if (ReferenceEquals(target, source))
                        return WeakEventManagers[key];
                }
                else
                    WeakEventManagers.Remove(key);
            }

            var manager = new WeakEventManager();
            WeakEventManagers.Add(new WeakReference<object>(source), manager);

            return manager;
        }
    }

    private readonly object _syncObj = new ();
    private readonly Dictionary<string, List<Tuple<WeakReference, MethodInfo>>> _eventHandlers = new();

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

    private void BuildEventHandler(string eventName, object? handlerTarget, MethodInfo methodInfo)
    {
        lock (_syncObj)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var target))
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
        lock (_syncObj)
        {
            return _eventHandlers.ContainsKey(eventName);
        }
    } 
        

    public void RaiseEvent(object sender, object args, string eventName)
    {
        var toRaise = new List<Tuple<object, MethodInfo>>();

        lock (_syncObj)
        {
            if (_eventHandlers.TryGetValue(eventName, out var targets))
            {
                foreach (var tuple in targets.ToArray())
                {
                    var target = tuple.Item1.Target;

                    if (target == null && !tuple.Item2.Name.Contains('<'))
                        targets.Remove(tuple);
                    else
                        toRaise.Add(Tuple.Create(target, tuple.Item2));
                }
            }
                
            if (targets is { Count: 0 })
                _eventHandlers.Remove(eventName);
                
        }

        foreach (var tuple in toRaise)
            tuple.Item2.Invoke(tuple.Item1, [sender, args]);
    }

    public void RemoveEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
        where TEventArgs : EventArgs
        => RemoveEventHandlerImpl(eventName, value.Target, value.GetMethodInfo());
		

    public void RemoveEventHandler(string eventName, EventHandler value)
        => RemoveEventHandlerImpl(eventName, value.Target, value.GetMethodInfo());
		

    private void RemoveEventHandlerImpl(string eventName, object? handlerTarget, MemberInfo methodInfo)
    {
        lock (_syncObj)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var targets))
                return;

            for (var i = 0; i < targets.Count;)
            {
                var target = targets[i];
                if (target.Item1.Target == handlerTarget && target.Item2.Name == methodInfo.Name)
                    targets.RemoveAt(i);
                else
                    i++;
            }
                
        }
    }
}
*/
