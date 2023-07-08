using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class MultiplyingConverter : ArithmeticConverter
  {

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Double multiplier = 0.0;
      if (!TryGetValue(parameter, out multiplier))
        multiplier=Param;
      Double multiplied = 0.0;
      if (!TryGetValue(value, out multiplied))
        return value;
      if (!Double.IsNaN(multiplied) && !Double.IsInfinity(multiplied) && !Double.IsNaN(multiplier) && !Double.IsInfinity(multiplier))
        return multiplied*multiplier;
      return value;
    }

  }
}
