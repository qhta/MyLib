using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

using Microsoft.Win32;

using Qhta.MVVM;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{

  /// <summary>
  /// Command to apply block mappings to Unicode code points.
  /// A list of mappings is read from a file, and each code point in the specified blocks is updated with the corresponding UcdBlock.
  /// </summary>
  public IRelayCommand ApplyBlockMappingCommand { [DebuggerStepThrough] get; }

  private void ApplyBlockMappingCommandExecute()
  {
    if (ApplyBlockMappingBackgroundWorker.IsBusy) return;

    var dialog = new OpenFileDialog();
    dialog.FileName = "Blocks.txt"; // Default file name
    dialog.DefaultExt = ".txt"; // Default file extension
    dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
    dialog.InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Resources");

    // Show open file dialog box
    var result = MessageBox.Show(Resources.CodePoint.ApplyBlockMappingConfirm, Resources.CodePoint.ApplyBlockMapping, MessageBoxButton.OKCancel);

    // Process open file dialog box results
    if (result == MessageBoxResult.OK)
    {
      ApplyBlockMappingBackgroundWorker.RunWorkerAsync();
    }
  }

  private void ApplyBlockMapping_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    UcdCodePoints.IsBusy = true;
    ApplyBlockMappings(worker, e);
  }

  private void ApplyBlockMapping_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    UcdCodePoints.ProgressValue = e.ProgressPercentage;
  }

  private void ApplyBlockMappings(BackgroundWorker worker, DoWorkEventArgs e)
  {
    var n = UcdBlocks.Count;
    var i = 0;
    UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Updating, Resources.CodePoint.UcdBlock);
    foreach (var block in UcdBlocks)
    {
      i++;
      worker.ReportProgress((i) * 100 / n);
      //if (i == 100) break;
      //Debug.WriteLine($"Processing line {i}");
      if (worker.CancellationPending)
      {
        e.Cancel = true; // Indicate that the operation was canceled
        return;
      }
      var start = block.Range!.Start;
      var end = block.Range.End ?? start;
      for (int cp = start; cp <= end; cp++)
      {
        var codePoint = UcdCodePoints.FindById(cp);
        if (codePoint != null)
        {
          //Debug.WriteLine($"Set Block {Block.Name} for cp {cp:X4}");
          codePoint.UcdBlock = block;
        }
      }
    }
  }

  /// <summary>
  /// Command to break the apply block mapping operation.
  /// </summary>
  public IRelayCommand BreakApplyBlockMappingCommand { [DebuggerStepThrough] get; }

  /// <summary>
  /// Executes the command to break the apply block mapping operation.
  /// </summary>
  public void BreakApplyBlockMappingCommandExecute()
  {
    if (ApplyBlockMappingBackgroundWorker.IsBusy)
    {
      ApplyBlockMappingBackgroundWorker.CancelAsync();
    }
  }

  /// <summary>
  /// Background worker for applying block mappings to Unicode code points.
  /// </summary>
  public BackgroundWorker ApplyBlockMappingBackgroundWorker = new BackgroundWorker
  {
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = true
  };


  private void ApplyBlockMapping_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
  {
    //UcdCodePoints.StatusMessage = null;
    if (e.Cancelled)
    {
      MessageBox.Show(Resources.Strings.OperationCancelled, Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    else if (e.Error != null)
    {
      MessageBox.Show(String.Format(Resources.Strings.ErrorOcurred, e.Error.Message), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
    }
    else
    {
      MessageBox.Show(Resources.Strings.OperationCompleted, Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    UcdCodePoints.IsBusy = false;
    UcdCodePoints.ProgressValue = 0;
    UcdCodePoints.StatusMessage = null;
  }

}