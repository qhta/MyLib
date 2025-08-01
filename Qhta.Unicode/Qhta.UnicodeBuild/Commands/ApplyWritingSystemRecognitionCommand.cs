using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

using Qhta.SF.Tools;
using Qhta.TextUtils;
using Qhta.UndoManager;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to apply writing systems recognition in a set of Unicode code points. 
/// </summary>
public class ApplyWritingSystemRecognitionCommand : TimeConsumingCommand
{

  private record ApplyWritingSystemRecognitionArgs(List<UcdCodePointViewModel> ListOfPoints, WritingSystemType[]? SelectedWritingSystems);

  /// <inheritdoc/>
  public override void Execute(object? argument)
  {
    if (BackgroundWorker.IsBusy) return;
    if (argument is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Argument must be of type SfDataGrid.", nameof(argument));
    }
    if (!dataGrid.GetSelectedRowsAndColumns(out bool allColumnsSelected, out var selectedColumns, out bool allRowsSelected, out var selectedRows).Any()
        && !allColumnsSelected && !selectedColumns.Any() && !allRowsSelected && !selectedRows.Any())
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

    WritingSystemType[]? selectedWritingSystems = null;
    if (!allColumnsSelected && selectedColumns.Any())
    {
      selectedWritingSystems = selectedColumns
        .Select(col => col.MappingName)
        .Where(name => Enum.TryParse(name, out WritingSystemType _))
        .Select(name => (WritingSystemType)Enum.Parse(typeof(WritingSystemType), name))
        .OrderByDescending(type => type)
        .ToArray();
    }
    int n = listOfPoints.Count();

    if (MessageBox.Show(String.Format(Resources.Strings.ApplyWritingSystemsRecognitionConfirm, n), Resources.Strings.Confirm, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
      BackgroundWorker.RunWorkerAsync(new ApplyWritingSystemRecognitionArgs(listOfPoints, selectedWritingSystems));
  }

  /// <inheritdoc/>
  protected override void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    _ViewModels.Instance.UcdCodePoints.IsBusy = true;
    _ViewModels.Instance.UcdCodePoints.BreakCommand = BreakCommand; 
    if (e.Cancel)
    {
      _ViewModels.Instance.UcdCodePoints.IsBusy = true;
      return; // Check if the operation was canceled
    }
    ApplyWritingSystemRecognitions(worker, e);
  }

  /// <inheritdoc/>
  protected override void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    _ViewModels.Instance.UcdCodePoints.ProgressValue = e.ProgressPercentage;
  }


