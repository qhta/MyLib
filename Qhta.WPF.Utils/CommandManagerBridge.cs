namespace Qhta.WPF.Utils;

/// <summary>
/// Listener for CanExecuteChanged event implementation based on WPF <see cref="CommandManager"/>.
/// </summary>
public class CommandManagerBridge: ICanExecuteChangedListener
{

  /// <summary>
  /// EventHandler of CanExecuteChanged assigned to <see cref="CommandManager.RequerySuggested"/>.
  /// </summary>
  public event EventHandler CanExecuteChanged
  {
    add { CommandManager.RequerySuggested += value; }
    remove { CommandManager.RequerySuggested -= value; }
  }
}
