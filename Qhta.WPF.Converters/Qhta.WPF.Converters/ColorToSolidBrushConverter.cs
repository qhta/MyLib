using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way color to brush converter.
  /// </summary>
  [ValueConversion(typeof(Color), typeof(Brush))]
  public class ColorToSolidColorBrushConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Converts color to brush.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return new SolidColorBrush((Color)value);
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
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }

}
