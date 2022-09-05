using System.IO;
using System.Xml;

namespace Qhta.Xml.Serialization;

public class XmlAnyElement2Dictionary : XmlConverter
{
  public XmlAnyElement2Dictionary()
  {
  }

  public XmlAnyElement2Dictionary(Type itemType)
  {
    ItemType = itemType;
  }

  public Type? ItemType { get; }

  public override bool CanConvert(Type objectType)
  {
    return objectType.GetInterface("IDictionary") != null && objectType.GetConstructor(new Type[0]) != null;
  }

  public override object? ReadXml(XmlReader reader, SerializationTypeInfo objectTypeInfo,
    SerializationPropertyInfo? propertyInfo, SerializationItemInfo? itemInfo, QXmlSerializer? serializer)
  {
    if (serializer == null)
      throw new IOException($"Unknown serializer in {this.GetType()}.{nameof(ReadXml)}");

    var aReader = reader as XmlTextReader;
    if (aReader == null)
      throw new IOException($"Reader type in {this.GetType()}.{nameof(ReadXml)} must be of {typeof(IXmlTextReaderInitializer).Name} type.");

    var rootElementName = reader.Name;

    var serializationTypeInfo = propertyInfo?.TypeInfo;
    if (serializationTypeInfo == null)
      serializationTypeInfo = objectTypeInfo;
    if (serializationTypeInfo == null)
      throw new IOException($"Unknown type info for property {this.GetType()}");

    if (reader.EOF)
      return null;
    var constructor = serializationTypeInfo.KnownConstructor;

    if (constructor == null)
      throw new IOException($"Type {serializationTypeInfo.Type} has no parameterless public constructor");
    var dict = constructor.Invoke(new object[0]) as IDictionary;
    if (dict == null)
      throw new IOException($"Type {serializationTypeInfo.Type} must implement IDictionary interface");

    var valueTypeInfo = (propertyInfo?.CollectionInfo as DictionaryInfo)?.ValueTypeInfo;
    if (valueTypeInfo == null && ItemType != null)
      serializer.KnownTypes.TryGetValue(ItemType, out valueTypeInfo);
    if (valueTypeInfo == null)
      throw new IOException($"Unknown value type info for property {this.GetType()}");

    var valueConstructor = (valueTypeInfo.KnownConstructor);
    if (valueConstructor == null && valueTypeInfo.Type != typeof(string))
      throw new IOException($"Type {valueTypeInfo.Type} has no parameterless public constructor");
    reader.Read();
    while (reader.NodeType != XmlNodeType.EndElement)
    {
      var key = reader.LocalName;
      var itemName = reader.Name;
      object? value;
      if (valueConstructor != null)
      {
        value = valueConstructor.Invoke(new object[0]);
        serializer.ReadObject(value, aReader, valueTypeInfo);
      }
      else
      {
        aReader.Read();
        value = serializer.ReadValue(valueTypeInfo.Type, null, aReader);
      }
      if (value != null)
      {
        if (itemInfo?.AddMethod != null)
          itemInfo.AddMethod.Invoke(dict, new object[] { key, value });
        else
        {
          var itemTypeInfo = (objectTypeInfo)?.CollectionInfo?.KnownItemTypes.FindTypeInfo(value.GetType());
          if (itemTypeInfo != null && itemTypeInfo.AddMethod != null)
            itemTypeInfo.AddMethod.Invoke(dict, new object[] { key, value });
          else
            dict.Add(key, value);
        }
      }
      if (reader.NodeType == XmlNodeType.EndElement && reader.Name == key)
      {
        reader.Read();
        break;
      }
    }
    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == rootElementName)
    {
      reader.Read();
    }
    return dict;
  }

  public override bool CanWrite => false;

  public override void WriteXml(XmlWriter writer, object? value, QXmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }
}