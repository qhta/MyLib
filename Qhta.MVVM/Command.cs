using System;
using System.Windows;
using System.Windows.Input;

namespace Qhta.MVVM
{
  /// <summary>
  /// Abstract class implementing <see cref="ICommand"/> interface
  /// </summary>
  public abstract class Command: DependencyObject, ICommand
  {
    /// <summary>
    /// Event required by <see cref="ICommand"/> interface. 
    /// It is a hook for callback method invoked when a result of <see cref="CanExecute(object)"/> function may be changed.
    /// </summary>
    public virtual event EventHandler CanExecuteChanged
    {
      add { _CanExecuteChanged += value; }
      remove { _CanExecuteChanged -= value; }
    }
    /// <summary>
    /// Internal event for <see cref="CanExecuteChanged"/> to be called in descendant classes.
    /// </summary>
    protected EventHandler? _CanExecuteChanged;

    /// <summary>
    /// A method to notify that a result of <see cref="CanExecute(object)"/> function may be changed.
    /// Invokes <see cref="OnCanExecuteChanged"/> callback method.
    /// </summary>
    public virtual void NotifyCanExecuteChanged()
    {
      if (_CanExecuteChanged!=null)
      {
        OnCanExecuteChanged();
      }
    }

    /// <summary>
    /// Callback method to notify, that a result of <see cref="CanExecute(object)"/> function may bye changed.
    /// </summary>
    protected virtual void OnCanExecuteChanged()
    {
      _CanExecuteChanged?.DynamicInvoke(new object[] { this, new EventArgs() });
    }

    /// <summary>
    /// Default function which checks if a command can execute an action.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public virtual bool CanExecute(object parameter)
    {
      return true;
    }

    /// <summary>
    /// Default abstract method for command action. 
    /// </summary>
    /// <param name="parameter"></param>
    public abstract void Execute(object parameter);
  }
}
