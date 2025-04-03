using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Converts a true value to false and false to true value.
  /// </summary>
  public class NegateBoolConverter : IValueConverter
  {

    /// <summary>
    /// Direct conversion.
    /// </summary>
    public object? Convert (object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException ("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Reverse conversion.
    /// </summary>
    public object? ConvertBack (object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Internal convert method.
    /// </summary>
    private object? DoConvert (object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is bool vb)
        return !vb;
      return null;
    }


  }
}
