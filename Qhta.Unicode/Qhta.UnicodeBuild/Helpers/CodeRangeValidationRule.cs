using System.Globalization;
using System.Windows.Controls;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Validation rule for checking if a string is a valid code range.
/// </summary>
public class CodeRangeValidationRule : ValidationRule
{
  /// <summary>
  /// Validates the input value to ensure it is a valid code range in the format "XXXX..YYYY".
  /// </summary>
  /// <param name="value"></param>
  /// <param name="cultureInfo"></param>
  /// <returns></returns>
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
  {
    if (value is string str)
    {
      if (!CodeRange.TryParse(str, out var range))
        return new ValidationResult(false, String.Format(Resources.Strings.InvalidCodeRangeFormat, str));
      if (range != null && range.End.HasValue && range.End < range.Start)
        return new ValidationResult(false, "End value must be greater than or equal to Start value.");
    }

    return ValidationResult.ValidResult;
  }
}