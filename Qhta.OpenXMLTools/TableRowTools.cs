namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table rows in OpenXml documents.
/// </summary>
public static class TableRowTools
{
  /// <summary>
  /// Sets the keep with next property for all cells in the row.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="value"></param>
  public static void SetKeepWithNext(this DXW.TableRow row, bool value)
  {
    foreach (var cell in row.Elements<DXW.TableCell>())
    {
      cell.SetKeepWithNext(value);
    }
  }
}