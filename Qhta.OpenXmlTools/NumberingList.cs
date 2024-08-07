using System;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to represent a collection of numbered paragraphs in OpenXml document.
/// </summary>
/// <param name="instance"></param>
public class NumberingList(DXW.NumberingInstance instance) : List<DXW.Paragraph>
{
  /// <summary>
  /// OpenXml numbering instance.
  /// </summary>
  public DXW.NumberingInstance Instance { get; internal set; } = instance;

  /// <summary>
  /// Abstract numbering definition.
  /// </summary>
  public DXW.AbstractNum AbstractNum => _AbstractNum ??= Instance.GetAbstractNum()!;
  private DXW.AbstractNum? _AbstractNum;

  /// <summary>
  /// Method to get the numbering string of the paragraph.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public string? GetNumberingString(DXW.Paragraph paragraph, GetTextOptions? options = null)
  {
    var index = IndexOf(paragraph);
    if (index == -1) return null;
    index++;
    if (options == null)
      options = GetTextOptions.Default;
    var level = paragraph.GetNumberingLevel();
    var aLevel = AbstractNum.Elements<DXW.Level>().FirstOrDefault(l => l.LevelIndex?.Value == level);
    if (aLevel == null) return null;
    var aText = aLevel.LevelText?.Val?.Value;
    if (aText == null) return null;
    var numberFormat = aLevel.NumberingFormat?.Val?.Value;
    if (numberFormat == null) return null;
    var numberStr = "";
    if (numberFormat == NumberFormatValues.Decimal)
    {
      numberStr = index.ToString();
    }
    else if (numberFormat == NumberFormatValues.Bullet)
    {
      numberStr = "•";
    }
    else if (numberFormat == NumberFormatValues.LowerLetter)
    {
      numberStr = ((char)('a' + index)).ToString();
    }
    else if (numberFormat == NumberFormatValues.UpperLetter)
    {
      numberStr = ((char)('A' + index)).ToString();
    }
    else if (numberFormat == NumberFormatValues.LowerRoman)
    {
      numberStr = ToRoman(index + 1).ToLower();
    }
    else if (numberFormat == NumberFormatValues.UpperRoman)
    {
      numberStr = ToRoman(index + 1);
    }
    aText = aText.Replace("%1", numberStr);
    if (aLevel.LevelSuffix?.Val?.Value == LevelSuffixValues.Space)
      aText += " ";
    else
    if (aLevel.LevelSuffix?.Val?.Value == LevelSuffixValues.Tab)
      aText += options.TabTag;
    if (aLevel.LevelSuffix?.Val?.Value != LevelSuffixValues.Nothing)
      aText += options.NumberingEndTag;
    aText= options.NumberingStartTag + aText;
    if (options.IndentNumberingLists && level!=null)
    {
      var indentStr = options.Indent.Duplicate((int)level) ?? "";
      aText = indentStr + aText;
    }
    return aText;
  }

  /// <summary>
  /// Converts a number to a Roman numeral.
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public string ToRoman(int number)
  {
    if ((number < 0) || (number > 3999))
      throw new ArgumentOutOfRangeException(nameof(number),"ToRoman value is out of range");
    if (number < 1)
      return "";
    if (number >= 1000) return "M" + ToRoman(number - 1000);
    if (number >= 900) return "CM" + ToRoman(number - 900);
    if (number >= 500) return "D" + ToRoman(number - 500);
    if (number >= 400) return "CD" + ToRoman(number - 400);
    if (number >= 100) return "C" + ToRoman(number - 100);
    if (number >= 90) return "XC" + ToRoman(number - 90);
    if (number >= 50) return "L" + ToRoman(number - 50);
    if (number >= 40) return "XL" + ToRoman(number - 40);
    if (number >= 10) return "X" + ToRoman(number - 10);
    if (number >= 9) return "IX" + ToRoman(number - 9);
    if (number >= 5) return "V" + ToRoman(number - 5);
    if (number >= 4) return "IV" + ToRoman(number - 4);
    return "I" + ToRoman(number - 1);
  }
}
