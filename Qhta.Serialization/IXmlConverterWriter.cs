namespace Qhta.Xml.Serialization;

public interface IXmlConverterWriter
{
  public XmlWriter Writer { get; }
  //public KnownTypesCollection KnownTypes { get; }
  //public object? ReadObject(object context, SerializationTypeInfo typeInfo);
  //public object? ReadValue(object context, Type expectedType, TypeConverter? typeConverter, SerializationMemberInfo? memberInfo);
}