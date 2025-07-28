using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using Qhta.UndoManager;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{
  /// <summary>
  /// Enumeration representing the type of data operation to perform.
  /// </summary>
  public enum DataOp
  {
    /// <summary>
    /// Data should be copied to clipboard.
    /// </summary>
    Copy,
    /// <summary>
    /// Data should be cut from the grid and copied to clipboard.
    /// </summary>
    Cut,
    /// <summary>
    /// Data should be deleted from the grid.
    /// </summary>
    Delete,
  }


  /// <summary>
  /// Determines whether a data operation can be executed on the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <param name="op"></param>
  /// <returns></returns>
  public static bool CanExecuteDataOp(SfDataGrid dataGrid, DataOp op)
  {
    if (op == DataOp.Copy)
      return true;
    try
    {
      var selectedCells = dataGrid.GetSelectedCells().ToArray();
      GridColumn[] selectedColumns;
      if (selectedCells.Length != 0)
        selectedColumns = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        selectedColumns = dataGrid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!selectedColumns.Any())
      {
        selectedColumns = dataGrid.Columns.ToArray();
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
  /// Performs a copy, cut or delete operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <param name="op"></param>
  public static void ExecuteDataOp(SfDataGrid dataGrid, DataOp op)
  {
    try
    {
      GetSelectedRowsAndColumns(dataGrid, out var allColumnsSelected, out var selectedColumns, out var allRowsSelected,
        out var selectedRows);
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        Debug.WriteLine("DataOp: No rows selected.");
        return;
      }

      var rowDataType = GetRowDataType(dataGrid);
      if (rowDataType == null)
      {
        Debug.WriteLine("DataOp: No row data type found.");
        return;
      }

      GridColumnInfo?[]? columnInfos = GetGridColumnInfos(selectedColumns, rowDataType, op == DataOp.Copy || op == DataOp.Cut);
      if (!columnInfos.Any())
      {
        Debug.WriteLine("DataOp: No column info available.");
        return;
      }
      ;

      var content = new List<string>();

      if (op == DataOp.Copy || op == DataOp.Cut)
      {
        if (selectedRows.Length != 1 || selectedColumns.Length != 1)
        {
          var headers = GetHeaders(dataGrid, selectedColumns);
          var headerLine = string.Join("\t", headers);
          content.Add(headerLine);
        }
      }


      if (op == DataOp.Delete)
      {
        if (allColumnsSelected && allRowsSelected)
        {
          if (MessageBox.Show(DataStrings.DeleteAllDataConfirm, DataStrings.Confirm, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
            return;
        }
        else if (allColumnsSelected)
        {
          int rowsCount = selectedRows.Count();
          if (MessageBox.Show(String.Format(DataStrings.DeleteRecordsConfirm, rowsCount), DataStrings.Confirm, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
            return;
        }
      }

      //await Task.Factory.StartNew(() =>
      //{
      if (op == DataOp.Cut || op == DataOp.Delete)
        UndoMgr.StartGrouping();

      if (op == DataOp.Delete)
      {
        if (allColumnsSelected)
          DeleteRecords(dataGrid, selectedRows);
        else
          DeleteCells(selectedColumns, columnInfos, selectedRows);
      }
      else
      {
        foreach (var row in selectedRows)
        {
          var cellValues = selectedColumns.Select(column =>
          {
            var cellInfo = new GridCellInfo(column, row, null);
            var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
            object? cellData = null;
            if (columnInfo != null)
            {
              cellData = GetCellData(cellInfo, columnInfo);
              if (op == DataOp.Cut && !allColumnsSelected)
                SetCellData(cellInfo, columnInfo, null);
            }
            return cellData?.ToString() ?? string.Empty;
          }).ToArray();
          var line = string.Join("\t", cellValues);
          //Debug.WriteLine($"{line}");
          content.Add(line);

        }
        if (op == DataOp.Cut && allColumnsSelected)
          DeleteRecords(dataGrid, selectedRows);
        Clipboard.Clear();
        var text = string.Join(Environment.NewLine, content);
        Clipboard.SetText(text, TextDataFormat.Text);
      }
      //});
      if (op == DataOp.Cut || op == DataOp.Delete)
        UndoMgr.StopGrouping();
      switch (op)
      {
        case DataOp.Copy:
          Debug.WriteLine("Copy data completed");
          break;
        case DataOp.Cut:
          Debug.WriteLine("Cut data completed");
          break;
        case DataOp.Delete:
          Debug.WriteLine("Delete data completed");
          break;
      }

    }
    catch (Exception e)
    {
      if (op == DataOp.Cut || op == DataOp.Delete)
        UndoMgr.StopGrouping();
      Console.WriteLine(e);
    }
  }

  /// <summary>
  /// Gets the selected rows and columns from the specified <see cref="SfDataGrid"/>.
  /// Returns the selected cells and outputs whether all columns or rows are selected, along with the selected columns and rows.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <param name="allColumnsSelected"></param>
  /// <param name="selectedColumns"></param>
  /// <param name="allRowsSelected"></param>
  /// <param name="selectedRows"></param>
  /// <returns></returns>
  public static GridCellInfo[] GetSelectedRowsAndColumns
  (this SfDataGrid dataGrid, out bool allColumnsSelected, out GridColumn[] selectedColumns, out bool allRowsSelected,
    out object[] selectedRows)
  {
    allColumnsSelected = false;
    var selectedCells = dataGrid.GetSelectedCells().ToArray();
    if (selectedCells.Length != 0)
      selectedColumns = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
    else
      selectedColumns = dataGrid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
    if (!selectedColumns.Any())
    {
      selectedColumns = dataGrid.Columns.ToArray();
    }
    else if (selectedColumns.Length == dataGrid.Columns.Count())
    {
      allColumnsSelected = true;
    }

    allRowsSelected = false;
    if (selectedCells.Length != 0)
      selectedRows = selectedCells.Select(cell => cell.RowData).Distinct().ToArray();
    else
      selectedRows = dataGrid.SelectionController.SelectedRows.Select(row => row.RowData).ToArray();
    if (!selectedRows.Any())
    {
      selectedRows = dataGrid.View.Records.Select(record => record.Data).ToArray();
    }
    if (selectedRows.Length == 0)
    {
      Debug.WriteLine("DataOp: No rows selected.");
      return selectedCells;
    }
    if (selectedRows.Length == dataGrid.View.Records.Count())
    {
      allRowsSelected = true;
    }
    return selectedCells;
  }

  private static void DeleteRecords(SfDataGrid dataGrid, object[] selectedRows)
  {
    if (dataGrid.ItemsSource is IList dataSource)
    {
      IErrorMessageProvider? errorMessageProvider = null;
      try
      {
        lock (dataGrid.ItemsSource)
        {
          UndoManager.UndoMgr.StartGrouping();
          foreach (var row in selectedRows)
          {
            errorMessageProvider = row as IErrorMessageProvider;
            var delRecordArgs = new DelRecordArgs(dataSource, dataSource.IndexOf(row), row);
            UndoManager.UndoMgr.Record(new DelRecordAction(), delRecordArgs);
            dataSource.Remove(row);
          }
          UndoManager.UndoMgr.StopGrouping();
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
  }

  private static void DeleteCells(GridColumn[] selectedColumns, GridColumnInfo?[]? columnInfos, object[] selectedRows)
  {
    foreach (var row in selectedRows)
    {
      foreach (var column in selectedColumns)
      {
        var cellInfo = new GridCellInfo(column, row, null, -1, false);
        var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
        if (columnInfo != null)
        {
          SetCellData(cellInfo, columnInfo, null);
        }
      }
    }

  }
}