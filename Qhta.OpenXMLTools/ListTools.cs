using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace Qhta.OpenXMLTools;

public static class ListTools
{
  public static int? GetNumberingId(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(wordDocument, paragraph);
    if (numPr != null)
    {
      var val = numPr.NumberingId?.Val.Value;
      if (val != null)
        return (int)val;
    }
    return null;
  }

  public static int? GetAbstractNumberingId(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(wordDocument, paragraph);
    if (numPr != null)
    {
      var val = numPr.NumberingId?.Val.Value;
      if (val != null)
      {
        int numId = (int)val;
        var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>().FirstOrDefault(item => item.NumberID == numId);
        if (numbering != null)
        {
          val = numbering.AbstractNumId?.Val?.Value;
          if (val != null)
            return (int)val;
        }
      }
    }
    return null;
  }

  public static int? GetAbstractNumberingId(this WordprocessingDocument wordDocument, int numId)
  {
    var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>().FirstOrDefault(item => item.NumberID == numId);
    if (numbering != null)
    {
      var val = numbering.AbstractNumId?.Val?.Value;
      if (val != null)
        return (int)val;
    }
    return null;
  }

  public static int GetNumberingLevel(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(wordDocument, paragraph);
    if (numPr != null)
    {
      var val = GetNumberingLevel(wordDocument, numPr);
      return val;
    }
    return 0;
  }

  public static NumberingProperties GetNumberingProperties(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = paragraph?.ParagraphProperties?.NumberingProperties;
    if (numPr != null)
    {
      return numPr;
    }
    else
    {
      var styleId = paragraph?.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
      if (styleId != null)
      {
        var styleDef = wordDocument.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>().FirstOrDefault(item => item.StyleId == styleId);
        if (styleDef != null)
        {
          numPr = styleDef.StyleParagraphProperties?.NumberingProperties;
          if (numPr != null)
          {
            return numPr;
          }
        }
      }
    }
    return null;
  }

  public static int GetNumberingLevel(this WordprocessingDocument wordDocument, NumberingProperties numPr)
  {
    int? numLevel = numPr.NumberingLevelReference?.Val?.Value;
    if (numLevel != null)
      return (int)numLevel + 1;
    int? numId = numPr.NumberingId?.Val?.Value;
    if (numId != null)
    {
      var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>().FirstOrDefault(item => item.NumberID == numId);
      if (numbering != null)
      {
        var abstractNumId = numbering.AbstractNumId?.Val?.Value;
        AbstractNum abstractNumbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<AbstractNum>().FirstOrDefault(item => item.AbstractNumberId == abstractNumId);
        if (abstractNumbering != null)
        {
          numLevel = abstractNumbering.Elements<Level>().FirstOrDefault()?.LevelIndex?.Value;
          if (numLevel != null)
            return (int)numLevel + 1;
        }
      }
    }
    return 0;
  }

  public static Level GetNumberingLevelDef(this WordprocessingDocument wordDocument, int abstractNumId, int levelNdx)
  {
    Numbering numbering = wordDocument.MainDocumentPart.NumberingDefinitionsPart?.Numbering;
    if (numbering == null)
      return null;
    Level level = numbering.ChildElements.OfType<AbstractNum>().FirstOrDefault(item => item.AbstractNumberId == abstractNumId)?
      .Elements<Level>().FirstOrDefault(item => item.LevelIndex == levelNdx-1);
    return level;
  }

  public static int GetOutlineLevel(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var styleId = paragraph?.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
    if (styleId != null)
    {
      var styleDef = wordDocument.MainDocumentPart?.StyleDefinitionsPart?.Styles?.Elements<Style>().FirstOrDefault(item => item.StyleId == styleId);
      if (styleDef != null)
      {
        var styleName = styleDef.StyleName?.Val?.Value?.ToLower();
        if (styleName != null && styleName.StartsWith("heading ") && char.IsDigit(styleName.Last()))
        {
          var val = (int)(styleName.Last() - '0');
          if (val >= 1 && val <= 9)
            return val;
        }
      }
    }
    return 0;
  }

  public static NumberFormatValues GetListType(this WordprocessingDocument wordDocument, int abstractNumId)
  {
    NumberingInstance numbering = wordDocument.MainDocumentPart.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>()
      .FirstOrDefault(item => item.NumberID?.Value == abstractNumId);
    if (numbering != null)
    {
      AbstractNum abstractNum = wordDocument.MainDocumentPart.NumberingDefinitionsPart?.Numbering?.Elements<AbstractNum>()
        .FirstOrDefault(abstNum => abstNum.AbstractNumberId == abstractNumId);
      if (abstractNum != null)
      {
        var numFmtValues = abstractNum.GetFirstChild<Level>().NumberingFormat?.Val?.Value;
        if (numFmtValues != null)
          return (NumberFormatValues)numFmtValues;
      }
    }
    return NumberFormatValues.None;
  }
}