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
    var tag = CreateElementTag(serializedTypeInfo, aType);
    WriteObject(context, obj, tag);
  }

  public void WriteObject(object? context, object obj, XmlQualifiedTagName? tag)
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
        Writer.EmitNamespaces = Options.EmitNamespaces;
        if (tag != null)
          Writer.WriteStartElement(tag);
        if (Options.EmitNamespaces && context == null)
        {
          if (Options.UseNilValue)
            Writer.WriteNamespaceDef("xsi", QXmlSerializationHelper.xsiNamespace);

          if (Options.UseXsdScheme)
            Writer.WriteNamespaceDef("xsd", QXmlSerializationHelper.xsdNamespace);

          if (Options.AutoSetPrefixes)
            foreach (var item in KnownNamespaces)
              if (item.Prefix != null)
                Writer.WriteNamespaceDef(item.Prefix, item.XmlNamespace);
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
    {
      WritePropertiesAsElements(context, obj, null, typeInfo);
      WriteContentProperty(obj, null, null, typeInfo.ContentProperty, typeInfo);
    }
    else
    if (typeInfo.TextProperty != null)
    {
      WritePropertiesAsElements(context, obj, null, typeInfo);
      WriteContentProperty(obj, null, null, typeInfo.TextProperty, typeInfo);
    }
    else
    if (typeInfo.IsCollection && typeInfo.ContentInfo != null && obj is IEnumerable collection)
    {
      WritePropertiesAsElements(context, obj, null, typeInfo);
      WriteCollectionProperty(context, collection, null, null, typeInfo.ContentInfo);
    }
    else
      WritePropertiesAsElements(context, obj, null, typeInfo);
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

      var attrTag = CreateAttributeTag(memberInfo);
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
      var propValue = memberInfo.GetValue(obj);
      var propTag = CreateElementTag(memberInfo, propValue?.GetType());
      if (propValue == null)
      {
        if (Options?.UseNilValue == true && memberInfo.IsNullable)
        {
          if (propTag != null)
            Writer.WriteStartElement(propTag);
          Writer.WriteNilAttribute();
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
              WriteValue(context, ConvertMemberValueToString(memberInfo, propValue));
            else if (memberInfo.IsReference)
              WriteValue(context, ConvertMemberValueToString(memberInfo, propValue));
            else if (propValue is ICollection collection)
            {
              if (memberInfo.ValueType?.MembersAsAttributes.Count > 0 || memberInfo.IsObject /*&& memberInfo.ContentInfo != null*/)
                WriteObject(context, propValue);
              else
                WriteCollectionItems(context, collection, elementTag, null, memberInfo.ContentInfo);
            }
            else
              WriteObject(context, propValue);
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
    ContentItemInfo contentInfo)
  {
    var result = 0;
    if (contentInfo != null)
    {
      var itemTypes = contentInfo.KnownItemTypes;
      var propInfo = contentMemberInfo.Property;
      if (propInfo != null)
      {
        var propTag = CreatePropertyTag(elementTag, propName);
        var collection = propInfo.GetValue(context, null) as IEnumerable;
        if (collection != null)
          result = WriteCollectionItems(context, collection, null, null, contentInfo);
        if (propTag != null)
          Writer.WriteEndElement(propTag);
      }
    }
    return result;
  }

  public int WriteCollectionProperty(object? context, IEnumerable collection, XmlQualifiedTagName? elementTag, string? propName, ContentItemInfo collectionInfo)
  {
    var propTag = CreatePropertyTag(elementTag, propName);
    if (propTag != null)
      Writer.WriteStartElement(propTag);
    var itemsWritten = WriteCollectionItems(context, collection, propTag, null, collectionInfo);
    if (propTag != null)
      Writer.WriteEndElement(propTag);
    return itemsWritten;
  }

  public int WriteCollectionItems(object? context, IEnumerable collection, XmlQualifiedTagName? elementTag, string? propName, ContentItemInfo? collectionInfo)
  {
    int itemsWritten = 0;
    var itemTypes = collectionInfo?.KnownItemTypes;
    foreach (var item in collection)
    {
      if (item == null)
      {
        Writer.WriteStartElement("null");
        Writer.WriteEndElement("null");
      }
      else
      {
        var itemType = item.GetType();
        XmlQualifiedTagName itemTag = CreateElementTag(itemType);
        TypeConverter? typeConverter = null;
        if (itemTypes != null)
        {
          if (!itemTypes.TryGetValue(itemType, out var itemTypeInfo))
            itemTypeInfo = itemTypes.FindTypeInfo(itemType);

          if (itemTypeInfo != null)
          {
            itemTag = CreateElementTag(itemTypeInfo, item.GetType());
            typeConverter = itemTypeInfo.TypeInfo.TypeConverter;
          }
        }

        if (item.GetType().IsSimple())
        {
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
            Writer.WriteStartElement(itemTag);
            WriteValue(context, typeConverter.ConvertToString(item) ?? "");
            Writer.WriteEndElement(itemTag);
          }
          else
          {
            if (collectionInfo?.StoresReferences == true)
            {
              Writer.WriteStartElement(itemTag);
              WriteValue(context, item.ToString());
              Writer.WriteEndElement(itemTag);
            }
            else
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
        if (value is string str)
        {
          //if (str.Contains("\\"))
          //  TestTools.Stop();
          var valStr = str.EncodeStringValue();
          Writer.WriteValue(valStr);
        }
        else
        {
          var typeConverter = new ValueTypeConverter(value.GetType(), KnownTypes.Keys, KnownNamespaces.XmlNamespaceToPrefix, null, null, null, Options.ConversionOptions);
          var valStr = typeConverter.ConvertToInvariantString(value);
          if (valStr != null)
          {
            Writer.WriteValue(valStr);
          }
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
    if (memberInfo.ValueType?.Type == typeof(string) && propValue is string valStr)
    {
      var str = valStr.EncodeStringValue();
      return str;
    }
    var typeConverter = memberInfo.GetTypeConverter();
    if (typeConverter != null)
    {
      var str = typeConverter.ConvertToInvariantString(propValue);
      return str;
    }
    else
    {
      typeConverter = new ValueTypeConverter(memberInfo.Property.PropertyType, KnownTypes.Keys, KnownNamespaces.XmlNamespaceToPrefix, memberInfo.DataType,
        memberInfo.Format, memberInfo.Culture, /*memberInfo.ConversionOptions ?? */Options.ConversionOptions);
      var str = typeConverter.ConvertToInvariantString(propValue);
      return str;
    }
  }

  #endregion

  #region Create element tag methods

  protected XmlQualifiedTagName CreateElementTag(Type type)
  {
    var typeName = TypeNaming.GetTypeName(type);
    var nspace = type.Namespace ?? "";
    if (!typeName.Contains('.'))
      return new XmlQualifiedTagName(typeName);

    var result = Mapper.GetXmlTag(type);
    if (KnownNamespaces.XmlNamespaceToPrefix.TryGetValue(nspace, out var prefix))
    {
      result.Prefix = prefix;
      KnownNamespaces[nspace].IsUsed = true;
    }
    return result;
  }

  protected XmlQualifiedTagName CreateElementTag(SerializationTypeInfo typeInfo, Type? type)
  {
    if (type != null)
      return CreateElementTag(type);
    if (typeInfo.XmlNamespace != null)
    {
      KnownNamespaces[typeInfo.XmlNamespace].IsUsed = true;
      var typeName = TypeNaming.GetTypeName(typeInfo.Type);
      if (!typeName.Contains('.'))
        return new XmlQualifiedTagName(typeName);
    }
    var result = Mapper.GetXmlTag(typeInfo);
    if (typeInfo.XmlNamespace != null)
      result.Prefix = KnownNamespaces[typeInfo.XmlNamespace].Prefix;
    return result;
  }

  protected XmlQualifiedTagName? CreateElementTag(SerializationMemberInfo memberInfo, Type? type)
  {
    if (memberInfo.IsTagSuppressed)
      return null;
    var result = Mapper.GetXmlTag(memberInfo);
    result.Namespace = "";
    return result;
  }

  protected XmlQualifiedTagName CreateElementTag(SerializationItemInfo itemInfo, Type? type)
  {
    if (type != null)
      return CreateElementTag(type);
    if (itemInfo.XmlNamespace != null)
    {
      KnownNamespaces[itemInfo.XmlNamespace].IsUsed = true;
      if (itemInfo.Type != null)
      {
        var typeName = TypeNaming.GetTypeName(itemInfo.Type);
        if (!typeName.Contains('.'))
          return new XmlQualifiedTagName(typeName);
      }
    }
    var result = Mapper.GetXmlTag(itemInfo);
    if (itemInfo.XmlNamespace != null)
      result.Prefix = KnownNamespaces[itemInfo.XmlNamespace].Prefix;
    return result;
  }

  protected XmlQualifiedTagName CreateAttributeTag(SerializationMemberInfo memberInfo/*, Type? type*/)
  {
    //if (type!=null)
    // return CreateElementTag(type);
    if (memberInfo.XmlNamespace != null)
    {
      KnownNamespaces[memberInfo.XmlNamespace].IsUsed = true;
      if (memberInfo.MemberType != null)
      {
        var typeName = TypeNaming.GetTypeName(memberInfo.MemberType);
        if (!typeName.Contains('.'))
          return new XmlQualifiedTagName(typeName);
      }
    }
    var result = Mapper.GetXmlTag(memberInfo);
    if (memberInfo.XmlNamespace != null)
      result.Prefix = KnownNamespaces[memberInfo.XmlNamespace].Prefix;
    return result;
  }

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
  #endregion
}