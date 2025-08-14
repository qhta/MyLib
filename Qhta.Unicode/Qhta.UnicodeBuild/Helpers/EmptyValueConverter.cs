using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using Qhta.SF.Tools;
using Qhta.SF.Tools.Resources;
using Qhta.UnicodeBuild.Resources;
using Strings = Qhta.SF.Tools.Resources.Strings;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Value converter that returns a string value or "empty" tag.
/// </summary>
public class EmptyValueConverter : IValueConverter
{

  /// <summary>
  /// Converts a value to a string representation.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value == null) return Strings.EmptyValue;
    return value.ToString();
  }

  /// <summary>
  /// Unimplemented method for converting back from the target type to the source type.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException("ConvertBack is not supported.");
  }
}