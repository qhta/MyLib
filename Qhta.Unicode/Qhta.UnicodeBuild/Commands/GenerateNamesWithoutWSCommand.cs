using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

using Qhta.SF.WPF.Tools;
using Qhta.TypeUtils;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.NameGen;
using Strings = Qhta.UnicodeBuild.Resources.Strings;
using Qhta.UnicodeBuild.ViewModels;
using Qhta.UnicodeBuild.Views;

using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Command to apply character names generation to selected Unicode code points.
/// </summary>
public class GenerateNamesWithoutWSCommand : TimeConsumingCommand
{
  private record ApplyCharNamesGenerationArgs(List<UcdCodePointViewModel> ListOfPoints);

  /// <inheritdoc/>
  public override bool CanExecute(object? parameter)
  {
    if (BackgroundWorker.IsBusy) return false;
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Argument must be of type SfDataGrid.", nameof(parameter));
    }
    if (!dataGrid.GetSelectedRowsAndColumns(out bool allColumnsSelected, out var selectedColumns, out bool allRowsSelected, out var selectedRows).Any()
        && !allRowsSelected && !selectedRows.Any())
      return false;
    var listOfPoints = selectedRows.OfType<UcdCodePointViewModel>().ToList();
    if (!listOfPoints.Any())
      return false;

    return true;
  }

  /// <inheritdoc/>
  public override void Execute(object? parameter)
  {
    if (BackgroundWorker.IsBusy) return;
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Argument must be of type SfDataGrid.", nameof(parameter));
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

    if (MessageBox.Show(String.Format(Resources.Strings.GenerateNamesWithoutWSConfirm, n), Resources.Strings.Confirm, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
    {
      try
      {
        NameGenerator = new NameGenerator { WritingSystems = _ViewModels.Instance.WritingSystems };
        BackgroundWorker.RunWorkerAsync(new ApplyCharNamesGenerationArgs(listOfPoints));
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }

  private NameGenerator NameGenerator = null!;


  /// <inheritdoc/>
  protected override void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    if (!(e.Argument is ApplyCharNamesGenerationArgs argument))
    {
      throw new ArgumentException("Argument must be of type List<UcdCodePointViewModel>.", nameof(e.Argument));
    }
    _ViewModels.Instance.UcdCodePoints.IsBusy = true;
    _ViewModels.Instance.UcdCodePoints.BreakCommand = BreakCommand;

    var listOfPoints = argument.ListOfPoints;
    try
    {
      var n = listOfPoints.Count;
      GeneratedNamesCount = 0;
      var i = 0;
      _ViewModels.Instance.UcdCodePoints.StatusMessage =
        String.Format(Resources.Strings.UpdatingField, Resources.UcdCodePointStrings.CharName);
      UndoRedoManager.StartGrouping();
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

        var oldName = codePoint.CharName;
        {
          var newName = NameGenerator.GenerateNameWithoutWS(codePoint);
          if (newName == null || newName==oldName)
            continue; //newName = oldName;
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
      UndoRedoManager.StopGrouping();
    }
  }

  /// <summary>
  /// Count of recognized writing systems during the last recognition operation.
  /// </summary>
  private int GeneratedNamesCount;


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
      var count = GeneratedNamesCount;
      string msg = string.Format(Resources.Strings.GenerateCharNamesCompleted,
        count);
      MessageBox.Show(msg,
        Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    _ViewModels.Instance.UcdCodePoints.IsBusy = false;
    _ViewModels.Instance.UcdCodePoints.ProgressValue = 0;
    _ViewModels.Instance.UcdCodePoints.StatusMessage = null;
  }

}