#nullable enable
using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.UI.Xaml.Controls;
using MUXControlsTestApp.Utilities;
using Uno.Disposables;

namespace Microsoft.UI.Xaml.Tests.Enterprise;

internal class SafeEventRegistration<TElement, TDelegate>(string eventName)
    where TElement : class
    where TDelegate : Delegate
{
    private readonly EventInfo _eventInfo = typeof(TElement).GetEvent(eventName)!;
    private IDisposable? _last;

    internal IDisposable Attach(TElement element, TDelegate handler)
    {
        Detach(); // Detach any previous handler

 
        return _last = Disposable.Create(() =>
        {
            // On Windows, token is EventRegistrationToken, on Uno, token is null
#if WINAPPSDK
            var token = _eventInfo.GetAddMethod()!.Invoke(element, [handler]);
            RunOnUIThread.Execute( () => _eventInfo.GetRemoveMethod()!.Invoke(element, [token]) );
#else
            RunOnUIThread.Execute( () => _eventInfo.GetRemoveMethod()!.Invoke(element, [handler]) );
#endif
        });
    }

    internal void Detach() => _last?.Dispose();
}
