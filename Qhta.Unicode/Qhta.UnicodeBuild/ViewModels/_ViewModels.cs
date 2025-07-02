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

public partial class _ViewModels : IDisposable
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

      //int i = 0;
      foreach (var cp in _Context.CodePoints)
      {
        //if (i++ >= 1000) break;
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
    ApplyBlockMappingCommand = new RelayCommand(ApplyBlockMappingCommandExecute);
    BreakApplyBlockMappingCommand = new RelayCommand(BreakApplyBlockMappingCommandExecute);
    ApplyBlockMappingBackgroundWorker.DoWork += ApplyBlockMapping_DoWork;
    ApplyBlockMappingBackgroundWorker.ProgressChanged += ApplyBlockMapping_ProgressChanged;
    ApplyBlockMappingBackgroundWorker.RunWorkerCompleted += ApplyBlockMapping_RunWorkerCompleted;
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

  public IEnumerable<UcdBlockViewModel> SelectableBlocks
  {
    get
    {
      var list = _ViewModels.Instance.UcdBlocks.Where(item => !String.IsNullOrEmpty(item.Name)) // items empty names are not selectable
        .OrderBy(vm => vm.Name).ToList();
      list.Insert(0, dummyUcdBlockViewModel);
      return list;
    }
  }
  readonly UcdBlockViewModel dummyUcdBlockViewModel = new UcdBlockViewModel(new UcdBlock { BlockName = "" });

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

}