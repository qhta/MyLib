namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Generic column filter view model that enables user to change data type
/// </summary>
public class GenericColumnFilterViewModel : FilterViewModel, IObjectOwner
{

  /// <summary>
  /// Default constructor
  /// </summary>
  public GenericColumnFilterViewModel(IObjectOwner? owner) : base(owner)
  {
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
    SpecificFilter = CreateSpecificFilter();
  }

  /// <summary>
  /// Initializing constructor
  /// </summary>
  public GenericColumnFilterViewModel(PropPath? propPath, string? columnName, IObjectOwner? owner) : base(propPath, columnName, owner)
  {
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
    SpecificFilter = CreateSpecificFilter();
  }

  /// <summary>
  /// Initializing constructor
  /// </summary>
  public GenericColumnFilterViewModel(ColumnFilter columnFilter, string columnName, IObjectOwner? owner) : base(columnFilter.PropPath, columnName, owner)
  {
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
    SpecificFilter = CreateSpecificFilter();
  }

  /// <summary>
  /// Copying constructor that copies the data of the view model is needed
  /// because we need to edit its copy and after Cancel button is pressed
  /// the previous content remains unchanged.
  /// </summary>
  public GenericColumnFilterViewModel(FilterViewModel other, string columnName) : base(other)
  {
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
    PropPath = other.PropPath;
    ColumnName = columnName;

  }

  /// <summary>
  /// Returns specific filter or self. Enables others to change it.
  /// </summary>
  public FilterViewModel? SpecificFilter
  {
    [DebuggerStepThrough]
    get { return _SpecificFilter; }
    set
    {
      if (_SpecificFilter != value)
      {
        _OldInstance = _SpecificFilter;
        if (_SpecificFilter != null)
          _SpecificFilter.PropertyChanged -= _SpecificFilter_PropertyChanged;
        _SpecificFilter = value;
        if (_SpecificFilter != null)
          _SpecificFilter.PropertyChanged += _SpecificFilter_PropertyChanged;
        NotifyPropertyChanged(nameof(SpecificFilter));
      }
    }
  }

  private void _SpecificFilter_PropertyChanged(object? sender, PropertyChangedEventArgs args)
  {
    if (sender != null && args.PropertyName != null)
    {
      if (args.PropertyName == nameof(CanCreateFilter))
        NotifyPropertyChanged(this, args.PropertyName);
    }
  }

  private FilterViewModel? _SpecificFilter;
  private FilterViewModel? _OldInstance;


