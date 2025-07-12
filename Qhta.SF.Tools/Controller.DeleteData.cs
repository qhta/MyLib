using System.Diagnostics;
using System.Windows;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

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

  public static void DeleteData(SfDataGrid grid)
  {
    try
    {
      var noColumnsSelected = false;
      var selectedCells = grid.GetSelectedCells().ToArray();
      GridColumn[] selectedColumns;
      if (selectedCells.Length != 0)
        selectedColumns = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        selectedColumns = grid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!selectedColumns.Any())
      {
        noColumnsSelected = true;
        selectedColumns = grid.Columns.ToArray();
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

      if (noColumnsSelected && noRowsSelected && rowsSelected.Length>0)
      {
        if (MessageBox.Show(DataStrings.DeleteAllDataConfirmation, null, MessageBoxButton.YesNo)== MessageBoxResult.No)
          return;

      }
      var rowDataType = rowsSelected.FirstOrDefault()?.GetType();


      GridColumnInfo?[]? columnInfos = null;
      if (rowDataType != null)
      {
        columnInfos = GetGridColumnInfos(selectedColumns, rowDataType, true);
      }
      var content = new List<string>();


      if (columnInfos != null && columnInfos.Any())
      {
        //await Task.Factory.StartNew(() =>
        //{

        foreach (var row in rowsSelected)
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
        //});
        Debug.WriteLine($"Delete data completed");
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }

  }

}