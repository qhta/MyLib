using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


namespace MyLib.WpfUtils
{
  public class String2ObjectConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        var val = value.ToString();
        if (Dictionary.TryGetValue(val, out object result))
          return result;
        return Dictionary[val.ToLowerInvariant()];
      }
      else
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }

    public String2ObjectDictionary Dictionary { get; set; }
  }
}
