using System.Collections;
using System.Reflection;
using System.Windows;

using Qhta.MVVM;
using Qhta.SF.Tools.Resources;
using Qhta.SF.Tools.Views;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

/// <summary>
/// Specifies how to find a value in the context of the current selection.
/// </summary>
public enum FindInSequence
{
  /// <summary>
  /// Find the next occurrence of the value.
  /// </summary>
  FindNext,
  /// <summary>
  /// Find the first occurrence of the value.
  /// </summary>
  FindFirst,
  /// <summary>
  /// Find all occurrences of the value.
  /// </summary>
  FindAll,
}

/// <summary>
/// Command to find a value selected by the user int the current column in the data grid.
/// </summary>
public class FindCommand : Command
{
  /// <summary>
  /// Specifies the last data grid where the find operation was performed.
  /// </summary>
  public SfDataGrid? LastDataGrid { get; private set; }
  /// <summary>
  /// Specifies the last column where the find operation was performed.
  /// </summary>
  public GridColumn? LastColumn { get; private set; }
  /// <summary>
  /// Specifies the last dialog view mode.
  /// </summary>
  public SpecificViewMode? LastViewMode { get; private set; }
  /// <summary>
  /// Value of the last search value in the column. It can be a selectable item or a text value.
  /// </summary>
  public object? LastValue { get; private set; }
  /// <summary>
  /// Search predicate used in the last find operation. It is used to filter the values in the column.
  /// </summary>
  public FilterPredicate? LastPredicate { get; private set; } = null!;
  /// <summary>
  /// Specifies the last find operation, which can be FindNext, FindFirst, or FindAll.
  /// </summary>
  public FindInSequence? LastFindInSequence { get; private set; }
  /// <summary>
  /// Specifies the last filter type, which can be Equals, NotEquals, and so on.
  /// </summary>
  public FilterType? LastFilterType { get; private set; }
  /// <summary>
  /// Specifies whether the last find operation was case-sensitive or not.
  /// </summary>
  public bool? LastCaseSensitive { get; private set; }


