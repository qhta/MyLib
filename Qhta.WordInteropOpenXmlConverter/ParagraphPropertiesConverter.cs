using System;
using System.Diagnostics;
using DocumentFormat.OpenXml;
using Microsoft.Office.Interop.Word;
using Qhta.OpenXmlTools;
using Word = Microsoft.Office.Interop.Word;
using W = DocumentFormat.OpenXml.Wordprocessing;
using O = DocumentFormat.OpenXml;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.BordersConverter;

namespace Qhta.WordInteropOpenXmlConverter;

public class ParagraphPropertiesConverter
{
  private readonly Word.ParagraphFormat? defaultParagraph;

  public ParagraphPropertiesConverter()
  {
  }

  public ParagraphPropertiesConverter(Word.Style defaultStyle)
  {
    defaultParagraph = defaultStyle.ParagraphFormat;
  }

  public W.StyleParagraphProperties ConvertStyleParagraphFormat(Word.Style wordStyle)
  {
    var paraFormat = wordStyle.ParagraphFormat;
    var xParaProperties = ConvertParagraphFormat(paraFormat);

    if (wordStyle.NoSpaceBetweenParagraphsOfSameStyle)
      xParaProperties.ContextualSpacing = new W.ContextualSpacing();

    try
    {
      var styleBorders = wordStyle.Borders;
      if (styleBorders != null)
      {
        var bordersConverter = new BordersConverter();
        var xBorders = bordersConverter.CreateBorders(styleBorders);
        if (xBorders != null)
          xParaProperties.ParagraphBorders = xBorders;
      }
    }
    catch { }

    return xParaProperties.ToStyleParagraphProperties();
  }

  public W.ParagraphProperties ConvertParagraphFormat(Word.ParagraphFormat paraFormat)
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xParaProperties = new W.ParagraphProperties();

    #region paragraph alignment
    if (paraFormat.Alignment != 0)
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
    var xIndentation = new W.Indentation();
    var addIndentation = false;
    if (paraFormat.LeftIndent != 0)
    {
      xIndentation.Left = PointsToTwips(paraFormat.LeftIndent).ToString();
      addIndentation = true;
    }
    if (paraFormat.RightIndent != 0)
    {
      xIndentation.Right = PointsToTwips(paraFormat.RightIndent).ToString();
      addIndentation = true;
    }
    if (paraFormat.FirstLineIndent != 0)
    {
      if (paraFormat.FirstLineIndent > 0)
        xIndentation.FirstLine = PointsToTwips(paraFormat.FirstLineIndent).ToString();
      else
        xIndentation.Hanging = PointsToTwips(-paraFormat.FirstLineIndent).ToString();
      addIndentation = true;
    }
    if (paraFormat.CharacterUnitFirstLineIndent != 0)
    {

      if (paraFormat.CharacterUnitFirstLineIndent > 0)
        xIndentation.FirstLineChars = (int)paraFormat.CharacterUnitFirstLineIndent;
      else
        xIndentation.HangingChars = -(int)paraFormat.CharacterUnitFirstLineIndent;
      addIndentation = true;
    }
    if (paraFormat.RightIndent != 0)
    {
      xIndentation.Right = PointsToTwips(paraFormat.RightIndent).ToString();
      addIndentation = true;
    }
    if (paraFormat.CharacterUnitRightIndent != 0)
    {
      xIndentation.RightChars = (int)paraFormat.CharacterUnitRightIndent;
      addIndentation = true;
    }

    if (addIndentation)
      xParaProperties.Indentation = xIndentation;

    if (paraFormat.AutoAdjustRightIndent != defaultParagraph.AutoAdjustRightIndent)
    {
      xParaProperties.AdjustRightIndent = new W.AdjustRightIndent();
      if (paraFormat.AutoAdjustRightIndent == 0)
        xParaProperties.AdjustRightIndent.Val = OnOffValue.FromBoolean(false);
    }

    if (paraFormat.MirrorIndents != defaultParagraph.MirrorIndents)
    {
      xParaProperties.MirrorIndents = new W.MirrorIndents();
      if (paraFormat.MirrorIndents == 0)
        xParaProperties.MirrorIndents.Val = OnOffValue.FromBoolean(false);
    }
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
    if (paraFormat.NoLineNumber != defaultParagraph.NoLineNumber)
    {
      xParaProperties.SuppressLineNumbers = new W.SuppressLineNumbers();
      if (paraFormat.NoLineNumber == 0)
        xParaProperties.SuppressLineNumbers.Val = OnOffValue.FromBoolean(false);
    }
    if (paraFormat.OutlineLevel != defaultParagraph.OutlineLevel)
    {
      xParaProperties.OutlineLevel = new W.OutlineLevel { Val = (byte)paraFormat.OutlineLevel - 1 };
    }
    if (paraFormat.Hyphenation != defaultParagraph.Hyphenation)
    {
      xParaProperties.SuppressAutoHyphens = new W.SuppressAutoHyphens();
      if (paraFormat.Hyphenation == 0)
        xParaProperties.SuppressAutoHyphens.Val = OnOffValue.FromBoolean(false);
    }

    return xParaProperties;
  }
}
