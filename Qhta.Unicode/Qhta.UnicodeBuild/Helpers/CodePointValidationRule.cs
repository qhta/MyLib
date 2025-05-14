using System.Globalization;
using System.Windows.Controls;

namespace Qhta.UnicodeBuild.Helpers;

public class CodePointValidationRule : ValidationRule
{
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
  {
    if (value is string str)
    {
      if (str.Length>6)
        return new ValidationResult(false, "Too long value");
      if (!int.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
        return new ValidationResult(false, $"Invalid code point format: {str}");
    }

    return ValidationResult.ValidResult;
  }
}