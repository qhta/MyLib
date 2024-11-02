using System;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with OpenXml Hyperlink elements.
/// </summary>
public static class HyperlinkTools
{
  /// <summary>
  /// Get the text of the hyperlink run elements.
  /// </summary>
  /// <param name="hyperlink">source hyperlink</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetText(this DXW.Hyperlink hyperlink, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    var sb = new StringBuilder();
    foreach (var item in hyperlink.Elements())
    {
      var text = item.GetText(options);
      sb.Append(text);
    }
    var result = sb.ToString();
    return result;
  }

  /// <summary>
  /// Checks if the hyperlink is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Hyperlink? element)
  {
    if (element == null)
      return true;
    var text = element.GetText();
    return string.IsNullOrEmpty(text);
  }
}