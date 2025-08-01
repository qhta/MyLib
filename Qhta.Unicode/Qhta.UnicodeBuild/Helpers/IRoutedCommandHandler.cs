using System.Windows.Input;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Interface for handling routed commands in a WPF application.
/// </summary>
public interface IRoutedCommandHandler
{
  /// <summary>
  /// Handles the CanExecute event for a routed command.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  void OnCanExecute(object sender, CanExecuteRoutedEventArgs e);
  /// <summary>
  /// Handles the event that occurs after a routed command has been executed.
  /// </summary>
  /// <remarks>This method is invoked when a routed command completes execution. Use the event data in 
  /// <paramref name="e"/> to determine the command that was executed and any associated parameters.</remarks>
  /// <param name="sender">The source of the event, typically the control that raised the command.</param>
  /// <param name="e">The event data associated with the executed routed command.</param>
  void OnExecuted(object sender, ExecutedRoutedEventArgs e);
}