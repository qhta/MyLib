using System;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class FloatDoubleFormater : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double x = (double)value;
      return x.ToString((string)parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
