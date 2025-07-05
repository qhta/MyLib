using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

using Microsoft.Win32;

using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  public WritingSystemMappingCollection WritingSystemMappings { get; set; } = new();

  public IRelayCommand ApplyWritingSystemMappingCommand { get; }

  private void ApplyWritingSystemMappingCommandExecute()
  {
    if (ApplyWritingSystemMappingBackgroundWorker.IsBusy) return;

    var dialog = new OpenFileDialog();
    //dialog.FileName = "WritingSystems.txt"; // Default file name
    dialog.DefaultExt = ".txt"; // Default file extension
    dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
    dialog.InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Resources");

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
      ApplyWritingSystemMappingBackgroundWorker.RunWorkerAsync(filename);
    }
  }

  private void ApplyWritingSystemMapping_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    UcdCodePoints.IsBusy = true;
    LoadWritingSystemsFile(worker, e);
    if (e.Cancel)
    {
      UcdCodePoints.IsBusy = true;
      return; // Check if the operation was canceled
    }
    ApplyWritingSystemMappings(worker, e);
  }

  private void ApplyWritingSystemMapping_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    UcdCodePoints.ProgressValue = e.ProgressPercentage;
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
    UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Loading, Path.GetFileName(filename));
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
      var ss = s.Split(new char[]{';', '\t'});
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

  private void ApplyWritingSystemMappings(BackgroundWorker worker, DoWorkEventArgs e)
  {
    WritingSystemViewModel? WritingSystem = null;
    var n = WritingSystemMappings.Count;
    var i = 0;
    UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Updating, Resources.CodePoint.WritingSystem);
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
          var codePoint = UcdCodePoints.FindById(cp);
          if (codePoint != null)
          {
            //Debug.WriteLine($"Set WritingSystem {WritingSystem.Name} for cp {cp:X4}");
            switch (WritingSystem.Type)
            {
              case WritingSystemType.Area:
                codePoint.Area = WritingSystem;
                break;
              case WritingSystemType.Family:
              case WritingSystemType.Script:
                codePoint.Script = WritingSystem;
                break;
              case WritingSystemType.Language:
                codePoint.Language = WritingSystem;
                break;
              case WritingSystemType.Notation:
                codePoint.Notation = WritingSystem;
                break;
              case WritingSystemType.SymbolSet:
                codePoint.SymbolSet = WritingSystem;
                break;
              case WritingSystemType.Subset:
                codePoint.Subset = WritingSystem;
                break;
              case WritingSystemType.Artefact:
                codePoint.Artefact = WritingSystem;
                break;
              default:
                Debug.WriteLine($"Unknown WritingSystem type {WritingSystem.Type} for {WritingSystem.Name}");
                break;
            }
          }
        }
      }
    }

  }

  public IRelayCommand BreakApplyWritingSystemMappingCommand { get; }

  public void BreakApplyWritingSystemMappingCommandExecute()
  {
    if (ApplyWritingSystemMappingBackgroundWorker.IsBusy)
    {
      ApplyWritingSystemMappingBackgroundWorker.CancelAsync();
    }
  }

  public BackgroundWorker ApplyWritingSystemMappingBackgroundWorker = new BackgroundWorker
  {
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = true
  };


  private void ApplyWritingSystemMapping_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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