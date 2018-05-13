using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyLib.WpfUtils
{
  public class Command : ICommand
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
