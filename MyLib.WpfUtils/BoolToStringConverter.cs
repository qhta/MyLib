using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


namespace MyLib.WpfUtils
{
  public class BoolToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var str = value.ToString();
      if (Dictionary.TryGetValue(str, out string result))
        return result;
      return Dictionary[str.ToLowerInvariant()];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public BoolToStringDictionary Dictionary { get; set; }
  }
}
