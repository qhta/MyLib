using System;
using System.Text;

namespace Qhta.OpenXmlTools;


/// <summary>
/// Required upper or lower case numbering.
/// </summary>
[Flags]
public enum UpperLower
{
  /// <summary>
  /// Requires upper case numbering.
  /// </summary>
  Upper = 1,
  /// <summary>
  /// Requires lower case numbering.
  /// </summary>
  Lower = 2
}
/// <summary>
/// Converts Roman numerals to integers and vice versa.
/// </summary>
public static class RomanNumeralConverter
{
  private static readonly Dictionary<char, int> RomanMap = new Dictionary<char, int>
  {
    {'I', 1},
    {'V', 5},
    {'X', 10},
    {'L', 50},
    {'C', 100},
    {'D', 500},
    {'M', 1000}
  };

  /// <summary>
  /// Converts a Roman numeral to an integer.
  /// If conversion is impossible, returns null.
  /// </summary>
  /// <param name="roman">converted string (without spaces)</param>
  /// <param name="caseRequirement">Upper/lower case requirement. To no requirement set to 0</param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  public static int? FromRoman(string roman, UpperLower caseRequirement)
  {
    if (string.IsNullOrEmpty(roman))
      return null;

    int total = 0;
    int prevValue = 0;

    foreach (char c in roman)
    {
      if (caseRequirement.HasFlag(UpperLower.Upper) && !char.IsUpper(c))
        return null;
      if (caseRequirement.HasFlag(UpperLower.Lower) && !char.IsLower(c))
        return null;
      var ch = char.ToUpper(c);
      if (!RomanMap.TryGetValue(ch, out int value))
        return null;

      if (value > prevValue)
      {
        total += value - 2 * prevValue; // Adjust for previous addition
      }
      else
      {
        total += value;
      }

      prevValue = value;
    }

    return total;
  }

  /// <summary>
  /// Converts an integer to a Roman numeral.
  /// If conversion is impossible, returns null.
  /// </summary>
  /// <param name="number">Number to convert</param>
  /// <param name="caseRequirement">Upper/lower case requirement. To no requirement set to 0</param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public static string? ToRoman(int number, UpperLower caseRequirement = UpperLower.Upper)
  {
    if (number < 1 || number > 3999)
      return null;

    StringBuilder result = new StringBuilder();

    var romanNumerals = new[]
    {
      new { Value = 1000, Numeral = "M" },
      new { Value = 900, Numeral = "CM" },
      new { Value = 500, Numeral = "D" },
      new { Value = 400, Numeral = "CD" },
      new { Value = 100, Numeral = "C" },
      new { Value = 90, Numeral = "XC" },
      new { Value = 50, Numeral = "L" },
      new { Value = 40, Numeral = "XL" },
      new { Value = 10, Numeral = "X" },
      new { Value = 9, Numeral = "IX" },
      new { Value = 5, Numeral = "V" },
      new { Value = 4, Numeral = "IV" },
      new { Value = 1, Numeral = "I" }
    };

    foreach (var item in romanNumerals)
    {
      while (number >= item.Value)
      {
        if (caseRequirement.HasFlag(UpperLower.Lower))
          result.Append(item.Numeral.ToLowerInvariant());
        else
          result.Append(item.Numeral);
        number -= item.Value;
      }
    }

    return result.ToString();
  }
}