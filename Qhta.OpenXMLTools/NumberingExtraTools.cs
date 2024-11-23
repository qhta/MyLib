using System;
using System.Xml.Linq;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Numbering elements.
/// </summary>
public static class NumberingExtraTools
{
  private static readonly Dictionary<(int docId, int numId), NumberingList> NumberingListsForDocument = new();
  private static readonly Dictionary<int, NumberingList> NumberingListsForParagraphs = new();

  /// <summary>
  /// Gets the numbering string from a string.
  /// Numbering string is the first sequence of digits, periods or letters followed with a space or tab.
  /// Numbering string may be ended with a closing parenthesis.
  /// Numbering string should be valid.
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
        else if (ch == ')')
        {
          isNumberingString = true;
          break;
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
    if (isNumberingString)
      if (!IsValidNumbering(text))
        isNumberingString = false;
    return isNumberingString ? text : null;
  }

  /// <summary>
  /// Determines if the string is a valid numbering string.
  /// String is trimmed and final closing parenthesis (if exists) is removed before checking.
  /// First string is divided to parts by periods.
  /// Then each part is checked if it is a valid numbering part.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  public static bool IsValidNumbering(string text)
  {
    text = text.Trim();
    if (text.EndsWith(")"))
      text = text.Substring(0, text.Length - 1);
    else if (text.EndsWith("."))
      text = text.Substring(0, text.Length - 1);
    if (text.Length == 0)
      return false;
    var segments = text.Split('.');
    foreach (var part in segments)
    {
      if (!IsValidNumberingSegment(part))
        return false;
    }
    return true;
  }

