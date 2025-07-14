using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Value converter for converting between a <see cref="CodeRange"/> and its string representation.
/// </summary>
public class CodeRangeValueConverter: IValueConverter
{
  /// <summary>
  /// Converts a <see cref="CodeRange"/> to its string representation..
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is CodeRange range)
    {
      return range.ToString();
    }
    return null;
  }

  /// <summary>
  /// Converts a string representation of a code range back to a <see cref="CodeRange"/>.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  /// <exception cref="FormatException"></exception>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is string str)
    {
      if (CodeRange.TryParse(str, out var result))
        return result;
      throw new FormatException(String.Format(Resources.Strings.InvalidCodeRangeFormat, str));
    }
    return null;
  }
}