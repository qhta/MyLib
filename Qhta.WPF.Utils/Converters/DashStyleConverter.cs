using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public sealed class DashStyleConverter : TypeConverter
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
      if (context.Instance is DashStyle)
      {
        return ((DashStyle)context.Instance).CanSerializeToString();
      }
      object[] args = new object[] { "DashStyle" };
      throw new ArgumentException("Expected general type", "context");
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null)
      {
        throw base.GetConvertFromException(value);
      }
      string str = value as string;
      return ((str == null) ? base.ConvertFrom(context, culture, value) : DashStyleUtils.Parse(str, context));

    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if ((destinationType != null) && (value is DashStyle))
      {
        DashStyle DashStyle = (DashStyle)value;
        if (destinationType == typeof(string))
        {
          if (((context != null) && (context.Instance != null)) && !DashStyle.CanSerializeToString())
          {
            throw new NotSupportedException("Convert to not supported");
          }
          return DashStyle.ConvertToString(null, culture);
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
