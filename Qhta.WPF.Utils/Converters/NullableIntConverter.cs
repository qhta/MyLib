using System;
using System.Globalization;
using System.Windows.Data;


namespace Qhta.WPF.Utils
{
  public class NullableIntConverter : IValueConverter
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
        if (int.TryParse(value.ToString(), out int val))
          return val;
      }
      return null;
    }

  }
}
