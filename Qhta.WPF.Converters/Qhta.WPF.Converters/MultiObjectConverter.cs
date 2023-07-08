using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way multi-value converter that clones value objects.
  /// </summary>
  public class MultiObjectConverter : IMultiValueConverter
  {

    /// <summary>
    /// Clones values.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values.Clone();
    }

    /// <summary>
    /// Unimplemented backward conversion.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetTypes"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
