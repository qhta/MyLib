using System.Xml;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Xml equivalent of JsonConverter.
/// Reads and writes object from/to XML.
/// </summary>
public abstract class XmlConverter
{
  public virtual bool CanRead => true;

  public virtual bool CanWrite => true;

  public abstract void WriteXml(XmlWriter writer, object? value, QXmlSerializer? serializer);

  public abstract object? ReadXml(XmlReader reader, SerializationTypeInfo objectTypeInfo, 
    SerializationMemberInfo? propertyInfo, SerializationItemInfo? itemInfo, QXmlSerializer? serializer);


  public abstract bool CanConvert(Type objectType);
}