using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

using Microsoft.Win32;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Resources;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Command to apply writing system mappings from a file.
/// </summary>
public class ApplyWritingSystemMappingCommand : TimeConsumingCommand
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ApplyWritingSystemMappingCommand"/> class.
  /// </summary>
  public ApplyWritingSystemMappingCommand()
  {

  }

  /// <summary>
   /// Collection of writing system mappings to be applied to Unicode code points.
   /// </summary>
  public WritingSystemMappingCollection WritingSystemMappings { [DebuggerStepThrough] get; set; } = new();

  /// <inheritdoc/>
  public override void Execute(object? parameter)
  {
    if (BackgroundWorker.IsBusy) return;

    var dialog = new OpenFileDialog
    {
      FileName = "WritingSystems.txt", // Default file name
      DefaultExt = ".txt", // Default file extension
      Filter = Strings.TextFilesFilter,
      InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Resources")
    };


    // Show open file dialog box
    bool? result = dialog.ShowDialog();

    // Process open file dialog box results
    if (result == true)
    {
      // Open document
      string filename = dialog.FileName;
      if (!File.Exists(filename))
      {
        MessageBox.Show(String.Format(Resources.Strings.FileNotFound, filename), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      BackgroundWorker.RunWorkerAsync(filename);
    }
  }

  private void LoadWritingSystemsFile(BackgroundWorker worker, DoWorkEventArgs e)
  {
    if (e.Argument is not string filename)
    {
      throw new ArgumentException("Filename must be provided as an argument.", nameof(e.Argument));
    }
    WritingSystemMappings.Clear();
    string[] fileLines;
    try
    {
      fileLines = File.ReadAllLines(filename);
    }
    catch (Exception ex)
    {
      MessageBox.Show(String.Format(Resources.Strings.ErrorOcurred, ex.Message), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
      throw;
    }
    int n = fileLines.Length;
    int i = 0;
    _ViewModels.Instance.UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Loading, Path.GetFileName(filename));
    foreach (var line in fileLines)
    {
      i++;
      worker.ReportProgress((i) * 100 / n);
      //Debug.WriteLine($"Loading line {i++}");
      if (worker.CancellationPending)
      {
        e.Cancel = true; // Indicate that the operation was canceled
        return;
      }
      var s = line.Trim();
      if (string.IsNullOrEmpty(s) || s.StartsWith("#")) // Skip empty lines and comments
        continue;
      var k = s.IndexOf('#');
      if (k != -1) s = s.Substring(0, k).Trim(); // Remove comment part
      var ss = s.Split(new char[] { ';', '\t' });
      if (ss.Length < 2)
      {
        MessageBox.Show(String.Format(Resources.Strings.ErrorInLine, i), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        continue;
      }
      var range = ss[0].Trim();
      var WritingSystemName = ss[1].Trim().Replace("_", " ");
      if (string.IsNullOrEmpty(range) || string.IsNullOrEmpty(WritingSystemName))
      {
        MessageBox.Show(String.Format(Resources.Strings.ErrorInLine, i), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        continue;
      }
      WritingSystemMappings.Add(new WritingSystemMapping { Range = range, WritingSystemName = WritingSystemName });
    }
  }

  /// <inheritdoc/>
  protected override void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    LoadWritingSystemsFile(worker, e);
    _ViewModels.Instance.UcdCodePoints.IsBusy = true;
    _ViewModels.Instance.UcdCodePoints.BreakCommand = BreakCommand;
    WritingSystemViewModel? WritingSystem = null;
    var n = WritingSystemMappings.Count;
    var i = 0;
    _ViewModels.Instance.UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Updating, Resources.UcdCodePointStrings.WritingSystem);
    foreach (var mapping in WritingSystemMappings)
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
      if (WritingSystem == null || WritingSystem.Name != mapping.WritingSystem?.Name)
      {
        WritingSystem = _ViewModels.Instance.WritingSystems.FindByName(mapping.WritingSystem!.Name!);
        if (WritingSystem == null)
        {
          Debug.WriteLine($"WritingSystem {mapping.WritingSystem?.Name} not found.");
          continue;
        }
        var start = mapping.Range.Start;
        var end = mapping.Range.End ?? start;
        for (int cp = start; cp <= end; cp++)
        {
          var codePoint = _ViewModels.Instance.UcdCodePoints.FindById(cp);
          if (codePoint != null)
          {
            codePoint.WritingSystem = WritingSystem;
          }
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