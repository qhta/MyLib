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
  /// <param name="grid"></param>
  /// <param name="op"></param>
  /// <returns></returns>
  public static bool CanExecuteDataOp(SfDataGrid grid, DataOp op)
  {
    if (op == DataOp.Copy)
      return true;
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
  /// Performs a copy, cut or delete operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <param name="op"></param>
  public static void ExecuteDataOp(SfDataGrid dataGrid, DataOp op)
  {
    try
    {
      var allColumnsSelected = false;
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
      else if (selectedColumns.Length == dataGrid.Columns.Count())
      {
        allColumnsSelected = true;
      }

      var allRowsSelected = false;
      object[] selectedRows;
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
        return;
      }
      if (selectedRows.Length == dataGrid.View.Records.Count())
      {
        allRowsSelected = true;
      }

      var rowDataType = GetRowDataType(dataGrid);
      if (rowDataType == null)
      {
        Debug.WriteLine("DataOp: No row data type found.");
        return;
      }

      bool del = op == DataOp.Cut || op == DataOp.Delete;
      GridColumnInfo?[]? columnInfos = GetGridColumnInfos(selectedColumns, rowDataType, del);

      var content = new List<string>();

      if (selectedRows.Length != 1 || selectedColumns.Length != 1)
      {
        var headers = GetHeaders(dataGrid, selectedColumns);
        var headerLine = string.Join("\t", headers);
        content.Add(headerLine);
        //Debug.WriteLine($"{headerLine}'.");
        Clipboard.SetText(headerLine);
      }

      if (!columnInfos.Any())
        return;

      if (op == DataOp.Delete && allColumnsSelected && allRowsSelected)
      {
        if (MessageBox.Show(DataStrings.DeleteAllDataConfirmation, null, MessageBoxButton.YesNo) == MessageBoxResult.No)
          return;

      }

      //await Task.Factory.StartNew(() =>
      //{
      if (del)
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
            var cellInfo = new GridCellInfo(column, row, null, -1, false);
            var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
            object? cellData = null;
            if (columnInfo != null)
            {
              cellData = GetCellData(cellInfo, columnInfo);
              if (del && !allColumnsSelected)
                SetCellData(cellInfo, columnInfo, null);
            }
            return cellData?.ToString() ?? string.Empty;
          }).ToArray();
          var line = string.Join("\t", cellValues);
          //Debug.WriteLine($"{line}");
          content.Add(line);

        }
        if (del && allColumnsSelected)
          DeleteRecords(dataGrid, selectedRows);
        if (del)
          UndoMgr.StopGrouping();
        Clipboard.Clear();
        var text = string.Join(Environment.NewLine, content);
        Clipboard.SetText(text, TextDataFormat.Text);
      }
      //});
      switch (op)
      {
        case DataOp.Copy:
          Debug.WriteLine($"Copy data completed");
          break;
        case DataOp.Cut:
          Debug.WriteLine($"Cut data completed");
          break;
        case DataOp.Delete:
          Debug.WriteLine($"Delete data completed");
          break;
      }

    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
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
          UndoMgr.StartGrouping();
          foreach (var row in selectedRows)
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