using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class Base64TypeConverter : TypeConverter, ITypeConverter
{
  public Type? ExpectedType { get; set; }
  public XsdSimpleType? XsdType { get; set; }
  public string? Format { get; set; }
  public CultureInfo? Culture { get; set; }

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
      return Convert.FromBase64String(str);
    return null;
  }
}