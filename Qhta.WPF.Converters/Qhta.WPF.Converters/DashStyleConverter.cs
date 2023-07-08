using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Type Converter to convert between <see cref="DashStyle"/> and string or other type.
  /// </summary>
  public sealed class DashStyleConverter : TypeConverter
  {
    /// <summary>
    /// Checks if source type is string or context can convert from source type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sourceType"></param>
    /// <returns></returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      return ((sourceType == typeof(string)) ? true : base.CanConvertFrom(context, sourceType));
    }

    /// <summary>
    /// Checks if context can convert to destination .
    /// </summary>
    /// <param name="context"></param>
    /// <param name="destinationType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
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

    /// <summary>
    /// Converts from source type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="culture"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value == null)
      {
        throw base.GetConvertFromException(value);
      }
      string? str = value as string;
      return ((str == null) ? base.ConvertFrom(context, culture, value) 
        : DashStyleUtils.Parse(str, context));

    }

    /// <summary>
    /// Converts to destination type
    /// </summary>
    /// <param name="context"></param>
    /// <param name="culture"></param>
    /// <param name="value"></param>
    /// <param name="destinationType"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (value is DashStyle dashStyle)
      {
        if (destinationType == typeof(string))
        {
          if (((context != null) && (context.Instance != null)) && !dashStyle.CanSerializeToString())
          {
            throw new NotSupportedException("Convert to not supported");
          }
          return dashStyle.ConvertToString(null, culture);
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
