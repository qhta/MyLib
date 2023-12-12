namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Abstract view model of the filter stored and edited in ColumnFilterDialog.
/// </summary>
public abstract class FilterViewModel : ViewModel
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public FilterViewModel()
  {
    AddFilterCommand = new RelayCommand<object>(AddFilterExecute, AddFilterCanExecute) { Name = "AddFilterCommand" };
    RemoveFilterCommand = new RelayCommand<object>(RemoveFilterExecute, RemoveFilterCanExecute) { Name = "RemoveFilterCommand" };
  }

  /// <summary>
  /// Simple initializing constructor.
  /// </summary>
  public FilterViewModel(IObjectOwner? owner): this()
  {
    Owner = owner;
  }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public FilterViewModel(PropPath? propPath, string? columnName, IObjectOwner? owner) : this(owner)
  {
    PropPath = propPath;
    ColumnName = columnName;
  }

  /// <summary>
  /// Copying constructor that copies the data of the view model is needed
  /// because we need to edit its copy and after Cancel button is pressed
  /// the previous content remains unchanged.
  /// </summary>
  public FilterViewModel(FilterViewModel other) : this(other.Owner)
  {
    PropPath = other.PropPath;
    ColumnName = other.ColumnName;
    EditOpEnabled = true;
    DefaultOp = true;
  }

  /// <summary>
  /// Returns self. 
  /// Set accessor not implemented but overriden in GenericColumnFilterViewModel.
  /// </summary>
  public virtual FilterViewModel? EditedInstance
  {
    [DebuggerStepThrough]
    get { return this; }
    set { }
  }

  /// <summary>
  /// Gets/sets owner of this filter.
  /// </summary>
  public IObjectOwner? Owner
  {
    [DebuggerStepThrough]
    get { return _Owner; }
    set
    {
      if (_Owner != value)
      {
        _Owner = value;
        NotifyPropertyChanged(nameof(Owner));
      }
    }
  }
  private IObjectOwner? _Owner;

  /// <summary>
  /// Info of the selected column
  /// </summary>
  public FilterableColumnInfo? Column
  {
    get
    {
      return _Column ?? (Owner as FilterViewModel)?.Column;
    }
    set
    {
      if (_Column != value)
      {
        _Column = value;
        NotifyPropertyChanged(nameof(Column));
      }
    }
  }
  private FilterableColumnInfo? _Column;


  /// <summary>
  /// Info of all filterable columns
  /// </summary>
  public FilterableColumns? Columns
  {
    get
    {
      if (Owner is FilterViewModel ownerModel)
        return ownerModel.Columns;
      return _Columns;
    }
    set
    {
      if (_Columns != value)
      {
        _Columns = value;
        NotifyPropertyChanged(nameof(Columns));
      }
    }
  }
  private FilterableColumns? _Columns;

  /// <summary>
  /// Gets a collection of filtered columns in the EditedInstance.
  /// In the basic implementation a 1-object collection containing edited instance column is returned. 
  /// </summary>
  public virtual FilterableColumns? GetFilteredColumns()
  {
    if (EditedInstance?.Column!=null)
    {
      var result = new FilterableColumns();
      if (EditedInstance.CanCreateFilter)
        result.Add(EditedInstance.Column);
      return result;
    }
    return null;
  }

  #region abstract methods
  /// <summary>
  /// This method must create a copy of the original instance;
  /// </summary>
  /// <returns></returns>
  public abstract FilterViewModel? CreateCopy();

  /// <summary>
  /// This method copies properties from the other instance of the same type.
  /// </summary>
  public abstract void CopyFrom(FilterViewModel? other);

  /// <summary>
  /// Observable property which tells if filter view model can create a filter.
  /// </summary>
  public abstract bool CanCreateFilter { get; }

  /// <summary>
  /// Creates Predicate basing on current properties.
  /// </summary>
  /// <returns>Predicate that takes a property value from the object.</returns>
  public abstract IFilter? CreateFilter();

  /// <summary>
  /// Clear filter properties.
  /// </summary>
  public abstract void ClearFilter();

  #endregion

  /// <summary>
  ///  Specifies what to do with a column filter.
  /// </summary>
  public FilterResultOperation ResultOperation
  {
    get => _ResultOperation;
    set
    {
      if (_ResultOperation != value)
      {
        _ResultOperation = value;
        NotifyOpChanged();
      }
    }
  }
  private FilterResultOperation _ResultOperation;

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
    get => ResultOperation == FilterResultOperation.Add || ResultOperation == FilterResultOperation.Edit;
    set
    {
      if (value)
      {
        if (EditOpEnabled)
          ResultOperation = FilterResultOperation.Edit;
        else
          ResultOperation = FilterResultOperation.Add;
      }
    }
  }


  /// <summary>
  /// Specifies whether Operation is Add.
  /// </summary>
  public bool AddOp { get => ResultOperation == FilterResultOperation.Add; set { if (value) ResultOperation = FilterResultOperation.Add; } }

  /// <summary>
  /// Specifies whether Operation is Edit.
  /// </summary>
  public bool EditOp { get => ResultOperation == FilterResultOperation.Edit; set { if (value) ResultOperation = FilterResultOperation.Edit; } }

  /// <summary>
  ///  Specifies whether Operation is Clear.
  /// </summary>
  public bool ClearOp
  {
    get => ResultOperation == FilterResultOperation.Clear;
    set
    {
      if (value)
      {
        ResultOperation = FilterResultOperation.Clear;
        ClearFilter();
      }
    }
  }

  private void NotifyOpChanged()
  {
    NotifyPropertyChanged(nameof(ResultOperation));
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

  #region AddFilterCommand

  /// <summary>
  /// This command adds AND/OR filter above this filter.
  /// </summary>
  public Command AddFilterCommand { get; private set; }

  /// <summary>
  /// In theory can always execute, but must check owner (and parameter).
  /// </summary>
  /// <returns></returns>
  protected virtual bool AddFilterCanExecute(object? parameter)
  {
    return Owner != null && parameter is FilterEditOperation;
  }

  /// <summary>
  /// Creates a new compound filter and adds this to it.
  /// In current owner changes edited instance to the compound filter.
  /// Also creates a new generic column filter and adds it to the owner filter as next operand.
  /// </summary>
  protected virtual void AddFilterExecute(object? parameter)
  {
    if (Owner != null && parameter is FilterEditOperation editOp)
    {
      BooleanOperation? op = null;
      switch (editOp)
      {
        case FilterEditOperation.AddAndAbove:
          op = BooleanOperation.And;
          break;
        case FilterEditOperation.AddOrAbove:
          op = BooleanOperation.Or;
          break;
      }
      if (op != null)
      {
        var compoundFilter = new CompoundFilterViewModel(Owner) { Operation = op };
        compoundFilter.Add(this);
        Owner.ChangeComponent(EditedInstance, compoundFilter);
        EditedInstance!.Owner = compoundFilter;
        var nextOp = new GenericColumnFilterViewModel(compoundFilter);
        compoundFilter.Add(nextOp);
      }
    }
  }
  #endregion

  #region RemoveFilterCommand

  /// <summary>
  /// This command removes this filter from owner AND/OR filter.
  /// </summary>
  public Command RemoveFilterCommand { get; private set; }

  /// <summary>
  /// Can execute only if has owner compound filter.
  /// </summary>
  /// <returns></returns>
  protected virtual bool RemoveFilterCanExecute(object? parameter)
  {
    return Owner is CompoundFilterViewModel;
  }

  /// <summary>
  /// Removes this filter from owner compound filter.
  /// </summary>
  protected virtual void RemoveFilterExecute(object? parameter)
  {
    if (Owner is CompoundFilterViewModel compoundFilter)
    {
      compoundFilter.Remove(this);
    }
  }
  #endregion

}
