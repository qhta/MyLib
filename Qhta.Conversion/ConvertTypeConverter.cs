using System.Buffers.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Qhta.Conversion;

public class ConvertTypeConverter : TypeConverter
{
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string)
           || destinationType == typeof(int)
           || destinationType == typeof(byte)
           || destinationType == typeof(uint)
           || destinationType == typeof(sbyte)
           || destinationType == typeof(short)
           || destinationType == typeof(ushort)
           || destinationType == typeof(long)
           || destinationType == typeof(ulong)
           || destinationType == typeof(float)
           || destinationType == typeof(double)
           || destinationType == typeof(decimal)
           || destinationType == typeof(bool)
           || destinationType == typeof(char)
           || destinationType == typeof(DateTime)
           || destinationType == typeof(object);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    return Convert.ChangeType(value, destinationType, culture);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return false;
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    throw new NotImplementedException("ConvertTypeConverter does not implement ConvertFrom method");
  }
}