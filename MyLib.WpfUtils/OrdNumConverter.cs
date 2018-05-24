using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MyLib.OrdNumbers;

namespace MyLib.WpfUtils
{
  public class OrdNumConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        var val = value.ToString();
        return val;
      }
      else
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        var s = value.ToString().Trim();
        if (s!="")
          return (OrdNum)s;
      }
      return null;
    }

  }
}
