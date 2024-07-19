using System;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using static Qhta.WordInteropOpenXmlConverter.BorderConverter;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.LangConverter;
using O = DocumentFormat.OpenXml;
using W = DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;

#nullable enable


namespace Qhta.WordInteropOpenXmlConverter;

public class StyleConverter
{
  public StyleConverter(Word.Document document)
  {
    StyleTools = new StyleTools(document);
    BuildInStyleNumbers = StyleTools.LocalNameMyBuiltinStyles;
    DefaultStyle = StyleTools.GetStyle(WdBuiltinStyle.wdStyleNormal);
    defaultFont = DefaultStyle.Font;
    defaultParagraph = DefaultStyle.ParagraphFormat;
  }

  private readonly StyleTools StyleTools;
  private readonly Dictionary<string, MyBuiltinStyle> BuildInStyleNumbers;
  private readonly Word.Style DefaultStyle;
  private readonly Word.Font defaultFont;
  private readonly ParagraphFormat defaultParagraph;

  public W.Style ConvertStyle(Word.Style wordStyle)
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xStyle = new W.Style();

    #region style id and name
    var styleId = wordStyle.NameLocal;
    styleId = StyleTools.StyleNameToId(styleId!);
    xStyle.StyleId = styleId;

    if (!StyleTools.TryLocalNameToBuiltinName(wordStyle.NameLocal, out var styleName))
      styleName = wordStyle.NameLocal;
    if (styleName!= null)
      xStyle.StyleName = new W.StyleName { Val = styleName };
    #endregion style id and name

    #region style type
    W.StyleValues styleType = wordStyle.Type switch
    {
      WdStyleType.wdStyleTypeParagraph => W.StyleValues.Paragraph,
      WdStyleType.wdStyleTypeCharacter => W.StyleValues.Character,
      WdStyleType.wdStyleTypeTable => W.StyleValues.Table,
      WdStyleType.wdStyleTypeList => W.StyleValues.Numbering,
      WdStyleType.wdStyleTypeParagraphOnly => W.StyleValues.Paragraph,
      WdStyleType.wdStyleTypeLinked => W.StyleValues.Paragraph,
      _ => throw new ArgumentOutOfRangeException(nameof(wordStyle.Type),
        // ReSharper disable once LocalizableElement
        $"Unsupported style type: {wordStyle.Type}")
    };
    xStyle.Type = new EnumValue<W.StyleValues>(styleType);
    #endregion style type

    if (wordStyle.Hidden)
      xStyle.SemiHidden = new W.SemiHidden();
    if (wordStyle.UnhideWhenUsed)
      xStyle.UnhideWhenUsed = new W.UnhideWhenUsed();
    if (wordStyle.Priority > 0)
      xStyle.UIPriority = new W.UIPriority { Val = wordStyle.Priority - 1 };
    try
    {
      if (wordStyle.AutomaticallyUpdate)
        xStyle.AutoRedefine = new W.AutoRedefine();
    }
    catch { }
    try
    {
      Word.Style baseStyle = (Word.Style)wordStyle.get_BaseStyle();
      if (baseStyle.NameLocal.Length > 0)
        xStyle.BasedOn = new W.BasedOn { Val = StyleTools.StyleNameToId(baseStyle.NameLocal) };
    }
    catch { }
    try
    {
      Word.Style linkStyle = (Word.Style)wordStyle.get_LinkStyle();
      xStyle.LinkedStyle = new W.LinkedStyle { Val = StyleTools.StyleNameToId(linkStyle.NameLocal) };
    }
    catch { }
    try
    {
      Word.Style nextParagraphStyle = (Word.Style)wordStyle.get_NextParagraphStyle();
      xStyle.NextParagraphStyle = new W.NextParagraphStyle { Val = StyleTools.StyleNameToId(nextParagraphStyle.NameLocal) };
    }
    catch { }

