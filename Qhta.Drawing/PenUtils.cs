using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Qhta.Drawing
{
  public static class PenUtils
  {
    public static bool CanSerializeToString(this Pen pen)
    {
      return pen.Width==1
        && pen.PenType==PenType.SolidColor
        && pen.Brush.CanSerializeToString()
        //&& pen.Alignment == PenAlignment.Center
        //&& pen.StartCap==LineCap.Flat && pen.EndCap==LineCap.Flat
        //&& pen.LineJoin==LineJoin.Miter && pen.MiterLimit==0
        //&& pen.StartCap==LineCap.Flat && pen.EndCap==LineCap.Flat
        ;


    }

    public static string ConvertToString(this Pen pen, string format, IFormatProvider provider)
    {
      if (pen.CanSerializeToString())
      {
        return pen.Brush.ToString();
      }
      throw new NotImplementedException("Only SolidColor pen can be converted to string");
    }

    public static Pen Parse(string value, ITypeDescriptorContext context)
    {
      var brush = Parsers.ParseBrush(value, CultureInfo.InvariantCulture, context);
      Pen pen = new Pen(brush, 1);
      return pen;
    }

  }
}

