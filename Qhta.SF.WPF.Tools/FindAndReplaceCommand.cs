using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

using Qhta.MVVM;
using Qhta.SF.WPF.Tools.Resources;
using Qhta.SF.WPF.Tools.Views;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

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
/// Parameter that specifies whether to find or replace a value.
/// </summary>
public enum FindOrReplaceMode
{
  /// <summary>
  /// In automatic mode, the command decides whether to find or replace a value based on the column read/write capabilities.
  /// </summary>
  Auto,
  /// <summary>
  /// Find the value only, do not replace.
  /// </summary>
  Find,
  /// <summary>
  /// Replace the value - needs write access to the column.
  /// </summary>
  Replace
}

/// <summary>
/// Command to find a value selected by the user int the current column in the data grid.
/// </summary>
public class FindAndReplaceCommand : Command
{
  /// <summary>
  /// A mode that specifies whether to find or replace a value.
  /// </summary>
  public FindOrReplaceMode FindOrReplaceMode { get; set; } = FindOrReplaceMode.Auto;

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
    dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      // All columns selected or multiple columns selected
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        // No rows selected
        return false;
      }
      // Column selected
      if (FindOrReplaceMode == FindOrReplaceMode.Replace && (!column.AllowEditing || dataGrid.IsReadOnly))
      {
        // Cannot replace in a read-only column or grid
        return false;
      }
      return true;
    }
    // No column selected;
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

    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      // All columns selected or multiple columns selected
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var finder = column.GetFinder();
      if (finder == null)
      {
        // No rows selected
        return false;
      }
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        // No rows selected
        return false;
      }
      if (!finder.Found)
      {
        // Finder has not found any value yet
        return false;
      }
      if (FindOrReplaceMode == FindOrReplaceMode.Replace && (!column.AllowEditing || dataGrid.IsReadOnly))
      {
        // Cannot replace in a read-only column or grid
        return false;
      }
      return true;
    }
    // No column selected;
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
      //Debug.WriteLine("FindAndReplaceCommand.Execute: Cannot execute command.");
      return;
    }
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindAndReplaceCommand.Execute: No column selected or multiple columns selected.");
      return;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      SpecificViewMode mode = (column is GridComboBoxColumn) ? SpecificViewMode.Both : SpecificViewMode.Edit;

      var selectValueWindow = new SpecificValueWindow
      {
        Prompt = String.Format(Strings.EnterValueForField, column.HeaderText),
        ViewMode = mode,
      };
      List<ISelectableItem>? items = null;
      if (column is GridComboBoxColumn comboBoxColumn)
      {
        var itemTemplate = comboBoxColumn.ItemTemplate;

        if (comboBoxColumn.ItemsSource is IEnumerable<ISelectableItem> selectableItems)
        {
          items = selectableItems.ToList();
          if (items.First() is { } firstItem && !firstItem.IsEmpty)
          {
            // Add "Empty" item if not present.                        
            items.Insert(0, new SelectableItem { DisplayName = Strings.EmptyValue });
          }
          items.Insert(1, new SelectableItem { DisplayName = Strings.NonEmptyValue, Value = NonEmptyValue.Instance });
          selectValueWindow.ItemsSource = items;
          selectValueWindow.ReplacementSource = selectableItems;
        }
        else
        {
          items = new List<ISelectableItem>();
          var items2 = new List<ISelectableItem>(); // almost same as items, but without NonEmptyValue
          items.Add(new SelectableItem { DisplayName = Strings.EmptyValue });
          items2.Add(new SelectableItem { DisplayName = Strings.EmptyValue });
          items.Add(new SelectableItem { DisplayName = Strings.NonEmptyValue, Value = NonEmptyValue.Instance });
          foreach (var item in comboBoxColumn.ItemsSource)
            if (item != null)
            {
              items.Add(new SelectableItem { Value = item });
              items2.Add(new SelectableItem { Value = item });
            }
          selectValueWindow.ItemsSource = items;
          selectValueWindow.ReplacementSource = items2;
        }
      }

      SfDataGridFinder? finder = column.GetFinder();
      if (finder == null)
      {
        finder = new SfDataGridFinder { DataGrid = dataGrid, Column = column };
        column.SetFinder(finder);
      }
      else
      {
        selectValueWindow.CurrentViewMode = finder.StrongTyped ? SpecificViewMode.Selector : SpecificViewMode.Edit;
        selectValueWindow.FilterType = finder.Predicate.FilterType;
        selectValueWindow.CaseSensitive = finder.Predicate.IsCaseSensitive;
        if (finder.SpecifiedValue is string str)
          selectValueWindow.TextValue = str;
        else if (items != null)
        {
          if (finder.SpecifiedValue is ISelectableItem selectableItem)
          {
            var foundItem = items.FirstOrDefault(item => item.DisplayName == selectableItem.DisplayName);
            if (foundItem != null) selectValueWindow.SelectedItem = foundItem;
          }
          else
          {
            var foundItem = items.FirstOrDefault(item => item.Equals(finder.SpecifiedValue));
            if (foundItem != null) selectValueWindow.SelectedItem = foundItem;
          }
        }
      }

      if (FindOrReplaceMode == FindOrReplaceMode.Replace && (!column.AllowEditing || dataGrid.IsReadOnly))
      {
        throw new InvalidOperationException("Cannot replace in a read-only column or grid.");
      }
      if (FindOrReplaceMode == FindOrReplaceMode.Auto && (!dataGrid.IsReadOnly && finder.Property.CanWrite))
      {
        selectValueWindow.WindowMode = SpecificWindowMode.FindAndReplace;
        selectValueWindow.Replace = finder.Replace;
        selectValueWindow.ReplacementItem = finder.ReplacementValue;
        selectValueWindow.ReplacementText = finder.ReplacementValue?.ToString();
      }
      else
      {
        selectValueWindow.WindowMode = SpecificWindowMode.Find;
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
        var predicate = CreatePredicate(ref specifiedValue, viewMode, filterType, caseSensitive);
        finder.Predicate = predicate;
        finder.SpecifiedValue = specifiedValue;
        finder.Replace = selectValueWindow.Replace;
        finder.ReplacementValue = (viewMode == SpecificViewMode.Selector)
          ? selectValueWindow.ReplacementItem : selectValueWindow.ReplacementText;
        finder.StrongTyped = (viewMode == SpecificViewMode.Selector);
        switch (findInSequence)
        {
          case FindInSequence.FindNext:
            var selectedRow = dataGrid.CurrentItem;
            if (selectedRow == null)
              goto FindFirst;
            if (!finder.FindNext())
              MessageBox.Show(Strings.NotMoreValueFound);
            break;
          case FindInSequence.FindFirst:
          FindFirst:
            if (!finder.FindFirst())
              MessageBox.Show(Strings.ValueNotFound);
            break;
          case FindInSequence.FindAll:
            var n = finder.FindAll();
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
      //Debug.WriteLine("FindAndReplaceCommand.ExecuteFindNext: Cannot execute command.");
      return;
    }
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindAndReplaceCommand.ExecuteFindNext: No column selected or multiple columns selected.");
      return;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var finder = column.GetFinder();
      if (finder == null)
      {
        Debug.WriteLine("Finder not defined for FindNext column");
        return;
      }
      if (!finder.FindNext())
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
      if (selectableItem.IsEmpty)
      {
        specifiedValue = null; // Treat empty value as null for filtering
        return new FilterPredicate { FilterBehavior = FilterBehavior.StronglyTyped, FilterType = FilterType.Equals };
      }
      if (selectableItem.IsNonEmpty)
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
