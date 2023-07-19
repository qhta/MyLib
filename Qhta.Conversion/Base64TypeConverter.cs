using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class Base64TypeConverter : BaseTypeConverter
{
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is byte[] bytes) return Convert.ToBase64String(bytes);
    return null;
  }


  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      try
      {
        return Convert.FromBase64String(str);
      }
      // Base64String is default encoder for bytes[] type, however sometimes it can be encoded with HexString.
      catch 
      {
        return ArrayTypeConverter.FromHexString(str);
      }
    }
    return null;
  }
}