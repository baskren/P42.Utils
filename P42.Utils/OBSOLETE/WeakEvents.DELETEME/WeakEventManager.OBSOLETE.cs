using System;

// REPLACE WITH AsyncAwaitBestPractices

namespace P42.Utils;

[Obsolete("Use AsyncAwaitBestPractices instead.", true)]
public class WeakEventManager
{
    private static readonly WeakEventManager Instance = new(); 
    
    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    public static WeakEventManager GetWeakEventManager(object source)
        => Instance;
    
    private WeakEventManager()
    {
    }

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    public void AddEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
        where TEventArgs : EventArgs
    {
    }

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    public void AddEventHandler(string eventName, EventHandler value)
    {
    }


    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    public bool EventExists(string eventName)
        => false;

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    public void RaiseEvent(object sender, object args, string eventName)
    {
    }

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    public void RemoveEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
        where TEventArgs : EventArgs
    {
    }

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    public void RemoveEventHandler(string eventName, EventHandler value)
    {
    }
    
}
