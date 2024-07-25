using System;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Numbering elements.
/// </summary>
public static class NumberingTools
{
  /// <summary>
  /// Get the numbering instance id of the paragraph.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the paragraph belongs.</param>
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Integer numbering instance id or null</returns>
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
  /// Get the abstract numbering definition id of the paragraph.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the paragraph belongs.</param>
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Integer abstract numbering id or null</returns>
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
  /// Get the abstract numbering definition id of the numbering instance id.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the numbering instance belongs.</param>
  /// <param name="numId">Numbering instance id</param>
  /// <returns>Integer abstract numbering id or null</returns>
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
  /// Get the numbering level of the paragraph.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the paragraph belongs.</param>
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Integer numbering level (from 0 to 9) or null</returns>
  public static int? GetNumberingLevel(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(wordDocument, paragraph);
    if (numPr != null)
    {
      var val = GetNumberingLevel(wordDocument, numPr);
      return val;
    }
    return null;
  }

  /// <summary>
  /// Get the numbering properties of the paragraph or of the paragraph style.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the paragraph belongs.</param>
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Numbering properties or null</returns>
  public static NumberingProperties? GetNumberingProperties(this WordprocessingDocument wordDocument, Paragraph paragraph)
  {
    var numPr = paragraph?.ParagraphProperties?.NumberingProperties;
    if (numPr != null)
      return numPr;

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
    return null;
  }

  /// <summary>
  /// Get the numbering level of the numbering properties.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the numbering properties element belongs.</param>
  /// <param name="numPr">Numbering properties element</param>
  /// <returns>Integer numbering level (from 0 to 8) or null</returns>
  public static int? GetNumberingLevel(this WordprocessingDocument wordDocument, NumberingProperties numPr)
  {
    int? numLevel = numPr.NumberingLevelReference?.Val?.Value;
    if (numLevel != null)
      return (int)numLevel;
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
            return (int)numLevel;
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Get the numbering level definition of the abstract numbering id and the level index.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document</param>
  /// <param name="abstractNumId">Abstract numbering definition id</param>
  /// <param name="levelNdx">level index</param>
  /// <returns>Level definition or null</returns>
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
  ///  Get the outline level of the paragraph.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the paragraph belongs.</param>
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Integer outline level (from 0 to 8) or null</returns>
  public static int? GetOutlineLevel(this WordprocessingDocument wordDocument, Paragraph paragraph)
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
          if (val is >= 0 and <= 8)
            return val;
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Get the list type of the abstract numbering id.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the abstract numbering belongs.</param>
  /// <param name="abstractNumId">id of the abstract numbering</param>
  /// <returns><see cref="NumberFormatValues"/> enum value or null</returns>
  public static NumberFormatValues? GetListType(this WordprocessingDocument wordDocument, int abstractNumId)
  {
    var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>()
      .FirstOrDefault(item => item.NumberID?.Value == abstractNumId);
    if (numbering != null)
    {
      var abstractNum = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<AbstractNum>()
        .FirstOrDefault(aNum => aNum.AbstractNumberId?.Value == abstractNumId);
      if (abstractNum != null)
      {
        var numFmtValues = abstractNum.GetFirstChild<Level>()?.NumberingFormat?.Val?.Value;
        if (numFmtValues != null)
          return (NumberFormatValues)numFmtValues;
      }
    }
    return null;
  }

  /// <summary>
  /// Get the list text of the abstract numbering id and the level index.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the abstract numbering belongs.</param>
  /// <param name="abstractNumId">id of the abstract numbering</param>
  /// <param name="levelNdx">level index</param>
  /// <returns>Level text or null</returns>
  public static string? GetListText(this WordprocessingDocument wordDocument, int abstractNumId, int levelNdx)
  {
    var level = GetNumberingLevelDef(wordDocument, abstractNumId, levelNdx);
    if (level != null)
    {
      var levelText = level.LevelText?.Val?.Value;
      if (levelText != null)
        return levelText;
    }
    return null;
  }

  /// <summary>
  /// Create a new hybrid multilevel abstract numbering definition from an array a single level abstract numbering definition.
  /// </summary>
  /// <param name="levels">Array of source levels. Should be single element elements</param>
  /// <returns>Joined hybrid multilevel abstract numbering definition element containing source elements</returns>
  /// <remarks>
  /// Subsequent levels are additionally indented by the left indent of the previous level.
  /// </remarks>
  public static AbstractNum JoinSingleLevelsHybridMultiLevel(Level[] levels)
  {
    var abstractNum = new AbstractNum { MultiLevelType = new MultiLevelType{ Val = MultiLevelValues.HybridMultilevel }};
    var indent0 = 0;
    for (int lc = 0; lc < levels.Length; lc++)
    {
      var level = (Level)levels[lc].CloneNode(true);
      level.LevelIndex = lc;
      var paraProps = level.GetFirstChild<ParagraphProperties>();
      if (paraProps != null)
      {
        var ind = paraProps.GetFirstChild<Indentation>();
        if (ind != null)
        {
          if (ind.Left?.Value != null)
          {
            var indent = Int32.Parse(ind.Left.Value);
            var indent1 = indent + indent0;
            ind.Left = indent1.ToString();
            indent0 = indent1;
          }
        }
      }
      abstractNum.Append(level);
    }
    return abstractNum;
  }
}