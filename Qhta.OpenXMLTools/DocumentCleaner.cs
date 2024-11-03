using System;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for cleaning OpenXml document.
/// </summary>
public class DocumentCleaner
{
  /// <summary>
  /// Cleans the document using all the methods available.
  /// </summary>
  /// <param name="fileName"></param>
  public void CleanDocument(string fileName)
  {
    using var wordDoc = DXPack.WordprocessingDocument.Open(fileName, true);
    TrimParagraphs(wordDoc);
    RemoveEmptyParagraphs(wordDoc);
    RemoveFalseHeadersAndFooters(wordDoc);
    ResetHeadingsFormat(wordDoc);
    ReplaceSymbolEncoding(wordDoc);
    RepairBulletContainedParagraph(wordDoc);
  }

  /// <summary>
  /// Trims all the paragraphs in the document removing starting and ending whitespaces.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void TrimParagraphs(DXPack.WordprocessingDocument wordDoc)
  {
    var body = wordDoc.GetBody();
    TrimParagraphs(body);
    foreach (var header in wordDoc.GetHeaders())
      TrimParagraphs(header);
    foreach (var footer in wordDoc.GetFooters())
      TrimParagraphs(footer);
  }

  /// <summary>
  /// Removes all the empty paragraphs from the document.
  /// </summary>
  /// <param name="body"></param>
  public void TrimParagraphs(DX.OpenXmlCompositeElement body)
  {
    var paragraphs = body.Descendants<DXW.Paragraph>().ToList();
    foreach (var paragraph in paragraphs)
      TrimParagraph(paragraph);
  }

  /// <summary>
  /// Trims the paragraph removing starting and ending whitespaces.
  /// </summary>
  /// <param name="paragraph"></param>
  public void TrimParagraph(DXW.Paragraph paragraph)
  {
    var paraText = paragraph.GetText();
    var paraTextTrimmed = paraText.Trim();
    if (paraTextTrimmed == paraText)
      return;
    for (int i = 0; i < 1000; i++)
    {
      var firstRun = paragraph.Descendants<DXW.Run>().FirstOrDefault();
      if (firstRun != null)
      {
        var firstRunItem = firstRun.Elements().Where(e => e is not DXW.RunProperties)?.FirstOrDefault();
        if (firstRunItem is DXW.TabChar || firstRunItem is DXW.LastRenderedPageBreak)
        {
          firstRunItem.Remove();
          firstRunItem = null;
        }
        else if (firstRunItem is DXW.Text firstRunText)
        {
          firstRunText.Text = firstRunText.Text.TrimStart();
          if (firstRunText.Text.Length == 0)
          {
            firstRunItem.Remove();
            firstRunItem = null;
          }
        }
        if (firstRun.IsEmpty())
        {
          DX.OpenXmlElement element = firstRun;
          var parent = element.Parent;
          if (parent is DX.OpenXmlCompositeElement compositeElement)
            compositeElement.RemoveChild(element);
          while (parent != null && parent is not DXW.Paragraph && parent.IsEmpty())
          {
            element = parent;
            parent = element.Parent;
            if (parent is DX.OpenXmlCompositeElement compositeElement1)
              compositeElement1.RemoveChild(element);
          }
        }
      }
      var lastRun = paragraph.Descendants<DXW.Run>().LastOrDefault();
      if (lastRun != null)
      {
        var lastRunItem = lastRun.Elements().Where(e => e is not DXW.RunProperties)?.LastOrDefault();
        if (lastRunItem is DXW.TabChar || lastRunItem is DXW.LastRenderedPageBreak)
        {
          lastRunItem.Remove();
        }
        else if (lastRunItem is DXW.Text lastRunText)
        {
          lastRunText.Text = lastRunText.Text.TrimEnd();
          if (lastRunText.Text.Length == 0)
          {
            lastRunItem.Remove();
          }
        }
        if (lastRun.IsEmpty())
        {
          DX.OpenXmlElement element = lastRun;
          var parent = element.Parent;
          if (parent is DX.OpenXmlCompositeElement compositeElement)
            compositeElement.RemoveChild(element);
          while (parent != null && parent is not DXW.Paragraph && parent.IsEmpty())
          {
            element = parent;
            parent = element.Parent;
            if (parent is DX.OpenXmlCompositeElement compositeElement1)
              compositeElement1.RemoveChild(element);
          }
        }
      }
      paraText = paragraph.GetText();
      if (paraTextTrimmed == paraText)
        return;
    }
  }

  /// <summary>
  /// Removes all the empty paragraphs from the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RemoveEmptyParagraphs(DXPack.WordprocessingDocument wordDoc)
  {
    var body = wordDoc.GetBody();
    RemoveEmptyParagraphs(body);
    foreach (var header in wordDoc.GetHeaders())
      RemoveEmptyParagraphs(header);
    foreach (var footer in wordDoc.GetFooters())
      RemoveEmptyParagraphs(footer);
  }

