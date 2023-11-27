namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Information about filterable column in data grid.
/// </summary>
public record FilterableColumnInfo
{

  /// <summary>
  /// Initializing constructor
  /// </summary>
  public FilterableColumnInfo(PropPath propPath, string columnName, Type? dataType, object column)
  {
    //Debug.WriteLine($"propPath={propPath} columnName={columnName} dataType={dataType} column={column}");
    Column = column;
    ColumnName = columnName;
    PropName = propPath.Last().Name;
    DataType = dataType ?? propPath.Last().PropertyType;
    PropPath = propPath;
  }

  /// <summary>
  /// This instance needed by selector.
  /// </summary>
  public Object? Column { get; private set; }

  /// <summary>
  /// Name of the column to display.
  /// </summary>
  public string ColumnName { get; private set; }

  /// <summary>
  /// Name of the property.
  /// </summary>
  public string? PropName { get; private set; }

  /// <summary>
  /// Data type of the property.
  /// </summary>
  public Type? DataType { get; private set; }

  /// <summary>
  /// Sequence of PropertyInfo from DataContext object
  /// to the filtered property.
  /// </summary>
  public PropPath? PropPath { get; private set; }
}
