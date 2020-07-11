using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
