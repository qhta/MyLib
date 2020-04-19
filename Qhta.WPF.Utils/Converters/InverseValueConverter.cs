using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Konwerter wartości liczbowej na jej odwrotność
  /// </summary>
  public class InverseValueConverter : IValueConverter
  {

    /// <summary>
    /// Konwersja wprost
    /// </summary>
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException ("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Konwersja wstecz
    /// </summary>
    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Właściwa procedura konwersji
    /// </summary>
    private object DoConvert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double && targetType == typeof(string))
        return (1.0 / (double)value).ToString(CultureInfo.InvariantCulture);
      if (value is double && targetType == typeof(double))
        return (1.0 / (double)value);
      if (value is string && targetType == typeof(string))
        return (1.0 / Double.Parse((string)value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
      if (value is string && targetType == typeof(double))
        return (1.0 / Double.Parse((string)value, CultureInfo.InvariantCulture));
      if (value is decimal && targetType == typeof(string))
        return ((decimal)1.0 / (decimal)value).ToString(CultureInfo.InvariantCulture);
      if (value is decimal && targetType == typeof(decimal))
        return ((decimal)1.0 / (decimal)value);
      if (value is string && targetType == typeof(string))
        return ((decimal)1.0 / decimal.Parse((string)value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
      if (value is string && targetType == typeof(decimal))
        return ((decimal)1.0 / decimal.Parse((string)value, CultureInfo.InvariantCulture));
      if (value is double && targetType == typeof(decimal))
        return ((decimal)1.0 / (decimal)value);
      if (value is decimal && targetType == typeof(double))
        return ((double)1.0 / (double)value);
      return null;
    }


  }
}
