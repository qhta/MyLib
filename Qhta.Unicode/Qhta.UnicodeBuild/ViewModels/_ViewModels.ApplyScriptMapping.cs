using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Microsoft.VisualBasic;
using Microsoft.Win32;

using Qhta.DeepCopy;
using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels : IDisposable
{
  public ScriptMappingCollection ScriptMappings { get; set; } = new();

  public IRelayCommand ApplyScriptMappingCommand { get; }

  private void ApplyScriptMappingCommandExecute()
  {
    if (ApplyScriptMappingBackgroundWorker.IsBusy) return;

    var dialog = new OpenFileDialog();
    dialog.FileName = "Scripts.txt"; // Default file name
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
      ApplyScriptMappingBackgroundWorker.RunWorkerAsync(filename);
    }
  }

  private void ApplyScriptMapping_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    UcdCodePoints.IsBusy = true;
    LoadScriptsFile(worker, e);
    if (e.Cancel)
    {
      UcdCodePoints.IsBusy = true;
      return; // Check if the operation was canceled
    }
    ApplyScriptMappings(worker, e);
  }

  private void ApplyScriptMapping_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    UcdCodePoints.ProgressValue = e.ProgressPercentage;
  }

  private void LoadScriptsFile(BackgroundWorker worker, DoWorkEventArgs e)
  {
    if (e.Argument is not string filename)
    {
      throw new ArgumentException("Filename must be provided as an argument.", nameof(e.Argument));
    }
    ScriptMappings.Clear();
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
      var ss = s.Split(';');
      if (ss.Length < 2)
      {
        MessageBox.Show(String.Format(Resources.Strings.ErrorInLine, i), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        continue;
      }
      var range = ss[0].Trim();
      var scriptName = ss[1].Trim().Replace("_", " ");
      if (string.IsNullOrEmpty(range) || string.IsNullOrEmpty(scriptName))
      {
        MessageBox.Show(String.Format(Resources.Strings.ErrorInLine, i), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        continue;
      }
      ScriptMappings.Add(new ScriptMapping { Range = range, Script = scriptName });
    }
  }

  private void ApplyScriptMappings(BackgroundWorker worker, DoWorkEventArgs e)
  {
    WritingSystemViewModel? Script = null;
    var n = ScriptMappings.Count;
    var i = 0;
    UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Updating, Resources.CodePoint.WritingSystem);
    foreach (var mapping in ScriptMappings)
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
      if (Script == null || Script.Name != mapping.Script?.Name)
      {
        Script = _ViewModels.Instance.WritingSystems.FindByName(mapping.Script!.Name!);
        if (Script == null)
        {
          Debug.WriteLine($"Script {mapping.Script?.Name} not found.");
          continue;
        }
        var start = mapping.Range.Start;
        var end = mapping.Range.End ?? start;
        for (int cp = start; cp <= end; cp++)
        {
          var codePoint = UcdCodePoints.FindById(cp);
          if (codePoint != null && codePoint.WritingSystem == null)
          {
            //Debug.WriteLine($"Set Script {Script.Name} for cp {cp:X4}");
            codePoint.WritingSystem = Script;
          }
        }
      }
    }

  }

  public IRelayCommand BreakApplyScriptMappingCommand { get; }

  public void BreakApplyScriptMappingCommandExecute()
  {
    if (ApplyScriptMappingBackgroundWorker.IsBusy)
    {
      ApplyScriptMappingBackgroundWorker.CancelAsync();
    }
  }

  public BackgroundWorker ApplyScriptMappingBackgroundWorker = new BackgroundWorker
  {
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = true
  };


  private void ApplyScriptMapping_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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