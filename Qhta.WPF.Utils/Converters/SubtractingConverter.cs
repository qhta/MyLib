using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class SubtractingConverter : ArithmeticConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Double subtrahend = 0.0;
      if (!TryGetValue(parameter, out subtrahend))
        subtrahend=Param;
      Double minuend = 0.0;
      if (!TryGetValue(value, out minuend))
        return value;
      if (!Double.IsNaN(minuend) && !Double.IsInfinity(minuend) && !Double.IsNaN(subtrahend) && !Double.IsInfinity(subtrahend) && subtrahend!=0.0)
        return minuend - subtrahend;
      return value;
    }

  }
}
