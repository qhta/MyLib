namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with numbering levels.
/// </summary>
public static class LevelTools
{
  /// <summary>
  /// Determines if the level format is bulleted.
  /// </summary>
  /// <param name="level">Level to examine</param>
  /// <returns></returns>
  public static bool IsBulleted(this DXW.Level level)
  {
    return level.NumberingFormat?.Val?.Value == DXW.NumberFormatValues.Bullet;
  }

  /// <summary>
  /// Determines if the level format is decimal.
  /// </summary>
  /// <param name="level">Level to examine</param>
  /// <returns></returns>
  public static bool IsDecimal(this DXW.Level level)
  {
    return level.NumberingFormat?.Val?.Value == DXW.NumberFormatValues.Decimal;
  }

  /// <summary>
  /// Determines if the level format is lower letter.
  /// </summary>
  /// <param name="level">Level to examine</param>
  /// <returns></returns>
  public static bool IsLowerLetter(this DXW.Level level)
  {
    return level.NumberingFormat?.Val?.Value == DXW.NumberFormatValues.LowerLetter;
  }

  /// <summary>
  /// Determines if the level format is upper letter.
  /// </summary>
  /// <param name="level">Level to examine</param>
  /// <returns></returns>
  public static bool IsUpperLetter(this DXW.Level level)
  {
    return level.NumberingFormat?.Val?.Value == DXW.NumberFormatValues.UpperLetter;
  }

  /// <summary>
  /// Determines if the level format is lower roman.
  /// </summary>
  /// <param name="level">Level to examine</param>
  /// <returns></returns>
  public static bool IsLowerRoman(this DXW.Level level)
  {
    return level.NumberingFormat?.Val?.Value == DXW.NumberFormatValues.LowerRoman;
  }

  /// <summary>
  /// Determines if the level format is upper roman.
  /// </summary>
  /// <param name="level">Level to examine</param>
  /// <returns></returns>
  public static bool IsUpperRoman(this DXW.Level level)
  {
    return level.NumberingFormat?.Val?.Value == DXW.NumberFormatValues.UpperRoman;
  }

  /// <summary>
  /// Determines if the level formatting is compatible with the numbering string.
  /// </summary>
  /// <param name="level"></param>
  /// <param name="numberingString"></param>
  /// <returns></returns>
  public static bool IsCompatibleWith(this DXW.Level level, string numberingString)
  {
    var lastNumberingChar = numberingString.LastOrDefault();
    var formattingText = level.LevelText?.Val?.Value;
    var lastFormatingChar = formattingText?.LastOrDefault();
    if (lastNumberingChar == '\0' || lastFormatingChar == '\0')
      return false;
    if (lastNumberingChar != lastFormatingChar)
      return false;
    if (level.IsBulleted())
      return numberingString == formattingText;
    var numberString = numberingString.Substring(0, numberingString.Length - 1);
    if (level.IsDecimal())
      return int.TryParse(numberString, out _);
    if (level.IsLowerLetter())
      return numberString.Length==1 && char.IsLower(numberString.First()) && numberString.First() != 'i';
    if (level.IsUpperLetter())
      return numberString.Length == 1 && char.IsUpper(numberString.First()) && numberString.First() != 'I';
    if (level.IsLowerRoman())
      return RomanNumeralConverter.FromRoman(numberString, UpperLower.Lower).HasValue;
    if (level.IsUpperRoman())
      return RomanNumeralConverter.FromRoman(numberString, UpperLower.Upper).HasValue;
    return false;
  }
}