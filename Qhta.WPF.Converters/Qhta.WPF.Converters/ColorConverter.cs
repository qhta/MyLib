using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Converts a color to string or brush.
  /// </summary>
  [ValueConversion(typeof(Color), typeof(Brush))]
  [ValueConversion(typeof(Color), typeof(String))]
  public class ColorConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Converts a color to string or brush.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Color)
      {
        if (targetType==typeof(string))
          return mediaConverter.ConvertToString(value);
        else if (targetType==typeof(Brush))
          return new SolidColorBrush((Color)value);
      }
      throw new NotImplementedException();
    }

    private readonly System.Windows.Media.ColorConverter mediaConverter = new System.Windows.Media.ColorConverter();

    /// <summary>
    /// Converts from string to color.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
      if (targetType==typeof(Color))
      {
        if (value is string)
          return System.Windows.Media.ColorConverter.ConvertFromString((string)value);
      }
      throw new NotImplementedException();
    }

    #endregion
  }

}
