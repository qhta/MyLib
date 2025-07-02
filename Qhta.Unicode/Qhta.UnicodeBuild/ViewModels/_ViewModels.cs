using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Microsoft.VisualBasic;

using Qhta.DeepCopy;
using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.ViewModels;

public class _ViewModels : IDisposable
{
  public static _ViewModels Instance
  {
    get
    {
      if (_Instance == null)
      {
        _Instance = new _ViewModels();
      }
      return _Instance;
    }
  }

  public _DbContext DBContext
  {
    get
    {
      if (_Context == null)
      {
        _Context = new _DbContext();
      }
      return _Context;
    }
  }

  private static _ViewModels? _Instance;
  private static _DbContext? _Context = null!;


  private _ViewModels()
  {
    _Context = new _DbContext();
    {
      foreach (var wst in _Context.WritingSystemTypes.ToList())
      {
        WritingSystemTypesList.Add(new WritingSystemTypeViewModel(wst));
      }

      foreach (var wsk in _Context.WritingSystemKinds.ToList())
      {
        WritingSystemKindsList.Add(new WritingSystemKindViewModel(wsk));
      }


      foreach (var ur in _Context.UcdRanges
                 //.Include(ub => ub.WritingSystem)
                 //.Include(ub => ub.UcdRanges)
                 .ToList())
      {
        UcdRanges.Add(ur);
        //Debug.WriteLine($"{ub.BlockName}.UcdRanges = {ub.UcdRanges.Count}");
      }

      foreach (var ub in _Context.UcdBlocks
                 .ToList())
      {
        UcdBlocks.Add(ub);
        //Debug.WriteLine($"{ub.BlockName}.UcdRanges = {ub.UcdRanges.Count}");
      }

      foreach (var ws in _Context.WritingSystems.ToList().OrderBy(ws => ws.Name).ToList())
      {
        WritingSystems.Add(ws);
      }
      WritingSystems.IsLoaded = true;

      int i = 0;
      foreach (var cp in _Context.CodePoints)
      {
        if (i++ > 1000) break;
        UcdCodePoints.Add(cp);
      }

    }

    NewWritingSystemCommand = new RelayCommand<WritingSystemType?>(NewWritingSystemCommandExecute);
    EditWritingSystemCommand = new RelayCommand<WritingSystemViewModel>(EditWritingSystemCommandExecute);
    DeleteWritingSystemCommand = new RelayCommand<WritingSystemViewModel>(DeleteWritingSystemCommandExecute, CanDeleteWritingSystem);
    ApplyScriptMappingCommand = new RelayCommand(ApplyScriptMappingCommandExecute);
    BreakApplyScriptMappingCommand = new RelayCommand(BreakApplyScriptMappingCommandExecute);
    ApplyScriptMappingBackgroundWorker.DoWork += ApplyScriptMapping_DoWork;
    ApplyScriptMappingBackgroundWorker.ProgressChanged += ApplyScriptMapping_ProgressChanged;
    ApplyScriptMappingBackgroundWorker.RunWorkerCompleted += ApplyScriptMapping_RunWorkerCompleted;

  }


  public UcdBlocksCollection UcdBlocks { get; set; } = new();
  public UcdRangeCollection UcdRanges { get; set; } = new();
  public WritingSystemsCollection WritingSystems { get; set; } = new();
  public List<WritingSystemTypeViewModel> WritingSystemTypesList { get; } = new();
  public List<WritingSystemKindViewModel> WritingSystemKindsList { get; } = new();
  public Array WritingSystemTypes { get; } = Enum.GetValues(typeof(WritingSystemType));
  public Array WritingSystemKinds { get; } = Enum.GetValues(typeof(WritingSystemKind));
  public Array Categories { get; } = Enum.GetNames(typeof(UcdCategory));
  public UcdCodePointsCollection UcdCodePoints { get; set; } = new();
  public ScriptMappingCollection ScriptMappings { get; set; } = new();

  public IEnumerable<WritingSystemViewModel> SelectableWritingSystems
  {
    get
    {
      var list = _ViewModels.Instance.WritingSystems.Where(item => !item.Name!.StartsWith("<")) // items with special names are not selectable
        .OrderBy(vm => vm.FullName).ToList();
      list.Insert(0, dummyWritingSystemViewModel);
      return list;
    }
  }
  readonly WritingSystemViewModel dummyWritingSystemViewModel = new WritingSystemViewModel(new WritingSystem { Name = "" });

  public IEnumerable<UcdRangeViewModel> SelectableRanges
  {
    get
    {
      var list = _ViewModels.Instance.UcdRanges.Where(item => !String.IsNullOrWhiteSpace(item.RangeName))
        .OrderBy(vm => vm.RangeName).ToList();
      list.Insert(0, dummyRangeViewModel);
      return list;
    }
  }
  readonly UcdRangeViewModel dummyRangeViewModel = new UcdRangeViewModel(new UcdRange { Range = "" });

  public void Dispose()
  {
    if (_Context != null)
    {
      _Context.SaveChanges();
      _Context.Dispose();
      _Context = null;
    }
  }

