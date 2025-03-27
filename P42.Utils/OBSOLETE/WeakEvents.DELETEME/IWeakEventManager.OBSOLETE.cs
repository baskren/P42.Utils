using System;
namespace P42.Utils;

[Obsolete("Use AsyncAwaitBestPractices instead.", true)]
public interface IWeakEventManager
{
    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    void AddEventHandler<TEventArgs>(object source, string eventName, EventHandler<TEventArgs> value)
        where TEventArgs : EventArgs;

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    void AddEventHandler(object source, string eventName, EventHandler value);

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    void HandleEvent<TEventArgs>(object sender, TEventArgs args, string eventName)
        where TEventArgs : EventArgs;

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    void RemoveEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
        where TEventArgs : EventArgs;

    [Obsolete("Use AsyncAwaitBestPractices instead.", true)]
    void RemoveEventHandler(string eventName, EventHandler value);
}
