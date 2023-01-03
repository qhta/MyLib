namespace Qhta.Xml.Serialization;

/// <summary>
///   Xml equivalent of JsonConverter.
///   Reads and writes object from/to XML.
/// </summary>
public abstract class XmlConverter
{
  public virtual bool CanRead => true;

  public virtual bool CanWrite => true;

  public abstract void WriteXml(IXmlConverterWriter writer, object? value);

  public abstract object? ReadXml(object context, IXmlConverterReader reader, SerializationTypeInfo objectTypeInfo,
    SerializationMemberInfo? propertyInfo, SerializationItemInfo? itemInfo);


  public abstract bool CanConvert(Type objectType);
}