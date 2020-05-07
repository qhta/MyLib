using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class DividingConverter : ArithmeticConverter
  {
    public double Param { get; set; } = Double.NaN;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Double divider = 0.0;
      if (!TryGetValue(parameter, out divider))
        divider=Param;
      Double divident = 0.0;
      if (!TryGetValue(value, out divident))
        return value;
      if (!Double.IsNaN(divident) && !Double.IsInfinity(divident) && !Double.IsNaN(divider) && !Double.IsInfinity(divider) && divider!=0.0)
        return divident/divider;
      return value;
    }

  }
}
