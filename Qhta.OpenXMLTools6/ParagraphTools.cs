using System;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools
{
  public static class ParagraphTools
  {

    public static string GetText(this Paragraph paragraph)
    {
      return String.Join("", paragraph.Elements<Run>().Select(item => item.GetText()));
    }
  }
}
