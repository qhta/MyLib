using System.Net;
using Qhta.Conversion;

namespace Qhta.Xml.Serialization;

public delegate void OnUnknownMember(object readObject, string elementName);

public class QXmlSerializationReader : IXmlConverterReader
{
  public QXmlSerializationReader(XmlReader xmlReader, QXmlSerializerSettings settings)
  {
    this.Reader = xmlReader;
    Mapper = settings.Mapper;
    Options = settings.Options;
    XmlWriterSettings = settings.XmlWriterSettings;

  }

  public XmlReader Reader { get; private set; }

  public XmlSerializationInfoMapper Mapper { get; private set; }

  public KnownTypesCollection KnownTypes => Mapper.KnownTypes;

  public string? DefaultNamespace => Mapper.DefaultNamespace;

  public SerializationOptions Options { get; private set; }

  public XmlWriterSettings XmlWriterSettings { get; private set; }

  public XmlSerializerNamespaces Namespaces { get; private set; } = new();

  #region Deserialize methods

  public object? ReadObject()
  {
    SkipToElement();
    if (Reader.EOF)
      return null;
    var name = Reader.Name;
    var prefix = Reader.Prefix;
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
        throw new XmlInternalException($"Element {xmlName} not recognized while deserialization.", Reader);
    }
    var constructor = typeInfo.KnownConstructor;
    if (constructor == null)
      throw new XmlInternalException($"Type {typeInfo.Type.Name} must have a public, parameterless constructor.", Reader);
    var obj = constructor.Invoke(new object[0]);

