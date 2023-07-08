using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way converter from DataGridRow index to integer+1.
  /// </summary>
  public class RowToIndexConverter : IValueConverter
  {

    #region IValueConverter Members

    /// <summary>
    /// Converts from DataGridRow index to integer+1.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is DataGridRow row)
      {
        return row.GetIndex() + 1;
      }
      return null;
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

    #endregion
  }
}
