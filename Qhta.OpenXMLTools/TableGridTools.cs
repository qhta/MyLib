namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table grid columns in OpenXml documents.
/// </summary>
public static class TableGridTools
{
  /// <summary>
  /// Gets columns from the table grid.
  /// </summary>
  /// <param name="tableGrid"></param>
  /// <returns></returns>
  public static IEnumerable<DXW.GridColumn> GetColumns(this DXW.TableGrid tableGrid)
  {
    return tableGrid.ChildElements.OfType<DXW.GridColumn>();
  }
}