    //if (obj is IXSerializable qSerializable)
    //{
    //  qSerializable.Deserialize(this);
    //}
    //else 
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(Reader);
    }
    else
    {
      ReadObject(obj, typeInfo);
    }
    return obj;
  }

  #endregion

  #region Read methods

  public void SkipToElement()
  {
    while (!Reader.EOF && Reader.NodeType != XmlNodeType.Element)
      Reader.Read();
  }

  public void SkipWhitespaces()
  {
    while (Reader.NodeType == XmlNodeType.Whitespace)
      Reader.Read();
  }

  public object? ReadObject(object context, SerializationTypeInfo typeInfo)
  {
    return ReadObject(context, typeInfo, null);
  }

  public object? ReadObject(object context, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember)
  {
    object? result = null;
    if (Reader.NodeType != XmlNodeType.EndElement)
    {
      if (Reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
      bool isEmptyElement = Reader.IsEmptyElement;
      ReadAttributesBase(context, typeInfo, onUnknownMember);
      Reader.Read(); // read end of start element and go to its content;
      SkipWhitespaces();
      if (isEmptyElement)
        return null;
      ReadElementsBase(context, typeInfo);
      result = ReadTextMemberValue(context, typeInfo.TextProperty);
    }
    if (Reader.NodeType == XmlNodeType.EndElement)
      Reader.Read();
    return result;
  }

  public object? ReadMemberObject(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo, OnUnknownMember? onUnknownMember = null)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    bool isEmptyElement = Reader.IsEmptyElement;
    ReadAttributesBase(context, typeInfo, onUnknownMember);
    Reader.Read(); // read end of start element and go to its content;
    SkipWhitespaces();
    if (isEmptyElement)
      return null;
    ReadElementsBase(context, typeInfo);
    var result = ReadTextMemberValue(context, memberInfo);
    if (Reader.NodeType == XmlNodeType.EndElement)
      Reader.Read();
    return result;
  }

  public int ReadAttributesBase(object context, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
    var aType = context.GetType();
    var attribs = typeInfo.MembersAsAttributes;
    var props = typeInfo.MembersAsElements;
    //foreach (var prop in props)
    //  if (!attribs.ContainsKey(prop.QualifiedName))
    //    attribs.Add(prop.QualifiedName, prop);
    var propList = attribs;

    int attrsRead = 0;
    Reader.MoveToFirstAttribute();
    var attrCount = Reader.AttributeCount;
    for (int i = 0; i < attrCount; i++)
    {
      string attrPrefix = Reader.Prefix;
      string attrName = Reader.LocalName;
      if (attrName == "xmlns" && attrPrefix == "")
      {
        attrPrefix = "xmlns";
        attrName = "";
      }
      if (!String.IsNullOrEmpty(attrPrefix))
      {
        //TestTools.Stop();
        switch (attrPrefix)
        {
          case "xml":
            if (attrName == "space")
            {
              Reader.ReadAttributeValue();
              var str = Reader.ReadContentAsString();
              if (Reader is XmlTextReader xmlTextReader)
              {
                if (str == "preserve")
                  xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
                else
                  xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
              }
            }
            Reader.MoveToNextAttribute();
            continue;
          case "xmlns":
            Reader.ReadAttributeValue();
            var ns = Reader.ReadContentAsString();
            Namespaces.Add(attrName, ns);
            Reader.MoveToNextAttribute();
            continue;
          case "xsd":
            Reader.MoveToNextAttribute();
            continue;
        }
      }

      var propertyToRead = propList.FirstOrDefault(item => item.XmlName == attrName);
      if (propertyToRead == null)
      {
        if (onUnknownMember == null)
          throw new XmlInternalException($"Unknown property for attribute \"{attrName}\" in type {aType.Name}", Reader);
        onUnknownMember(context, attrName);
      }
      else
      {
        Reader.ReadAttributeValue();
        object? propValue = ReadValue(context, propertyToRead);
        if (propValue != null)
        {
          try
          {
            propertyToRead.SetValue(context, propValue);
          }
          catch (Exception ex)
          {
            throw new XmlInternalException($"Could not set value for property {propertyToRead.Member} in type {aType.Name}", Reader, ex);
          }

          attrsRead++;
        }
      }

      Reader.MoveToNextAttribute();
    }
    return attrsRead;
  }

  public int ReadElementsBase(object context, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
    var aType = context.GetType();
    var attribs = typeInfo.MembersAsAttributes;
    var props = typeInfo.MembersAsElements;
    //foreach (var prop in attribs)
    //  if (!props.ContainsKey(prop.QualifiedName))
    //    props.Add(prop.QualifiedName, prop);
    var propList = props;


    int propsRead = 0;
    while (Reader.NodeType == XmlNodeType.Element)
    {
      var elementPrefix = Reader.Prefix;
      var elementName = Reader.LocalName;
      bool isEmptyElement = Reader.IsEmptyElement;
      //if (elementName == "null")
      //  TestTools.Stop();
      var propertyToRead = propList.FirstOrDefault(item => item.XmlName == elementName);
      if (propertyToRead != null)
      {
        object? propValue = null;
        Type propType = propertyToRead.MemberType ?? typeof(Object);
        if (propertyToRead.XmlConverter?.CanRead == true)
        {
          propValue = propertyToRead.XmlConverter.ReadXml(context, this, typeInfo, propertyToRead, null);
        }
        else
        {
          if (propType.IsClass && propType != typeof(string))
          {
            if (propertyToRead.ContentInfo != null)
              ReadElementAsCollectionMember(context, propertyToRead, propertyToRead.ContentInfo);
            else
            {
              propValue = ReadElementAsMember(context, propertyToRead);
            }
          }
          else

            propValue = ReadElementAsMember(context, propertyToRead);
        }
        if (propValue != null)
        {
          propertyToRead.SetValue(context, propValue);
          propsRead++;
        }
      }
      else
      {
        if (typeInfo.ContentProperty != null)
        {
          var content = ReadElementAsContent(context, typeInfo.ContentProperty, typeInfo);
          if (content != null)
          {
            typeInfo.ContentProperty.SetValue(context, content);
            propsRead++;
            continue;
          }
        }
        if (typeInfo.ContentInfo?.KnownItemTypes != null)
        {
          if (!typeInfo.ContentInfo.KnownItemTypes.TryGetValue(elementName, out var knownItemTypeInfo))
          {
            knownItemTypeInfo = (typeInfo.ContentInfo.KnownItemTypes as IEnumerable<SerializationItemInfo>).FirstOrDefault();
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
            //if (elementName == "null")
            //  TestTools.Stop();
            object? item;
            if (knownItemTypeInfo.DictionaryInfo != null)
            {
              var collectionInfo = typeInfo.ContentInfo;
              if (collectionInfo is DictionaryInfo dictionaryInfo)
              {
                (key, item) = ReadElementAsKVPair(context, null, knownItemTypeInfo.Type, dictionaryInfo);
              }
              else
                throw new InternalException($"TypeInfo({typeInfo.Type}).CollectionInfo must be a DictionaryInfo");
            }
            else
              item = ReadElementWithKnownTypeInfo(context, knownItemTypeInfo);

            //if (item is decimal)
            //  TestTools.Stop();

            if (item == null)
              throw new XmlInternalException($"Item of type {knownItemTypeInfo.Type} could not be read at \"{elementName}\"", Reader);
            if (item.GetType() == typeof(object))
              item = null;
            if (key == null)
              throw new InternalException($"Key for {item} must be not null");

            if (knownItemTypeInfo.AddMethod != null)
            {
              if (context is IDictionary dictionaryObj)
                knownItemTypeInfo.AddMethod.Invoke(context, new object?[] { key, item });
              else
                knownItemTypeInfo.AddMethod.Invoke(context, new object?[] { item });
            }
            else
            {
              if (context is IDictionary dictionaryObj)
                dictionaryObj.Add(key, item);
              else
              {
                var adddMethod = context.GetType().GetMethod("Add", new Type[] { knownItemTypeInfo.Type });
                if (adddMethod != null)
                  adddMethod.Invoke(context, new object?[] { item });
                else
                if (context is ICollection collectionObj)
                  throw new XmlInternalException($"Add method for {knownItemTypeInfo.Type} item not found in type {aType.FullName}", Reader);
              }
            }
            propsRead++;
            continue;
          }
        }
        if (onUnknownMember != null)
          onUnknownMember(context, elementName);
        else
        {
          if (Options.IgnoreUnknownElements)
          {
            if (Reader.IsEmptyElement)
              Reader.Read();
            else
            {
              do
              {
                Reader.Read();
              } while (!(Reader.Name == elementName && !Reader.IsStartElement()));
              Reader.Read();
            }
            continue;
          }
          throw new XmlInternalException(
            $"No property to read and no content property found for element \"{elementName}\" in type {aType.FullName}", Reader);
        }
      }

      SkipWhitespaces();
    }
    return propsRead;
  }

  public object? ReadElementAsObject(object context, Type? expectedType)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var name = Reader.Name;
    if (!KnownTypes.TryGetValue(name, out var typeInfo))
    {
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", Reader);
    }
    if (expectedType != null)
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    return ReadElementWithKnownTypeInfo(context, typeInfo);
  }

  public object? ReadElementAsContent(object context, SerializationMemberInfo memberInfo, SerializationTypeInfo objectTypeInfo)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var name = Reader.Name;
    if (objectTypeInfo.ContentInfo == null)
      return null;
    SerializationTypeInfo? typeInfo = null;
    if (objectTypeInfo.ContentInfo.KnownItemTypes.TryGetValue(name, out var typeItemInfo))
      typeInfo = typeItemInfo.TypeInfo;
    else
    {
      typeInfo = memberInfo.ValueType;
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", Reader);
    }
    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType != null)
    {
      if (expectedType.IsNullable(out var baseType))
        expectedType = baseType;
      if (expectedType == null)
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but expected type is null", Reader);
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    }

    return ReadMemberWithKnownTypeInfo(context, typeInfo, memberInfo);
  }

  public object? ReadElementAsMember(object context, SerializationMemberInfo memberInfo)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var name = Reader.Name;
    if (!KnownTypes.TryGetValue(name, out var typeInfo))
    {
      typeInfo = memberInfo.ValueType;
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", Reader);
    }
    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType != null)
    {
      if (expectedType.IsNullable(out var baseType))
        expectedType = baseType;
      if (expectedType == null)
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but expected type is null", Reader);
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    }

    return ReadMemberWithKnownTypeInfo(context, typeInfo, memberInfo);
  }

  public object? ReadMemberWithKnownTypeInfo(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo)
  {
    if (typeInfo.XmlConverter?.CanRead == true)
      return typeInfo.XmlConverter.ReadXml(context, this, typeInfo, null, null);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType == typeof(byte))
      {
        Reader.Read();
        var str = Reader.ReadContentAsString();
        var result = ConvertMemberValueFromString(context, memberInfo, str);
        Reader.Read();
        return result;
      }
      if (typeInfo.Type.IsArray)
      {
        Reader.Read();
        return null;
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
    }
    var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(Reader);
    }
    //else 
    return ReadMemberObject(context, typeInfo, memberInfo);
    //return obj;
  }

  protected object? ConvertMemberValueFromString(object context, SerializationMemberInfo memberInfo, string? str)
  {
    if (str == null)
      return null;
    if (memberInfo.Property == null)
      return null;
    var typeConverter = memberInfo.GetTypeConverter();
    if (typeConverter != null)
    {
      var result = typeConverter.ConvertFromInvariantString(new TypeDescriptorContext(context, memberInfo.Property), str);
      return result;
    }
    return str;
  }

  public object? ReadElementAsItem(object context, Type? expectedType, ContentItemInfo collectionInfo)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var name = Reader.Name;
    //if (name == "HeadingPair")
    //  TestTools.Stop();
    SerializationTypeInfo? typeInfo = null;
    if (collectionInfo.KnownItemTypes.Any())
    {
      if (collectionInfo.KnownItemTypes.TryGetValue(name, out var typeItemInfo))
        typeInfo = typeItemInfo.TypeInfo;
    }
    else
      KnownTypes.TryGetValue(name, out typeInfo);
    if (typeInfo == null)
      throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", Reader);
    if (expectedType != null)
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    return ReadElementWithKnownTypeInfo(context, typeInfo);
  }

  public object? ReadElementWithKnownTypeInfo(object context, SerializationTypeInfo typeInfo)
  {
    if (typeInfo.XmlConverter?.CanRead == true)
      return typeInfo.XmlConverter.ReadXml(context, this, typeInfo, null, null);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType==typeof(byte))
      {
        Reader.Read();
        var result = ReadValue(context, typeInfo.Type, null, null);
        Reader.Read();
        return result;
      }
      if (typeInfo.Type.IsArray)
      {
        Reader.Read();
        return null;
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
    }
    var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
    //if (obj is IXSerializable qSerializable)
    //{
    //  qSerializable.Deserialize(this);
    //}
    //else 
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(Reader);
    }
    else
      ReadObject(obj, typeInfo);
    return obj;
  }

  public object? ReadElementWithKnownTypeInfo(object context, SerializationItemInfo itemTypeInfo)
  {
    //if (Reader.LocalName == "null")
    //  TestTools.Stop();
    var typeInfo = itemTypeInfo.TypeInfo;
    if (typeInfo.XmlConverter?.CanRead == true)
      return typeInfo.XmlConverter.ReadXml(context, this, typeInfo, null, itemTypeInfo);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType == typeof(byte))
      {
        if (Reader.IsEmptyElement)
        {
          var result = Reader.LocalName;
          Reader.Read();
          if (result == "null")
            return new object();
          if (result == "DBNull")
            return DBNull.Value;
          return null;
        }
        else
        {
          Reader.Read();
          var result = ReadValue(context, typeInfo.Type, typeInfo.TypeConverter, null);
          Reader.Read();
          return result;
        }
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
    }
    var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
    //if (obj is IXSerializable qSerializable)
    //{
    //  qSerializable.Deserialize(this);
    //}
    //else 
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.ReadXml(Reader);
    }
    else
      ReadObject(obj, typeInfo);
    return obj;
  }


  public (object? key, object? value) ReadElementAsKVPair(object context, Type? expectedKeyType, Type? expectedValueType, DictionaryInfo dictionaryInfo)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var keyName = Reader.Name;
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
      throw new XmlInternalException($"Unknown type for element \"{keyName}\" on deserialize", Reader);
    var foundValueType = typeInfo.Type;
    if (expectedValueType != null)
      if (!foundValueType.IsEqualOrSubclassOf(expectedValueType))
      {
        if (!expectedValueType.IsCollection(foundValueType))
          throw new XmlInternalException($"Element \"{keyName}\" is mapped to {typeInfo.Type.Name}" +
                                         $" but {expectedValueType.Name} or its subclass expected", Reader);
      }
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple())
      {
        var emptyElement = Reader.IsEmptyElement;
        if (keyName == null)
          throw new XmlInternalException($"Key name unknown for dictionary item \"{Reader.Name}\" on deserialize", Reader);
        Reader.MoveToAttribute(keyName);
        var key = Reader.Value;
        if (emptyElement)
        {
          Reader.Read(); // read to end of start element
          return (key, null);
        }
        else
        {
          Reader.Read(); // read to end of start element
          var result = ReadValue(context, typeInfo.Type, null, null);
          Reader.Read();
          return (key, result);
        }
      }
      throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
    }
    KVPair kvPair = new();
    kvPair.Value = typeInfo.KnownConstructor.Invoke(new object[0]);
    ReadObject(kvPair.Value, typeInfo, (object readObject, string elementName) =>
    {
      if (Reader.NodeType == XmlNodeType.Attribute)
      {
        if (elementName == keyName)
        {
          var keyValue = ReadValue(context, dictionaryInfo.KeyTypeInfo?.Type ?? typeof(string), null, null);
          kvPair.Key = keyValue;
        }
        else
          throw new XmlInternalException($"Unknown attribute \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", Reader);
      }
      else
      if (Reader.NodeType == XmlNodeType.Element)
      {
        throw new XmlInternalException($"Unknown element \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", Reader);
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

  public void ReadElementAsCollectionMember(object context, SerializationMemberInfo propertyInfo, ContentItemInfo collectionInfo)
  {
    //if (propertyInfo.Member.Name == "HeadingPairs")
    //  TestTools.Stop();
    if (propertyInfo.ValueType == null)
      throw new XmlInternalException($"Collection type at property {propertyInfo.Member.Name}" +
                                     $" of type {context.GetType().Name} unknown", Reader);
    var collectionType = propertyInfo.MemberType;
    //if (collectionType == null)
    //  throw new XmlInternalException($"Unknown collection type for {propertyInfo.Member.DeclaringType}.{propertyInfo.Member.Name}", reader);

    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var elementName = Reader.Name;
    if (elementName == "decimal")
      TestTools.Stop();
    // all items will be read to this temporary list;
    List<object> tempList = new List<object>();

    var collection = propertyInfo.GetValue(context);
    if (collection == null)
    { // Check if collection can be written - if not we will not be able to set the property
      if (!propertyInfo.CanWrite)
        throw new XmlInternalException($"Collection at property {propertyInfo.Member.Name}" +
                                       $" of type {context.GetType().Name} is  but readonly", Reader);
    }


    CollectionTypeKind? collectionTypeKind = null;
    Type? itemType;
    Type? valueType = typeof(object);
    Type? keyType = typeof(object);
    if (collectionType == null)
      throw new XmlInternalException($"Collection type is null", Reader);
    if (collectionType.IsArray)
    {
      itemType = collectionType.GetElementType();
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", Reader);
      collectionTypeKind = CollectionTypeKind.Array;
    }
    else if (collectionType.IsDictionary(out keyType, out valueType))
    {
      if (keyType == null)
        throw new XmlInternalException($"Unknown key type of {collectionType} collection", Reader);
      if (valueType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", Reader);
      collectionTypeKind = CollectionTypeKind.Dictionary;
      itemType = valueType;

    }
    else if (collectionType.IsList(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", Reader);
      collectionTypeKind = CollectionTypeKind.List;
    }
    else if (collectionType.IsCollection(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", Reader);
      collectionTypeKind = CollectionTypeKind.Collection;
    }

    if (collectionTypeKind == null)
    {
      throw new XmlInternalException($"Invalid type kind of {collectionType} collection", Reader);
    }

    if (itemType == null)
      throw new XmlInternalException($"Unknown item type of {collectionType} collection", Reader);

    if (collectionTypeKind == CollectionTypeKind.Dictionary)
    {
      var dictionaryInfo = propertyInfo.ContentInfo as DictionaryInfo;
      if (dictionaryInfo == null)
        throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlDictionaryAttribute)} attribute", Reader);
      if (Reader.IsStartElement(propertyInfo.XmlName) && !Reader.IsEmptyElement)
      {
        Reader.Read();
        if (keyType == null)
          throw new XmlInternalException($"Unknown key type of {collectionType} collection", Reader);
        if (valueType == null)
          throw new XmlInternalException($"Unknown item type of {collectionType} collection", Reader);
        ReadDictionaryItems(context, tempList, keyType, valueType, dictionaryInfo);
      }
    }
    else
    {
      if (collectionInfo == null)
        throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlCollectionAttribute)} attribute", Reader);

      if (Reader.IsStartElement(propertyInfo.XmlName) && !Reader.IsEmptyElement)
      {
        Reader.Read();
        ReadCollectionItems(context, tempList, itemType, collectionInfo);
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
          propertyInfo.SetValue(context, arrayObject);
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
              throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", Reader);
            newCollectionObject = constructor.Invoke(new object[0]);
          }
          if (newCollectionObject == null)
            throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", Reader);

          // ICollection has no Add method so we must localize this method using reflection.
          var addMethod = newCollectionObject.GetType().GetMethod("Add");
          if (addMethod == null)
            throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", Reader);
          for (int i = 0; i < tempList.Count; i++)
            addMethod.Invoke(newCollectionObject, new object[] { tempList[i] });
          propertyInfo.SetValue(context, newCollectionObject);
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
              throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", Reader);
            newListObject = constructor.Invoke(new object[0]) as IList;
          }
          if (newListObject == null)
            throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", Reader);
          for (int i = 0; i < tempList.Count; i++)
            newListObject.Add(tempList[i]);
          propertyInfo.SetValue(context, newListObject);
          break;

        case CollectionTypeKind.Dictionary:
          IDictionary? newDictionaryObject;
          if (collectionType.IsConstructedGenericType)
          {
            Type d1 = typeof(Dictionary<,>);
            if (keyType == null)
              throw new XmlInternalException($"Unknown key type of {collectionType} collection", Reader);
            if (valueType == null)
              throw new XmlInternalException($"Unknown item type of {collectionType} collection", Reader);
            Type[] typeArgs = { keyType, valueType };
            Type newListType = d1.MakeGenericType(typeArgs);
            newDictionaryObject = Activator.CreateInstance(newListType) as IDictionary;
          }
          else
          {
            var constructor = collectionType.GetConstructor(new Type[0]);
            if (constructor == null)
              throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", Reader);
            newDictionaryObject = constructor.Invoke(new object[0]) as IDictionary;
          }
          if (newDictionaryObject == null)
            throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", Reader);
          foreach (var item in tempList)
          {
            var kvPair = (KeyValuePair<object, object?>)item;
            newDictionaryObject.Add(kvPair.Key, kvPair.Value);
          }
          propertyInfo.SetValue(context, newDictionaryObject);
          break;

        default:
          throw new XmlInternalException($"Collection type {collectionType} is not implemented for creation", Reader);
      }
    }
    else
    {
      switch (collectionTypeKind)
      {
        case CollectionTypeKind.Array:
          var arrayObject = collection as Array;
          if (arrayObject == null)
            throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to Array", Reader);
          if (arrayObject.Length == tempList.Count)
            for (int i = 0; i < tempList.Count; i++)
              arrayObject.SetValue(tempList[i], i);
          else
          if (!propertyInfo.CanWrite)
            throw new XmlInternalException($"Collection at property {propertyInfo.Member.Name}" +
                                           $" is an array of different length than number of read items but is readonly and can't be changed", Reader);
          var itemArray = Array.CreateInstance(itemType, tempList.Count);
          for (int i = 0; i < tempList.Count; i++)
            itemArray.SetValue(tempList[i], i);
          propertyInfo.SetValue(context, itemArray);
          break;

        case CollectionTypeKind.Collection:
          // We can't cast a collection to non-generic ICollection because implementation of ICollection<T>
          // does implicate implementation of ICollection.
          object? collectionObject = collection;
          // ICollection has no Add method so we must localize this method using reflection.
          var addMethod = collectionObject.GetType().GetMethod("Add");
          if (addMethod == null)
            throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", Reader);
          // We must do the same with Clear method.
          var clearMethod = collectionObject.GetType().GetMethod("Clear");
          if (clearMethod == null)
            throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", Reader);

          clearMethod.Invoke(collectionObject, new object[0]);
          for (int i = 0; i < tempList.Count; i++)
            addMethod.Invoke(collectionObject, new object[] { tempList[i] });
          break;

        case CollectionTypeKind.List:
          IList? listObject = collection as IList;
          if (listObject == null)
            throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to IList", Reader);
          listObject.Clear();
          for (int i = 0; i < tempList.Count; i++)
            listObject.Add(tempList[i]);
          break;

        case CollectionTypeKind.Dictionary:
          IDictionary? dictionaryObject = collection as IDictionary;
          if (dictionaryObject == null)
            throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to IDictionary", Reader);
          dictionaryObject.Clear();
          for (int i = 0; i < tempList.Count; i++)
          {
            KVPair? kvPair = tempList[i] as KVPair;
            if (kvPair != null && kvPair.Key != null)
              dictionaryObject.Add(kvPair.Key, kvPair.Value);
          }
          break;

        default:
          throw new XmlInternalException($"Collection type {collectionType} is not implemented for set content", Reader);
      }
    }
    Reader.Read();
  }

  public int ReadCollectionItems(object context, ICollection<object> collection, Type collectionItemType, ContentItemInfo collectionInfo)
  {
    int itemsRead = 0;
    while (Reader.NodeType == XmlNodeType.Whitespace)
      Reader.Read();
    while (Reader.NodeType == XmlNodeType.Element)
    {
      var item = ReadElementAsItem(context, collectionItemType, collectionInfo);
      if (item != null)
        collection.Add(item);
      SkipWhitespaces();
    }
    return itemsRead;
  }

  public int ReadDictionaryItems(object context, ICollection<object> collection, Type collectionKeyType, Type collectionValueType, DictionaryInfo dictionaryInfo)
  {
    int itemsRead = 0;
    while (Reader.NodeType == XmlNodeType.Element)
    {
      (object? key, object? val) = ReadElementAsKVPair(context, collectionKeyType, collectionValueType, dictionaryInfo);
      if (key != null)
        collection.Add(new KeyValuePair<object, object?>(key, val));
      SkipWhitespaces();
      itemsRead++;
    }
    return itemsRead;
  }

  public object? ReadTextMemberValue(object context, SerializationMemberInfo? memberInfo)
  {
    if (Reader.NodeType == XmlNodeType.Text)
    {
      //var aType = obj.GetType();
      //if (!KnownTypes.TryGetValue(aType, out var typeInfo))
      //  throw new XmlInternalException($"Unknown type {aType.Name} on deserialize", reader);
      //memberInfo = typeInfo.TextProperty;
      if (memberInfo != null)
      {
        var value = ReadValue(context, memberInfo);
        return value;
      }
      else
      {
        var str = Reader.ReadString();
        if (str != null)
          str = str.DecodeStringValue();
        return str;
      }
    }
    return null;
    //throw new XmlInternalException($"Text node expected on deserialize", reader);
  }


  public object? ReadValue(object context, SerializationMemberInfo serializationMemberInfo)
  {
    var propType = serializationMemberInfo.MemberType;
    if (propType==null)
      return null;
    var typeConverter = serializationMemberInfo.GetTypeConverter();
    return ReadValue(context, propType, typeConverter, serializationMemberInfo);
  }

  public object? ReadValue(object context, Type expectedType, TypeConverter? typeConverter, SerializationMemberInfo? memberInfo)
  {
    if (Reader.NodeType == XmlNodeType.None)
      return null;
    var str = Reader.ReadContentAsString();
    str = str.DecodeStringValue();
    object? propValue = null;
    if (expectedType.Name.StartsWith("Nullable`1"))
      expectedType = expectedType.GetGenericArguments()[0];
    // insert typeconverter invocation
    if (expectedType == typeof(string))
      propValue = str;
    else
    if (typeConverter != null)
    {
      propValue = typeConverter.ConvertFromInvariantString(new TypeDescriptorContext(context, memberInfo?.Property), str);
      return propValue;
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
            propValue = typeConverter.ConvertFromInvariantString(new TypeDescriptorContext(context, memberInfo?.Property), str);
            break;
          }
          throw new XmlInternalException($"Cannot convert \"{str}\" to boolean value", Reader);
      }
    }
    else if (expectedType.IsEnum)
    {
      if (!Enum.TryParse(expectedType, str, Options.IgnoreCaseOnEnum, out propValue))
        throw new XmlInternalException($"Cannot convert \"{str}\" to enum value of type {expectedType.Name}", Reader);
    }
    else if (expectedType == typeof(int))
    {
      if (!int.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to int value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(uint))
    {
      if (!uint.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to uint value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(long))
    {
      if (!long.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to int64 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(ulong))
    {
      if (!ulong.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to uint64 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(byte))
    {
      if (!byte.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to byte value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(sbyte))
    {
      if (!sbyte.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to signed byte value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(short))
    {
      if (!short.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to int16 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(ushort))
    {
      if (!ushort.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to uint16 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(float))
    {
      if (!float.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to float value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(double))
    {
      if (!double.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to double value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(decimal))
    {
      if (!decimal.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to decimal value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(DateTime))
    {
      if (!DateTime.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to date/time value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(DateOnly))
    {
      if (!DateOnly.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to date-only value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(TimeOnly))
    {
      if (!TimeOnly.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to time-only value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(TimeSpan))
    {
      if (!TimeSpan.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to time span value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(Guid))
    {
      if (!Guid.TryParse(str, out var val))
        throw new XmlInternalException($"Cannot convert \"{str}\" to guid value", Reader);
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
        throw new XmlInternalException($"Array type converter not supported", Reader);
    }
    //else
    //  throw new XmlInternalException($"Value type \"{expectedType}\" not supported for deserialization", reader);
    return propValue;
  }
  #endregion

}