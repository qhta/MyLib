using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with underline element.
/// </summary>
public static class UnderlineTools
{
  /// <summary>
  /// Get underline color.
  /// </summary>
  /// <param name="underline">Underline element to examine</param>
  /// <returns></returns>
  public static DXO13W.Color? GetColor(this DXW.Underline underline)
  {
    var themeColor = underline.ThemeColor;
    if (themeColor != null)
    {
      var color = new DXO13W.Color { ThemeColor = underline.ThemeColor };
      if (underline.ThemeShade != null)
        color.ThemeShade = underline.ThemeShade;
      if (underline.ThemeTint != null)
        color.ThemeTint = underline.ThemeTint;
      return color;
    }
    if (underline.Color != null)
      return new DXO13W.Color { Val = underline.Color.Value };
    return null;
  }
}