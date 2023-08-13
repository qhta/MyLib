namespace Qhta.MVVM
{
  /// <summary>
  /// Abstract class implementing <see cref="ICommand"/> interface
  /// </summary>
  public abstract class Command : ObservableObject, ICommand
  {
    /// <summary>
    /// Listener of CanExecuteChanged event. In WPF it should be bridged to CommandManager.
    /// </summary>
    public static ICanExecuteChangedListener? CommandManager { get; set; }

    /// <summary>
    /// Name to trace command.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Event required by <see cref="ICommand"/> interface. 
    /// It is a hook for callback method invoked when a result of <see cref="CanExecute(object)"/> function may be changed.
    /// </summary>
    public virtual event EventHandler? CanExecuteChanged
    {
      add
      {
        _CanExecuteChanged += value;
        if (value != null)
          value.Invoke(this, EventArgs.Empty);
        if (CommandManager != null)
          CommandManager.CanExecuteChanged += value;
      }
      remove
      {
        _CanExecuteChanged -= value;
        if (CommandManager != null)
          CommandManager.CanExecuteChanged -= value;
      }
    }

    /// <summary>
    /// Internal event for <see cref="CanExecuteChanged"/> to be called in descendant classes.
    /// </summary>
    protected EventHandler? _CanExecuteChanged;

    /// <summary>
    /// A method to notify that a result of <see cref="CanExecute(object)"/> function may be changed.
    /// </summary>
    public virtual void NotifyCanExecuteChanged()
    {
      if (_CanExecuteChanged != null)
      {
        var dispatcher = base.Dispatcher;
        foreach (EventHandler eventHandler in _CanExecuteChanged.GetInvocationList())
        {
          if (dispatcher != null)
          {
            if (BeginInvokeActionEnabled)
              dispatcher.BeginInvoke(eventHandler, this, EventArgs.Empty);
            else
              dispatcher.Invoke(eventHandler, this, EventArgs.Empty);
          }
          else
          {
            if (BeginInvokeActionEnabled)
              eventHandler.BeginInvoke(this, EventArgs.Empty, null, null);
            else
              eventHandler.Invoke(this, EventArgs.Empty);
          }
        }
      }
    }

    ///// <summary>
    ///// Callback method to notify, that a result of <see cref="CanExecute(object)"/> function may be changed.
    ///// </summary>
    //protected virtual void OnCanExecuteChanged()
    //{

    //}

    /// <summary>
    /// Default function which checks if a command can execute an action.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public virtual bool CanExecute(object? parameter)
    {
      return true;
    }

    /// <summary>
    /// Default abstract method for command action. 
    /// </summary>
    /// <param name="parameter"></param>
    public abstract void Execute(object? parameter);
  }
}
