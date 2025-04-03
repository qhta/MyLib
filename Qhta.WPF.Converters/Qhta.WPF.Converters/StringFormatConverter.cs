using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way converter that converts a value using parameter as format.
  /// </summary>
  public class StringFormatConverter : DependencyObject, IValueConverter, IMultiValueConverter
  {

    /// <summary>
    /// Converts a value using parameter as format.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value != null && parameter is String format)
      {
        return String.Format(format, value);
      }
      else
      if (value != null && Format != null)
      {
        return String.Format(Format, value);
      }
      return value?.ToString();
    }

    /// <summary>
    /// Converts an array of values using parameter as format.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
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

    /// <summary>
    /// Unimplemented backward conversion.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException("StringFormatConverter.ConvertBack not implemented");
    }

    /// <summary>
    /// Unimplemented backward conversion.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetTypes"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException("StringFormatConverter.ConvertBack not implemented");
    }

    /// <summary>
    /// Static format dependency property.
    /// </summary>
    public static DependencyProperty FormatProperty = DependencyProperty.Register(
      "Format", typeof(string), typeof(StringFormatConverter));

    /// <summary>
    /// Dependency property.
    /// </summary>
    public string Format
    {
      get => (string)GetValue(FormatProperty);
      set => SetValue(FormatProperty, value);
    }

  }
}
