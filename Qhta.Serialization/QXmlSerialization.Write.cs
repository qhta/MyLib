using System.ComponentModel;

using Qhta.Conversion;

namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{
  //public QXmlSerializationWriter(XmlWriter xmlWriter, QXmlSerializerSettings settings, QXmlSerializer serializer)
  //{
  //  Writer = xmlWriter;
  //  Mapper = settings.Mapper;
  //  Options = settings.Options;
  //  XmlWriterSettings = settings.XmlWriterSettings;
  //  Serializer = serializer;
  //}
  public XmlWriterSettings XmlWriterSettings { get; } = new()
  {
    Indent = true,
    NamespaceHandling = NamespaceHandling.OmitDuplicates,
  };


  public QXmlWriter Writer { get; protected set; } = null!;

  //public XmlSerializationInfoMapper Mapper { get; }

  //public KnownTypesCollection KnownTypes => Mapper.KnownTypes;

  //public string? DefaultNamespace => Mapper.DefaultNamespace;

  //public SerializationOptions Options { get; }

  //public XmlWriterSettings XmlWriterSettings { get; }

  //public QXmlSerializer Serializer { get; }

  #region Write methods

  public void WriteObject(object obj, object? context = null)
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
        var tag = Mapper.GetXmlTag(serializedTypeInfo);
        var emitNamespaces = Options.EmitNamespaces;
        Writer.EmitNamespaces = emitNamespaces;
        //if (DefaultNamespace != null)
        //  Writer.WriteStartElement(tag.Name, DefaultNamespace);
        //else
        Writer.WriteStartElement(tag);
        if (emitNamespaces)
        {
          if (Options.UseNilValue)
            //Namespaces.Add("xsi", xsiNamespace);
            Writer.WriteNamespaceDef("xsi", QXmlSerializationHelper.xsiNamespace);

          if (Options.UseXsdScheme)
            //Namespaces.Add("xsd", xsdNamespace);
            Writer.WriteNamespaceDef("xsd", QXmlSerializationHelper.xsdNamespace);
          emitNamespaces = false;
        }

        WriteObjectInterior(obj, null, serializedTypeInfo);
        //if (DefaultNamespace != null)
        //  Writer.WriteEndElement(tag.Name, DefaultNamespace);
        //else
        Writer.WriteEndElement(tag);
      }
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

  public int WritePropertiesBase(string? elementTag, object obj, SerializationTypeInfo typeInfo)
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

      var propTag = memberInfo.XmlName;
      if (Options?.PrecedePropertyNameWithClassName == true)
        propTag = elementTag + "." + propTag;
      var propValue = memberInfo.GetValue(obj);
      propValue = memberInfo.GetTypeConverter()?.ConvertToInvariantString(typeDescriptorContext, propValue) ?? propValue;
      if (propValue == null)
      {
        if (Options?.UseNilValue == true && memberInfo.IsNullable)
        {
          if (!String.IsNullOrEmpty(propTag))
            Writer.WriteStartElement(propTag);
          Writer.WriteNilAttribute(QXmlSerializationHelper.xsiNamespace);
          if (!String.IsNullOrEmpty(propTag))
            Writer.WriteEndElement(propTag);
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
            Writer.WriteStartElement(propTag);
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
          //      Writer.WriteStartElement(itemName);
          //      if (arrayItem.GetType().Name == "SwitchCase")
          //        TestUtils.Stop();
          //      WriteObjectInterior(itemName, arrayItem);
          //      Writer.WriteEndElement(itemName);
          //    }
          //  }
          //}
          //else
          //{
          //  WriteValue(ConvertMemberValueToString(memberInfo, propValue));
          //}         
          if (!String.IsNullOrEmpty(propTag))
            Writer.WriteEndElement(propTag);
        }
        propsWritten++;
      }
    }
    return propsWritten;
  }

  public int WriteContentProperty(object? context, string? elementTag, string? propTag, SerializationMemberInfo contentMemberInfo,
    SerializationTypeInfo typeInfo)
  {
    if (typeInfo.ContentInfo != null)
      return WriteCollectionContentProperty(context, elementTag, propTag, contentMemberInfo, typeInfo.ContentInfo);
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
            if (propTag != null)
            {
              if (Options?.PrecedePropertyNameWithClassName == true)
                propTag = elementTag + "." + propTag;
            }
            if (value.GetType().IsSimple())
            {
              if (!string.IsNullOrEmpty(propTag))
                Writer.WriteStartElement(propTag);
              WriteValue(value);
              if (!string.IsNullOrEmpty(propTag))
                Writer.WriteEndElement(propTag);
            }
            else
            {
              ITypeDescriptorContext? typeDescriptorContext = (context != null) ? new TypeDescriptorContext(context) : null;
              if (typeConverter != null && typeConverter.CanConvertTo(typeDescriptorContext, typeof(string)))
              {
                if (!string.IsNullOrEmpty(propTag))
                  Writer.WriteStartElement(propTag);
                var str = typeConverter.ConvertToInvariantString(typeDescriptorContext, value);
                WriteValue(str);
                if (!string.IsNullOrEmpty(propTag))
                  Writer.WriteEndElement(propTag);
              }
              else
              {
                if (!string.IsNullOrEmpty(propTag))
                  Writer.WriteStartElement(propTag);
                WriteObjectInterior(value, propTag);
                if (!string.IsNullOrEmpty(propTag))
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

  public int WriteCollectionContentProperty(object? Context, string? elementTag, string? propTag, SerializationMemberInfo contentMemberInfo,
    ContentItemInfo contentInfo/*, SerializationTypeInfo typeInfo*/)
  {
    if (contentInfo != null)
    {
      var itemTypes = contentInfo.KnownItemTypes;
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
              WriteValue(item);
              Writer.WriteEndElement(itemTag);
            }
            else
            {
              if (typeConverter != null)
              {
                if (!string.IsNullOrEmpty(itemTag))
                  Writer.WriteStartElement(itemTag);
                WriteValue(typeConverter.ConvertToString(item) ?? "");
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
                  WriteObjectInterior(item, itemTag);
                  Writer.WriteEndElement(itemTag);
                }
              }
            }
          }
        }
        if (!String.IsNullOrEmpty(propTag))
          Writer.WriteEndElement(propTag);
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
          WriteValue(item);
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
                  WriteObjectInterior(val);
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
            WriteValue(typeConverter.ConvertToString(item) ?? "");
            if (!string.IsNullOrEmpty(itemTag))
              Writer.WriteEndElement(itemTag);
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
                Writer.WriteStartElement(itemTag);
                WriteValue(item.ToString());
                Writer.WriteEndElement(itemTag);
              }
            }
            //if (KnownTypes.TryGetValue(item.GetType(), out var serializationTypeInfo))
            //  WriteObject(item);
            //else
            {
              Writer.WriteStartElement(itemTag);
              WriteObjectInterior(item, itemTag);
              Writer.WriteEndElement(itemTag);
            }
          }
        }
      }
      itemsWritten++;
    }
    if (!String.IsNullOrEmpty(propTag))
      Writer.WriteEndElement(propTag);

    Writer.Flush();
    return itemsWritten;
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
}