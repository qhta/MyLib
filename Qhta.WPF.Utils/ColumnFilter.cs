namespace Qhta.WPF.Utils;

/// <summary>
/// Prepared filter for a column. Holds property info, other value to compare, compare function and predicate function.
/// </summary>
public class ColumnFilter
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="propertyPath"></param>
  /// <param name="compareFunction"></param>
  public ColumnFilter(PropertyInfo[] propertyPath, Func<object?, object?, bool> compareFunction)
  {
    PropertyPath = propertyPath;
    CompareFunction = compareFunction;
    Predicate = new Predicate<object>(obj =>
      {
        var value = obj;
        foreach (var propertyInfo in PropertyPath)
         value = propertyInfo.GetValue(value, null);
        return CompareFunction(value, OtherValue);
      });

  }

  /// <summary>
  /// Stored info on column binding properties.
  /// Predicate function gets values sequentially.
  /// First value is got from the object argument,
  /// next from the previous value.
  /// </summary>
  public PropertyInfo[] PropertyPath { get; private set; }

  /// <summary>
  /// Storage for other value to compare.
  /// </summary>
  public object? OtherValue { get; set; }

  /// <summary>
  /// Predicate to evaluate for the column value.
  /// </summary>
  public Func<object?, object?, bool> CompareFunction { get; private set; }

  /// <summary>
  /// Predicate to evaluate for the column value.
  /// </summary>
  public Predicate<object> Predicate { get; private set; } 

}
