using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

using Qhta.MVVM;
using Qhta.UndoManager;
using Qhta.Unicode.Models;
using Syncfusion.Data.Extensions;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// Manages the view models for the application.
/// It is a singleton class that provides access to the database context and various collections of view models.
/// </summary>
public partial class _ViewModels : ViewModel, IDisposable
{
  /// <summary>
  /// Instance property to access the singleton instance of the _ViewModels class.
  /// </summary>
  public static _ViewModels Instance
  {
    [DebuggerStepThrough]
    get
    {
      if (_Instance == null)
      {
        _Instance = new _ViewModels();
      }
      return _Instance;
    }
  }

  /// <summary>
  /// Gets the database context for the application.
  /// </summary>
  public _DbContext DbContext
  {
    [DebuggerStepThrough]
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
    UndoMgr.Enabled = false;
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

      foreach (var ws in _Context.WritingSystems.ToList()/*.OrderBy(ws => ws.Name).ToList()*/)
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
        for (int g = 1; g <= codePointsArray.Count() / 1000; g++)
        {
          int i = 0;
          lock (UcdCodePoints)
          {
            for (; i < 1000; i++)
            {
              if (i + g * 1000 >= codePointsArray.Count()) break;
              var cp = codePointsArray[g];
              UcdCodePoints.Add(cp);
            }
          }
          Application.Current.Dispatcher.BeginInvoke(() =>
            {
              UcdCodePoints.ProgressValue = (i * 100) / codePointsArray.Count();
            });
          //Thread.Sleep(100);

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
    UndoMgr.Enabled = true;
  }

  /// <summary>
  /// Code points collection representing Unicode code points.
  /// </summary>
  public UcdCodePointsCollection UcdCodePoints { [DebuggerStepThrough] get; set; } = new();
  /// <summary>
  /// Blocks collection representing Unicode blocks.
  /// </summary>
  public UcdBlocksCollection UcdBlocks { [DebuggerStepThrough] get; set; } = new();
  /// <summary>
  /// Writing systems collection representing various writing systems.
  /// </summary>
  public WritingSystemsCollection WritingSystems { [DebuggerStepThrough] get; set; } = new();
  /// <summary>
  /// List of all writing system types exposed to the UI.
  /// </summary>
  public List<WritingSystemTypeViewModel> WritingSystemTypesList { [DebuggerStepThrough] get; } = new();
  /// <summary>
  /// List of all writing system kinds exposed to the UI.
  /// </summary>
  public List<WritingSystemKindViewModel> WritingSystemKindsList { [DebuggerStepThrough] get; } = new();
  /// <summary>
  /// List of all Unicode categories exposed to the UI.
  /// </summary>
  public List<UnicodeCategoryViewModel> UnicodeCategoriesList { [DebuggerStepThrough] get; } = new();
  /// <summary>
  /// Collection of writing system types exposed to the UI as an array.
  /// </summary>
  public WritingSystemType[] WritingSystemTypes { [DebuggerStepThrough] get; } = Enum.GetValues<WritingSystemType>();

  /// <summary>
  /// Collection of writing system kinds exposed to the UI as an array.
  /// </summary>
  public WritingSystemKind[] WritingSystemKinds { [DebuggerStepThrough] get; } = Enum.GetValues<WritingSystemKind>();
  /// <summary>
  /// Collection of Unicode categories exposed to the UI as an array.
  /// </summary>
  public Array Categories { [DebuggerStepThrough] get; } = Enum.GetNames(typeof(UcdCategory));

  /// <summary>
  /// Collection of selectable writing types exposed to the UI as an enumerable.
  /// </summary>
  public IEnumerable<WritingSystemType?> SelectableWritingSystemTypes
  {
    get
    {
      List<WritingSystemType?> list = new();
      foreach (var item in WritingSystemTypes)
        list.Add(item);
      return list;
    }
  }

  /// <summary>
  /// Collection of selectable writing kinds exposed to the UI as an enumerable.
  /// </summary>
  public IEnumerable<WritingSystemKind?> SelectableWritingSystemKinds
  {
    get
    {
      List<WritingSystemKind?> list = new();
      foreach (var item in WritingSystemKinds)
        list.Add(item);
      return list;
    }
  }

  /// <summary>
  /// Collection of selectable Unicode categories exposed to the UI as an enumerable.
  /// </summary>
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

  /// <summary>
  /// Collection of selectable Unicode blocks exposed to the UI as an enumerable.
  /// </summary>
  public IEnumerable<UcdBlockViewModel?> SelectableBlocks
  {
    get
    {
      List<UcdBlockViewModel?> list = new();
      list.AddRange(_ViewModels.Instance.UcdBlocks.Where(item => !String.IsNullOrEmpty(item.Name))
        .OrderBy(vm => vm.Name));
      return list;
    }
  }

  /// <summary>
  /// Collection of selectable writing systems based on their types.
  /// </summary>
  /// <param name="types"></param>
  /// <returns></returns>
  public IEnumerable<WritingSystemViewModel?> GetSelectableWritingSystems(params WritingSystemType[] types)
  {
    List<WritingSystemViewModel?> list = new();
    list.AddRange(_ViewModels.Instance.WritingSystems.Where(item => item.Type != null && types.Contains((WritingSystemType)item.Type))
      .OrderBy(vm => vm.FullName));
    return list;
  }

  /// <summary>
  /// Collection of selectable writing systems that can be used in the UI.
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableWritingSystems =>
    GetSelectableWritingSystems(WritingSystemType.Area, WritingSystemType.Family, WritingSystemType.Script, WritingSystemType.Notation, WritingSystemType.SymbolSet);

  /// <summary>
  /// Collection of selectable areas, which are a type of writing system.
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableAreas => GetSelectableWritingSystems(WritingSystemType.Area);

  /// <summary>
  /// Collection of selectable families, which are a type of writing system.
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableScripts => GetSelectableWritingSystems(WritingSystemType.Script, WritingSystemType.Family);

  /// <summary>
  /// Collection of selectable languages, which are a type of writing system.
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableLanguages => GetSelectableWritingSystems(WritingSystemType.Language);

  /// <summary>
  /// Collection of selectable notations, which are a type of writing system.
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableNotations => GetSelectableWritingSystems(WritingSystemType.Notation);

  /// <summary>
  /// Collection of selectable symbol sets, which are a type of writing system.
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableSymbolSets => GetSelectableWritingSystems(WritingSystemType.SymbolSet);

  /// <summary>
  /// C
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableSubsets => GetSelectableWritingSystems(WritingSystemType.Subset);

  /// <summary>
  /// Collection of selectable artefacts, which are a type of writing system.
  /// </summary>
  public IEnumerable<WritingSystemViewModel?> SelectableArtefacts => GetSelectableWritingSystems(WritingSystemType.Artefact);

  /// <summary>
  /// Gets the next available writing system ID.
  /// </summary>
  /// <returns></returns>
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

  #region IDisposable implementation
  /// <summary>
  /// Dispatches the Dispose method to clean up resources.
  /// </summary>
  /// <param name="disposing"></param>
  protected virtual void Dispose(bool disposing)
  {
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

  /// <summary>
  /// Disposes the _ViewModels instance and releases resources.
  /// </summary>
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// Destructs the _ViewModels instance and releases resources.
  /// </summary>
  ~_ViewModels()
  {
    Dispose(false);
  }
  #endregion
}