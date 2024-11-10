using System;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing RunProperties element.
/// </summary>
public static class RunPropertiesTools
{
  /// <summary>
  /// Converts a <see cref="RunProperties"/> element to a <see cref="StyleRunProperties"/> element.
  /// </summary>
  /// <param name="runProperties">source properties to convert</param>
  /// <returns>target properties</returns>
  public static StyleRunProperties ToStyleRunProperties(this RunProperties runProperties)
  {
    var styleRunProperties = new StyleRunProperties();
    if (runProperties.Bold != null) styleRunProperties.Bold = (Bold)runProperties.Bold.CloneNode(true);
    if (runProperties.BoldComplexScript != null) styleRunProperties.BoldComplexScript = (BoldComplexScript)runProperties.BoldComplexScript.CloneNode(true);
    if (runProperties.Border != null) styleRunProperties.Border = (Border)runProperties.Border.CloneNode(true);
    if (runProperties.Caps != null) styleRunProperties.Caps = (Caps)runProperties.Caps.CloneNode(true);
    if (runProperties.CharacterScale != null) styleRunProperties.CharacterScale = (CharacterScale)runProperties.CharacterScale.CloneNode(true);
    if (runProperties.Color != null) styleRunProperties.Color = (Color)runProperties.Color.CloneNode(true);
    if (runProperties.DoubleStrike != null) styleRunProperties.DoubleStrike = (DoubleStrike)runProperties.DoubleStrike.CloneNode(true);
    if (runProperties.EastAsianLayout != null) styleRunProperties.EastAsianLayout = (EastAsianLayout)runProperties.EastAsianLayout.CloneNode(true);
    if (runProperties.Emboss != null) styleRunProperties.Emboss = (Emboss)runProperties.Emboss.CloneNode(true);
    if (runProperties.Emphasis != null) styleRunProperties.Emphasis = (Emphasis)runProperties.Emphasis.CloneNode(true);
    if (runProperties.FitText != null) styleRunProperties.FitText = (FitText)runProperties.FitText.CloneNode(true);
    if (runProperties.FontSize != null) styleRunProperties.FontSize = (FontSize)runProperties.FontSize.CloneNode(true);
    if (runProperties.FontSizeComplexScript != null) styleRunProperties.FontSizeComplexScript = (FontSizeComplexScript)runProperties.FontSizeComplexScript.CloneNode(true);
    if (runProperties.Imprint != null) styleRunProperties.Imprint = (Imprint)runProperties.Imprint.CloneNode(true);
    if (runProperties.Italic != null) styleRunProperties.Italic = (Italic)runProperties.Italic.CloneNode(true);
    if (runProperties.ItalicComplexScript != null) styleRunProperties.ItalicComplexScript = (ItalicComplexScript)runProperties.ItalicComplexScript.CloneNode(true);
    if (runProperties.Kern != null) styleRunProperties.Kern = (Kern)runProperties.Kern.CloneNode(true);
    if (runProperties.Languages != null) styleRunProperties.Languages = (Languages)runProperties.Languages.CloneNode(true);
    if (runProperties.NoProof != null) styleRunProperties.NoProof = (NoProof)runProperties.NoProof.CloneNode(true);
    if (runProperties.Outline != null) styleRunProperties.Outline = (Outline)runProperties.Outline.CloneNode(true);
    if (runProperties.Position != null) styleRunProperties.Position = (Position)runProperties.Position.CloneNode(true);
    if (runProperties.RunFonts != null) styleRunProperties.RunFonts = (RunFonts)runProperties.RunFonts.CloneNode(true);
    if (runProperties.Shading != null) styleRunProperties.Shading = (Shading)runProperties.Shading.CloneNode(true);
    if (runProperties.Shadow != null) styleRunProperties.Shadow = (Shadow)runProperties.Shadow.CloneNode(true);
    if (runProperties.SmallCaps != null) styleRunProperties.SmallCaps = (SmallCaps)runProperties.SmallCaps.CloneNode(true);
    if (runProperties.SnapToGrid != null) styleRunProperties.SnapToGrid = (SnapToGrid)runProperties.SnapToGrid.CloneNode(true);
    if (runProperties.Spacing != null) styleRunProperties.Spacing = (Spacing)runProperties.Spacing.CloneNode(true);
    if (runProperties.SpecVanish != null) styleRunProperties.SpecVanish = (SpecVanish)runProperties.SpecVanish.CloneNode(true);
    if (runProperties.Strike != null) styleRunProperties.Strike = (Strike)runProperties.Strike.CloneNode(true);
    if (runProperties.TextEffect != null) styleRunProperties.TextEffect = (TextEffect)runProperties.TextEffect.CloneNode(true);
    if (runProperties.Underline != null) styleRunProperties.Underline = (Underline)runProperties.Underline.CloneNode(true);
    if (runProperties.Vanish != null) styleRunProperties.Vanish = (Vanish)runProperties.Vanish.CloneNode(true);
    if (runProperties.VerticalTextAlignment != null) styleRunProperties.VerticalTextAlignment = (VerticalTextAlignment)runProperties.VerticalTextAlignment.CloneNode(true);
    if (runProperties.WebHidden != null) styleRunProperties.WebHidden = (WebHidden)runProperties.WebHidden.CloneNode(true);

    return styleRunProperties;
  }

