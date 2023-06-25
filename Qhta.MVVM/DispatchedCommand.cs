using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qhta.MVVM
{
  /// <summary>
  ///  A command which invokes Dispatcher when NotifyCanExecuteChanged is called.
  /// </summary>
  public abstract class DispatchedCommand: Command, ICommand
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DispatchedCommand() { }

    /// <summary>
    /// Overriden method to notify that a result of <see cref="Command.CanExecute(object)"/> function may be changed.
    /// </summary>
    public override void NotifyCanExecuteChanged()
    {
      if (_CanExecuteChanged != null)
      {
        if (Dispatcher != null)
          Dispatcher.Invoke(() =>
          {
            _CanExecuteChanged.Invoke(this, new EventArgs());
          });
        else
          _CanExecuteChanged.Invoke(this, new EventArgs());
      }
    }

  }
}
