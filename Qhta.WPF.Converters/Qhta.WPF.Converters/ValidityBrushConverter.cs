using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way converter that gets a color from string-to-color dictionary
  /// and converts it to solid brush.
  /// </summary>
  public class ValidityBrushConverter : IValueConverter
  {
    /// <summary>
    ///  Gets a color from string-to-color dictionary and converts it to solid brush.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var s = value.ToString();
      if (s!=null)
      {
        Color color = ColorDictionary[s];
        return new SolidColorBrush { Color = color };
      }
      else
      {
        Color color = ColorDictionary["True"];
        return new SolidColorBrush { Color = color };
      }
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
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// String to color dictionary. Index is "True" or "False".
    /// </summary>
    public ColorDictionary ColorDictionary { get; set; } = null!;
  }
}
