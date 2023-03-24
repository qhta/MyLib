namespace Qhta.Xml.Serialization;

/// <summary>
///   Xml equivalent of JsonConverter.
///   Reads and writes object from/to XML.
/// </summary>
public abstract class XmlConverter: IXmlConverter
{
  /// <summary>
  /// Specifies whether the converter can read values.
  /// </summary>
  public virtual bool CanRead => true;

  /// <summary>
  /// Specifies whether the converter can write values.
  /// </summary>
  public virtual bool CanWrite => true;

  /// <summary>
  /// Abstract function checking if the converter can be applied to the specified type
  /// </summary>
  /// <param name="objectType"></param>
  /// <returns></returns>
  public abstract bool CanConvert(Type objectType);

  /// <summary>
  /// Abstract method to write to XML writer
  /// </summary>
  public abstract void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer);

  /// <summary>
  /// Unimplemented virtual method to read from XML reader
  /// </summary>
  public virtual object? ReadXml(object? context, IXmlReader reader, Type objectType, object? existingValue, IXmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }
}