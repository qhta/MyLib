using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

public class CodePointValueConverter: IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is CodePoint cp)
    {
      return cp.ToString();
    }
    return null;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is string str)
    {
      if (CodePoint.TryParse(str, out var result))
        return result;
      throw new FormatException($"Invalid code point format: {str}");
    }
    return null;
  }
}