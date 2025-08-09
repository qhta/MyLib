using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{
  /// <summary>
  /// Checks if the specific <see cref="SfDataGrid"/> column is selected or not.
  /// </summary>
  /// <param name="column">Column to check</param>
  public static bool IsSelected(this GridColumn column)
  {
    return (!SfDataGridColumnBehavior.GetIsSelected(column));
  }

  /// <summary>
  /// Selects or unselects the specific <see cref="SfDataGrid"/> column.
  /// Any cell selection is cleared before selecting or unselecting the column.
  /// </summary>
  /// <param name="column">Column to select/unselect</param>
  /// <param name="select">True for select, false for unselect.</param>
  /// <param name="unselectOther">True to unselect other columns</param>
  public static void SetIsSelected(this GridColumn column, bool select, bool unselectOther)
  {
    // In the current implementation, we know that the column DataGrid property is not public.
    if (column.GetDataGrid() is { } dataGrid)
    {
      dataGrid.SelectionController.ClearSelections(false);
      if (unselectOther)
      {
        // Unselect all columns if unselectOther is true
        foreach (var col in dataGrid.Columns) SfDataGridColumnBehavior.SetIsSelected(col, false);
      }
    }
    SfDataGridColumnBehavior.SetIsSelected(column, select);
  }

  /// <summary>
  /// Checks if all columns in the specified <see cref="SfDataGrid"/> are selected or not.
  /// </summary>
  /// <param name="dataGrid">The <see cref="SfDataGrid"/> instance.</param>
  public static bool AreAllColumnsSelected(this SfDataGrid dataGrid)
  {
    if (dataGrid.Columns.Count==0)
      return false;
    foreach (var column in dataGrid.Columns)
      if (!SfDataGridColumnBehavior.GetIsSelected(column))
        return false;
    return true;
  }

  /// <summary>
  /// Selects or unselects all columns in the specified <see cref="SfDataGrid"/>.
  /// Any cell selection is cleared before selecting or unselecting columns.
  /// </summary>
  /// <param name="dataGrid">The <see cref="SfDataGrid"/> instance.</param>
  /// <param name="select">True for select all, false for unselect all.</param>
  public static void SelectAllColumns(this SfDataGrid dataGrid, bool select)
  {
    dataGrid.SelectionController.ClearSelections(false);
    foreach (var column in dataGrid.Columns) SfDataGridColumnBehavior.SetIsSelected(column, select);
  }

}