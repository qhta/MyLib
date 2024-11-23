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
  public static TableCellBorders GetTableCellBorders(this TableCellProperties cellProperties)
  {
    var borders = cellProperties.TableCellBorders;
    if (borders == null)
    {
      borders = new TableCellBorders();
      cellProperties.TableCellBorders = borders;
    }
    return borders;
  }

  /// <summary>
  /// Sets the cell borders.
  /// </summary>
  /// <param name="cellProperties"></param>
  /// <param name="borders"></param>
  public static void SetTableCellBorders(this TableCellProperties cellProperties, TableCellBorders? borders)
  {
    cellProperties.TableCellBorders = borders;
  }

  /// <summary>
  /// Sets the cell shading
  /// </summary>
  /// <param name="cellProperties"></param>
  /// <param name="color"></param>
  /// <param name="fillPattern"></param>
  /// <param name="fillColor"></param>
  public static void SetShading (this TableCellProperties cellProperties, ShadingPatternValues? fillPattern, string? color = null,  string? fillColor = null)
  {
    if (fillPattern == null)
    {
      cellProperties.Shading = null;
      return;
    }
    var shading = cellProperties.Shading;
    if (shading == null)
    {
      shading = new Shading();
      cellProperties.Shading = shading;
    }
    shading.Val = (ShadingPatternValues)fillPattern;
    shading.Color = color ?? "auto";
    shading.Fill = fillColor ?? "auto";
  }
}