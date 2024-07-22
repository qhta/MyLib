using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

public static class TablePropertiesTools
{
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
}
