using System.Buffers.Text;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Qhta.Conversion;

public class HexBinaryTypeConverter : TypeConverter
{
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is byte[] bytes)
    {
      return Convert.ToHexString(bytes);
    }
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
      return Convert.FromHexString(str);
    }
    return null;
  }
}