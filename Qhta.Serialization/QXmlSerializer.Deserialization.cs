using Qhta.Conversion;

namespace Qhta.Xml.Serialization;

public delegate void OnUnknownMember(object readObject, string elementName);

public partial class QXmlSerializer
{

  #region Deserialize methods
  //public object? Deserialize(Stream stream)
  //{
  //  using (var xmlReader = new XmlTextReader(stream))
  //  {
  //    xmlReader.WhitespaceHandling = WhitespaceHandling.None;
  //    return Deserialize(xmlReader);
  //  }
  //}

  //public object? Deserialize(TextReader textReader)
  //{
  //  using (var xmlReader = new XmlTextReader(textReader))
  //  {
  //    xmlReader.WhitespaceHandling = WhitespaceHandling.None;
  //    return Deserialize(xmlReader);
  //  }
  //}

  //public object? Deserialize(string str)
  //{
  //  using (var xmlReader = new XmlTextReader(new StringReader(str)))
  //  {
  //    xmlReader.WhitespaceHandling = WhitespaceHandling.None;
  //    return Deserialize(xmlReader);
  //  }
  //}

  public partial bool CanDeserialize(XmlReader xmlReader)
  {
    throw new NotImplementedException("CanDeserialize(XmlReader)");
  }



  protected partial object? DeserializeObject(XmlReader xmlReader, string? encodingStyle, XmlDeserializationEvents events)
  {
    if (xmlReader == null)
      throw new InternalException("$Reader must be set prior to deserialize");
    SkipToElement(xmlReader);
    if (xmlReader.EOF)
      return null;
    var name = xmlReader.Name;
    var prefix = xmlReader.Prefix;
    SerializationTypeInfo? typeInfo;
    //if (String.IsNullOrEmpty(prefix))
    //{
    //  if (!KnownTypes.TryGetValue(name, out typeInfo))
    //    throw new XmlInternalException($"Element {name} not recognized while deserialization.", xmlReader);
    //}
    //else
    {
      var xmlName = new XmlQualifiedTagName(name, prefix);
      var clrName = Mapper.ToQualifiedName(xmlName);
      if (!KnownTypes.TryGetValue(clrName, out typeInfo))
        throw new XmlInternalException($"Element {xmlName} not recognized while deserialization.", xmlReader);
    }
    var constructor = typeInfo.KnownConstructor;
    if (constructor == null)
      throw new XmlInternalException($"Type {typeInfo.Type.Name} must have a public, parameterless constructor.", xmlReader);
    var obj = constructor.Invoke(new object[0]);

