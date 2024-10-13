using System;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools;

/// <summary>
/// Extension methods for the paragraph.
/// </summary>
public static class ParagraphTools
{

  /// <summary>
  /// Gets the text of the all the runs in the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  public static string GetText(this Paragraph paragraph)
  {
    return String.Join("", paragraph.Elements<Run>().Select(item => item.GetText()));
  }
}