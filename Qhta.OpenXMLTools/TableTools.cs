using System;

using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;
using Qhta.TypeUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with tables in OpenXml documents.
/// </summary>
public static class TableTools
{

  /// <summary>
  /// Gets the text of all rows in the table.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this Table table, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    List<string> sl = new();
    var indentLevel = options.IndentLevel;
    string indentStr = "";
    if (options.IndentTableContent && options.TableRowInSeparateLine)
    {
      options.IndentLevel++;
      indentStr = options.Indent.Duplicate(options.IndentLevel) ?? "";
    }
    foreach (var element in table.Elements())
    {
      if (element is TableRow row)
      {
        if (options.TableRowInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableRowStartTag);
        var aText = row.GetText(options);
        if (aText != null)
          sl.Add(aText);
        if (options.TableRowInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableRowEndTag);
      }
    }
    options.IndentLevel = indentLevel;
    return string.Join("", sl);
  }

  /// <summary>
  /// Gets the text of all cells in the table row.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string? GetText(this TableRow row, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    List<string> sl = new();
    var indentLevel = options.IndentLevel;
    string indentStr = "";
    if (options.IndentTableContent && options.TableRowInSeparateLine)
    {
      options.IndentLevel++;
      indentStr = options.Indent.Duplicate(options.IndentLevel) ?? "";
    }
    foreach (var element in row.Elements())
    {
      if (element is TableCell cell)
      {
        if (options.TableCellInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableCellStartTag);
        var aText = cell.GetText(options);
        if (aText != null)
          sl.Add(aText);
        if (options.TableCellInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableCellEndTag);
      }
    }
    options.IndentLevel = indentLevel;
    return string.Join("", sl);
  }


  /// <summary>
  /// Gets the style name of the table.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public static string? Style(this Table table)
  {
    return table.Elements<TableProperties>().FirstOrDefault()?.TableStyle?.Val?.Value;
  }

  /// <summary>
  /// Gets the table properties of the table.
  /// If the <c>TableProperties</c> element is null, creates a new one.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public static TableProperties GetTableProperties(this Table table)
  {
    var tableProperties = table.Elements<TableProperties>().FirstOrDefault();
    if (tableProperties == null)
    {
      tableProperties = new TableProperties();
      table.AddChild(tableProperties);
    }
    return tableProperties;
  }

  /// <summary>
  /// Gets the table grid  of the table.
  /// If the <c>TableGrid</c> element is null, creates a new one.
  /// </summary>
  /// <param name="table"/>
  /// <returns></returns>
  public static TableGrid GetTableGrid(this Table table)
  {
    var tableGrid = table.Elements<TableGrid>().FirstOrDefault();
    if (tableGrid == null)
    {
      tableGrid = new TableGrid();
      table.AppendChild(tableGrid);
    }
    return tableGrid;
  }

  /// <summary>
  /// Try to keep the table on the same page.
  /// It is done by setting the first rows to be kept with next on the same page.
  /// Last row is never kept with next.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="rowLimit"></param>
  /// <returns></returns>
  public static bool TryKeepOnPage(this Table table, int rowLimit)
  {
    var rows = table.Elements<TableRow>().ToList();
    var rowNumber = 0;
    var keepAll = rows.Count <= rowLimit;
    if (keepAll)
      rowLimit = rows.Count - 1;
    foreach (var row in rows)
    {
      rowNumber++;
      row.SetKeepWithNext(rowNumber <= rowLimit);
    }
    return true;
  }

  /// <summary>
  /// Try to limit left indent to zero.
  /// </summary>
  /// <param name="table"></param>
  /// <returns>true if indent was less than zero</returns>
  public static bool TryLimitLeftIndent(this Table table)
  {
    var tableProperties = table.GetTableProperties();
    var tableIndent = tableProperties.TableIndentation;
    if (tableIndent?.Type?.Value == TableWidthUnitValues.Dxa)
    {
      var indent = (int?)tableIndent.Width!;
      if (indent < 0)
      {
        if (table.GetLeftMargin() == -indent)
        {
          table.SetLeftMargin(0);
        }
        tableIndent.Width = 0;

        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Try to limit table width.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="widthLimit"></param>
  /// <returns>true if limit was set, false if the current table width is less then limit</returns>
  public static bool LimitWidth(this Table table, ulong widthLimit)
  {
    var tableGrid = table.GetTableGrid();
    var gridColumns = tableGrid.Elements<GridColumn>().ToList();
    ulong totalWidth = 0;
    foreach (var gridColumn in gridColumns)
    {
      var width = gridColumn.GetWidth();
      if (width != null)
        totalWidth += (uint)width;
      else
        return false;
    }

    var tableProperties = table.GetTableProperties();
    var tableIndent = tableProperties.TableIndentation;
    if (tableIndent?.Type?.Value == TableWidthUnitValues.Dxa)
    {
      var indent = (int?)tableIndent.Width!;
      if (indent < 0)
      {
        tableIndent.Width = 0;
      }
      else if (indent > 0)
      {
        widthLimit -= (ulong)indent;
      }
    }

    if (totalWidth <= widthLimit)
      return false;

    var ratio = (double)widthLimit / totalWidth;
    tableProperties.TableWidth = new TableWidth { Width = widthLimit.ToString(), Type = TableWidthUnitValues.Dxa };
    int columIndex = 0;
    //var newColumnWidths = new List<int>();
    foreach (var column in gridColumns)
    {
      var width = column.GetWidth();
      if (width != null)
      {
        var newWidth = (int)(ratio * (uint)width);
        //newColumnWidths.Add(newWidth);
        column.SetWidth(newWidth);
        table.SetColumnCellsWidth(columIndex, newWidth);
        columIndex++;
      }
    }
    //Debug.WriteLine(String.Join("; ", newColumnWidths.Select(item => item.ToString())));
    return true;
  }

  /// <summary>
  /// Set the width of all cells in the column.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="columnIndex"></param>
  /// <param name="width"></param>
  public static void SetColumnCellsWidth(this DXW.Table table, int columnIndex, int width)
  {
    var rows = table.Elements<TableRow>().ToList();
    foreach (var row in rows)
    {
      var cell = row.GetCell(columnIndex);
      if (cell != null)
      {
        cell.SetWidth(width);
      }
    }
  }

  /// <summary>
  /// Set the height of all rows in the table to auto.
  /// </summary>
  /// <param name="table"></param>
  public static int ClearRowsHeight(this DXW.Table table)
  {
    int count = 0;
    foreach (var row in table.Elements<DXW.TableRow>())
    {
      row.GetTableRowProperties().SetTableRowHeight(0, DXW.HeightRuleValues.Auto);
      count++;
    }
    return count;
  }

  /// <summary>
  /// Set the height of all rows in the table to auto.
  /// </summary>
  /// <param name="table">Table to process</param>
  /// <param name="left">Left cell margin (in twips)</param>
  /// <param name="top">Top cell margin (in twips)</param>
  /// <param name="right">Right cell margin (in twips)</param>
  /// <param name="bottom">Bottom cell margin (in twips)</param>
  /// <returns>Number of cell affected</returns>
  public static int SetUniformCellMargins(this DXW.Table table, int left, int top, int right, int bottom)
  {
    int count = 0;
    var tableCellMarginDefault = table.GetTableProperties().GetTableCellMarginDefault();
    tableCellMarginDefault.SetMargins(left, top, right, bottom);
    foreach (var row in table.Elements<DXW.TableRow>())
    {
      foreach (var cell in row.Elements<TableCell>())
      {
        var done = false;
        var cellProperties = cell.GetTableCellProperties();
        if (cellProperties.TableCellMargin != null)
        {
          cellProperties.TableCellMargin = null;
          done = true;
        }
        foreach (var paragraph in cell.Elements<DXW.Paragraph>())
        {
          var paragraphProperties = paragraph.GetParagraphProperties();
          if (paragraphProperties.Indentation != null)
          {
            paragraphProperties.Indentation = null;
            done = true;
          }
        }
        if (done)
          count++;
      }
    }
    return count;
  }

  /// <summary>
  /// Get the section properties of the table.
  /// </summary>
  /// <returns></returns>
  public static DXW.SectionProperties? GetSectionProperties(this Table table)
  {
    var parent = table.Parent;
    DX.OpenXmlElement? element = table;
    while (parent != null && parent is not DXW.Body)
    {
      element = parent;
      parent = element.Parent;
    }
    if (parent is DXW.Body && element is DXW.Table topTable)
    {
      element = topTable.NextSibling<DXW.Paragraph>();
    }
    if (parent is DXW.Body && element is DXW.Paragraph topParagraph)
    {
      return topParagraph.GetSectionProperties();
    }
    if (parent is DXW.Body body)
    {
      element = body.Elements<DXW.SectionProperties>().LastOrDefault();
      if (element != null)
        return (DXW.SectionProperties)element;
    }
    return null;
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
  public static bool TryFixInvalidColumns(this DXW.Table table)
  {
    var done = false;
    var columns = table.GetTableGrid().Elements<GridColumn>().ToList();
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
              cell.SetGridSpan(cell.GetGridSpan() - 1);
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
  /// Checks if the paragraph is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Table? element)
  {
    if (element == null)
      return true;
    foreach (var row in element.Elements<TableRow>())
    {
      if (!row.IsEmpty())
        return false;
    }
    return true;
  }

  /// <summary>
  /// Get the left margin of TableCellMarginDefault.LeftMargin.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public static int? GetLeftMargin(this DXW.Table table)
  {
    var tableProperties = table.GetTableProperties();
    var tableCellMarginDefault = tableProperties.TableCellMarginDefault;
    if (tableCellMarginDefault?.TableCellLeftMargin?.Type?.Value == TableWidthValues.Dxa)
    {
      return (int?)tableCellMarginDefault?.TableCellLeftMargin?.Width?.Value ?? 0;
    }
    return null;
  }

  /// <summary>
  /// Set the left margin of TableCellMarginDefault.LeftMargin.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetLeftMargin(this DXW.Table table, int? value)
  {
    var tableProperties = table.GetTableProperties();
    var tableCellMarginDefault = tableProperties.GetTableCellMarginDefault();
    var element = tableCellMarginDefault.TableCellLeftMargin;
    if (value == null)
    {
      if (element != null)
        element.Remove();
      return;
    }
    if (element == null)
    {
      element = new TableCellLeftMargin();
      tableCellMarginDefault.TableCellLeftMargin = element;
    }
    element.Type = TableWidthValues.Dxa;
    element.Width = new DX.Int16Value{ Value = (short)value };
  }


  /// <summary>
  /// Get the right margin of TableCellMarginDefault.LeftMargin.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public static int? GetRightMargin(this DXW.Table table)
  {
    var tableProperties = table.GetTableProperties();
    var tableCellMarginDefault = tableProperties.TableCellMarginDefault;
    if (tableCellMarginDefault?.TableCellRightMargin?.Type?.Value == TableWidthValues.Dxa)
    {
      return (int?)tableCellMarginDefault?.TableCellRightMargin?.Width?.Value ?? 0;
    }
    return null;
  }
}