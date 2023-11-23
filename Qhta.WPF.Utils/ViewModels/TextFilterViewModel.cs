namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Specific ColumnFilterViewModel of string property filter edited in TextFilterView.
/// </summary>
public class TextFilterViewModel : ColumnFilterViewModel
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public TextFilterViewModel(PropPath propPath, string columnName) : base(propPath, columnName) { }

  /// <summary>
  /// Copying constructor.
  /// </summary>
  /// <returns></returns>
  public TextFilterViewModel(TextFilterViewModel other) : base(other)
  {
    this.FilterText = other.FilterText;
    this.Function = other.Function;
    this.IgnoreCase = other.IgnoreCase;
  }

  /// <summary>
  /// Creates a copy of this instance;
  /// </summary>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public override ColumnFilterViewModel CreateCopy()
  {
    return new TextFilterViewModel(this);
  }

  /// <summary>
  /// Clears <see cref="FilterText"/> and <see cref="Function"/> properties.
  /// </summary>
  public override void ClearFilter()
  {
    FilterText = null;
    Function = 0;
    IgnoreCase = false;
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
  public TextPredicateFunction Function
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
  private TextPredicateFunction _Function;

  private void NotifyFunctionChanged()
  {
    NotifyPropertyChanged(nameof(Function));
    foreach (var enumName in typeof(TextPredicateFunction).GetEnumNames())
      NotifyPropertyChanged(enumName);
  }

  /// <summary>
  /// Specifies whether letter case should be ignored.
  /// </summary>
  public bool IgnoreCase
  {
    get { return _IgnoreCase; }
    set
    {
      if (_IgnoreCase != value)
      {
        _IgnoreCase = value;
        NotifyPropertyChanged(nameof(IgnoreCase));
      }
    }
  }
  private bool _IgnoreCase;

  #region Individual boolean properties for Function used in RadioButton.

  /// <summary>
  /// Specifies whether predicate function is IsEqual.
  /// </summary>
  public bool IsEqual { get => Function == TextPredicateFunction.IsEqual; set { if (value) Function = TextPredicateFunction.IsEqual; } }

  /// <summary>
  /// Specifies whether predicate function is NotEqual.
  /// </summary>
  public bool NotEqual { get => Function == TextPredicateFunction.NotEqual; set { if (value) Function = TextPredicateFunction.NotEqual; } }

  /// <summary>
  /// Specifies whether predicate function is IsEmpty.
  /// </summary>
  public bool IsEmpty { get => Function == TextPredicateFunction.IsEmpty; set { if (value) Function = TextPredicateFunction.IsEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is NotEmpty.
  /// </summary>
  public bool NotEmpty { get => Function == TextPredicateFunction.NotEmpty; set { if (value) Function = TextPredicateFunction.NotEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is StartsWith.
  /// </summary>
  public bool StartsWith { get => Function == TextPredicateFunction.StartsWith; set { if (value) Function = TextPredicateFunction.StartsWith; } }

  /// <summary>
  /// Specifies whether predicate function is EndsWith.
  /// </summary>
  public bool EndsWith { get => Function == TextPredicateFunction.EndsWith; set { if (value) Function = TextPredicateFunction.EndsWith; } }

  /// <summary>
  /// Specifies whether predicate function is Contains.
  /// </summary>
  public bool Contains { get => Function == TextPredicateFunction.Contains; set { if (value) Function = TextPredicateFunction.Contains; } }

  /// <summary>
  /// Specifies whether predicate function is RegExpr.
  /// </summary>
  public bool RegExpr { get => Function == TextPredicateFunction.RegExpr; set { if (value) Function = TextPredicateFunction.RegExpr; } }

  #endregion

  /// <summary>
  /// Creates DataGridColumnFilter basing on current properties.
  /// </summary>
  /// <returns></returns>
  public override ColumnFilter? CreateFilter()
  {
    if (String.IsNullOrEmpty(FilterText) && !IsEmpty && !NotEmpty || PropPath == null)
      return null;
    Func<object?, object?, bool> compareFunction;
    switch (Function)
    {
      case TextPredicateFunction.IsEmpty:
        compareFunction = IsEmptyFunction;
        break;
      case TextPredicateFunction.NotEmpty:
        compareFunction = NotEmptyFunction;
        break;
      case TextPredicateFunction.IsEqual:
        compareFunction = (IgnoreCase) ? IsEqualIgnoreCaseFunction : IsEqualFunction;
        break;
      case TextPredicateFunction.NotEqual:
        compareFunction = (IgnoreCase) ? NotEqualIgnoreCaseFunction : NotEqualFunction;
        break;
      case TextPredicateFunction.Contains:
        compareFunction = (IgnoreCase) ? ContainsIgnoreCaseFunction : ContainsFunction;
        break;
      case TextPredicateFunction.StartsWith:
        compareFunction = (IgnoreCase) ? StartsWithIgnoreCaseFunction : StartsWithFunction;
        break;
      case TextPredicateFunction.EndsWith:
        compareFunction = (IgnoreCase) ? EndsWithIgnoreCaseFunction : EndsWithFunction;
        break;
      case TextPredicateFunction.RegExpr:
        compareFunction = (IgnoreCase) ? RegExprIgnoreCaseFunction : RegExprFunction;
        break;
      default:
        return null;
    }
    var dataGridColumnFilter = new ColumnFilter(PropPath, compareFunction);
    dataGridColumnFilter.OtherValue = FilterText;
    return dataGridColumnFilter;
  }

  private bool IsEmptyFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString)
      return propString.Equals(String.Empty);
    return propValue == null;
  }

  private bool NotEmptyFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString)
      return !propString.Equals(String.Empty);
    return propValue != null;
  }

  private bool IsEqualFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.Equals(otherString);
    return false;
  }

  private bool IsEqualIgnoreCaseFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.Equals(otherString, StringComparison.CurrentCultureIgnoreCase);
    return false;
  }

  private bool NotEqualFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return !propString.Equals(otherString);
    return false;
  }

  private bool NotEqualIgnoreCaseFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return !propString.Equals(otherString, StringComparison.CurrentCultureIgnoreCase);
    return false;
  }

  private bool ContainsFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.Contains(otherString);
    return false;
  }

  private bool ContainsIgnoreCaseFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.Contains(otherString, StringComparison.CurrentCultureIgnoreCase);
    return false;
  }

  private bool StartsWithFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.StartsWith(otherString);
    return false;
  }

  private bool StartsWithIgnoreCaseFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.StartsWith(otherString, StringComparison.CurrentCultureIgnoreCase);
    return false;
  }

  private bool EndsWithFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.EndsWith(otherString);
    return false;
  }

  private bool EndsWithIgnoreCaseFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return propString.EndsWith(otherString, StringComparison.CurrentCultureIgnoreCase);
    return false;
  }

  private bool RegExprFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return Regex.IsMatch(propString, otherString);
    return false;
  }

  private bool RegExprIgnoreCaseFunction(object? propValue, object? otherValue)
  {
    if (propValue is string propString && otherValue is string otherString)
      return Regex.IsMatch(propString, otherString, RegexOptions.IgnoreCase);
    return false;
  }

}