  /// <summary>
  /// Converts a <see cref="StyleRunProperties"/> element to a <see cref="RunProperties"/> element.
  /// </summary>
  /// <param name="styleRunProperties">source properties to convert</param>
  /// <returns>target properties</returns>
  public static RunProperties ToRunProperties(this StyleRunProperties styleRunProperties)
  {
    var runProperties = new RunProperties();
    if (styleRunProperties.Bold != null) runProperties.Bold = (Bold)styleRunProperties.Bold.CloneNode(true);
    if (styleRunProperties.BoldComplexScript != null) runProperties.BoldComplexScript = (BoldComplexScript)styleRunProperties.BoldComplexScript.CloneNode(true);
    if (styleRunProperties.Border != null) runProperties.Border = (Border)styleRunProperties.Border.CloneNode(true);
    if (styleRunProperties.Caps != null) runProperties.Caps = (Caps)styleRunProperties.Caps.CloneNode(true);
    if (styleRunProperties.CharacterScale != null) runProperties.CharacterScale = (CharacterScale)styleRunProperties.CharacterScale.CloneNode(true);
    if (styleRunProperties.Color != null) runProperties.Color = (Color)styleRunProperties.Color.CloneNode(true);
    if (styleRunProperties.DoubleStrike != null) runProperties.DoubleStrike = (DoubleStrike)styleRunProperties.DoubleStrike.CloneNode(true);
    if (styleRunProperties.EastAsianLayout != null) runProperties.EastAsianLayout = (EastAsianLayout)styleRunProperties.EastAsianLayout.CloneNode(true);
    if (styleRunProperties.Emboss != null) runProperties.Emboss = (Emboss)styleRunProperties.Emboss.CloneNode(true);
    if (styleRunProperties.Emphasis != null) runProperties.Emphasis = (Emphasis)styleRunProperties.Emphasis.CloneNode(true);
    if (styleRunProperties.FitText != null) runProperties.FitText = (FitText)styleRunProperties.FitText.CloneNode(true);
    if (styleRunProperties.FontSize != null) runProperties.FontSize = (FontSize)styleRunProperties.FontSize.CloneNode(true);
    if (styleRunProperties.FontSizeComplexScript != null) runProperties.FontSizeComplexScript = (FontSizeComplexScript)styleRunProperties.FontSizeComplexScript.CloneNode(true);
    if (styleRunProperties.Imprint != null) runProperties.Imprint = (Imprint)styleRunProperties.Imprint.CloneNode(true);
    if (styleRunProperties.Italic != null) runProperties.Italic = (Italic)styleRunProperties.Italic.CloneNode(true);
    if (styleRunProperties.ItalicComplexScript != null) runProperties.ItalicComplexScript = (ItalicComplexScript)styleRunProperties.ItalicComplexScript.CloneNode(true);
    if (styleRunProperties.Kern != null) runProperties.Kern = (Kern)styleRunProperties.Kern.CloneNode(true);
    if (styleRunProperties.Languages != null) runProperties.Languages = (Languages)styleRunProperties.Languages.CloneNode(true);
    if (styleRunProperties.NoProof != null) runProperties.NoProof = (NoProof)styleRunProperties.NoProof.CloneNode(true);
    if (styleRunProperties.Outline != null) runProperties.Outline = (Outline)styleRunProperties.Outline.CloneNode(true);
    if (styleRunProperties.Position != null) runProperties.Position = (Position)styleRunProperties.Position.CloneNode(true);
    if (styleRunProperties.RunFonts != null) runProperties.RunFonts = (RunFonts)styleRunProperties.RunFonts.CloneNode(true);
    if (styleRunProperties.Shading != null) runProperties.Shading = (Shading)styleRunProperties.Shading.CloneNode(true);
    if (styleRunProperties.Shadow != null) runProperties.Shadow = (Shadow)styleRunProperties.Shadow.CloneNode(true);
    if (styleRunProperties.SmallCaps != null) runProperties.SmallCaps = (SmallCaps)styleRunProperties.SmallCaps.CloneNode(true);
    if (styleRunProperties.SnapToGrid != null) runProperties.SnapToGrid = (SnapToGrid)styleRunProperties.SnapToGrid.CloneNode(true);
    if (styleRunProperties.Spacing != null) runProperties.Spacing = (Spacing)styleRunProperties.Spacing.CloneNode(true);
    if (styleRunProperties.SpecVanish != null) runProperties.SpecVanish = (SpecVanish)styleRunProperties.SpecVanish.CloneNode(true);
    if (styleRunProperties.Strike != null) runProperties.Strike = (Strike)styleRunProperties.Strike.CloneNode(true);
    if (styleRunProperties.TextEffect != null) runProperties.TextEffect = (TextEffect)styleRunProperties.TextEffect.CloneNode(true);
    if (styleRunProperties.Underline != null) runProperties.Underline = (Underline)styleRunProperties.Underline.CloneNode(true);
    if (styleRunProperties.Vanish != null) runProperties.Vanish = (Vanish)styleRunProperties.Vanish.CloneNode(true);
    if (styleRunProperties.VerticalTextAlignment != null) runProperties.VerticalTextAlignment = (VerticalTextAlignment)styleRunProperties.VerticalTextAlignment.CloneNode(true);
    if (styleRunProperties.WebHidden != null) runProperties.WebHidden = (WebHidden)styleRunProperties.WebHidden.CloneNode(true);
    return runProperties;
  }

