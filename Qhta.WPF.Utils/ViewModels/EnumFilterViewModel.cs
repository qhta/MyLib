namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Specific ColumnFilterViewModel of enum property filter edited in EnumFilterView.
/// </summary>
public class EnumFilterViewModel : FilterViewModel
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public EnumFilterViewModel(Type enumType, PropPath propPath, string columnName, IObjectOwner? owner) : base(propPath, columnName, owner)
  {
    this.EnumType = enumType;
    this.Function = EnumPredicateFunction.IsEqual;
  }

  /// <summary>
  /// Copying constructor.
  /// </summary>
  /// <returns></returns>
  public EnumFilterViewModel(EnumFilterViewModel other) : base(other)
  {
    this.EnumType = other.EnumType;
    this.PropPath = other.PropPath;
    this.ColumnName = other.ColumnName;
    this.FilterValue = other.FilterValue;
    this.Function = other.Function;
  }

  /// <summary>
  /// Creates a copy of this instance;
  /// </summary>
  /// <returns></returns>
  public override FilterViewModel CreateCopy()
  {
    return new EnumFilterViewModel(this);
  }

  /// <summary>
  /// This method copies properties from the other instance of the same type.
  /// </summary>
  public override void CopyFrom(FilterViewModel? other)
  {
    if (other is EnumFilterViewModel otherFilter)
    {
      this.Function = otherFilter.Function;
      this.FilterValue = otherFilter.FilterValue;
    }
  }

  /// <summary>
  /// Clears the <see cref="FilterValue"/> and <see cref="Function"/> property.
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  public override void ClearFilter()
  {
    FilterValue = null;
    Function = null;
  }

  /// <summary>
  /// Enum type to compare.
  /// </summary>
  public Type EnumType { get; private set; }

  /// <summary>
  /// Simple filter value.
  /// </summary>
  public object? FilterValue
  {
    get { return _FilterValue; }
    set
    {
      if (!object.Equals(_FilterValue, value))
      {
        _FilterValue = value;
        NotifyPropertyChanged(nameof(FilterValue));
      }
    }
  }
  private object? _FilterValue;

  /// <summary>
  /// Array of enum values to select.
  /// </summary>
  public Array EnumValues => EnumType.GetEnumValues();

  /// <summary>
  /// Selected predicate function.
  /// </summary>
  public EnumPredicateFunction? Function
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
  private EnumPredicateFunction? _Function;

  private void NotifyFunctionChanged()
  {
    NotifyPropertyChanged(nameof(Function));
    foreach (var enumName in typeof(EnumPredicateFunction).GetEnumNames())
      NotifyPropertyChanged(enumName);
    NotifyPropertyChanged(nameof(CanCreateFilter));
  }

  #region Individual boolean properties for Function used in RadioButton.

  /// <summary>
  /// Specifies whether predicate function is IsEqual.
  /// </summary>
  public bool IsEqual { get => Function == EnumPredicateFunction.IsEqual; set { if (value) Function = EnumPredicateFunction.IsEqual; } }

  /// <summary>
  /// Specifies whether predicate function is NotEqual.
  /// </summary>
  public bool NotEqual { get => Function == EnumPredicateFunction.NotEqual; set { if (value) Function = EnumPredicateFunction.NotEqual; } }

  /// <summary>
  /// Specifies whether predicate function is IsEmpty.
  /// </summary>
  public bool IsEmpty { get => Function == EnumPredicateFunction.IsEmpty; set { if (value) Function = EnumPredicateFunction.IsEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is NotEmpty.
  /// </summary>
  public bool NotEmpty { get => Function == EnumPredicateFunction.NotEmpty; set { if (value) Function = EnumPredicateFunction.NotEmpty; } }

  #endregion

  /// <inheritdoc/>
  public override bool CanCreateFilter => Function!=null;

  /// <summary>
  /// Creates DataGridColumnFilter basing on current properties.
  /// </summary>
  /// <returns></returns>
  public override ColumnFilter? CreateFilter()
  {
    if (Function == null || PropPath==null)
      return null;
    Func<object?, object?, bool> compareFunction;
    bool needsFilterValue = true;
    switch (Function)
    {
      case EnumPredicateFunction.IsEmpty:
        compareFunction = IsEmptyFunction;
        needsFilterValue = false;
        break;
      case EnumPredicateFunction.NotEmpty:
        compareFunction = NotEmptyFunction;
        needsFilterValue = false;
        break;
      case EnumPredicateFunction.IsEqual:
        compareFunction = IsEqualFunction;
        break;
      case EnumPredicateFunction.NotEqual:
        compareFunction = NotEqualFunction;
        break;
      default:
        return null;
    }
    object? otherValue = null;
    if (needsFilterValue)
    {
      if (FilterValue==null)
      {
        MessageBox.Show(CommonStrings.UndeclaredCompareValue);
        return null;
      }
      otherValue = FilterValue;
    }

    var dataGridColumnFilter = new ColumnFilter(PropPath, compareFunction);
    dataGridColumnFilter.OtherValue = otherValue;
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

  private bool IsEqualFunction(object? propValue, object? otherValue)
  {
    if (propValue != null && otherValue != null)
      return propValue.Equals(otherValue);
    return false;
  }

  private bool NotEqualFunction(object? propValue, object? otherValue)
  {
    if (propValue != null && otherValue != null)
      return !propValue.Equals(otherValue);
    return false;
  }

}

