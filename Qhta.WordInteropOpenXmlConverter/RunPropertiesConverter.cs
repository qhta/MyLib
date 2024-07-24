using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using DocumentFormat.OpenXml;

using Qhta.OpenXmlTools;

using static Microsoft.Office.Interop.Word.WdLanguageID;
using static Microsoft.Office.Interop.Word.WdUnderline;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.LanguageConverter;
using static Qhta.WordInteropOpenXmlConverter.PropertiesConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public class RunPropertiesConverter
{

  private readonly Word.Font? defaultFont;
  public readonly ThemeTools? themeTools;
  private readonly Word.Style? _defaultStyle;

  public RunPropertiesConverter()
  {

  }

  public RunPropertiesConverter(Word.Style defaultStyle, ThemeTools themeTools)
  {
    _defaultStyle = defaultStyle;
    defaultFont = defaultStyle.Font;
    this.themeTools = themeTools;
  }

  public W.StyleRunProperties ConvertStyleFont(Word.Style wordStyle)
  {
    var wordFont = wordStyle.Font;
    var xRunProperties = ConvertFont(wordFont);
    #region lang props

    var langProps = new W.Languages();
    bool addLangProps = false;

    if (wordStyle.LanguageID != 0 && wordStyle.LanguageID != _defaultStyle?.LanguageID)
    {
      langProps.Val = LanguageIdToBcp47Tag(wordStyle.LanguageID);
      addLangProps = true;
    }
    if (wordStyle.LanguageIDFarEast != wdNoProofing
        && wordStyle.LanguageIDFarEast != _defaultStyle?.LanguageIDFarEast)
    {
      langProps.EastAsia = LanguageIdToBcp47Tag(wordStyle.LanguageIDFarEast);
      addLangProps = true;
    }

    //Not supported in Microsoft.Office.Interop.Word v.15
    //try
    //{
    //  if (wordStyle.LanguageIDOther != WdLanguageID.wdNoProofing)
    //    langProps.Bidi = LanguageIdToBcp47Tag(wordStyle.LanguageIDOther);
    //} catch (COMException) { }

    if (addLangProps)
      xRunProperties.Languages = langProps;

    #endregion lang props

    if (wordStyle.NoProofing != 0)
      xRunProperties.NoProof = new W.NoProof();
    return xRunProperties.ToStyleRunProperties();
  }

  public W.RunProperties ConvertFont(Word.Font wordFont)
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xRunProps = new W.RunProperties();

    var runFonts = new W.RunFonts();
    W.FontTypeHintValues? hintValue = null;
    int definedFonts = 0;
    string? fontName;
    if (themeTools != null && themeTools.TryGetThemeFont(wordFont.NameFarEast, out var eaThemeFont))
      runFonts.EastAsiaTheme = new EnumValue<W.ThemeFontValues>(eaThemeFont);
    else
    {
      fontName = NotEmptyString(wordFont.NameFarEast);
      if (fontName != null)
      {
        runFonts.EastAsia = fontName;
        hintValue = W.FontTypeHintValues.EastAsia; 
        definedFonts++;
      }
    }

    if (themeTools != null && themeTools.TryGetThemeFont(wordFont.NameBi, out var csThemeFont))
      runFonts.ComplexScriptTheme = new EnumValue<W.ThemeFontValues>(csThemeFont);
    else
    {
      fontName = NotEmptyString(wordFont.NameBi);
      if (fontName != null)
      {
        runFonts.ComplexScript = fontName;
        definedFonts++;
        hintValue = W.FontTypeHintValues.ComplexScript;
      }
    }
    if (themeTools != null && themeTools.TryGetThemeFont(wordFont.Name, out var _ThemeFont))
      runFonts.HighAnsiTheme = new EnumValue<W.ThemeFontValues>(_ThemeFont);
    else
    {
      fontName = NotEmptyString(wordFont.Name);
      if (fontName != null)
      {
        runFonts.HighAnsi = fontName;
        definedFonts++;
        hintValue = W.FontTypeHintValues.Default;
      }
    }

    if (themeTools != null && themeTools.TryGetThemeFont(wordFont.NameAscii, out var aThemeFont))
      runFonts.AsciiTheme = new EnumValue<W.ThemeFontValues>(aThemeFont);
    else
    {
      fontName = NotEmptyString(wordFont.NameAscii);
      if (fontName != null)
      {
        runFonts.Ascii = fontName;
        definedFonts++;
      }
    }

    if (definedFonts<4 && hintValue!=null)
      runFonts.Hint = new EnumValue<W.FontTypeHintValues>(hintValue);
    xRunProps.RunFonts = runFonts;


    xRunProps.FontSize = GetFontSizeTypeElement<W.FontSize>(wordFont.Size, defaultFont?.Size);
    xRunProps.FontSizeComplexScript = GetFontSizeTypeElement<W.FontSizeComplexScript>(wordFont.SizeBi, defaultFont?.SizeBi);

    xRunProps.Bold = GetOnOffTypeElement<W.Bold>(wordFont.Bold, defaultFont?.Bold);
    xRunProps.BoldComplexScript = GetOnOffTypeElement<W.BoldComplexScript>(wordFont.BoldBi, defaultFont?.BoldBi);
    xRunProps.Italic = GetOnOffTypeElement<W.Italic>(wordFont.Italic, defaultFont?.Italic);
    xRunProps.ItalicComplexScript = GetOnOffTypeElement<W.ItalicComplexScript>(wordFont.ItalicBi, defaultFont?.ItalicBi);
    xRunProps.Strike = GetOnOffTypeElement<W.Strike>(wordFont.StrikeThrough, defaultFont?.StrikeThrough);
    xRunProps.DoubleStrike = GetOnOffTypeElement<W.DoubleStrike>(wordFont.DoubleStrikeThrough, defaultFont?.DoubleStrikeThrough);
    xRunProps.Caps = GetOnOffTypeElement<W.Caps>(wordFont.AllCaps, defaultFont?.AllCaps);
    xRunProps.SmallCaps = GetOnOffTypeElement<W.SmallCaps>(wordFont.SmallCaps, defaultFont?.SmallCaps);

    if ((int)wordFont.Underline != (int)Word.WdConstants.wdUndefined && wordFont.Underline != defaultFont?.Underline)
    {
      var underline = wordFont.Underline;
      var xUnderline = new W.Underline();
      W.UnderlineValues underlineValue = underline switch
      {
        wdUnderlineNone => W.UnderlineValues.None,
        wdUnderlineSingle => W.UnderlineValues.Single,
        wdUnderlineWords => W.UnderlineValues.Words,
        wdUnderlineDouble => W.UnderlineValues.Double,
        wdUnderlineDotted => W.UnderlineValues.Dotted,
        wdUnderlineThick => W.UnderlineValues.Thick,
        wdUnderlineDash => W.UnderlineValues.Dash,
        wdUnderlineDotDash => W.UnderlineValues.DotDash,
        wdUnderlineDotDotDash => W.UnderlineValues.DotDotDash,
        wdUnderlineWavy => W.UnderlineValues.Wave,
        wdUnderlineDottedHeavy => W.UnderlineValues.DottedHeavy,
        wdUnderlineDashHeavy => W.UnderlineValues.DashedHeavy,
        wdUnderlineDotDashHeavy => W.UnderlineValues.DashDotHeavy,
        wdUnderlineDotDotDashHeavy => W.UnderlineValues.DashDotDotHeavy,
        wdUnderlineWavyHeavy => W.UnderlineValues.WavyHeavy,
        _ => throw new ArgumentOutOfRangeException(nameof(underline), underline, null)
      };
      xUnderline.Val = new EnumValue<W.UnderlineValues>(underlineValue);
      xRunProps.Underline = xUnderline;
    }

    #region color props
    //// Not implemented in Office15
    //Word.ColorFormat? colorFormat = null;
    //try
    //{
    //  colorFormat = wordFont.TextColor;
    //}
    //catch (COMException) { }
    var xColor = ConvertColor(wordFont.Color, wordFont.ColorIndex, null);
    if (xColor != null)
      xRunProps.Color = xColor;
    #endregion color props

    xRunProps.Vanish = GetOnOffTypeElement<W.Vanish>(wordFont.Hidden, defaultFont?.Hidden);
    xRunProps.Emboss = GetOnOffTypeElement<W.Emboss>(wordFont.Emboss, defaultFont?.Emboss);
    xRunProps.Imprint = GetOnOffTypeElement<W.Imprint>(wordFont.Engrave, defaultFont?.Engrave);
    xRunProps.Outline = GetOnOffTypeElement<W.Outline>(wordFont.Outline, defaultFont?.Outline);
    xRunProps.Shadow = GetOnOffTypeElement<W.Shadow>(wordFont.Shadow, defaultFont?.Shadow);

    try
    {
      if (wordFont.Kerning != 0 && wordFont.Kerning != defaultFont?.Kerning)
        xRunProps.Kern = GetFontSizeTypeElement<W.Kern>(wordFont.Kerning, defaultFont?.Kerning);
      xRunProps.Position = GetStringValTypeElement<W.Position>(wordFont.Position, defaultFont?.Position);
      if (wordFont.Spacing != 0)
        xRunProps.Spacing = GetIntValTypeElement<W.Spacing>(wordFont.Spacing, defaultFont?.Spacing);
      xRunProps.VerticalTextAlignment =
        GetVerticalTextAlignment(W.VerticalPositionValues.Superscript, wordFont.Superscript, defaultFont?.Superscript) ??
        GetVerticalTextAlignment(W.VerticalPositionValues.Subscript, wordFont.Subscript, defaultFont?.Subscript);
    }
    catch (Exception e)
    {
      Debug.WriteLine(e);
      throw;
    }
    return xRunProps;
  }
}