  private void ApplyWritingSystemRecognitions(BackgroundWorker worker, DoWorkEventArgs e)
  {
    if (!(e.Argument is ApplyWritingSystemRecognitionArgs argument))
    {
      throw new ArgumentException("Argument must be of type (List<UcdCodePointViewModel>, WritingSystemType[]).", nameof(e.Argument));
    }
    var listOfPoints = argument.ListOfPoints;
    WritingSystemType[] allowedWritingSystems = (argument.SelectedWritingSystems != null)
      ? argument.SelectedWritingSystems.ToArray()
      : [WritingSystemType.Subset, WritingSystemType.SymbolSet, WritingSystemType.Notation, WritingSystemType.Language, WritingSystemType.Script];
    try
    {
      InitWritingSystemCategoryPhraseMap();
      var n = listOfPoints.Count;
      RecognizedWritingSystemsCount = 0;
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

        WritingSystemViewModel[] oldWritingSystems = codePoint.GetWritingSystems(allowedWritingSystems)!.ToArray();


        var newWritingSystems = RecognizeWritingSystems(codePoint);
        foreach (var newWritingSystem in newWritingSystems)
        {
          if (newWritingSystem.Type == null ||
              oldWritingSystems.Any(item => item.Type == (WritingSystemType)newWritingSystem.Type))
            continue;
          codePoint.WritingSystem = newWritingSystem;
          RecognizedWritingSystemsCount++;
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

  private readonly Dictionary<UcdCategory, Dictionary<string, WritingSystemViewModel>> WritingSystemCategoryPhraseMap = new();
  private readonly Dictionary<string, WritingSystemViewModel> WritingSystemPhraseMap = new();
  /// <summary>
  /// Count of recognized writing systems during the last recognition operation.
  /// </summary>
  private int RecognizedWritingSystemsCount;

  /// <summary>
  /// Initializes the writing system category phrase map for efficient recognition of writing systems based on code point categories and phrases.
  /// </summary>
  /// <exception cref="InvalidOperationException"></exception>
  private void InitWritingSystemCategoryPhraseMap()
  {
    WritingSystemCategoryPhraseMap.Clear();
    WritingSystemPhraseMap.Clear();

    // We need to add first WritingSystems with concrete category
    foreach (var writingSystem in _ViewModels.Instance.WritingSystems.Where(item => item.Ctg != null && !item.Ctg.Contains('*')))
      AddWritingSystemCategoryPhraseMap(writingSystem);
    // Next, when we add WritingSystems with group categories (containing wildcard '*'),
    // we will omit those mappings which were added in the previous step.
    foreach (var writingSystem in _ViewModels.Instance.WritingSystems.Where(item => item.Ctg != null && item.Ctg.Contains('*')))
      AddWritingSystemCategoryPhraseMap(writingSystem);

    foreach (var writingSystem in _ViewModels.Instance.WritingSystems.Where(item => item.Ctg == null))
      AddWritingSystemCategoryPhraseMap(writingSystem);
  }

  private void AddWritingSystemCategoryPhraseMap(WritingSystemViewModel writingSystem)
  {
    if (String.IsNullOrEmpty(writingSystem.Ctg))
    {
      var phrase = writingSystem.KeyPhrase ?? "";
      if (string.IsNullOrEmpty(phrase)) return;
      if (!phrase.Contains('*'))
        phrase = phrase.Length == 0 ? "*" : "*" + phrase + "*"; // Ensure the phrase contains wildcards
      if (!WritingSystemPhraseMap.TryAdd(phrase, writingSystem))
      {
        throw new InvalidOperationException(String.Format(Resources.Strings.AmbiguousWritingSystemPhrase, writingSystem.Name));
      }
    }
    else
    {
      if (writingSystem.Ctg.EndsWith('*'))
      {
        int added = 0;
        foreach (var category in Enum.GetValues<UcdCategory>()
                   .Where(val => val.ToString()[0] == writingSystem.Ctg[0]))
        {
          AddWritingSystemCategoryPhraseMap(writingSystem, category, true);
          added++;
        }
        if (added == 0)
        {
          throw new InvalidOperationException(
            String.Format(Resources.Strings.InvalidWritingSystemCategory, writingSystem.Ctg, writingSystem.Name));
        }
      }
      else
      {
        var category = Enum
          .GetValues<UcdCategory>().FirstOrDefault(val => val.ToString() == writingSystem.Ctg);
        if (category == default(UcdCategory))
          throw new InvalidOperationException(
            String.Format(Resources.Strings.InvalidWritingSystemCategory, writingSystem.Ctg, writingSystem.Name));
        AddWritingSystemCategoryPhraseMap(writingSystem, category, false);
      }
    }
  }

  private void AddWritingSystemCategoryPhraseMap(WritingSystemViewModel writingSystem, UcdCategory category, bool categoryInGroup)
  {
    if (!WritingSystemCategoryPhraseMap.TryGetValue(category, out var categoryMap))
    {
      categoryMap = new Dictionary<string, WritingSystemViewModel>();
      WritingSystemCategoryPhraseMap[category] = categoryMap;
    }
    var phrase = writingSystem.KeyPhrase ?? "";
    if (!phrase.Contains('*'))
      phrase = phrase.Length == 0 ? "*" : "*" + phrase + "*"; // Ensure the phrase contains wildcards
    if (!categoryMap.TryAdd(phrase, writingSystem))
    {
      if (!String.IsNullOrEmpty(writingSystem.KeyPhrase))
        throw new InvalidOperationException(String.Format(Resources.Strings.AmbiguousWritingSystemCategory,
          writingSystem.Name));
    }
  }

  /// <summary>
  /// Recognizes the writing system for a given Unicode code point based on its category and description.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>

  private IEnumerable<WritingSystemViewModel> RecognizeWritingSystems(UcdCodePointViewModel codePoint)
  {
    if (codePoint.Ctg == null)
      throw new ArgumentNullException(nameof(codePoint.Ctg), String.Format(Resources.Strings.CodePointCategoryIsNull, codePoint.CP.ToString()));
    if (codePoint.Description == null)
      throw new ArgumentNullException(nameof(codePoint.Description), String.Format(Resources.Strings.CodePointCategoryIsNull, codePoint.CP.ToString()));
    List<WritingSystemViewModel> result = new List<WritingSystemViewModel>(); ;
    if (WritingSystemCategoryPhraseMap.TryGetValue((UcdCategory)codePoint.Ctg, out var categoryMap))
    {
      foreach (var entry in categoryMap)
      {
        var phrase = entry.Key;
        if (codePoint.Description.IsLike(phrase))
          result.Add(entry.Value);
      }
    }
    foreach (var entry in WritingSystemPhraseMap)
    {
      var phrase = entry.Key;
      if (codePoint.Description.IsLike(phrase))
      {
        if (!result.Contains(entry.Value))
          result.Add(entry.Value);
      }
    }
    return result;
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
      var count = RecognizedWritingSystemsCount;
      string msg = string.Format(Resources.Strings.ApplyWritingSystemsRecognitionCompleted,
        count);
      if (count == 0)
        msg = string.Join(" ", [msg, Resources.Strings.ApplyWritingSystemsRecognitionAdvice]);
      MessageBox.Show(msg,
        Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    _ViewModels.Instance.UcdCodePoints.IsBusy = false;
    _ViewModels.Instance.UcdCodePoints.ProgressValue = 0;
    _ViewModels.Instance.UcdCodePoints.StatusMessage = null;
  }

}