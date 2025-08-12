using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Qhta.SF.Tools.Resources;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace Qhta.SF.Tools;

/// <summary>
/// Class containing tools for finding values and text in Syncfusion DataGrid columns.
/// </summary>
public static partial class SfDataGridFinder
{
  /// <summary>
  /// Static object representing an empty value.
  /// </summary>
  public static EmptyValue EmptyValue { get; } = new EmptyValue();
  /// <summary>
  /// Static object representing a non-empty value.
  /// </summary>
  public static NonEmptyValue NonEmptyValue { get; } = new NonEmptyValue();

  /// <summary>
  /// Finds the first occurrence of a specified value in the column and selects the corresponding cell.
  /// </summary>
  /// <param name="column"></param>
  /// <param name="specifiedValue"></param>
  /// <returns></returns>
  public static bool FindFirstValue(this GridColumn column, object? specifiedValue)
  {
    if (column.GetDataGrid() is not { } dataGrid)
    {
      Debug.WriteLine($"Column DataGrid not found.");
      return false;
    }
    var mappingName = column.MappingName;
    var property = SfDataGridCommander.GetRowDataType(dataGrid)?.GetProperty(mappingName);
    if (property == null)
    {
      Debug.WriteLine($"Property '{mappingName}' not found in row data type.");
      return false;
    }
    specifiedValue = PrepareSpecifiedValue(specifiedValue);

    var columnIndex = dataGrid.Columns.IndexOf(column);
    foreach (var row in dataGrid.View.Records.Select(record => record.Data))
    {
      var value = property.GetValue(row);
      if (specifiedValue == null && value == null || specifiedValue == NonEmptyValue && value != null || value != null && value.Equals(specifiedValue))
      {
        //Debug.WriteLine($"Found value: {value} in row: {row}");
        var rowIndex = dataGrid.ResolveToRowIndex(row);
        dataGrid.SelectAllColumns(false);
        dataGrid.SelectCells(row, column, row, column);
        dataGrid.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Checks if the specified value is a selectable item or a special marker and prepares it for comparison.
  /// if the value is an empty string, or it is an EmptyValue, it is treated as null.
  /// </summary>
  /// <param name="specifiedValue"></param>
  /// <returns></returns>
  private static object? PrepareSpecifiedValue(object? specifiedValue)
  {
    if (specifiedValue is ISelectableItem selectableItem)
    {
      if (selectableItem.DisplayName == DataStrings.EmptyValue)
        specifiedValue = null; // Treat "EmptyValue" as null for comparison
      else if (selectableItem.DisplayName == DataStrings.NonEmptyValue)
        specifiedValue = NonEmptyValue; // Treat "NonEmptyValue" as a special marker for non-empty values
    }
    else
    if (specifiedValue is EmptyValue)
      specifiedValue = null; // Treat EmptyValue as null for comparison
    else
    if (specifiedValue is NonEmptyValue)
      specifiedValue = NonEmptyValue; // Treat NonEmptyValue as a special marker for non-empty values
    else if (specifiedValue is string str && string.IsNullOrWhiteSpace(str))
      specifiedValue = null; // Treat empty strings as null
    return specifiedValue;
  }

  /// <summary>
  /// Finds the next occurrence of a specified value in the column and selects the corresponding cell.
  /// </summary>
  /// <param name="column"></param>
  /// <param name="specifiedValue"></param>
  /// <param name="currentRow">If null then top of selected rows is assumed as current</param>
  /// <returns></returns>
  public static bool FindNextValue(this GridColumn column, object? specifiedValue, object? currentRow = null)
  {
    if (column.GetDataGrid() is not { } dataGrid)
    {
      Debug.WriteLine($"Column DataGrid not found.");
      return false;
    }
    var mappingName = column.MappingName;
    var property = SfDataGridCommander.GetRowDataType(dataGrid)?.GetProperty(mappingName);
    if (property == null)
    {
      Debug.WriteLine($"Property '{mappingName}' not found in row data type.");
      return false;
    }
    specifiedValue = PrepareSpecifiedValue(specifiedValue);

    var columnIndex = dataGrid.Columns.IndexOf(column);
    var selectedRow = dataGrid.CurrentItem;
    var startSearching = selectedRow == null;
    foreach (var row in dataGrid.View.Records.Select(record => record.Data))
    {
      // Skip rows until we reach the row after the currently selected one
      if (!startSearching)
      {
        if (row == selectedRow) startSearching = true;
        continue;
      }
      var value = property.GetValue(row);
      if (specifiedValue == null && value == null || specifiedValue == NonEmptyValue && value != null || value != null && value.Equals(specifiedValue))
      {
        //Debug.WriteLine($"Found value: {value} in row: {row}");
        var rowIndex = dataGrid.ResolveToRowIndex(row);
        dataGrid.SelectAllColumns(false);
        dataGrid.SelectCells(row, column, row, column);
        dataGrid.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Finds all occurrences of a specified value in the column and selects the corresponding cells.
  /// </summary>
  /// <param name="column"></param>
  /// <param name="specifiedValue"></param>
  /// <returns></returns>
  public static bool FindAllValues(this GridColumn column, object? specifiedValue)
  {
    bool found = false;
    // In the current implementation, we know that the column DataGrid property is not public.
    if (column.GetDataGrid() is not { } dataGrid)
    {
      Debug.WriteLine($"Column DataGrid not found.");
      return false;
    }
    var mappingName = column.MappingName;
    var property = SfDataGridCommander.GetRowDataType(dataGrid)?.GetProperty(mappingName);
    if (property == null)
    {
      Debug.WriteLine($"Property '{mappingName}' not found in row data type.");
      return false;
    }
    if (specifiedValue is EmptyValue)
      specifiedValue = null; // Treat EmptyValue as null for comparison
    else
    if (specifiedValue is NonEmptyValue)
      specifiedValue = NonEmptyValue; // Treat NonEmptyValue as a special marker for non-empty values
    else if (specifiedValue is string str && string.IsNullOrWhiteSpace(str))
      specifiedValue = null; // Treat empty strings as null
    var columnIndex = dataGrid.Columns.IndexOf(column);
    var selectedRow = dataGrid.GetSelectedCells().FirstOrDefault()?.RowData;
    var startSearching = selectedRow == null;
    var firstRowIndex = -1;

    foreach (var row in dataGrid.View.Records.Select(record => record.Data))
    {
      var value = property.GetValue(row);
      if (specifiedValue == null && value == null || specifiedValue == NonEmptyValue && value != null || value != null && value.Equals(specifiedValue))
      {
        //Debug.WriteLine($"Found value: {value} in row: {row}");
        var rowIndex = dataGrid.ResolveToRowIndex(row);
        if (!found)
        {
          firstRowIndex = rowIndex;
          dataGrid.SelectAllColumns(false);
          found = true;
        }
        dataGrid.SelectCells(row, column, row, column);
      }
    }
    if (firstRowIndex >= 0)
      dataGrid.ScrollInView(new RowColumnIndex(firstRowIndex, columnIndex));
    return found;

  }
}