    #region style run properties
    try
    {
      var styleFont = wordStyle.Font;
      // ReSharper disable once UseObjectOrCollectionInitializer
      var xRunProps = new W.StyleRunProperties();
      xRunProps.RunFonts = new W.RunFonts { Ascii = styleFont.NameAscii, HighAnsi = styleFont.Name, ComplexScript = styleFont.NameBi };
      if (styleFont.Size != 0 && styleFont.Size != defaultFont.Size)
        xRunProps.FontSize = new W.FontSize { Val = FontSizeToHps(styleFont.Size).ToString() };
      if (styleFont.SizeBi != 0 && styleFont.SizeBi != defaultFont.SizeBi)
        xRunProps.FontSizeComplexScript = new W.FontSizeComplexScript { Val = FontSizeToHps(styleFont.SizeBi).ToString() };
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
      var xColor = new W.Color();

      #region color props
      var addColor = false;
      if (styleFont.Color != WdColor.wdColorAutomatic)
      {
        xColor.Val = WordColorToHex(styleFont.Color);
        addColor = true;
      }
      if (styleFont.ColorIndex != WdColorIndex.wdAuto)
      {
        xColor.Val = WordColorIndexToHex(styleFont.ColorIndex);
        addColor = true;
      }
      try
      {
        var themeColor = styleFont.TextColor.ObjectThemeColor;
        if (themeColor != WdThemeColorIndex.wdNotThemeColor)
        {
          xColor.ThemeColor = new EnumValue<W.ThemeColorValues>(WdThemeColorToOpenXmlThemeColor(themeColor));
          addColor = true;
        }
      }
      catch { }
      try
      {
        var tintAndShade = styleFont.TextColor.TintAndShade;
        if (tintAndShade < 0)
        {
          xColor.ThemeShade = ConvertTintAndShadeToThemeShade(tintAndShade);
          addColor = true;
        }
      }
      catch { }

      if (addColor)
        xRunProps.Color = xColor;
      #endregion color props

      #region lang props
      var langProps = new W.Languages();
      bool addLangProps = false;

      if (wordStyle.LanguageID != 0 && wordStyle.LanguageID != DefaultStyle.LanguageID)
      {
        langProps.Val = LanguageIdToBcp47Tag(wordStyle.LanguageID);
        addLangProps = true;
      }
      if (wordStyle.LanguageIDFarEast != WdLanguageID.wdNoProofing
          && wordStyle.LanguageIDFarEast != DefaultStyle.LanguageIDFarEast)
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
        var borders = styleFont.Borders;
        if (borders != null)
        {
          try
          {
            var border = borders[WdBorderType.wdBorderBottom];
            if (border != null && border.LineStyle != 0)
            {
              // ReSharper disable once UseObjectOrCollectionInitializer
              var xBorder = new W.BottomBorder();
              xBorder.Val = new EnumValue<W.BorderValues>(WdBorderToOpenXmlBorder(border.LineStyle));
              xBorder.Color = WordColorToHex(border.Color);
              xBorder.Size = WdLineWidthToBorderWidth(border.LineWidth);
              xRunProps.Append(xBorder);
            }
          }
          catch { }
          try
          {
            var border = borders[WdBorderType.wdBorderTop];
            if (border != null && border.LineStyle != 0)
            {
              // ReSharper disable once UseObjectOrCollectionInitializer
              var xBorder = new W.TopBorder();
              xBorder.Val = new EnumValue<W.BorderValues>(WdBorderToOpenXmlBorder(border.LineStyle));
              xBorder.Color = WordColorToHex(border.Color);
              xBorder.Size = WdLineWidthToBorderWidth(border.LineWidth);
              xRunProps.Append(xBorder);
            }
          }
          catch { }
          try
          {
            var border = borders[WdBorderType.wdBorderLeft];
            if (border != null && border.LineStyle != 0)
            {
              // ReSharper disable once UseObjectOrCollectionInitializer
              var xBorder = new W.LeftBorder();
              xBorder.Val = new EnumValue<W.BorderValues>(WdBorderToOpenXmlBorder(border.LineStyle));
              xBorder.Color = WordColorToHex(border.Color);
              xBorder.Size = WdLineWidthToBorderWidth(border.LineWidth);
              xRunProps.Append(xBorder);
            }
          }
          catch { }
          try
          {
            var border = borders[WdBorderType.wdBorderRight];
            if (border != null && border.LineStyle != 0)
            {
              // ReSharper disable once UseObjectOrCollectionInitializer
              var xBorder = new W.RightBorder();
              xBorder.Val = new EnumValue<W.BorderValues>(WdBorderToOpenXmlBorder(border.LineStyle));
              xBorder.Color = WordColorToHex(border.Color);
              xBorder.Size = WdLineWidthToBorderWidth(border.LineWidth);
              xRunProps.Append(xBorder);
            }
          }
          catch { }
        }
        #endregion text borders
      }
      catch { }

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

