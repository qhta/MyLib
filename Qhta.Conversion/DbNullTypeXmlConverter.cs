using System.ComponentModel;
using System.Globalization;

using Qhta.Xml.Serialization;

namespace Qhta.Conversion;

public class DbNullTypeXmlConverter : BaseTypeConverter, IXmlConverter
{
  public DbNullTypeXmlConverter()
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

  public bool CanRead => true;
  public bool CanWrite => false;

  public void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }

  public object? ReadXml(object? context, IXmlReader reader, Type objectType, object? existingValue, IXmlSerializer? serializer)
  {
    reader.Read();
    return DBNull.Value;
  }

  public bool CanConvert(Type objectType)
  {
    return objectType == typeof(DBNull);
  }
}