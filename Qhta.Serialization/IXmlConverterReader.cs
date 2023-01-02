namespace Qhta.Xml.Serialization;

public interface IXmlConverterReader
{
  public XmlReader Reader { get; }
  public KnownTypesCollection KnownTypes { get; }
  public object? ReadObject(object context, SerializationTypeInfo typeInfo);
  public object? ReadValue(object context, Type expectedType, TypeConverter? typeConverter, SerializationMemberInfo? memberInfo);
}