using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Qhta.MVVM;

namespace Qhta.WPF.Utils
{
  public class RelayCommand: Command
  {
    protected Action<object> _doExecuteMethod;
    protected Func<object, bool> _canExecuteMethod;

    public RelayCommand() { }

    public RelayCommand(Action<object> doExecuteMethod, Func<object, bool> canExecuteMethod=null)
    {
      _doExecuteMethod = doExecuteMethod;
      _canExecuteMethod = canExecuteMethod;
    }

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

    public override bool CanExecute(object parameter)
    {
      if (_canExecuteMethod != null)
      {
        return _canExecuteMethod(parameter);
      }
      else
      {
        return true;
      }
    }

    public override void Execute(object parameter)
    {
      if (_doExecuteMethod!=null && CanExecute(parameter))
      _doExecuteMethod(parameter);
    }
  }  
}
