namespace Qhta.MVVM
{
  /// <summary>
  ///  A command which relays its functionality to other objects by invoking delegates.
  /// </summary>
  public sealed class RelayCommand<ParamType> : Command, IRelayCommand, ICommand where ParamType: class
  {
    /// <summary>
    /// Action to invoke on Execute.
    /// </summary>
    private readonly Action<ParamType?> execute;

    //
    /// <summary>
    ///  Function to invoke on CanExecute.
    /// </summary>
    private readonly Func<ParamType?,bool>? canExecute;

    /// <summary>
    ///  Initializes a new instance with action to execute.
    /// </summary>
    /// <param name="execute">Action to execute.</param>
    public RelayCommand(Action<ParamType?> execute)
    {
      this.execute = execute;
    }

    /// <summary>
    ///  Initializes a new instance with action to execute and function to check if can execute.
    /// </summary>
    /// <param name="execute">Action to execute.</param>
    /// <param name="canExecute">Function to check if Action can be executed</param>
    public RelayCommand(Action<ParamType?> execute, Func<ParamType?, bool> canExecute)
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }

    /// <summary>
    /// Overriden function which checks if a command can execute an action.
    /// </summary>
    /// <param name="parameter"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool CanExecute(object? parameter)
    {
      if (canExecute!=null)
        Debug.Assert(true);
      if (parameter != null)
        return canExecute?.Invoke((ParamType)parameter) ?? true;
      return canExecute?.Invoke(default(ParamType)) ?? true;
    }

    /// <summary>
    /// Overriden method to invoke command action.
    /// </summary>
    /// <param name="parameter"></param>
    public override void Execute(object? parameter)
    {
      if (parameter != null)
        execute((ParamType)parameter);
      else
        execute(default(ParamType));
    }
  }
}

