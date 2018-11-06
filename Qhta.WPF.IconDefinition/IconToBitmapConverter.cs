using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF
{
  public class IconToBitmapConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      //Debug.WriteLine($"Convert {value} to {targetType}");
      if (value==null)
        return null;
      if (!(value is IconDef iconDef))
        throw new NotSupportedException($"Unsupported conversion of {value}. IconDef expected");
      if (!(parameter is int size))
      {
        if (!(parameter is string str) || !int.TryParse(str, out size))
          throw new NotSupportedException($"Parameter must be an integer size during IconDef conversion");
      }
      if (targetType==typeof(ImageSource))
      {
        return iconDef.GetBitmapSource(size);
      }
      else
        throw new NotSupportedException($"Unsupported conversion of IconDef to {targetType}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Debug.WriteLine($"ConvertBack {value} to {targetType}");
      throw new NotSupportedException("No backward conversion of IconDef");
    }
  }
}
