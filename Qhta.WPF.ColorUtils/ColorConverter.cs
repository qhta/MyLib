using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF
{
  [ValueConversion(typeof(System.Windows.Media.Color), typeof(System.Windows.Media.Brush))]
  [ValueConversion(typeof(System.Windows.Media.Color), typeof(String))]
  public class ColorConverter : TypeConverter, IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is System.Windows.Media.Color color)
      {
        if (targetType==typeof(string))
          return MediaColorToString(color, (string)parameter);
        else if (targetType==typeof(System.Windows.Media.Brush))
          return new SolidColorBrush(color);
        else if (targetType==typeof(decimal))
        {
          var d = (decimal)MediaColorToNumber(color, (string)parameter);
          //Debug.WriteLine($"hue({color})={d}");
          return d;
        }
        else if (targetType==typeof(int))
          return (int)MediaColorToNumber(color, (string)parameter);
        else if (targetType==typeof(uint))
          return (uint)MediaColorToNumber(color, (string)parameter);
        else if (targetType==typeof(byte))
          return (byte)MediaColorToNumber(color, (string)parameter);
      }
      else
      if (value is AhsvColor hsvColor)
      {
        if (targetType == typeof(System.Windows.Media.Color))
          return ColorHSVToMediaColor(hsvColor);
      }
      else if (value is System.Windows.Media.Brush brush)
        if (targetType == typeof(System.Windows.Media.Color))
          return BrushToColor(brush);
      throw new NotImplementedException();
    }

    private static System.Windows.Media.ColorConverter mediaConverter = new System.Windows.Media.ColorConverter();

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (targetType==typeof(System.Windows.Media.Color))
      {
        if (value is string)
          return System.Windows.Media.ColorConverter.ConvertFromString((string)value);
      }
      if (targetType==typeof(System.Windows.Media.Brush))
      {
        if (value is string)
          value = System.Windows.Media.ColorConverter.ConvertFromString((string)value);
        if (value is System.Windows.Media.Color color)
          return new SolidColorBrush(color);
      }
      throw new NotImplementedException();
    }

    #endregion

    #region TypeConverter override
    public override bool CanConvertFrom(ITypeDescriptorContext td, Type t)
    {
      return mediaConverter.CanConvertFrom(td, t);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return mediaConverter.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext td, CultureInfo ci, object value)
    {
      return mediaConverter.ConvertFrom(td, ci, value);
    }

    public static new object ConvertFromString(string value)
    {
      return System.Windows.Media.ColorConverter.ConvertFromString((string)value);
    }
    #endregion;

    public static System.Drawing.Color MediaColorToDrawingColor(System.Windows.Media.Color color)
    {
      return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static System.Windows.Media.Color DrawingColorToMediaColor(System.Drawing.Color color)
    {
      return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static string MediaColorToString(System.Windows.Media.Color color, string format=null)
    {
      if (format != null)
      {
        var str = format;
        str=str.ToUpperInvariant();
        if (!str.Contains(":"))
          str+=":D";
        var dc = color.ToDrawingColor();
        if (str=="ARGB:X")
          return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", dc.A, dc.R, dc.G, dc.B);
        if (str=="ARGB:D")
          return String.Format("{0} {1} {2} {3}", dc.A, dc.R, dc.G, dc.B);
        if (str=="A:X")
          return String.Format("{0:X2}", dc.A);
        if (str=="A:D")
          return String.Format("{0}", dc.A);
        if (str=="R:X")
          return String.Format("{0:X2}", dc.R);
        if (str=="R:D")
          return String.Format("{0}", dc.R);
        if (str=="G:X")
          return String.Format("{0:X2}", dc.G);
        if (str=="G:D")
          return String.Format("{0}", dc.G);
        if (str=="B:X")
          return String.Format("{0:X2}", dc.B);
        if (str=="B:D")
          return String.Format("{0}", dc.B);
        var HSV = dc.ToAhsv();
        if (str=="H:X")
          return String.Format("{0:X2}", (int)(HSV.H*255));
        if (str=="H:D")
          return String.Format("{0}", (int)(HSV.H*360));
        if (str=="V:X")
          return String.Format("{0:X2}", (int)(HSV.V*255));
        if (str=="V:D")
          return String.Format("{0}", (int)(HSV.V*255));
        if (str=="S:X")
          return String.Format("{0:X2}", (int)(HSV.S*255));
        if (str=="S:D")
          return String.Format("{0}", (int)(HSV.S*255));
      }
      return mediaConverter.ConvertToString(color);
    }

    public static uint MediaColorToNumber(System.Windows.Media.Color color, string format = null)
    {
      var dc = color.ToDrawingColor();
      if (format != null)
      {
        var str = format;
        str=str.ToUpperInvariant();
        int k = str.IndexOf(':');
        if (k>0)
          str = str.Substring(0, k);
        if (str=="ARGB")
          return (uint)dc.A<<24 | (uint)dc.R<<16 | (uint)dc.G<<8 | (uint)dc.B;
        if (str=="A")
          return dc.A;
        if (str=="R")
          return dc.R;
        if (str=="G")
          return dc.G;
        if (str=="B")
          return dc.B;
        var HSV = dc.ToAhsv();
        if (str=="H")
          return (uint)(HSV.H*360);
        if (str=="V")
          return (uint)(HSV.V*255);
        if (str=="S")
          return (uint)(HSV.S*255);
      }
      return (uint)dc.A<<24 | (uint)dc.R<<16 | (uint)dc.G<<8 | (uint)dc.B;
    }
    public static System.Windows.Media.Color ColorHSVToMediaColor(AhsvColor hsvColor)
    {
      var dc = hsvColor.ToColor();
      return System.Windows.Media.Color.FromArgb(dc.A, dc.R, dc.G, dc.B);
    }

    public static System.Windows.Media.Color BrushToColor(System.Windows.Media.Brush brush)
    {
      if (brush is System.Windows.Media.SolidColorBrush solidBrush)
        return solidBrush.Color;
      if (brush is System.Windows.Media.GradientBrush gradientBrush)
        return gradientBrush.GradientStops.FirstOrDefault()?.Color ?? Colors.Transparent;
      throw new InvalidOperationException($"Cannot convert from {brush.GetType()} to Color");
    }
  }

}


