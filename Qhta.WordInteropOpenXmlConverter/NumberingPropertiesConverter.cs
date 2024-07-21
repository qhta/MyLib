using System;
using System.Diagnostics;
using System.Globalization;
using DocumentFormat.OpenXml;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using W = DocumentFormat.OpenXml.Wordprocessing;
using O = DocumentFormat.OpenXml;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.BordersConverter;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.WordInteropOpenXmlConverter;

public class NumberingPropertiesConverter(Word.Style defaultStyle)
{
  private readonly Word.ParagraphFormat defaultParagraph = defaultStyle.ParagraphFormat;

  public W.AbstractNum? CreateNumberingProperties(Word.Style wordStyle)
  {
    var abstractNum = new W.AbstractNum();
    var hasNumbering = false;
    try
    {
      var listTemplate = wordStyle.ListTemplate;
      Debug.WriteLine($"{wordStyle.Type} style {wordStyle.NameLocal} has list template: {listTemplate.Name}");

      var lCount = listTemplate.ListLevels.Count;
      Debug.WriteLine($"{wordStyle.Type} style {wordStyle.NameLocal} has {lCount} list levels");
      if (lCount > 1)
      {
        Debug.Assert(true);
      }
      for (int level = 1; level <= lCount; level++)
      {
        try
        {
          Word.ListLevel wordLevel = wordStyle.ListTemplate.ListLevels[level];
          hasNumbering = true;
          string numberFormat = wordLevel.NumberFormat;
          W.NumberFormatValues numberFormatValues = wordLevel.NumberStyle switch
          {
            WdListNumberStyle.wdListNumberStyleArabic => W.NumberFormatValues.Decimal,
            WdListNumberStyle.wdListNumberStyleUppercaseRoman => W.NumberFormatValues.UpperRoman,
            WdListNumberStyle.wdListNumberStyleLowercaseRoman => W.NumberFormatValues.LowerRoman,
            WdListNumberStyle.wdListNumberStyleUppercaseLetter => W.NumberFormatValues.UpperLetter,
            WdListNumberStyle.wdListNumberStyleLowercaseLetter => W.NumberFormatValues.LowerLetter,
            WdListNumberStyle.wdListNumberStyleOrdinal => W.NumberFormatValues.Ordinal,
            WdListNumberStyle.wdListNumberStyleOrdinalText => W.NumberFormatValues.OrdinalText,
            WdListNumberStyle.wdListNumberStyleArabicFullWidth => W.NumberFormatValues.DecimalFullWidth,
            WdListNumberStyle.wdListNumberStyleNumberInCircle => W.NumberFormatValues.DecimalEnclosedCircle,
            WdListNumberStyle.wdListNumberStyleAiueo => W.NumberFormatValues.Aiueo,
            WdListNumberStyle.wdListNumberStyleIroha => W.NumberFormatValues.Iroha,
            WdListNumberStyle.wdListNumberStyleGanada => W.NumberFormatValues.Ganada,
            WdListNumberStyle.wdListNumberStyleChosung => W.NumberFormatValues.Chosung,
            WdListNumberStyle.wdListNumberStyleHebrew1 => W.NumberFormatValues.Hebrew1,
            WdListNumberStyle.wdListNumberStyleArabic1 => W.NumberFormatValues.ArabicAbjad,
            WdListNumberStyle.wdListNumberStyleHebrew2 => W.NumberFormatValues.Hebrew2,
            WdListNumberStyle.wdListNumberStyleArabic2 => W.NumberFormatValues.ArabicAlpha,
            WdListNumberStyle.wdListNumberStyleHindiArabic => W.NumberFormatValues.HindiNumbers,
            WdListNumberStyle.wdListNumberStyleNone => W.NumberFormatValues.None,
            WdListNumberStyle.wdListNumberStylePictureBullet => W.NumberFormatValues.Bullet,
            _ => NumberFormatValues.Decimal
          };

          try
          {
            if (numberFormat.Length == 1)
            {
              if (numberFormat[0] == '\uF0B7')
              {
                numberFormatValues = NumberFormatValues.Bullet;
                numberFormat = "•";
              }
              else if (Char.GetUnicodeCategory(numberFormat[0]) == UnicodeCategory.PrivateUse)
              {
                numberFormatValues = NumberFormatValues.Bullet;
                numberFormat = "•";
              }
              else if (numberFormat[0] == '•')
              {
                numberFormatValues = NumberFormatValues.Bullet;
              }
            }
            var pictureBullet = wordLevel.PictureBullet;
            if (pictureBullet != null)
            {
              numberFormatValues = NumberFormatValues.Bullet;
              numberFormat = "•";
            }
          }
          catch { }

          W.LevelJustificationValues levelJustificationValues = wordLevel.Alignment switch
          {
            WdListLevelAlignment.wdListLevelAlignLeft => LevelJustificationValues.Left,
            WdListLevelAlignment.wdListLevelAlignCenter => LevelJustificationValues.Center,
            WdListLevelAlignment.wdListLevelAlignRight => LevelJustificationValues.Right,
            _ => LevelJustificationValues.Left
          };
          // ReSharper disable once UseObjectOrCollectionInitializer
          var xNumberingLevel = new W.Level();
          xNumberingLevel.LevelIndex = wordLevel.Index-1;
          xNumberingLevel.StartNumberingValue = new StartNumberingValue() { Val = wordLevel.StartAt };
          xNumberingLevel.NumberingFormat = new NumberingFormat() { Val = numberFormatValues };
          xNumberingLevel.LevelText = new LevelText() { Val = numberFormat };
          xNumberingLevel.LevelJustification = new LevelJustification() { Val = levelJustificationValues };
          abstractNum.Append(xNumberingLevel);
        }
        catch { }
      }
    }
    catch { }
    if (hasNumbering)
      return abstractNum;
    return null;
  }


}
