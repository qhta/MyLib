using System.Globalization;
using System.Windows.Controls;

namespace Qhta.UnicodeBuild.Helpers;

public class CodeRangeValidationRule : ValidationRule
{
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
  {
    if (value is string str)
    {
      if (!CodeRange.TryParse(str, out var range))
        return new ValidationResult(false, "Invalid range format. Expected format: XXXX..YYYY.");
      if (range != null && range.End.HasValue && range.End < range.Start)
        return new ValidationResult(false, "End value must be greater than or equal to Start value.");
    }

    return ValidationResult.ValidResult;
  }
}