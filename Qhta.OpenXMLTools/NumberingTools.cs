using System;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Numbering elements.
/// </summary>
public static class NumberingTools
{
  private static readonly Dictionary<(int docId, int numId), NumberingList> NumberingListsForDocument = new();
  private static readonly Dictionary<int, NumberingList> NumberingListsForParagraphs = new();

  /// <summary>
  /// Gets the numbering string from a string.
  /// Numbering string is the first sequence of digits, periods or letters ended with a space or tab.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string? GetNumberingString(this string str)
  {
    var text = str.Trim();
    var isNumberingString = false;
    var k = text.IndexOfAny([' ', '\t']);
    if (k > 0)
    {
      isNumberingString = true;
      text = text.Substring(0, k);
      var wasPeriod = true;
      var wasLetter = false;
      foreach (var ch in text)
      {
        var isLetter = char.IsLetter(ch);
        var isPeriod = ch == '.';
        if (isLetter)
        {
          if (wasLetter)
          {
            isNumberingString = false;
            break;
          }
        }
        else
        if (char.IsDigit(ch))
        {
          if (wasLetter)
          {
            isNumberingString = false;
            break;
          }
        }
        else if (isPeriod)
        {
          if (wasPeriod)
          {
            isNumberingString = false;
            break;
          }
        }
        else
        {
          isNumberingString = false;
          break;
        }
        wasLetter = isLetter;
        wasPeriod = isPeriod;
      }
    }
    return isNumberingString ? text : null;
  }
  /// <summary>
  /// Get the numbering string of the paragraph.
  /// If the paragraph is numbered, the string is taken from the numbering list.
  /// If the paragraph is not numbered, the string is taken from the paragraph text.
  /// First sequence of digits, periods or letters is returned.
  /// Only single periods and single letters are allowed.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string? GetNumberingString(this Paragraph paragraph, GetTextOptions? options = null)
  {
    var numberingList = paragraph.GetNumberingList();
    if (numberingList != null)
    {
      return numberingList.GetNumberingString(paragraph, options);
    }
    var text = paragraph.GetText(options);
    return GetNumberingString(text);
  }

  /// <summary>
  /// Get the numbering list of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static NumberingList? GetNumberingList(this Paragraph paragraph)
  {
    var paraId = paragraph.GetParagraphId() ?? 0;
    if (NumberingListsForParagraphs.TryGetValue(paraId, out var numberingList))
      return numberingList;
    var numberingInstance = paragraph.GetNumberingInstance();
    if (numberingInstance != null)
    {
      var docId = paragraph.GetWordprocessingDocument()?.GetDocumentId() ?? 0;
      var numId = numberingInstance.NumberID?.Value ?? 0;
      var key = (docId, numId);
      if (!NumberingListsForDocument.TryGetValue(key, out numberingList))
      {
        numberingList = new NumberingList(numberingInstance);
        NumberingListsForDocument.Add(key, numberingList);
        NumberingListsForParagraphs.Add(paraId, numberingList);
      }
      numberingList.Add(paragraph); 
      return numberingList;
    }
    return null;
  }

