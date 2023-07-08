using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class AddingConverter : ArithmeticConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Double addient = 0.0;
      if (!TryGetValue(parameter, out addient))
        addient=Param;
      Double component = 0.0;
      if (!TryGetValue(value, out component))
        return value;
      if (!Double.IsNaN(component) && !Double.IsInfinity(component) && !Double.IsNaN(addient) && !Double.IsInfinity(addient) && addient!=0.0)
        return component + addient;
      return value;
    }

  }
}
