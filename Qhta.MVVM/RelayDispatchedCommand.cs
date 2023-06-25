using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qhta.MVVM
{
  /// <summary>
  ///  A dispatched command which relays its functionality to other objects by invoking delegates.
  /// </summary>
  public sealed class RelayDispatchedCommand : DispatchedCommand, IRelayCommand, ICommand
  {
    /// <summary>
    /// Action to invoke on Execute.
    /// </summary>
    private readonly Action execute;

    //
    /// <summary>
    ///  Function to invoke on CanExecute.
    /// </summary>
    private readonly Func<bool>? canExecute;

    /// <summary>
    ///  Initializes a new instance with action to execute.
    /// </summary>
    /// <param name="execute">Action to execute.</param>
    public RelayDispatchedCommand(Action execute)
    {
      this.execute = execute;
    }

    /// <summary>
    ///  Initializes a new instance with action to execute and function to check if can execute.
    /// </summary>
    /// <param name="execute">Action to execute.</param>
    /// <param name="canExecute">Function to check if Action can be executed</param>
    public RelayDispatchedCommand(Action execute, Func<bool> canExecute)
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }

    /// <summary>
    /// Overriden function which checks if a command can execute an action.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool CanExecute(object? parameter)
    {
      return canExecute?.Invoke() ?? true;
    }

    /// <summary>
    /// Overriden method for command action. It uses <see cref="Dispatcher"/>
    /// </summary>
    /// <param name="parameter"></param>
    public override void Execute(object? parameter)
    {
      if (Dispatcher != null)
        Dispatcher.Invoke(() =>
        {
          execute();
        });
      else
        execute();

    }
  }
}

