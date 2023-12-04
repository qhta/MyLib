namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Generic NumFilterViewModel for numeric values filter with a type parameter. Valid types are all comparable values, 
/// however internal TryParseFilterText method recognizes only integers, floats, DateTime and TimeSpan.
/// </summary>
public class NumFilterViewModel<T> : NumFilterViewModel where T : IComparable<T>
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public NumFilterViewModel(PropPath propPath, string columnName, IObjectOwner? owner) : base(propPath, columnName, owner)
  {
    this.PropPath = propPath;
    this.ColumnName = columnName;
    this.Function = NumPredicateFunction.IsEqual;
  }

  /// <summary>
  /// Copying constructor.
  /// </summary>
  /// <returns></returns>
  public NumFilterViewModel(NumFilterViewModel<T> other) : base(other)
  {
    this.PropPath = other.PropPath;
    this.ColumnName = other.ColumnName;
    this.Function = other.Function;
  }

  /// <summary>
  /// Creates a copy of this instance;
  /// </summary>
  /// <returns></returns>
  public override FilterViewModel CreateCopy()
  {
    return new NumFilterViewModel<T>(this);
  }

  /// <summary>
  /// Clears the <see cref="FilterText"/> and <see cref="Function"/> property.
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  public override void ClearFilter()
  {
    FilterText = null;
    Function = null;
  }

  /// <summary>
  /// Simple filter text.
  /// </summary>
  public string? FilterText
  {
    get { return _FilterText; }
    set
    {
      if (_FilterText != value)
      {
        _FilterText = value;
        NotifyPropertyChanged(nameof(FilterText));
      }
    }
  }
  private string? _FilterText;


  /// <summary>
  /// Selected predicate function.
  /// </summary>
  public NumPredicateFunction? Function
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
  private NumPredicateFunction? _Function;

  private void NotifyFunctionChanged()
  {
    NotifyPropertyChanged(nameof(Function));
    foreach (var enumName in typeof(NumPredicateFunction).GetEnumNames())
      NotifyPropertyChanged(enumName);
    NotifyPropertyChanged(nameof(CanCreateFilter));
  }

  #region Individual boolean properties for Function used in RadioButton.

  /// <summary>
  /// Specifies whether predicate function is IsEqual.
  /// </summary>
  public bool IsEqual { get => Function == NumPredicateFunction.IsEqual; set { if (value) Function = NumPredicateFunction.IsEqual; } }

  /// <summary>
  /// Specifies whether predicate function is NotEqual.
  /// </summary>
  public bool NotEqual { get => Function == NumPredicateFunction.NotEqual; set { if (value) Function = NumPredicateFunction.NotEqual; } }

  /// <summary>
  /// Specifies whether predicate function is IsEmpty.
  /// </summary>
  public bool IsEmpty { get => Function == NumPredicateFunction.IsEmpty; set { if (value) Function = NumPredicateFunction.IsEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is NotEmpty.
  /// </summary>
  public bool NotEmpty { get => Function == NumPredicateFunction.NotEmpty; set { if (value) Function = NumPredicateFunction.NotEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is IsGreater.
  /// </summary>
  public bool IsGreater { get => Function == NumPredicateFunction.IsGreater; set { if (value) Function = NumPredicateFunction.IsGreater; } }

  /// <summary>
  /// Specifies whether predicate function is NotGreater.
  /// </summary>
  public bool NotGreater { get => Function == NumPredicateFunction.NotGreater; set { if (value) Function = NumPredicateFunction.NotGreater; } }

  /// <summary>
  /// Specifies whether predicate function is IsLess.
  /// </summary>
  public bool IsLess { get => Function == NumPredicateFunction.IsLess; set { if (value) Function = NumPredicateFunction.IsLess; } }

  /// <summary>
  /// Specifies whether predicate function is NotLess.
  /// </summary>
  public bool NotLess { get => Function == NumPredicateFunction.NotLess; set { if (value) Function = NumPredicateFunction.NotLess; } }

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
    bool shouldParseFilterText = true;
    switch (Function)
    {
      case NumPredicateFunction.IsEmpty:
        compareFunction = IsEmptyFunction;
        shouldParseFilterText = false;
        break;
      case NumPredicateFunction.NotEmpty:
        compareFunction = NotEmptyFunction;
        shouldParseFilterText = false;
        break;
      case NumPredicateFunction.IsEqual:
        compareFunction = IsEqualFunction;
        break;
      case NumPredicateFunction.NotEqual:
        compareFunction = NotEqualFunction;
        break;
      case NumPredicateFunction.IsGreater:
        compareFunction = IsGreaterFunction;
        break;
      case NumPredicateFunction.NotGreater:
        compareFunction = NotGreaterFunction;
        break;
      case NumPredicateFunction.IsLess:
        compareFunction = IsLessFunction;
        break;
      case NumPredicateFunction.NotLess:
        compareFunction = NotLessFunction;
        break;

      default:
        return null;
    }
    object? otherValue = null;
    if (shouldParseFilterText)
    {
      if (String.IsNullOrEmpty(FilterText))
      {
        MessageBox.Show(CommonStrings.UndeclaredCompareValue);
        return null;
      }
      else
      if (!TryParseFilterText(FilterText, out otherValue))
      {
        MessageBox.Show(String.Format(CommonStrings.CantParseCompareValue_0, typeof(T).Name));
        return null;
      }
    }

    var dataGridColumnFilter = new ColumnFilter(PropPath, compareFunction);
    dataGridColumnFilter.OtherValue = otherValue;
    return dataGridColumnFilter;
  }

  private bool TryParseFilterText(string filterText, out object? value)
  {
    if (typeof(T) == typeof(int))
    {
      if (int.TryParse(filterText, out var valInt))
      {
        value = valInt;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(uint))
    {
      if (uint.TryParse(filterText, out var valUInt))
      {
        value = valUInt;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(byte))
    {
      if (byte.TryParse(filterText, out var valByte))
      {
        value = valByte;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(sbyte))
    {
      if (sbyte.TryParse(filterText, out var valSByte))
      {
        value = valSByte;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(Int16))
    {
      if (Int16.TryParse(filterText, out var valInt16))
      {
        value = valInt16;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(UInt16))
    {
      if (UInt16.TryParse(filterText, out var valUInt16))
      {
        value = valUInt16;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(Int64))
    {
      if (Int64.TryParse(filterText, out var valInt64))
      {
        value = valInt64;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(UInt64))
    {
      if (UInt64.TryParse(filterText, out var valUInt64))
      {
        value = valUInt64;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(DateTime))
    {
      if (DateTime.TryParse(filterText, out var valDateTime))
      {
        value = valDateTime;
        return true;
      }
    }
    else
    if (typeof(T) == typeof(TimeSpan))
    {
      if (TimeSpan.TryParse(filterText, out var valTimeSpan))
      {
        value = valTimeSpan;
        return true;
      }
    }

    MessageBox.Show(String.Format(CommonStrings.NumFilter_parse_error, typeof(T).Name));
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
