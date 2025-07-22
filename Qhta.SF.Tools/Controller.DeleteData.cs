using System.Collections;
using System.Diagnostics;
using System.Windows;

using Qhta.UndoManager;

using Syncfusion.Data;
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
  /// <param name="dataGrid"></param>
  public static void DeleteData(SfDataGrid dataGrid)
  {
    try
    {
      var allColumnsSelected = false;
      var selectedCells = dataGrid.GetSelectedCells().ToArray();
      GridColumn[] columnsToDelete;
      if (selectedCells.Length != 0)
        columnsToDelete = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        columnsToDelete = dataGrid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!columnsToDelete.Any())
      {
        columnsToDelete = dataGrid.Columns.ToArray();
      }
      else if (columnsToDelete.Length == dataGrid.Columns.Count())
      {
        allColumnsSelected = true;
      }

      var allRowsSelected = false;
      object[] rowsSelected;
      if (selectedCells.Length != 0)
        rowsSelected = selectedCells.Select(cell => cell.RowData).Distinct().ToArray();
      else
        rowsSelected = dataGrid.SelectionController.SelectedRows.Select(row => row.RowData).ToArray();
      if (!rowsSelected.Any())
      {
        rowsSelected = dataGrid.View.Records.Select(record => record.Data).ToArray();
      }

      if (rowsSelected.Length == 0)
      {
        Debug.WriteLine("DeleteData: No rows to delete.");
        return;
      }
      if (rowsSelected.Length == dataGrid.View.Records.Count())
      {
        allRowsSelected = true;
      }

      if (allColumnsSelected && allRowsSelected)
      {
        if (MessageBox.Show(DataStrings.DeleteAllDataConfirmation, null, MessageBoxButton.YesNo) == MessageBoxResult.No)
          return;

      }

      if (allColumnsSelected)
      {
        if (dataGrid.ItemsSource is IList dataSource)
        {
          IErrorMessageProvider? errorMessageProvider = null;
          try
          {
            lock (dataGrid.ItemsSource)
            {
              UndoMgr.StartGrouping();
              foreach (var row in rowsSelected)
              {
                errorMessageProvider = row as IErrorMessageProvider;
                var delRecordArgs = new DelRecordArgs(dataSource, dataSource.IndexOf(row), row);
                UndoMgr.Record(new DelRecordAction(), delRecordArgs);
                dataSource.Remove(row);
              }
              UndoMgr.StopGrouping();
            }
          }
          catch (Exception e)
          {
            Debug.WriteLine($"DeleteData: Error removing rows from data source: {e.Message}");
            if (errorMessageProvider != null)
            {
              errorMessageProvider.ErrorMessage = e.Message;
            }
          }
        }
        return;
      }

      var rowDataType = GetRowDataType(dataGrid);
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