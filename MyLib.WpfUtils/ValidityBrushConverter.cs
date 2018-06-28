using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MyLib.WpfUtils
{
  public class ValidityBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        Color color = ColorDictionary[value.ToString()];
        return new SolidColorBrush { Color = color };
      }
      else
      {
        Color color = ColorDictionary["True"];
        return new SolidColorBrush { Color = color };
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public ColorDictionary ColorDictionary { get; set; }
  }
}
