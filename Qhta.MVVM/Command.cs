using System;
using System.Windows;
using System.Windows.Input;

namespace Qhta.MVVM
{
  public abstract class Command: DependencyObject, ICommand
  {
    public virtual event EventHandler CanExecuteChanged
    {
      add { _CanExecuteChanged += value; }
      remove { _CanExecuteChanged -= value; }
    }
    protected EventHandler _CanExecuteChanged;

    public void NotifyCanExecuteChanged()
    {
      if (_CanExecuteChanged!=null)
      {
        OnCanExecuteChanged();
      }
    }

    protected virtual void OnCanExecuteChanged()
    {
      _CanExecuteChanged.DynamicInvoke(new object[] { this, new EventArgs() });
    }

    public virtual bool CanExecute(object parameter)
    {
      return true;
    }

    public virtual void Execute(object parameter)
    {
    }
  }
}
