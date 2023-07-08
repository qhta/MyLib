using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Qhta.TextUtils;

namespace Qhta.WPF.Utils
{
  public class StringsWidthConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var ss = value as IEnumerable<string>;
      double width = 0;
      foreach (var s in ss)
      {
        var w = s.TextWidth();
        if (w > width)
          width = w;
      }

      if (parameter is string && Double.TryParse(parameter as string, out double dw))
      {
        width += dw;
      }
      return width;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException("StringsWidthConverter.ConvertBack not implemented");
    }

  }
}