  /// <summary>
  /// Converts a <see cref="RunProperties"/> element to a <see cref="NumberingSymbolRunProperties"/> element.
  /// </summary>
  /// <param name="runProperties">source properties to convert</param>
  /// <returns>target properties</returns>
  public static NumberingSymbolRunProperties ToNumberingSymbolRunProperties(this RunProperties runProperties)
  {
    var symbolRunProperties = new NumberingSymbolRunProperties();
    if (runProperties.Bold != null) symbolRunProperties.Bold = (Bold)runProperties.Bold.CloneNode(true);
    if (runProperties.BoldComplexScript != null) symbolRunProperties.BoldComplexScript = (BoldComplexScript)runProperties.BoldComplexScript.CloneNode(true);
    if (runProperties.Border != null) symbolRunProperties.Border = (Border)runProperties.Border.CloneNode(true);
    if (runProperties.Caps != null) symbolRunProperties.Caps = (Caps)runProperties.Caps.CloneNode(true);
    if (runProperties.CharacterScale != null) symbolRunProperties.CharacterScale = (CharacterScale)runProperties.CharacterScale.CloneNode(true);
    if (runProperties.Color != null) symbolRunProperties.Color = (Color)runProperties.Color.CloneNode(true);
    if (runProperties.DoubleStrike != null) symbolRunProperties.DoubleStrike = (DoubleStrike)runProperties.DoubleStrike.CloneNode(true);
    if (runProperties.EastAsianLayout != null) symbolRunProperties.EastAsianLayout = (EastAsianLayout)runProperties.EastAsianLayout.CloneNode(true);
    if (runProperties.Emboss != null) symbolRunProperties.Emboss = (Emboss)runProperties.Emboss.CloneNode(true);
    if (runProperties.Emphasis != null) symbolRunProperties.Emphasis = (Emphasis)runProperties.Emphasis.CloneNode(true);
    if (runProperties.FitText != null) symbolRunProperties.FitText = (FitText)runProperties.FitText.CloneNode(true);
    if (runProperties.FontSize != null) symbolRunProperties.FontSize = (FontSize)runProperties.FontSize.CloneNode(true);
    if (runProperties.FontSizeComplexScript != null) symbolRunProperties.FontSizeComplexScript = (FontSizeComplexScript)runProperties.FontSizeComplexScript.CloneNode(true);
    if (runProperties.Imprint != null) symbolRunProperties.Imprint = (Imprint)runProperties.Imprint.CloneNode(true);
    if (runProperties.Italic != null) symbolRunProperties.Italic = (Italic)runProperties.Italic.CloneNode(true);
    if (runProperties.ItalicComplexScript != null) symbolRunProperties.ItalicComplexScript = (ItalicComplexScript)runProperties.ItalicComplexScript.CloneNode(true);
    if (runProperties.Kern != null) symbolRunProperties.Kern = (Kern)runProperties.Kern.CloneNode(true);
    if (runProperties.Languages != null) symbolRunProperties.Languages = (Languages)runProperties.Languages.CloneNode(true);
    if (runProperties.NoProof != null) symbolRunProperties.NoProof = (NoProof)runProperties.NoProof.CloneNode(true);
    if (runProperties.Outline != null) symbolRunProperties.Outline = (Outline)runProperties.Outline.CloneNode(true);
    if (runProperties.Position != null) symbolRunProperties.Position = (Position)runProperties.Position.CloneNode(true);
    if (runProperties.RunFonts != null) symbolRunProperties.RunFonts = (RunFonts)runProperties.RunFonts.CloneNode(true);
    if (runProperties.Shading != null) symbolRunProperties.Shading = (Shading)runProperties.Shading.CloneNode(true);
    if (runProperties.Shadow != null) symbolRunProperties.Shadow = (Shadow)runProperties.Shadow.CloneNode(true);
    if (runProperties.SmallCaps != null) symbolRunProperties.SmallCaps = (SmallCaps)runProperties.SmallCaps.CloneNode(true);
    if (runProperties.SnapToGrid != null) symbolRunProperties.SnapToGrid = (SnapToGrid)runProperties.SnapToGrid.CloneNode(true);
    if (runProperties.Spacing != null) symbolRunProperties.Spacing = (Spacing)runProperties.Spacing.CloneNode(true);
    if (runProperties.SpecVanish != null) symbolRunProperties.SpecVanish = (SpecVanish)runProperties.SpecVanish.CloneNode(true);
    if (runProperties.Strike != null) symbolRunProperties.Strike = (Strike)runProperties.Strike.CloneNode(true);
    if (runProperties.TextEffect != null) symbolRunProperties.TextEffect = (TextEffect)runProperties.TextEffect.CloneNode(true);
    if (runProperties.Underline != null) symbolRunProperties.Underline = (Underline)runProperties.Underline.CloneNode(true);
    if (runProperties.Vanish != null) symbolRunProperties.Vanish = (Vanish)runProperties.Vanish.CloneNode(true);
    if (runProperties.VerticalTextAlignment != null) symbolRunProperties.VerticalTextAlignment = (VerticalTextAlignment)runProperties.VerticalTextAlignment.CloneNode(true);
    if (runProperties.WebHidden != null) symbolRunProperties.WebHidden = (WebHidden)runProperties.WebHidden.CloneNode(true);

    return symbolRunProperties;
  }

