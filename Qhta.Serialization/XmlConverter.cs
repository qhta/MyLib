namespace Qhta.Xml.Serialization;

/// <summary>
///   Xml equivalent of JsonConverter.
///   Reads and writes object from/to XML.
/// </summary>
public abstract class XmlConverter: IXmlConverter
{
  public virtual bool CanRead => true;

  public virtual bool CanWrite => true;

  public abstract void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer);

  public abstract bool CanConvert(Type objectType);

  public object? ReadXml(object? context, IXmlReader reader, Type objectType, object? existingValue, IXmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }
}