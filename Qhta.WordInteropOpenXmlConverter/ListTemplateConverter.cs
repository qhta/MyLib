using System;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.LanguageConverter;
using static Qhta.OpenXMLTools.RunTools;
using O = DocumentFormat.OpenXml;
using W = DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using Qhta.OpenXmlTools;
using Qhta.OpenXMLTools;
using System.Globalization;

#nullable enable


namespace Qhta.WordInteropOpenXmlConverter;

public class ListTemplateConverter
{
  public ListTemplateConverter(Word.Document document)
  {
  }


  public W.AbstractNum ConvertListTemplate(Word.ListTemplate ListTemplate)
  {
    var abstractNum = new W.AbstractNum();
    try
    {
      var lCount = ListTemplate.ListLevels.Count;
      if (lCount > 1)
      {
        abstractNum.MultiLevelType = new MultiLevelType
          { Val = new EnumValue<MultiLevelValues>(W.MultiLevelValues.HybridMultilevel) };
      }
      else
      {
        abstractNum.MultiLevelType = new MultiLevelType
          { Val = new EnumValue<MultiLevelValues>(W.MultiLevelValues.SingleLevel) };
      }
      for (int level = 1; level <= lCount; level++)
      {
        try
        {
          Word.ListLevel wordLevel = ListTemplate.ListLevels[level];
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
                //numberFormat = "•";
              }
              else if (Char.GetUnicodeCategory(numberFormat[0]) == UnicodeCategory.PrivateUse)
              {
                numberFormatValues = NumberFormatValues.Bullet;
                //numberFormat = "•";
              }
              numberFormatValues = NumberFormatValues.Bullet;
            }
            //var pictureBullet = wordLevel.PictureBullet;
            //if (pictureBullet != null)
            //{
            //  numberFormatValues = NumberFormatValues.Bullet;
            //  numberFormat = "•";
            //}
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
          xNumberingLevel.LevelIndex = wordLevel.Index - 1;
          xNumberingLevel.StartNumberingValue = new StartNumberingValue() { Val = wordLevel.StartAt };
          xNumberingLevel.NumberingFormat = new NumberingFormat() { Val = numberFormatValues };
          xNumberingLevel.LevelText = new LevelText() { Val = numberFormat };
          xNumberingLevel.LevelJustification = new LevelJustification() { Val = levelJustificationValues };


          try
          {
            Word.Font font = wordLevel.Font;
            var xRunProperties = new RunPropertiesConverter().ConvertFont(font);
            xNumberingLevel.NumberingSymbolRunProperties=xRunProperties.ToNumberingSymbolRunProperties();
          }
          catch { }
          abstractNum.Append(xNumberingLevel);
        }
        catch { }
      }
    }
    catch { }

    return abstractNum;
  }
}

