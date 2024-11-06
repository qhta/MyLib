using System;

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
    var paraProperties = paragraph.GetParagraphProperties();
    paragraph.RemoveAllChildren();
    paragraph.AppendChild(paraProperties);
    var run = new Run();
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

}