  /// <summary>
  /// Removes all the empty paragraphs from the document.
  /// </summary>
  /// <param name="body"></param>
  public void RemoveEmptyParagraphs(DX.OpenXmlCompositeElement body)
  {
    var emptyParagraphs = body.Descendants<DXW.Paragraph>().Where(p => p.IsEmpty()).ToList();
    foreach (var paragraph in emptyParagraphs)
    {
      if (paragraph.Ancestors<DXW.Table>().Any())
      {
        if (paragraph.Parent is DXW.TableCell tableCell)
        {
          if (tableCell.Elements<DXW.Paragraph>().Count() > 1)
          {
            if (paragraph.PreviousSibling() is DXW.Table && paragraph.NextSibling() == null)
            {
              Debug.Assert(true);
              // Do not remove the last paragraph in the table cell when the table is the previous sibling.
            }
            else
            {
              paragraph.Remove();
            }
          }
          else
          {
            Debug.Assert(true);
            // Do not remove the single paragraph in the table cell.
          }
        }
        else
        {
          Debug.Assert(true);
          // Do not remove the paragraph when it is not in a table cell.
        }
      }
      else
        paragraph.Remove();
    }
  }

  /// <summary>
  /// Removes paragraphs that are same as headers or footers but are contained in body.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RemoveFalseHeadersAndFooters(DXPack.WordprocessingDocument wordDoc)
  {
    HashSet<string> headers = new();
    var allHeaders = wordDoc.GetHeaders().Select(h => h.GetText(null)).ToArray();
    foreach (var str in allHeaders)
      if (!string.IsNullOrEmpty(str))
      {
        var s = CleanWhitespace(TryRemoveNumberingDot(str!));
        if (s.Length > 0 && !IsNumber(s))
          headers.Add(s);
      }
    var allFooters = wordDoc.GetFooters().Select(f => f.GetText(null)).ToArray();
    foreach (var str in allFooters)
      if (!string.IsNullOrEmpty(str))
      {
        var s = CleanWhitespace(TryRemoveNumberingDot(str!));
        if (s.Length > 0 && !IsNumber(s))
          headers.Add(s);
      }
    var body = wordDoc.GetBody();
    var headings1 = body.Elements<DXW.Paragraph>().Where(p => p.HeadingLevel() == 1)
      .Select(h => h.GetText()).ToList();
    foreach (var str in headings1)
      if (!string.IsNullOrEmpty(str))
      {
        var s = CleanWhitespace(TryRemoveNumberingDot(str!));
        if (s.Length > 0 && !IsNumber(s))
          headers.Add(s);
      }
    var paragraphs = body.Elements<DXW.Paragraph>().Where(p => !p.IsHeading()).ToList();
    foreach (var paragraph in paragraphs)
    {
      var paraText = paragraph.GetText();
      var s = CleanWhitespace(TryRemoveNumberingDot(paraText));
      if (headers.Contains(s))
      {
        paragraph.Remove();
      }
    }
  }

