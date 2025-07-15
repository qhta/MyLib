using System.Diagnostics;
using System.Windows;

using Qhta.UndoManager;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

  /// <summary>
  /// Determines whether data can be deleted from the specified data grid.
  /// This method checks if all selected columns are editable and not read-only.
  /// </summary>
  /// <param name="grid"></param>
  /// <returns></returns>
  public static bool CanDeleteData(SfDataGrid grid)
  {
    try
    {
      var selectedCells = grid.GetSelectedCells().ToArray();
      GridColumn[] selectedColumns;
      if (selectedCells.Length != 0)
        selectedColumns = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        selectedColumns = grid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!selectedColumns.Any())
      {
        selectedColumns = grid.Columns.ToArray();
      }
      return !selectedColumns.Any(column => column.AllowEditing && column.IsReadOnly);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
    return false;
  }

  /// <summary>
  /// Performs a delete operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="grid"></param>
  public static void DeleteData(SfDataGrid grid)
  {
    try
    {
      var noColumnsSelected = false;
      var selectedCells = grid.GetSelectedCells().ToArray();
      GridColumn[] columnsToDelete;
      if (selectedCells.Length != 0)
        columnsToDelete = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        columnsToDelete = grid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!columnsToDelete.Any())
      {
        noColumnsSelected = true;
        columnsToDelete = grid.Columns.ToArray();
      }

      var noRowsSelected = false;
      object[] rowsSelected;
      if (selectedCells.Length != 0)
        rowsSelected = selectedCells.Select(cell => cell.RowData).Distinct().ToArray();
      else
        rowsSelected = grid.SelectionController.SelectedRows.Select(row => row.RowData).ToArray();
      if (!rowsSelected.Any())
      {
        noRowsSelected = true;
        rowsSelected = grid.View.Records.Select(record => record.Data).ToArray();
      }

      if (rowsSelected.Length == 0)
      {
        Debug.WriteLine("DeleteData: No rows to delete.");
        return;
      }

      if (noColumnsSelected && noRowsSelected && rowsSelected.Length > 0)
      {
        if (MessageBox.Show(DataStrings.DeleteAllDataConfirmation, null, MessageBoxButton.YesNo) == MessageBoxResult.No)
          return;

      }
      var rowDataType = GetRowDataType(grid);
      if (rowDataType == null)
      {
        Debug.WriteLine("DeleteData: No row data type found.");
        return;
      }

      GridColumnInfo?[]? columnInfos = GetGridColumnInfos(columnsToDelete, rowDataType, true);
      if (!columnInfos.Any())
      {
        Debug.WriteLine("DeleteData: No columns data to delete.");
        return;
      }
      //await Task.Factory.StartNew(() =>
      //{
      UndoMgr.StartGrouping();
      foreach (var row in rowsSelected)
      {
        foreach (var column in columnsToDelete)
        {
          var cellInfo = new GridCellInfo(column, row, null, -1, false);
          var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
          if (columnInfo != null)
          {
            SetCellData(cellInfo, columnInfo, null);
          }
        }

      }
      UndoMgr.StopGrouping();
      //});
      Debug.WriteLine($"Delete data completed");
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }

  }

}