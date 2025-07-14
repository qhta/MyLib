using System.Globalization;
using System.Windows.Controls;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Validation rule for checking if a string is a valid Unicode code point.
/// </summary>
public class CodePointValidationRule : ValidationRule
{
  /// <summary>
  /// Validates the input value to ensure it is a valid Unicode code point.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="cultureInfo"></param>
  /// <returns></returns>
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
  {
    if (value is string str)
    {
      if (str.Length>6)
        return new ValidationResult(false, Resources.Strings.TooLongValue);
      if (!int.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
        return new ValidationResult(false, String.Format(Resources.Strings.InvalidCodePointFormat, str));
    }

    return ValidationResult.ValidResult;
  }
}