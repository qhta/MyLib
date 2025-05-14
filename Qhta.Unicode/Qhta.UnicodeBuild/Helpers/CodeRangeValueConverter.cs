using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

public class CodeRangeValueConverter: IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is CodeRange range)
    {
      return range.ToString();
    }
    return null;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is string str)
    {
      if (CodeRange.TryParse(str, out var result))
        return result;
      throw new FormatException($"Invalid range format: {str}");
    }
    return null;
  }
}