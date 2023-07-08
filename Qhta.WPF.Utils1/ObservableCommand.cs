using System;
using System.Windows.Input;

using Qhta.MVVM;

namespace Qhta.WPF.Utils
{
  public class ObservableCommand: Command
  {

    public override event EventHandler CanExecuteChanged
    {
      add
      {
        _CanExecuteChanged += value;
        CommandManager.RequerySuggested += value;
      }
      remove
      {
        _CanExecuteChanged -= value;
        CommandManager.RequerySuggested -= value;
      }
    }

  }  
}
