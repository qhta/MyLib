using System.ComponentModel;
using System.Windows;

using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to map code points to blocks.
/// A code point is mapped to a block if its code value is within the block range, this command fills the Block property of code points.
/// </summary>
public class ApplyBlockMappingCommand : TimeConsumingCommand
{
  /// <inheritdoc/>
  public override void Execute(object? parameter)
  {
    if (BackgroundWorker.IsBusy) return;

    // Show open file dialog box
    var result = MessageBox.Show(Resources.Strings.ApplyBlockMappingConfirm, Resources.Strings.ApplyBlockMapping, MessageBoxButton.OKCancel);

    // Process open file dialog box results
    if (result == MessageBoxResult.OK)
    {
      BackgroundWorker.RunWorkerAsync();
    }
  }

  /// <inheritdoc/>
  protected override void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    _ViewModels.Instance.UcdCodePoints.IsBusy = true;
    _ViewModels.Instance.UcdCodePoints.BreakCommand = BreakCommand;
    var n = _ViewModels.Instance.UcdBlocks.Count;
    var i = 0;
    _ViewModels.Instance.UcdCodePoints.StatusMessage = String.Format(Resources.Strings.UpdatingField, Resources.UcdCodePointStrings.UcdBlock);
    foreach (var block in _ViewModels.Instance.UcdBlocks)
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
        var codePoint = _ViewModels.Instance.UcdCodePoints.FindById(cp);
        if (codePoint != null)
        {
          //Debug.WriteLine($"Set Block {Block.Name} for cp {cp:X4}");
          codePoint.Block = block;
        }
      }
    }
  }

  /// <inheritdoc/>
  protected override void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    _ViewModels.Instance.UcdCodePoints.ProgressValue = e.ProgressPercentage;
  }

  /// <inheritdoc/>
  protected override void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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
    _ViewModels.Instance.UcdCodePoints.IsBusy = false;
    _ViewModels.Instance.UcdCodePoints.ProgressValue = 0;
    _ViewModels.Instance.UcdCodePoints.StatusMessage = null;
  }


}