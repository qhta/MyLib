using System.ComponentModel;

using Qhta.Conversion;

namespace Qhta.Xml.Serialization;

public class QXmlSerializationWriter
{
  public QXmlSerializationWriter(XmlWriter xmlWriter, QXmlSerializerSettings settings)
  {
    Writer = xmlWriter;
    Mapper = settings.Mapper;
    Options = settings.Options;
    XmlWriterSettings = settings.XmlWriterSettings;
  }

  public XmlWriter Writer { get; }

  public XmlSerializationInfoMapper Mapper { get; }

  public KnownTypesCollection KnownTypes => Mapper.KnownTypes;

  public string? DefaultNamespace => Mapper.DefaultNamespace;

  public SerializationOptions Options { get; }

  public XmlWriterSettings XmlWriterSettings { get; }

  #region Write methods

  public void WriteObject(object obj, bool emitNamespaces = false)
  {
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.WriteXml(Writer);
    }
    else
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
        throw new InternalException($"Type \"{aType}\" not registered");
      var tag = Mapper.GetXmlTag(serializedTypeInfo);
      //if (emitNamespaces && writer is XmlTextWriter xmlTextWriter)
      //{
      //  xmlTextWriter.Namespaces = true;
      //}
      if (DefaultNamespace != null)
        Writer.WriteStartElement(tag.Name, DefaultNamespace);
      else
        Writer.WriteStartElement(tag.Name, tag.XmlNamespace);
      if (emitNamespaces)
      {
        if (Options.UseNilValue)
          //Namespaces.Add("xsi", xsiNamespace);
          Writer.WriteAttributeString("xmlns", "xsi", null, QXmlSerializationHelper.xsiNamespace);

        if (Options.UseXsdScheme)
          //Namespaces.Add("xsd", xsdNamespace);
          Writer.WriteAttributeString("xmlns", "xsd", null, QXmlSerializationHelper.xsdNamespace);
        emitNamespaces = false;
      }

