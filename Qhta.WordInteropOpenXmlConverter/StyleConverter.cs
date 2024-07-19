using System;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using static Qhta.WordInteropOpenXmlConverter.BorderConverter;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.LanguageConverter;
using static Qhta.OpenXMLTools.RunTools;
using O = DocumentFormat.OpenXml;
using W = DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using Qhta.OpenXMLTools;

#nullable enable


namespace Qhta.WordInteropOpenXmlConverter;

public class StyleConverter
{
  public StyleConverter(Word.Document document)
  {
    styleTools = new StyleTools(document);
    buildInStyleNumbers = styleTools.LocalNameMyBuiltinStyles;
    defaultStyle = styleTools.GetStyle(Word.WdBuiltinStyle.wdStyleNormal);
    defaultFont = defaultStyle.Font;
    defaultParagraph = defaultStyle.ParagraphFormat;
    themeTools = new ThemeTools(document);
  }

  private readonly StyleTools styleTools;
  private readonly ThemeTools themeTools;
  private readonly Dictionary<string, MyBuiltinStyle> buildInStyleNumbers;
  private readonly Word.Style defaultStyle;
  private readonly Word.Font defaultFont;
  private readonly ParagraphFormat defaultParagraph;

  private readonly Dictionary<string, MyThemeGroupName> themeGroupNames = new();

  public W.Style ConvertStyle(Word.Style wordStyle)
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xStyle = new W.Style();

    #region style id and name
    var styleId = wordStyle.NameLocal;
    styleId = StyleTools.StyleNameToId(styleId!);
    xStyle.StyleId = styleId;

    if (!styleTools.TryLocalNameToBuiltinName(wordStyle.NameLocal, out var styleName))
      styleName = wordStyle.NameLocal;
    if (styleName != null)
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
      var xRunProps = new RunPropertiesConverter(defaultStyle, themeTools).CreateRunProperties(wordStyle);
      xStyle.StyleRunProperties = xRunProps.ToStyleRunProperties();
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
