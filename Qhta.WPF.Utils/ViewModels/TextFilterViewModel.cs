using System.Text.RegularExpressions;

namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// View model shown in TextFilterWindow.
/// </summary>
public class TextFilterViewModel : ColumnFilterViewModel
{
  /// <summary>
  /// Specifies if other filters should be cleared.
  /// </summary>
  public bool ClearAllFilters { get; set; }

  /// <summary>
  /// Simple filter text.
  /// </summary>
  public string? FilterText { get; set; }

  /// <summary>
  /// Selected predicate function.
  /// </summary>
  public TextPredicateFunction Function { get; set; }

  /// <summary>
  /// Specifies whether letter case should be ignored.
  /// </summary>
  public bool IgnoreCase { get; set; }

  #region Individual boolean properties used in view.

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
  public override DataGridColumnFilter? CreateFilter(PropertyInfo propertyInfo)
  {
    if (String.IsNullOrEmpty(FilterText) && !IsEmpty && !NotEmpty)
      return null;
    var dataGridColumnFilter = new DataGridColumnFilter(propertyInfo);
    switch (Function)
    {
      case TextPredicateFunction.IsEmpty:
        dataGridColumnFilter.CompareFunction = IsEmptyFunction;
        break;
      case TextPredicateFunction.NotEmpty:
        dataGridColumnFilter.CompareFunction = NotEmptyFunction;
        break;
      case TextPredicateFunction.IsEqual:
        dataGridColumnFilter.CompareFunction = (IgnoreCase) ? IsEqualIgnoreCaseFunction : IsEqualFunction;
        break;
      case TextPredicateFunction.NotEqual:
        dataGridColumnFilter.CompareFunction = (IgnoreCase) ? NotEqualIgnoreCaseFunction : NotEqualFunction;
        break;
      case TextPredicateFunction.Contains:
        dataGridColumnFilter.CompareFunction = (IgnoreCase) ? ContainsIgnoreCaseFunction : ContainsFunction;
        break;
      case TextPredicateFunction.StartsWith:
        dataGridColumnFilter.CompareFunction = (IgnoreCase) ? StartsWithIgnoreCaseFunction : StartsWithFunction;
        break;
      case TextPredicateFunction.EndsWith:
        dataGridColumnFilter.CompareFunction = (IgnoreCase) ? EndsWithIgnoreCaseFunction : EndsWithFunction;
        break;
      case TextPredicateFunction.RegExpr:
        dataGridColumnFilter.CompareFunction = (IgnoreCase) ? RegExprIgnoreCaseFunction : RegExprFunction;
        break;
      default:
        return null;
    }
    dataGridColumnFilter.OtherValue = FilterText;
    dataGridColumnFilter.Predicate =
      new Predicate<object>(obj =>
      {
        var value = dataGridColumnFilter.PropertyInfo.GetValue(obj, null);
        return dataGridColumnFilter.CompareFunction(value, dataGridColumnFilter.OtherValue);
      });
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
