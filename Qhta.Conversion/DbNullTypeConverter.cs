using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class DbNullTypeConverter : BaseTypeConverter
{
  public DbNullTypeConverter()
  {
    ExpectedType = typeof(DBNull);
    XsdType = XsdSimpleType.String;
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    if (value is DBNull)
      return null!;
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    return DBNull.Value;
  }
}