using System;
using System.Globalization;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Arithmetic converter that multiplies a double value by the double parameter.
  /// </summary>
  public class MultiplyingConverter : ArithmeticConverter
  {

    /// <summary>
    /// Multiplies a double value by the double parameter.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Double value2;
      if (!TryGetValue(parameter, out value2))
        value2=Param;
      Double value1;
      if (!TryGetValue(value, out value1))
        return value;
      if (!Double.IsNaN(value1) && !Double.IsInfinity(value1) && !Double.IsNaN(value2) && !Double.IsInfinity(value2))
        return value1*value2;
      return value;
    }

  }
}