    //if (obj is IXSerializable qSerializable)
    //{
    //  qSerializable.Deserialize(this);
    //}
    //else 
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(xmlReader);
    }
    else
    {
      ReadObject(obj, xmlReader, typeInfo);
    }
    return obj;
  }


  //public object? Deserialize(XmlReader xmlReader, string? encodingStyle)
  //{
  //  throw new NotImplementedException($"XmlSerializer.Deserialize");
  //}

  //public object? Deserialize(XmlReader xmlReader, string? encodingStyle, XmlDeserializationEvents events)
  //{
  //  throw new NotImplementedException($"XmlSerializer.Deserialize");
  //}

  //public bool CanDeserialize(XmlReader xmlReader)
  //{
  //  throw new NotImplementedException($"XmlSerializer.CanDeserialize");
  //}

  #endregion

  #region Read methods

  public void SkipToElement(XmlReader reader)
  {
    while (!reader.EOF && reader.NodeType != XmlNodeType.Element)
      reader.Read();
  }

  public void SkipWhitespaces(XmlReader reader)
  {
    while (reader.NodeType == XmlNodeType.Whitespace)
      reader.Read();
  }

  public void ReadObject(object obj, XmlReader reader, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
    if (reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
    bool isEmptyElement = reader.IsEmptyElement;
    ReadAttributesBase(obj, reader, typeInfo, onUnknownMember);
    reader.Read(); // read end of start element and go to its content;
    SkipWhitespaces(reader);
    if (isEmptyElement)
      return;
    ReadElementsBase(obj, reader, typeInfo);
    ReadTextMemberValue(obj, reader);
    if (reader.NodeType == XmlNodeType.EndElement)
      reader.Read();
  }

  public int ReadAttributesBase(object obj, XmlReader reader, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
    var aType = obj.GetType();
    var attribs = typeInfo.MembersAsAttributes;
    var props = typeInfo.MembersAsElements;
    foreach (var prop in props)
      if (!attribs.ContainsKey(prop.QualifiedName))
        attribs.Add(prop.QualifiedName, prop);
    var propList = attribs;

    int attrsRead = 0;
    reader.MoveToFirstAttribute();
    var attrCount = reader.AttributeCount;
    for (int i = 0; i < attrCount; i++)
    {
      string attrPrefix = reader.Prefix;
      string attrName = reader.LocalName;
      if (attrName == "xmlns" && attrPrefix == "")
      {
        attrPrefix = "xmlns";
        attrName = "";
      }
      if (!String.IsNullOrEmpty(attrPrefix))
      {
        TestTools.Stop();
        switch (attrPrefix)
        {
          case "xml":
            if (attrName == "space")
            {
              reader.ReadAttributeValue();
              var str = reader.ReadContentAsString();
              if (reader is XmlTextReader xmlTextReader)
              {
                if (str == "preserve")
                  xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
                else
                  xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
              }
            }
            reader.MoveToNextAttribute();
            continue;
          case "xmlns":
            reader.ReadAttributeValue();
            var ns = reader.ReadContentAsString();
            Namespaces.Add(attrName, ns);
            reader.MoveToNextAttribute();
            continue;
          case "xsd":
            reader.MoveToNextAttribute();
            continue;
        }
      }

      var propertyToRead = propList.FirstOrDefault(item => item.XmlName == attrName);
      if (propertyToRead == null)
      {
        if (onUnknownMember == null)
          throw new XmlInternalException($"Unknown property for attribute \"{attrName}\" in type {aType.Name}", reader);
        onUnknownMember(obj, attrName);
      }
      else
      {
        reader.ReadAttributeValue();
        object? propValue = ReadValue(propertyToRead, reader);
        if (propValue != null)
        {
          try
          {
            propertyToRead.SetValue(obj, propValue);
          }
          catch (Exception ex)
          {
            throw new XmlInternalException($"Could not set value for property {propertyToRead.Member} in type {aType.Name}", reader, ex);
          }

          attrsRead++;
        }
      }

      reader.MoveToNextAttribute();
    }
    return attrsRead;
  }

  public int ReadElementsBase(object obj, XmlReader reader, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
    var aType = obj.GetType();
    var attribs = typeInfo.MembersAsAttributes;
    var props = typeInfo.MembersAsElements;
    foreach (var prop in attribs)
      if (!props.ContainsKey(prop.QualifiedName))
        props.Add(prop.QualifiedName, prop);
    var propList = props;


    int propsRead = 0;
    while (reader.NodeType == XmlNodeType.Element)
    {
      var elementPrefix = reader.Prefix;
      var elementName = reader.LocalName;
      //if (elementName == "Base64Binary")
      //  TestTools.Stop();
      var propertyToRead = propList.FirstOrDefault(item => item.XmlName == elementName);
      if (propertyToRead != null)
      {
        object? propValue = null;
        Type propType = propertyToRead.MemberType ?? typeof(Object);
        if (propertyToRead.XmlConverter != null)
        {
          propValue = propertyToRead.XmlConverter.ReadXml(reader, typeInfo, propertyToRead, null, this);
        }
        else
        {
          if (propType.IsClass && propType != typeof(string))
          {
            if (propertyToRead.CollectionInfo != null)
              ReadElementAsCollectionMember(obj, propertyToRead, propertyToRead.CollectionInfo, reader);
            else
            {
              propValue = ReadElementAsMember(propType, propertyToRead, reader);
            }
          }
          else

            propValue = ReadElementAsMember(propType, propertyToRead, reader);
        }
        if (propValue != null)
        {
          propertyToRead.SetValue(obj, propValue);
          propsRead++;
        }
      }
      else
      {
        if (typeInfo.ContentProperty != null)
        {
          var content = ReadElementAsObject(typeInfo.ContentProperty.MemberType, reader);
          if (content != null)
          {
            typeInfo.ContentProperty.SetValue(obj, content);
            propsRead++;
            continue;
          }
        }
        if (typeInfo.CollectionInfo?.KnownItemTypes != null)
        {
          if (!typeInfo.CollectionInfo.KnownItemTypes.TryGetValue(elementName, out var knownItemTypeInfo))
          {
            knownItemTypeInfo = (typeInfo.CollectionInfo.KnownItemTypes as IEnumerable<SerializationItemInfo>).FirstOrDefault();
            if (knownItemTypeInfo == null)
              if (KnownTypes.TryGetValue(elementName, out var itemTypeInfo))
                knownItemTypeInfo = new SerializationItemInfo(elementName, itemTypeInfo);
            //else
            //{
            //  throw new XmlInternalException($"Unrecognized element \"{elementName}\"", reader);
            //}
          }
          if (knownItemTypeInfo != null)
          {
            object? key = elementName;
            object? item;
            if (knownItemTypeInfo.DictionaryInfo != null)
            {
              var collectionInfo = typeInfo.CollectionInfo;
              if (collectionInfo is DictionaryInfo dictionaryInfo)
              {
                (key, item) = ReadElementAsKVPair(null, knownItemTypeInfo.Type, dictionaryInfo, reader);
              }
              else
                throw new InternalException($"TypeInfo({typeInfo.Type}).CollectionInfo must be a DictionaryInfo");
            }
            else
              item = ReadElementWithKnownTypeInfo(knownItemTypeInfo, reader);

            if (item == null)
              throw new XmlInternalException($"Item of type {knownItemTypeInfo.Type} could not be read at \"{elementName}\"", reader);

            if (key == null)
              throw new InternalException($"Key for {item} must be not null");

            if (knownItemTypeInfo.AddMethod != null)
            {
              if (obj is IDictionary dictionaryObj)
                knownItemTypeInfo.AddMethod.Invoke(obj, new object[] { key, item });
              else
                knownItemTypeInfo.AddMethod.Invoke(obj, new object[] { item });
            }
            else
            {
              if (obj is IDictionary dictionaryObj)
                dictionaryObj.Add(key, item);
              else
              {
                var adddMethod = obj.GetType().GetMethod("Add", new Type[] { knownItemTypeInfo.Type });
                if (adddMethod != null)
                  adddMethod.Invoke(obj, new object[] { item });
                else
                if (obj is ICollection collectionObj)
                  throw new XmlInternalException($"Add method for {knownItemTypeInfo.Type} item not found in type {aType.FullName}", reader);
              }
            }
            propsRead++;
            continue;
          }
        }
        if (onUnknownMember != null)
          onUnknownMember(obj, elementName);
        else
        {
          if (Options.IgnoreUnknownElements)
          {
            if (reader.IsEmptyElement)
              reader.Read();
            else
            {
              do
              {
                reader.Read();
              } while (!(reader.Name == elementName && !reader.IsStartElement()));
              reader.Read();
            }
            continue;
          }
          throw new XmlInternalException($"No property to read and no content property found for element \"{elementName}\"" +
                                         $" in type {aType.FullName}", reader);
        }
      }

      SkipWhitespaces(reader);
    }
    return propsRead;
  }

  public object? ReadElementAsObject(Type? expectedType, XmlReader reader)
  {
    if (reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
    var name = reader.Name;
    if (!KnownTypes.TryGetValue(name, out var typeInfo))
    {
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
    }
    if (expectedType != null)
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", reader);
    return ReadElementWithKnownTypeInfo(typeInfo, reader);
  }

  public object? ReadElementAsMember(Type? expectedType, SerializationMemberInfo memberInfo, XmlReader reader)
  {
    if (reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
    var name = reader.Name;
    if (!KnownTypes.TryGetValue(name, out var typeInfo))
    {
      if (expectedType != null)
        typeInfo = RegisterType(expectedType);
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
    }

    if (expectedType != null)
    {
      if (expectedType.IsNullable(out var baseType))
        expectedType = baseType;
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", reader);
    }

    return ReadMemberWithKnownTypeInfo(typeInfo, memberInfo, reader);
  }

  public object? ReadMemberWithKnownTypeInfo(SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo, XmlReader reader)
  {
    if (typeInfo.XmlConverter?.CanRead == true)
      return typeInfo.XmlConverter.ReadXml(reader, typeInfo, null, null, this);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType == typeof(byte))
      {
        reader.Read();
        var str = reader.ReadContentAsString();
        var result = ConvertMemberValueFromString(memberInfo, str);
        reader.Read();
        return result;
      }
      if (typeInfo.Type.IsArray)
      {
        reader.Read();
        return null;
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
    }
    var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(reader);
    }
    else
      ReadObject(obj, reader, typeInfo);
    return obj;
  }

  protected object? ConvertMemberValueFromString(SerializationMemberInfo memberInfo, string? str)
  {
    if (str == null)
      return null;
    if (memberInfo.Property == null)
      return null;
    var typeConverter = memberInfo.GetTypeConverter();
    if (typeConverter == null)
      typeConverter = new ValueTypeConverter(memberInfo.Property.PropertyType, memberInfo.DataType, 
        memberInfo.Format, memberInfo.Culture, memberInfo.ConversionOptions ?? Options.ConversionOptions);
    var result = typeConverter.ConvertFromInvariantString(str);
    return result;


    //if (propValue is string str)
    //  return EncodeStringValue(str);
    //if (propValue is bool || propValue is bool?)
    //  return ((bool)propValue) ? Options.TrueString : Options.FalseString;
    //if (propValue is DateTime || propValue is DateTime?)
    //  return ((DateTime)propValue).ToString(Options.DateTimeFormat, CultureInfo.InvariantCulture);
  }

  public object? ReadElementAsItem(Type? expectedType, CollectionInfo collectionInfo, XmlReader reader)
  {
    if (reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
    var name = reader.Name;
    if (name == "HeadingPair")
      TestTools.Stop();
    SerializationTypeInfo? typeInfo = null;
    if (collectionInfo.KnownItemTypes.Any())
    {
      if (collectionInfo.KnownItemTypes.TryGetValue(name, out var typeItemInfo))
        typeInfo = typeItemInfo.TypeInfo;
    }
    else
      KnownTypes.TryGetValue(name, out typeInfo);
    if (typeInfo == null)
      throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
    if (expectedType != null)
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", reader);
    return ReadElementWithKnownTypeInfo(typeInfo, reader);
  }

  public object? ReadElementWithKnownTypeInfo(SerializationTypeInfo typeInfo, XmlReader reader)
  {
    if (typeInfo.XmlConverter?.CanRead==true)
      return typeInfo.XmlConverter.ReadXml(reader, typeInfo, null, null, this);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType==typeof(byte))
      {
        reader.Read();
        var result = ReadValue(typeInfo.Type, null, reader);
        reader.Read();
        return result;
      }
      if (typeInfo.Type.IsArray)
      {
        reader.Read();
        return null;
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
    }
    var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
    //if (obj is IXSerializable qSerializable)
    //{
    //  qSerializable.Deserialize(this);
    //}
    //else 
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(reader);
    }
    else
      ReadObject(obj, reader, typeInfo);
    return obj;
  }

  public object? ReadElementWithKnownTypeInfo(SerializationItemInfo itemTypeInfo, XmlReader reader)
  {
    var typeInfo = itemTypeInfo.TypeInfo;
    if (typeInfo.XmlConverter != null && typeInfo.XmlConverter.CanRead)
      return typeInfo.XmlConverter.ReadXml(reader, typeInfo, null, itemTypeInfo, this);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple())
      {
        reader.Read();
        var result = ReadValue(typeInfo.Type, null, reader);
        reader.Read();
        return result;
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
    }
    var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
    //if (obj is IXSerializable qSerializable)
    //{
    //  qSerializable.Deserialize(this);
    //}
    //else 
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(reader);
    }
    else
      ReadObject(obj, reader, typeInfo);
    return obj;
  }


  public (object? key, object? value) ReadElementAsKVPair(Type? expectedKeyType, Type? expectedValueType, DictionaryInfo dictionaryInfo, XmlReader reader)
  {
    if (reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
    var keyName = reader.Name;
    SerializationTypeInfo? typeInfo = null;
    if (dictionaryInfo.KnownItemTypes.Count > 0)
    {
      if (dictionaryInfo.KnownItemTypes.TryGetValue(keyName, out var typeItemInfo))
      {
        typeInfo = typeItemInfo.TypeInfo;
        keyName = typeItemInfo.KeyName;
      }
    }

    if (keyName == null)
      keyName = dictionaryInfo.KeyName;
    if (typeInfo == null)
      throw new XmlInternalException($"Unknown type for element \"{keyName}\" on deserialize", reader);
    var foundValueType = typeInfo.Type;
    if (expectedValueType != null)
      if (!foundValueType.IsEqualOrSubclassOf(expectedValueType))
      {
        if (!expectedValueType.IsCollection(foundValueType))
          throw new XmlInternalException($"Element \"{keyName}\" is mapped to {typeInfo.Type.Name}" +
                                         $" but {expectedValueType.Name} or its subclass expected", reader);
      }
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple())
      {
        var emptyElement = reader.IsEmptyElement;
        if (keyName == null)
          throw new XmlInternalException($"Key name unknown for dictionary item \"{reader.Name}\" on deserialize", reader);
        reader.MoveToAttribute(keyName);
        var key = reader.Value;
        if (emptyElement)
        {
          reader.Read(); // read to end of start element
          return (key, null);
        }
        else
        {
          reader.Read(); // read to end of start element
          var result = ReadValue(typeInfo.Type, null, reader);
          reader.Read();
          return (key, result);
        }
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
    }
    KVPair kvPair = new();
    kvPair.Value = typeInfo.KnownConstructor.Invoke(new object[0]);
    ReadObject(kvPair.Value, reader, typeInfo, (object readObject, string elementName) =>
    {
      if (reader.NodeType == XmlNodeType.Attribute)
      {
        if (elementName == keyName)
        {
          var keyValue = ReadValue(dictionaryInfo.KeyTypeInfo?.Type ?? typeof(string), null, reader);
          kvPair.Key = keyValue;
        }
        else
          throw new XmlInternalException($"Unknown attribute \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", reader);
      }
      else
      if (reader.NodeType == XmlNodeType.Element)
      {
        throw new XmlInternalException($"Unknown element \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", reader);
      }
    });
    return (kvPair.Key, kvPair.Value);
  }

  public enum CollectionTypeKind
  {
    Array,
    Collection,
    List,
    Dictionary,
  }

  public record KVPair
  {
    public object? Key { get; set; }
    public object? Value { get; set; }

    internal void Deconstruct(out object? key, out object? item)
    {
      key = Key;
      item = Value;
    }
  }

  public void ReadElementAsCollectionMember(object obj, SerializationMemberInfo propertyInfo, CollectionInfo collectionInfo, XmlReader reader)
  {
    if (propertyInfo.Member.Name == "HeadingPairs")
      TestTools.Stop();
    if (propertyInfo.ValueType == null)
      throw new XmlInternalException($"Collection type at property {propertyInfo.Member.Name}" +
                                     $" of type {obj.GetType().Name} unknown", reader);
    var collectionType = propertyInfo.MemberType;
    //if (collectionType == null)
    //  throw new XmlInternalException($"Unknown collection type for {propertyInfo.Member.DeclaringType}.{propertyInfo.Member.Name}", reader);

    if (reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
    var elementName = reader.Name;

    // all items will be read to this temporary list;
    List<object> tempList = new List<object>();

    var collection = propertyInfo.GetValue(obj);
    if (collection == null)
    { // Check if collection can be written - if not we will not be able to set the property
      if (!propertyInfo.CanWrite)
        throw new XmlInternalException($"Collection at property {propertyInfo.Member.Name}" +
                                       $" of type {obj.GetType().Name} is  but readonly", reader);
    }


    CollectionTypeKind? collectionTypeKind = null;
    Type? itemType;
    Type? valueType = typeof(object);
    Type? keyType = typeof(object);
    if (collectionType.IsArray)
    {
      itemType = collectionType.GetElementType();
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
      collectionTypeKind = CollectionTypeKind.Array;
    }
    else if (collectionType.IsDictionary(out keyType, out valueType))
    {
      if (keyType == null)
        throw new XmlInternalException($"Unknown key type of {collectionType} collection", reader);
      if (valueType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
      collectionTypeKind = CollectionTypeKind.Dictionary;
      itemType = valueType;

    }
    else if (collectionType.IsList(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
      collectionTypeKind = CollectionTypeKind.List;
    }
    else if (collectionType.IsCollection(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
      collectionTypeKind = CollectionTypeKind.Collection;
    }

    if (collectionTypeKind == null)
    {
      throw new XmlInternalException($"Invalid type kind of {collectionType} collection", reader);
    }

    if (itemType == null)
      throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);

    if (collectionTypeKind == CollectionTypeKind.Dictionary)
    {
      var dictionaryInfo = propertyInfo.CollectionInfo as DictionaryInfo;
      if (dictionaryInfo == null)
        throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlDictionaryAttribute)} attribute", reader);
      if (reader.IsStartElement(propertyInfo.XmlName) && !reader.IsEmptyElement)
      {
        reader.Read();
        ReadDictionaryItems(tempList, keyType, valueType, dictionaryInfo, reader);
      }
    }
    else
    {
      if (collectionInfo == null)
        throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlCollectionAttribute)} attribute", reader);

      if (reader.IsStartElement(propertyInfo.XmlName) && !reader.IsEmptyElement)
      {
        reader.Read();
        ReadCollectionItems(tempList, itemType, collectionInfo, reader);
      }
    }

    if (collection == null)
    {
      switch (collectionTypeKind)
      {
        case CollectionTypeKind.Array:
          var arrayObject = Array.CreateInstance(itemType, tempList.Count);
          for (int i = 0; i < tempList.Count; i++)
            arrayObject.SetValue(tempList[i], i);
          propertyInfo.SetValue(obj, arrayObject);
          break;

        case CollectionTypeKind.Collection:
          // We can't use non-generic ICollection interface because implementation of ICollection<T>
          // does implicate implementation of ICollection.
          object? newCollectionObject;
          if (collectionType.IsConstructedGenericType)
          {
            Type d1 = typeof(Collection<>);
            Type[] typeArgs = { itemType };
            Type newListType = d1.MakeGenericType(typeArgs);
            newCollectionObject = Activator.CreateInstance(newListType);
          }
          else
          {
            var constructor = collectionType.GetConstructor(new Type[0]);
            if (constructor == null)
              throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", reader);
            newCollectionObject = constructor.Invoke(new object[0]);
          }
          if (newCollectionObject == null)
            throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", reader);

          // ICollection has no Add method so we must localize this method using reflection.
          var addMethod = newCollectionObject.GetType().GetMethod("Add");
          if (addMethod == null)
            throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", reader);
          for (int i = 0; i < tempList.Count; i++)
            addMethod.Invoke(newCollectionObject, new object[] { tempList[i] });
          propertyInfo.SetValue(obj, newCollectionObject);
          break;

        case CollectionTypeKind.List:
          IList? newListObject;
          if (collectionType.IsConstructedGenericType)
          {
            Type d1 = typeof(List<>);
            Type[] typeArgs = { itemType };
            Type newListType = d1.MakeGenericType(typeArgs);
            newListObject = Activator.CreateInstance(newListType) as IList;
          }
          else
          {
            var constructor = collectionType.GetConstructor(new Type[0]);
            if (constructor == null)
              throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", reader);
            newListObject = constructor.Invoke(new object[0]) as IList;
          }
          if (newListObject == null)
            throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", reader);
          for (int i = 0; i < tempList.Count; i++)
            newListObject.Add(tempList[i]);
          propertyInfo.SetValue(obj, newListObject);
          break;

        case CollectionTypeKind.Dictionary:
          IDictionary? newDictionaryObject;
          if (collectionType.IsConstructedGenericType)
          {
            Type d1 = typeof(Dictionary<,>);
            Type[] typeArgs = { keyType, valueType };
            Type newListType = d1.MakeGenericType(typeArgs);
            newDictionaryObject = Activator.CreateInstance(newListType) as IDictionary;
          }
          else
          {
            var constructor = collectionType.GetConstructor(new Type[0]);
            if (constructor == null)
              throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", reader);
            newDictionaryObject = constructor.Invoke(new object[0]) as IDictionary;
          }
          if (newDictionaryObject == null)
            throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", reader);
          foreach (var item in tempList)
          {
            var kvPair = (KeyValuePair<object, object?>)item;
            newDictionaryObject.Add(kvPair.Key, kvPair.Value);
          }
          propertyInfo.SetValue(obj, newDictionaryObject);
          break;

        default:
          throw new XmlInternalException($"Collection type {collectionType} is not implemented for creation", reader);
      }
    }
    else
    {
      switch (collectionTypeKind)
      {
        case CollectionTypeKind.Array:
          var arrayObject = collection as Array;
          if (arrayObject == null)
            throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to Array", reader);
          if (arrayObject.Length == tempList.Count)
            for (int i = 0; i < tempList.Count; i++)
              arrayObject.SetValue(tempList[i], i);
          else
          if (!propertyInfo.CanWrite)
            throw new XmlInternalException($"Collection at property {propertyInfo.Member.Name}" +
                                           $" is an array of different length than number of read items but is readonly and can't be changed", reader);
          var itemArray = Array.CreateInstance(itemType, tempList.Count);
          for (int i = 0; i < tempList.Count; i++)
            itemArray.SetValue(tempList[i], i);
          propertyInfo.SetValue(obj, itemArray);
          break;

        case CollectionTypeKind.Collection:
          // We can't cast a collection to non-generic ICollection because implementation of ICollection<T>
          // does implicate implementation of ICollection.
          object? collectionObject = collection;
          // ICollection has no Add method so we must localize this method using reflection.
          var addMethod = collectionObject.GetType().GetMethod("Add");
          if (addMethod == null)
            throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", reader);
          // We must do the same with Clear method.
          var clearMethod = collectionObject.GetType().GetMethod("Clear");
          if (clearMethod == null)
            throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", reader);

          clearMethod.Invoke(collectionObject, new object[0]);
          for (int i = 0; i < tempList.Count; i++)
            addMethod.Invoke(collectionObject, new object[] { tempList[i] });
          break;

        case CollectionTypeKind.List:
          IList? listObject = collection as IList;
          if (listObject == null)
            throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to IList", reader);
          listObject.Clear();
          for (int i = 0; i < tempList.Count; i++)
            listObject.Add(tempList[i]);
          break;

        case CollectionTypeKind.Dictionary:
          IDictionary? dictionaryObject = collection as IDictionary;
          if (dictionaryObject == null)
            throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to IDictionary", reader);
          dictionaryObject.Clear();
          for (int i = 0; i < tempList.Count; i++)
          {
            KVPair? kvPair = tempList[i] as KVPair;
            if (kvPair != null && kvPair.Key != null)
              dictionaryObject.Add(kvPair.Key, kvPair.Value);
          }
          break;

        default:
          throw new XmlInternalException($"Collection type {collectionType} is not implemented for set content", reader);
      }
    }
    reader.Read();
  }

  public int ReadCollectionItems(ICollection<object> collection, Type collectionItemType, CollectionInfo collectionInfo, XmlReader reader)
  {
    int itemsRead = 0;
    while (reader.NodeType == XmlNodeType.Whitespace)
      reader.Read();
    while (reader.NodeType == XmlNodeType.Element)
    {
      var item = ReadElementAsItem(collectionItemType, collectionInfo, reader);
      if (item != null)
        collection.Add(item);
      SkipWhitespaces(reader);
    }
    return itemsRead;
  }

  public int ReadDictionaryItems(ICollection<object> collection, Type collectionKeyType, Type collectionValueType, DictionaryInfo dictionaryInfo, XmlReader reader)
  {
    int itemsRead = 0;
    while (reader.NodeType == XmlNodeType.Element)
    {
      (object? key, object? val) = ReadElementAsKVPair(collectionKeyType, collectionValueType, dictionaryInfo, reader);
      if (key != null)
        collection.Add(new KeyValuePair<object, object?>(key, val));
      SkipWhitespaces(reader);
      itemsRead++;
    }
    return itemsRead;
  }

  public void ReadTextMemberValue(object obj, XmlReader reader)
  {
    if (reader.NodeType == XmlNodeType.Text)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var typeInfo))
        throw new XmlInternalException($"Unknown type {aType.Name} on deserialize", reader);
      var textMemberInfo = typeInfo.TextProperty;
      if (textMemberInfo == null)
        throw new XmlInternalException($"Unknown text property in {aType.Name} on deserialize", reader);
      var value = ReadValue(textMemberInfo, reader);
      textMemberInfo.SetValue(obj, value);
    }
  }


  public object? ReadValue(SerializationMemberInfo serializationMemberInfo, XmlReader reader)
  {
    var propType = serializationMemberInfo.MemberType;
    if (propType==null)
      return null;
    var typeConverter = serializationMemberInfo.GetTypeConverter();
    return ReadValue(propType, typeConverter, reader);
  }

  public object? ReadValue(Type expectedType, TypeConverter? typeConverter, XmlReader reader)
  {
    var str = reader.ReadContentAsString();
    object? propValue;
    if (expectedType.Name.StartsWith("Nullable`1"))
      expectedType = expectedType.GetGenericArguments()[0];

    if (expectedType == typeof(string))
      propValue = str;
    else
    if (str == "")
    {
      if (typeConverter != null)
      {
        propValue = typeConverter.ConvertFromString(str);
        return propValue;
      }
      return null;
    }
    else
    if (expectedType == typeof(char))
    {
      if (str.Length > 0)
        propValue = str[0];
      else
        propValue = '\0';
    }
    else
    if (expectedType == typeof(bool))
    {
      switch (str.ToLower())
      {
        case "true":
        case "yes":
        case "on":
        case "t":
        case "1":
          propValue = true;
          break;
        case "false":
        case "no":
        case "off":
        case "f":
        case "0":
          propValue = false;
          break;
        default:
          if (typeConverter != null)
          {
            propValue = typeConverter.ConvertFromString(str);
            break;
          }
          throw new XmlInternalException($"Cannot convert \"{str}\" to boolean value", reader);
      }
    }
    else if (expectedType.IsEnum)
    {
      if (!Enum.TryParse(expectedType, str, Options.IgnoreCaseOnEnum, out propValue))
        throw new XmlInternalException($"Cannot convert \"{str}\" to enum value of type {expectedType.Name}", reader);
    }
    else if (expectedType == typeof(int))
    {
      if (!int.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to int value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(uint))
    {
      if (!uint.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to uint value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(long))
    {
      if (!long.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to int64 value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(ulong))
    {
      if (!ulong.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to uint64 value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(byte))
    {
      if (!byte.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to byte value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(sbyte))
    {
      if (!sbyte.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to signed byte value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(short))
    {
      if (!short.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to int16 value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(ushort))
    {
      if (!ushort.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to uint16 value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(float))
    {
      if (!float.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to float value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(double))
    {
      if (!double.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to double value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(decimal))
    {
      if (!decimal.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to decimal value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(DateTime))
    {
      if (!DateTime.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to date/time value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(TimeSpan))
    {
      if (!TimeSpan.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to time span value", reader);
      propValue = val;
    }
    else if (expectedType == typeof(object))
    {
      propValue = str;
    }
    else if (expectedType.IsArray)
    {
      if (typeConverter != null)
        propValue = typeConverter.ConvertFrom(str);
      else
        throw new XmlInternalException($"Array type converter not supported", reader);
    }
    else
      throw new XmlInternalException($"Value type \"{expectedType}\" not supported for deserialization", reader);
    return propValue;
  }
  #endregion

}