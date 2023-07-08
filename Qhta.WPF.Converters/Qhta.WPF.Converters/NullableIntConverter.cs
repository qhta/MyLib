using System;
using System.Globalization;
using System.Windows.Data;


namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Int value converter that converts null string to null int.
  /// </summary>
  public class NullableIntConverter : IValueConverter
  {
    /// <summary>
    /// Direct conversion.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        var val = value.ToString();
        return val;
      }
      else
        return null;
    }

    /// <summary>
    /// Backward conversion
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        if (int.TryParse(value.ToString(), out int val))
          return val;
      }
      return null;
    }

  }
}