  /// <summary>
  /// Converts a <see cref="NumberingSymbolRunProperties"/> element to a <see cref="RunProperties"/> element.
  /// </summary>
  /// <param name="styleRunProperties">source properties to convert</param>
  /// <returns>target properties</returns>
  public static RunProperties ToRunProperties(this NumberingSymbolRunProperties styleRunProperties)
  {
    var runProperties = new RunProperties();
    if (styleRunProperties.Bold != null) runProperties.Bold = (Bold)styleRunProperties.Bold.CloneNode(true);
    if (styleRunProperties.BoldComplexScript != null) runProperties.BoldComplexScript = (BoldComplexScript)styleRunProperties.BoldComplexScript.CloneNode(true);
    if (styleRunProperties.Border != null) runProperties.Border = (Border)styleRunProperties.Border.CloneNode(true);
    if (styleRunProperties.Caps != null) runProperties.Caps = (Caps)styleRunProperties.Caps.CloneNode(true);
    if (styleRunProperties.CharacterScale != null) runProperties.CharacterScale = (CharacterScale)styleRunProperties.CharacterScale.CloneNode(true);
    if (styleRunProperties.Color != null) runProperties.Color = (Color)styleRunProperties.Color.CloneNode(true);
    if (styleRunProperties.DoubleStrike != null) runProperties.DoubleStrike = (DoubleStrike)styleRunProperties.DoubleStrike.CloneNode(true);
    if (styleRunProperties.EastAsianLayout != null) runProperties.EastAsianLayout = (EastAsianLayout)styleRunProperties.EastAsianLayout.CloneNode(true);
    if (styleRunProperties.Emboss != null) runProperties.Emboss = (Emboss)styleRunProperties.Emboss.CloneNode(true);
    if (styleRunProperties.Emphasis != null) runProperties.Emphasis = (Emphasis)styleRunProperties.Emphasis.CloneNode(true);
    if (styleRunProperties.FitText != null) runProperties.FitText = (FitText)styleRunProperties.FitText.CloneNode(true);
    if (styleRunProperties.FontSize != null) runProperties.FontSize = (FontSize)styleRunProperties.FontSize.CloneNode(true);
    if (styleRunProperties.FontSizeComplexScript != null) runProperties.FontSizeComplexScript = (FontSizeComplexScript)styleRunProperties.FontSizeComplexScript.CloneNode(true);
    if (styleRunProperties.Imprint != null) runProperties.Imprint = (Imprint)styleRunProperties.Imprint.CloneNode(true);
    if (styleRunProperties.Italic != null) runProperties.Italic = (Italic)styleRunProperties.Italic.CloneNode(true);
    if (styleRunProperties.ItalicComplexScript != null) runProperties.ItalicComplexScript = (ItalicComplexScript)styleRunProperties.ItalicComplexScript.CloneNode(true);
    if (styleRunProperties.Kern != null) runProperties.Kern = (Kern)styleRunProperties.Kern.CloneNode(true);
    if (styleRunProperties.Languages != null) runProperties.Languages = (Languages)styleRunProperties.Languages.CloneNode(true);
    if (styleRunProperties.NoProof != null) runProperties.NoProof = (NoProof)styleRunProperties.NoProof.CloneNode(true);
    if (styleRunProperties.Outline != null) runProperties.Outline = (Outline)styleRunProperties.Outline.CloneNode(true);
    if (styleRunProperties.Position != null) runProperties.Position = (Position)styleRunProperties.Position.CloneNode(true);
    if (styleRunProperties.RunFonts != null) runProperties.RunFonts = (RunFonts)styleRunProperties.RunFonts.CloneNode(true);
    if (styleRunProperties.Shading != null) runProperties.Shading = (Shading)styleRunProperties.Shading.CloneNode(true);
    if (styleRunProperties.Shadow != null) runProperties.Shadow = (Shadow)styleRunProperties.Shadow.CloneNode(true);
    if (styleRunProperties.SmallCaps != null) runProperties.SmallCaps = (SmallCaps)styleRunProperties.SmallCaps.CloneNode(true);
    if (styleRunProperties.SnapToGrid != null) runProperties.SnapToGrid = (SnapToGrid)styleRunProperties.SnapToGrid.CloneNode(true);
    if (styleRunProperties.Spacing != null) runProperties.Spacing = (Spacing)styleRunProperties.Spacing.CloneNode(true);
    if (styleRunProperties.SpecVanish != null) runProperties.SpecVanish = (SpecVanish)styleRunProperties.SpecVanish.CloneNode(true);
    if (styleRunProperties.Strike != null) runProperties.Strike = (Strike)styleRunProperties.Strike.CloneNode(true);
    if (styleRunProperties.TextEffect != null) runProperties.TextEffect = (TextEffect)styleRunProperties.TextEffect.CloneNode(true);
    if (styleRunProperties.Underline != null) runProperties.Underline = (Underline)styleRunProperties.Underline.CloneNode(true);
    if (styleRunProperties.Vanish != null) runProperties.Vanish = (Vanish)styleRunProperties.Vanish.CloneNode(true);
    if (styleRunProperties.VerticalTextAlignment != null) runProperties.VerticalTextAlignment = (VerticalTextAlignment)styleRunProperties.VerticalTextAlignment.CloneNode(true);
    if (styleRunProperties.WebHidden != null) runProperties.WebHidden = (WebHidden)styleRunProperties.WebHidden.CloneNode(true);
    return runProperties;
  }

