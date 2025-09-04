using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

using Qhta.MVVM;
using Qhta.SF.WPF.Tools;
using Qhta.SF.WPF.Tools.Resources;
using Qhta.UndoManager;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Resources;
using Qhta.WPF.Converters;
using Syncfusion.Data.Extensions;
using DataStrings = Qhta.SF.WPF.Tools.Resources.Strings;

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
    UndoManager.UndoRedoManager.IsEnabled = false;
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
      foreach (var ngm in _Context.NameGenMethods.ToList())
      {
        NameGenMethodsList.Add(new NameGenMethodViewModel(ngm));
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
      //Debug.WriteLine($"codePointsArray.Length = {codePointsArray.Length}");
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
              int n = i + g * 1000;
              if (n >= codePointsArray.Count()) break;
              var cp = codePointsArray[n];
              UcdCodePoints.Add(cp);
            }
          }
          Application.Current.Dispatcher.Invoke(() =>
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

    UndoManager.UndoRedoManager.IsEnabled = true;
  }

  /// <summary>
  /// Code points collection representing Unicode code points.
  /// </summary>
  public UcdCodePointsCollection UcdCodePoints { [DebuggerStepThrough] get; set; } = new();

  /// <summary>
  /// Aliases of code point names.
  /// </summary>
  public AliasCollection Aliases { [DebuggerStepThrough] get; set; } = new();

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
  /// List of all name generation methods exposed to the UI.
  /// </summary>
  public List<NameGenMethodViewModel> NameGenMethodsList { [DebuggerStepThrough] get; } = new();
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
  /// Collection of name generation methods exposed to the UI as an array.
  /// </summary>
  public NameGenMethod[] NameGenMethods { [DebuggerStepThrough] get; } = Enum.GetValues<NameGenMethod>();
  /// <summary>
  /// Collection of Unicode categories exposed to the UI as an array.
  /// </summary>
  public Array Categories { [DebuggerStepThrough] get; } = Enum.GetNames(typeof(UcdCategory));
  /// <summary>
  /// Collection of selectable writing types exposed to the UI as an enumerable.
  /// </summary>
  public IEnumerable<SelectableItem> SelectableWritingSystemTypes
  {
    get
    {
      List<SelectableItem> list = new();
      list.Add(new SelectableItem { DisplayName = DataStrings.EmptyValue});
      foreach (var item in WritingSystemTypes)
        list.Add(new SelectableItem 
        { 
          Value=item, 
          ValueConverter= WritingSystemTypeValueConverter, 
          ToolTipConverter = WritingSystemTypeTooltipConverter
        });
      return list;
    }
  }
  /// <summary>
  /// Value converter for writing system types, converting enum values to localized strings.
  /// </summary>
  public static readonly IValueConverter WritingSystemTypeValueConverter =
    new EnumToResourceConverter { ResourceType = typeof(Resources.WritingSystemTypeStrings) };
  /// <summary>
  /// Value converter for writing system types, converting enum values to localized tooltips.
  /// </summary>
  public static readonly IValueConverter WritingSystemTypeTooltipConverter =
    new EnumToResourceConverter { ResourceType = typeof(Resources.WritingSystemTypeStrings), Suffix="Tooltip"};

  /// <summary>
  /// Collection of selectable writing kinds exposed to the UI as an enumerable.
  /// </summary>
  public IEnumerable<SelectableItem> SelectableWritingSystemKinds
  {
    get
    {
      List<SelectableItem> list = new();
      list.Add(new SelectableItem { DisplayName = DataStrings.EmptyValue });
      foreach (var item in WritingSystemKinds)
        list.Add(new SelectableItem
        {
          Value = item, 
          ValueConverter = WritingSystemKindValueConverter,
          ToolTipConverter = WritingSystemKindTooltipConverter
        });
      return list;
    }
  }
  /// <summary>
  /// Value converter for writing system kinds, converting enum values to localized strings.
  /// </summary>
  public static readonly IValueConverter WritingSystemKindValueConverter = 
    new EnumToResourceConverter { ResourceType = typeof(Resources.WritingSystemKindStrings) };
  /// <summary>
  /// Value converter for writing system kinds, converting enum values to localized tooltips.
  /// </summary>
  public static readonly IValueConverter WritingSystemKindTooltipConverter =
    new EnumToResourceConverter { ResourceType = typeof(Resources.WritingSystemKindStrings), Suffix = "Tooltip" };

  /// <summary>
  /// Collection of selectable Unicode categories exposed to the UI as an enumerable.
  /// </summary>
  public IEnumerable<UnicodeCategoryViewModel?> SelectableCategories
  {
    get
    {
      List<UnicodeCategoryViewModel?> list = new();
      list.Add(new UnicodeCategoryViewModel { DisplayName = DataStrings.EmptyValue });
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
      list.Add(new UcdBlockViewModel { DisplayName = DataStrings.EmptyValue });
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
  public IEnumerable<ISelectableItem> GetSelectableWritingSystems(params WritingSystemType[] types)
  {
    List<ISelectableItem> list = new();
    list.Add(new SelectableItem(){ DisplayName = DataStrings.EmptyValue });
    list.AddRange(_ViewModels.Instance.WritingSystems.Where(item => item.Type != null && types.Contains((WritingSystemType)item.Type))
      .OrderBy(vm => vm.FullName));
    return list;
  }

  /// <summary>
  /// Collection of selectable writing systems that can be used in the UI.
  /// </summary>
  public IEnumerable<ISelectableItem> SelectableWritingSystems =>
    GetSelectableWritingSystems(WritingSystemType.Area, WritingSystemType.Group, WritingSystemType.Script, WritingSystemType.Notation, WritingSystemType.SymbolSet);

  /// <summary>
  /// Collection of selectable areas, which are a type of writing system.
  /// </summary>
  public IEnumerable<ISelectableItem> SelectableAreas => GetSelectableWritingSystems(WritingSystemType.Area);

  /// <summary>
  /// Collection of selectable families, which are a type of writing system.
  /// </summary>
  public IEnumerable<ISelectableItem> SelectableScripts => GetSelectableWritingSystems(WritingSystemType.Script, WritingSystemType.Group);

  /// <summary>
  /// Collection of selectable languages, which are a type of writing system.
  /// </summary>
  public IEnumerable<ISelectableItem> SelectableLanguages => GetSelectableWritingSystems(WritingSystemType.Language);

  /// <summary>
  /// Collection of selectable notations, which are a type of writing system.
  /// </summary>
  public IEnumerable<ISelectableItem> SelectableNotations => GetSelectableWritingSystems(WritingSystemType.Notation);

  /// <summary>
  /// Collection of selectable symbol sets, which are a type of writing system.
  /// </summary>
  public IEnumerable<ISelectableItem> SelectableSymbolSets => GetSelectableWritingSystems(WritingSystemType.SymbolSet);

  /// <summary>
  /// C
  /// </summary>
  public IEnumerable<ISelectableItem> SelectableSubsets => GetSelectableWritingSystems(WritingSystemType.Subset);


  #region IDisposable implementation
  /// <summary>
  /// Dispatches the Dispose method to clean up resources.
  /// </summary>
  /// <param name="disposing"></param>
  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
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