  public int GetNewWritingSystemId()
  {
    var lastItem = _Context?.WritingSystems.OrderByDescending(ws => ws.Id).FirstOrDefault();
    if (lastItem == null) return 1; // If no writing systems exist, start with ID 1.
    var maxId = lastItem.Id ?? 0;
    if (lastItem.Name == "" || lastItem.Name!.StartsWith("<"))
    {
      // If the last item has no name, or it has a special name we can use its ID directly.
      return maxId;
    }
    return maxId + 1;
  }

  public IRelayCommand NewWritingSystemCommand { get; }

  private void NewWritingSystemCommandExecute(WritingSystemType? newType)
  {
    var vm = FindEmptyWritingSystemViewModel();
    // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
    if (vm == null)
    {
      // No empty writing system view model found, create a new one
      vm = NewWritingSystem;
    }
    else
    {
      var existingItem = vm;
      vm = (WritingSystemViewModel?)DeepCopier
        .CopyFrom(existingItem); // Create a copy of the existing empty writing system view model
    }
    if (vm is null) return;
    vm.Type = newType;
    var vmWindow = new Views.EditWritingSystemWindow
    {
      AddMode = true,
      DataContext = vm,
      Owner = Application.Current.MainWindow,
    };
    vmWindow.ShowDialog();
  }

  private WritingSystemViewModel? FindEmptyWritingSystemViewModel()
  {
    var existingItem = _ViewModels.Instance.WritingSystems.LastOrDefault(item => item.Name!.StartsWith("<"));
    return existingItem;
  }

  public WritingSystemViewModel NewWritingSystem
  {
    get
    {
      var data = new WritingSystem();
      var viewModel = new WritingSystemViewModel(data);
      return viewModel;
    }
  }

  public RelayCommand<WritingSystemViewModel> DeleteWritingSystemCommand { get; }


  private void DeleteWritingSystemCommandExecute(WritingSystemViewModel? item)
  {
    if (item == null) return;
    if (!CanDeleteWritingSystem(item))
    {
      MessageBox.Show("Cannot delete writing system", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      return;
    }
    // Writing system entity is not really deleted, just marked as deleted
    item.Name = "<deleted>";
    if (item.Parent != null)
    {
      item.Parent.Children?.Remove(item);
      item.Parent.NotifyPropertyChanged(nameof(WritingSystemViewModel.Children));
    }
    foreach (var prop in item.Model.GetType().GetProperties())
    {
      if (prop.Name != nameof(WritingSystemViewModel.Id) && prop.Name != nameof(WritingSystemViewModel.Name) && prop.CanWrite)
      {
        prop.SetValue(item.Model, null);
      }
    }
    _ViewModels.Instance.DBContext.SaveChanges();
  }

  private bool CanDeleteWritingSystem(WritingSystemViewModel? item)
  {
    if (item == null) return false;

    var ok = !item.IsUsed;
    return ok;
  }

  public IRelayCommand EditWritingSystemCommand { get; }

  private void EditWritingSystemCommandExecute(WritingSystemViewModel? item)
  {
    var vmWindow = new Views.EditWritingSystemWindow
    {
      AddMode = false,
      DataContext = item,
      Owner = Application.Current.MainWindow,
    };
    vmWindow.ShowDialog();
  }

  public IRelayCommand ApplyScriptMappingCommand { get; }

  private void ApplyScriptMappingCommandExecute()
  {
    if (ApplyScriptMappingBackgroundWorker.IsBusy) return;

    var dialog = new Microsoft.Win32.OpenFileDialog();
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
    LoadScriptsFile(worker, e);
    if (e.Cancel) return; // Check if the operation was canceled
    ApplyMappings(worker, e);
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
    } catch (Exception ex)
    {
      MessageBox.Show(String.Format(Resources.Strings.ErrorOcurred, ex.Message), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
      throw;
    }
    int n = fileLines.Length;
    int i = 0;
    UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Loading,Path.GetFileName(filename));
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
      var scriptName = ss[1].Trim().Replace("_"," ");
      if (string.IsNullOrEmpty(range) || string.IsNullOrEmpty(scriptName))
      {
        MessageBox.Show(String.Format(Resources.Strings.ErrorInLine, i), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        continue;
      }
      ScriptMappings.Add(new ScriptMapping { Range = range, Script = scriptName });
    }
  }

  private void ApplyMappings(BackgroundWorker worker, DoWorkEventArgs e)
  {
    WritingSystemViewModel? script = null;
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
      if (script == null || script.Name != mapping.Script?.Name)
      {
        script = _ViewModels.Instance.WritingSystems.FindByName(mapping.Script!.Name!);
        if (script == null)
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
            //Debug.WriteLine($"Set script {script.Name} for cp {cp:X4}");
            codePoint.WritingSystem = script;
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
      MessageBox.Show(String.Format(Resources.Strings.ErrorOcurred,e.Error.Message), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
    }
    else
    {
      MessageBox.Show(Resources.Strings.OperationCompleted, Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    UcdCodePoints.ProgressValue = 0;
    UcdCodePoints.StatusMessage = null;
  }

}