  /// <summary>
  /// Determines if the string is a valid numbering segment.
  /// To be valid, the segment should contain only digits, letters.
  /// It may be a single letter or a valid roman number sequence of digits.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  public static bool IsValidNumberingSegment(string text)
  {
    var firstChar = text.FirstOrDefault();
    if (char.IsDigit(firstChar))
    {
      foreach (var ch in text)
      {
        if (!char.IsDigit(ch))
          return false;
      }
      return true;
    }
    else if (char.IsLetter(firstChar))
    {
      foreach (var ch in text)
      {
        if (!char.IsLetter(ch))
          return false;
      }
      if (text.Length == 1)
        return true;
      text = text.ToUpper();
      foreach (var ch in text)
        if (!("IVXLCM".Contains(ch)))
          return false;
      if (RomanNumeralConverter.FromRoman(text, 0) == null)
        return false;
      return true;
    }
    return false;
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
  public static string? GetNumberingString(this Paragraph paragraph, TextOptions? options = null)
  {
    if (options == null)
      options = TextOptions.PlainText;
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
  public static int? GetNumberingId(this Paragraph paragraph)
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
  public static int? GetAbstractNumberingId(this Paragraph paragraph)
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
  /// Get the numbering element from the word document's numbering definition part.
  /// If the numbering definition part is not found, it is created.
  /// </summary>
  /// <param name="mainDocumentPart">Main part of the Wordprocessing document.</param>
  public static DXW.Numbering GetNumberingDefinitions(this MainDocumentPart mainDocumentPart)
  {
    var numberingDefinitionsPart = mainDocumentPart.NumberingDefinitionsPart;
    if (numberingDefinitionsPart == null)
    {
      numberingDefinitionsPart = mainDocumentPart.AddNewPart<DXPack.NumberingDefinitionsPart>();
    }
    return numberingDefinitionsPart.Numbering;
  }

  /// <summary>
  /// Get numbering instance which is assigned to abstract numbering level with bullet.
  /// If there is no such numbering instance, it is created.
  /// </summary>
  /// <param name="mainDocumentPart"></param>
  /// <param name="bullet">symbol to compare with numbering text</param>
  /// <param name="level">minimal level that is checked</param>
  /// <returns></returns>
  public static DXW.NumberingInstance? GetBulletedNumbering(this MainDocumentPart mainDocumentPart, char bullet, int level)
  {
    var numbering = mainDocumentPart.GetNumberingDefinitions();
    var abstractNums = numbering.Elements<DXW.AbstractNum>()
      .Where(absNum => absNum.Elements<Level>().Any(lvl => lvl.GetLevelIndex() >= level)).ToList();
    foreach (var absNum in abstractNums)
    {
      var absNumId = absNum.AbstractNumberId?.Value;
      var lvl = absNum.Elements<Level>().FirstOrDefault(lvl => lvl.GetLevelIndex() >= level);
      var numIns = numbering.Elements<NumberingInstance>().FirstOrDefault(numIns => numIns.AbstractNumId?.Val?.Value == absNumId);
      if (numIns != null)
        return numIns;
    }
    var firstAbsNum = abstractNums.FirstOrDefault();
    if (firstAbsNum != null)
    {
      var absNumId = firstAbsNum.AbstractNumberId?.Value;
      var numIns = numbering.Elements<NumberingInstance>().FirstOrDefault(numIns => numIns.AbstractNumId?.Val?.Value == absNumId);
      if (numIns != null)
        return numIns;
    }
    return null;
  }

  /// <summary>
  /// Get the abstract numbering definition id of the numbering instance id.
  /// </summary>
  /// <param name="wordDocument">Source Wordprocessing document to which the numbering instance belongs.</param>
  /// <param name="numId">Numbering instance id</param>
  /// <returns>Integer abstract numbering id or null</returns>
  public static int? GetAbstractNumId(this WordprocessingDocument wordDocument, int numId)
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
  /// Get the abstract numbering definition with an id.
  /// </summary>
  /// <param name="numbering">Numbering element to which the abstract numbering belongs.</param>
  /// <param name="numId">Numbering instance id</param>
  /// <returns>Integer abstract numbering id or null</returns>
  public static DXW.AbstractNum? GetAbstractNum(this Numbering numbering, int numId)
  {
    return numbering.Elements<AbstractNum>().FirstOrDefault(e => e.AbstractNumberId?.Value == numId);
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
  /// Set the numbering of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="abstractNum"></param>
  /// <param name="level"></param>
  public static void SetNumbering(this DXW.Paragraph paragraph, DXW.AbstractNum abstractNum, DXW.Level level)
  {
    var paraProps = paragraph.GetParagraphProperties();
    var numbering = paragraph.GetMainDocumentPart()?.GetNumberingDefinitions();
    if (numbering == null)
      return;
    var previousNumberingParagraph = paragraph.GetPreviousNumberedParagraph();
    var previousAbstractNumId = previousNumberingParagraph?.GetAbstractNumberingId();
    if (previousAbstractNumId == abstractNum.AbstractNumberId?.Value)
    {
      var previousNumberingProperties = previousNumberingParagraph?.GetNumberingProperties();
      var newNumberingProperties = (DXW.NumberingProperties)previousNumberingProperties!.CloneNode(true);
      newNumberingProperties.NumberingLevelReference = new NumberingLevelReference { Val = level.LevelIndex };
      paraProps.NumberingProperties = newNumberingProperties;
    }
    else
    {
      var newNumberingProperties = paraProps.NumberingProperties;
      if (newNumberingProperties == null)
      {
        newNumberingProperties = new NumberingProperties();
        var numberingInstance = numbering.GetNumberingInstance(abstractNum.AbstractNumberId!);
        newNumberingProperties.SetNumberingId(numberingInstance.NumberID!.Value);
      }
      newNumberingProperties.NumberingLevelReference = new NumberingLevelReference { Val = level.GetLevelIndex() };
      paraProps.NumberingProperties = newNumberingProperties;
    }

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
      .Elements<Level>().FirstOrDefault(item => item.GetLevelIndex() == levelNdx - 1);
    return level;
  }

  /// <summary>
  /// Get the numbering level definition of the level definition.
  /// </summary>
  /// <param name="level"></param>
  /// <returns></returns>
  public static int? GetLevelIndex(this DXW.Level level)
  {
    return level.LevelIndex?.Value;
  }

  /// <summary>
  /// Set the numbering level definition of the level definition.
  /// </summary>
  /// <param name="level"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetLevelIndex(this DXW.Level level, int? value)
  {
    level.LevelIndex = value;
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

  /// <summary>
  /// Check if the abstract numbering definition is bulleted.
  /// </summary>
  /// <param name="abstractNum"></param>
  /// <returns></returns>
  public static bool IsBulleted(this AbstractNum abstractNum)
  {
    if (abstractNum.MultiLevelType?.Val?.Value == DXW.MultiLevelValues.SingleLevel)
    {
      var level = abstractNum.Elements<Level>().FirstOrDefault();
      return level != null && level.IsBulleted();
    }
    if (abstractNum.MultiLevelType?.Val?.Value == DXW.MultiLevelValues.Multilevel)
    {
      var level = abstractNum.Elements<Level>().FirstOrDefault();
      return level != null && level.IsBulleted();
    }
    if (abstractNum.MultiLevelType?.Val?.Value == DXW.MultiLevelValues.HybridMultilevel)
    {
      foreach (var level in abstractNum.Elements<Level>())
      {
        if (!level.IsBulleted())
        {
          return false;
        }
      }
      return true;
    }
    return false;
  }

  /// <summary>
  /// Get statistics of abstract numbering used in paragraphs. If the run does not contain font information, return default properties statistic.
  /// </summary>
  /// <param name="element">composite element to examine</param>
  public static ObjectStatistics<DXW.AbstractNum> GetNumberingStatistics(this DX.OpenXmlCompositeElement element)
  {
    ObjectStatistics<AbstractNum> statistics = new ObjectStatistics<AbstractNum>();
    var numbering = element.GetMainDocumentPart()?.NumberingDefinitionsPart?.Numbering;
    if (numbering == null)
      return statistics;
    var paragraphs = element.Descendants<DXW.Paragraph>().ToList();
    foreach (var paragraph in paragraphs)
    {
      var numberingProperties = paragraph.GetNumberingProperties();
      if (numberingProperties == null)
        continue;
      var abstractNumId = numberingProperties.NumberingId?.Val;
      if (abstractNumId == null)
        continue;
      var abstractNum = numbering.GetAbstractNum(abstractNumId);
      if (abstractNum == null)
        continue;
      statistics.Add(abstractNum);
    }
    return statistics;
  }

  /// <summary>
  /// Find and return the most frequent bulleted abstract numbering.
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  public static DXW.AbstractNum? FindMostFrequentBulletedAbstractNumbering(this DX.OpenXmlCompositeElement body)
  {
    var numberingStatistics = body.GetNumberingStatistics();
    var nonBulletedNumberings = numberingStatistics.Keys.Where(a => !a.IsBulleted()).ToList();
    foreach (var abstractNum in nonBulletedNumberings)
    {
      numberingStatistics.Remove(abstractNum);
    }
    var mostFrequentBulletedNumbering = numberingStatistics.MostFrequent();
    return mostFrequentBulletedNumbering;
  }

  /// <summary>
  /// Get statistics of numbering instances used in paragraphs. If the run does not contain font information, return default properties statistic.
  /// </summary>
  /// <param name="element">composite element to examine</param>
  /// <param name="abstractNumId">optional abstract numbering Id to filter statistics</param>
  public static ObjectStatistics<DXW.NumberingInstance> GetNumberingInstanceStatistics
    (this DX.OpenXmlCompositeElement element, int? abstractNumId = null)
  {
    ObjectStatistics<NumberingInstance> statistics = new ObjectStatistics<NumberingInstance>();
    var numbering = element.GetMainDocumentPart()?.NumberingDefinitionsPart?.Numbering;
    if (numbering == null)
      return statistics;
    var paragraphs = element.Descendants<DXW.Paragraph>().ToList();
    foreach (var paragraph in paragraphs)
    {
      var numberingInstance = paragraph.GetNumberingInstance();
      if (numberingInstance != null)
      {
        if (abstractNumId != null && numberingInstance.AbstractNumId?.Val?.Value != abstractNumId)
          continue;
        statistics.Add(numberingInstance);
      }
    }
    return statistics;
  }


  /// <summary>
  /// Check if the paragraph is numbered.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static bool IsNumbered(this DXW.Paragraph paragraph)
  {
    return paragraph.ParagraphProperties?.NumberingProperties != null;
  }

  /// <summary>
  /// Find the previous paragraph that is numbered.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static DXW.Paragraph? GetPreviousNumberedParagraph(this DXW.Paragraph paragraph)
  {
    DX.OpenXmlElement? element = paragraph;
    while (element != null)
    {
      element = element.PreviousSibling();
      if (element is DXW.Paragraph para)
      {
        if (para.IsNumbered())
        {
          return para;
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Find the next paragraph that is numbered.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static DXW.Paragraph? GetNextNumberedParagraph(this DXW.Paragraph paragraph)
  {
    DX.OpenXmlElement? element = paragraph;
    while (element != null)
    {
      element = element.NextSibling();
      if (element is DXW.Paragraph para)
      {
        if (para.IsNumbered())
        {
          return para;
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Check if the paragraph is bulleted.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static bool IsBulleted(this DXW.Paragraph paragraph)
  {
    var numberingProperties = paragraph.ParagraphProperties?.NumberingProperties;
    if (numberingProperties == null)
      return false;
    var numId = numberingProperties.NumberingId?.Val?.Value;
    if (numId == null)
      return false;
    var wordDocument = paragraph.GetWordprocessingDocument();
    if (wordDocument == null)
      return false;
    var numbering = wordDocument.MainDocumentPart?.NumberingDefinitionsPart?.Numbering;
    if (numbering == null)
      return false;
    var numberingInstance = numbering.Elements<NumberingInstance>().FirstOrDefault(n => n.NumberID?.Value == numId);
    if (numberingInstance == null)
      return false;
    var abstractNumId = numberingInstance.AbstractNumId?.Val?.Value;
    if (abstractNumId == null)
      return false;
    var abstractNum = numbering.Elements<AbstractNum>().FirstOrDefault(a => a.AbstractNumberId?.Value == abstractNumId);
    if (abstractNum == null)
      return false;
    return abstractNum.IsBulleted();
  }

  /// <summary>
  /// Find the previous paragraph that is bulleted.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static DXW.Paragraph? GetPreviousBulletedParagraph(this DXW.Paragraph paragraph)
  {
    DX.OpenXmlElement? element = paragraph;
    while (element != null)
    {
      element = element.PreviousSibling();
      if (element is DXW.Paragraph para)
      {
        if (para.IsBulleted())
        {
          return para;
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Find the next paragraph that is numbered.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static DXW.Paragraph? GetNextBulletedParagraph(this DXW.Paragraph paragraph)
  {
    DX.OpenXmlElement? element = paragraph;
    while (element != null)
    {
      element = element.NextSibling();
      if (element is DXW.Paragraph para)
      {
        if (para.IsBulleted())
        {
          return para;
        }
      }
    }
    return null;
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