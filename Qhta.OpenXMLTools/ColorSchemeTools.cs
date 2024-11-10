namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with color schemes.
/// </summary>
public static class ColorSchemeTools
{
  /// <summary>
  /// Get the color from color scheme.
  /// </summary>
  /// <param name="colorScheme">Color scheme to examine</param>
  /// <param name="themeColorValue">determines which color to select</param>
  /// <returns></returns>
  public static DXD.Color2Type? GetThemeColor(this DXD.ColorScheme colorScheme, DXW.ThemeColorValues themeColorValue)
  {
    if (themeColorValue == DXW.ThemeColorValues.Accent1)
      return colorScheme.Accent1Color;
    if (themeColorValue == DXW.ThemeColorValues.Accent2)
      return colorScheme.Accent2Color;
    if (themeColorValue == DXW.ThemeColorValues.Accent3)
      return colorScheme.Accent3Color;
    if (themeColorValue == DXW.ThemeColorValues.Accent4)
      return colorScheme.Accent4Color;
    if (themeColorValue == DXW.ThemeColorValues.Accent5)
      return colorScheme.Accent5Color;
    if (themeColorValue == DXW.ThemeColorValues.Accent6)
      return colorScheme.Accent6Color;

    if (themeColorValue == DXW.ThemeColorValues.Dark1)
      return colorScheme.Dark1Color;
    if (themeColorValue == DXW.ThemeColorValues.Dark2)
      return colorScheme.Dark2Color;

    if (themeColorValue == DXW.ThemeColorValues.Light1)
      return colorScheme.Light1Color;
    if (themeColorValue == DXW.ThemeColorValues.Light2)
      return colorScheme.Light2Color;

    if (themeColorValue == DXW.ThemeColorValues.Hyperlink)
      return colorScheme.Hyperlink;
    if (themeColorValue == DXW.ThemeColorValues.FollowedHyperlink)
      return colorScheme.FollowedHyperlinkColor;

    return null;
  }
}