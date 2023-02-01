using System.Xml;

namespace Qhta.Xml.Serialization;

/// <summary>
///   Xml equivalent of JsonConverter.
///   Reads and writes object from/to XML.
/// </summary>
public interface IXmlConverter
{
  public bool CanRead {get; }
  public bool CanWrite { get; }
  public void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer);

  public object? ReadXml(object? context, IXmlReader reader, Type objectType, object? existingValue, IXmlSerializer? serializer);

  public bool CanConvert(Type objectType);
}