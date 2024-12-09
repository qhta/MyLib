﻿using System;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Paragraph element.
/// </summary>
public static class ParagraphTools
{

  /// <summary>
  /// Gets the integer identifier of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static int? GetParagraphId(this DXW.Paragraph paragraph)
  {
    var val = paragraph.ParagraphId?.Value;
    return val == null ? null : int.Parse(val, NumberStyles.HexNumber);
  }

  /// <summary>
  /// Find a paragraph in the document by the paragraph id.
  /// </summary>
  /// <param name="document">document to browse</param>
  /// <param name="paraId">id of the paragraph</param>
  /// <returns>found paragraph or null</returns>
  public static Paragraph? FindParagraph(DXPack.WordprocessingDocument document, string paraId)
  {
    return document.MainDocumentPart?.Document?.Body?.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  }

  /// <summary>
  /// Find a paragraph in the composite element by the paragraph id.
  /// </summary>
  /// <param name="compositeElement">element to browse</param>
  /// <param name="paraId">id of the paragraph</param>
  /// <returns>found paragraph or nul</returns>
  public static Paragraph? FindParagraph(DX.OpenXmlCompositeElement compositeElement, string paraId)
  {
    return compositeElement.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  }

  /// <summary>
  /// Get the <c>ParagraphProperties</c> element of the paragraph. If it is null, create a new one.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static ParagraphProperties GetParagraphProperties(this Paragraph paragraph)
  {
    if (paragraph.ParagraphProperties == null)
      paragraph.ParagraphProperties = new ParagraphProperties();
    return paragraph.ParagraphProperties;
  }

