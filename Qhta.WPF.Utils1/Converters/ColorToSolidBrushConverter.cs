using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  [ValueConversion(typeof(Color), typeof(Brush))]
  public class ColorToSolidColorBrushConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return new SolidColorBrush((Color)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }

}
