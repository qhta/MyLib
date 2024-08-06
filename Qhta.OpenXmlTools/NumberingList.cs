using DocumentFormat.OpenXml.Wordprocessing;

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
    //else if (numberFormat == NumberFormatValues.LowerRoman)
    //{
    //  numberStr = ToRoman(index + 1).ToLower();
    //}
    //else if (numberFormat == NumberFormatValues.UpperRoman)
    //{
    //  numberStr = ToRoman(index + 1);
    //}
    aText = aText.Replace("%1", numberStr);
    if (aLevel.LevelSuffix?.Val?.Value == LevelSuffixValues.Space)
      aText = aText + " ";
    else
    if (aLevel.LevelSuffix?.Val?.Value != LevelSuffixValues.Nothing)
      aText = aText + options.TabTag;
    return aText;
  }
}
