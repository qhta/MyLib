using System;
using System.Globalization;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Appending converter that adds a string parameter to the string value.
  /// </summary>
  public class AppendingConverter : StringConverter
  {
    /// <summary>
    /// Adds a double parameter to the double value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string? value2;
      TryGetValue(parameter ?? Param, out value2);
      return value.ToString() + value2;
    }

  }
}
