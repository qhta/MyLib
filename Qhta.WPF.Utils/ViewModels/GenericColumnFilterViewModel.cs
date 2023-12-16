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
  public GenericColumnFilterViewModel(ColumnViewInfo column, IObjectOwner? owner) : base(column, owner)
  {
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
    SpecificFilter = CreateSpecificFilter();
  }

  /// <summary>
  /// Copying constructor that copies the data of the view model is needed
  /// because we need to edit its copy and after Cancel button is pressed
  /// the previous content remains unchanged.
  /// </summary>
  public GenericColumnFilterViewModel(FilterViewModel other) : base(other)
  {
    SpecificFilter = CreateSpecificFilter();
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
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
        var oldInstance = _SpecificFilter;
        if (_SpecificFilter != null)
          _SpecificFilter.PropertyChanged -= _SpecificFilter_PropertyChanged;
        _SpecificFilter = value;
        if (_SpecificFilter != null)
        {
          _SpecificFilter.Owner = this;
          _SpecificFilter.PropertyChanged += _SpecificFilter_PropertyChanged;
        }
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
  //private FilterViewModel? _OldInstance;


  private void GenericColumnFilterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs args)
  {
    //Debug.WriteLine($"PropertyChanged({e.PropertyName})");
    if (args.PropertyName == nameof(PropPath))
    {
      if (PropPath != null)
      {
        //Debug.WriteLine($"GenericColumnFilterViewModel_PropertyChanged(Property={args.PropertyName}, Value={PropPath})");
        if (SpecificFilter?.Column?.PropPath != PropPath)
          SpecificFilter = CreateSpecificFilter();
      }
    }
    if (args.PropertyName == nameof(Column))
    {
      //Debug.WriteLine($"GenericColumnFilterViewModel_PropertyChanged(Property={args.PropertyName}, Value={Column})");
    }
    if (args.PropertyName == nameof(SpecificFilter))
    {
      //if (Owner!=null)
      //  Owner.ChangeComponent(_OldInstance, SpecificFilter);
    }
    if (args.PropertyName == nameof(CanCreateFilter))
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
      Type? type = Column?.PropPath?.Last().PropertyType;
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
    Debug.Assert(Column!=null);
    var result = new GenericColumnFilterViewModel(Column,Owner);
    result.SpecificFilter = this.SpecificFilter?.CreateCopy();      
    return result;
  }

  /// <summary>
  /// This method copies properties from the other instance of the same type.
  /// </summary>
  public override void CopyFrom(FilterViewModel? other)
  {
    if (other is GenericColumnFilterViewModel otherFilter)
    {
      this.SpecificFilter = otherFilter.SpecificFilter;
    }
  }

  /// <summary>
  /// Creates a specific Filter.
  /// </summary>
  public FilterViewModel? CreateSpecificFilter()
  {
    //Debug.WriteLine($"CreateSpecificFilter()");
    //if (Column != null)
    //{
    //  Debug.Assert(Column.ColumnName!=null);
    //  return CreateSpecificFilter(Column, DataType, this);
    //}
    //else
    if (Column != null)
      return CreateSpecificFilter(Column, DataType, this);
    return null;
  }

  /// <summary>
  /// Creates a specific dataType Filter.
  /// </summary>
  public static FilterViewModel CreateSpecificFilter(ColumnViewInfo column, Type? dataType, IObjectOwner? owner)
  {
    if (dataType == typeof(string))
      return new TextFilterViewModel(column, owner);
    else
      if (dataType == typeof(bool))
      return new BoolFilterViewModel(column, owner);
    else
      if (dataType?.IsEnum == true)
      return new EnumFilterViewModel(column, owner);
    else
      if (dataType == typeof(int))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(uint))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(byte))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(sbyte))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(Int16))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(UInt16))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(Int64))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(UInt64))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(Single))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(Double))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(Decimal))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(DateTime))
      return new NumFilterViewModel(column, owner);
    else
      if (dataType == typeof(TimeSpan))
      return new NumFilterViewModel(column, owner);
    else
      return new ObjFilterViewModel(column, owner);
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

  /// <inheritdoc/>
  public override bool ContainsColumn(ColumnViewInfo column)
  {
    return SpecificFilter?.ContainsColumn(column) ?? false;
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
    var oldInstance = SpecificFilter;
    if (newComponent is FilterViewModel newFilter)
      SpecificFilter = newFilter;
    else
    if (newComponent == null)
      SpecificFilter = null;
    else
      throw new InvalidOperationException($"New Instance object must be a {nameof(FilterViewModel)}");
    if (Owner != null)
      Owner.ChangeComponent(oldInstance, SpecificFilter);
    return true;
  }

  /// <inheritdoc/>
  public override string? ToString()
  {
    return SpecificFilter?.ToString() ?? base.ToString();
  }
}
