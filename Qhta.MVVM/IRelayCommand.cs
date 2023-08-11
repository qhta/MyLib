namespace Qhta.MVVM
{
  /// <summary>
  /// An interface expanding ICommand (from System.Windows.Input) with NotifyCanExecuteChanged method.
  /// </summary>
  public interface IRelayCommand : ICommand
  {
    /// <summary>
    ///    Notifies that the System.Windows.Input.ICommand.CanExecute(System.Object) property
    ///    has changed.
    /// </summary>
    void NotifyCanExecuteChanged();
  }
}
