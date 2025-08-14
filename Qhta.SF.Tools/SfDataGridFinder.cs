using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Qhta.SF.Tools.Resources;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace Qhta.SF.Tools;

/// <summary>
/// Class containing tools for finding values and text in Syncfusion DataGrid columns.
/// </summary>
public static partial class SfDataGridFinder
{

  /// <summary>
  /// Finds the first occurrence of a specified value in the column and selects the corresponding cell.
  /// </summary>
  /// <param name="column">Grid column to search</param>
  /// <param name="specifiedValue">Searched value. It can be an object or text.</param>
  /// <param name="predicate">Filter predicate. It contains conditions to evaluate the current value against the specific value.</param>
  /// <returns>True if first cells is found.</returns>
  public static bool FindFirst(this GridColumn column, object? specifiedValue, FilterPredicate predicate)
  {
    if (column.GetDataGrid() is not { } dataGrid)
    {
      //Debug.WriteLine($"Column DataGrid not found.");
      return false;
    }
    var mappingName = column.MappingName;
    var property = SfDataGridCommander.GetRowDataType(dataGrid)?.GetProperty(mappingName);
    if (property == null)
    {
      Debug.WriteLine($"Property '{mappingName}' not found in row data type.");
      return false;
    }

    var columnIndex = dataGrid.Columns.IndexOf(column);
    foreach (var row in dataGrid.View.Records.Select(record => record.Data))
    {
      var currentValue = property.GetValue(row);
      if (EvaluatePredicate(currentValue, specifiedValue, predicate))
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
  /// Finds the next occurrence of a specified value in the column and selects the corresponding cell.
  /// </summary>
  /// <param name="column">Grid column to search</param>
  /// <param name="specifiedValue">Searched value. It can be an object or text.</param>
  /// <param name="predicate">Filter predicate. It contains conditions to evaluate the current value against the specific value.</param>
  /// <param name="currentRow">If null then top of selected rows is assumed as current</param>
  /// <returns>True if next cells is found.</returns>
  public static bool FindNext(this GridColumn column, object? specifiedValue, FilterPredicate predicate, object? currentRow = null)
  {
    if (column.GetDataGrid() is not { } dataGrid)
    {
      //Debug.WriteLine($"Column DataGrid not found.");
      return false;
    }
    var mappingName = column.MappingName;
    var property = SfDataGridCommander.GetRowDataType(dataGrid)?.GetProperty(mappingName);
    if (property == null)
    {
      Debug.WriteLine($"Property '{mappingName}' not found in row data type.");
      return false;
    }

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
      var currentValue = property.GetValue(row);
      if (EvaluatePredicate(currentValue, specifiedValue, predicate))
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
  /// <param name="column">Grid column to search</param>
  /// <param name="specifiedValue">Searched value. It can be an object or text.</param>
  /// <param name="predicate">Filter predicate. It contains conditions to evaluate the current value against the specific value.</param>
  /// <returns>Number of selected cells</returns>
  public static int FindAll(this GridColumn column, object? specifiedValue, FilterPredicate predicate)
  {
    int found = 0;
    // In the current implementation, we know that the column DataGrid property is not public.
    if (column.GetDataGrid() is not { } dataGrid)
    {
      //Debug.WriteLine($"Column DataGrid not found.");
      return 0;
    }
    var mappingName = column.MappingName;
    var property = SfDataGridCommander.GetRowDataType(dataGrid)?.GetProperty(mappingName);
    if (property == null)
    {
      Debug.WriteLine($"Property '{mappingName}' not found in row data type.");
      return 0;
    }

    var columnIndex = dataGrid.Columns.IndexOf(column);
    var selectedRow = dataGrid.GetSelectedCells().FirstOrDefault()?.RowData;
    var startSearching = selectedRow == null;
    var firstRowIndex = -1;

    foreach (var row in dataGrid.View.Records.Select(record => record.Data))
    {
      var currentValue = property.GetValue(row);
      if (EvaluatePredicate(currentValue, specifiedValue, predicate))
      {
        //Debug.WriteLine($"Found value: {value} in row: {row}");
        var rowIndex = dataGrid.ResolveToRowIndex(row);
        if (found == 0)
        {
          firstRowIndex = rowIndex;
          dataGrid.SelectAllColumns(false);
        }
        found++;
        dataGrid.SelectCells(row, column, row, column);
      }
    }
    if (firstRowIndex >= 0)
      dataGrid.ScrollInView(new RowColumnIndex(firstRowIndex, columnIndex));
    return found;

  }


  /// <summary>
  /// Evaluates the specified predicate against the current value and specified value.
  /// </summary>
  /// <param name="currentValue">Value found in the cell</param>
  /// <param name="specifiedValue">Specified value to compare to</param>
  /// <param name="predicate">Conditions of comparison.
  /// If predicate FilterBehavior is StringTyped, then all values of FilterType are evaluated.
  /// For StronglyTyped behavior only Equals and NotEquals functions are evaluated.</param>
  /// <returns>True if the current value compared against specified value satisfies the conditions of the predicate.</returns>
  private static bool EvaluatePredicate(object? currentValue, object? specifiedValue, FilterPredicate predicate)
  {
    if (predicate.FilterBehavior == FilterBehavior.StringTyped)
    {
      // If the filter behavior is StringTyped, we treat both values as strings for comparison
      var currentText = (currentValue is ISelectableItem selectableItem) ? selectableItem.DisplayName : currentValue?.ToString() ?? "";
      var specifiedText = (specifiedValue is ISelectableItem selectableItem2) ? selectableItem2.DisplayName : specifiedValue?.ToString() ?? "";
      if (!predicate.IsCaseSensitive)
      {
        // Case-insensitive comparison
        specifiedText = specifiedText.ToLower();
        currentText = currentText.ToLower();
      }
      double currentNumber;
      double specifiedNumber;
      //Debug.WriteLine($"Current text = {currentText}");
      switch (predicate.FilterType)
      {
        case FilterType.Equals:
         if (currentText.Equals(specifiedText)) return true;
         break;
        case FilterType.NotEquals:
          if (!currentText.Equals(specifiedText)) return true;
          break;
        case FilterType.StartsWith:
          if (currentText.StartsWith(specifiedText)) return true;
          break;
        case FilterType.NotStartsWith:
          if (!currentText.StartsWith(specifiedText)) return true;
          break;
        case FilterType.EndsWith:
          if (currentText.EndsWith(specifiedText)) return true;
          break;
        case FilterType.NotEndsWith:
          if (!currentText.EndsWith(specifiedText)) return true;
          break;
        case FilterType.Contains:
          if (currentText.Contains(specifiedText)) return true;
          break;
        case FilterType.NotContains:
          if (!currentText.Contains(specifiedText)) return true;
          break;
        case FilterType.GreaterThan:
          if (double.TryParse(currentText, out currentNumber) && double.TryParse(specifiedText, out specifiedNumber))
            if (currentNumber > specifiedNumber) return true;
          break;
        case FilterType.GreaterThanOrEqual:
          if (double.TryParse(currentText, out currentNumber) && double.TryParse(specifiedText, out specifiedNumber))
            if (currentNumber >= specifiedNumber) return true;
          break;
        case FilterType.LessThan:
          if (double.TryParse(currentText, out currentNumber) && double.TryParse(specifiedText, out specifiedNumber))
            if (currentNumber < specifiedNumber) return true;
          break;
        case FilterType.LessThanOrEqual:
          if (double.TryParse(currentText, out currentNumber) && double.TryParse(specifiedText, out specifiedNumber))
            if (currentNumber <= specifiedNumber) return true;
          break;
      }
    }
    else if (predicate.FilterBehavior == FilterBehavior.StronglyTyped)
    {
      if (predicate.FilterType == FilterType.Equals)
      {
        if (specifiedValue == null && currentValue == null) return true;
        if (currentValue != null && currentValue.Equals(specifiedValue)) return true;
      }
      else if (predicate.FilterType == FilterType.NotEquals)
      {
        if (specifiedValue == null && currentValue != null) return true;
        if (currentValue != null && !currentValue.Equals(specifiedValue)) return true;
      }
    }
    return false;
  }
}