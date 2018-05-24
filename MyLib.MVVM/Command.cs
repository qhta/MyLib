using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MyLib.MVVM
{
  public abstract class Command: ICommand
  {
    public event EventHandler CanExecuteChanged;

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
