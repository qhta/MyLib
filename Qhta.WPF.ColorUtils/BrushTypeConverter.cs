using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Diagnostics;
using System.Linq;

namespace Qhta.WPF
{

  public class BrushTypeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value==null)
        return null;
      if (targetType==typeof(System.Windows.Media.Brush))
      {
        if (value is System.Windows.Media.Brush Brush)
          return Brush;
        throw new InvalidOperationException($"Cannot convert {value.GetType()} to Brush");
      }
      if (targetType==typeof(GradientBrush))
      {
        if (value is GradientBrush gradientBrush)
          return gradientBrush;
        throw new InvalidOperationException($"Cannot convert {value.GetType()} to GradientBrush");
      }
      throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }

}