  /// <summary>
  /// Checks whether the command can be executed basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  /// <returns></returns>
  public override bool CanExecute(object? parameter)
  {
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        //Debug.WriteLine("FindInColumnCommand: No rows selected.");
        return false;
      }
      //Debug.WriteLine($"FindInColumnCommandCanExecute: Column {column.MappingName} is selected.");
      return true;
    }
    //Debug.WriteLine("FindInColumnCommandCanExecute: No column selected.");
    return false;
  }

  /// <summary>
  /// Checks whether the FindNext command can be executed basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  /// <returns></returns>
  public bool CanExecuteFindNext(object? parameter)
  {
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    if (LastDataGrid != dataGrid)
    {
      //Debug.WriteLine("FindInColumnCommand: LastDataGrid does not match the current data grid.");
      return false;
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      if (LastColumn != column)
      {
        //Debug.WriteLine("FindInColumnCommand: LastColumn does not match the current column.");
        return false;
      }
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        //Debug.WriteLine("FindInColumnCommand: No rows selected.");
        return false;
      }
      //Debug.WriteLine($"FindInColumnCommandCanExecute: Column {column.MappingName} is selected.");
      return true;
    }
    //Debug.WriteLine("FindInColumnCommandCanExecute: No column selected.");
    return false;
  }

  /// <summary>
  /// Executes the command basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  public override void Execute(object? parameter)
  {
    if (!CanExecute(parameter))
    {
      //Debug.WriteLine("FindInColumnCommand: Cannot execute command.");
      return;
    }
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      SpecificViewMode mode = (column is GridComboBoxColumn) ? SpecificViewMode.Both : SpecificViewMode.Edit;

      var selectValueWindow = new SpecificValueWindow
      {
        Prompt = String.Format(Strings.EnterValueForField, column.HeaderText),
        WindowMode = SpecificWindowMode.Find,
        ViewMode = mode,
      };

      if (column is GridComboBoxColumn comboBoxColumn && comboBoxColumn.ItemsSource is IEnumerable<ISelectableItem> selectableItems)
      {
        var items = selectableItems.ToList();
        if (items.First() is { } firstItem && firstItem.DisplayName != Strings.EmptyValue)
        {
          // Add "Empty" item if not present.                        
          items.Insert(0, new SelectableItemStub { DisplayName = Strings.EmptyValue });
        }
        items.Insert(1, new SelectableItemStub { DisplayName = Strings.NonEmptyValue });
        selectValueWindow.ItemsSource = items;
        if (LastDataGrid == dataGrid && LastColumn == column && LastValue is ISelectableItem lastSelectableItem)
        {
          var foundItem = items.FirstOrDefault(item => item.DisplayName == lastSelectableItem.DisplayName);
          if (foundItem != null)
          {
            selectValueWindow.SelectedItem = foundItem;
          }
        }
      }

      if (LastDataGrid == dataGrid && LastColumn == column)
      {
        selectValueWindow.TextValue = LastValue as string;
        if (LastFindInSequence != null)
          selectValueWindow.FindInSequence = (FindInSequence)LastFindInSequence;
        if (LastFilterType != null)
          selectValueWindow.FilterType = (FilterType)LastFilterType;
        if (LastCaseSensitive != null)
          selectValueWindow.CaseSensitive = (bool)LastCaseSensitive;
      }

      var dialogResult = selectValueWindow.ShowDialog();

      if (dialogResult == true)
      {
        var viewMode = selectValueWindow.CurrentViewMode;
        var specifiedValue = (viewMode == SpecificViewMode.Selector)
          ? selectValueWindow.SelectedItem : selectValueWindow.TextValue;
        var findInSequence = selectValueWindow.FindInSequence;
        var filterType = selectValueWindow.FilterType;
        var caseSensitive = selectValueWindow.CaseSensitive;

        LastDataGrid = dataGrid;
        LastColumn = column;
        LastValue = specifiedValue;
        LastFindInSequence = findInSequence;
        LastViewMode = viewMode;
        LastFilterType = filterType;
        LastCaseSensitive = caseSensitive;
        var predicate = CreatePredicate(ref specifiedValue, viewMode, filterType, caseSensitive);
        LastPredicate = predicate;

        switch (findInSequence)
        {
          case FindInSequence.FindNext:
            var selectedRow = dataGrid.CurrentItem;
            if (selectedRow == null)
              goto FindFirst;
            if (!column.FindNext(specifiedValue, predicate, selectedRow))
              MessageBox.Show(Strings.NotMoreValueFound);
            break;
          case FindInSequence.FindFirst:
          FindFirst:
            if (!column.FindFirst(specifiedValue, predicate))
              MessageBox.Show(Strings.ValueNotFound);
            break;
          case FindInSequence.FindAll:
            var n = column.FindAll(specifiedValue, predicate);
            if (n == 0)
              MessageBox.Show(Strings.ValueNotFound);
            else
              MessageBox.Show(String.Format(Strings.FoundNValues, n));
            break;
        }
      }
    }
  }

  /// <summary>
  /// Executes the FindNext command basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  public void ExecuteFindNext(object? parameter)
  {
    if (!CanExecuteFindNext(parameter))
    {
      //Debug.WriteLine("FindInColumnCommand: Cannot execute command.");
      return;
    }
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var specifiedValue = LastValue;
      if (LastPredicate == null)
        throw new InvalidOperationException("LastPredicate cannot be null when executing FindNext.");
      var predicate = LastPredicate;
      var selectedRow = selectedRows.FirstOrDefault();
      if (!column.FindNext(specifiedValue, predicate, selectedRow))
        MessageBox.Show(Strings.NotMoreValueFound);
    }
  }

  /// <summary>
  /// Checks if the specified value is a selectable item or a special marker and prepares it for comparison.
  /// if the value is an empty string, or it is an EmptyValue, it is treated as null.
  /// </summary>
  /// <param name="specifiedValue">Searched value. It can be an ISelectableItem or a text value</param>
  /// <param name="viewMode">Window view mode. Determines whether filter behavior is StronglyTyped or sStringTyped</param>
  /// <param name="filterType">Determines comparison functions</param>
  /// <param name="caseSensitive">Determines whether the text comparison in case-sensitive</param>
  /// <returns></returns>
  private static FilterPredicate CreatePredicate(ref object? specifiedValue, SpecificViewMode viewMode, FilterType filterType, bool caseSensitive)
  {
    if (viewMode == SpecificViewMode.Selector && specifiedValue is ISelectableItem selectableItem)
    {
      if (selectableItem.DisplayName == Strings.EmptyValue || selectableItem.DisplayName == string.Empty)
      {
        specifiedValue = null; // Treat empty value as null for filtering
        return new FilterPredicate { FilterBehavior = FilterBehavior.StronglyTyped, FilterType = FilterType.Equals };
      }
      if (selectableItem.DisplayName == Strings.NonEmptyValue)
      {
        specifiedValue = null; // Treat empty value as null for filtering
        return new FilterPredicate { FilterBehavior = FilterBehavior.StronglyTyped, FilterType = FilterType.NotEquals };
      }
      return new FilterPredicate { FilterBehavior = FilterBehavior.StronglyTyped, FilterType = FilterType.Equals };
    }
    else
    {
      specifiedValue = specifiedValue?.ToString();
      return new FilterPredicate { FilterBehavior = FilterBehavior.StringTyped, FilterType = filterType, IsCaseSensitive = caseSensitive, };
    }
  }
}