  /// <summary>
  /// Get statistics of properties used for text in the run. If the run does not contain font information, return default properties statistic.
  /// </summary>
  /// <param name="runProperties">Run properties which should have <c>RunFonts</c> and many have <c>Languages</c> elements</param>
  /// <param name="text">Fonts are chosen for each character of the text</param>
  /// <param name="defaultProperties">Default text properties used when there is no <c>RunFonts</c> element</param>
  public static ObjectStatistics<TextProperties> GetTextPropertiesStatistics(this DXW.RunProperties runProperties, string text, TextProperties? defaultProperties)
  {
    ObjectStatistics<TextProperties> statistics = new ObjectStatistics<TextProperties>();
    var runFonts = runProperties.RunFonts;
    if (runFonts != null)
    {
      var lang = runProperties.Languages;
      foreach (var ch in text)
      {
        var (font, script) = runProperties.GetFontAndScript(ch, lang);
        bool? isBold = runProperties.GetBold(script == ScriptType.ComplexScript);
        bool? isItalic = runProperties.GetItalic(script == ScriptType.ComplexScript);
        int? strikeThrough = runProperties.GetStrikeThrough();
        DXW.UnderlineValues? underline = runProperties.GetUnderline();
        DXO13W.Color? underlineColor = runProperties.GetUnderlineColor();
        int? fontSize = runProperties.GetFontSize(script == ScriptType.ComplexScript);
        var textProperties = new TextProperties
        {
          FontName = font,
          ScriptType = script,
          Bold = isBold,
          Italic = isItalic,
          StrikeThrough = strikeThrough,
          Underline = underline,
          UnderlineColor = underlineColor,
          FontSize = fontSize
        };
        statistics.Add(textProperties);
      }
    }
    return statistics;
  }

  /// <summary>
  /// Sets appropriate run properties to the values in text properties.
  /// </summary>
  /// <param name="runProperties"></param>
  /// <param name="value"></param>
  public static void SetTextProperties(this DXW.RunProperties runProperties, TextProperties value)
  {
    runProperties.RunFonts ??= new DXW.RunFonts();
    if (value.ScriptType == ScriptType.Ascii)
    {
      if (value.FontName != null)
        runProperties.RunFonts.Ascii = value.FontName;
    }
    else if (value.ScriptType == ScriptType.EastAsian)
    {
      if (value.FontName != null)
        runProperties.RunFonts.EastAsia = value.FontName;
      runProperties.RunFonts.Hint = new DX.EnumValue<FontTypeHintValues>(DXW.FontTypeHintValues.EastAsia);
    }
    else if (value.ScriptType == ScriptType.ComplexScript)
    {
      if (value.FontName != null)
        runProperties.RunFonts.ComplexScript = value.FontName;
      runProperties.RunFonts.Hint = new DX.EnumValue<FontTypeHintValues>(FontTypeHintValues.ComplexScript);
    }
    else
    {
      if (value.FontName != null)
        runProperties.RunFonts.HighAnsi = value.FontName;
    }
    if (value.FontSize != null)
      runProperties.SetFontSize(value.FontSize, value.ScriptType == ScriptType.ComplexScript);
    if (value.Bold != null)
      runProperties.SetBold(value.Bold, value.ScriptType == ScriptType.ComplexScript);
    if (value.Italic != null)
      runProperties.SetItalic(value.Italic, value.ScriptType == ScriptType.ComplexScript);
    if (value.StrikeThrough != null)
      runProperties.SetStrikeThrough(value.StrikeThrough);
    if (value.Underline != null)
      runProperties.SetUnderline(value.Underline, value.UnderlineColor);
  }

  /// <summary>
  /// Return Bold attribute from run properties.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="complexScript">determines if <c>Bold</c> or <c>BoldComplexScript</c> elements are examined</param>
  public static bool? GetBold(this DXW.RunProperties runProperties, bool complexScript)
  {
    if (complexScript)
    {
      if (runProperties.BoldComplexScript != null)
      {
        if (runProperties.BoldComplexScript.Val == DX.OnOffValue.FromBoolean(false))
          return false;
        else
          return true;
      }
    }
    else
    {
      if (runProperties.Bold != null)
      {
        if (runProperties.Bold.Val == DX.OnOffValue.FromBoolean(false))
          return false;
        else
          return true;
      }
    }
    return null;
  }

  /// <summary>
  /// If value is true, set bold to true, if value is false, set bold to false, if value is null, remove bold element.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="value">value to set</param>
  /// <param name="complexScript">determines if <c>Bold</c> or <c>BoldComplexScript</c> elements are set</param>
  public static void SetBold(this DXW.RunProperties runProperties, bool? value, bool complexScript)
  {
    if (value == true)
    {
      if (complexScript)
        runProperties.BoldComplexScript = new BoldComplexScript();
      else
        runProperties.Bold = new Bold();
    }
    else if (value == false)
    {
      if (complexScript)
        runProperties.BoldComplexScript = new BoldComplexScript { Val = DX.OnOffValue.FromBoolean(false) };
      else
        runProperties.Bold = new Bold { Val = DX.OnOffValue.FromBoolean(false) };
    }
    else runProperties.Bold = null;
  }

  /// <summary>
  /// Return Italic attribute from run properties.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="complexScript">determines if <c>Italic</c> or <c>ItalicComplexScript</c> elements are examined</param>
  public static bool? GetItalic(this DXW.RunProperties runProperties, bool complexScript)
  {
    if (complexScript)
    {
      if (runProperties.ItalicComplexScript != null)
      {
        if (runProperties.ItalicComplexScript.Val == DX.OnOffValue.FromBoolean(false))
          return false;
        else
          return true;
      }
    }
    else
    {
      if (runProperties.Italic != null)
      {
        if (runProperties.Italic.Val == DX.OnOffValue.FromBoolean(false))
          return false;
        else
          return true;
      }
    }
    return null;
  }

