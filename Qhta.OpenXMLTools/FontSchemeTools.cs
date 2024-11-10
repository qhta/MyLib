namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with font schemes.
/// </summary>
public static class FontSchemeTools
{
  /// <summary>
  /// Get the font name from the theme font values.
  /// </summary>
  /// <param name="fontScheme">Font scheme to examine</param>
  /// <param name="themeFontValue">determines which type name to select</param>
  /// <returns></returns>
  public static string? GetThemeFontName(this DXD.FontScheme fontScheme, DXW.ThemeFontValues themeFontValue)
  {
    if (themeFontValue == DXW.ThemeFontValues.MajorAscii)
      return fontScheme.MajorFont?.LatinFont?.Typeface;
    if (themeFontValue == DXW.ThemeFontValues.MajorHighAnsi)
      return fontScheme.MajorFont?.LatinFont?.Typeface;
    if (themeFontValue == DXW.ThemeFontValues.MajorEastAsia)
      return fontScheme.MajorFont?.EastAsianFont?.Typeface;
    if (themeFontValue == DXW.ThemeFontValues.MajorBidi)
      return fontScheme.MajorFont?.ComplexScriptFont?.Typeface;
    if (themeFontValue == DXW.ThemeFontValues.MinorAscii)
      return fontScheme.MinorFont?.LatinFont?.Typeface;
    if (themeFontValue == DXW.ThemeFontValues.MinorHighAnsi)
      return fontScheme.MinorFont?.LatinFont?.Typeface;
    if (themeFontValue == DXW.ThemeFontValues.MinorEastAsia)
      return fontScheme.MinorFont?.EastAsianFont?.Typeface;
    if (themeFontValue == DXW.ThemeFontValues.MinorBidi)
      return fontScheme.MinorFont?.ComplexScriptFont?.Typeface;

    return null;
  }
}