using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qhta.MVVM
{
  public abstract class DispatchedCommand: ICommand
  {
    public DispatchedCommand() { }

    public DispatchedCommand(Dispatcher dispatcher)
    {
      Dispatcher = dispatcher;
    }

    public Dispatcher Dispatcher { get; set; }

    public event EventHandler CanExecuteChanged;

    public void NotifyCanExecuteChanged()
    {
      if (CanExecuteChanged != null)
      {
        if (Dispatcher != null)
          Dispatcher.Invoke(() =>
          {
            CanExecuteChanged.Invoke(this, new EventArgs());
          });
        else
          CanExecuteChanged.Invoke(this, new EventArgs());
      }
    }

    public abstract bool CanExecute(object parameter);

    public abstract void Execute(object parameter);

  }
}
