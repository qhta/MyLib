namespace Qhta.Conversion;

/// <summary>
/// Converter for System.DBNull type to string and backward.
/// On ConvertTo, it converts null and DBNull values to null.
/// On ConvertFrom, it gives DBNull value.
/// This converter also implements Qhta.Xml.IXmlConverter interface.
/// </summary>
public class DbNullTypeXmlConverter : BaseTypeConverter, IXmlConverter
{
  /// <summary>
  /// Sets ExpectedType to DBNull and XsdType to XsdSimpleType.String.
  /// </summary>
  public DbNullTypeXmlConverter()
  {
    ExpectedType = typeof(DBNull);
    XsdType = XsdSimpleType.String;
  }

  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <inheritdoc/>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    if (value is DBNull)
      return null!;
    return base.ConvertTo(context, culture, value, destinationType);
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    return DBNull.Value;
  }

  /// <inheritdoc/>
  public bool CanRead => true;

  /// <inheritdoc/>
  public bool CanWrite => false;

  /// <inheritdoc/>
  public void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public object? ReadXml(object? context, IXmlReader reader, Type objectType, object? existingValue, IXmlSerializer? serializer)
  {
    reader.Read();
    return DBNull.Value;
  }

  /// <inheritdoc/>
  public bool CanConvert(Type objectType)
  {
    return objectType == typeof(DBNull);
  }
}