  /// <summary>
  /// Resets format of heading paragraphs.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void ResetHeadingsFormat(DXPack.WordprocessingDocument wordDoc)
  {
    var body = wordDoc.GetBody();
    var paragraphs = body.Elements<DXW.Paragraph>().Where(p => p.IsHeading()).ToList();
    foreach (var paragraph in paragraphs)
    {
      var properties = paragraph.ParagraphProperties;
      if (properties != null)
      {
        foreach (var item in properties.Elements().ToList())
        {
          if (item is not DXW.ParagraphStyleId)
            item.Remove();
        }
      }
      foreach (var run in paragraph.Elements<DXW.Run>())
      {
        var runProperties = run.RunProperties;
        if (runProperties != null)
        {
          foreach (var item in runProperties.Elements().ToList())
          {
            if (item is not DXW.RunStyle)
              item.Remove();
          }
        }
      }
    }
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void ReplaceSymbolEncoding(DXPack.WordprocessingDocument wordDoc)
  {
    var body = wordDoc.GetBody();
    ReplaceSymbolEncoding(body);
    foreach (var header in wordDoc.GetHeaders())
      ReplaceSymbolEncoding(header);
    foreach (var footer in wordDoc.GetFooters())
      ReplaceSymbolEncoding(footer);
  }


  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="body"></param>
  public void ReplaceSymbolEncoding(DX.OpenXmlCompositeElement body)
  {
    foreach (var paragraph in body.Descendants<DXW.Paragraph>())
    {
      ReplaceSymbolEncoding(paragraph);
    }
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="paragraph"></param>
  public void ReplaceSymbolEncoding(DXW.Paragraph paragraph)
  {
    foreach (var run in paragraph.Descendants<DXW.Run>())
    {
      ReplaceSymbolEncoding(run);
    }
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="run"></param>
  public void ReplaceSymbolEncoding(DXW.Run run)
  {
    foreach (var text in run.Elements<DXW.Text>())
    {
      text.Text = text.Text.ReplaceSymbolEncoding();
    }
  }

  /// <summary>
  /// Detect paragraphs that contain a bullet and enter a new new paragraph with bullet numbering.
  /// replace the bullet with a corresponding unicode character.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RepairBulletContainedParagraph(DXPack.WordprocessingDocument wordDoc)
  {
    var defaultParagraphProperties = GetMostFrequentNumberingParagraphProperties(wordDoc);
    if (defaultParagraphProperties == null)
    {
      var abstractNumbering = wordDoc.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?
        .Elements<DXW.AbstractNum>().FirstOrDefault(a=>a.MultiLevelType?.Val?.Value==DXW.MultiLevelValues.HybridMultilevel 
                                                       && a.Elements<DXW.Level>().FirstOrDefault(l => l.LevelText?.Val?.Value== "•")!=null);
      if (abstractNumbering != null)
      {
        var numbering = wordDoc.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?
          .Elements<DXW.NumberingInstance>().FirstOrDefault(n => n.AbstractNumId?.Val?.Value == abstractNumbering.AbstractNumberId?.Value);
        if (numbering != null)
        {
          defaultParagraphProperties = new DXW.ParagraphProperties
            { NumberingProperties = new DXW.NumberingProperties
            {
              NumberingLevelReference = new DXW.NumberingLevelReference{ Val = 0},
              NumberingId = new DXW.NumberingId { Val = numbering.NumberID }
            }
            };
        }
      }
    }
    var paragraphs = wordDoc.GetBody().Descendants<DXW.Paragraph>().ToList();
    for (int i = 0; i < paragraphs.Count; i++)
    {
      var paragraph = paragraphs[i];
      foreach (var run in paragraph.Elements<DXW.Run>())
      {
        var text = run.GetText();
        if (text.Contains("•"))
        {
          var textItem = run.Descendants<DXW.Text>().FirstOrDefault(t => t.Text.TrimStart().StartsWith("•"));
          if (textItem == null)
            continue;
          textItem.Text = text.Replace("•", "");
          var newParagraphProperties = GetNearbyNumberingParagraphProperties(paragraph) 
                                       ?? (DXW.ParagraphProperties?)defaultParagraphProperties?.CloneNode(true);
          if (run.PreviousSibling() != null && run.PreviousSibling() is not DXW.ParagraphProperties)
          {
            var newParagraph = new DXW.Paragraph();
            newParagraph.ParagraphProperties = newParagraphProperties;
            var tailItems = new List<DX.OpenXmlElement>();
            tailItems.Add(run);
            var siblingItem = run.NextSibling();
            while (siblingItem != null)
            {
              tailItems.Add(siblingItem);
              siblingItem = siblingItem.NextSibling();
            }
            foreach (var item in tailItems)
            {
              item.Remove();
              newParagraph.Append(item);
            }
            TrimParagraph(paragraph);
            var priorParagraph = paragraph.PreviousSibling<DXW.Paragraph>();
            if (priorParagraph != null && priorParagraph.ParagraphProperties?.NumberingProperties!=null)
              paragraph.ParagraphProperties = (DXW.ParagraphProperties)priorParagraph.ParagraphProperties.CloneNode(true);
            TrimParagraph(newParagraph);
            paragraph.InsertAfterSelf(newParagraph);
            paragraphs.Insert(i + 1, newParagraph);
          }
          else // if it is the first run in the paragraph then do not create a new paragraph.
          {
            paragraph.ParagraphProperties = newParagraphProperties;
            TrimParagraph(paragraph);
            i--;
          }
        }
      }
    }
  }

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
          if (numberingLevel== null || numberingLevel!=0)
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
    return (DXW.ParagraphProperties)numberingPropertiesIndex[mostFrequentNumberingId].CloneNode(true); ;
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

  private bool IsNumber(string str)
  {
    return str.ToCharArray().All(char.IsDigit);
  }

  /// <summary>
  /// Replaces all the whitespaces with a single space.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private string NormalizeWhitespace(string str)
  {
    var chars = str.ToList();
    var wasChar = false;
    for (int i = 0; i < chars.Count; i++)
    {
      if (char.IsWhiteSpace(chars[i]))
      {
        chars[i] = ' ';
        wasChar = false;
      }
      if (chars[i] == ' ')
      {
        if (wasChar)
        {
          chars.RemoveAt(i);
          i--;
        }
        else
          wasChar = true;
      }
      else
        wasChar = false;
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Removes all the whitespaces.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private string CleanWhitespace(string str)
  {
    var chars = str.ToList();
    for (int i = 0; i < chars.Count; i++)
    {
      if (char.IsWhiteSpace(chars[i]))
      {
        chars.RemoveAt(i);
        i--;
      }
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Removes a dot that ends the numbering string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private string TryRemoveNumberingDot(string str)
  {
    var numStr = str.GetNumberingString();
    if (numStr != null)
    {
      if (str.Length > numStr.Length && str[numStr.Length - 1] == '.')
        str = str.Remove(numStr.Length - 1, 1);
    }
    return str;
  }

}