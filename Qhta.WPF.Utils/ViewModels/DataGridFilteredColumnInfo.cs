namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Information about filterable column in data grid.
/// </summary>
public record DataGridFilteredColumnInfo
{

  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="itemType"></param>
  /// <param name="column"></param>
  public DataGridFilteredColumnInfo(Type itemType, DataGridBoundColumn column)
  {
    Column = column;
    var columnName = column.GetHeaderText();
    if (columnName == null)
      columnName = CollectionViewBehavior.GetHiddenHeader(column);
    if (columnName == null)
      columnName = "?";
    ColumnName = columnName;
    if (column.TryGetFilterablePropertyPath(itemType, out var propPath))
    {
      PropName = propPath.Last().Name;
      DataType = propPath.Last().PropertyType;
      PropPath = propPath;
    }
  }

  /// <summary>
  /// Referenced column.
  /// </summary>
  public DataGridBoundColumn Column { get; private set; }

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
