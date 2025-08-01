using System.ComponentModel;

using Qhta.MVVM;

namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Abstract command for time-consuming operations.
/// </summary>
public abstract class TimeConsumingCommand: Command
{
 /// <summary>
 /// Initializes a new instance of the <see cref="TimeConsumingCommand"/> class.
 /// </summary>
 /// <remarks>This constructor sets up event handlers for the <see cref="BackgroundWorker"/> to manage the
 /// execution of a time-consuming operation. The event handlers are responsible for handling the work process,
 /// reporting progress, and completing the operation.</remarks>
 protected TimeConsumingCommand()
  {
    BackgroundWorker.DoWork += BackgroundWorker_DoWork;
    BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
    BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
  }

  /// <summary>
  /// Background worker for time-consuming operations.
  /// </summary>
  public static BackgroundWorker BackgroundWorker { get; } = new()
  {
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = true
  };

  /// <summary>
  /// Command to break the time-consuming operation.
  /// </summary>
  public static RelayCommand BreakCommand { get; } = new(BreakCommandExecute);

  private static void BreakCommandExecute()
  {
    if (BackgroundWorker.IsBusy)
    {
      BackgroundWorker.CancelAsync();
    }
  }

  /// <summary>
  /// Method to start of the background worker operation.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  protected abstract void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e);

  /// <summary>
  /// Method to report progress during the background worker operation.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  protected abstract void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e);

  /// <summary>
  /// Method to handle the completion of the background worker operation.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  protected abstract void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e);
}