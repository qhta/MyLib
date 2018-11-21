using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// Konwerter szerokości obramowania 3D
  /// </summary>
  public class Border3DThicknessConverter: IValueConverter
  {
    /// <summary>
    /// Konwersja wprost
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("Border3DThicknessConverter value should not be null");
      if (!(value is Thickness))
        throw new ArgumentException("Border3DThicknessConverter value should be of Thickness class");
      if (parameter == null)
        throw new ArgumentNullException("Border3DThicknessConverter parameter should not be null");
      Thickness input = (Thickness)value;
      if (!(parameter is string))
        throw new ArgumentException("Border3DThicknessConverter parameter should be of string class");
      string layout = (string)parameter;
      double left = (layout.Contains("L")) ? input.Left: 0;
      double top = (layout.Contains("T")) ? input.Top: 0;
      double right = (layout.Contains("R")) ? input.Right: 0;
      double bottom = (layout.Contains("B")) ? input.Bottom : 0;
      return new Thickness(left, top, right, bottom);
    }

    /// <summary>
    /// Konwersja wstecz
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException("Border3DThicknessConverter.ConvertBack is not implemented");
    }

  }
}
