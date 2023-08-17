namespace Qhta.WPF.Utils;

/// <summary>
/// Prepared filter for a column. Holds property info, other value to compare, compare function and predicate function.
/// </summary>
public class ColumnFilter
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="propertyInfo"></param>
  public ColumnFilter(PropertyInfo propertyInfo)
  {
    PropertyInfo = propertyInfo;
  }

  /// <summary>
  /// Storage for property info.
  /// </summary>
  public PropertyInfo PropertyInfo { get; private set; }

  /// <summary>
  /// Storage for other value to compare.
  /// </summary>
  public object? OtherValue { get; set; }

  /// <summary>
  /// Predicate to evaluate for the column value.
  /// </summary>
  public Func<object?, object?, bool> CompareFunction { get; set; } = null!;

  /// <summary>
  /// Predicate to evaluate for the column value.
  /// </summary>
  public Predicate<object> Predicate { get; set; } = null!;
}