      WriteObjectInterior(obj, null, serializedTypeInfo);
      Writer.WriteEndElement();
    }
  }

  public void WriteObjectInterior(object obj, string? tag = null, SerializationTypeInfo? typeInfo = null)
  {
    if (typeInfo == null)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out typeInfo))
        throw new InternalException($"Type \"{aType}\" not registered");
    }

    WriteAttributesBase(obj, typeInfo);
    WritePropertiesBase(tag, obj, typeInfo);
    if (typeInfo.ContentProperty != null)
      WriteContentProperty(obj, tag, null, typeInfo.ContentProperty, typeInfo);
    WriteCollectionBase(tag, null, obj as IEnumerable, typeInfo);
  }

  public int WriteAttributesBase(object obj, SerializationTypeInfo typeInfo)
  {
    var aType = obj.GetType();
    var propList = typeInfo.MembersAsAttributes;
    var attrsWritten = 0;
    foreach (var memberInfo in propList)
    {
      if (memberInfo.CheckMethod != null)
      {
        var shouldSerializeProperty = memberInfo.CheckMethod.Invoke(new[] { obj }, new object[0]);
        if (shouldSerializeProperty is bool shouldSerialize)
          if (!shouldSerialize)
            continue;
      }

      var attrName = memberInfo.XmlName;
      var attrTag = Mapper.GetXmlTag(memberInfo);
      var propValue = memberInfo.GetValue(obj);
      if (propValue != null)
      {
        var defaultValue = memberInfo.DefaultValue;
        if (defaultValue != null && propValue.Equals(defaultValue))
          continue;
        var str = ConvertMemberValueToString(memberInfo, propValue);
        if (str != null)
        {
          Writer.WriteAttributeString(attrTag.Prefix, attrTag.Name, attrTag.XmlNamespace, str);
          attrsWritten++;
        }
      }
    }
    return attrsWritten;
  }

  public int WritePropertiesBase(string? elementTag, object obj, SerializationTypeInfo typeInfo)
  {
    var props = typeInfo.MembersAsElements;

    var propsWritten = 0;

    foreach (var memberInfo in props)
    {
      if (memberInfo.CheckMethod != null)
      {
        var shouldSerializeProperty = memberInfo.CheckMethod.Invoke(obj, new object[0]);
        if (shouldSerializeProperty is bool shouldSerialize)
          if (!shouldSerialize)
            continue;
      }

      var propTag = memberInfo.XmlName;

      if (Options?.PrecedePropertyNameWithClassName == true)
        propTag = elementTag + "." + propTag;
      var propValue = memberInfo.GetValue(obj);
      //if (propValue?.GetType().Name == "VectorVariant")
      //  TestTools.Stop();
      propValue = memberInfo.GetTypeConverter()?.ConvertToInvariantString(propValue) ?? propValue;
      if (propValue == null)
      {
        if (Options?.UseNilValue == true && memberInfo.IsNullable)
        {
          if (!String.IsNullOrEmpty(propTag))
            WriteStartElement(propTag);
          Writer.WriteAttributeString(null, "nil", QXmlSerializationHelper.xsiNamespace, "true");
          if (!String.IsNullOrEmpty(propTag))
            WriteEndElement(propTag);
        }
      }
      else
      {
        var defaultValue = memberInfo.DefaultValue;
        if (defaultValue != null)
        {
          if (propValue.Equals(defaultValue))
            continue;
          if (defaultValue is int iv && iv == 0 && (int)propValue == 0)
            continue;
        }
        else
        {
          if (!String.IsNullOrEmpty(propTag))
            WriteStartElement(propTag);
            var pType = propValue.GetType();
            KnownTypes.TryGetValue(pType, out var serializedTypeInfo);
            if (pType.IsSimple())
            {
              if (memberInfo.TypeConverter != null)
                WriteValue(ConvertMemberValueToString(memberInfo, propValue));
              else
                WriteValue(propValue);
            }
            else if (pType.IsArray(out var itemType) && itemType == typeof(byte))
            {
              WriteValue(ConvertMemberValueToString(memberInfo, propValue));
            }
            else if (memberInfo.IsReference)
            {
              WriteValue(ConvertMemberValueToString(memberInfo, propValue));
            }
            else
            {
              WriteObject(propValue);
            }
            //else if (propValue is ICollection collection)
            //{
            //  var arrayInfo = prop.CollectionInfo;
            //  foreach (var arrayItem in collection)
            //  {
            //    if (arrayItem != null)
            //    {
            //      var itemType = arrayItem.GetType();
            //      //SerializationTypeInfo? itemTypeInfo;
            //      string? itemName = null;
            //      if (arrayInfo != null)
            //      {

            //        var itemTypeInfoPair = arrayInfo.KnownItemTypes.FindTypeInfo(itemType);
            //        if (itemTypeInfoPair != null)
            //        {
            //          //itemTypeInfo = itemTypeInfoPair.TypeInfo;
            //          itemName = itemTypeInfoPair.ElementName;
            //        }
            //      }

            //      if (itemName == null)
            //        itemName = arrayItem.GetType().Name;
            //      WriteStartElement(itemName);
            //      if (arrayItem.GetType().Name == "SwitchCase")
            //        TestUtils.Stop();
            //      WriteObjectInterior(itemName, arrayItem);
            //      WriteEndElement(itemName);
            //    }
            //  }
            //}
            //else
            //{
            //  WriteValue(ConvertMemberValueToString(memberInfo, propValue));
            //}         
          if (!String.IsNullOrEmpty(propTag))
            WriteEndElement(propTag);
        }
        propsWritten++;
      }
    }
    return propsWritten;
  }

  public int WriteContentProperty(object? Context, string? elementTag, string? propTag, SerializationMemberInfo contentMemberInfo,
    SerializationTypeInfo typeInfo)
  {
    if (contentMemberInfo.CanWrite && typeInfo.ContentInfo != null)
    {
      var itemTypes = typeInfo.ContentInfo.KnownItemTypes;
      var propInfo = contentMemberInfo.Property;
      if (propInfo != null)
      {
        if (propTag != null && elementTag != null)
          if (Options?.PrecedePropertyNameWithClassName == true)
            propTag = elementTag + "." + propTag;
        var item = propInfo.GetValue(Context, null);
        if (item != null)
        {
          if (item == null)
          {
            WriteStartElement("null");
            WriteEndElement("null");
          }
          else
          {
            var itemTag = item.GetType().GetTypeTag();
            TypeConverter? typeConverter = null;
            if (itemTypes != null)
            {
              var itemType = item.GetType();
              if (!itemTypes.TryGetValue(itemType, out var itemTypeInfo))
                itemTypeInfo = itemTypes.FindTypeInfo(itemType);

              if (itemTypeInfo != null)
              {
                itemTag = itemTypeInfo.XmlName;
                typeConverter = itemTypeInfo.TypeInfo.TypeConverter;
              }
            }

            if (item.GetType().IsSimple())
            {
              //itemTag = GetItemTag(item.GetType());
              WriteStartElement(itemTag);
              WriteValue(item);
              WriteEndElement(itemTag);
            }
            else
            {
              if (typeConverter != null)
              {
                if (!string.IsNullOrEmpty(itemTag))
                  WriteStartElement(itemTag);
                WriteValue(typeConverter.ConvertToString(item) ?? "");
                if (!string.IsNullOrEmpty(itemTag))
                  WriteEndElement(itemTag);
              }
              else
              {
                //if (contentMemberInfo.StoresReference)
                //{
                //  if (string.IsNullOrEmpty(itemTag))
                //    WriteValue(item.ToString());
                //  else
                //  {
                //    WriteStartElement(itemTag);
                //    WriteValue(item.ToString());
                //    WriteEndElement(itemTag);
                //  }
                //}
                //if (itemTag != null
                //if (KnownTypes.TryGetValue(item.GetType(), out var serializationTypeInfo))
                //  WriteObject(item);
                //else
                {
                  WriteStartElement(itemTag);
                  WriteObjectInterior(item, itemTag);
                  WriteEndElement(itemTag);
                }
              }
            }
          }
        }
        if (!String.IsNullOrEmpty(propTag))
          Writer.WriteEndElement();
      }
      return 1;
    }
    return 0;
  }

  public int WriteCollectionBase(string? elementTag, string? propTag, IEnumerable? collection, SerializationTypeInfo? typeInfo)
  {
    if (typeInfo?.ContentInfo != null)
      return WriteCollectionBase(elementTag, propTag, collection, typeInfo.ContentInfo);
    return 0;
  }

  public int WriteCollectionBase(string? elementTag, string? propTag, IEnumerable? collection, ContentItemInfo collectionInfo)
  {
    var itemsWritten = 0;
    if (collection == null)
      return 0;
    var itemTypes = collectionInfo.KnownItemTypes;
    if (propTag != null && elementTag != null)
      if (Options?.PrecedePropertyNameWithClassName == true)
        propTag = elementTag + "." + propTag;

    if (!String.IsNullOrEmpty(propTag))
      Writer.WriteStartElement(propTag);
    foreach (var item in collection)
    {
      if (item == null)
      {
        WriteStartElement("null");
        WriteEndElement("null");
      }
      else
      {
        var itemTag = item.GetType().GetTypeTag();
        TypeConverter? typeConverter = null;
        if (itemTypes != null)
        {
          var itemType = item.GetType();
          if (!itemTypes.TryGetValue(itemType, out var itemTypeInfo))
            itemTypeInfo = itemTypes.FindTypeInfo(itemType);

          if (itemTypeInfo != null)
          {
            itemTag = itemTypeInfo.XmlName;
            typeConverter = itemTypeInfo.TypeInfo.TypeConverter;
          }
        }

        if (item.GetType().IsSimple())
        {
          //itemTag = GetItemTag(item.GetType());
          WriteStartElement(itemTag);
          WriteValue(item);
          WriteEndElement(itemTag);
        }
        //else if (item is IXSerializable serializableItem)
        //  serializableItem.Serialize(this);
        else if (item.GetType().Name.StartsWith("KeyValuePair`"))
        {
          var keyProp = item.GetType().GetProperty("Key");
          var valProp = item.GetType().GetProperty("Value");
          if (keyProp != null && valProp != null)
          {
            var key = keyProp.GetValue(item);
            var val = valProp.GetValue(item);
            if (key != null)
            {
              if (collectionInfo is DictionaryInfo dictionaryInfo && dictionaryInfo.KeyProperty != null && val != null
                  && KnownTypes.TryGetValue(val.GetType(), out var serializationTypeInfo))
              {
                WriteObject(val);
              }
              else
              {
                WriteStartElement(itemTag);
                WriteAttributeString("key", key.ToString() ?? "");
                if (val != null)
                  WriteObjectInterior(val);
                WriteEndElement(itemTag);
              }
            }
          }
        }
        else
        {
          if (typeConverter != null)
          {
            if (!string.IsNullOrEmpty(itemTag))
              WriteStartElement(itemTag);
            WriteValue(typeConverter.ConvertToString(item) ?? "");
            if (!string.IsNullOrEmpty(itemTag))
              WriteEndElement(itemTag);
          }
          else
          {
            if (collectionInfo.StoresReferences)
            {
              if (string.IsNullOrEmpty(itemTag))
              {
                WriteValue(item.ToString());
              }
              else
              {
                WriteStartElement(itemTag);
                WriteValue(item.ToString());
                WriteEndElement(itemTag);
              }
            }
            //if (KnownTypes.TryGetValue(item.GetType(), out var serializationTypeInfo))
            //  WriteObject(item);
            //else
            {
              WriteStartElement(itemTag);
              WriteObjectInterior(item, itemTag);
              WriteEndElement(itemTag);
            }
          }
        }
      }
      itemsWritten++;
    }
    if (!String.IsNullOrEmpty(propTag))
      Writer.WriteEndElement();

    Writer.Flush();
    return itemsWritten;
  }

  public void WriteStartElement(string propTag)
  {
    Writer.WriteStartElement(propTag);
  }

  public void WriteEndElement(string propTag)
  {
    Writer.WriteEndElement();
  }

  public void WriteAttributeString(string attrName, string valStr)
  {
    Writer.WriteAttributeString(attrName, valStr);
  }

  public void WriteValue(object? value)
  {
    if (value is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.WriteXml(Writer);
    }
    else
    {
      if (value != null)
      {
        var typeConverter = new ValueTypeConverter(value.GetType(), null, null, null, Options.ConversionOptions);
        var valStr = typeConverter.ConvertToInvariantString(value);
        if (valStr != null)
        {
          //valStr = valStr.EncodeStringValue();
          Writer.WriteValue(valStr);
        }
      }
    }
  }

  public void WriteValue(string propTag, object value)
  {
    WriteStartElement(propTag);
    WriteValue(value);
    WriteEndElement(propTag);
  }

  protected string? ConvertMemberValueToString(SerializationMemberInfo memberInfo, object? propValue)
  {
    if (propValue == null)
      return null;
    if (memberInfo.Property == null)
      return null;
    var typeConverter = memberInfo.GetTypeConverter();
    if (typeConverter != null)
    {
      var str = typeConverter.ConvertToInvariantString(propValue);
      return str;
    }
    else
    {
      typeConverter = new ValueTypeConverter(memberInfo.Property.PropertyType, memberInfo.DataType,
        memberInfo.Format, memberInfo.Culture, memberInfo.ConversionOptions ?? Options.ConversionOptions);
      var str = typeConverter.ConvertToInvariantString(propValue);
      return str;
    }
  }

  #endregion
}