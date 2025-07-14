using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
///  Value converter for converting between a <see cref="CodePoint"/> and its string representation.
/// </summary>
public class CodePointValueConverter: IValueConverter
{
  /// <summary>
  /// Converts a <see cref="CodePoint"/> to its string representation in hexadecimal format.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is CodePoint cp)
    {
      return cp.ToString();
    }
    return null;
  }

  /// <summary>
  /// Converts a string representation of a code point back to a <see cref="CodePoint"/>.
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
      if (CodePoint.TryParse(str, out var result))
        return result;
      throw new FormatException($"Invalid code point format: {str}");
    }
    return null;
  }
}