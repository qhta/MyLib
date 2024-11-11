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
        .ToDictionary(c => c, c=>c.IsLong());
    bool hasLong = cellsDict.Values.Any(e => e == true);
    if (hasLong)
    {
      row.GetTableRowProperties().SetCanSplit(!value);
      foreach (var cell in cellsDict.Keys)
      {
        if (cellsDict[cell])
          cell.SetKeepWithNext(value);
      }
    }
    else
    {
      row.GetTableRowProperties().SetCanSplit(true);
      foreach (var cell in cellsDict.Keys)
      {
        cell.SetKeepWithNext(value);
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
}