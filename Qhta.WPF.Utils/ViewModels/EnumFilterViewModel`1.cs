namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Generic view model for numeric values filter.
/// </summary>
public class EnumFilterViewModel<T> : EnumFilterViewModel where T : struct, IComparable<T>, IEquatable<T>
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public EnumFilterViewModel(PropertyInfo[] propPath, string propName) : base(typeof(T), propPath, propName)
  {
    this.PropPath = propPath;
    this.PropName = propName;
    this.Function = EnumPredicateFunction.IsEqual;
  }

  /// <summary>
  /// Copying constructor.
  /// </summary>
  /// <returns></returns>
  public EnumFilterViewModel(EnumFilterViewModel<T> other) : base(other)
  {
    this.PropPath = other.PropPath;
    this.PropName = other.PropName;
    this.Function = other.Function;
  }

  /// <summary>
  /// Creates a copy of this instance;
  /// </summary>
  /// <returns></returns>
  public override ColumnFilterViewModel CreateCopy()
  {
    return new EnumFilterViewModel<T>(this);
  }

  /// <summary>
  /// Simple filter text.
  /// </summary>
  public new T? FilterValue
  {
    get { return _FilterValue; }
    set
    {
      if (!_FilterValue.Equals(value))
      {
        _FilterValue = value;
        NotifyPropertyChanged(nameof(FilterValue));
      }
    }
  }
  private T? _FilterValue;


  private void NotifyFunctionChanged()
  {
    NotifyPropertyChanged(nameof(Function));
    foreach (var enumName in typeof(EnumPredicateFunction).GetEnumNames())
      NotifyPropertyChanged(enumName);
  }

  #region Individual boolean properties for Function used in RadioButton.

  /// <summary>
  /// Specifies whether predicate function is IsEqual.
  /// </summary>
  public new bool IsEqual { get => Function == EnumPredicateFunction.IsEqual; set { if (value) Function = EnumPredicateFunction.IsEqual; } }

  /// <summary>
  /// Specifies whether predicate function is NotEqual.
  /// </summary>
  public new bool NotEqual { get => Function == EnumPredicateFunction.NotEqual; set { if (value) Function = EnumPredicateFunction.NotEqual; } }

  /// <summary>
  /// Specifies whether predicate function is IsEmpty.
  /// </summary>
  public new bool IsEmpty { get => Function == EnumPredicateFunction.IsEmpty; set { if (value) Function = EnumPredicateFunction.IsEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is NotEmpty.
  /// </summary>
  public new bool NotEmpty { get => Function == EnumPredicateFunction.NotEmpty; set { if (value) Function = EnumPredicateFunction.NotEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is IsGreater.
  /// </summary>
  public bool IsGreater { get => Function == EnumPredicateFunction.IsGreater; set { if (value) Function = EnumPredicateFunction.IsGreater; } }

  /// <summary>
  /// Specifies whether predicate function is NotGreater.
  /// </summary>
  public bool NotGreater { get => Function == EnumPredicateFunction.NotGreater; set { if (value) Function = EnumPredicateFunction.NotGreater; } }

  /// <summary>
  /// Specifies whether predicate function is IsLess.
  /// </summary>
  public bool IsLess { get => Function == EnumPredicateFunction.IsLess; set { if (value) Function = EnumPredicateFunction.IsLess; } }

  /// <summary>
  /// Specifies whether predicate function is NotLess.
  /// </summary>
  public bool NotLess { get => Function == EnumPredicateFunction.NotLess; set { if (value) Function = EnumPredicateFunction.NotLess; } }

  #endregion

  /// <summary>
  /// Creates DataGridColumnFilter basing on current properties.
  /// </summary>
  /// <returns></returns>
  public override ColumnFilter? CreateFilter()
  {
    if (Function == null)
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
      case EnumPredicateFunction.IsGreater:
        compareFunction = IsGreaterFunction;
        break;
      case EnumPredicateFunction.NotGreater:
        compareFunction = NotGreaterFunction;
        break;
      case EnumPredicateFunction.IsLess:
        compareFunction = IsLessFunction;
        break;
      case EnumPredicateFunction.NotLess:
        compareFunction = NotLessFunction;
        break;
      default:
        return null;
    }
    object? otherValue = null;
    if (needsFilterValue)
    {
      if (FilterValue!=null)
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

  private bool TryParseFilterText(string filterText, out object? value)
  {
    if (Enum.TryParse<T>(filterText, out var valInt))
    {
      value = valInt;
      return true;
    }

    MessageBox.Show(CommonStrings.NumFilter_parse_error);
    value = null;
    return false;
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
    if (propValue is T value && otherValue is T other)
      return value.CompareTo(other) == 0;
    return false;
  }

  private bool NotEqualFunction(object? propValue, object? otherValue)
  {
    if (propValue is T value && otherValue is T other)
      return value.CompareTo(other) != 0;
    return false;
  }

  private bool IsGreaterFunction(object? propValue, object? otherValue)
  {
    if (propValue is T value && otherValue is T other)
      return value.CompareTo(other) == 1;
    return false;
  }

  private bool NotGreaterFunction(object? propValue, object? otherValue)
  {
    if (propValue is T value && otherValue is T other)
      return value.CompareTo(other) != 1;
    return false;
  }

  private bool IsLessFunction(object? propValue, object? otherValue)
  {
    if (propValue is T value && otherValue is T other)
      return value.CompareTo(other) == -1;
    return false;
  }

  private bool NotLessFunction(object? propValue, object? otherValue)
  {
    if (propValue is T value && otherValue is T other)
      return value.CompareTo(other) != -1;
    return false;
  }
}
