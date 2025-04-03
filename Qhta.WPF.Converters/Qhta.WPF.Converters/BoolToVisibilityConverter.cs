using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One way value converter bool to Visibility (Visible, Collapsed).
  /// Parameter can be a string consisting of two Visibility values divided by comma.
  /// First value is assigned when true, second when false.
  /// </summary>
  public class BoolToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Converts true to Visible, false to Collapsed.
    /// Parameter can be a string consisting of two Visibility values divided by comma.
    /// First value is assigned when true, second when false.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is bool bv)
      {
        if (parameter is string spr)
        {
          var ss = spr.Split(',');
          if (ss.Length == 2)
          {
            if (bv)
            {
              if (Enum.TryParse<Visibility>(ss[0], out Visibility visibilityOnTrue))
                return visibilityOnTrue;
            }
            else
            {
              if (Enum.TryParse<Visibility>(ss[1], out Visibility visibilityOnFalse))
                return visibilityOnFalse;
            }
          }
        }
        return bv ? Visibility.Visible : Visibility.Collapsed;
      }
      return null;
    }

    /// <summary>
    /// Unimplemented convert back method.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }
}
