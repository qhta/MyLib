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
  /// Get the text of the paragraph run elements.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetText(this Paragraph paragraph, GetTextOptions? options)
  {
    options ??= GetTextOptions.Default;
    var result = String.Join("", paragraph.Elements<Run>().Select(item => item.GetText(options)));
    if (options.IncludeParagraphNumbering)
      result = paragraph.GetNumberingString(options) + result;
    return result;
  }

  /// <summary>
  /// Get the style id of the paragraph.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <returns>style id or null</returns>
  public static string? StyleId(this Paragraph paragraph)
  {
    return paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
  }

  /// <summary>
  /// Get the style of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static Style? GetStyle(this Paragraph paragraph)
  {
    var styleId = paragraph.StyleId();
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
  private static bool IsHeading(this Paragraph paragraph)
  {
    var style = paragraph.GetStyle();
    return style?.IsHeading() ?? false;
  }

  /// <summary>
  /// Get the outline level of the paragraph.
  /// </summary>
  /// <param name="paragraph">paragraph to check</param>
  /// <returns>number of paragraph outline level</returns>
  public static int? OutlineLevel(this Paragraph paragraph)
  {
    return paragraph.ParagraphProperties?.OutlineLevel?.Val?.Value;
  }

}