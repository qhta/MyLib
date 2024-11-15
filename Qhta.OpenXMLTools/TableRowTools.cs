namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table rows in OpenXml documents.
/// </summary>
public static class TableRowTools
{
  /// <summary>
  /// Sets the keep with next property for all cells in the row.
  /// </summary>
  /// <param name="row">Table row to set</param>
  /// <param name="value">value to set</param>
  public static void SetKeepWithNext(this DXW.TableRow row, bool value)
  {
    Dictionary<DXW.TableCell, bool> cellsDict =
      row.MemberElements().OfType<DXW.TableCell>()
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
      var gridSpan = cell.GetGridSpan();
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
      var gridSpan = cell.GetGridSpan();
      if (gridSpan > 1)
      {
        for (int i = 1; i <= gridSpan - 1; i++)
        {
          cells.Insert(cellIndex + 1, cell);
        }
      }
    }
    return cells[columnIndex];
  }

  /// <summary>
  /// Join the cell in a row with the next cell.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="columnIndex"></param>
  /// <returns></returns>
  public static bool JoinCellWithNext(this DXW.TableRow row, int columnIndex)
  {
    var cell1 = row.GetCell(columnIndex);
    var cell2 = row.GetCell(columnIndex + 1);
    if ((cell1 == null || cell1.IsEmpty()) && cell2 != null)
    {
      if (cell1 != null)
      {
        cell2.SetGridSpan(1);
        cell2.SetWidth(cell1.GetWidth() + cell2.GetWidth());
        cell2.SetLeftBorder(cell1.GetLeftBorder());
        cell1.Remove();
      }
      return true;
    }
    else if ((cell2 == null || cell2.IsEmpty()) && cell1 != null)
    {
      if (cell2 != null)
      {
        cell1.SetWidth(cell1.GetWidth() + cell2.GetWidth());
        cell1.SetRightBorder(cell2.GetRightBorder());
        cell2.Remove();
      }
      else
        cell1.SetGridSpan(1);
      return true;
    }
    return false;
  }
}