using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way multi-value converter that invokes an indexing property from the first value item.
  /// The second value item is an index.
  /// </summary>
  public class IndexingConverter: IMultiValueConverter
  {
    /// <summary>
    /// Invokes an indexing property from the first value item.
    /// The second value item is an index.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var obj = values[0];
      var indexProp = values[1];
      if (obj != DependencyProperty.UnsetValue)
      {
        string str = indexProp as string ?? string.Empty;
        if (indexProp is Binding)
        {

        }
        foreach (PropertyInfo property in obj.GetType().GetProperties())
        {
          ParameterInfo[] parameterInfos = property.GetIndexParameters();
          if (parameterInfos.Length == 1)
          {
            return property.GetValue(obj, new object[] { str });
          }
        }
      }
      return null;
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
