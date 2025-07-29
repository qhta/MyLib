using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.TextUtils;
using Qhta.UndoManager;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.NameGen;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  private void InitializeApplyCharNamesGeneration()
  {
    ApplyCharNamesGenerationCommand = new RelayCommand<object>(ApplyCharNamesGenerationCommandExecute);
    ApplyCharNamesGenerationBackgroundWorker.DoWork += ApplyCharNamesGeneration_DoWork;
    ApplyCharNamesGenerationBackgroundWorker.ProgressChanged += ApplyCharNamesGeneration_ProgressChanged;
    ApplyCharNamesGenerationBackgroundWorker.RunWorkerCompleted += ApplyCharNamesGeneration_RunWorkerCompleted;
    BreakApplyCharNamesGenerationCommand = new RelayCommand(BreakApplyCharNamesGenerationCommandExecute);
  }

  private record ApplyCharNamesGenerationArgs(List<UcdCodePointViewModel> ListOfPoints);

  /// <summary>
  /// Command to apply short name generation on a set of Unicode code points. 
  /// </summary>
  public RelayCommand<object>? ApplyCharNamesGenerationCommand { [DebuggerStepThrough] get; set; }

  private void ApplyCharNamesGenerationCommandExecute(object? argument)
  {
    if (ApplyCharNamesGenerationBackgroundWorker.IsBusy) return;
    if (argument is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Argument must be of type SfDataGrid.", nameof(argument));
    }
    if (!dataGrid.GetSelectedRowsAndColumns(out bool allColumnsSelected, out var selectedColumns, out bool allRowsSelected, out var selectedRows).Any()
        && !allRowsSelected && !selectedRows.Any())
    {
      MessageBox.Show(Resources.Strings.NoCodePointsSelected, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
      return;
    }
    var listOfPoints = selectedRows.OfType<UcdCodePointViewModel>().ToList();
    if (!listOfPoints.Any())
    {
      MessageBox.Show(Resources.Strings.NoCodePointsSelected, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
      return;
    }

    int n = listOfPoints.Count();

    if (MessageBox.Show(String.Format(Resources.Strings.ApplyCharNamesGenerationConfirm, n), Resources.Strings.Confirm, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
      ApplyCharNamesGenerationBackgroundWorker.RunWorkerAsync(new ApplyCharNamesGenerationArgs(listOfPoints));
  }

  private void ApplyCharNamesGeneration_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    UcdCodePoints.IsBusy = true;
    if (e.Cancel)
    {
      UcdCodePoints.IsBusy = true;
      return; // Check if the operation was canceled
    }
    ApplyCharNamesGeneration(worker, e);
  }

  private void ApplyCharNamesGeneration_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    UcdCodePoints.ProgressValue = e.ProgressPercentage;
  }


  private void ApplyCharNamesGeneration(BackgroundWorker worker, DoWorkEventArgs e)
  {
    if (!(e.Argument is ApplyCharNamesGenerationArgs argument))
    {
      throw new ArgumentException("Argument must be of type List<UcdCodePointViewModel>.", nameof(e.Argument));
    }
    var listOfPoints = argument.ListOfPoints;
    try
    {
      var n = listOfPoints.Count;
      GeneratedNamesCount = 0;
      var i = 0;
      UcdCodePoints.StatusMessage =
        String.Format(Resources.Strings.Updating, Resources.UcdCodePointStrings.WritingSystem);
      var nameGen = new NameGenerator();
      UndoMgr.StartGrouping();
      foreach (var codePoint in listOfPoints)
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

        string? oldName = codePoint.CharName;
        if (String.IsNullOrEmpty(oldName))
        {
          var newName = nameGen.GenerateShortName(codePoint);
          if (newName == null ||
            oldName==newName)
            continue;
          codePoint.CharName = newName;
          GeneratedNamesCount++;
        }
      }

    }
    catch (Exception exception)
    {
      Debug.WriteLine(exception);
      MessageBox.Show(String.Format(Resources.Strings.ErrorOcurred, exception.Message), Resources.Strings.Error,
        MessageBoxButton.OK, MessageBoxImage.Error);
      throw;
    }
    finally
    {
      UndoMgr.StopGrouping();
    }
  }

  /// <summary>
  /// Count of recognized writing systems during the last recognition operation.
  /// </summary>
  private int GeneratedNamesCount;

  /// <summary>
  /// Command to break the apply writing system mapping operation.
  /// </summary>
  public IRelayCommand? BreakApplyCharNamesGenerationCommand { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Executes the command to break the apply writing system mapping operation.
  /// </summary>
  public void BreakApplyCharNamesGenerationCommandExecute()
  {
    if (ApplyCharNamesGenerationBackgroundWorker.IsBusy)
    {
      ApplyCharNamesGenerationBackgroundWorker.CancelAsync();
    }
  }

  /// <summary>
  /// Background worker for applying writing system mappings to Unicode code points.
  /// </summary>
  public BackgroundWorker ApplyCharNamesGenerationBackgroundWorker = new BackgroundWorker
  {
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = true
  };


  private void ApplyCharNamesGeneration_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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
      var count = GeneratedNamesCount;
      string msg = string.Format(Resources.Strings.ApplyCharNamesGenerationCompleted,
        count);
      MessageBox.Show(msg,
        Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    UcdCodePoints.IsBusy = false;
    UcdCodePoints.ProgressValue = 0;
    UcdCodePoints.StatusMessage = null;
  }

}