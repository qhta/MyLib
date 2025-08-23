using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using Qhta.SF.WPF.Tools.Resources;
using Qhta.UndoManager;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

public static partial class SfDataGridCommander
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
    try
    {
      GetSelectedRowsAndColumns(dataGrid, out var allColumnsSelected, out var selectedColumns, out var allRowsSelected,
        out var selectedRows);

      var rowDataType = GetRowDataType(dataGrid);
      if (rowDataType == null)
      {
        Debug.WriteLine($"{op}: No row data type found.");
        return false;
      }

      GridColumnInfo?[]? columnInfos = GetGridColumnInfos(selectedColumns, rowDataType, op == DataOp.Copy || op == DataOp.Cut);
      if (!columnInfos.Any())
      {
        Debug.WriteLine($"{op}: No column info available.");
        return false;
      }
      if (op == DataOp.Delete || op == DataOp.Cut)
      {
        if (!dataGrid.AllowDeleting)
        {
          Debug.WriteLine($"{op}: DataGrid does not allow data deleting.");
          return false;
        }
        if (allColumnsSelected)
        {
          var itemsSource = dataGrid.View?.SourceCollection;
          if (itemsSource == null)
          {
            Debug.WriteLine($"{op}: ItemsSource is null.");
            return false;
          }
          if (itemsSource is IList list && list.IsReadOnly)
          {
            Debug.WriteLine($"{op}: Data list is read-only");
            return false;
          }
          // Get the type of the ItemsSource
          var itemsSourceType = itemsSource.GetType();

          // Check if it implements ICollection<T>
          var iCollectionInterface = itemsSourceType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
          if (iCollectionInterface != null)
          {
            // Get the IsReadOnly property from ICollection<T>
            var isReadOnlyProperty = iCollectionInterface.GetProperty("IsReadOnly");
            if (isReadOnlyProperty != null)
            {
              // Get the value of IsReadOnly
              var isReadOnly = (bool)isReadOnlyProperty.GetValue(itemsSource)!;
              if (isReadOnly)
              {
                Debug.WriteLine($"{op}: Data collection is read-only");
                return false;
              }
            }
          }
          if (itemsSource is IRemovableCollection removableCollection)
          {
            if (removableCollection.IsReadOnly)
            {
              Debug.WriteLine($"{op}: Data collection is read-only");
              return false;
            }
            foreach (var item in selectedRows)
            {
              if (!removableCollection.CanRemove(item))
              {
                Debug.WriteLine($"{op}: Item cannot be removed");
                return false;
              }
            }
          }
        }
      }
      return true;
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
    if (!CanExecuteDataOp(dataGrid, op))
    {
      MessageBox.Show(String.Format(Strings.OperationNotAllowedInThisContext, op));
      return;
    }
    try
    {
      GetSelectedRowsAndColumns(dataGrid, out var allColumnsSelected, out var selectedColumns, out var allRowsSelected,
        out var selectedRows);

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
          if (MessageBox.Show(Strings.DeleteAllDataConfirm, Strings.Confirm, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
            return;
        }
        else if (allColumnsSelected)
        {
          int rowsCount = selectedRows.Count();
          if (MessageBox.Show(String.Format(Strings.DeleteRecordsConfirm, rowsCount), Strings.Confirm, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
            return;
        }
      }

      //await Task.Factory.StartNew(() =>
      //{
      if (op == DataOp.Cut || op == DataOp.Delete)
        UndoRedoManager.StartGrouping();

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
        UndoRedoManager.StopGrouping();
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
        UndoRedoManager.StopGrouping();
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
          UndoManager.UndoRedoManager.StartGrouping();
          foreach (var row in selectedRows)
          {
            errorMessageProvider = row as IErrorMessageProvider;
            var delRecordArgs = new DelRecordArgs(dataSource, dataSource.IndexOf(row), row);
            UndoManager.UndoRedoManager.Record(new DelRecordAction(), delRecordArgs);
            dataSource.Remove(row);
          }
          UndoManager.UndoRedoManager.StopGrouping();
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