      xStyle.StyleRunProperties = xRunProps;
    }
    catch { }
    #endregion style run properties

    #region paragraph properties
    try
    {
      var paraFormat = wordStyle.ParagraphFormat;
      // ReSharper disable once UseObjectOrCollectionInitializer
      var xParaProperties = new W.StyleParagraphProperties();

      #region paragraph alignment
      if (paraFormat.Alignment != WdParagraphAlignment.wdAlignParagraphLeft)
      {
        var alignment = paraFormat.Alignment;
        W.JustificationValues justification = alignment switch
        {
          WdParagraphAlignment.wdAlignParagraphLeft => W.JustificationValues.Left,
          WdParagraphAlignment.wdAlignParagraphCenter => W.JustificationValues.Center,
          WdParagraphAlignment.wdAlignParagraphRight => W.JustificationValues.Right,
          WdParagraphAlignment.wdAlignParagraphJustify => W.JustificationValues.Both,
          WdParagraphAlignment.wdAlignParagraphDistribute => W.JustificationValues.Distribute,
          WdParagraphAlignment.wdAlignParagraphJustifyMed => W.JustificationValues.MediumKashida,
          WdParagraphAlignment.wdAlignParagraphJustifyHi => W.JustificationValues.HighKashida,
          WdParagraphAlignment.wdAlignParagraphJustifyLow => W.JustificationValues.LowKashida,
          _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
        xParaProperties.Justification = new W.Justification { Val = justification };
      }
      #endregion paragraph alignment

      #region paragraph indentation
      if (paraFormat.LeftIndent != 0)
        xParaProperties.Indentation = new W.Indentation { Left = PointsToTwips(paraFormat.LeftIndent).ToString() };

      if (paraFormat.RightIndent != 0)
        xParaProperties.Indentation = new W.Indentation { Right = PointsToTwips(paraFormat.RightIndent).ToString() };

      if (paraFormat.FirstLineIndent != 0)
        xParaProperties.Indentation = new W.Indentation { FirstLine = PointsToTwips(paraFormat.FirstLineIndent).ToString() };
      #endregion paragraph indentation

      #region paragraph spacing
      var xSpacing = new W.SpacingBetweenLines();
      var addSpacing = false;
      if (paraFormat.SpaceBefore != defaultParagraph.SpaceBefore)
      {
        xSpacing.Before = PointsToTwips(paraFormat.SpaceBefore).ToString();
        addSpacing = true;
      }
      if (paraFormat.SpaceBeforeAuto != defaultParagraph.SpaceBeforeAuto)
      {
        xSpacing.BeforeAutoSpacing = new O.OnOffValue(paraFormat.SpaceBeforeAuto != 0);
        addSpacing = true;
      }
      if (paraFormat.LineUnitBefore != defaultParagraph.LineUnitBefore)
      {
        xSpacing.BeforeLines = (int)paraFormat.LineUnitBefore;
        addSpacing = true;
      }
      if (paraFormat.SpaceAfter != defaultParagraph.SpaceAfter)
      {
        xSpacing.After = PointsToTwips(paraFormat.SpaceAfter).ToString();
        addSpacing = true;
      }
      if (paraFormat.SpaceAfterAuto != defaultParagraph.SpaceAfterAuto)
      {
        xSpacing.AfterAutoSpacing = new O.OnOffValue(paraFormat.SpaceAfterAuto != 0);
        addSpacing = true;
      }
      if (paraFormat.LineUnitAfter != defaultParagraph.LineUnitAfter)
      {
        xSpacing.AfterLines = (int)paraFormat.LineUnitAfter;
        addSpacing = true;
      }
      if (paraFormat.LineSpacingRule != defaultParagraph.LineSpacingRule)
      {
        var lineSpacingRule = paraFormat.LineSpacingRule;
        W.LineSpacingRuleValues lineSpacing = lineSpacingRule switch
        {
          WdLineSpacing.wdLineSpaceSingle => W.LineSpacingRuleValues.Auto,
          WdLineSpacing.wdLineSpace1pt5 => W.LineSpacingRuleValues.AtLeast,
          WdLineSpacing.wdLineSpaceDouble => W.LineSpacingRuleValues.Exact,
          WdLineSpacing.wdLineSpaceAtLeast => W.LineSpacingRuleValues.AtLeast,
          WdLineSpacing.wdLineSpaceExactly => W.LineSpacingRuleValues.Exact,
          WdLineSpacing.wdLineSpaceMultiple => W.LineSpacingRuleValues.Auto,
          _ => throw new ArgumentOutOfRangeException(nameof(lineSpacingRule), lineSpacingRule, null)
        };
        xSpacing.LineRule = new EnumValue<W.LineSpacingRuleValues>(lineSpacing);
        addSpacing = true;
      }
      if (paraFormat.LineSpacing != defaultParagraph.LineSpacing)
      {
        xSpacing.Line = PointsToTwips(paraFormat.LineSpacing).ToString();
        addSpacing = true;
      }
      if (addSpacing)
        xParaProperties.SpacingBetweenLines = xSpacing;

      if (wordStyle.NoSpaceBetweenParagraphsOfSameStyle)
        xParaProperties.ContextualSpacing = new W.ContextualSpacing();
      #endregion paragraph spacing

      if (paraFormat.WidowControl != defaultParagraph.WidowControl)
      {
        xParaProperties.WidowControl = new W.WidowControl();
        if (paraFormat.WidowControl == 0)
          xParaProperties.WidowControl.Val = OnOffValue.FromBoolean(false);
      }

      if (paraFormat.KeepWithNext != defaultParagraph.KeepWithNext)
      {
        xParaProperties.KeepNext = new W.KeepNext();
        if (paraFormat.KeepWithNext == 0)
          xParaProperties.KeepNext.Val = OnOffValue.FromBoolean(false);
      }

      if (paraFormat.KeepTogether != defaultParagraph.KeepTogether)
      {
        xParaProperties.KeepLines = new W.KeepLines();
        if (paraFormat.KeepTogether == 0)
          xParaProperties.KeepLines.Val = OnOffValue.FromBoolean(false);
      }

      if (paraFormat.PageBreakBefore != defaultParagraph.PageBreakBefore)
      {
        xParaProperties.PageBreakBefore = new W.PageBreakBefore();
        if (paraFormat.PageBreakBefore == 0)
          xParaProperties.PageBreakBefore.Val = OnOffValue.FromBoolean(false);
      }

      xStyle.StyleParagraphProperties = xParaProperties;
    }
    catch { }
    #endregion paragraph formating

    //try
    //{
    //  var xListTemplate = new XElement("ListTemplate");
    //  xListTemplate.Add(new XAttribute("ListTemplate", wordStyle.ListTemplate));
    //}
    //catch (Exception ex)
    //{
    //}
    return xStyle;
  }

}
