using System.IO;
using System.Xml;

namespace Qhta.Xml.Serialization;

public class XmlAnyAttribute2Dictionary : XmlConverter
{
  public override bool CanConvert(Type objectType)
  {
    return objectType.GetInterface("IDictionary") != null && objectType.GetConstructor(new Type[0]) != null;
  }

  public override object? ReadXml(XmlReader reader, SerializationTypeInfo objectTypeInfo,
    SerializationPropertyInfo? propertyInfo, SerializationItemTypeInfo? itemInfo, XmlSerializer? serializer)
  {
    var serializationTypeInfo = propertyInfo?.TypeInfo;
    if (serializationTypeInfo == null)
      serializationTypeInfo = objectTypeInfo;
    if (serializationTypeInfo == null)
      throw new IOException($"Unknown type info for property XmlConverter");

    if (reader.EOF)
      return null;
    var constructor = serializationTypeInfo.KnownConstructor;

    if (constructor == null)
      throw new IOException($"Type {serializationTypeInfo.Type} has no parameterless public constructor");
    var dict = constructor.Invoke(new object[0]) as Dictionary<string, string>;
    if (dict == null)
      throw new IOException($"Type {serializationTypeInfo.Type} must be a Dictionary<string, string>");
    reader.MoveToFirstAttribute();
    for (int i = 0; i < reader.AttributeCount; i++)
    {
      dict.Add(reader.LocalName, reader.Value);
      reader.MoveToNextAttribute();
    }
    reader.Read();
    return dict;
  }

  public override bool CanWrite => false;

  public override void WriteXml(XmlWriter writer, object? value, XmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }
}