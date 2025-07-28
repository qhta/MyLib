using System.Diagnostics;
using System.Reflection;
using System.Windows;

using Qhta.UndoManager;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

  /// <summary>
  /// Determines whether data can be pasted into the specified data grid.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <returns></returns>
  public static bool CanPasteData(SfDataGrid dataGrid) => Clipboard.ContainsText();

  /// <summary>
  /// Performs a paste operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="dataGrid"></param>
  public static void PasteData(SfDataGrid dataGrid)
  {
    var text = Clipboard.GetText();
    string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
    if (lines.Length == 0) return;
    if (lines.Length == 1 && lines[0].Length == 0) return;
    var pasteHeaders = GetHeaders(lines[0]);
    if (pasteHeaders.Length == 0) return;
    var allColumns = dataGrid.Columns.ToArray();
    var allHeaders = GetHeaders(dataGrid, allColumns);
    var foundHeaders = allHeaders.Intersect(pasteHeaders).ToArray();

    var noColumnsSelected = false;
    var selectedCells = dataGrid.GetSelectedCells().ToArray();
    GridColumn[] columnsToFill;
    if (selectedCells.Length != 0)
      columnsToFill = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
    else
      columnsToFill = dataGrid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
    if (!columnsToFill.Any())
    {
      columnsToFill = dataGrid.Columns.ToArray();
    }

    object[] rowsToCopy;
    if (selectedCells.Length != 0)
      rowsToCopy = selectedCells.Select(cell => cell.RowData).Distinct().ToArray();
    else
      rowsToCopy = dataGrid.SelectionController.SelectedRows.Select(row => row.RowData).ToArray();
    if (!rowsToCopy.Any())
    {
      rowsToCopy = dataGrid.View.Records.Select(record => record.Data).ToArray();
    }
    var rowDataType = GetRowDataType(dataGrid);
    if (rowDataType == null)
    {
      Debug.WriteLine("PasteData: No row data type found.");
      return;
    }
    string[] content;
    if (foundHeaders.Any())
    {
      columnsToFill = allColumns.Where(column => foundHeaders.Contains(column.HeaderText)).Distinct().ToArray();
      content = lines.Skip(1).ToArray();
    }
    else
    {
      content = lines;
    }
    if (noColumnsSelected || !columnsToFill.Any())
      return;
    GridColumnInfo?[] ? columnInfos = GetGridColumnInfos(columnsToFill, rowDataType, true);
    if (!columnInfos.Any())
    {
      Debug.WriteLine("PasteData: No columns data to paste.");
      return;
    }

    if (content.Length==0)
    {
      Debug.WriteLine("PasteData: No content to paste.");
      return;
    }
    UndoMgr.StartGrouping();
    var lineNo = 0;
    foreach (var row in rowsToCopy)
    {
      var line = content[lineNo++];
      if (line.Length == 0) continue;

      if (lineNo >=content.Length) lineNo = 0; // Loop through content if more rows than lines
      var cellValues = line.Split('\t');
      var columnNo = 0;
      foreach (var column in columnsToFill)
      {
        var cellStr = cellValues[columnNo++];
        if (columnNo >= cellValues.Length) columnNo = 0; // Loop through content line if more columns than line items
        var cellInfo = new GridCellInfo(column, row, null);
        var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
        if (columnInfo != null)
        {
          SetCellData(cellInfo, columnInfo, cellStr);
        }
      }
    }
    UndoMgr.StopGrouping();

  }

  /// <summary>
  /// Splits a tab-delimited string into an array of header values.
  /// </summary>
  /// <param name="headerLine">A string containing tab-separated header values. Cannot be null.</param>
  /// <returns>An array of strings, each representing a header value extracted from the input string.</returns>
  public static string[] GetHeaders(string headerLine)
  {
    var headers = headerLine.Split('\t');
    return headers;
  }
}