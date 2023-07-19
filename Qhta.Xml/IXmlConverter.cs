namespace Qhta.Xml;

/// <summary>
///   Reads and writes object from/to XML.
///   Can be defined in a data class.
/// </summary>
public interface IXmlConverter
{
  /// <summary>
  /// Specifies if it can read.
  /// </summary>
  public bool CanRead {get; }
  /// <summary>
  /// Specifies if it can write.
  /// </summary>
  public bool CanWrite { get; }

  /// <summary>
  /// Writers an object to XML using IXmlWriter.
  /// </summary>
  public void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer);

  /// <summary>
  /// Reads an object from XML using IXmlReaded.
  /// </summary>
  public object? ReadXml(object? context, IXmlReader reader, Type objectType, object? existingValue, IXmlSerializer? serializer);

  /// <summary>
  /// Specifies if it can convert an object type.
  /// </summary>
  /// <param name="objectType"></param>
  /// <returns></returns>
  public bool CanConvert(Type objectType);
}