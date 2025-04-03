using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Qhta.WPF.Converters;

/// <summary>
///  Multi-value converter with two bindings. First binding returns an instance object.
///  Second binding returns a property name. Converter gets a value from this property found in the instance object.
/// </summary>
public class IndirectPropertyConverter : IMultiValueConverter
{
  /// <summary>
  /// Gets data from the property which name is returned by the second value. Property is searched in the first value.
  /// </summary>
  /// <param name="values"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
  {
    if (values.Length >= 2)
    {
      var instance = values[0];
      var propName = values[1].ToString();
      if (instance != null && propName != null)
      {
        var propInfo = instance.GetType().GetProperty(propName);
        if (propInfo!=null && propInfo.CanRead)
        {
          return propInfo.GetValue(instance, null);
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Unimplemented convert back method.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetTypes"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
