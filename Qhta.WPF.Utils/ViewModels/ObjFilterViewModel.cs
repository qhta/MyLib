namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// View model shown in ObjFilterView.
/// </summary>
public class ObjFilterViewModel : FilterViewModel
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public ObjFilterViewModel(ColumnViewInfo column, IObjectOwner? owner) : base(column, owner) { }

  /// <summary>
  /// Copying constructor.
  /// </summary>
  /// <returns></returns>
  public ObjFilterViewModel(ObjFilterViewModel other): base(other)
  {
    this.Function = other.Function;
  }

  /// <summary>
  /// Creates a copy of this instance;
  /// </summary>
  /// <returns></returns>
  public override FilterViewModel CreateCopy()
  {
    return new ObjFilterViewModel(this);
  }

  /// <summary>
  /// This method copies properties from the other instance of the same type.
  /// </summary>
  public override void CopyFrom(FilterViewModel? other)
  {
    if (other is ObjFilterViewModel otherFilter)
    {
      this.Function = otherFilter.Function;
    }
  }

  /// <summary>
  /// Clears the <see cref="Function"/> property.
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  public override void ClearFilter()
  {
    Function = null;
  }


  /// <summary>
  /// Selected predicate function.
  /// </summary>
  public ObjPredicateFunction? Function
  {
    get { return _Function; }
    set
    {
      if (_Function != value)
      {
        _Function = value;
        NotifyFunctionChanged();
      }
    }
  }
  private ObjPredicateFunction? _Function;

  private void NotifyFunctionChanged()
  {
    NotifyPropertyChanged(nameof(Function));
    foreach (var enumName in typeof(ObjPredicateFunction).GetEnumNames())
      NotifyPropertyChanged(enumName);
    NotifyPropertyChanged(nameof(CanCreateFilter));
  }

  #region Individual boolean properties for Function used in RadioButton.

  /// <summary>
  /// Specifies whether predicate function is IsEmpty.
  /// </summary>
  public bool IsEmpty { get => Function == ObjPredicateFunction.IsEmpty; set { if (value) Function = ObjPredicateFunction.IsEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is NotEmpty.
  /// </summary>
  public bool NotEmpty { get => Function == ObjPredicateFunction.NotEmpty; set { if (value) Function = ObjPredicateFunction.NotEmpty; } }

  #endregion

  /// <inheritdoc/>
  public override bool CanCreateFilter => Function!=null;

  /// <summary>
  /// Creates DataGridColumnFilter basing on current properties.
  /// </summary>
  /// <returns></returns>
  public override ColumnFilter? CreateFilter()
  {
    if (Function == null || PropPath == null)
      return null;
    Func<object?, object?, bool> compareFunction; 
    switch (Function)
    {
      case ObjPredicateFunction.IsEmpty:
        compareFunction = IsEmptyFunction;
        break;
      case ObjPredicateFunction.NotEmpty:
        compareFunction = NotEmptyFunction;
        break;
      default:
        return null;
    }
    var dataGridColumnFilter = new ColumnFilter(PropPath, compareFunction);
    return dataGridColumnFilter;
  }

  private bool IsEmptyFunction(object? propValue, object? otherValue)
  {
    return propValue == null;
  }

  private bool NotEmptyFunction(object? propValue, object? otherValue)
  {
    return propValue != null;
  }


  /// <inheritdoc/>
  public override string? ToString()
  {
    var str = $"{Column?.PropPath} {Function}";
    return str;
  }
}
