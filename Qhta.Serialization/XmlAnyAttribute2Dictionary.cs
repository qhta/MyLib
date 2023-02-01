namespace Qhta.Xml.Serialization;

public class XmlAnyAttribute2Dictionary : XmlConverter
{
  public override bool CanWrite => false;

  public override bool CanConvert(Type objectType)
  {
    return objectType.GetInterface("IDictionary") != null && objectType.GetConstructor(new Type[0]) != null;
  }

  public override object? ReadXml(object context, IXmlConverterReader iReader, SerializationTypeInfo objectTypeInfo,
    SerializationMemberInfo? propertyInfo, SerializationItemInfo? itemInfo)
  {
    var reader = iReader.Reader;
    var serializationTypeInfo = propertyInfo?.ValueType;
    if (serializationTypeInfo == null)
      serializationTypeInfo = objectTypeInfo;
    if (serializationTypeInfo == null)
      throw new IOException("Unknown type info for property XmlConverter");

    if (reader.EOF)
      return null;
    var constructor = serializationTypeInfo.KnownConstructor;

    if (constructor == null)
      throw new IOException($"Type {serializationTypeInfo.Type} has no parameterless public constructor");
    var dict = constructor.Invoke(new object[0]) as Dictionary<string, string>;
    if (dict == null)
      throw new IOException($"Type {serializationTypeInfo.Type} must be a Dictionary<string, string>");
    reader.MoveToFirstAttribute();
    for (var i = 0; i < reader.AttributeCount; i++)
    {
      dict.Add(reader.LocalName, reader.Value);
      reader.MoveToNextAttribute();
    }
    reader.Read();
    return dict;
  }

  public override void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }
}