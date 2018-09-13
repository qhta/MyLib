using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Diagnostics;

namespace MyLib.WpfUtils
{
  public class DoubleValueConverter: IValueConverter
  {
    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return 0;
      else if (value is string)
      {
        string s = value as string;
        s = s.Trim ();
        if (s.Length==0)
          return 0;
        double result;
        if (Double.TryParse (s, NumberStyles.Float, culture, out result))
          return result;
        else
          return Double.NaN;
      }
      else
        throw new NotImplementedException ();
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (targetType == typeof (string))
      {
        double d = (double)value;
        if (!Double.IsNaN(d))
          return (d.ToString (culture));
        return "";
      }
      else
        throw new NotImplementedException ();
    }
  }
}
