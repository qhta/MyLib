using System.ComponentModel;
using System.Xml;

namespace Qhta.Xml;

/// <summary>
///   Definition of methods that must be implemented in a Xml serializer.
/// </summary>
public interface IXmlSerializer
{
  #region required Serialize/Deserialize methods.
  /// <summary>
  /// Serializes an object to the Stream.
  /// </summary>
  /// <param name="stream">The target of serialization.</param>
  /// <param name="obj">Serialized object.</param>
  public void Serialize(Stream stream, object? obj);

  /// <summary>
  /// Serializes an object to the System.XmlWriter.
  /// </summary>
  /// <param name="xmlWriter">The target of serialization.</param>
  /// <param name="obj">Serialized object.</param>
  public void Serialize(XmlWriter xmlWriter, object? obj);

  /// <summary>
  /// Serializes an object to the System.XmlWriter.
  /// </summary>
  /// <param name="xmlWriter">The target of serialization.</param>
  /// <param name="obj">Serialized object.</param>
  public void Serialize(IXmlWriter xmlWriter, object? obj);

  /// <summary>
  /// Deserialized and object from the stream.
  /// </summary>
  /// <param name="stream">Source of serialized data.</param>
  /// <returns>Deserialized object.</returns>
  public object? Deserialize(Stream stream);

  /// <summary>
  /// Deserialized and object from the TextReader.
  /// </summary>
  /// <param name="textReader">Source of serialized data.</param>
  /// <returns>Deserialized object.</returns>
  public object? Deserialize(TextReader textReader);

  /// <summary>
  /// Deserialized and object from the System.XmlReader.
  /// </summary>
  /// <param name="xmlReader">Source of serialized data.</param>
  /// <returns>Deserialized object.</returns>
  public object? Deserialize(XmlReader xmlReader);

  /// <summary>
  /// Deserialized and object from the IXmlReader.
  /// </summary>
  /// <param name="xmlReader">Source of serialized data.</param>
  /// <returns>Deserialized object.</returns>
  public object? Deserialize(IXmlReader xmlReader);
  #endregion

  #region required Read/Write methods
  /// <summary>
  /// Writes an object to XML.
  /// </summary>
  public void WriteObject(object obj);

  /// <summary>
  /// Writes an object in a specified context to XML.
  /// </summary>
  public void WriteObject(object? context, object obj);

  /// <summary>
  /// Reads an object in a specified context from XML.
  /// Returns a newly created instance.
  /// </summary>
  public object? ReadObject(object? context = null);

  /// <summary>
  /// Reads an existing object in a specified context from XML.
  /// </summary>
  public void ReadObject(object? context, object obj);
  #endregion

  #region helper methods
  /// <summary>
  /// Tries to get a type for a specified type name.
  /// </summary>
  /// <param name="typeName"></param>
  /// <param name="type"></param>
  /// <returns></returns>
  public bool TryGetKnownType(string typeName, out Type type);

  /// <summary>
  /// Tries to get a type converter for a specified type.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="converter"></param>
  /// <returns></returns>
  public bool TryGetTypeConverter(Type type, out TypeConverter converter);
  #endregion


}