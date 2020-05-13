using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Qhta.WPF.Utils
{
  public class StringFormatConverter: DependencyObject, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null && parameter is String format)
      {
        return String.Format(format,value);
      }
      else
      if (value != null && Format!=null)
      {
        return String.Format(Format, value);
      }
      else
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }

    public static DependencyProperty FormatProperty = DependencyProperty.Register(
      "Format", typeof(string), typeof(StringFormatConverter));

    public string Format
    {
      get => (string)GetValue(FormatProperty);
      set => SetValue(FormatProperty, value);
    }

  }
}
