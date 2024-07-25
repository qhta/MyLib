using System;
using System.Runtime.InteropServices;

using DocumentFormat.OpenXml;

using Qhta.OpenXmlTools;

using static Microsoft.Office.Interop.Word.WdLineSpacing;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.PropertiesConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

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
        var borderList = bordersConverter.CreateBordersList(styleBorders);
        if (borderList != null)
          xParaProperties.ParagraphBorders = borderList.ToOpenXmlBorders<W.ParagraphBorders>();
      }
    }
    catch (COMException) { }

    return xParaProperties.ToStyleParagraphProperties();
  }

  public W.ParagraphProperties ConvertParagraphFormat(Word.ParagraphFormat paraFormat)
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xParaProperties = new W.ParagraphProperties();

    #region paragraph alignment
    if ((int)paraFormat.Alignment != wdUndefined && paraFormat.Alignment != defaultParagraph?.Alignment)
    {
      var alignment = paraFormat.Alignment;
      W.JustificationValues justification = alignment switch
      {
        Word.WdParagraphAlignment.wdAlignParagraphLeft => W.JustificationValues.Left,
        Word.WdParagraphAlignment.wdAlignParagraphCenter => W.JustificationValues.Center,
        Word.WdParagraphAlignment.wdAlignParagraphRight => W.JustificationValues.Right,
        Word.WdParagraphAlignment.wdAlignParagraphJustify => W.JustificationValues.Both,
        Word.WdParagraphAlignment.wdAlignParagraphDistribute => W.JustificationValues.Distribute,
        Word.WdParagraphAlignment.wdAlignParagraphJustifyMed => W.JustificationValues.MediumKashida,
        Word.WdParagraphAlignment.wdAlignParagraphJustifyHi => W.JustificationValues.HighKashida,
        Word.WdParagraphAlignment.wdAlignParagraphJustifyLow => W.JustificationValues.LowKashida,
        _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
      };
      xParaProperties.Justification = new W.Justification { Val = justification };
    }
    #endregion paragraph alignment

    #region paragraph indentation
    var xIndentation = new W.Indentation();
    var addIndentation = false;
    var xLeftIndent = GetTwipsValue(paraFormat.LeftIndent, defaultParagraph?.LeftIndent);
    if (xLeftIndent != null)
    {
      xIndentation.Left = xLeftIndent.ToString();
      addIndentation = true;
    }

    var xLeftChars = GetCharsNumber(paraFormat.CharacterUnitLeftIndent, defaultParagraph?.CharacterUnitLeftIndent);
    if (xLeftChars != null)
    {
      xIndentation.LeftChars = xLeftChars;
      addIndentation = true;
    }

    var xFirstLine = GetTwipsValue(paraFormat.FirstLineIndent, defaultParagraph?.FirstLineIndent);
    if (xFirstLine != null) 
    {
      if (xFirstLine > 0)
        xIndentation.FirstLine = xFirstLine.ToString();
      else
        xIndentation.Hanging = (-xFirstLine).ToString();
      addIndentation = true;
    }

    var xFirstLineChars = GetCharsNumber(paraFormat.CharacterUnitFirstLineIndent, defaultParagraph?.CharacterUnitFirstLineIndent);
    if (xFirstLineChars != null) 
    {
      if (xFirstLineChars > 0)
        xIndentation.FirstLineChars = xFirstLineChars;
      else
        xIndentation.HangingChars = -xFirstLineChars;
      addIndentation = true;
    }


    var rightIndent = GetTwipsValue(paraFormat.RightIndent, defaultParagraph?.RightIndent);
    if (rightIndent != null)
    {
      xIndentation.Right = rightIndent.ToString();
      addIndentation = true;
    }

    var rightChars = GetCharsNumber(paraFormat.CharacterUnitRightIndent, defaultParagraph?.CharacterUnitRightIndent);
    if (rightChars != null) 
    {
      xIndentation.RightChars = rightChars;
      addIndentation = true;
    }
    
    if (addIndentation)
      xParaProperties.Indentation = xIndentation;

    xParaProperties.AdjustRightIndent = GetOnOffTypeElement<W.AdjustRightIndent>(paraFormat.AutoAdjustRightIndent,
      defaultParagraph?.AutoAdjustRightIndent);
    xParaProperties.MirrorIndents =
      GetOnOffTypeElement<W.MirrorIndents>(paraFormat.AutoAdjustRightIndent, defaultParagraph?.AutoAdjustRightIndent);
    #endregion paragraph indentation

    #region paragraph spacing
    var xSpacing = new W.SpacingBetweenLines();
    var addSpacing = false;

    var spaceBefore = GetTwipsValue(paraFormat.SpaceBefore, defaultParagraph?.SpaceBefore);
    if (spaceBefore != null)
    {
      xSpacing.Before = spaceBefore.ToString();
      addSpacing = true;
    }

    var spaceBeforeAuto = GetOnOffValue(paraFormat.SpaceBeforeAuto, defaultParagraph?.SpaceBeforeAuto);
    if (spaceBeforeAuto != null)
    {
      xSpacing.BeforeAutoSpacing = spaceBeforeAuto;
      addSpacing = true;
    }

    var beforeLines = GetLinesNumber(paraFormat.LineUnitBefore, defaultParagraph?.LineUnitBefore);
    if (beforeLines != null)
    {
      xSpacing.BeforeLines = beforeLines;
      addSpacing = true;
    }

    var spaceAfter = GetTwipsValue(paraFormat.SpaceAfter, defaultParagraph?.SpaceAfter);
    if (spaceAfter != null) 
    {
      xSpacing.After = spaceAfter.ToString();
      addSpacing = true;
    }

    var spaceAfterAuto = GetOnOffValue(paraFormat.SpaceAfterAuto, defaultParagraph?.SpaceAfterAuto);
    if (spaceAfterAuto != null)
    {
      xSpacing.AfterAutoSpacing = spaceAfterAuto;
      addSpacing = true;
    }

    var afterLines = GetLinesNumber(paraFormat.LineUnitAfter, defaultParagraph?.LineUnitAfter);
    if (afterLines != null)
    {
      xSpacing.AfterLines = afterLines;
      addSpacing = true;
    }

    if ((int)paraFormat.LineSpacingRule != wdUndefined && paraFormat.LineSpacingRule != defaultParagraph?.LineSpacingRule)
    {
      var lineSpacingRule = paraFormat.LineSpacingRule;
      W.LineSpacingRuleValues lineSpacingRuleValue = lineSpacingRule switch
      {
        wdLineSpaceSingle => W.LineSpacingRuleValues.Auto,
        wdLineSpace1pt5 => W.LineSpacingRuleValues.AtLeast,
        wdLineSpaceDouble => W.LineSpacingRuleValues.Exact,
        wdLineSpaceAtLeast => W.LineSpacingRuleValues.AtLeast,
        wdLineSpaceExactly => W.LineSpacingRuleValues.Exact,
        wdLineSpaceMultiple => W.LineSpacingRuleValues.Auto,
        _ => throw new ArgumentOutOfRangeException(nameof(lineSpacingRule), lineSpacingRule, null)
      };
      xSpacing.LineRule = new EnumValue<W.LineSpacingRuleValues>(lineSpacingRuleValue);
      addSpacing = true;
    }

    var lineSpacing = GetTwipsValue(paraFormat.LineSpacing, defaultParagraph?.LineSpacing);
    if (lineSpacing != null) 
    {
      xSpacing.Line = lineSpacing.ToString();
      addSpacing = true;
    }

    if (addSpacing)
      xParaProperties.SpacingBetweenLines = xSpacing;

    #endregion paragraph spacing

    xParaProperties.WidowControl = GetOnOffTypeElement<W.WidowControl>(paraFormat.WidowControl, defaultParagraph?.WidowControl);
    xParaProperties.KeepNext = GetOnOffTypeElement<W.KeepNext>(paraFormat.KeepWithNext, defaultParagraph?.KeepWithNext);
    xParaProperties.KeepLines = GetOnOffTypeElement<W.KeepLines>(paraFormat.KeepTogether, defaultParagraph?.KeepTogether);
    xParaProperties.PageBreakBefore = GetOnOffTypeElement<W.PageBreakBefore>(paraFormat.PageBreakBefore, defaultParagraph?.PageBreakBefore);
    xParaProperties.SuppressLineNumbers = GetOnOffTypeElement<W.SuppressLineNumbers>(paraFormat.NoLineNumber, defaultParagraph?.NoLineNumber);
    xParaProperties.SuppressAutoHyphens = GetOnOffTypeElement<W.SuppressAutoHyphens>(paraFormat.Hyphenation, defaultParagraph?.Hyphenation);
    xParaProperties.OutlineLevel = GetIntValTypeElement<W.OutlineLevel>((int)paraFormat.OutlineLevel-1, (int?)defaultParagraph?.OutlineLevel-1);


    
    return xParaProperties;
  }
}
