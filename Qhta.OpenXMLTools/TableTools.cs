using System;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with tables in OpenXml documents.
/// </summary>
public static class TableTools
{
  //public static Table? FindParagraph(DXPack.WordprocessingDocument document, string paraId)
  //{
  //  return document.MainDocumentPart?.Document?.Body?.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  //}

  //public static Table? FindParagraph(DX.OpenXmlCompositeElement compositeElement, string paraId)
  //{
  //  return compositeElement.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  //}

  /// <summary>
  /// Gets the text of all paragraph in the table.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static string GetText(this TableCell cell)
  {
    return String.Join("\r\n", cell.Elements<Paragraph>().Select(p => p.GetText()));
  }

  /// <summary>
  /// Gets the style name of the table.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public static string? Style(this Table table)
  {
    return table.Elements<TableProperties>().FirstOrDefault()?.TableStyle?.Val?.Value;
  }
}