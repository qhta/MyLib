namespace Qhta.WPF.Utils;

/// <summary>
/// Preprocessed filter for a column.
/// </summary>
public class DataGridColumnFilter
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="propertyInfo"></param>
  public DataGridColumnFilter(PropertyInfo propertyInfo)
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
  public Predicate<object>? Predicate { get; set; }
}
