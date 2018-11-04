using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Qhta.Drawing
{
  public static class BrushUtils
  {
    public static bool CanSerializeToString(this Brush brush)
    {
      return brush is SolidBrush;
    }

    public static string ConvertToString(this Brush brush, string format, IFormatProvider provider)
    {
      if (brush is SolidBrush solidBrush)
      {
        return solidBrush.Color.ToString();
      }
      throw new NotImplementedException("Only SolidBrush can be converted to string");
    }

    public static Brush Parse(string value, ITypeDescriptorContext context)
    {
      Brush brush = Parsers.ParseBrush(value, CultureInfo.InvariantCulture, context);
      return brush;
    }

  }
}

