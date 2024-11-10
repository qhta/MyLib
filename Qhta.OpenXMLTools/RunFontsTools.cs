namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with RunFonts elements.
/// </summary>
public static class RunFontsTools
{

  /// <summary>
  /// Get Ascii font name or AsciiTheme font name (if defined)
  /// </summary>
  /// <param name="runFonts"></param>
  /// <returns></returns>
  public static string? GetAsciiFontName(this DXW.RunFonts runFonts)
  {
    if (runFonts.AsciiTheme != null)
    {
      var themeFontValue = runFonts.AsciiTheme.Value;
      var fontScheme = runFonts.GetMainDocumentPart()?.ThemePart?.Theme?.ThemeElements?.FontScheme;
      if (fontScheme != null)
        return fontScheme.GetThemeFontName(themeFontValue);
    }
    if (runFonts.Ascii != null)
      return runFonts.Ascii;
    return null;
  }

  /// <summary>
  /// Get HighAnsi font name or HighAnsiTheme font name (if defined)
  /// </summary>
  /// <param name="runFonts"></param>
  /// <returns></returns>
  public static string? GetHighAnsiFontName(this DXW.RunFonts runFonts)
  {
    if (runFonts.HighAnsiTheme != null)
    {
      var themeFontValue = runFonts.HighAnsiTheme.Value;
      var fontScheme = runFonts.GetMainDocumentPart()?.ThemePart?.Theme?.ThemeElements?.FontScheme;
      if (fontScheme != null)
        return fontScheme.GetThemeFontName(themeFontValue);
    }
    if (runFonts.HighAnsi != null)
      return runFonts.HighAnsi;
    return null;
  }

  /// <summary>
  /// Get ComplexScript font name or ComplexScriptTheme font name (if defined)
  /// </summary>
  /// <param name="runFonts"></param>
  /// <returns></returns>
  public static string? GetComplexScriptFontName(this DXW.RunFonts runFonts)
  {
    if (runFonts.ComplexScriptTheme != null)
    {
      var themeFontValue = runFonts.ComplexScriptTheme.Value;
      var fontScheme = runFonts.GetMainDocumentPart()?.ThemePart?.Theme?.ThemeElements?.FontScheme;
      if (fontScheme != null)
        return fontScheme.GetThemeFontName(themeFontValue);
    }
    if (runFonts.ComplexScript != null)
      return runFonts.ComplexScript;
    return null;
  }

  /// <summary>
  /// Get EastAsia font name or EastAsiaTheme font name (if defined).
  /// </summary>
  /// <param name="runFonts"></param>
  /// <returns></returns>
  public static string? GetEastAsiaFontName(this DXW.RunFonts runFonts)
  {
    if (runFonts.EastAsiaTheme != null)
    {
      var themeFontValue = runFonts.EastAsiaTheme.Value;
      var fontScheme = runFonts.GetMainDocumentPart()?.ThemePart?.Theme?.ThemeElements?.FontScheme;
      if (fontScheme != null)
        return fontScheme.GetThemeFontName(themeFontValue);
    }
    if (runFonts.EastAsia != null)
      return runFonts.EastAsia;
    return null;
  }

}