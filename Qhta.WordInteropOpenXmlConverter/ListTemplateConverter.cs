using DocumentFormat.OpenXml;

using Qhta.OpenXmlTools;

using static Qhta.WordInteropOpenXmlConverter.NumberConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public class ListTemplateConverter
{
 
  public W.AbstractNum ConvertListTemplate(Word.ListTemplate ListTemplate)
  {
    var xAbstractNum = new W.AbstractNum();
    try
    {
      var lCount = ListTemplate.ListLevels.Count;
      if (lCount > 1)
      {
        xAbstractNum.MultiLevelType = new W.MultiLevelType
        { Val = new EnumValue<W.MultiLevelValues>(W.MultiLevelValues.HybridMultilevel) };
      }
      else
      {
        xAbstractNum.MultiLevelType = new W.MultiLevelType
        { Val = new EnumValue<W.MultiLevelValues>(W.MultiLevelValues.SingleLevel) };
      }
      for (int level = 1; level <= lCount; level++)
      {
        try
        {
          Word.ListLevel wordLevel = ListTemplate.ListLevels[level];
          string numberFormat = wordLevel.NumberFormat;
          W.NumberFormatValues numberFormatValues = wordLevel.NumberStyle switch
          {
            Word.WdListNumberStyle.wdListNumberStyleArabic => W.NumberFormatValues.Decimal,
            Word.WdListNumberStyle.wdListNumberStyleUppercaseRoman => W.NumberFormatValues.UpperRoman,
            Word.WdListNumberStyle.wdListNumberStyleLowercaseRoman => W.NumberFormatValues.LowerRoman,
            Word.WdListNumberStyle.wdListNumberStyleUppercaseLetter => W.NumberFormatValues.UpperLetter,
            Word.WdListNumberStyle.wdListNumberStyleLowercaseLetter => W.NumberFormatValues.LowerLetter,
            Word.WdListNumberStyle.wdListNumberStyleOrdinal => W.NumberFormatValues.Ordinal,
            Word.WdListNumberStyle.wdListNumberStyleOrdinalText => W.NumberFormatValues.OrdinalText,
            Word.WdListNumberStyle.wdListNumberStyleArabicFullWidth => W.NumberFormatValues.DecimalFullWidth,
            Word.WdListNumberStyle.wdListNumberStyleNumberInCircle => W.NumberFormatValues.DecimalEnclosedCircle,
            Word.WdListNumberStyle.wdListNumberStyleAiueo => W.NumberFormatValues.Aiueo,
            Word.WdListNumberStyle.wdListNumberStyleIroha => W.NumberFormatValues.Iroha,
            Word.WdListNumberStyle.wdListNumberStyleGanada => W.NumberFormatValues.Ganada,
            Word.WdListNumberStyle.wdListNumberStyleChosung => W.NumberFormatValues.Chosung,
            Word.WdListNumberStyle.wdListNumberStyleHebrew1 => W.NumberFormatValues.Hebrew1,
            Word.WdListNumberStyle.wdListNumberStyleArabic1 => W.NumberFormatValues.ArabicAbjad,
            Word.WdListNumberStyle.wdListNumberStyleHebrew2 => W.NumberFormatValues.Hebrew2,
            Word.WdListNumberStyle.wdListNumberStyleArabic2 => W.NumberFormatValues.ArabicAlpha,
            Word.WdListNumberStyle.wdListNumberStyleHindiArabic => W.NumberFormatValues.HindiNumbers,
            Word.WdListNumberStyle.wdListNumberStyleNone => W.NumberFormatValues.None,
            Word.WdListNumberStyle.wdListNumberStylePictureBullet => W.NumberFormatValues.Bullet,
            _ => W.NumberFormatValues.Decimal
          };

          try
          {
            if (numberFormat.Length == 1)
            {
              //if (numberFormat[0] == '\uF0B7')
              //{
              //  numberFormatValues = NumberFormatValues.Bullet;
              //  //numberFormat = "•";
              //}
              //else if (Char.GetUnicodeCategory(numberFormat[0]) == UnicodeCategory.PrivateUse)
              //{
              //  numberFormatValues = NumberFormatValues.Bullet;
              //  //numberFormat = "•";
              //}
              numberFormatValues = W.NumberFormatValues.Bullet;
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
            Word.WdListLevelAlignment.wdListLevelAlignLeft => W.LevelJustificationValues.Left,
            Word.WdListLevelAlignment.wdListLevelAlignCenter => W.LevelJustificationValues.Center,
            Word.WdListLevelAlignment.wdListLevelAlignRight => W.LevelJustificationValues.Right,
            _ => W.LevelJustificationValues.Left
          };
          // ReSharper disable once UseObjectOrCollectionInitializer
          var xNumberingLevel = new W.Level();
          xNumberingLevel.LevelIndex = wordLevel.Index - 1;
          xNumberingLevel.StartNumberingValue = new W.StartNumberingValue { Val = wordLevel.StartAt };
          xNumberingLevel.NumberingFormat = new W.NumberingFormat { Val = numberFormatValues };
          xNumberingLevel.LevelText = new W.LevelText { Val = numberFormat };
          xNumberingLevel.LevelJustification = new W.LevelJustification { Val = levelJustificationValues };


          try
          {
            Word.Font font = wordLevel.Font;
            var xRunProperties = new RunPropertiesConverter().ConvertFont(font);
            xNumberingLevel.NumberingSymbolRunProperties = xRunProperties.ToNumberingSymbolRunProperties();
          }
          catch { }

          try
          {
            var linkedStyle = wordLevel.LinkedStyle;
            if (linkedStyle != null)
            {
              xNumberingLevel.ParagraphStyleIdInLevel = new W.ParagraphStyleIdInLevel { Val = linkedStyle };
            }
          }
          catch { }

          try
          {
            var xIndentation = new W.Indentation();
            var addIndentation = false;
            float leftIndentation = wordLevel.TextPosition;
            if (leftIndentation != wdUndefined)
            {
              xIndentation.Left = PointsToTwips(leftIndentation).ToString();
              addIndentation = true;
            }
            float hangingIndentation = wordLevel.NumberPosition;
            if (hangingIndentation != wdUndefined)
            {
              xIndentation.Hanging = PointsToTwips(hangingIndentation).ToString();
              addIndentation = true;
            }
            float tabPosition = wordLevel.TabPosition;
            if (tabPosition != wdUndefined)
            {
              xIndentation.FirstLine = PointsToTwips(tabPosition).ToString();
              addIndentation = true;
            }
            if (addIndentation)
            {
              xNumberingLevel.Append(new W.ParagraphProperties { Indentation = xIndentation });
            }
          }
          catch { }
          xAbstractNum.Append(xNumberingLevel);
        }
        catch { }
      }
    }
    catch { }

    return xAbstractNum;
  }
}

