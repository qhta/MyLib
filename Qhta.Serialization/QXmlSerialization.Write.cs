using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

using Qhta.Conversion;

namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{

  public XmlWriterSettings XmlWriterSettings { get; } = new()
  {
    Indent = true,
    NamespaceHandling = NamespaceHandling.OmitDuplicates,
  };

  public QXmlWriter Writer { get; protected set; } = null!;

  #region Write methods

  public void WriteObject(object obj)
  {
    WriteObject(null, obj);
  }
  public void WriteObject(object? context, object obj)
  {
    var aType = obj.GetType();
    if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
      throw new InternalException($"Type \"{aType}\" not registered");
    var tag = Mapper.GetXmlTag(serializedTypeInfo);
    WriteObject(context, obj, tag, Options.EmitNamespaces);
  }

  public void WriteObject(object? context, object obj, XmlQualifiedTagName? tag, bool emitNamespaces = false)
  {
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.WriteXml(Writer);
    }
    else
    {
      var aType = obj.GetType();
      if (aType.TryGetConverter(out var typeConverter) && typeConverter is IXmlConverter xmlConverter && xmlConverter.CanWrite)
      {
        xmlConverter.WriteXml(null, Writer, obj, this);
      }
      else
      {
        if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
          throw new InternalException($"Type \"{aType}\" not registered");
        Writer.EmitNamespaces = emitNamespaces;
        if (tag != null)
          Writer.WriteStartElement(tag);
        if (emitNamespaces && context == null)
        {
          if (Options.UseNilValue)
            Writer.WriteNamespaceDef("xsi", QXmlSerializationHelper.xsiNamespace);

          if (Options.UseXsdScheme)
            Writer.WriteNamespaceDef("xsd", QXmlSerializationHelper.xsdNamespace);
        }

        WriteObjectInterior(context, obj, null, serializedTypeInfo);
        if (tag != null)
          Writer.WriteEndElement(tag);
      }
    }
  }

  public void WriteObjectInterior(object? context, object obj, XmlQualifiedTagName? tag, SerializationTypeInfo? typeInfo = null)
  {
    if (typeInfo == null)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out typeInfo))
        throw new InternalException($"Type \"{aType}\" not registered");
    }

    WritePropertiesAsAttributes(context, obj, typeInfo);
    if (typeInfo.TypeConverter != null)
    {
      var str = typeInfo.TypeConverter.ConvertToInvariantString(obj);
      Writer.WriteValue(str);
    }
    else
    if (typeInfo.Type.IsSimple())
    {
      var typeConverter = new ValueTypeConverter(typeInfo.Type);
      var str = typeConverter.ConvertToInvariantString(obj);
      Writer.WriteValue(str);
    }
    else
    if (typeInfo.ContentProperty != null)
      WriteContentProperty(obj, tag, null, typeInfo.ContentProperty, typeInfo);
    else
    if (typeInfo.IsCollection && typeInfo.ContentInfo != null && obj is IEnumerable collection)
      WriteCollection(context, collection, tag, null, typeInfo.ContentInfo);
    else
      WritePropertiesAsElements(context, obj, tag, typeInfo);
  }

  public int WritePropertiesAsAttributes(object? context, object obj, SerializationTypeInfo typeInfo)
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
          Writer.WriteAttributeString(attrTag, str);
          attrsWritten++;
        }
      }
    }
    return attrsWritten;
  }

  public int WritePropertiesAsElements(object? context, object obj, XmlQualifiedTagName? elementTag, SerializationTypeInfo typeInfo)
  {
    var props = typeInfo.MembersAsElements;
    var propsWritten = 0;
    ITypeDescriptorContext? typeDescriptorContext = new TypeDescriptorContext(obj);
    foreach (var memberInfo in props)
    {
      if (memberInfo.CheckMethod != null)
      {
        var shouldSerializeProperty = memberInfo.CheckMethod.Invoke(obj, new object[0]);
        if (shouldSerializeProperty is bool shouldSerialize)
          if (!shouldSerialize)
            continue;
      }
      var propTag = new XmlQualifiedTagName(memberInfo.XmlName, memberInfo.XmlNamespace);
      if (propTag.Name == "CompatibilitySettings")
        Debug.Assert(true);
      var propValue = memberInfo.GetValue(obj);
      if (propValue == null)
      {
        if (Options?.UseNilValue == true && memberInfo.IsNullable)
        {
          if (propTag != null)
            Writer.WriteStartElement(propTag);
          Writer.WriteNilAttribute(QXmlSerializationHelper.xsiNamespace);
          if (propTag != null)
            Writer.WriteEndElement(propTag);
        }
      }
      else
      {
        string? str = null;
        var typeConverter = memberInfo.GetTypeConverter();
        if (typeConverter != null && typeConverter.CanConvertTo(typeof(string)))
          str = typeConverter.ConvertToInvariantString(typeDescriptorContext, propValue);
        if (str != null)
        {
          if (propTag != null)
            Writer.WriteStartElement(propTag);
          Writer.WriteString(str);
          if (propTag != null)
            Writer.WriteEndElement(propTag);
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
            if (propTag != null)
              Writer.WriteStartElement(propTag);
            var pType = propValue.GetType();
            KnownTypes.TryGetValue(pType, out var serializedTypeInfo);
            if (pType.IsSimple())
            {
              if (memberInfo.TypeConverter != null)
                WriteValue(context, ConvertMemberValueToString(memberInfo, propValue));
              else
                WriteValue(context, propValue);
            }
            else if (pType.IsArray(out var itemType) && itemType == typeof(byte))
            {
              WriteValue(context, ConvertMemberValueToString(memberInfo, propValue));
            }
            else if (memberInfo.IsReference)
            {
              WriteValue(context, ConvertMemberValueToString(memberInfo, propValue));
            }
            else if (propValue is ICollection collection)
            {
              var arrayInfo = memberInfo.ContentInfo;
              foreach (var collectionItem in collection)
              {
                if (collectionItem != null)
                {
                  var collectionItemType = collectionItem.GetType();
                  //SerializationTypeInfo? itemTypeInfo;
                  XmlQualifiedTagName? collectionItemTag = null;
                  if (arrayInfo != null)
                  {

                    var itemTypeInfoPair = arrayInfo.KnownItemTypes.FindTypeInfo(collectionItemType);
                    if (itemTypeInfoPair != null)
                    {
                      collectionItemTag = new XmlQualifiedTagName(itemTypeInfoPair.XmlName, itemTypeInfoPair.XmlNamespace);
                    }
                  }

                  if (collectionItemTag == null)
                    collectionItemTag = new XmlQualifiedTagName(collectionItem.GetType().Name, collectionItem.GetType().Namespace);
                  Writer.WriteStartElement(collectionItemTag);
                  WriteObjectInterior(collection, collectionItem, collectionItemTag);
                  Writer.WriteEndElement(collectionItemTag);
                }
              }
            }
            //else
            //{
            //  WriteValue(collection, ConvertMemberValueToString(memberInfo, propValue));
            //}
            else
            {
              WriteObject(propValue);
            }
            if (propTag != null)
              Writer.WriteEndElement(propTag);
          }
        }
        propsWritten++;
      }
    }
    return propsWritten;
  }

  public int WriteContentProperty(object? context, XmlQualifiedTagName? elementTag, string? propName, SerializationMemberInfo contentMemberInfo,
    SerializationTypeInfo typeInfo)
  {
    if (typeInfo.ContentInfo != null)
      return WriteCollectionContentProperty(context, elementTag, propName, contentMemberInfo, typeInfo.ContentInfo);
    if (contentMemberInfo.CanWrite)
    {
      var propInfo = contentMemberInfo.Property;
      if (propInfo != null)
      {
        var value = propInfo.GetValue(context, null);
        if (value != null)
        {
          var typeConverter = contentMemberInfo.GetTypeConverter();
          if (typeConverter is IXmlConverter xmlConverter && xmlConverter.CanConvert(value.GetType()))
          {
            xmlConverter.WriteXml(context, Writer, value, this);
          }
          else
          {
            var propTag = CreatePropertyTag(elementTag, propName);
            if (value.GetType().IsSimple())
            {
              if (propTag != null)
                Writer.WriteStartElement(propTag);
              WriteValue(context, value);
              if (propTag != null)
                Writer.WriteEndElement(propTag);
            }
            else
            {
              ITypeDescriptorContext? typeDescriptorContext = (context != null) ? new TypeDescriptorContext(context) : null;
              if (typeConverter != null && typeConverter.CanConvertTo(typeDescriptorContext, typeof(string)))
              {
                if (propTag != null)
                  Writer.WriteStartElement(propTag);
                var str = typeConverter.ConvertToInvariantString(typeDescriptorContext, value);
                WriteValue(context, str);
                if (propTag != null)
                  Writer.WriteEndElement(propTag);
              }
              else
              {
                if (propTag != null)
                  Writer.WriteStartElement(propTag);
                WriteObjectInterior(context, value, propTag);
                if (propTag != null)
                  Writer.WriteEndElement(propTag);
              }
            }
          }
        }
        return 1;
      }
    }
    return 0;
  }

  public int WriteCollectionContentProperty(object? context, XmlQualifiedTagName? elementTag, string? propName, SerializationMemberInfo contentMemberInfo,
    ContentItemInfo contentInfo/*, SerializationTypeInfo typeInfo*/)
  {
    if (contentInfo != null)
    {
      var itemTypes = contentInfo.KnownItemTypes;
      var propInfo = contentMemberInfo.Property;
      if (propInfo != null)
      {
        var propTag = CreatePropertyTag(elementTag, propName);
        var item = propInfo.GetValue(context, null);
        if (item != null)
        {
          if (item == null)
          {
            Writer.WriteStartElement("null");
            Writer.WriteEndElement("null");
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
              Writer.WriteStartElement(itemTag);
              WriteValue(context, item);
              Writer.WriteEndElement(itemTag);
            }
            else
            {
              if (typeConverter != null)
              {
                if (!string.IsNullOrEmpty(itemTag))
                  Writer.WriteStartElement(itemTag);
                WriteValue(context, typeConverter.ConvertToString(item) ?? "");
                if (!string.IsNullOrEmpty(itemTag))
                  Writer.WriteEndElement(itemTag);
              }
              else
              {
                //if (contentMemberInfo.StoresReference)
                //{
                //  if (string.IsNullOrEmpty(itemTag))
                //    WriteValue(item.ToString());
                //  else
                //  {
                //    Writer.WriteStartElement(itemTag);
                //    WriteValue(item.ToString());
                //    Writer.WriteEndElement(itemTag);
                //  }
                //}
                //if (itemTag != null
                //if (KnownTypes.TryGetValue(item.GetType(), out var serializationTypeInfo))
                //  WriteObject(item);
                //else
                {
                  Writer.WriteStartElement(itemTag);
                  WriteObjectInterior(context, item, itemTag);
                  Writer.WriteEndElement(itemTag);
                }
              }
            }
          }
        }
        if (propTag != null)
          Writer.WriteEndElement(propTag);
      }
      return 1;
    }
    return 0;
  }

  public int WriteCollection(object? context, IEnumerable collection, XmlQualifiedTagName? elementTag, string? propName, ContentItemInfo collectionInfo)
  {
    var itemsWritten = 0;
    var itemTypes = collectionInfo.KnownItemTypes;
    var propTag = CreatePropertyTag(elementTag, propName);

    if (propTag != null)
      Writer.WriteStartElement(propTag);
    foreach (var item in collection)
    {
      if (item == null)
      {
        Writer.WriteStartElement("null");
        Writer.WriteEndElement("null");
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
          Writer.WriteStartElement(itemTag);
          WriteValue(context, item);
          Writer.WriteEndElement(itemTag);
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
                Writer.WriteStartElement(itemTag);
                Writer.WriteAttributeString("key", key.ToString() ?? "");
                if (val != null)
                  WriteObjectInterior(context, val, null);
                Writer.WriteEndElement(itemTag);
              }
            }
          }
        }
        else
        {
          if (typeConverter != null)
          {
            if (!string.IsNullOrEmpty(itemTag))
              Writer.WriteStartElement(itemTag);
            WriteValue(context, typeConverter.ConvertToString(item) ?? "");
            if (!string.IsNullOrEmpty(itemTag))
              Writer.WriteEndElement(itemTag);
          }
          else
          {
            if (collectionInfo.StoresReferences)
            {
              if (string.IsNullOrEmpty(itemTag))
              {
                WriteValue(context, item.ToString());
              }
              else
              {
                Writer.WriteStartElement(itemTag);
                WriteValue(context, item.ToString());
                Writer.WriteEndElement(itemTag);
              }
            }
            //if (KnownTypes.TryGetValue(item.GetType(), out var serializationTypeInfo))
            //  WriteObject(item);
            //else
            {
              Writer.WriteStartElement(itemTag);
              WriteObjectInterior(context, item, itemTag);
              Writer.WriteEndElement(itemTag);
            }
          }
        }
      }
      itemsWritten++;
    }
    if (propTag != null)
      Writer.WriteEndElement(propTag);

    Writer.Flush();
    return itemsWritten;
  }

  public void WriteValue(object? context, object? value)
  {
    if (value is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.WriteXml(Writer);
    }
    else
    {
      if (value != null)
      {
        var typeConverter = new ValueTypeConverter(value.GetType(), KnownTypes.Keys, null, null, null, Options.ConversionOptions);
        var valStr = typeConverter.ConvertToInvariantString(value);
        if (valStr != null)
        {
          //valStr = valStr.EncodeStringValue();
          Writer.WriteValue(valStr);
        }
      }
    }
  }

  //public void WriteValue(string propTag, object value)
  //{
  //  Writer.WriteStartElement(propTag);
  //  WriteValue(value);
  //  Writer.WriteEndElement(propTag);
  //}

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
      typeConverter = new ValueTypeConverter(memberInfo.Property.PropertyType, KnownTypes.Keys, memberInfo.DataType,
        memberInfo.Format, memberInfo.Culture, memberInfo.ConversionOptions ?? Options.ConversionOptions);
      var str = typeConverter.ConvertToInvariantString(propValue);
      return str;
    }
  }

  #endregion

  protected XmlQualifiedTagName? CreatePropertyTag(XmlQualifiedTagName? elementTag, string? propName)
  {
    var propTag = elementTag;
    if (propTag == null && propName != null)
      propTag = new XmlQualifiedTagName(propName);
    if (propTag != null && elementTag != null)
      if (Options?.PrecedePropertyNameWithClassName == true)
        propTag = elementTag + ("." + propName);
    return propTag;
  }
}