namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{

  /// <summary>
  ///   Main serialization entry.
  /// </summary>
  /// <param name="xmlWriter">The target of serialization.</param>
  /// <param name="obj">Serialized object.</param>
  public void SerializeObject(XmlWriter xmlWriter, object? obj)
  {
    if (obj == null)
      return;
    Writer = new QXmlWriter(xmlWriter);
    WriteObject(obj);
    xmlWriter.Flush();
  }

  /// <summary>
  /// System settings for XmlWriter.
  /// </summary>
  public XmlWriterSettings XmlWriterSettings { get; } = new()
  {
    Indent = true,
    NamespaceHandling = NamespaceHandling.OmitDuplicates,
  };

  /// <summary>
  /// System XmlWriter wrapper.
  /// </summary>
  public QXmlWriter Writer { get; protected set; } = null!;

  #region Write methods

  /// <summary>
  /// Contextless object write method.
  /// </summary>
  /// <param name="obj">Object to writer</param>
  public void WriteObject(object obj)
  {
    WriteObject(null, obj);
  }

  /// <summary>
  /// Contextless object write method.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="obj">Object to writer</param>
  public void WriteObject(object? context, object obj)
  {
    var aType = obj.GetType();
    if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
      throw new InternalException($"Type \"{aType}\" not registered");
    var tag = CreateElementTag(serializedTypeInfo, aType);
    WriteObject(context, obj, tag);
  }

  /// <summary>
  /// Writes an object using specific tag name.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="obj">Object to writer</param>
  /// <param name="tag">Tag name of the opening and closing element</param>
  /// <exception cref="InternalException">Thrown when the object type is not registered.</exception>
  /// <remarks>
  /// If a type has XmlConverter, this converter is used. 
  /// If not <see cref="WriteObjectInterior"/> is invoked.
  /// </remarks>
  public void WriteObject(object? context, object obj, XmlQualifiedTagName? tag)
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

      WriteObjectInterior(context, obj, serializedTypeInfo);
      if (tag != null)
        Writer.WriteEndElement(tag);
    }
  }

  /// <summary>
  /// Writes an object attributes, elements and content of the object.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="obj">Object to writer</param>
  /// <param name="typeInfo">Serialization info for the object type.</param>
  /// <exception cref="InternalException">Thrown when the object type is not registered.</exception>
  public void WriteObjectInterior(object? context, object obj, SerializationTypeInfo? typeInfo = null)
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
      WriteConvertedString(typeInfo.TypeConverter, obj);
    }
    else
    if (typeInfo.Type.IsSimple())
    {
      var typeConverter = new ValueTypeConverter(typeInfo.Type);
      WriteConvertedString(typeConverter, obj);
    }
    else
    if (typeInfo.ContentProperty != null)
    {
      WritePropertiesAsElements(context, obj, typeInfo);
      WriteContentProperty(obj, typeInfo.ContentProperty);
    }
    else
    if (typeInfo.TextProperty != null)
    {
      WritePropertiesAsElements(context, obj, typeInfo);
      WriteContentProperty(obj, typeInfo.TextProperty);
    }
    else
    if (typeInfo.IsCollection && typeInfo.ContentInfo is ContentItemInfo contentItemInfo && obj is IEnumerable collection)
    {
      WritePropertiesAsElements(context, obj, typeInfo);
      WriteCollectionElement(context, collection, null, contentItemInfo);
    }
    else
    if (typeInfo.IsDictionary && typeInfo.ContentInfo is DictionaryInfo dictionaryInfo && obj is IDictionary dictionary)
    {
      WritePropertiesAsElements(context, obj, typeInfo);
      WriteDictionaryElement(context, dictionary, null, dictionaryInfo);
    }
    else
      WritePropertiesAsElements(context, obj, typeInfo);
  }

  /// <summary>
  /// Writes properties declared to serialize as attributes.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="obj">Object to writer</param>
  /// <param name="typeInfo">Serialization info for the object type.</param>
  /// <returns>Number of properties written</returns>
  public int WritePropertiesAsAttributes(object? context, object obj, SerializationTypeInfo typeInfo)
  {
    var type = obj.GetType();
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

      var attrTag = CreateAttributeTag(memberInfo, type);
      var propValue = memberInfo.GetValue(obj);
      if (propValue != null)
      {
        var defaultValue = memberInfo.DefaultValue;
        if (defaultValue != null && propValue.Equals(defaultValue))
          continue;
        var str = ConvertMemberValueToString(propValue, memberInfo);
        if (str != null)
        {
          Writer.WriteAttributeString(attrTag, str);
          attrsWritten++;
        }
      }
    }
    return attrsWritten;
  }

  /// <summary>
  /// Writes properties declared to serialize as properties.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="obj">Object to write</param>
  /// <param name="typeInfo">Serialization info for the object type.</param>
  /// <returns>Number of properties written</returns>
  public int WritePropertiesAsElements(object? context, object obj, SerializationTypeInfo typeInfo)
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
          if (memberInfo.IsContentElement)
            propTag = Mapper.GetXmlTag(memberInfo.ValueType);
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
                WriteConvertedString(memberInfo.TypeConverter, obj);
              else
                WriteValue(propValue);
            }
            else if (pType.IsArray(out var itemType) && itemType == typeof(byte))
              WriteValue(ConvertMemberValueToString(propValue, memberInfo));
            else if (memberInfo.IsReference)
              WriteValue(ConvertMemberValueToString(propValue, memberInfo));
            else if (propValue is ICollection collection)
            {
              if (memberInfo.ValueType.MembersAsAttributes.Count() > 0 || memberInfo.IsObject /*&& memberInfo.ContentInfo != null*/)
                WriteObject(context, propValue);
              else
                WriteCollectionItems(context, collection, memberInfo.ContentInfo);
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

  /// <summary>
  /// Writes a property declared to serialize as content or text property.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="contentMemberInfo">Content property info</param>
  /// <returns>Number of properties written</returns>
  /// <remarks>
  /// If a property has XmlConverter or TypeConverter, this converter is used.
  /// Simple value is written by using <see cref="WriteValue"/>, not simple by <see cref="WriteObjectInterior"/>.
  /// </remarks>
  public int WriteContentProperty(object? context, SerializationMemberInfo contentMemberInfo)
  {
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
            ITypeDescriptorContext? typeDescriptorContext = (context != null) ? new TypeDescriptorContext(context) : null;
            if (typeConverter != null && typeConverter.CanConvertTo(typeDescriptorContext, typeof(string)))
            {
              var str = typeConverter.ConvertToInvariantString(typeDescriptorContext, value);
              if (str != null && str.Length > 0 && (Char.IsWhiteSpace(str.First()) || Char.IsWhiteSpace(str.Last())))
                Writer.WriteSignificantSpaces(true);
              WriteValue(str);
            }
            else
            {
              if (value.GetType().IsSimple())
                WriteValue(value);
              else
                WriteObjectInterior(context, value, null);
            }
          }
        }
        return 1;
      }
    }
    return 0;
  }

  /// <summary>
  /// Writes a collection property of the object.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="collection">Collection of the items to write</param>
  /// <param name="elementTag">Name of the opening element that encapsulates items elements.</param>
  /// <param name="collectionInfo">Serialization info of collection items.</param>
  /// <returns>Number of items written</returns>
  /// <remarks>Uses <see cref="WriteCollectionItems"/></remarks>
  public int WriteCollectionElement(object? context, IEnumerable collection, XmlQualifiedTagName? elementTag, ContentItemInfo collectionInfo)
  {
    if (elementTag != null)
      Writer.WriteStartElement(elementTag);
    var itemsWritten = WriteCollectionItems(context, collection, collectionInfo);
    if (elementTag != null)
      Writer.WriteEndElement(elementTag);
    return itemsWritten;
  }

  /// <summary>
  /// Writes collection items.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="collection">Collection of the items to write</param>
  /// <param name="collectionInfo">Serialization info of collection items.</param>
  /// <returns>Number of items written</returns>
  public int WriteCollectionItems(object? context, IEnumerable collection, ContentItemInfo? collectionInfo)
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
          WriteValue(item);
          Writer.WriteEndElement(itemTag);
        }
        //else if (item is IXSerializable serializableItem)
        //  serializableItem.Serialize(this);
        //else if (item.GetType().Name.StartsWith("KeyValuePair`"))
        //{
        //  var keyProp = item.GetType().GetProperty("Key");
        //  var valProp = item.GetType().GetProperty("Value");
        //  if (keyProp != null && valProp != null)
        //  {
        //    var key = keyProp.GetValue(item);
        //    var val = valProp.GetValue(item);
        //    if (key != null)
        //    {
        //      if (collectionInfo is DictionaryInfo dictionaryInfo && dictionaryInfo.KeyProperty != null && val != null
        //          && KnownTypes.TryGetValue(val.GetType(), out var serializationTypeInfo))
        //      {
        //        WriteObject(val);
        //      }
        //      else
        //      {
        //        Writer.WriteStartElement(itemTag);
        //        Writer.WriteAttributeString("key", key.ToString() ?? "");
        //        if (val != null)
        //          WriteObjectInterior(context, val, null);
        //        Writer.WriteEndElement(itemTag);
        //      }
        //    }
        //  }
        //}
        else
        {
          if (typeConverter != null)
          {
            Writer.WriteStartElement(itemTag);
            var str = typeConverter.ConvertToString(item) ?? "";
            WriteString(str);
            Writer.WriteEndElement(itemTag);
          }
          else
          {
            if (collectionInfo?.StoresReferences == true)
            {
              Writer.WriteStartElement(itemTag);
              WriteString(item.ToString());
              Writer.WriteEndElement(itemTag);
            }
            else
            {
              Writer.WriteStartElement(itemTag);
              WriteObjectInterior(context, item);
              Writer.WriteEndElement(itemTag);
            }
          }
        }
      }
      itemsWritten++;
    }
    return itemsWritten;
  }

  /// <summary>
  /// Writes a dictionary property of the object.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="dictionary">Collection of dictionary items to write</param>
  /// <param name="elementTag">Name of the opening element that encapsulates items elements.</param>
  /// <param name="dictionaryInfo">Serialization info of dictionary items.</param>
  /// <returns>Number of items written</returns>
  /// <remarks>Uses <see cref="WriteDictionaryItems"/></remarks>
  public int WriteDictionaryElement(object? context, IDictionary dictionary, XmlQualifiedTagName? elementTag, DictionaryInfo dictionaryInfo)
  {
    if (elementTag != null)
      Writer.WriteStartElement(elementTag);
    var itemsWritten = WriteDictionaryItems(context, dictionary, dictionaryInfo);
    if (elementTag != null)
      Writer.WriteEndElement(elementTag);
    return itemsWritten;
  }

  /// <summary>
  /// Writes dictionary items.
  /// </summary>
  /// <param name="context">Context object for write operation. Usually a container of the written object</param>
  /// <param name="dictionary">Collection of dictionary items to write</param>
  /// <param name="dictionaryInfo">Serialization info of dictionary items.</param>
  /// <returns>Number of items written</returns>
  public int WriteDictionaryItems(object? context, IDictionary dictionary, DictionaryInfo dictionaryInfo)
  {
    var keyTypeInfo = dictionaryInfo.KeyTypeInfo;
    var valueTypeInfo = dictionaryInfo.ValueTypeInfo;
    int itemsWritten = 0;
    var itemTypes = dictionaryInfo.KnownItemTypes;
    var enumerator = dictionary.GetEnumerator();
    while (enumerator.MoveNext())
    {
      var key = enumerator.Key;
      var value = enumerator.Value;
      var itemTag = new XmlQualifiedTagName("Item");
      Writer.WriteStartElement(itemTag);
      var keyName = dictionaryInfo.KeyName ?? "Key";
      Writer.WriteAttributeString(keyName, key.ToString());
      if (value == null)
      {
        Writer.WriteStartElement("null");
        Writer.WriteEndElement("null");
      }
      else
      {
        var valueType = value.GetType();
        var valueTag = CreateElementTag(valueType);
        TypeConverter? typeConverter = null;
        if (itemTypes != null)
        {
          if (!itemTypes.TryGetValue(valueType, out var itemTypeInfo))
            itemTypeInfo = itemTypes.FindTypeInfo(valueType);
          if (itemTypeInfo != null)
          {
            valueTag = CreateElementTag(itemTypeInfo, value.GetType());
            typeConverter = itemTypeInfo.TypeInfo.TypeConverter;
          }
        }

        if (valueType.IsSimple())
        {
          Writer.WriteStartElement(valueTag);
          WriteValue(value);
          Writer.WriteEndElement(valueTag);
        }
        //else if (item is IXSerializable serializableItem)
        //  serializableItem.Serialize(this);
        else
        {
          if (typeConverter != null)
          {
            Writer.WriteStartElement(valueTag);
            WriteConvertedString(typeConverter, value);
            Writer.WriteEndElement(valueTag);
          }
          else
          {
            if (dictionaryInfo.StoresReferences == true)
            {
              Writer.WriteStartElement(valueTag);
              WriteString(value.ToString());
              Writer.WriteEndElement(valueTag);
            }
            else
            {
              Writer.WriteStartElement(valueTag);
              WriteObjectInterior(context, value);
              Writer.WriteEndElement(valueTag);
            }
          }
        }
      }
      Writer.WriteEndElement(itemTag);
      itemsWritten++;
    }
    return itemsWritten;
  }

  /// <summary>
  /// Writes a simple value of the object
  /// </summary>
  /// <param name="value">value to write</param>
  /// <remarks>
  /// If value is a string, it is encoded to handle html entities and control characters
  /// Value of other types is converted by using its <see cref="ValueTypeConverter"/>
  /// </remarks>
  public void WriteValue(object? value)
  {
    if (value != null)
    {
      if (value is string str)
      {
        WriteString(str);
      }
      else
      {
        var typeConverter = new ValueTypeConverter(value.GetType(), KnownTypes.Keys, KnownNamespaces.XmlNamespaceToPrefix, null, null, null, Options.ConversionOptions);
        var valStr = (string?)typeConverter.ConvertToInvariantString(value);
        if (valStr != null)
        {
          WriteRawString(valStr);
        }
      }
    }
  }

  /// <summary>
  /// Writes object converted to string.
  /// </summary>
  /// <param name="typeConverter">Type converter (must not be null)</param>
  /// <param name="obj">Object to convert (may be null)</param>
  public void WriteConvertedString(TypeConverter typeConverter, object? obj)
  {
    var str = (string?)typeConverter.ConvertToInvariantString(obj);
    if (str!=null)
    {
      if (typeConverter is Base64TypeConverter)
        WriteLongString(str, Options.LineLengthLimit);
      else
        WriteString(str);
    }
  }

  /// <summary>
  /// Writes a string encoded to handle html entities and control characters.
  /// </summary>
  /// <param name="value">string to write</param>
  public void WriteString(string? value)
  {
    if (value is string str)
    {
      var valStr = str.EncodeStringValue();
      Writer.WriteString(valStr);
    }
  }

  /// <summary>
  /// Writes a string encoded with Base64Binary converter.
  /// </summary>
  /// <param name="value">String to write</param>
  /// <param name="lineLengthLimit">Divides long string to lines</param>
  public void WriteLongString(string? value, int lineLengthLimit)
  {
    if (value is string str)
    {
      if (str.Length>lineLengthLimit)
      {
        var n = str.Length / lineLengthLimit;
        for (int i = 0; i <= n; i++)
        {
          int k = i*lineLengthLimit;
          int l = Math.Min(lineLengthLimit, str.Length-k);
          var subString = str.Substring(k,l);
          if (i==0)
            subString = "\n"+subString;
          subString += "\n";
          Writer.WriteString(subString);
        }
        return;
      }
    }
    Writer.WriteString(value);
  }

  /// <summary>
  /// Writes a raw string - without encodind
  /// </summary>
  /// <param name="value">value to write</param>
  /// <remarks>
  /// If value is a string, it is encoded to handle html entities and control characters
  /// Value of other types is converted by using its <see cref="ValueTypeConverter"/>
  /// </remarks>
  public void WriteRawString(string? value)
  {
    if (value is string str)
    {
      if (str.Contains('&'))
        Debugger.Break();
      Writer.WriteString(str);
    }
  }

  /// <summary>
  /// Helper method to convert value of some member to string.
  /// </summary>
  /// <param name="memberInfo"></param>
  /// <param name="value"></param>
  /// <returns>string version of the value</returns>
  protected string? ConvertMemberValueToString(object? value, SerializationMemberInfo memberInfo)
  {
    if (value == null)
      return null;
    if (memberInfo.Property == null)
      return null;
    if (memberInfo.ValueType.Type == typeof(string) && value is string valStr)
    {
      var str = valStr.EncodeStringValue();
      return str;
    }
    var typeConverter = memberInfo.GetTypeConverter();
    if (typeConverter != null)
    {
      var str = typeConverter.ConvertToInvariantString(value);
      return str;
    }
    else
    {
      typeConverter = new ValueTypeConverter(memberInfo.Property.PropertyType, KnownTypes.Keys, KnownNamespaces.XmlNamespaceToPrefix, memberInfo.DataType,
        memberInfo.Format, memberInfo.Culture, /*memberInfo.ConversionOptions ?? */Options.ConversionOptions);
      var str = typeConverter.ConvertToInvariantString(value);
      return str;
    }
  }

  #endregion

  #region Create element tag methods

  /// <summary>
  /// Creates element tag for a specific type
  /// </summary>
  /// <param name="type">Type to create tag</param>
  /// <returns>Tag string</returns>
  /// <remarks>
  /// Creates element tag using type name and namespace.
  /// Uses <see cref="Mapper"/> to get XML tag.
  /// </remarks>
  protected XmlQualifiedTagName CreateElementTag(Type type)
  {
    var typeTag = type.GetTypeTag();
    if (!typeTag.Contains('.'))
      return new XmlQualifiedTagName(typeTag);

    var nspace = type.Namespace ?? "";
    var result = Mapper.GetXmlTag(type);
    if (KnownNamespaces.XmlNamespaceToPrefix.TryGetValue(nspace, out var prefix))
    {
      result.Prefix = prefix;
      KnownNamespaces[nspace].IsUsed = true;
    }
    return result;
  }

  /// <summary>
  /// Creates element tag for a specific typeInfo and type.
  /// </summary>
  /// <param name="typeInfo">Serialization info of the type to create tag</param>
  /// <param name="type">Type to create tag (can be different than specified by typeInfo)</param>
  /// <returns>Tag string</returns>
  /// <remarks>
  /// If a type is not null, then <see cref="CreateElementTag(Type)"/> is used.
  /// Otherwise uses <see cref="Mapper"/> to get XML tag.
  /// </remarks>
  protected XmlQualifiedTagName CreateElementTag(SerializationTypeInfo typeInfo, Type? type)
  {
    if (type != null)
      return CreateElementTag(type);
    if (typeInfo.XmlNamespace != null)
    {
      KnownNamespaces[typeInfo.XmlNamespace].IsUsed = true;
      var typeTag = typeInfo.Type.GetTypeTag();
      if (!typeTag.Contains('.'))
        return new XmlQualifiedTagName(typeTag);
    }
    var result = Mapper.GetXmlTag(typeInfo);
    if (typeInfo.XmlNamespace != null)
      result.Prefix = KnownNamespaces[typeInfo.XmlNamespace].Prefix;
    return result;
  }

  /// <summary>
  /// Creates element tag for a specific memberInfo and type.
  /// </summary>
  /// <param name="memberInfo">Serialization info of the member to create tag</param>
  /// <param name="type">Type to create tag (can be different than specified by memberInfo)</param>
  /// <returns>Tag string</returns>
  /// <remarks>
  /// If a member is a content element, then the method return null.
  /// Otherwise uses <see cref="Mapper"/> to get XML tag.
  /// Namespace of the result tag is set to an empty string.
  /// </remarks>
  protected XmlQualifiedTagName? CreateElementTag(SerializationMemberInfo memberInfo, Type? type)
  {
    if (memberInfo.IsContentElement)
      return null;
    var result = Mapper.GetXmlTag(memberInfo);
    result.Namespace = "";
    return result;
  }

  /// <summary>
  /// Creates element tag for a specific itemInfo and type.
  /// </summary>
  /// <param name="itemInfo">Serialization info of the item to create tag</param>
  /// <param name="type">Type to create tag (can be different than specified by itemInfo)</param>
  /// <returns>Tag string</returns>
  /// <remarks>
  /// If a type is not null, then <see cref="CreateElementTag(Type)"/> is used.
  /// Otherwise uses <see cref="Mapper"/> to get XML tag.
  /// </remarks>
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

  /// <summary>
  /// Creates attribute tag for a specific memberInfo and type.
  /// </summary>
  /// <param name="memberInfo">Serialization info of the member to create tag</param>
  /// <param name="type">Type to create tag (can be different than specified by memberInfo)</param>
  /// <returns>Tag string</returns>
  /// <remarks>
  /// Uses <see cref="Mapper"/> to get XML tag.
  /// Namespace of the result tag is set to an empty string.
  /// </remarks>
  protected XmlQualifiedTagName CreateAttributeTag(SerializationMemberInfo memberInfo, Type? type)
  {
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

  #endregion
}