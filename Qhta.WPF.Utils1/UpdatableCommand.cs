using System;
using System.Windows.Input;

namespace Qhta.MVVM
{
  public abstract class UpdatableCommand: Command
  {
    public override event EventHandler CanExecuteChanged
    {
      add
      {
        CommandManager.RequerySuggested += value;
        base._CanExecuteChanged+=value;
      }
      remove
      {
        CommandManager.RequerySuggested -= value;
        base._CanExecuteChanged-=value;
      }
    }

  //  public virtual bool CanExecute(object parameter)
  //  {
  //    return true;
  //  }

  //  public void Execute(object parameter)
  //  {
  //    DoExecute(parameter);
  //  }
  //  public abstract void DoExecute(object parameter);
  }
}

