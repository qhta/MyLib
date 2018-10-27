using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Qhta.WPF.Utils
{
  [Bindable(true)]
  [ContentProperty(nameof(Dictionary))]
  public class String2ObjectConverter: IValueConverter
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
