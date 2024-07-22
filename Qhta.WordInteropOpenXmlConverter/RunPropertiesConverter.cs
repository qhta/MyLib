using System;
using System.Diagnostics;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Office.Interop.Word;
using Qhta.OpenXmlTools;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.LanguageConverter;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;

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
    if (wordStyle.LanguageIDFarEast != WdLanguageID.wdNoProofing
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
    //} catch { }

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

    if ((int)wordFont.Underline != (int)WdConstants.wdUndefined)
    {
      var underline = wordFont.Underline;
      var xUnderline = new W.Underline();
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
      xRunProps.Underline = xUnderline;
    }

    #region color props
    Word.ColorFormat? colorFormat = null;
    try
    {
      colorFormat = wordFont.TextColor;
    }
    catch { }
    var xColor = ConvertColor(wordFont.Color, wordFont.ColorIndex, colorFormat);
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

  private String? NotEmptyString(string value)
  {
    if (!string.IsNullOrEmpty(value))
      return value;
    return null;
  }

  private XType? GetOnOffTypeElement<XType>(int wordValue, int? defaultValue) where XType : W.OnOffType, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
      return new XType { Val = OnOffValue.FromBoolean(wordValue != 0) };
    return null;
  }

  private XType? GetStringValTypeElement<XType>(int wordValue, int? defaultValue) where XType : OpenXmlLeafElement, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var valProperty = typeof(XType).GetProperty("Val");
      var element = new XType();
      var valStr = wordValue.ToString();
      valProperty?.SetValue(element, valStr);
      return element;
    }
    return null;
  }

  private XType? GetIntValTypeElement<XType>(float wordValue, float? defaultValue) where XType : OpenXmlLeafElement, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var valProperty = typeof(XType).GetProperty("Val");
      var element = new XType();
      valProperty?.SetValue(element, new Int32Value((int)wordValue));
      return element;
    }
    return null;
  }

  private XType? GetFontSizeTypeElement<XType>(float wordValue, float? defaultValue) where XType : OpenXmlLeafElement, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var valProperty = typeof(XType).GetProperty("Val");
      var element = new XType();
      valProperty?.SetValue(element, new Int32Value(FontSizeToHps(wordValue)));
      return element;
    }
    return null;
  }

  private W.VerticalTextAlignment? GetVerticalTextAlignment(W.VerticalPositionValues positionValues, int wordValue, int? defaultValue)
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var element = new W.VerticalTextAlignment { Val = positionValues };
      return element;
    }
    return null;
  }
}
