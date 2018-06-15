using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MyLib.WpfUtils
{
  public class DividingConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Double divider = 0.0;
      if (!TryGetValue(parameter, out divider))
        return value;
      Double divident = 0.0;
      if (!TryGetValue(value, out divident))
        return value;
      if (!Double.IsNaN(divident) && !Double.IsInfinity(divident) && !Double.IsNaN(divider) && !Double.IsInfinity(divider) && divider!=0.0)
        return divident/divider;
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private bool TryGetValue(object parameter, out double value)
    {
      value = double.NaN;
      if (parameter is Double)
        value = (Double)parameter;
      else if (parameter is Single)
        value = (Single)parameter;
      else if (parameter is Int32)
        value = (Int32)parameter;
      else if (parameter is Int16)
        value = (Int16)parameter;
      else if (parameter is Byte)
        value = (Byte)parameter;
      else if (parameter is Int64)
        value = (Int64)parameter;
      else if (parameter is UInt64)
        value = (UInt64)parameter;
      else if (parameter is UInt32)
        value = (UInt32)parameter;
      else if (parameter is UInt16)
        value = (UInt16)parameter;
      else if (parameter is SByte)
        value = (SByte)parameter;
      else
        return false;
      return true;
    }
  }
}
