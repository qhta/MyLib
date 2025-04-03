using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Converts a decimal value to string and back.
  /// </summary>
  public class DecimalValueConverter : IValueConverter
  {
    /// <summary>
    /// If value is string and target type is decimal then it converts string to decimal.
    /// If value is decimal and target type is string then it converts decimal to string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return null;
      else if (value is string s && targetType==typeof(decimal))
        return StringToDouble(s, parameter, culture);
      else if (value is decimal d  && targetType==typeof(string))
        return DoubleToString(d, parameter, culture);
      else
        throw new NotImplementedException();
    }

    /// <summary>
    /// If value is string and target type is decimal then it converts string to decimal.
    /// if value is decimal and target type is string then it converts decimal to string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return null;
      else if (value is string s && targetType==typeof(decimal))
        return StringToDouble(s, parameter, culture);
      else if (value is decimal d  && targetType==typeof(string))
        return DoubleToString(d, parameter, culture);
      else
        throw new NotImplementedException();
    }

    private decimal? StringToDouble(string s, object? parameter, System.Globalization.CultureInfo culture)
    {
      s = s.Trim();
      if (s.Length == 0)
        return 0;
      if (decimal.TryParse(s, NumberStyles.Float, culture, out var result))
        return result;
      else
        return null;
    }

    private string DoubleToString(decimal d, object? parameter, System.Globalization.CultureInfo culture)
    {
      return (d.ToString(culture));
    }
  }
}
