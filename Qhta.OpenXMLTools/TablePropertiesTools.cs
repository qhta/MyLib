﻿using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table properties in OpenXml documents.
/// </summary>
public static class TablePropertiesTools
{
  /// <summary>
  /// Converts a <c>TableProperties</c> object to a <c>StyleTableProperties</c> object.
  /// </summary>
  /// <param name="tableProperties"></param>
  /// <returns></returns>
  public static StyleTableProperties ToStyleTableProperties(this TableProperties tableProperties)
  {
    var styleTableProperties = new StyleTableProperties();
    if (tableProperties.Shading != null)
      styleTableProperties.Shading = (Shading)tableProperties.Shading.CloneNode(true);
    if (tableProperties.TableBorders != null)
      styleTableProperties.TableBorders = (TableBorders)tableProperties.TableBorders.CloneNode(true);
    if (tableProperties.TableCellMarginDefault != null)
      styleTableProperties.TableCellMarginDefault = (TableCellMarginDefault)tableProperties.TableCellMarginDefault.CloneNode(true);
    if (tableProperties.TableCellSpacing != null)
      styleTableProperties.TableCellSpacing = (TableCellSpacing)tableProperties.TableCellSpacing.CloneNode(true);
    if (tableProperties.TableIndentation != null)
      styleTableProperties.TableIndentation = (TableIndentation)tableProperties.TableIndentation.CloneNode(true);
    if (tableProperties.TableJustification != null)
      styleTableProperties.TableJustification = (TableJustification)tableProperties.TableJustification.CloneNode(true);

    return styleTableProperties;
  }


  /// <summary>
  /// Get the default table cell margins of the table properties.
  /// </summary>
  /// <param name="tableProperties"></param>
  /// <returns></returns>
  public static DXW.TableCellMarginDefault GetTableCellMarginDefault(this DXW.TableProperties tableProperties)
  {
    return tableProperties.TableCellMarginDefault ??= new DXW.TableCellMarginDefault();
  }

  /// <summary>
  /// Gets the table borders. If the table properties do not contain a <c>TableBorders</c> element, a new one is created.
  /// </summary>
  /// <param name="TableProperties"></param>
  /// <returns></returns>
  public static TableBorders GetTableBorders(this TableProperties TableProperties)
  {
    var borders = TableProperties.TableBorders;
    if (borders == null)
    {
      borders = new TableBorders();
      TableProperties.TableBorders = borders;
    }
    return borders;
  }

  /// <summary>
  /// Sets the table borders.
  /// </summary>
  /// <param name="TableProperties"></param>
  /// <param name="borders"></param>
  public static void SetTableBorders(this TableProperties TableProperties, TableBorders? borders)
  {
    TableProperties.TableBorders = borders;
  }
}