  /// <summary>
  /// If value is true, set italic to true, if value is false, set italic to false, if value is null, remove italic element.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="value">value to set</param>
  /// <param name="complexScript">determines if <c>Italic</c> or <c>ItalicComplexScript</c> elements are set</param>
  public static void SetItalic(this DXW.RunProperties runProperties, bool? value, bool complexScript)
  {
    if (value == true)
    {
      if (complexScript)
        runProperties.ItalicComplexScript = new ItalicComplexScript();
      else
        runProperties.Italic = new Italic();
    }
    else if (value == false)
    {
      if (complexScript)
        runProperties.ItalicComplexScript = new ItalicComplexScript { Val = DX.OnOffValue.FromBoolean(false) };
      else
        runProperties.Italic = new Italic { Val = DX.OnOffValue.FromBoolean(false) };
    }
    else runProperties.Italic = null;
  }

  /// <summary>
  /// Return StrikeThrough attribute from run properties.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  public static int? GetStrikeThrough(this DXW.RunProperties runProperties)
  {
    if (runProperties.DoubleStrike != null)
      if (runProperties.DoubleStrike.Val != DX.OnOffValue.FromBoolean(false))
        return 2;
    if (runProperties.Strike != null)
      if (runProperties.Strike.Val != DX.OnOffValue.FromBoolean(false))
        return 2;
      else
        return 0;
    return null;
  }

  /// <summary>
  /// If value is 1, set <c>Strike</c> element, if value is 2, set <c>DoubleStrike</c>.
  /// In other case remove both <c>Strike</c> and <c>DoubleStrike</c> elements.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="value">value to set</param>
  public static void SetStrikeThrough(this DXW.RunProperties runProperties, int? value)
  {
    if (value == 1) runProperties.Strike = new Strike();
    else if (value == 2) runProperties.DoubleStrike = new DoubleStrike();
    else
    {
      runProperties.Strike = null;
      runProperties.DoubleStrike = null;
    }
  }


  /// <summary>
  /// Return Underline attribute from run properties.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  public static UnderlineValues? GetUnderline(this DXW.RunProperties runProperties)
  {
    if (runProperties.Underline != null)
      return runProperties.Underline.Val?.Value;
    return null;
  }

  /// <summary>
  /// Return Underline attribute from run properties.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  public static DXO13W.Color? GetUnderlineColor(this DXW.RunProperties runProperties)
  {
    if (runProperties.Underline != null)
      return runProperties.Underline.GetColor();
    return null;
  }

  /// <summary>
  /// If value not null, set underline.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="value">underline value to set</param>
  /// <param name="color">underline color to set</param>
  public static void SetUnderline(this DXW.RunProperties runProperties, DXW.UnderlineValues? value, DXO13W.Color? color)
  {
    if (value != null)
    {
      runProperties.Underline = new DXW.Underline { Val = value };
      if (color != null)
      { 
        runProperties.Underline.Color = color.Val;
        runProperties.Underline.ThemeColor = color.ThemeColor;
        runProperties.Underline.ThemeShade = color.ThemeShade;
        runProperties.Underline.ThemeTint = color.ThemeTint;
      }
    }
  }

  /// <summary>
  /// Return font size from run properties.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="complexScript">determines if <c>FontSize</c> or <c>FontSizeComplexScript</c> elements are examined</param>
  public static int? GetFontSize(this DXW.RunProperties runProperties, bool complexScript)
  {
    if (complexScript)
    {
      if (runProperties.FontSizeComplexScript != null)
      {
        if (int.TryParse(runProperties.FontSizeComplexScript.Val, out var size))
          return size;
        return null;
      }
    }
    else
    {
      if (runProperties.FontSize != null)
      {
        if (int.TryParse(runProperties.FontSize.Val, out var size))
          return size;
        return null;
      }
    }
    return null;
  }

  /// <summary>
  /// Set font size from run properties.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="value">value to set</param>
  /// <param name="complexScript">determines if <c>FontSize</c> or <c>FontSizeComplexScript</c> elements are set</param>
  public static void SetFontSize(this DXW.RunProperties runProperties, int? value, bool complexScript)
  {
    if (complexScript)
    {
      if (value!=null)
        runProperties.FontSizeComplexScript = new DXW.FontSizeComplexScript { Val = value.ToString() };
      else
        runProperties.FontSizeComplexScript = null;
    }
    else
    {
      if (runProperties.FontSize != null)
      {
        if (value!=null)
          runProperties.FontSize.Val = value.ToString();
        else
          runProperties.FontSize = null;
      }
    }
  }

  /// <summary>
  /// Get statistics of fonts used for text in the run. If the run does not contain font information, return default font statistic.
  /// </summary>
  /// <param name="runProperties">Run properties which should have <c>RunFonts</c> and many have <c>Languages</c> elements</param>
  /// <param name="text">Fonts are chosen for each character of the text</param>
  /// <param name="defaultFont">Font name used when there is no <c>RunFonts</c> element</param>
  public static StringStatistics? GetRunFonts(this DXW.RunProperties runProperties, string text, string? defaultFont)
  {
    StringStatistics? fonts = null;
    var runFonts = runProperties.RunFonts;
    if (runFonts != null)
    {
      var lang = runProperties.Languages;
      foreach (var ch in text)
      {
        var font = runProperties.GetFontName(ch, lang);
        if (font != null)
        {
          fonts ??= new StringStatistics();
          fonts.Add(font);
        }
        else
        {
          if (defaultFont != null)
          {
            fonts ??= new StringStatistics();
            fonts.Add(defaultFont);
          }
        }
      }
    }
    if (fonts == null && defaultFont != null)
    {
      fonts ??= new StringStatistics();
      fonts.Add(defaultFont, text.Length);
    }
    return fonts;
  }

