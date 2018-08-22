using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MyLib.MVVM
{
  public abstract class Command: ICommand
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

    public void Execute(object parameter)
    {
      DoExecute(parameter);
    }
    public abstract void DoExecute(object parameter);
  }
}
