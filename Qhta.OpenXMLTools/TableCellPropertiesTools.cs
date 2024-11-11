using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table cell properties in OpenXml documents.
/// </summary>
public static class TableCellPropertiesTools
{
  /// <summary>
  /// Gets the cell borders. If the cell properties do not contain a <c>TableCellBorders</c> element, a new one is created.
  /// </summary>
  /// <param name="cellProperties"></param>
  /// <returns></returns>
  public static TableCellBorders GetBorders(this TableCellProperties cellProperties)
  {
    var borders = cellProperties.TableCellBorders;
    if (borders == null)
    {
      borders = new TableCellBorders();
      cellProperties.TableCellBorders = borders;
    }
    return borders;
  }

}