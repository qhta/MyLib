using System;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

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

  public static string GetText(this TableCell cell)
  {
    return String.Join("\r\n", cell.Elements<Paragraph>().Select(p => p.GetText()));
  }

  public static string? Style(this Table table)
  {
    return table.Elements<TableProperties>().FirstOrDefault()?.TableStyle?.Val?.Value;
  }
}