using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

public static class BordersTools
{
  public static ParagraphBorders ToParagraphBorders(this List<BorderType> borderList)
  {
    var paragraphBorders = new ParagraphBorders();
    foreach (var border in borderList)
    {
     paragraphBorders.Append(border);
    }
    return paragraphBorders;
  }

  public static TableBorders ToTableBorders(this List<BorderType> borderList)
  {
    var tableBorders = new TableBorders();
    foreach (var border in borderList)
    {
      tableBorders.Append(border);
    }
    return tableBorders;
  }
}