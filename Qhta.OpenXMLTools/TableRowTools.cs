using System;

using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table rows in OpenXml documents.
/// </summary>
public static class TableRowTools
{
  /// <summary>
  /// Get the <c>TableCell</c> elements of the row.
  /// </summary>
  /// <param name="row"></param>
  /// <returns></returns>
  public static IEnumerable<DXW.TableCell> GetCells(this DXW.TableRow row)
  {
    return row.Elements<DXW.TableCell>();
  }


  /// <summary>
  /// Gets the row height. If the row properties do not contain a <c>TableRowHeight</c> element, null is returned.
  /// </summary>
  /// <param name="tableRow">Table row to examine</param>
  /// <returns></returns>
  public static TableRowHeight? GetHeight(this TableRow tableRow)
  {
    var tableRowProperties = tableRow.TableRowProperties;
    var rowHeight = tableRowProperties?.Elements<TableRowHeight>().FirstOrDefault();
    return rowHeight;
  }

  /// <summary>
  /// Sets the row height.
  /// If value to set is null, the height is removed.
  /// </summary>
  /// <param name="tableRow">Table row to set</param>
  /// <param name="value">value to set</param>
  /// <param name="rule">rule to set</param>
  /// <returns></returns>
  public static void SetHeight(this TableRow tableRow, int? value, DXW.HeightRuleValues? rule = null)
  {
    var tableRowProperties = tableRow.GetTableRowProperties();
    tableRowProperties.SetTableRowHeight(value, rule);
  }

  /// <summary>
  /// Sets the keep with next property for all cells in the row.
  /// </summary>
  /// <param name="row">Table row to set</param>
  /// <param name="value">value to set</param>
  public static void SetKeepWithNext(this DXW.TableRow row, bool value)
  {
    Dictionary<DXW.TableCell, bool> cellsDict =
      row.GetMembers().OfType<DXW.TableCell>()
        .ToDictionary(c => c, c => c.IsLong());
    bool hasLong = cellsDict.Values.Any(e => e == true);
    if (hasLong)
    {
      row.GetTableRowProperties().SetCantSplit(false);
      foreach (var cell in cellsDict.Keys)
      {
        cell.SetKeepWithNext(false, WhichParagraphs.All);
        if (cellsDict[cell])
          cell.SetKeepWithNext(true, WhichParagraphs.First);
      }
    }
    else
    {
      row.GetTableRowProperties().SetCantSplit(true);
      foreach (var cell in cellsDict.Keys)
      {
        cell.SetKeepWithNext(value, WhichParagraphs.All);
      }
    }
  }

  /// <summary>
  /// Get the <c>TableRowProperties</c> element of the row. If it does not exist, it will be created.
  /// </summary>
  /// <param name="row">Table row to examine</param>
  /// <returns></returns>
  public static DXW.TableRowProperties GetTableRowProperties(this DXW.TableRow row)
  {
    var rowProperties = row.TableRowProperties;
    if (rowProperties == null)
    {
      rowProperties = new DXW.TableRowProperties();
      row.AddChild(rowProperties);
    }
    return rowProperties;
  }

  /// <summary>
  /// Checks if the paragraph is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.TableRow? element)
  {
    if (element == null)
      return true;
    foreach (var cell in element.Elements<DXW.TableCell>())
    {
      if (!cell.IsEmpty())
        return false;
    }
    return true;
  }

  /// <summary>
  /// Get row cell by column index. If the cell in this columnIndex is merged, the null is returned.
  /// Also, if the column index is out of range, the null is returned.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="columnIndex"></param>
  /// <returns></returns>
  public static DXW.TableCell? GetCell(this DXW.TableRow row, int columnIndex)
  {
    var cells = row.Elements<DXW.TableCell>().ToList();
    for (int cellIndex = 0; cellIndex <= cells.Count - 1; cellIndex++)
    {
      var cell = cells[cellIndex];
      var gridSpan = cell.GetSpan();
      if (gridSpan > 1)
      {
        for (int i = 1; i <= gridSpan - 1; i++)
        {
          cells.Insert(cellIndex + 1, null!);
        }
        cellIndex += gridSpan - 1;
      }
    }
    if (columnIndex < 0 || columnIndex >= cells.Count)
      return null;
    return cells[columnIndex];
  }

  /// <summary>
  /// Get row cell by column index. If the cell in this columnIndex is merged, the first cell in merge is returned.
  /// If the column index is out of range, the null is returned.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="columnIndex"></param>
  /// <returns></returns>
  public static DXW.TableCell? GetMergedCell(this DXW.TableRow row, int columnIndex)
  {
    var cells = row.Elements<DXW.TableCell>().ToList();
    for (int cellIndex = 0; cellIndex <= cells.Count - 1; cellIndex++)
    {
      var cell = cells[cellIndex];
      var gridSpan = cell.GetSpan();
      if (gridSpan > 1)
      {
        for (int i = 1; i <= gridSpan - 1; i++)
        {
          cells.Insert(cellIndex + 1, cell);
        }
        cellIndex += gridSpan - 1;
      }
    }
    if (columnIndex < 0 || columnIndex >= cells.Count)
      return null;
    return cells[columnIndex];
  }

  /// <summary>
  /// Set the row background color.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="backgroundColor"></param>
  public static void SetBackgroundColor(this DXW.TableRow row, int backgroundColor)
  {
    foreach (var cell in row.GetCells())
    {
      cell.SetBackgroundColor(backgroundColor);
    }
  }
}