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

  public override bool CanWrite => false;

  public override bool CanConvert(Type objectType)
  {
    return objectType.GetInterface("IDictionary") != null && objectType.GetConstructor(new Type[0]) != null;
  }

  public override object? ReadXml(object context, IXmlConverterReader? iReader, SerializationTypeInfo objectTypeInfo,
    SerializationMemberInfo? propertyInfo, SerializationItemInfo? itemInfo)
  {
    if (iReader == null)
      throw new IOException($"Unknown serializer in {GetType()}.{nameof(ReadXml)}");

    var reader = iReader.Reader as XmlTextReader;
    if (reader == null)
      throw new IOException($"Reader type in {GetType()}.{nameof(ReadXml)} must be of {typeof(IXmlTextReaderInitializer).Name} type.");

    var rootElementName = reader.Name;

    var serializationTypeInfo = propertyInfo?.ValueType;
    if (serializationTypeInfo == null)
      serializationTypeInfo = objectTypeInfo;
    if (serializationTypeInfo == null)
      throw new IOException($"Unknown type info for property {GetType()}");

    if (reader.EOF)
      return null;
    var constructor = serializationTypeInfo.KnownConstructor;

    if (constructor == null)
      throw new IOException($"Type {serializationTypeInfo.Type} has no parameterless public constructor");
    var dict = constructor.Invoke(new object[0]) as IDictionary;
    if (dict == null)
      throw new IOException($"Type {serializationTypeInfo.Type} must implement IDictionary interface");

    var valueTypeInfo = (propertyInfo?.ContentInfo as DictionaryInfo)?.ValueTypeInfo;
    if (valueTypeInfo == null && ItemType != null)
      iReader.TryGetTypeInfo(ItemType, out valueTypeInfo);
    if (valueTypeInfo == null)
      throw new IOException($"Unknown value type info for property {GetType()}");

    var valueConstructor = valueTypeInfo.KnownConstructor;
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
        iReader.ReadObject(value, valueTypeInfo);
      }
      else
      {
        reader.Read();
        value = iReader.ReadValue(context, valueTypeInfo.Type, null, propertyInfo);
      }
      if (value != null)
      {
        if (itemInfo?.AddMethod != null)
        {
          itemInfo.AddMethod.Invoke(dict, new[] { key, value });
        }
        else
        {
          var itemTypeInfo = objectTypeInfo?.ContentInfo?.KnownItemTypes.FindTypeInfo(value.GetType());
          if (itemTypeInfo != null && itemTypeInfo.AddMethod != null)
            itemTypeInfo.AddMethod.Invoke(dict, new[] { key, value });
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
    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == rootElementName) reader.Read();
    return dict;
  }

  public override void WriteXml(object? context, IXmlWriter writer, object? value, IXmlSerializer? serializer)
  {
    throw new NotImplementedException();
  }
}