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
public class SfDataGridFinder
{

  /// <summary>
  /// DataGrid instance to operate on.
  /// </summary>
  public required SfDataGrid DataGrid { get; set; }

  /// <summary>
  /// Gets or sets the column to search.
  /// </summary>
  public required GridColumn Column
  {
    get => _Column;
    set
    {
      var mappingName = value.MappingName;
      Property = SfDataGridCommander.GetRowDataType(DataGrid)?.GetProperty(mappingName)!;
      if (Property == null)
        throw new InvalidOperationException($"Property '{mappingName}' not found in grid source data type.");
      if (!Property.CanRead)
        throw new InvalidOperationException($"Property '{mappingName}' can't be read.");
      _Column = value;
    }
  }
  private GridColumn _Column = null!;

  internal PropertyInfo Property { get; private set; } = null!;

  /// <summary>
  /// Specifies whether the specified value is strongly typed or string typed.
  /// </summary>
  public bool StrongTyped { get; set; }

  /// <summary>
  /// Specified value to search for.
  /// It can be a string or any other type, depending on the FilterBehavior of the predicate.
  /// </summary>
  public object? SpecifiedValue
  {
    get => specifiedValue;
    set
    {
      if (!Property.CanRead)
        throw new InvalidOperationException($"Property '{Property.Name}' can't be read.");
      specifiedValue = value;
    }
  }
  internal object? specifiedValue { get; set; }

  /// <summary>
  /// Determines whether to replace the found value with the specified value.
  /// </summary>
  public bool Replace { get; set; }

  /// <summary>
  /// Specified value to search for.
  /// It can be a string or any other type, depending on the FilterBehavior of the predicate.
  /// </summary>
  public object? Replacement
  {
    get => _Replacement;
    set
    {
      if (DataGrid.IsReadOnly)
        throw new InvalidOperationException($"DataGrid is read-only.");
      if (!Property.CanWrite)
        throw new InvalidOperationException($"Property '{Property.Name}' is read-only.");
      _Replacement = value;
    }
  }
  internal object? _Replacement { get; set; }

  /// <summary>
  /// Predicate defining the conditions of comparison.
  /// </summary>
  public FilterPredicate Predicate
  {
    get => predicate;
    set => predicate = value;
  }
  internal FilterPredicate predicate { get; set; } = new()
  {
    FilterBehavior = FilterBehavior.StringTyped,
    FilterType = FilterType.Contains,
    IsCaseSensitive = false
  };

  /// <summary>
  /// Finds the first occurrence of a specified text in the column and selects the corresponding cell.
  /// </summary>
  /// <returns>True if first cells is found.</returns>
  public bool FindFirst()
  {
    var columnIndex = DataGrid.Columns.IndexOf(Column);
    foreach (var row in DataGrid.View.Records.Select(record => record.Data))
    {
      var currentValue = Property.GetValue(row);
      if (EvaluatePredicate(currentValue, specifiedValue, predicate))
      {
        //Debug.WriteLine($"Found value: {value} in row: {row}");
        var rowIndex = DataGrid.ResolveToRowIndex(row);
        DataGrid.SelectAllColumns(false);
        DataGrid.SelectCells(row, Column, row, Column);
        DataGrid.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Finds the next occurrence of a specified value in the column and selects the corresponding cell.
  /// </summary>
  /// <returns>True if next cells is found.</returns>
  public bool FindNext()
  {
    var columnIndex = DataGrid.Columns.IndexOf(Column);
    var selectedRow = DataGrid.CurrentItem;
    var startSearching = selectedRow == null;
    foreach (var row in DataGrid.View.Records.Select(record => record.Data))
    {
      // Skip rows until we reach the row after the currently selected one
      if (!startSearching)
      {
        if (row == selectedRow) startSearching = true;
        continue;
      }
      var currentValue = Property.GetValue(row);
      if (EvaluatePredicate(currentValue, specifiedValue, predicate))
      {
        //Debug.WriteLine($"Found value: {value} in row: {row}");
        var rowIndex = DataGrid.ResolveToRowIndex(row);
        DataGrid.SelectAllColumns(false);
        DataGrid.SelectCells(row, Column, row, Column);
        DataGrid.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Finds all occurrences of a specified value in the column and selects the corresponding cells.
  /// </summary>
  /// <returns>Number of selected cells</returns>
  public int FindAll()
  {
    int found = 0;
    var columnIndex = DataGrid.Columns.IndexOf(Column);
    var selectedRow = DataGrid.GetSelectedCells().FirstOrDefault()?.RowData;
    var startSearching = selectedRow == null;
    var firstRowIndex = -1;

    foreach (var row in DataGrid.View.Records.Select(record => record.Data))
    {
      var currentValue = Property.GetValue(row);
      if (EvaluatePredicate(currentValue, specifiedValue, predicate))
      {
        //Debug.WriteLine($"Found value: {value} in row: {row}");
        var rowIndex = DataGrid.ResolveToRowIndex(row);
        if (found == 0)
        {
          firstRowIndex = rowIndex;
          DataGrid.SelectAllColumns(false);
        }
        found++;
        DataGrid.SelectCells(row, Column, row, Column);
      }
    }
    if (firstRowIndex >= 0)
      DataGrid.ScrollInView(new RowColumnIndex(firstRowIndex, columnIndex));
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
      if (specifiedValue is ISelectableItem selectableItem) 
        specifiedValue = selectableItem?.ActualValue;
      if (currentValue is ISelectableItem currentItem)
        specifiedValue = currentItem?.ActualValue;

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