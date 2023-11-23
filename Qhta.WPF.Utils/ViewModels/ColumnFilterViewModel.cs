using System.Security.Permissions;

namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Abstract view model of the filter stored and edited in ColumnFilterDialog.
/// </summary>
public abstract class ColumnFilterViewModel : ViewModel
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public ColumnFilterViewModel()
  {
    _Instance = this;
  }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public ColumnFilterViewModel(PropPath propPath, string columnName)
  {
    PropPath = propPath;
    ColumnName = columnName;
    _Instance = this;
  }

  /// <summary>
  /// Copying constructor that copies the data of the view model is needed
  /// because we need to edit its copy and after Cancel button is pressed
  /// the previous content remains unchanged.
  /// </summary>
  public ColumnFilterViewModel(ColumnFilterViewModel other)
  {
    PropPath = other.PropPath;
    _ColumnName = other.ColumnName;
    EditOpEnabled = true;
    DefaultOp = true;
    _Instance = this;
  }

  /// <summary>
  /// Returns specific filter or self.
  /// </summary>
  public ColumnFilterViewModel Instance
  {
    get { return _Instance; }
    set
    {
      if (_Instance != value)
      {
        _Instance = value;
        NotifyPropertyChanged(nameof(Instance));
      }
    }
  }
  private ColumnFilterViewModel _Instance;


  /// <summary>
  ///  Specifies what to do with a column filter.
  /// </summary>
  public FilterOperation Operation
  {
    get => _Operation;
    set
    {
      if (_Operation != value)
      {
        _Operation = value;
        NotifyOpChanged();
      }
    }
  }
  private FilterOperation _Operation;

  /// <summary>
  /// Specifies whether Edit operation is enabled. If not, then Add operation is enabled.
  /// </summary>
  public bool EditOpEnabled { get; set; }

  #region Individual boolean properties for Operation used in view.

  /// <summary>
  /// Specifies whether Operation is Add or Edit.
  /// </summary>
  public bool DefaultOp
  {
    get => Operation == FilterOperation.Add || Operation == FilterOperation.Edit;
    set
    {
      if (value)
      {
        if (EditOpEnabled)
          Operation = FilterOperation.Edit;
        else
          Operation = FilterOperation.Add;
      }
    }
  }


  /// <summary>
  /// Specifies whether Operation is Add.
  /// </summary>
  public bool AddOp { get => Operation == FilterOperation.Add; set { if (value) Operation = FilterOperation.Add; } }

  /// <summary>
  /// Specifies whether Operation is Edit.
  /// </summary>
  public bool EditOp { get => Operation == FilterOperation.Edit; set { if (value) Operation = FilterOperation.Edit; } }

  /// <summary>
  ///  Specifies whether Operation is Clear.
  /// </summary>
  public bool ClearOp
  {
    get => Operation == FilterOperation.Clear;
    set
    {
      if (value)
      {
        Operation = FilterOperation.Clear;
        ClearFilter();
      }
    }
  }

  private void NotifyOpChanged()
  {
    NotifyPropertyChanged(nameof(Operation));
    NotifyPropertyChanged(nameof(DefaultOp));
    NotifyPropertyChanged(nameof(AddOp));
    NotifyPropertyChanged(nameof(EditOp));
    NotifyPropertyChanged(nameof(ClearOp));
  }
  #endregion


  /// <summary>
  /// Holds info of column binding properties.
  /// As binding path may complex, it is an array of property info items, which values must be evaluated in cascade.
  /// </summary>
  public PropPath? PropPath
  {
    get { return _PropPath; }
    set
    {
      if (_PropPath != value)
      {
        _PropPath = value;
        NotifyPropertyChanged(nameof(PropPath));
      }
    }
  }
  private PropPath? _PropPath;

  /// <summary>
  /// Displayed name of column binding property.
  /// </summary>
  public string? ColumnName
  {
    get { return _ColumnName; }
    set
    {
      if (_ColumnName != value)
      {
        _ColumnName = value;
        NotifyPropertyChanged(nameof(ColumnName));
      }
    }
  }
  private string? _ColumnName;

  /// <summary>
  /// This method must create a copy of the original instance;
  /// </summary>
  /// <returns></returns>
  public abstract ColumnFilterViewModel CreateCopy();

  /// <summary>
  /// Creates Predicate basing on current properties.
  /// </summary>
  /// <returns>Predicate that takes a property value from the object.</returns>
  public abstract ColumnFilter? CreateFilter();

  /// <summary>
  /// Clear filter properties.
  /// </summary>
  public abstract void ClearFilter();
}
