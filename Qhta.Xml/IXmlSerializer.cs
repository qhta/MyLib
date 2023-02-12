using System.ComponentModel;
using System.Xml;

namespace Qhta.Xml;

/// <summary>
///   Xml equivalent of JsonConverter.
///   Reads and writes object from/to XML.
/// </summary>
public interface IXmlSerializer
{
  public bool TryGetKnownType(string typeName, out Type type);

  public bool TryGetTypeConverter(Type type, out TypeConverter converter);

  public void Serialize(XmlWriter xmlWriter, object? obj);

  public object? Deserialize(XmlReader xmlReader);

  public void WriteObject(object obj);
  public void WriteObject(object? context, object obj);

  public object? ReadObject(object? context = null);
}