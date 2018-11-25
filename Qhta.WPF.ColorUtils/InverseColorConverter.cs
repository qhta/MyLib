using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF
{
  /// <summary>
  /// Konwerter koloru na jego negatyw
  /// </summary>
  public class InverseColorConverter : IValueConverter
  {

    /// <summary>
    /// Konwersja wprost
    /// </summary>
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException ("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Konwersja wstecz
    /// </summary>
    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      return DoConvert(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Właściwa procedura konwersji
    /// </summary>
    private object DoConvert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Color color)
      {
        var invColor = color.Inverse(Contrast);
        if (targetType==typeof(System.Windows.Media.Color))
          return invColor;
        if (targetType==typeof(System.Windows.Media.Brush))
          return new SolidColorBrush(invColor);
        throw new ArgumentException("InverseColorConverter target type must be System.Windows.Media.Color or Brush");
      }
      throw new ArgumentException("InverseColorConverter argument must be of System.Windows.Media.Color type");
    }

    public bool Contrast
    {
      get; set;
    }

  }
}
