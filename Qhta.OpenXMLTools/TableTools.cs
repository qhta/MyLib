using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

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
  /// Last row is newer kept with next.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="rowLimit"></param>
  /// <returns></returns>
  public static bool TryKeepOnPage(this Table table, int rowLimit)
  {
    var rows = table.Elements<TableRow>().ToList();
    var rowNumber = 0;
    foreach (var row in rows)
    {
      rowNumber++;
      row.SetKeepWithNext(rowNumber < rowLimit && rowNumber < rows.Count);
    }
    return rows.Count <= rowLimit + 1;
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
      var indent = tableIndent.Width!;
      if (indent < 0)
      {
        tableIndent.Width = 0;
      }
    }
    if (tableProperties.TableCellMarginDefault!=null)
      Debug.Assert(true);
    var tableLeftMargin = tableProperties.TableCellMarginDefault?.TableCellLeftMargin;
    if (tableLeftMargin?.Type?.Value == TableWidthValues.Dxa)
    {
      var margin = tableLeftMargin.GetValue();
      if (margin != null)
      {
        totalWidth -= (ulong)margin;
      }
    }
    var tableRightMargin = tableProperties.TableCellMarginDefault?.TableCellRightMargin;
    if (tableRightMargin?.Type?.Value == TableWidthValues.Dxa)
    {
      var margin = tableRightMargin.GetValue();
      if (margin != null)
      {
        totalWidth -= (ulong)margin;
      }
    }

    if (totalWidth <= widthLimit)
      return false;

    var ratio = (double)widthLimit / totalWidth;
    totalWidth = (ulong)(ratio* totalWidth);
    tableProperties.TableWidth = new TableWidth{ Width = totalWidth.ToString(), Type = TableWidthUnitValues.Dxa};
    foreach (var column in gridColumns)
    {
      var width = column.GetWidth();
      if (width != null)
        column.SetWidth((int)((int)width * ratio));
    }
    return true;
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
    return null;
  }
}