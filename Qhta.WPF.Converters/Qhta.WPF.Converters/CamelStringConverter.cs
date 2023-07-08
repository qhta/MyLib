using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Qhta.TextUtils;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// String value converter between "camel string" and "CamelString".
  /// </summary>
  public class CamelStringConverter : IValueConverter
  {
    /// <summary>
    /// Convert from "camel string" to "CamelString".
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value as string)?.DeCamelCase();
    }

    /// <summary>
    /// Convert from "CamelString" to "camel string".
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value as string)?.CamelCase();
    }

  }
}
