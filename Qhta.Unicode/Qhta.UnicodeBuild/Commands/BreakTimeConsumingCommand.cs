using System.ComponentModel;

using Qhta.MVVM;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to break a time-consuming operation. Starting a time-consuming operation requires a <see cref="BackgroundWorker"/> to be set.
/// </summary>
public class BreakTimeConsumingCommand : Command
{
  /// <summary>
  /// Reference to the <see cref="BackgroundWorker"/> that is used for time-consuming operations.
  /// </summary>
  public BackgroundWorker? BackgroundWorker { get; set; }

  /// <summary>
  /// The command to break the time-consuming operation can be executed only if the <see cref="BackgroundWorker"/> is set and currently busy.
  /// </summary>
  /// <param name="parameter"></param>
  /// <returns></returns>
  public override bool CanExecute(object? parameter)
  {
    return BackgroundWorker != null && BackgroundWorker.IsBusy;
  }

  /// <summary>
  /// Executes the cancellation of the background operation if it is currently running.
  /// </summary>
  /// <remarks>This method checks if the <see cref="BackgroundWorker"/> is busy and, if so, initiates an
  /// asynchronous cancellation request.</remarks>
  /// <param name="parameter">An optional parameter that is not used in this method.</param>
  public override void Execute(object? parameter)
  {
    if (BackgroundWorker != null && BackgroundWorker.IsBusy)
    {
      BackgroundWorker.CancelAsync();
    }
  }
}
