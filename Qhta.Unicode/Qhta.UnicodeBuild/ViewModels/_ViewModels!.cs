using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Qhta.DeepCopy;
using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels : ViewModel, IDisposable
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

  public _DbContext DbContext
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

      foreach (var wsk in _Context.WritingSystemKinds.ToList())
      {
        WritingSystemKindsList.Add(new WritingSystemKindViewModel(wsk));
      }

      foreach (var uc in _Context.UnicodeCategories.ToList())
      {
        UnicodeCategoriesList.Add(new UnicodeCategoryViewModel(uc));
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

      var codePointsArray = _Context.CodePoints.ToArray();

      for (int i = 0; i < 1000; i++)
      {
        var cp = codePointsArray[i];
        UcdCodePoints.Add(cp);
      }

      Task.Factory.StartNew(() =>
      {
        for (int i = 1000; i < codePointsArray.Count(); i++)
        {
          var cp = codePointsArray[i];
          UcdCodePoints.Add(cp);
          if (i % 1000 == 0)
          {
            //UcdCodePoints = codePointsLoaded;
            // Report progress every 1000 items
            Application.Current.Dispatcher.Invoke(() =>
            {
              UcdCodePoints.ProgressValue = (i * 100) / codePointsArray.Count();
            }, DispatcherPriority.Background);
          }
        }
      }).ContinueWith(t =>
      {
 
        NotifyPropertyChanged(nameof(UcdCodePoints));
        UcdCodePoints.IsLoaded = true;
        UcdCodePoints.StatusMessage = null;
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    NewWritingSystemCommand = new RelayCommand<WritingSystemType?>(NewWritingSystemCommandExecute);
    EditWritingSystemCommand = new RelayCommand<WritingSystemViewModel>(EditWritingSystemCommandExecute);
    DeleteWritingSystemCommand = new RelayCommand<WritingSystemViewModel>(DeleteWritingSystemCommandExecute, CanDeleteWritingSystem);
    ApplyWritingSystemMappingCommand = new RelayCommand(ApplyWritingSystemMappingCommandExecute);
    BreakApplyWritingSystemMappingCommand = new RelayCommand(BreakApplyWritingSystemMappingCommandExecute);
    ApplyWritingSystemMappingBackgroundWorker.DoWork += ApplyWritingSystemMapping_DoWork;
    ApplyWritingSystemMappingBackgroundWorker.ProgressChanged += ApplyWritingSystemMapping_ProgressChanged;
    ApplyWritingSystemMappingBackgroundWorker.RunWorkerCompleted += ApplyWritingSystemMapping_RunWorkerCompleted;
    ApplyBlockMappingCommand = new RelayCommand(ApplyBlockMappingCommandExecute);
    BreakApplyBlockMappingCommand = new RelayCommand(BreakApplyBlockMappingCommandExecute);
    ApplyBlockMappingBackgroundWorker.DoWork += ApplyBlockMapping_DoWork;
    ApplyBlockMappingBackgroundWorker.ProgressChanged += ApplyBlockMapping_ProgressChanged;
    ApplyBlockMappingBackgroundWorker.RunWorkerCompleted += ApplyBlockMapping_RunWorkerCompleted;
    FillColumnCommand = new RelayCommand<object?>(FillColumnCommandExecute);
  }


  public UcdCodePointsCollection UcdCodePoints { get; set; } = new();
  public UcdBlocksCollection UcdBlocks { get; set; } = new();
  public WritingSystemsCollection WritingSystems { get; set; } = new();
  public List<WritingSystemTypeViewModel> WritingSystemTypesList { get; } = new();
  public List<WritingSystemKindViewModel> WritingSystemKindsList { get; } = new();
  public List<UnicodeCategoryViewModel> UnicodeCategoriesList { get; } = new();
  public Array WritingSystemTypes { get; } = Enum.GetValues(typeof(WritingSystemType));
  public Array WritingSystemKinds { get; } = Enum.GetValues(typeof(WritingSystemKind));
  public Array Categories { get; } = Enum.GetNames(typeof(UcdCategory));

  public IEnumerable<UnicodeCategoryViewModel?> SelectableCategories
  {
    get
    {
      List<UnicodeCategoryViewModel?> list = new();
      list.AddRange(_ViewModels.Instance.UnicodeCategoriesList.Where(item => !String.IsNullOrEmpty(item.Name))
        .OrderBy(vm => vm.Name));
      return list;
    }
  }

  public IEnumerable<UcdBlockViewModel?> SelectableBlocks
  {
    get
    {
      List<UcdBlockViewModel?> list = new();
      list.AddRange( _ViewModels.Instance.UcdBlocks.Where(item => !String.IsNullOrEmpty(item.Name))
        .OrderBy(vm => vm.Name));
      return list;
    }
  }
 
  public IEnumerable<WritingSystemViewModel?> GetSelectableWritingSystems(params WritingSystemType[] types)
  {
      List<WritingSystemViewModel?> list = new();
      list.AddRange(_ViewModels.Instance.WritingSystems.Where(item => item.Type !=null && types.Contains((WritingSystemType)item.Type))
        .OrderBy(vm => vm.FullName));
      return list;
  }

  public IEnumerable<WritingSystemViewModel?> SelectableWritingSystems =>
    GetSelectableWritingSystems(WritingSystemType.Area, WritingSystemType.Family, WritingSystemType.Script, WritingSystemType.Notation, WritingSystemType.SymbolSet);

  public IEnumerable<WritingSystemViewModel?> SelectableAreas => GetSelectableWritingSystems(WritingSystemType.Area);

  public IEnumerable<WritingSystemViewModel?> SelectableScripts =>
    GetSelectableWritingSystems(WritingSystemType.Script, WritingSystemType.Family);

  public IEnumerable<WritingSystemViewModel?> SelectableLanguages => GetSelectableWritingSystems(WritingSystemType.Language); 

  public IEnumerable<WritingSystemViewModel?> SelectableNotations => GetSelectableWritingSystems(WritingSystemType.Notation);

  public IEnumerable<WritingSystemViewModel?> SelectableSymbolSets => GetSelectableWritingSystems(WritingSystemType.SymbolSet);

  public IEnumerable<WritingSystemViewModel?> SelectableSubsets => GetSelectableWritingSystems(WritingSystemType.Subset);

  public IEnumerable<WritingSystemViewModel?> SelectableArtefacts => GetSelectableWritingSystems(WritingSystemType.Artefact);

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
    _ViewModels.Instance.DbContext.SaveChanges();
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

  private void ReleaseUnmanagedResources()
  {
    // TODO release unmanaged resources here
  }

  protected virtual void Dispose(bool disposing)
  {
    ReleaseUnmanagedResources();
    if (disposing)
    {
      ApplyBlockMappingBackgroundWorker.Dispose();
      ApplyWritingSystemMappingBackgroundWorker.Dispose();
      if (_Context != null)
      {
        _Context.Dispose();
        _Context = null;
      }
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  ~_ViewModels()
  {
    Dispose(false);
  }
}