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
public class GenerateCharNamesCommand : TimeConsumingCommand
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

    var dialog = new NameGenOptionsDialog { NameGenOptions = NameGenOptions with { CodePointsCount = n } };
    if (dialog.ShowDialog() == true)
    //MessageBox.Show(String.Format(Resources.Strings.ApplyCharNamesGenerationConfirm, n), Resources.Strings.Confirm, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
    {
      try
      {
        NameGenOptions = dialog.NameGenOptions;
        LoadPredefinedNames(NameGenOptions.PredefinedNamesFile);
        LoadAndParseKnownPhrasesFile(NameGenOptions.KnownPhrasesFile);
        HashSet<string> allNames = new(_ViewModels.Instance.UcdCodePoints.Where(item => !String.IsNullOrEmpty(item.CharName)).Select(item => item.CharName!));
        NameGenerator = new NameGenerator
        {
          PredefinedNames = PredefinedNames,
          AbbreviatedPhrases = AbbreviatedPhrases,
          Adjectives = Adjectives,
          Removables = Removables,
          SpecialPhrases = SpecialPhrases,
          SpecialFunctions = SpecialFunctions
        };

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
    PredefinedNamesFile = "Data/PredefinedNames.csv",
    KnownPhrasesFile = "Data/KnownPhrases.csv",
  };

  private Dictionary<CodePoint, string> PredefinedNames = new();
  private Dictionary<string, string> AbbreviatedPhrases = new();
  private HashSet<string> Adjectives = new();
  private HashSet<string> Removables = new();
  private Dictionary<string, SpecialFunction> SpecialPhrases = new();
  private Dictionary<SpecialFunction, string> SpecialFunctions = new();

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

  /// <summary>
  /// Loads and parses the abbreviated words file.
  /// Parses each line, expecting the format:
  /// phrase; abbreviation; [adj]
  /// </summary>
  /// <param name="filename"></param>
  private void LoadAndParseKnownPhrasesFile(string filename)
  {
    AbbreviatedPhrases = new();
    Adjectives = new();
    Removables = new();
    SpecialPhrases = new();
    SpecialFunctions = new();

    var lines = File.ReadAllText(filename).Split(['\n']);
    foreach (string line in lines)
    {
      string str = line.Trim();
      if (string.IsNullOrEmpty(line)) continue;
      if (line.StartsWith("#")) continue; // Skip comments
      var parts = line.Split(['\t', ';', ','], StringSplitOptions.TrimEntries);
      if (parts.Length < 2)
      {
        Debug.WriteLine($"Invalid line in abbreviated words file: {line}");
        continue;
      }
      var key = parts[0];
      var value = parts[1];
      if (value == "<null>")
        value = string.Empty;
      if (!AbbreviatedPhrases.TryAdd(key, value))
      {
        Debug.WriteLine($"Duplicate key in abbreviated words file: {key}");
      }
      if (parts.Length > 2)
      {
        if (parts[2].StartsWith("adj", StringComparison.OrdinalIgnoreCase))
          Adjectives.Add(key);
        else if (parts[2].StartsWith("del", StringComparison.OrdinalIgnoreCase))
          Removables.Add(key);
        else if (parts[2].StartsWith("num", StringComparison.OrdinalIgnoreCase))
#pragma warning disable CS0642 // Possible mistaken empty statement
          ;// ignore numbers
#pragma warning restore CS0642 // Possible mistaken empty statement
        else
        {
          switch (parts[2])
          {
            case "<NC>":
              SpecialPhrases[parts[0]] = SpecialFunction.NoChange;
              break;
            case "<UC>":
              SpecialPhrases[parts[0]] = SpecialFunction.UpperCase;
              if (SpecialFunctions.TryGetValue(SpecialFunction.UpperCase, out var upperCaseFunction) && upperCaseFunction != parts[1])
                Debug.WriteLine($"Warning: Different upper case functions defined: '{upperCaseFunction}' and '{parts[1]}'");
              SpecialFunctions[SpecialFunction.UpperCase] = parts[1];
              break;
            case "<LC>":
              SpecialPhrases[parts[0]] = SpecialFunction.LowerCase;
              if (SpecialFunctions.TryGetValue(SpecialFunction.LowerCase, out var lowerCaseFunction) && lowerCaseFunction != parts[1])
                Debug.WriteLine($"Warning: Different lower case functions defined: '{lowerCaseFunction}' and '{parts[1]}'");
              SpecialFunctions[SpecialFunction.LowerCase] = parts[1];
              break;
            case "<TC>":
              SpecialPhrases[parts[0]] = SpecialFunction.TitleCase;
              if (SpecialFunctions.TryGetValue(SpecialFunction.TitleCase, out var titleCaseFunction) && titleCaseFunction != parts[1])
                Debug.WriteLine($"Warning: Different title case functions defined: '{titleCaseFunction}' and '{parts[1]}'");
              SpecialFunctions[SpecialFunction.TitleCase] = parts[1];
              break;
            case "<TN>":
              SpecialPhrases[parts[0]] = SpecialFunction.Tone;
              if (SpecialFunctions.TryGetValue(SpecialFunction.Tone, out var toneFunction) && toneFunction != parts[1])
                Debug.WriteLine($"Warning: Different tone functions defined: '{toneFunction}' and '{parts[1]}'");
              SpecialFunctions[SpecialFunction.Tone] = parts[1];
              break;
            case "<LG>":
              SpecialPhrases[parts[0]] = SpecialFunction.Ligature;
              if (SpecialFunctions.TryGetValue(SpecialFunction.Ligature, out var ligatureFunction) && ligatureFunction != parts[1])
                Debug.WriteLine($"Warning: Different ligature functions defined: '{ligatureFunction}' and '{parts[1]}'");
              SpecialFunctions[SpecialFunction.Ligature] = parts[1];
              break;
            default:
              if (!String.IsNullOrEmpty(parts[2]))
                Debug.WriteLine($"Unknown special function: {parts[2]}");
              break;
          }
        }
      }
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
        String.Format(Resources.Strings.UpdatingField, Resources.UcdCodePointStrings.Name);
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