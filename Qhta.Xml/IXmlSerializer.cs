using System.Xml;

namespace Qhta.Xml.Serialization;

/// <summary>
///   Xml equivalent of JsonConverter.
///   Reads and writes object from/to XML.
/// </summary>
public interface IXmlSerializer
{
  public void Serialize(XmlWriter xmlWriter, object? obj);
  public object? Deserialize(XmlReader xmlReader);

  public void WriteObject(object obj, bool emitNamespaces = false);
  public object? ReadObject(object? context = null);
}