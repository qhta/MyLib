using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public abstract class ArithmeticConverter: IValueConverter
  {
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    protected bool TryGetValue(object parameter, out double value)
    {
      value = double.NaN;
      if (parameter is string str)
        return double.TryParse(str, out value);
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
