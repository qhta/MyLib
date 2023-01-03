namespace Qhta.Xml.Serialization;

public class QualifiedNameTypeConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
      return new XmlQualifiedTagName(str);
    throw new NotSupportedException($"Cannot convert {nameof(XmlQualifiedTagName)} from {value.GetType().Name}");
  }

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is XmlQualifiedTagName aName && destinationType == typeof(string))
      return aName.ToString();
    throw new NotSupportedException($"Cannot convert {nameof(XmlQualifiedTagName)} to {destinationType.Name}");
  }
}