  /// <summary>
  /// Get the numbering instance of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static NumberingInstance? GetNumberingInstance(this Paragraph paragraph)
  {
    var numberingId = paragraph.ParagraphProperties?.NumberingProperties?.NumberingId?.Val?.Value;
    if (numberingId != null)
    {
      var wordDoc = paragraph.GetWordprocessingDocument();
      if (wordDoc != null)
      {
        var numberingPart = wordDoc.MainDocumentPart?.NumberingDefinitionsPart;
        if (numberingPart != null)
        {
          return numberingPart.Numbering?.Elements<NumberingInstance>().FirstOrDefault(n => n.NumberID?.Value == numberingId);
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Get the abstract numbering of the numbering instance
  /// </summary>
  /// <param name="numInstance"></param>
  /// <returns></returns>
  public static AbstractNum? GetAbstractNum(this NumberingInstance numInstance)
  {
    var numbering = numInstance.Ancestors<Numbering>().FirstOrDefault();
    return numbering?.Elements<AbstractNum>().FirstOrDefault(n => n.AbstractNumberId?.Value == numInstance.AbstractNumId?.Val?.Value);
  }

  /// <summary>
  /// Get the numbering instance id of the paragraph.
  /// </summary>
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Integer numbering instance id or null</returns>
  public static int? GetNumberingId(Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(paragraph);
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
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Integer abstract numbering id or null</returns>
  public static int? GetAbstractNumberingId(Paragraph paragraph)
  {
    var wordDocument = paragraph.GetWordprocessingDocument();
    if (wordDocument == null)
      return null;
    var numPr = GetNumberingProperties(paragraph);
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
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Integer numbering level (from 0 to 9) or null</returns>
  public static int? GetNumberingLevel(this Paragraph paragraph)
  {
    var numPr = GetNumberingProperties(paragraph);
    if (numPr != null)
    {
      var val = GetNumberingLevel(numPr);
      return val;
    }
    return null;
  }

  /// <summary>
  /// Get the numbering properties of the paragraph or of the paragraph style.
  /// </summary>
  /// <param name="paragraph">Source paragraph element</param>
  /// <returns>Numbering properties or null</returns>
  public static NumberingProperties? GetNumberingProperties(this DXW.Paragraph paragraph)
  {
    var numPr = paragraph?.ParagraphProperties?.NumberingProperties;
    if (numPr != null)
      return numPr;

    var styleId = paragraph?.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
    if (styleId != null)
    {
      var wordDocument = paragraph.GetWordprocessingDocument();
      if (wordDocument == null)
        return null;
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
  /// Get the numbering properties of the paragraph or of the paragraph style.
  /// </summary>
  /// <param name="paragraph">Source paragraph element</param>
  /// <param name="numPr">Numbering properties or null</param>
  public static void SetNumberingProperties(this DXW.Paragraph paragraph, NumberingProperties numPr)
  {
    var paraProps = paragraph.GetParagraphProperties();
    paraProps.NumberingProperties = numPr;
  }

  /// <summary>
  /// Get the numbering level of the numbering properties.
  /// </summary>
  /// <param name="numPr">Numbering properties element</param>
  /// <returns>Integer numbering level (from 0 to 8) or null</returns>
  public static int? GetNumberingLevel(this DXW.NumberingProperties numPr)
  {
    int? numLevel = numPr.NumberingLevelReference?.Val?.Value;
    if (numLevel != null)
      return (int)numLevel;
    int? numId = numPr.NumberingId?.Val?.Value;
    if (numId != null)
    {
      var wordDocument = numPr.GetWordprocessingDocument();
      if (wordDocument == null)
        return null; var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?.Elements<NumberingInstance>()
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
      .Elements<Level>().FirstOrDefault(item => item.LevelIndex?.Value == levelNdx - 1);
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
    var abstractNum = new AbstractNum { MultiLevelType = new MultiLevelType { Val = MultiLevelValues.HybridMultilevel } };
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

  ///// <summary>
  ///// Get the line numbering restart value.
  ///// </summary>
  ///// <param name="lineNumberType"></param>
  ///// <returns></returns>
  //public static LineNumberRestartValues? GetLineNumberRestart(this LineNumberType lineNumberType)
  //{
  //  return lineNumberType.Restart?.Value;
  //}

  ///// <summary>
  ///// Set <c>TextDirection</c> value in the section properties.
  ///// </summary>
  ///// <param name="lineNumberType"></param>
  ///// <param name="value"></param>
  //public static void SetLineNumberRestart(this LineNumberType lineNumberType, LineNumberRestartValues? value)
  //{
  //  lineNumberType.Restart = (value is not null) ? new DX.EnumValue<LineNumberRestartValues>(value) : null;
  //}
}