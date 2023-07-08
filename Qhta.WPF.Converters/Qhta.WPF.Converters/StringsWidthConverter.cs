using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way converter thar gets a pixel width of the string.
  /// </summary>
  public class StringsWidthConverter : IValueConverter
  {
    /// <summary>
    /// Gets a string text width.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double width = Double.NaN;
      if (value is IEnumerable<string> strings)
      {
        var ss = strings;
        width = 0;
        foreach (var s in ss)
        {
          var w = s.TextWidth();
          if (w > width)
            width = w;
        }
      }
      if (parameter is string && Double.TryParse(parameter as string, out double dw))
      {
        width += dw;
      }
      return width;
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
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException("StringsWidthConverter.ConvertBack not implemented");
    }

  }
}
