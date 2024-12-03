using System;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Extra tools for working with tables in OpenXml documents.
/// </summary>
public static class TableExtraTools
{

  /// <summary>
  /// Find the first and the last non-empty column in the group of rows.
  /// </summary>
  /// <param name="rows"></param>
  /// <returns></returns>
  public static (int firstNonEmptyColumn, int lastNonEmptyColumn) GetNonEmptyColumns(this IEnumerable<TableRow> rows)
  {
    // find the first non-empty column which will be the first column of the internal table
    // find the last non-empty column which will be the last column of the internal table
    var firstNonEmptyColumn = int.MaxValue;
    var lastNonEmptyColumn = 0;
    foreach (var row in rows)
    {
      (int firstNonEmptyCell, int lastNonEmptyCell) = row.GetNonEmptyColumns();
      if (firstNonEmptyCell < firstNonEmptyColumn)
        firstNonEmptyColumn = firstNonEmptyCell;
      if (lastNonEmptyCell > lastNonEmptyColumn)
        lastNonEmptyColumn = lastNonEmptyCell;
    }
    return (firstNonEmptyColumn, lastNonEmptyColumn);
  }

  /// <summary>
  /// Find the first and the last non-empty cell in the row.
  /// </summary>
  /// <param name="row"></param>
  /// <returns></returns>
  public static (int firstNonEmptyCell, int lastNonEmptyCell) GetNonEmptyColumns(this TableRow row)
  {
    var cells = row.GetCells().ToList();
    var firstNonEmptyCell = cells.FindIndex(c => !c.IsEmpty());
    var lastNonEmptyCell = cells.FindLastIndex(c => !c.IsEmpty());
    return (firstNonEmptyCell, lastNonEmptyCell);
  }

  /// <summary>
  /// Find the first and the last non-separated column in the group of rows.
  /// Only first sequence of non-separated cells are recognized.
  /// </summary>
  /// <param name="rows"></param>
  /// <returns></returns>
  public static (int firstNonSeparatedColumn, int lastNonSeparatedCOlumn) GetNonSeparatedColumns(this IEnumerable<TableRow> rows)
  {
    // find the first non-separated column which can be the first column of the fake table
    // find the last non-separated column which can be the last column of the fake table
    var firstNonSeparatedColumn = int.MaxValue;
    var lastNonSeparatedColumn = 0;
    foreach (var row in rows)
    {
      (int firstNonSeparatedCell, int lastNonSeparatedCell) = row.GetNonSeparatedColumns();
      if (firstNonSeparatedCell < firstNonSeparatedColumn)
        firstNonSeparatedColumn = firstNonSeparatedCell;
      if (lastNonSeparatedCell > lastNonSeparatedColumn)
        lastNonSeparatedColumn = lastNonSeparatedCell;
    }
    return (firstNonSeparatedColumn, lastNonSeparatedColumn);
  }

  /// <summary>
  /// Find the first and the last non-separated cell in the row.
  /// Non-separated cells are the cells which have no borders between.
  /// Only first sequence of non-separated cells is recognized.
  /// </summary>
  /// <param name="row"></param>
  /// <returns></returns>
  public static (int firstNonSeparatedCell, int lastNonSeparatedCell) GetNonSeparatedColumns(this TableRow row)
  {
    var cells = row.GetCells().ToList();
    int firstNonSeparatedCell = int.MaxValue;
    int lastNonSeparatedCell = 0;
    for (int i = 0; i < cells.Count - 1; i++)
    {
      var cell = cells[i];
      var nextCell = cells[i + 1];
      if (!cell.GetBorder<DXW.RightBorder>().IsVisible() && !nextCell.GetBorder<DXW.LeftBorder>().IsVisible())
      {
        if (firstNonSeparatedCell == int.MaxValue)
          firstNonSeparatedCell = i;
        if (i > lastNonSeparatedCell)
          firstNonSeparatedCell = i;
      }
      if (nextCell.GetBorder<DXW.RightBorder>().IsVisible())
        break;
    }
    return (firstNonSeparatedCell, lastNonSeparatedCell);
  }


  /// <summary>
  /// Get the cells in the selected column from the table.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="columnNdx"></param>
  /// <returns></returns>
  public static List<TableCell> GetCellsInColumn(this Table table, int columnNdx)
  {
    return table.GetRows().GetCellsInColumn(columnNdx);
  }

  /// <summary>
  /// Get the cells in the selected column from the group of rows.
  /// </summary>
  /// <param name="rows"></param>
  /// <param name="columnNdx"></param>
  /// <returns></returns>
  public static List<TableCell> GetCellsInColumn(this IEnumerable<TableRow> rows, int columnNdx)
  {
    var column = new List<TableCell>();
    foreach (var row in rows)
    {
      var cell = row.GetCell(columnNdx);
      column.Add(cell ?? new TableCell());
    }
    return column;
  }

  /// <summary>
  /// Check if the cells in the cellColumn are merged.
  /// True if the first cell has VerticalMerge element
  /// and additionally first VerticalMerge element has value of Restart.
  /// </summary>
  /// <param name="cellColumn"></param>
  /// <returns></returns>
  public static bool AreMerged(this IEnumerable<TableCell> cellColumn)
  {
    int i = 0;
    foreach (var cell in cellColumn)
    {
      var verticalMerge = cell.TableCellProperties?.VerticalMerge;
      if (verticalMerge == null)
        return false;
      if (i == 0)
      {
        if (verticalMerge.Val?.Value != MergedCellValues.Restart)
          return false;
      }
      i++;
    }
    return true;
  }

  /// <summary>
  /// Gets the new table grid of the table by evaluating all columns in the table.
  /// </summary>
  /// <param name="table"/>
  /// <returns></returns>
  public static TableGrid GetNewTableGrid(this Table table)
  {
    var tableGrid = new TableGrid();
    var firstRow = table.GetRows().FirstOrDefault();
    var tableWidth = (int)Math.Round(table.GetWidth() ?? 15.0 / 2.54 * 14440);
    if (firstRow != null)
    {
      var columnCount = firstRow.GetColumnsCount();
      for (int i = 0; i < columnCount; i++)
      {
        var newColumn = new GridColumn();
        var width = table.GetColumnWith(i);
        if (width != 0)
          //width = tableWidth / columnCount;
          newColumn.SetWidth(width);
        tableGrid.AppendChild(newColumn);
      }
    }
    return tableGrid;
  }

  /// <summary>
  /// Evaluate the column count in the row.
  /// </summary>
  /// <param name="aRow"></param>
  /// <returns></returns>
  public static int GetColumnsCount(this TableRow aRow)
  {
    var count = 0;
    foreach (var cell in aRow.GetCells())
    {
      count += cell.GetSpan();
    }
    return count;
  }

  /// <summary>
  /// Evaluate the width of the specific column in the table.
  /// Only cells which are not spanned are taken into account.
  /// </summary>
  /// <param name="aTable"></param>
  /// <param name="columnNdx">column index</param>
  /// <returns></returns>
  public static int GetColumnWith(this Table aTable, int columnNdx)
  {
    var cellColumn = aTable.GetCellsInColumn(columnNdx);
    var width = 0;
    foreach (var cell in cellColumn)
    {
      if (cell.GetSpan() > 1)
        continue;
      var aWidth = cell.GetWidth() ?? 0;
      if (aWidth > width)
        width = aWidth;
    }
    return width;
  }
}