  /// <summary>
  /// Get the text of the paragraph run elements.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetText(this Paragraph paragraph, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    var result = String.Join("", paragraph.Elements().Select(item => item.GetText(options)));
    if (options.IncludeParagraphNumbering)
      result = paragraph.GetNumberingString(options) + result;
    return result;
  }

  /// <summary>
  /// Set the text of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="text"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static void SetText(this Paragraph paragraph, string? text, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    var textProperties = paragraph.GetTextProperties("defaultFont");
    paragraph.RemoveAllMembers();
    var run = new Run();
    if (textProperties != null)
      run.SetTextProperties(textProperties);
    run.SetText(text, options);
    paragraph.AppendChild(run);
  }

  /// <summary>
  /// Get the style id of the paragraph.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <returns>style id or null</returns>
  public static string? GetStyleId(this Paragraph paragraph)
  {
    return paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
  }

  /// <summary>
  /// Set the style id of the paragraph.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="styleId">style id or null</param>
  public static void SetStyleId(this Paragraph paragraph, string styleId)
  {
    if (paragraph.ParagraphProperties == null)
      paragraph.ParagraphProperties = new ParagraphProperties();
    paragraph.ParagraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = styleId };
  }

  /// <summary>
  /// Get the style of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static Style? GetStyle(this Paragraph paragraph)
  {
    var styleId = paragraph.GetStyleId();
    if (styleId != null)
    {
      var wordDoc = paragraph.GetWordprocessingDocument();
      if (wordDoc != null)
      {
        var stylePart = wordDoc.MainDocumentPart?.StyleDefinitionsPart;
        if (stylePart != null)
        {
          return stylePart.Styles?.Elements<Style>().FirstOrDefault(s => s.StyleId == styleId);
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Check if the paragraph is a heading.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <returns></returns>
  public static bool IsHeading(this Paragraph paragraph)
  {
    var style = paragraph.GetStyle();
    return style?.IsHeading() ?? false;
  }

  /// <summary>
  /// Get the outline level of the paragraph.
  /// Outline level is counted rom 0 to 9,
  /// where 9 specifically indicates that there is no outline level specifically applied to this paragraph.
  /// </summary>
  /// <param name="paragraph">paragraph to check</param>
  /// <returns>number of paragraph outline level</returns>
  public static int? OutlineLevel(this Paragraph paragraph)
  {
    var result = paragraph.ParagraphProperties?.OutlineLevel?.Val?.Value;
    if (result == null)
    {
      var style = paragraph.GetStyle();
      result = style?.HeadingLevel();
      if (result != null)
        result--;
      else
        result = 9;
    }
    return result;
  }

  /// <summary>
  /// Get the heading level of the paragraph.
  /// Heading level is counted from 1 to 9 (outline level + 1).
  /// If a paragraph is not a heading then the return value is null.
  /// </summary>
  /// <param name="paragraph">paragraph to check</param>
  /// <returns>number of paragraph heading level (or null)</returns>
  public static int? HeadingLevel(this Paragraph paragraph)
  {
    var result = paragraph.ParagraphProperties?.OutlineLevel?.Val?.Value;
    if (result == 9)
      result = null;
    if (result != null)
      return result + 1;
    if (result == null)
    {
      var style = paragraph.GetStyle();
      result = style?.HeadingLevel();
    }
    return result;
  }

  /// <summary>
  /// Checks if the paragraph is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Paragraph? element)
  {
    if (element == null)
      return true;
    foreach (var e in element.MemberElements())
    {
      if (e is DXW.Run run)
      {
        if (!run.IsEmpty())
          return false;
      }
      else
        return false;
    }
    return true;
  }

  /// <summary>
  /// Trims the paragraph removing leading whitespaces.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns>true if the trimming was successful, false if it was not needed.</returns>
  public static bool TrimStart(this DXW.Paragraph paragraph)
  {
    bool done = false;
    var firstElement = paragraph.MemberElements().FirstOrDefault();
    while (firstElement != null)
    {
      var nextElement = firstElement.NextSibling();
      if (firstElement is DXW.BookmarkStart || firstElement is DXW.BookmarkEnd)
      {
        // ignore;
      }
      else
      if (firstElement is DXW.Run run)
      {
        if (run.TrimStart())
        {
          done = true;
          if (run.IsEmpty())
            run.Remove();
          else
            break;
        }
        else
          break;
      }
      else if (firstElement is DXW.Hyperlink hyperlink)
      {
        if (!hyperlink.TrimStart())
          break;
      }
      firstElement = nextElement;
    }
    return done;
  }

  /// <summary>
  /// Trims the paragraph removing trailing whitespaces.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns>true if the trimming was successful, false if it was not needed.</returns>
  public static bool TrimEnd(this DXW.Paragraph paragraph)
  {
    bool done = false;
    var lastElement = paragraph.MemberElements().LastOrDefault();
    while (lastElement != null)
    {
      var previousElement = lastElement.PreviousSibling();
      if (lastElement is DXW.BookmarkEnd || lastElement is DXW.BookmarkStart)
      {
        // ignore;
      }
      else
      if (lastElement is DXW.Run run)
      {
        if (run.TrimEnd())
        {
          done = true;
          if (run.IsEmpty())
            run.Remove();
          else
            break;
        }
        else
          break;
      }
      else if (lastElement is DXW.Hyperlink hyperlink)
      {
        if (hyperlink.TrimEnd())
          done = true;
        else
          break;
      }
      lastElement = previousElement;
    }
    return done;
  }

  /// <summary>
  /// Reset paragraph format by removing all the properties except the style id and numbering properties.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="deep">reset also text format</param>
  /// <returns>true if any properties were removed</returns>
  public static bool TryResetFormat(this DXW.Paragraph paragraph, bool deep)
  {
    bool done = false;
    var properties = paragraph.ParagraphProperties;
    if (properties != null)
    {
      foreach (var item in properties.Elements().ToList())
      {
        if (item is not DXW.ParagraphStyleId and not DXW.NumberingProperties)
        {
          item.Remove();
          done = true;
        }
      }
    }
    if (deep)
    {
      foreach (var run in paragraph.Elements<DXW.Run>())
      {
        if (run.TryResetFormat())
          done = true;
      }
    }
    return done;
  }

  /// <summary>
  /// Removes all the members of the paragraph (leaving paragraph properties).
  /// </summary>
  /// <param name="paragraph"></param>
  public static void RemoveAllMembers(this DXW.Paragraph paragraph)
  {
    foreach (var member in paragraph.MemberElements().ToList())
      member.Remove();
  }

  /// <summary>
  /// Get the name of the font most frequently used in the paragraph.
  /// </summary>
  /// <param name="paragraph">Paragraph element to examine</param>
  /// <param name="defaultFont">Font name used when there is no runFonts element</param>
  public static string? GetFont(this DXW.Paragraph paragraph, string? defaultFont)
  {
    var fonts = paragraph.GetRunFonts(defaultFont);
    if (fonts != null)
      return fonts.MostFrequent();
    return null;
  }

  /// <summary>
  /// Get statistics of fonts used in the paragraph.
  /// If the paragraph has no font information, return default font statistics.
  /// </summary>
  /// <param name="paragraph">Paragraph element to examine</param>
  /// <param name="defaultFont">Font name used when there is no runFonts element</param>
  public static StringStatistics? GetRunFonts(this DXW.Paragraph paragraph, string? defaultFont)
  {
    StringStatistics? fonts = null;
    foreach (var run in paragraph.Descendants<DXW.Run>().ToList())
    {
      var runFonts = run.GetRunFonts(defaultFont);
      if (runFonts != null)
      {
        fonts ??= new StringStatistics();
        fonts.Add(runFonts);
      }
    }
    return fonts;
  }

  /// <summary>
  /// Get the most frequently used text properties of the paragraph.
  /// </summary>
  /// <param name="paragraph">Paragraph element to examine</param>
  /// <param name="defaultFontName">Default font name used when there is no runFonts element</param>
  /// <param name="defaultFontSize">Default font name used when there is no runFonts element</param>
  public static TextProperties? GetTextProperties(this DXW.Paragraph paragraph, string? defaultFontName, int? defaultFontSize = null)
  {
    var defaultProperties = new TextProperties { FontName = defaultFontName, FontSize = defaultFontSize };
    return paragraph.GetTextProperties(defaultProperties);
  }

  /// <summary>
  /// Get the most frequently used text properties of the paragraph.
  /// </summary>
  /// <param name="paragraph">Paragraph element to examine</param>
  /// <param name="defaultProperties">Default text properties used when there is no text properties</param>
  public static TextProperties? GetTextProperties(this DXW.Paragraph paragraph, TextProperties? defaultProperties)
  {
    ObjectStatistics<TextProperties>? statistics = null;
    foreach (var run in paragraph.Descendants<DXW.Run>().ToList())
    {
      var runStatistics = run.GetTextPropertiesStatistics(defaultProperties);
      if (runStatistics != null)
      {
        statistics ??= new ObjectStatistics<TextProperties>();
        statistics.Add(runStatistics);
      }
    }
    return statistics?.MostFrequent();
  }

  /// <summary>
  /// Normalize whitespaces in all runs in the paragraph.
  /// </summary>
  /// <param name="paragraph">Paragraph element to process</param>
  public static void NormalizeWhitespaces(this DXW.Paragraph paragraph)
  {
    foreach (var run in paragraph.Descendants<DXW.Run>().ToList())
      run.NormalizeWhitespaces();
  }

  /// <summary>
  /// Append text to the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="text"></param>
  public static void AppendText(this DXW.Paragraph paragraph, string text)
  {
    var run = paragraph.Descendants<DXW.Run>().LastOrDefault();
    if (run == null)
    {
      run = new DXW.Run();
      var textProperties = paragraph.GetTextProperties(null);
      if (textProperties != null)
        run.SetTextProperties(textProperties);
      paragraph.AppendChild(run);
    }
    run.AppendText(text);
  }

  /// <summary>
  /// Split the paragraph at the specified index, which is the number of characters from the beginning of the paragraph.
  /// Returns the second part of the paragraph.
  /// Split is not possible if the index is at the beginning or end of the paragraph.
  /// Split is possible only in a run element.
  /// </summary>
  /// <param name="paragraph">Paragraph element to process</param>
  /// <param name="index">Char position number</param>
  /// <param name="options">Options for text extraction</param>
  /// <returns>Next, newly created paragraph (or null) if split is not available</returns>
  public static DXW.Paragraph? SplitAt(this DXW.Paragraph paragraph, int index, GetTextOptions? options = null)
  {
    if (index <= 0 || index >= paragraph.GetText().Length)
      return null;
    options ??= GetTextOptions.Default;
    var textLength = 0;
    DXW.Paragraph? newParagraph = null;
    foreach (var member in paragraph.MemberElements().ToList())
    {
      var memberText = member.GetText(options);
      if (memberText != null)
      {
        var memberTextLength = memberText.Length;
        var newTextLength = textLength + memberTextLength;
        if (index <= newTextLength)
        {
          if (index < newTextLength)
          {
            if (member is DXW.Run run)
            {
              DX.OpenXmlElement? newMember = run.SplitAt(index - textLength, options);
              if (newMember != null)
              {
                newParagraph ??= NewParagraph(paragraph);
                newParagraph.AppendChild(newMember);
              }
            }
            else
              return null;
          }
          var nextSibling = member.NextSibling();
          while (nextSibling != null)
          {
            newParagraph ??= NewParagraph(paragraph);
            var nextSibling1 = nextSibling.NextSibling();
            nextSibling.Remove();
            newParagraph.AppendChild(nextSibling);
            nextSibling = nextSibling1;
          }
          break;
        }
        else
        {
          textLength = newTextLength;
        }
      }
    }
    return newParagraph;
  }

  /// <summary>
  /// Create a new paragraph with the same properties as the source paragraph.
  /// </summary>
  /// <param name="paragraph">Paragraph element to process</param>
  public static DXW.Paragraph NewParagraph(this DXW.Paragraph paragraph)
  {
    var newParagraph = new DXW.Paragraph();
    var properties = paragraph.ParagraphProperties;
    if (properties != null)
    {
      newParagraph.ParagraphProperties = (DXW.ParagraphProperties)properties.CloneNode(true);
      //if (paragraph.PreviousSibling() is Paragraph priorParagraph)
      //{
      //  var priorSpacing = priorParagraph.ParagraphProperties?.SpacingBetweenLines;
      //  if (priorSpacing != null)
      //  {
      //    var spacing = properties.GetSpacingBetweenLines();
      //    spacing.After = priorSpacing.After;
      //    spacing.AfterAutoSpacing = priorSpacing.AfterAutoSpacing;
      //    spacing.AfterLines = priorSpacing.AfterLines;
      //  }
      //}
    }
    return newParagraph;
  }

  /// <summary>
  /// Get the indentation of the paragraph. If it is not defined, return null.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static DXW.Indentation GetIndentation(this DXW.Paragraph paragraph)
  {
    var paraProperties = paragraph.GetParagraphProperties();
    var indentation = paraProperties.Indentation;
    if (indentation == null)
    {
      indentation = new DXW.Indentation();
      paraProperties.Indentation = indentation;
    }
    return indentation;
  }

  /// <summary>
  /// Get the section properties of the paragraph.
  /// </summary>
  /// <returns></returns>
  public static DXW.SectionProperties? GetSectionProperties(this Paragraph paragraph)
  {
    var parent = paragraph.Parent;
    DX.OpenXmlElement? element = paragraph;
    while (parent != null && parent is not DXW.Body)
    {
      element = parent;
      parent = element.Parent;
    }
    if (parent is DXW.Body body && element is DXW.Paragraph topParagraph)
    {
      var nextElement = topParagraph.NextSibling();
      do
      {
        if (nextElement is DXW.SectionProperties sectionProperties)
          return sectionProperties;
        if (nextElement is DXW.Paragraph nextParagraph)
        {
          var sectionProperties1 = nextParagraph.ParagraphProperties?.SectionProperties;
          if (sectionProperties1 != null)
            return sectionProperties1;
        }
        nextElement = nextElement?.NextSibling();
      } while (nextElement != null);
    }
    return null;
  }


  /// <summary>
  /// Join next paragraph to the paragraph. Insert space before the next paragraph.
  /// </summary>
  /// <param name="para"></param>
  /// <param name="nextPara"></param>
  public static void JoinNextParagraph(this DXW.Paragraph para, DXW.Paragraph nextPara)
  {
    var lastText = para.Elements<DXW.Run>().LastOrDefault()?.Elements<DXW.Text>().LastOrDefault();
    if (lastText != null)
      lastText.Text = lastText.Text + " ";
    foreach (var item in nextPara.MemberElements().ToList())
    {
      item.Remove();
      para.AppendChild(item);
    }
  }

  /// <summary>
  /// Break the paragraph before the specified text.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="str"></param>
  /// <returns></returns>
  public static bool BreakBefore(this DXW.Paragraph paragraph, string str)
  {
    var done = false;
    var paraText = paragraph.GetText().Trim();
    var index = paraText.IndexOf(str);
    while (index > 0 && index < paraText.Length - str.Length)
    {
      var newParagraph = paragraph.SplitAt(index);
      if (newParagraph == null)
        break;
      paragraph.TrimEnd();
      newParagraph.TrimStart();
      paraText = paragraph.GetText();
      var newText = newParagraph.GetText();
      paragraph.InsertAfterSelf(newParagraph);
      Debug.WriteLine($"Break \"{paraText}\" & \"{newText}\"");
      done = true;
      paragraph = newParagraph;
      if (paragraph.GetText().Trim() == "")
        paragraph.Remove();
      paraText = newText;
      index = paraText.IndexOf(str);
    }
    return done;
  }

  /// <summary>
  /// Set the paragraph justification.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="value"></param>
  public static void SetJustification(this DXW.Paragraph paragraph, JustificationValues value)
  {
    var paragraphProperties = paragraph.GetParagraphProperties();
    paragraphProperties.Justification = new Justification { Val = value };
  }
}