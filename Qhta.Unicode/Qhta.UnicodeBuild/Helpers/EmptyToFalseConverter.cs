using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Converts empty or whitespace strings to false value, otherwise true.
/// </summary>
public class EmptyToFalseConverter : IValueConverter
{
  /// <summary>
  /// Converts an empty or whitespace string to false; otherwise, returns true.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    var str = value as string;
    return string.IsNullOrWhiteSpace(str);
  }


  /// <summary>
  /// Unimplemented. Converts a value back to its original form.
  /// </summary>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}