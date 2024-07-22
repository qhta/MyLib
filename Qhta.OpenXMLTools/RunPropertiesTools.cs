namespace Qhta.OpenXmlTools;
using DocumentFormat.OpenXml.Wordprocessing;

public static class RunPropertiesTools
{
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
}
