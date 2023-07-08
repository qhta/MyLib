using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{

  /// <summary>
  /// One-way converter that safely converts collection to an array.
  /// </summary>
  public class SafeCollectionConverter : IValueConverter
  {
    /// <summary>
    /// Safely converts collection to an array.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is ICollection collection)
      {
        object[] array = new object[collection.Count];
        collection.CopyTo(array, 0);
        return array;
      }
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }
  }
}
