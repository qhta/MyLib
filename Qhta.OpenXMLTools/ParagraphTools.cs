using System;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools;

public static class ParagraphTools
{
  public static Paragraph? FindParagraph(DXPack.WordprocessingDocument document, string paraId)
  {
    return document.MainDocumentPart?.Document?.Body?.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  }

  public static Paragraph? FindParagraph(DX.OpenXmlCompositeElement compositeElement, string paraId)
  {
    return compositeElement.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  }

  public static string GetText(this Paragraph paragraph)
  {
    return String.Join("", paragraph.Elements<Run>().Select(item => item.GetText()));
  }

  public static string? Style(this Paragraph paragraph)
  {
    return paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
  }

  public static bool IsHeading(this Paragraph paragraph)
  {
     var styleName = paragraph.Style();
     if (styleName != null)
     {
       return IsHeading(styleName, "Heading") || IsHeading(styleName, "Nagwek");
     }
     return false;
  }

  private static bool IsHeading(string styleName, string headingName)
  {
    return styleName.StartsWith(headingName) && Char.IsDigit(styleName[headingName.Length]);
  }

  public static int? OutlineLevel(this Paragraph paragraph)
  {
    return paragraph.ParagraphProperties?.OutlineLevel?.Val?.Value;
  }
}