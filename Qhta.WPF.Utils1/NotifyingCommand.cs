using System;
using System.Windows.Input;

namespace Qhta.WPF.Utils
{
  public class NotifyingCommand : ICommand
  {
    public object DataContext { get; set; }

    public event EventHandler CanExecuteChanged;

    public void NotifyCanExecuteChanged()
    {
      var handler = CanExecuteChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    public virtual bool CanExecute(object parameter)
    {
      return true;
    }

    public virtual void Execute(object parameter)
    {
      throw new NotImplementedException();
    }


  }
}
