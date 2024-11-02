using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Extension methods for the numbering list.
/// </summary>
public static class ListTools
{
  /// <summary>
  /// Gets the numbering id of the paragraph.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static int? GetNumberingId(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(wordDocument, paragraph);
    if (numPr != null)
    {
      var val = numPr.NumberingId?.Val?.Value;
      if (val != null)
        return (int)val;
    }
    return null;
  }

  /// <summary>
  /// Gets the abstract numbering id of the paragraph.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static int? GetAbstractNumberingId(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(wordDocument, paragraph);
    if (numPr != null)
    {
      var val = numPr.NumberingId?.Val?.Value;
      if (val != null)
      {
        int numId = (int)val;
        var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>()
          .FirstOrDefault(item => item.NumberID?.Value == numId);
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

  /// <summary>
  /// Gets the abstract numbering id of the numbering id.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="numId"></param>
  /// <returns></returns>
  public static int? GetAbstractNumberingId(this WordprocessingDocument wordDocument, int numId)
  {
    var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>()
      .FirstOrDefault(item => item.NumberID?.Value == numId);
    if (numbering != null)
    {
      var val = numbering.AbstractNumId?.Val?.Value;
      if (val != null)
        return (int)val;
    }
    return null;
  }

  /// <summary>
  /// Gets the numbering level of the paragraph.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="paragraph"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Gets the numbering level of the paragraph.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static NumberingProperties? GetNumberingProperties(this WordprocessingDocument wordDocument, Paragraph paragraph)
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
        var styleDef = wordDocument.MainDocumentPart?.StyleDefinitionsPart?.Styles?.Elements<Style>()
          .FirstOrDefault(item => item.StyleId == styleId);
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

  /// <summary>
  /// Gets the numbering level of the numbering properties.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="numPr"></param>
  /// <returns></returns>
  public static int GetNumberingLevel(this WordprocessingDocument wordDocument, NumberingProperties numPr)
  {
    int? numLevel = numPr.NumberingLevelReference?.Val?.Value;
    if (numLevel != null)
      return (int)numLevel + 1;
    int? numId = numPr.NumberingId?.Val?.Value;
    if (numId != null)
    {
      var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>()
        .FirstOrDefault(item => item.NumberID?.Value == numId);
      if (numbering != null)
      {
        var abstractNumId = numbering.AbstractNumId?.Val?.Value;
        var abstractNumbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<AbstractNum>()
          .FirstOrDefault(item => item.AbstractNumberId?.Value == abstractNumId);
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

  /// <summary>
  /// Gets the numbering level definition.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="abstractNumId"></param>
  /// <param name="levelNdx"></param>
  /// <returns></returns>
  public static Level? GetNumberingLevelDef(this WordprocessingDocument wordDocument, int abstractNumId, int levelNdx)
  {
    var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering;
    if (numbering == null)
      return null;
    var level = numbering.ChildElements.OfType<AbstractNum>()
      .FirstOrDefault(item => item.AbstractNumberId?.Value == abstractNumId)?
      .Elements<Level>().FirstOrDefault(item => item.LevelIndex?.Value == levelNdx-1);
    return level;
  }

  /// <summary>
  /// Gets the outline level of the paragraph.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static int GetOutlineLevel(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var styleId = paragraph?.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
    if (styleId != null)
    {
      var styleDef = wordDocument.MainDocumentPart?.StyleDefinitionsPart?.Styles?.Elements<Style>()
        .FirstOrDefault(item => item.StyleId == styleId);
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

  /// <summary>
  /// Gets the list type of the abstract numbering id.
  /// </summary>
  /// <param name="wordDocument"></param>
  /// <param name="abstractNumId"></param>
  /// <returns></returns>
  public static NumberFormatValues GetListType(this WordprocessingDocument wordDocument, int abstractNumId)
  {
    var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>()
      .FirstOrDefault(item => item.NumberID?.Value == abstractNumId);
    if (numbering != null)
    {
      var abstractNum = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<AbstractNum>()
        .FirstOrDefault(abstNum => abstNum.AbstractNumberId?.Value == abstractNumId);
      if (abstractNum != null)
      {
        var numFmtValues = abstractNum.GetFirstChild<Level>()?.NumberingFormat?.Val?.Value;
        if (numFmtValues != null)
          return (NumberFormatValues)numFmtValues;
      }
    }
    return NumberFormatValues.None;
  }
}