using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class DecimalValueConverter: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is decimal val)
        return val.ToString(culture);
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string str)
      {
        if (Decimal.TryParse(str, NumberStyles.Any, culture, out var val))
          return val;
      }
      return null;
    }
  }
}
