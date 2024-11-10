namespace Qhta.OpenXmlTools;

/// <summary>
/// A composite tool for cleaning a Wordprocessing document.
/// </summary>
public partial class DocumentCleaner
{

  private DXW.ParagraphProperties? GetMostFrequentNumberingParagraphProperties(DXPack.WordprocessingDocument wordDoc)
  {
    Dictionary<int, int> numberingFrequency = new();
    Dictionary<int, DXW.ParagraphProperties> numberingPropertiesIndex = new();
    foreach (var paragraph in wordDoc.GetBody().Descendants<DXW.Paragraph>())
    {
      var paragraphProperties = paragraph.ParagraphProperties;
      if (paragraphProperties != null)
      {
        var numberingProperties = paragraphProperties.NumberingProperties;
        if (numberingProperties != null)
        {
          var numberingLevel = numberingProperties.NumberingLevelReference?.Val;
          if (numberingLevel == null || numberingLevel != 0)
            continue;
          var numberingId = numberingProperties.NumberingId?.Val;
          if (numberingId == null)
            continue;
          if (numberingFrequency.ContainsKey(numberingId))
            numberingFrequency[numberingId]++;
          else
          {
            numberingPropertiesIndex[numberingId] = paragraphProperties;
            numberingFrequency[numberingId] = 1;
          }
        }
      }
    }
    if (numberingFrequency.Count == 0)
      return null;
    var mostFrequentNumberingId = numberingFrequency.OrderByDescending(kvp => kvp.Value).First().Key;
    return (DXW.ParagraphProperties)numberingPropertiesIndex[mostFrequentNumberingId].CloneNode(true);
    ;
  }

  private DXW.ParagraphProperties? GetNearbyNumberingParagraphProperties(DXW.Paragraph paragraph)
  {
    var paragraphProperties = paragraph.ParagraphProperties;
    if (paragraphProperties != null && paragraphProperties.NumberingProperties != null)
      return (DXW.ParagraphProperties)paragraphProperties.CloneNode(true);
    paragraphProperties = (paragraph.PreviousSibling() as DXW.Paragraph)?.ParagraphProperties;
    if (paragraphProperties != null && paragraphProperties.NumberingProperties != null)
      return (DXW.ParagraphProperties)paragraphProperties.CloneNode(true);
    paragraphProperties = (paragraph.NextSibling() as DXW.Paragraph)?.ParagraphProperties;
    if (paragraphProperties != null && paragraphProperties.NumberingProperties != null)
      return (DXW.ParagraphProperties)paragraphProperties.CloneNode(true);
    return null;
  }

}