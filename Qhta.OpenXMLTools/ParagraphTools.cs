using System;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Paragraph element.
/// </summary>
public static class ParagraphTools
{
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
  /// <returns>joined text</returns>
  public static string GetText(this Paragraph paragraph)
  {
    return String.Join("", paragraph.Elements<Run>().Select(item => item.GetText()));
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

  //public static Style? Style(this Paragraph paragraph)
  //{
  //  var styleId = paragraph.StyleId();
  //  return styleId == null ? null : StyleTools.GetStyle(paragraph.GetDocumentPart().styleId);
  //}
  //public static string? StyleName(this Paragraph paragraph)
  //{
  //  return paragraph.StyleId();
  //}
  ///// <summary>
  ///// Check if the paragraph is a heading using its style name.
  ///// </summary>
  ///// <param name="paragraph">Checked paragraph</param>
  ///// <returns>true if the paragraph outline level is not base text level</returns>
  //public static bool IsHeading(this Paragraph paragraph)
  //{
  //  var styleId = paragraph.StyleId();

  //  return false;
  //}

  /// <summary>
  /// Check if the paragraph is a heading using the style name.
  /// </summary>
  /// <param name="styleName">style name to check</param>
  /// <param name="headingName">heading name string (preceding a number)</param>
  /// <returns></returns>
  private static bool IsHeading(string styleName, string headingName)
  {
    return styleName.StartsWith(headingName) && Char.IsDigit(styleName[headingName.Length]);
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