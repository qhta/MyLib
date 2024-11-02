using System;

using DocumentFormat.OpenXml.Drawing;

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
    //if (body is DXW.Header header)
    //{
    //  var aText = header.GetText();
    //  if (aText!.StartsWith("17. WordprocessingML Reference Material"))
    //    Debug.Assert(true);
    //}
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
      if (str.Length > numStr.Length && str[numStr.Length-1]== '.')
        str = str.Remove(numStr.Length - 1,1);
    }
    return str;
  }
}