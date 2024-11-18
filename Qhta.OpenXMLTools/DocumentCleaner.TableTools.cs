using System;

using DocumentFormat.OpenXml.Spreadsheet;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A composite tool for cleaning a Wordprocessing document.
/// </summary>
public partial class DocumentCleaner
{
  private bool stop = false;
  /// <summary>
  /// Find multi-column rows in tables and make them internal tables.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FixInternalTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFixing internal tables");
    var body = wordDoc.GetBody();
    var count = 0;
    var tables = body.Descendants<DXW.Table>().ToList();
    for (int i = 0; i < tables.Count; i++)
    {
      var table = tables[i];
      Console.WriteLine($"  Checking table {i + 1}");
      if (i + 1 == 694)
        stop = true;
      if (TryFixInternalTable(table))
        count++;
    }
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} internal tables created");
  }

  /// <summary>
  /// If a table has rows with different number of cells than try to convert these rows to internal tables.
  /// We assume that the number of rows in the table is greater than 2.
  /// We also assume that the merged column is the second column.
  /// </summary>
  /// <param name="table"></param>
  public bool TryFixInternalTable(DXW.Table table)
  {
    //if (stop)
    //  Debug.Assert(true);
    var tableGrid = table.GetTableGrid();
    var tableGridColumns = tableGrid.GetColumns().ToList();
    if (tableGridColumns.Count <= 2)
      return false;
    if (table.Elements<DXW.TableRow>().Count() <= 2)
      return false;

    var emptyCellHeadersFixed = TryFixEmptyCellHeaders(table);
    if (emptyCellHeadersFixed)
      Debug.Assert(true);
    var done = false;
    var rowsList = table.GetRows().ToList();
    var rowsCellsCount = rowsList.ToDictionary(r => r, r => r.GetCells().Count());
    var maxColumns = rowsCellsCount.Values.Max();
    var uniformRows = rowsCellsCount.Count(r => r.Value == maxColumns);
    if (uniformRows == rowsCellsCount.Count)
      return false;

    //var rowGroupings = rowsCellsCount.GroupBy(item => item.Value,
    //  item => rowsList.IndexOf(item.Key));
    var rowGroups = GetRowGroups(rowsCellsCount);

    if (rowGroups.Count() <= 1)
      return false;

    for (var rowGroupNdx = 0; rowGroupNdx < rowGroups.Count; rowGroupNdx++)
    {
      var rowGroup = rowGroups[rowGroupNdx];
      if (rowGroup.CellsCount == maxColumns)
      {
        if (TryToCreateInternalTable(rowGroup))
        {
          done = true;
          if (rowGroupNdx > 0)
          {
            var previousGroup = rowGroups[rowGroupNdx - 1];
            foreach (var previousRow in previousGroup.Rows)
            {
              var mergedCell = previousRow.GetMergedCell(rowGroup.FirstNonEmptyColumn);
              if (mergedCell != null)
              {
                mergedCell.SetSpan(0);
              }
              if (previousRow == previousGroup.Rows.Last())
              {
                if (mergedCell != null)
                {
                  mergedCell.Append(rowGroup.InternalTable!);
                  mergedCell.Append(new DXW.Paragraph());
                }
              }
            }
            foreach (var row in rowGroup.Rows)
            {
              row.Remove();
            }
          }
        }
      }
      else
      {

      }
    }

    if (done)
    {
      table.SetTableGrid(table.GetNewTableGrid());
    }

    return done;
  }

  /// <summary>
  /// Check if the table has heading rows and try to fix them.
  /// Heading row is a row that is shaded and bordered
  /// Heading rows need to be fixed if they have empty cells.
  /// Fixing heading rows means joining empty cells with next non-empty cells.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public bool TryFixEmptyCellHeaders(DXW.Table table)
  {
    bool done = false;
    var columns = table.GetTableGrid().GetColumns().ToList();
    if (columns.Count <= 1)
      return false;
    foreach (var row in table.Elements<DXW.TableRow>())
    {
      if (IsHeadingRow(row))
        if (TryFixRowWithEmptyCells(row))
          done = true;
    }
    return done;
  }
  /// <summary>
  /// Check if the table has rows with empty cells and try to fix them.
  /// Fixing rows means joining empty cells with next non-empty cells.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public bool TryFixEmptyCellRows(DXW.Table table)
  {
    bool done = false;
    var columns = table.GetTableGrid().GetColumns().ToList();
    if (columns.Count <= 1)
      return false;
    foreach (var row in table.Elements<DXW.TableRow>())
    {
      if (TryFixRowWithEmptyCells(row))
        done = true;
    }
    return done;
  }

  /// <summary>
  /// Check if the row is a heading row.
  /// Heading row is a row that has at least one cell shaded and bordered.
  /// </summary>
  /// <param name="row"></param>
  /// <returns></returns>
  public bool IsHeadingRow(DXW.TableRow row)
  {
    foreach (var cell in row.Elements<DXW.TableCell>())
    {
      var shading = cell.TableCellProperties?.Shading;
      var borders = cell.TableCellProperties?.TableCellBorders;
      if (shading != null && borders != null)
        return true;
    }
    return false;
  }

  /// <summary>
  /// Try to fix row if the row has empty cells.
  /// These empty cells are joined with next non-empty cells.
  /// </summary>
  /// <param name="row"></param>
  /// <returns></returns>
  public bool TryFixRowWithEmptyCells(DXW.TableRow row)
  {
    bool done = false;
    var cells = row.GetCells().ToList();
    for (int i = 0; i < cells.Count; i++)
    {
      var cell = cells[i];
      if (cell.IsEmpty())
      {
        if (TryJoinEmptyCellWithNext(cell) || TryJoinEmptyCellWithPrevious(cell))
        {
          cell.Remove();
          done = true;
        }
      }
    }
    return done;
  }

  /// <summary>
  /// Try to join empty emptyCell with the next non-empty cell.
  /// Join is possible if the next cell is not empty and
  /// there is no border between empty cell and non-empty cell.
  /// </summary>
  /// <param name="emptyCell"></param>
  /// <returns></returns>
  public bool TryJoinEmptyCellWithNext(DXW.TableCell emptyCell)
  {
    var nextCell = emptyCell.NextSibling() as DXW.TableCell;
    if (nextCell == null)
      return false;
    if (nextCell.IsEmpty())
      return false;
    if (emptyCell.GetRightBorder().IsVisible() || !nextCell.GetLeftBorder().IsVisible())
      return false;
    nextCell.SetLeftBorder(emptyCell.GetLeftBorder());
    nextCell.SetWidth(nextCell.GetWidth() + emptyCell.GetWidth());
    nextCell.SetSpan(nextCell.GetSpan() + 1);
    emptyCell.Remove();
    return true;
  }
  /// <summary>
  /// Try to join empty emptyCell with the previous non-empty cell.
  /// Join is possible if the previous cell is not empty and
  /// there is no border between empty cell and non-empty cell.
  /// </summary>
  /// <param name="emptyCell"></param>
  /// <returns></returns>
  public bool TryJoinEmptyCellWithPrevious(DXW.TableCell emptyCell)
  {
    var previousCell = emptyCell.PreviousSibling() as DXW.TableCell;
    if (previousCell == null)
      return false;
    if (previousCell.IsEmpty())
      return false;
    if (emptyCell.GetLeftBorder().IsVisible() || !previousCell.GetRightBorder().IsVisible())
      return false;
    previousCell.SetRightBorder(emptyCell.GetRightBorder());
    previousCell.SetWidth(previousCell.GetWidth() + emptyCell.GetWidth());
    previousCell.SetSpan(previousCell.GetSpan() + 1);
    emptyCell.Remove();
    return true;
  }

  /// <summary>
  /// Check it the table has empty columns and remove them.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public bool TryRemoveEmptyColumns(DXW.Table table)
  {
    var done = false;
    var columns = table.GetTableGrid().GetColumns().ToList();
    for (int columnNdx = columns.Count - 1; columnNdx >= 0; columnNdx--)
    {
      var columnCells = table.GetCellsInColumn(columnNdx);
      if (columnCells.All(c => c.IsEmpty() && c.GetSpan() > 1))
      {
        var column = columns[columnNdx];
        column.Remove();
        done = true;
        if (columnNdx > 0)
        {
          var previousColumn = columns[columnNdx - 1];
          previousColumn.SetWidth(previousColumn.GetWidth() + column.GetWidth());
          var previousColumnCells = table.GetCellsInColumn(columnNdx - 1);
          previousColumnCells.ForEach(c => c.SetSpan(c.GetSpan() - 1));
        }
      }
    }
    return done;
  }

  /// <summary>
  /// Helper record for grouping rows by the number of cells.
  /// </summary>
  public record RowGroup
  {
    /// <summary>
    /// Start of the group (row index).
    /// </summary>
    public int StartIndex;

    /// <summary>
    /// RowsCount of the group;
    /// </summary>
    public int RowsCount;

    /// <summary>
    /// CellsCount of the group.
    /// </summary>
    public int CellsCount;

    /// <summary>
    /// First non-empty column in the group.
    /// </summary>
    public int FirstNonEmptyColumn;

    /// <summary>
    /// Last non-empty column in the group.
    /// </summary>
    public int LastNonEmptyColumn;

    /// <summary>
    /// List of rows in the group.
    /// </summary>
    public List<DXW.TableRow> Rows = new();

    /// <summary>
    /// Internal table created from the group.
    /// </summary>
    public DXW.Table? InternalTable;
  }

  /// <summary>
  /// Helper method for grouping rows by the number of cells.
  /// </summary>
  /// <param name="rowsCellsCount"></param>
  /// <returns></returns>
  private List<RowGroup> GetRowGroups(Dictionary<DXW.TableRow, int> rowsCellsCount)
  {
    var rowsList = rowsCellsCount.Keys.ToList();
    var rowGroups = new List<RowGroup>();
    var rowGroup = new RowGroup();
    foreach (var row in rowsCellsCount)
    {
      if (rowGroup.RowsCount == 0)
      {
        rowGroup.StartIndex = rowsList.IndexOf(row.Key);
        rowGroup.RowsCount = 1;
        rowGroup.CellsCount = row.Value;
        rowGroup.Rows.Add(row.Key);
      }
      else if (row.Value == rowGroup.CellsCount)
      {
        rowGroup.RowsCount++;
        rowGroup.Rows.Add(row.Key);
      }
      else
      {
        rowGroups.Add(rowGroup);
        rowGroup = new RowGroup();
        rowGroup.StartIndex = rowsList.IndexOf(row.Key);
        rowGroup.RowsCount = 1;
        rowGroup.CellsCount = row.Value;
        rowGroup.Rows.Add(row.Key);
      }
    }
    if (rowGroup.RowsCount > 0)
      rowGroups.Add(rowGroup);
    return rowGroups;
  }

  /// <summary>
  /// Helper method for creating internal table.
  /// If the rowGroup has empty columns from the left or from the right
  /// then we can create an internal table.
  /// The internal table has non-empty columns copied from the original table.
  /// </summary>
  /// <param name="rowGroup"></param>
  /// <returns></returns>
  private bool TryToCreateInternalTable(RowGroup rowGroup)
  {
    var done = false;
    (int firstNonEmptyColumn, int lastNonEmptyColumn) = (rowGroup.Rows).GetNonEmptyColumns();
    rowGroup.FirstNonEmptyColumn = firstNonEmptyColumn;
    rowGroup.LastNonEmptyColumn = lastNonEmptyColumn;
    if (firstNonEmptyColumn > 0 || lastNonEmptyColumn < rowGroup.CellsCount - 1)
    {
      if (rowGroup.Rows.First().Parent is DXW.Table parentTable)
      {
        var internalTable = new DXW.Table();
        done = true;
        var tableGrid = parentTable.GetTableGrid();
        var tableGridColumns = tableGrid.GetColumns().ToList();
        var internalTableGrid = internalTable.GetTableGrid();
        for (int i = firstNonEmptyColumn; i <= lastNonEmptyColumn; i++)
        {
          internalTableGrid.AppendChild(tableGridColumns[i].CloneNode((true)));
        }
        foreach (var row in rowGroup.Rows)
        {
          var newRow = new DXW.TableRow();
          newRow.TableRowProperties = row.TableRowProperties?.CloneNode(true) as DXW.TableRowProperties;
          var cells = row.GetCells().ToList();
          for (int i = firstNonEmptyColumn; i <= lastNonEmptyColumn; i++)
          {
            var cell = cells[i];
            var newCell = cell.CloneNode(true) as DXW.TableCell;
            newRow.AppendChild(newCell);
          }
          internalTable.AppendChild(newRow);
        }
        if (firstNonEmptyColumn > 0)
        {
          var indentColumnNdx = firstNonEmptyColumn - 1;
          var indentColumnCells = rowGroup.Rows.GetCellsInColumn(indentColumnNdx);
          if (indentColumnCells.AreMerged())
          {
            var indent = tableGridColumns[indentColumnNdx].GetWidth();
            if (indent != null)
            {
              internalTable.GetTableProperties().TableIndentation = new DXW.TableIndentation
              { Width = indent, Type = DXW.TableWidthUnitValues.Dxa };
            }
          }
        }
        rowGroup.InternalTable = internalTable;
      }
    }
    return done;
  }


  /// <summary>
  /// Find tables that have invalid columns and fix them.
  /// Such tables have rows filled with empty cells.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FixTablesWithInvalidColumns(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFixing tables with invalid columns");
    var body = wordDoc.GetBody();
    var count = FixTablesWithInvalidColumns(body);
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} tables fixed");
  }

  /// <summary>
  /// Find tables that have invalid columns and fix them.
  /// Such tables have rows filled with empty cells.
  /// </summary>
  /// <param name="body">Processed body</param>
  /// <returns>number of tables fixed</returns>
  public int FixTablesWithInvalidColumns(DX.OpenXmlCompositeElement body)
  {
    var count = 0;
    var tables = body.Descendants<DXW.Table>().ToList();
    foreach (var table in tables)
    {
      //if (table.Elements<DXW.TableRow>().RowsCount() <= 3)
      //  continue;
      var firstRow = table.GetFirstChild<DXW.TableRow>();
      var firstCell = firstRow?.GetFirstChild<DXW.TableCell>();
      var borders = firstCell?.TableCellProperties?.GetFirstChild<DXW.TableCellBorders>();
      if (borders == null)
        continue;
      if (borders.LeftBorder?.Val?.Value == DXW.BorderValues.Nil)
        continue;
      var shading = firstCell?.TableCellProperties?.GetFirstChild<DXW.Shading>();
      if (shading == null || shading.Fill != "C0C0C0")
        continue;
      //var nextRow = firstRow?.NextSibling() as DXW.TableRow;
      //firstCell = nextRow?.GetFirstChild<DXW.TableCell>();
      //var firstParagraph = firstCell?.GetFirstChild<DXW.Paragraph>();
      //if (firstParagraph != null)
      //  Debug.WriteLine($"{i + 1}: {firstParagraph.GetText()}");
      if (TryFixInvalidColumns(table))
        count++;
    }
    return count;
  }


  /// <summary>
  /// Check if the table has invalid columns and fix them.
  /// Only tables which have borders are considered.
  /// Table has invalid columns if each row has some empty cells and the number of cells in each row is less to the number of columns.
  /// If we find a column which is empty in all rows, we can safely remove this column in a table grid and in each row.
  /// If there are adjacent columns which are empty in some rows, we try check if we can merge cells in these columns in each row.
  /// If it is possible, we merge these cells and remove the higher column in a table grid.
  /// The lower column width is increased by the width of the higher column.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public bool TryFixInvalidColumns(DXW.Table table)
  {
    var done = false;
    var columns = table.GetTableGrid().Elements<DXW.GridColumn>().ToList();
    var columnsCount = columns.Count();
    var columnUsage = new List<int>(new int[columnsCount]);
    var rows = table.Elements<DXW.TableRow>().ToList();
    foreach (var row in rows)
    {
      var filledCellsCount = 0;
      for (int columnNdx = 0; columnNdx < columnsCount; columnNdx++)
      {
        var cell = row.GetCell(columnNdx);
        if (cell != null && !cell.IsEmpty())
        {
          filledCellsCount++;
          columnUsage[columnNdx]++;
        }

      }
      if (filledCellsCount == columnsCount)
      {
        return false;
      }
    }
    for (int columnNdx = 0; columnNdx < columnUsage.Count; columnNdx++)
    {
      if (columnUsage[columnNdx] == 0)
      {
        foreach (var row in rows)
        {
          // Remove column from row
          var cell = row.GetCell(columnNdx);
          if (cell != null)
          {
            cell.Remove();
          }
          else
          {
            cell = row.GetMergedCell(columnNdx);
            if (cell != null)
            {
              cell.SetSpan(cell.GetSpan() - 1);
            }
          }
        }
        var column = table.GetTableGrid().Elements<DXW.GridColumn>().ElementAt(columnNdx);
        column.Remove();
        columnsCount--;
        columnUsage.RemoveAt(columnNdx);
        //columnNdx--;
        done = true;
      }
      else if (columnUsage[columnNdx] < rows.Count)
      {
        for (int rowNdx = 0; rowNdx < rows.Count; rowNdx++)
        {
          var row = rows[rowNdx];
          row.JoinCellWithNext(columnNdx);
        }
        var column1 = columns[columnNdx];
        if (columnNdx < columns.Count - 1)
        {
          var column2 = columns[columnNdx + 1];
          column1.SetWidth(column1.GetWidth() + column2.GetWidth());
          if (column2.Parent != null)
            column2.Remove();
          columnsCount--;
          columnUsage.RemoveAt(columnNdx);
          //columnNdx--;
        }
        done = true;
      }
    }
    return done;
  }


  /// <summary>
  /// Joins adjacent tables that have the same number of columns.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void JoinAdjacentTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nJoining adjacent tables");
    var body = wordDoc.GetBody();
    var count = body.JoinAdjacentTables();
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} tables appended to previous ones");
  }


  /// <summary>
  /// Fix page-divided tables.
  /// Page-divided table is (usually long) table that have been split across consecutive pages
  /// so that the headings of the table are repeated on each page.
  /// Sometimes there are no repeating headings but the table is divided.
  /// After <see cref="JoinAdjacentTables"/> it should be a single table.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FixDividedTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFixing divided tables");
    var fixedTables = 0;
    int removedRows = 0;
    int joinedRows = 0;
    foreach (var table in wordDoc.GetBody().Descendants<DXW.Table>())
    {
      var openXmlComparableSimpleValue = table.GetTableLook()?.FirstRow;
      if (openXmlComparableSimpleValue != null && openXmlComparableSimpleValue == true)
      {
        if (FixTableWithRepeatedHeaders(table, out var removed, out var joined))
        {
          fixedTables++;
          removedRows += removed;
          joinedRows += joined;
        }
      }
      else
      {
        if (FixTableWithWithDividedRows(table, out var joined))
        {
          fixedTables++;
          joinedRows += joined;
        }
      }
    }

    if (VerboseLevel > 0)
    {
      Console.WriteLine($"  {fixedTables} tables fixed");
      Console.WriteLine($"  {removedRows} repeated headings removed");
      Console.WriteLine($"  {joinedRows} divided rows joined");
    }
  }

  /// <summary>
  /// Check if the table has first (heading) row repeated in the middle of the table.
  /// These repeating heading rows should be removed.
  /// Then the last row of the first part of the table
  /// should be joined with the first row of the second part.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="removedRepeatedHeadings">count of removed repeated headings</param>
  /// <param name="joinedRows">count of joined rows</param>
  /// <returns>true when a table was divided</returns>
  public bool FixTableWithRepeatedHeaders(DXW.Table table, out int removedRepeatedHeadings, out int joinedRows)
  {
    removedRepeatedHeadings = 0;
    joinedRows = 0;
    var done = false;
    var tableRows = table.Elements<DXW.TableRow>().ToList();
    var headingText = tableRows[0].GetText();
    for (int i = 1; i < tableRows.Count; i++)
    {
      var row = tableRows[i];
      var rowText = row.GetText();
      if (rowText != headingText)
        continue;
      // remove repeating heading row
      row.Remove();
      tableRows.RemoveAt(i);
      i--;
      done = true;
      removedRepeatedHeadings++;
      if (i < 1 || i >= tableRows.Count - 1)
        continue;

      var priorRow = tableRows[i];
      var nextRow = tableRows[i + 1];
      if (TryJoinDividedRows(priorRow, nextRow))
      {
        nextRow.Remove();
        tableRows.RemoveAt(i + 1);
        joinedRows++;
        i--;
      }
    }
    return done;
  }

  /// <summary>
  /// If the table does not have repeating headings, it can still be divided.
  /// We must check if two adjacent rows should be joined.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="joinedRows">count of joined rows</param>
  /// <returns>true when a table was divided</returns>
  public bool FixTableWithWithDividedRows(DXW.Table table, out int joinedRows)
  {
    joinedRows = 0;
    var done = false;
    var tableRows = table.Elements<DXW.TableRow>().ToList();
    for (int i = 1; i < tableRows.Count; i++)
    {
      var priorRow = tableRows[i - 1];
      var nextRow = tableRows[i];
      if (TryJoinDividedRows(priorRow, nextRow))
      {
        nextRow.Remove();
        tableRows.RemoveAt(i);
        joinedRows++;
        done = true;
        i--;
      }
    }
    return done;
  }

  /// <summary>
  /// Try to join two rows in page-divided table.
  /// Page-divided table is (usually long) table that have been split across consecutive pages
  /// so that the headings of the table are repeated on each page.
  /// The repeating heading rows has been removed.
  /// Now we try to join the last row of the first part of the table
  /// with the first row of the second part.
  /// </summary>
  /// <param name="upperRow">Upper table row.</param>
  /// <param name="lowerRow">Lower table row.</param>
  private bool TryJoinDividedRows(DXW.TableRow upperRow, DXW.TableRow lowerRow)
  {
    if (!ShouldBeJoined(upperRow, lowerRow))
      return false;
    var upperCells = upperRow.Elements<DXW.TableCell>().ToList();
    var lowerCells = lowerRow.Elements<DXW.TableCell>().ToList();
    if (upperCells.Count != lowerCells.Count)
      return false;
    for (int i = 0; i < upperCells.Count; i++)
    {
      var upperCell = upperCells[i];
      var lowerCell = lowerCells[i];
      if (!JoinDividedCells(upperCell, lowerCell))
        return false;
    }
    return true;
  }

  /// <summary>
  /// Check if two sibling rows in a table should be joined.
  /// First check if the number of cells in the rows are the same.
  /// Then check the corresponding cells.
  /// </summary>
  /// <param name="upperRow">Upper table row.</param>
  /// <param name="lowerRow">Lower table row.</param>
  private bool ShouldBeJoined(DXW.TableRow upperRow, DXW.TableRow lowerRow)
  {
    var upperCells = upperRow.Elements<DXW.TableCell>().ToList();
    var lowerCells = lowerRow.Elements<DXW.TableCell>().ToList();
    if (upperCells.Count != lowerCells.Count)
      return false;
    var check = 0;
    for (int i = 0; i < upperCells.Count; i++)
    {
      var upperCell = upperCells[i];
      var lowerCell = lowerCells[i];
      check += ShouldBeJoined(upperCell, lowerCell);
    }
    return check > 0;
  }

  /// <summary>
  /// Check if two corresponding cells in sibling table rows should be joined.
  /// Check the last paragraph in the upper cell and the first paragraph in the lower cell.
  /// Returns 2 if the cells should definitely be joined.
  /// Returns 1 if the cells should rather be joined.
  /// Returns -1 if the cells should rather not be joined,
  /// Returns -2 if the cells should definitely not be joined.
  /// Returns 0 if the algorithm can't tell. 
  /// </summary>
  /// <param name="upperCell">Upper table row.</param>
  /// <param name="lowerCell">Lower table row.</param>
  private int ShouldBeJoined(DXW.TableCell upperCell, DXW.TableCell lowerCell)
  {
    if (upperCell.TableCellProperties?.TableCellBorders?.BottomBorder?.Val?.Value == DXW.BorderValues.Nil
        && lowerCell.TableCellProperties?.TableCellBorders?.TopBorder?.Val?.Value == DXW.BorderValues.Nil)
      return -2;
    var upperPara = upperCell.Elements<DXW.Paragraph>().LastOrDefault();
    var lowerPara = lowerCell.Elements<DXW.Paragraph>().FirstOrDefault();
    if (upperPara == null || lowerPara == null)
      return 0;
    // If the lower paragraph is empty then the cells should definitely be joined
    // because it is very probable that the cells were created by division.
    if (lowerPara.IsEmpty())
      return 2;

    // Last paragraph in the upper cell should end with a run element
    // and the first paragraph in the lower cell should start with a run element
    // to be possibly parts of one cell. If they are not then we can't tell.
    if (upperPara.Elements().LastOrDefault() is not DXW.Run && lowerPara.Elements().FirstOrDefault() is not DXW.Run)
      return 0;

    var upperText = upperPara.GetText().Trim();
    var lowerText = lowerPara.GetText().Trim();

    // If the last paragraph in the upper cell ends with a comma
    // then it is very possible that the cells were created by division.
    var upperTextEndsWithSentenceDivMark = upperText.EndsWith(",");
    if (upperTextEndsWithSentenceDivMark)
      return 2;

    // If the last paragraph in the upper cell ends with a dot, exclamation mark, question mark or colon
    // then it is possible that the cells were created by division, but be can't tell it.
    var upperTextEndsWithSentenceEndMark = upperText.EndsWith(".")
                                           || upperText.EndsWith("!")
                                           || upperText.EndsWith("?")
                                           || upperText.EndsWith(":");
    if (upperTextEndsWithSentenceEndMark)
      return 0;

    var upperSentences = upperText.GetSentences();
    var lowerSentences = lowerText.GetSentences();

    // If the last paragraph in the upper cell and the first paragraph in the lower cell
    // both do not contain any sentence than it is rather not possible that the cells were created by division.
    if (upperSentences.Count == 0 && lowerSentences.Count == 0)
      return -1;

    // if the first paragraph in the lower cell
    // starts with a non-letter character then the cells were rather not divided.
    if (!char.IsLetter(lowerText.FirstOrDefault()))
      return -1;
    return 0;
  }


  /// <summary>
  /// Join two corresponding cells in possibly divided row.
  /// Last paragraph in the upper cell and the first paragraph in the lower cell should be joined.
  /// If it is not possible to join the cells then return false.
  /// </summary>
  /// <param name="upperCell">Cell taken from the upper table row.</param>
  /// <param name="lowerCell">Cell taken from the lower table row.</param>
  public bool JoinDividedCells(DXW.TableCell upperCell, DXW.TableCell lowerCell)
  {
    var upperPara = upperCell.GetMembers().LastOrDefault() as DXW.Paragraph;
    var lowerPara = lowerCell.GetMembers().FirstOrDefault() as DXW.Paragraph;
    if (upperPara != null && lowerPara != null)
    {
      if (!lowerPara.IsEmpty())
      {
        var lastElement = upperPara.MemberElements().LastOrDefault();
        var firstElement = lowerPara.MemberElements().FirstOrDefault();
        if (lastElement == null || firstElement == null)
          return true;
        if (lastElement is DXW.Hyperlink lastHyperlink && firstElement is DXW.Hyperlink firstHyperlink)
        {
          if (lastHyperlink.GetRel().IsEqual(firstHyperlink.GetRel()))
          {
            lastHyperlink.SetText(lastHyperlink.GetText() + firstHyperlink.GetText());
            firstHyperlink.Remove();
          }
          foreach (var item in lowerPara.MemberElements().ToList())
          {
            item.Remove();
            upperPara.AppendChild(item);
          }
          lowerPara.Remove();
        }
        else
        {
          var upperText = upperPara.GetText();
          var lowerText = lowerPara.GetText();
          if (char.IsLower(lowerText.FirstOrDefault()))
          {
            if (!upperText.EndsWith(" ") && !lowerText.StartsWith(" "))
            {
              upperPara.AppendChild(new DXW.Run(new DXW.Text(" ")));
            }
            foreach (var item in lowerPara.MemberElements().ToList())
            {
              item.Remove();
              upperPara.AppendChild(item);
            }
            lowerPara.Remove();
          }
          else
          {
            if (!upperText.EndsWith(" ") && !lowerText.StartsWith(" "))
            {
              upperPara.AppendChild(new DXW.Run(new DXW.Text(" ")));
            }
            upperPara.AppendChild(new DXW.Run(new DXW.Text(" ")));
            foreach (var item in lowerPara.MemberElements().ToList())
            {
              item.Remove();
              upperPara.AppendChild(item);
            }
            lowerPara.Remove();
          }
        }
      }
      else
      {
        lowerPara.Remove();
        upperCell.Append(lowerPara);
      }
    }

    var tailingMembers = lowerCell.GetMembers().ToList();
    foreach (var member in tailingMembers)
    {
      if (member != lowerPara)
      {
        member.Remove();
        upperCell.Append(member);
      }
    }
    return true;
  }

  /// <summary>
  /// Browse through the document and join paragraphs in the first column of tables.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void JoinParagraphsInFirstColumn(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nJoin paragraphs in first column");
    var body = wordDoc.GetBody();
    var count = body.JoinParagraphsInFirstColumn();
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} cells fixed");
  }

  /// <summary>
  /// Browse paragraphs and break them before the specified string.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <param name="str">string to break paragraphs before</param>
  public void BreakParagraphsBefore(DXPack.WordprocessingDocument wordDoc, string str)
  {
    if (VerboseLevel > 0)
      Console.WriteLine($"\nBreak paragraphs before \"{str}\"");
    var body = wordDoc.GetBody();
    var count = body.BreakParagraphsBefore(str);
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} paragraphs broken");
  }

  /// <summary>
  /// Format all tables in the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FormatTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFormatting tables");
    var body = wordDoc.GetBody();
    var formatted = 0;
    var indented = 0;
    var limited = 0;
    var rowsCleared = 0;
    var cellMarginsSet = 0;
    var tables = body.Descendants<DXW.Table>().ToList();
    foreach (var table in tables)
    {
      if (TryFormatTable(table))
        formatted++;
      if (TryLimitLeftIndent(table))
        indented++;
      if (TryLimitWidth(table))
        limited++;
      rowsCleared += SetRowsHeightAuto(table);
      cellMarginsSet += SetUniformCellMargins(table);
    }
    if (VerboseLevel > 0)
    {
      Console.WriteLine($"  {formatted} tables formatted");
      Console.WriteLine($"  {indented} tables negative indent set to zero");
      Console.WriteLine($"  {limited} tables width limited");
      Console.WriteLine($"  {rowsCleared} rows height cleared");
      Console.WriteLine($"  {cellMarginsSet} cells margins uniformed");

    }
  }

  /// <summary>
  /// Keep short tables on the same page.
  /// </summary>
  /// <param name="table"></param>
  public bool TryFormatTable(DXW.Table table)
  {
    // ReSharper disable once ReplaceWithSingleAssignment.False
    var done = table.TryKeepOnPage(5);
    return done;
  }

  /// <summary>
  /// Keep short tables on the same page.
  /// </summary>
  /// <param name="table"></param>
  public bool TryLimitLeftIndent(DXW.Table table)
  {
    bool done = table.TryLimitLeftIndent();
    return done;
  }

  /// <summary>
  /// Keep short tables on the same page.
  /// </summary>
  /// <param name="table"></param>
  public bool TryLimitWidth(DXW.Table table)
  {
    var done = false;
    var sectionProperties = table.GetSectionProperties();
    if (sectionProperties != null)
    {
      var widthLimit = sectionProperties.GetPageSize()?.Width?.Value ?? 0;
      widthLimit -= sectionProperties.GetPageMargin()?.Left?.Value ?? 0;
      widthLimit -= sectionProperties.GetPageMargin()?.Right?.Value ?? 0;

      if (widthLimit > 0)
      {
        if (table.LimitWidth((uint)widthLimit))
          done = true;
      }
    }
    return done;
  }

  /// <summary>
  /// Set the height of all rows in the table to auto.
  /// </summary>
  /// <param name="table"></param>
  public int SetRowsHeightAuto(DXW.Table table)
  {
    var done = table.ClearRowsHeight();
    return done;
  }

  /// <summary>
  /// Set the cell margins to uniform values
  /// </summary>
  /// <param name="table"></param>
  public int SetUniformCellMargins(DXW.Table table)
  {
    var done = table.SetUniformCellMargins(75, 50, 75, 50);
    return done;
  }
}