using System;
using System.Windows.Input;

namespace P42.Utils.Uno;

/// <summary>
/// A command whose sole purpose is to relay its functionality
/// to other objects by invoking delegates.
/// The default return value for the CanExecute method is 'true'.
/// RaiseCanExecuteChanged needs to be called whenever
/// CanExecute is expected to return a different value.
/// </summary>
// ReSharper disable once UnusedType.Global
public class RelayCommand : ICommand
{
    #region Fields
    private readonly Action? _execute;
    private readonly Func<bool>? _canExecute;
    private readonly Action<object?>? _executeEvaluator;
    private readonly Func<object?, bool>? _canExecuteEvaluator;
    #endregion

    
    #region Events
    /// <summary>
    /// Raised when RaiseCanExecuteChanged is called.
    /// </summary>
    //public event EventHandler CanExecuteChanged;
    private readonly AsyncAwaitBestPractices.WeakEventManager _canExecuteChangedEventManager = new();
    public event EventHandler? CanExecuteChanged
    {
        add => _canExecuteChangedEventManager.AddEventHandler(value);
        remove => _canExecuteChangedEventManager.RemoveEventHandler(value);
    }
    #endregion
    
    
    #region Constructor
    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    
    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action execute, Func<object?, bool>? canExecute)
    {
        _execute = execute;
        _canExecuteEvaluator = canExecute;
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action<object?> execute, Func<bool>? canExecute = null)
    {
        _executeEvaluator = execute;
        _canExecute = canExecute;
    }
    
    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute)
    {
        _executeEvaluator = execute;
        _canExecuteEvaluator = canExecute;
    }
    #endregion
    
    
    #region ICommand Methods
    /// <summary>
    /// Determines whether this RelayCommand can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed,
    /// this object can be set to null.
    /// </param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public virtual bool CanExecute(object? parameter)
        => _canExecute?.Invoke() ?? _canExecuteEvaluator?.Invoke(parameter) ?? true;
    
    /// <summary>
    /// Executes the RelayCommand on the current command target.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed,
    /// this object can be set to null.
    /// </param>
    public virtual void Execute(object? parameter)
    {
        if (_execute != null)
            _execute();
        else if (_executeEvaluator != null)
            _executeEvaluator(parameter);
        else
            throw new ArgumentException("Execute Action is null");
    }
    /// <summary>
    /// Method used to raise the CanExecuteChanged event
    /// to indicate that the return value of the CanExecute
    /// method has changed.
    /// </summary>
    public virtual void RaiseCanExecuteChanged()
        => _canExecuteChangedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
    #endregion
}

