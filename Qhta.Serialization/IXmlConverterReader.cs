namespace Qhta.Xml.Serialization;

public interface IXmlConverterReader
{
  public XmlReader Reader { get; }
  public bool TryGetTypeInfo(Type type, [NotNullWhen(true)] out SerializationTypeInfo? typeInfo);
  public bool TryGetTypeInfo(XmlQualifiedTagName name, [NotNullWhen(true)] out SerializationTypeInfo? typeInfo);
  public object? ReadObject(object context, SerializationTypeInfo typeInfo);
  public object? ReadValue(object context, Type expectedType, TypeConverter? typeConverter, SerializationMemberInfo? memberInfo);
}