  private void GenericColumnFilterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs args)
  {
    //Debug.WriteLine($"PropertyChanged({e.PropertyName})");
    if (args.PropertyName == nameof(PropPath))
    {
      if (PropPath != null)
      {
        if (SpecificFilter?.Column?.PropPath != PropPath)
          SpecificFilter = CreateSpecificFilter();
      }
    }
    if (args.PropertyName == nameof(Column))
    {
      PropPath = Column?.PropPath;
    }
    if (args.PropertyName == nameof(SpecificFilter))
    {
      //if (Owner!=null)
      //  Owner.ChangeComponent(_OldInstance, SpecificFilter);
    }
    if (args.PropertyName==nameof(CanCreateFilter))
    {
      //    NotifyPropertyChanged(this, args.PropertyName);
    }
  }

  /// <summary>
  /// Data type of selected property.
  /// </summary>
  public Type? DataType
  {
    [DebuggerStepThrough]
    get
    {
      var type = PropPath?.Last().PropertyType;
      return type;
    }
  }

  ///// <summary>
  ///// Specific filter which is changed on dataType change.
  ///// </summary>

  //public FilterViewModel? SpecificFilter
  //{
  //  get { return _SpecificFilter; }
  //  set
  //  {
  //    if (_SpecificFilter != value)
  //    {
  //      _SpecificFilter = value;
  //      NotifyPropertyChanged(nameof(SpecificFilter));
  //      if (value != null)
  //        Instance = value;
  //    }
  //  }
  //}
  //private FilterViewModel? _SpecificFilter;

  /// <summary>
  /// Implements abstract CreateCopy
  /// </summary>
  /// <returns></returns>
  public override FilterViewModel CreateCopy()
  {
    //if (PropPath != null && ColumnName != null)
    //  return CreateSpecificFilter(PropPath, ColumnName, DataType, Owner);
    return new GenericColumnFilterViewModel(PropPath, ColumnName, Owner);
  }

  /// <summary>
  /// Creates a specific Filter.
  /// </summary>
  public FilterViewModel? CreateSpecificFilter()
  {
    if (PropPath != null && ColumnName != null)
      return CreateSpecificFilter(PropPath, ColumnName, DataType, this);
    else
    if (Column != null)
      return CreateSpecificFilter(Column.PropPath!, Column.ColumnName, DataType, this);
    return null;
  }

  /// <summary>
  /// Creates a specific dataType Filter.
  /// </summary>
  public static FilterViewModel CreateSpecificFilter(PropPath propPath, string columnName, Type? dataType, IObjectOwner? owner)
  {
    if (dataType == typeof(string))
      return new TextFilterViewModel(propPath, columnName, owner);
    else
      if (dataType == typeof(bool))
      return new BoolFilterViewModel(propPath, columnName, owner);
    else
      if (dataType?.IsEnum == true)
      return new EnumFilterViewModel(dataType, propPath, columnName, owner);
    else
      if (dataType == typeof(int))
      return new NumFilterViewModel<int>(propPath, columnName, owner);
    else
      if (dataType == typeof(uint))
      return new NumFilterViewModel<uint>(propPath, columnName, owner);
    else
      if (dataType == typeof(byte))
      return new NumFilterViewModel<byte>(propPath, columnName, owner);
    else
      if (dataType == typeof(sbyte))
      return new NumFilterViewModel<sbyte>(propPath, columnName, owner);
    else
      if (dataType == typeof(Int16))
      return new NumFilterViewModel<Int16>(propPath, columnName, owner);
    else
      if (dataType == typeof(UInt16))
      return new NumFilterViewModel<UInt16>(propPath, columnName, owner);
    else
      if (dataType == typeof(Int64))
      return new NumFilterViewModel<Int64>(propPath, columnName, owner);
    else
      if (dataType == typeof(UInt64))
      return new NumFilterViewModel<UInt64>(propPath, columnName, owner);
    else
      if (dataType == typeof(Single))
      return new NumFilterViewModel<Single>(propPath, columnName, owner);
    else
      if (dataType == typeof(Double))
      return new NumFilterViewModel<Double>(propPath, columnName, owner);
    else
      if (dataType == typeof(Decimal))
      return new NumFilterViewModel<Decimal>(propPath, columnName, owner);
    else
      if (dataType == typeof(DateTime))
      return new NumFilterViewModel<DateTime>(propPath, columnName, owner);
    else
      if (dataType == typeof(TimeSpan))
      return new NumFilterViewModel<TimeSpan>(propPath, columnName, owner);
    else
      return new ObjFilterViewModel(propPath, columnName, owner);
  }

  /// <summary>
  /// Tells if the specific ColumnFilter can create a filter.
  /// </summary>
  public override bool CanCreateFilter
  {
    get
    {
      if (SpecificFilter != this && SpecificFilter != null)
        return SpecificFilter.CanCreateFilter;
      return false;
    }
  }

  /// <summary>
  /// Creates a specific ColumnFilter.
  /// </summary>
  /// <returns></returns>
  public override IFilter? CreateFilter()
  {
    if (SpecificFilter != this)
      return SpecificFilter?.CreateFilter();
    throw new InvalidOperationException($"Cant create filter in {GetType()}");
  }

  /// <summary>
  /// Clears the specific filter
  /// </summary>
  public override void ClearFilter()
  {
    SpecificFilter?.ClearFilter();
  }

  /// <summary>
  /// Returns the Instance component.
  /// </summary>
  public object? GetComponent(string propName)
  {
    if (propName != nameof(SpecificFilter))
      throw new InvalidOperationException($"You can get only {nameof(SpecificFilter)} component");
    return SpecificFilter;
  }

  /// <summary>
  /// Changes the Instance component to new instance.
  /// </summary>
  public bool ChangeComponent(string propName, object? newComponent)
  {
    if (propName != nameof(SpecificFilter))
      throw new InvalidOperationException($"To change and Instance object an old object mus be equal");
    if (newComponent is FilterViewModel newFilter)
      SpecificFilter = newFilter;
    else
    if (newComponent == null)
      SpecificFilter = null;
    else
      throw new InvalidOperationException($"New Instance object must be a {nameof(FilterViewModel)}");
    return true;
  }

  /// <summary>
  /// Changes the Instance component to new instance.
  /// </summary>
  public bool ChangeComponent(object? oldComponent, object? newComponent)
  {
    if (oldComponent != SpecificFilter)
      throw new InvalidOperationException($"To change and Instance object an old object mus be equal");
    if (newComponent is FilterViewModel newFilter)
      SpecificFilter = newFilter;
    else
    if (newComponent == null)
      SpecificFilter = null;
    else
      throw new InvalidOperationException($"New Instance object must be a {nameof(FilterViewModel)}");
    if (Owner != null)
      Owner.ChangeComponent(_OldInstance, SpecificFilter);
    return true;
  }
}
