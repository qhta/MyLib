using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Qhta.Drawing
{
  public sealed class PenConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return (!(sourceType == typeof(string)) ? base.CanConvertFrom(context, sourceType) : true);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (!(destinationType == typeof(string)))
      {
        return base.CanConvertTo(context, destinationType);
      }
      if ((context == null) || (context.Instance == null))
      {
        return true;
      }
      if (context.Instance is Pen)
      {
        return ((Pen)context.Instance).CanSerializeToString();
      }
      object[] args = new object[] { "Pen" };
      throw new ArgumentException(SR.Get("General_Expected_Type", args), "context");
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null)
      {
        throw base.GetConvertFromException(value);
      }
      string str = value as string;
      return ((str == null) ? base.ConvertFrom(context, culture, value) : PenUtils.Parse(str, context));
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if ((destinationType != null) && (value is Pen))
      {
        Pen Pen = (Pen)value;
        if (destinationType == typeof(string))
        {
          if (((context != null) && (context.Instance != null)) && !Pen.CanSerializeToString())
          {
            throw new NotSupportedException(SR.Get("Converter_ConvertToNotSupported"));
          }
          return Pen.ConvertToString(null, culture);
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
