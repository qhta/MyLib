using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

using Qhta.SF.Tools;
using Qhta.TypeUtils;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.NameGen;
using Qhta.UnicodeBuild.Resources;
using Qhta.UnicodeBuild.ViewModels;
using Qhta.UnicodeBuild.Views;

using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Command to apply character names generation to selected Unicode code points.
/// </summary>
public class ApplyCharNamesGenerationCommand : TimeConsumingCommand
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

    var dialog = new NameGenOptionsDialog { NameGenOptions = NameGenOptions };
    if (dialog.ShowDialog() == true)
    //MessageBox.Show(String.Format(Resources.Strings.ApplyCharNamesGenerationConfirm, n), Resources.Strings.Confirm, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
    {
      try
      {
        NameGenOptions = dialog.NameGenOptions;
        LoadPredefinedNames(NameGenOptions.PredefinedNamesFile);
        LoadAbbreviatedWords(NameGenOptions.AbbreviatedWordsFile);
        HashSet<string> allNames = new(_ViewModels.Instance.UcdCodePoints.Where(item => String.IsNullOrEmpty(item.CharName)).Select(item => item.CharName!));
        NameGenerator = new NameGenerator { PredefinedNames = PredefinedNames, AbbreviatedWords = AbbreviatedWords, AllNames = allNames};
        if (NameGenOptions.UseKnownNumerals)
        {
          LoadKnownNumerals(NameGenOptions.KnownNumeralsFile);
          NameGenerator.KnownNumerals = KnownNumerals;
        }

        BackgroundWorker.RunWorkerAsync(new ApplyCharNamesGenerationArgs(listOfPoints));
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }

  private NameGenOptions NameGenOptions = new NameGenOptions
  {
    PredefinedNamesFile = "Resources/PredefinedNames.csv",
    AbbreviatedWordsFile = "Resources/AbbreviatedWords.csv",
    KnownNumeralsFile = "Resources/KnownNumerals.csv",
  };

  private Dictionary<CodePoint, string> PredefinedNames = new();
  private Dictionary<string, string> AbbreviatedWords = new();
  private Dictionary<string, string> KnownNumerals = new();

  private NameGenerator NameGenerator = null!;

  private void LoadPredefinedNames(string filename)
  {
    PredefinedNames = new();
    var lines = File.ReadAllText(filename).Split(['\n']);
    foreach (string line in lines)
    {
      string str = line.Trim();
      if (string.IsNullOrEmpty(line)) continue;
      var parts = line.Split(['\t', ';', ','], 2);
      if (parts.Length < 2) continue;
      if (!CodePoint.TryParse(parts[0], out var codePoint) || codePoint == null) continue;
      PredefinedNames[codePoint] = parts[1].Trim();
    }
  }

  private void LoadAbbreviatedWords(string filename)
  {
    AbbreviatedWords = new();
    var lines = File.ReadAllText(filename).Split(['\n']);
    foreach (string line in lines)
    {
      string str = line.Trim();
      if (string.IsNullOrEmpty(line)) continue;
      var parts = line.Split(['\t', ';', ','], 2);
      if (parts.Length < 2) continue;
      AbbreviatedWords[parts[0].Trim()] = parts[1].Trim();
    }
  }

  private void LoadKnownNumerals(string filename)
  {
    KnownNumerals = new();
    var lines = File.ReadAllText(filename).Split(['\n']);
    foreach (string line in lines)
    {
      string str = line.Trim();
      if (string.IsNullOrEmpty(line)) continue;
      var parts = line.Split(['\t', ';', ','], 2);
      if (parts.Length < 2) continue;
      KnownNumerals[parts[0].Trim()] = parts[1].Trim();
    }
  }

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
        String.Format(Resources.Strings.Updating, Resources.UcdCodePointStrings.WritingSystem);
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
          var newName = NameGenerator.GenerateShortName(codePoint);
          if (newName == null ||
            oldName == newName)
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
      string msg = string.Format(Resources.Strings.ApplyCharNamesGenerationCompleted,
        count);
      MessageBox.Show(msg,
        Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    _ViewModels.Instance.UcdCodePoints.IsBusy = false;
    _ViewModels.Instance.UcdCodePoints.ProgressValue = 0;
    _ViewModels.Instance.UcdCodePoints.StatusMessage = null;
  }

}