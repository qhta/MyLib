using System;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with styles in OpenXml documents.
/// </summary>
public static class StyleTools
{

  /// <summary>
  /// Converts a style name to a valid style ID.
  /// </summary>
  /// <param name="styleName">Name to convert</param>
  /// <returns>Style normalized ID</returns>
  public static string StyleNameToId(string styleName)
  {
    styleName = styleName.CamelCase();
    var chars = styleName.ToCharArray().ToList();
    chars.RemoveAll(c => !(c <= '\x7f' && Char.IsLetterOrDigit(c) || c == '-'));
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Checks if the style name starts with "Heading".
  /// </summary>
  /// <param name="styleName">Name to check</param>
  /// <returns>true or false</returns>
  public static bool IsHeading (string styleName)
  {
    return styleName.StartsWith("Heading", StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>
  /// Checks if the style is a heading style.
  /// </summary>
  /// <param name="style">Style to check</param>
  /// <returns>true or false</returns>
  public static bool IsHeading (this DXW.Style style)
  {
    return style.StyleName!.Val!.Value!.StartsWith("Heading", StringComparison.OrdinalIgnoreCase);
  }
}