  /// <summary>
  /// Get font name selected from runFonts for a character.
  /// </summary>
  /// <param name="runProperties">Processed run properties</param>
  /// <param name="ch">single character to evaluate font and strict</param>
  /// <param name="lang">Languages settings of the run</param>
  private static string? GetFontName(this DXW.RunProperties runProperties, char ch, DXW.Languages? lang)
  {
    return GetFontAndScript(runProperties, ch, lang).Item1;
  }

  /// <summary>
  /// Get font name and a script selected from runFonts for a character.
  /// </summary>
  /// <param name="runProperties"></param>
  /// <param name="ch"></param>
  /// <param name="lang"></param>
  public static (string?, ScriptType?) GetFontAndScript(this DXW.RunProperties runProperties, char ch, DXW.Languages? lang)
  {
    var runFonts = runProperties.RunFonts;
    if (runFonts != null)
    {
      var result = runFonts.GetFontAndScript(ch, lang);
      if (result.Item2 == ScriptType.EastAsian && runFonts.Hint?.Value == FontTypeHintValues.EastAsia)
        return result;
      var cs = runProperties.ComplexScript != null || runProperties.RightToLeftText != null;
      if (cs && runFonts.Hint?.Value == FontTypeHintValues.ComplexScript)
        return (runFonts.ComplexScript, ScriptType.ComplexScript);
      return result;
    }
    return (null, null);
  }

