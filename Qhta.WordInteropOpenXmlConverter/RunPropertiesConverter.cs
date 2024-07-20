using System;

using DocumentFormat.OpenXml;

using Microsoft.Office.Interop.Word;

using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.LanguageConverter;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public class RunPropertiesConverter(Word.Style defaultStyle, ThemeTools themeTools)
{
  private readonly Word.Font defaultFont = defaultStyle.Font;
  public readonly ThemeTools themeTools = themeTools;

  public W.RunProperties CreateRunProperties(Word.Style wordStyle)
  {
    var styleFont = wordStyle.Font;
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xRunProps = new W.RunProperties();

    var runFonts = new W.RunFonts( );

    if (themeTools.TryGetThemeFont(styleFont.Name, out var _ThemeFont))
      runFonts.HighAnsiTheme = new EnumValue<W.ThemeFontValues>(_ThemeFont);
    else
      runFonts.HighAnsi = styleFont.Name;
    
    if (themeTools.TryGetThemeFont(styleFont.NameAscii, out var aThemeFont))
      runFonts.AsciiTheme = new EnumValue<W.ThemeFontValues>(aThemeFont);
    else
      runFonts.Ascii = styleFont.NameAscii;

    if (themeTools.TryGetThemeFont(styleFont.NameBi, out var csThemeFont))
      runFonts.ComplexScriptTheme = new EnumValue<W.ThemeFontValues>(csThemeFont);
    else
      runFonts.ComplexScript = styleFont.NameBi;

    if (themeTools.TryGetThemeFont(styleFont.NameFarEast, out var eaThemeFont))
      runFonts.EastAsiaTheme = new EnumValue<W.ThemeFontValues>(eaThemeFont);
    else
      runFonts.EastAsia = styleFont.NameFarEast;

    xRunProps.RunFonts = runFonts;


    if (styleFont.Size != 0 && styleFont.Size != defaultFont.Size)
      xRunProps.FontSize = new W.FontSize { Val = FontSizeToHps(styleFont.Size).ToString() };
    if (styleFont.SizeBi != 0 && styleFont.SizeBi != defaultFont.SizeBi)
      xRunProps.FontSizeComplexScript = new W.FontSizeComplexScript
      { Val = FontSizeToHps(styleFont.SizeBi).ToString() };

    if (styleFont.Bold != defaultFont.Bold)
      xRunProps.Bold = new W.Bold { Val = OnOffValue.FromBoolean(styleFont.Bold != 0) };
    if (styleFont.BoldBi != 0)
      xRunProps.BoldComplexScript = new W.BoldComplexScript();
    if (styleFont.Italic != 0)
      xRunProps.Italic = new W.Italic();
    if (styleFont.ItalicBi != 0)
      xRunProps.ItalicComplexScript = new W.ItalicComplexScript();
    if (styleFont.Underline != 0)
    {
      var wUnderline = new W.Underline();
      var underline = styleFont.Underline;
      W.UnderlineValues underlineValue = underline switch
      {
        WdUnderline.wdUnderlineNone => W.UnderlineValues.None,
        WdUnderline.wdUnderlineSingle => W.UnderlineValues.Single,
        WdUnderline.wdUnderlineWords => W.UnderlineValues.Words,
        WdUnderline.wdUnderlineDouble => W.UnderlineValues.Double,
        WdUnderline.wdUnderlineDotted => W.UnderlineValues.Dotted,
        WdUnderline.wdUnderlineThick => W.UnderlineValues.Thick,
        WdUnderline.wdUnderlineDash => W.UnderlineValues.Dash,
        WdUnderline.wdUnderlineDotDash => W.UnderlineValues.DotDash,
        WdUnderline.wdUnderlineDotDotDash => W.UnderlineValues.DotDotDash,
        WdUnderline.wdUnderlineWavy => W.UnderlineValues.Wave,
        WdUnderline.wdUnderlineDottedHeavy => W.UnderlineValues.DottedHeavy,
        WdUnderline.wdUnderlineDashHeavy => W.UnderlineValues.DashedHeavy,
        WdUnderline.wdUnderlineDotDashHeavy => W.UnderlineValues.DashDotHeavy,
        WdUnderline.wdUnderlineDotDotDashHeavy => W.UnderlineValues.DashDotDotHeavy,
        WdUnderline.wdUnderlineWavyHeavy => W.UnderlineValues.WavyHeavy,
        _ => throw new ArgumentOutOfRangeException(nameof(underline), underline, null)
      };
      xRunProps.Underline = wUnderline;
    }
    if (styleFont.StrikeThrough != 0)
      xRunProps.Strike = new W.Strike();
    if (styleFont.DoubleStrikeThrough != 0)
      xRunProps.DoubleStrike = new W.DoubleStrike();
    if (styleFont.AllCaps != 0)
      xRunProps.Caps = new W.Caps();
    if (styleFont.SmallCaps != 0)
      xRunProps.SmallCaps = new W.SmallCaps();

    #region color props
    Word.ColorFormat? colorFormat = null;
    try
    {
      colorFormat = styleFont.TextColor;
    }
    catch { }
    var xColor = ConvertColor(styleFont.Color, styleFont.ColorIndex, colorFormat);
    xRunProps.Color = xColor;
    #endregion color props

    #region lang props

    var langProps = new W.Languages();
    bool addLangProps = false;

    if (wordStyle.LanguageID != 0 && wordStyle.LanguageID != defaultStyle.LanguageID)
    {
      langProps.Val = LanguageIdToBcp47Tag(wordStyle.LanguageID);
      addLangProps = true;
    }
    if (wordStyle.LanguageIDFarEast != WdLanguageID.wdNoProofing
        && wordStyle.LanguageIDFarEast != defaultStyle.LanguageIDFarEast)
    {
      langProps.EastAsia = LanguageIdToBcp47Tag(wordStyle.LanguageIDFarEast);
      addLangProps = true;
    }

    //Not supported in Microsoft.Office.Interop.Word v.15
    //try
    //{
    //  if (wordStyle.LanguageIDOther != WdLanguageID.wdNoProofing)
    //    langProps.Bidi = LanguageIdToBcp47Tag(wordStyle.LanguageIDOther);
    //} catch { }

    if (addLangProps)
      xRunProps.Languages = langProps;

    #endregion lang props

    if (wordStyle.NoProofing != 0)
      xRunProps.NoProof = new W.NoProof();
    if (styleFont.Hidden != 0)
      xRunProps.Vanish = new W.Vanish();
    if (styleFont.Emboss != 0)
      xRunProps.Emboss = new W.Emboss();
    if (styleFont.Engrave != 0)
      xRunProps.Imprint = new W.Imprint();

    #region text borders
    try
    {

    }
    catch { }
    #endregion text borders

    if (styleFont.Outline != 0)
      xRunProps.Outline = new W.Outline();
    if (styleFont.Shadow != 0)
      xRunProps.Shadow = new W.Shadow();
    if (styleFont.Kerning != 0 && styleFont.Kerning != defaultFont.Kerning)
      xRunProps.Kern = new W.Kern { Val = (uint)FontSizeToHps(styleFont.Kerning) };
    if (styleFont.Position != 0)
      xRunProps.Position = new W.Position { Val = styleFont.Position.ToString() };
    if (styleFont.Spacing != 0)
      xRunProps.Spacing = new W.Spacing { Val = PointsToTwips(styleFont.Spacing) };
    if (styleFont.Superscript != 0)
      xRunProps.VerticalTextAlignment = new W.VerticalTextAlignment { Val = W.VerticalPositionValues.Superscript };
    if (styleFont.Subscript != 0)
      xRunProps.VerticalTextAlignment = new W.VerticalTextAlignment { Val = W.VerticalPositionValues.Subscript };
    return xRunProps;
  }


}
