using System.Diagnostics;
using System.Windows;
using Qhta.UndoManager;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

  /// <summary>
  /// Performs a cut or copy operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="grid"></param>
  /// <param name="cut"></param>
  public static void CutCopyData(SfDataGrid grid, bool cut)
  {
    try
    {
      var selectedCells = grid.GetSelectedCells().ToArray();
      GridColumn[] columnsToCopy;
      if (selectedCells.Length != 0)
        columnsToCopy = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        columnsToCopy = grid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!columnsToCopy.Any())
      {
        columnsToCopy = grid.Columns.ToArray();
      }

      object[] rowsToCopy;
      if (selectedCells.Length != 0)
        rowsToCopy = selectedCells.Select(cell => cell.RowData).Distinct().ToArray();
      else
        rowsToCopy = grid.SelectionController.SelectedRows.Select(row => row.RowData).ToArray();
      if (!rowsToCopy.Any())
      {
        rowsToCopy = grid.View.Records.Select(record => record.Data).ToArray();
      }
      if (rowsToCopy.Length==0)
      {
        Debug.WriteLine("CutCopyData: No rows selected.");
        return;
      }

      var rowDataType = GetRowDataType(grid);
      if (rowDataType == null)
      {
        Debug.WriteLine("CutCopyData: No row data type found.");
        return;
      }

      GridColumnInfo?[]? columnInfos = GetGridColumnInfos(columnsToCopy, rowDataType, cut);

      var content = new List<string>();

      if (rowsToCopy.Length != 1 || columnsToCopy.Length != 1)
      {
        var headers = GetHeaders(grid, columnsToCopy);
        var headerLine = string.Join("\t", headers);
        content.Add(headerLine);
        //Debug.WriteLine($"{headerLine}'.");
        Clipboard.SetText(headerLine);
      }

      if (!columnInfos.Any())
        return;

      //await Task.Factory.StartNew(() =>
      //{
      if (cut)
        UndoMgr.StartGrouping();
      foreach (var row in rowsToCopy)
      {
        var cellValues = columnsToCopy.Select(column =>
        {
          var cellInfo = new GridCellInfo(column, row, null, -1, false);
          var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
          object? cellData = null;
          if (columnInfo != null)
          {
            cellData = GetCellData(cellInfo, columnInfo);
            if (cut)
            {
              SetCellData(cellInfo, columnInfo, null);
            }
          }
          return cellData?.ToString() ?? string.Empty;
        }).ToArray();
        var line = string.Join("\t", cellValues);
        //Debug.WriteLine($"{line}");
        content.Add(line);

      }
      if (cut)
        UndoMgr.StopGrouping();
      Clipboard.Clear();
      var text = string.Join(Environment.NewLine, content);
      Clipboard.SetText(text, TextDataFormat.Text);
      //});
      if (cut)
        Debug.WriteLine($"Cut data completed");
      else
        Debug.WriteLine($"Copy data completed");
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
  }

}