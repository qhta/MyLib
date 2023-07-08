using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Qhta.WPF.Utils
{
  public class StringFormatConverter: DependencyObject, IValueConverter, IMultiValueConverter
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
        return value?.ToString();
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values != null && parameter is String format)
      {
        return String.Format(format, values);
      }
      else
      if (values != null && Format != null)
      {
        return String.Format(Format, values);
      }
      else if (values != null)
        return String.Join(" ", values.Select(item => item?.ToString()));
      else
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException("StringFormatConverter.ConvertBack not implemented");
    }

    public static DependencyProperty FormatProperty = DependencyProperty.Register(
      "Format", typeof(string), typeof(StringFormatConverter));

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException("StringFormatConverter.ConvertBack not implemented");
    }

    public string Format
    {
      get => (string)GetValue(FormatProperty);
      set => SetValue(FormatProperty, value);
    }

  }
}
