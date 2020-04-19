using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Qhta.TextUtils;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// String value converter "camel string" <=> "CamelString"
  /// </summary>
  public class CamelStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value as string).DeCamelCase();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value as string).CamelCase();
    }

  }
}
