using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Converts a numeric value to a negate numeric value.
  /// </summary>
  public class NegateValueConverter : IValueConverter
  {

    /// <summary>
    /// Direct conversion.
    /// </summary>
    public object? Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException ("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Reverse conversion.
    /// </summary>
    public object? ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Internal convert method.
    /// </summary>
    private object? DoConvert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double && targetType == typeof(string))
        return (-(double)value).ToString(CultureInfo.InvariantCulture);
      if (value is double && targetType == typeof(double))
        return (-(double)value);
      if (value is string && targetType == typeof(string))
        return (-Double.Parse((string)value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
      if (value is string && targetType == typeof(double))
        return (-Double.Parse((string)value, CultureInfo.InvariantCulture));
      if (value is decimal && targetType == typeof(string))
        return (-(decimal)value).ToString(CultureInfo.InvariantCulture);
      if (value is decimal && targetType == typeof(decimal))
        return (-(decimal)value);
      if (value is string && targetType == typeof(string))
        return (-decimal.Parse((string)value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
      if (value is string && targetType == typeof(decimal))
        return (-decimal.Parse((string)value, CultureInfo.InvariantCulture));
      if (value is double && targetType == typeof(decimal))
        return (-(decimal)value);
      if (value is decimal && targetType == typeof(double))
        return (-(double)value);
      return null;
    }


  }
}
