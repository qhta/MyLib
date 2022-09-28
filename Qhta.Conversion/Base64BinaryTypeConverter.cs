using System.Buffers.Text;
using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class Base64BinaryTypeConverter : TypeConverter
{
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is byte[] bytes)
      return Convert.ToBase64String(bytes);
    return null;
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      return Convert.FromBase64String(str);
    }
    return null;
  }
}