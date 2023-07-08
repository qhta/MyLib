using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{

  /// <summary>
  /// Converts a double value to string and back.
  /// </summary>
  public class DoubleValueConverter : IValueConverter
  {
    /// <summary>
    /// If value is string and target type is double then it converts string to double.
    /// If value is double and target type is string then it converts double to string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return null;
      else if (value is string s && targetType==typeof(double))
        return StringToDouble(s, parameter, culture);
      else if (value is double d  && targetType==typeof(string))
        return DoubleToString(d, parameter, culture);
      else
        throw new NotImplementedException();
    }

    /// <summary>
    /// If value is string and target type is double then it converts string to double.
    /// if value is double and target type is string then it converts double to string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return null;
      else if (value is string s && targetType==typeof(double))
        return StringToDouble(s, parameter, culture);
      else if (value is double d  && targetType==typeof(string))
        return DoubleToString(d, parameter, culture);
      else
        throw new NotImplementedException();
    }

    private double StringToDouble(string s, object parameter, System.Globalization.CultureInfo culture)
    {
      s = s.Trim();
      if (s.Length == 0)
        return 0;
      double result;
      if (Double.TryParse(s, NumberStyles.Float, culture, out result))
        return result;
      else
        return Double.NaN;
    }

    private string DoubleToString(double d, object parameter, System.Globalization.CultureInfo culture)
    {
      if (!Double.IsNaN(d))
        return (d.ToString(culture));
      return string.Empty;
    }
  }
}