  /// <summary>
  /// Get font name and a script selected from runFonts for a character.
  /// </summary>
  /// <param name="runFonts"></param>
  /// <param name="ch"></param>
  /// <param name="lang"></param>
  private static (string?, ScriptType?) GetFontAndScript(this DXW.RunFonts runFonts, char ch, DXW.Languages? lang)
  {
    var unicodeRange = UnicodeTools.GetUnicodeRange(ch);
    if (unicodeRange == UnicodeRange.BasicLatin)
      return (runFonts.Ascii, ScriptType.Ascii);
    if (unicodeRange == UnicodeRange.Latin1Supplement)
    {
      if (ch == 0xA1 || ch == 0xA4 || ch >= 0xA7 && ch <= 0xA8 ||
          ch == 0xAA || ch == 0xAD || ch == 0xAF || ch >= 0xB0 && ch <= 0xB4 ||
          ch >= 0xB6 && ch <= 0xBA || ch >= 0xBC && ch <= 0xBF || ch == 0xD7 || ch == 0xF7)
      {
        var eastAsiaFont = runFonts.GetEastAsiaFontName();
        if (eastAsiaFont != null)
          return (eastAsiaFont, ScriptType.EastAsian);
      }
      if (runFonts.Hint?.Value == FontTypeHintValues.EastAsia && lang?.EastAsia?.Value?.StartsWith("zh") == true)
      {
        if (ch >= 0xE0 && ch <= 0xE1 ||
            ch == 0xE8 || ch == 0xEA || ch >= 0xEC && ch <= 0xED ||
            ch >= 0xF2 && ch <= 0xF3 || ch >= 0xF9 && ch <= 0xFA || ch == 0xFC)
        {
          var eastAsiaFont = runFonts.GetEastAsiaFontName();
          if (eastAsiaFont != null)
            return (eastAsiaFont, ScriptType.EastAsian);
        }
      }
      return (runFonts.HighAnsi, ScriptType.HighAnsi);
    }
    if (unicodeRange == UnicodeRange.LatinExtendedA || unicodeRange == UnicodeRange.LatinExtendedB || unicodeRange == UnicodeRange.IPAExtensions)
    {
      if (runFonts.Hint?.Value == FontTypeHintValues.EastAsia && lang?.EastAsia?.Value?.StartsWith("zh") == true)
      {
        if (ch >= 0xE0 && ch <= 0xE1 ||
            ch == 0xE8 || ch == 0xEA || ch >= 0xEC && ch <= 0xED ||
            ch >= 0xF2 && ch <= 0xF3 || ch >= 0xF9 && ch <= 0xFA || ch == 0xFC)
        {
          var eastAsiaFont = runFonts.GetEastAsiaFontName();
          if (eastAsiaFont == "Big5" || eastAsiaFont == "GB2312")
            return (eastAsiaFont, ScriptType.EastAsian);
        }
      }
      return (runFonts.HighAnsi, ScriptType.HighAnsi);
    }
    if (unicodeRange == UnicodeRange.SpacingModifierLetters || unicodeRange == UnicodeRange.CombiningDiacriticalMarks ||
        unicodeRange == UnicodeRange.GreekAndCoptic || unicodeRange == UnicodeRange.Cyrillic)
    {
      if (runFonts.Hint?.Value == FontTypeHintValues.EastAsia)
      {
        var eastAsiaFont = runFonts.GetEastAsiaFontName();
        if (eastAsiaFont != null)
          return (eastAsiaFont, ScriptType.EastAsian);
      }
      return (runFonts.HighAnsi, ScriptType.HighAnsi);
    }
    if (unicodeRange == UnicodeRange.Hebrew || unicodeRange == UnicodeRange.Arabic ||
        unicodeRange == UnicodeRange.Syriac || unicodeRange == UnicodeRange.ArabicSupplement || unicodeRange == UnicodeRange.Thaana)
      return (runFonts.Ascii, ScriptType.Ascii);
    if (unicodeRange == UnicodeRange.HangulJamo)
    {
      var eastAsiaFont = runFonts.GetEastAsiaFontName();
      if (eastAsiaFont != null)
        return (eastAsiaFont, ScriptType.EastAsian);
    }
    if (unicodeRange == UnicodeRange.LatinExtendedAdditional)
    {
      if (runFonts.Hint?.Value == FontTypeHintValues.EastAsia && lang?.EastAsia?.Value?.StartsWith("zh") == true)
      {
        var eastAsiaFont = runFonts.GetEastAsiaFontName();
        if (eastAsiaFont != null)
          return (eastAsiaFont, ScriptType.EastAsian);
      }
      return (runFonts.HighAnsi, ScriptType.HighAnsi);
    }
    if (unicodeRange == UnicodeRange.GreekExtended)
    {
      return (runFonts.HighAnsi, ScriptType.HighAnsi);
    }
    if (unicodeRange == UnicodeRange.GeneralPunctuation || unicodeRange == UnicodeRange.SuperscriptsAndSubscripts ||
        unicodeRange == UnicodeRange.CurrencySymbols || unicodeRange == UnicodeRange.CombiningDiacriticalMarksForSymbols ||
        unicodeRange == UnicodeRange.LetterlikeSymbols || unicodeRange == UnicodeRange.NumberForms ||
        unicodeRange == UnicodeRange.Arrows || unicodeRange == UnicodeRange.MathematicalOperators ||
        unicodeRange == UnicodeRange.MiscellaneousTechnical || unicodeRange == UnicodeRange.ControlPictures ||
        unicodeRange == UnicodeRange.OpticalCharacterRecognition || unicodeRange == UnicodeRange.EnclosedAlphanumerics ||
        unicodeRange == UnicodeRange.BoxDrawing || unicodeRange == UnicodeRange.BlockElements ||
        unicodeRange == UnicodeRange.GeometricShapes || unicodeRange == UnicodeRange.MiscellaneousSymbols ||
        unicodeRange == UnicodeRange.Dingbats)
    {
      if (runFonts.Hint?.Value == FontTypeHintValues.EastAsia)
      {
        var eastAsiaFont = runFonts.GetEastAsiaFontName();
        if (eastAsiaFont != null)
          return (eastAsiaFont, ScriptType.EastAsian);
      }
      return (runFonts.HighAnsi, ScriptType.HighAnsi);
    }
    if (unicodeRange == UnicodeRange.CJKRadicalsSupplement || unicodeRange == UnicodeRange.KangxiRadicals ||
        unicodeRange == UnicodeRange.IdeographicDescriptionCharacters || unicodeRange == UnicodeRange.CJKSymbolsAndPunctuation ||
        unicodeRange == UnicodeRange.Hiragana || unicodeRange == UnicodeRange.Katakana ||
        unicodeRange == UnicodeRange.Bopomofo || unicodeRange == UnicodeRange.HangulCompatibilityJamo ||
        unicodeRange == UnicodeRange.Kanbun || unicodeRange == UnicodeRange.EnclosedCJKLettersAndMonths ||
        unicodeRange == UnicodeRange.CJKCompatibility || unicodeRange == UnicodeRange.CJKUnifiedIdeographs ||
        unicodeRange == UnicodeRange.YiSyllables || unicodeRange == UnicodeRange.YiRadicals ||
        unicodeRange == UnicodeRange.HangulSyllables || unicodeRange == UnicodeRange.HighSurrogates ||
        unicodeRange == UnicodeRange.HighPrivateUseSurrogates || unicodeRange == UnicodeRange.LowSurrogates ||
        unicodeRange == UnicodeRange.CJKCompatibilityIdeographs || unicodeRange == UnicodeRange.CJKCompatibilityForms ||
        unicodeRange == UnicodeRange.SmallFormVariants || unicodeRange == UnicodeRange.HalfwidthAndFullwidthForms)
    {
      var eastAsiaFont = runFonts.GetEastAsiaFontName();
      return (eastAsiaFont, ScriptType.EastAsian);
    }
    if (unicodeRange == UnicodeRange.PrivateUseArea)
    {
      if (runFonts.Hint?.Value == FontTypeHintValues.EastAsia)
      {
        var eastAsiaFont = runFonts.GetEastAsiaFontName();
        if (eastAsiaFont != null)
          return (eastAsiaFont, ScriptType.EastAsian);
        return (runFonts.HighAnsi, ScriptType.HighAnsi);
      }
    }
    if (unicodeRange == UnicodeRange.ArabicPresentationFormsA || unicodeRange == UnicodeRange.ArabicPresentationFormsB)
    {
      return (runFonts.Ascii, ScriptType.Ascii);
    }
    if (unicodeRange == UnicodeRange.AlphabeticPresentationForms)
    {
      if (ch >= 0xFB1D && ch <= 0xFB4F)
        return (runFonts.Ascii, ScriptType.Ascii);
      var eastAsiaFont = runFonts.GetEastAsiaFontName();
      if (eastAsiaFont != null)
        return (eastAsiaFont, ScriptType.EastAsian);
      return (runFonts.HighAnsi, ScriptType.HighAnsi);
    }
    return (null, null);
  }


}
