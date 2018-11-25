using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  [ValueConversion(typeof(Color), typeof(Brush))]
  [ValueConversion(typeof(Color), typeof(String))]
  public class ColorConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Color)
      {
        if (targetType==typeof(string))
          return mediaConverter.ConvertToString(value);
        else if (targetType==typeof(Brush))
          return new SolidColorBrush((Color)value);
      }
      throw new NotImplementedException();
    }

    private System.Windows.Media.ColorConverter mediaConverter = new System.Windows.Media.ColorConverter();

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (targetType==typeof(Color))
      {
        if (value is string)
          return System.Windows.Media.ColorConverter.ConvertFromString((string)value);
      }
      throw new NotImplementedException();
    }

    #endregion
  }

}
