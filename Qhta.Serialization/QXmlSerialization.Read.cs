//#define TraceReader

using System;
using System.Linq;
using System.Reflection;

using Qhta.Conversion;
using Qhta.TypeUtils;

using static Qhta.Xml.Serialization.QXmlSerializer;

namespace Qhta.Xml.Serialization;

public delegate void OnUnknownMember(object readObject, string elementName);

public partial class QXmlSerializer : IXmlConverterReader
{
  public XmlReaderSettings XmlReaderSettings { get; } = new() { IgnoreWhitespace = true };

  public XmlSerializerNamespaces Namespaces { get; private set; } = new();

  XmlReader IXmlConverterReader.Reader => Reader;

  public QXmlReader Reader { get; private set; } = null!;

  #region Mapping methods
  public bool TryGetTypeInfo(Type type, [NotNullWhen(true)] out SerializationTypeInfo? typeInfo)
  {
    return Mapper.KnownTypes.TryGetValue(type, out typeInfo);
  }

  public bool TryGetTypeInfo(XmlQualifiedTagName tag, [NotNullWhen(true)] out SerializationTypeInfo? typeInfo)
  {
    //if (tag.Name=="sbyte")
    //  TestTools.Stop();
    if (String.IsNullOrEmpty(tag.Namespace))
    {
      var aType = TypeNaming.GetType(tag.Name);
      if (aType != null)
      {
        if (Mapper.KnownTypes.TryGetValue(aType, out typeInfo))
          return true;
      }
    }
    var clrName = Mapper.ToQualifiedName(tag);
    var ns = clrName.Namespace;
    if (Mapper.KnownTypes.TryGetValue(clrName, out typeInfo))
      return true;
    while (String.IsNullOrEmpty(tag.Prefix) && !String.IsNullOrEmpty(ns))
    {
      var k = ns.LastIndexOf('.');
      if (k <= 0) break;
      ns = ns.Substring(0, k);
      clrName.Namespace = ns;
      if (Mapper.KnownTypes.TryGetValue(clrName, out typeInfo))
        return true;
    }
    if (String.IsNullOrEmpty(tag.Prefix))
    {
      clrName.Namespace = "System";
      if (Mapper.KnownTypes.TryGetValue(clrName, out typeInfo))
        return true;
    }
    if (Mapper.KnownTypes.TryGetValue(clrName.Name, out typeInfo))
      return true;
    return false;
  }
  #endregion

  #region Deserialize methods

  public object? ReadObject(object? context = null)
  {
    SkipToElement();
    if (Reader.EOF)
      return null;
    SerializationTypeInfo? typeInfo;

    var qualifiedName = Reader.Name;
    if (!TryGetTypeInfo(qualifiedName, out typeInfo))
      throw new XmlInternalException($"Element {qualifiedName} not recognized while deserialization.", Reader);
    if (typeInfo.XmlConverter != null)
      return typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, this);
    return ReadElementWithKnownTypeInfo(context, typeInfo);
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
#if TraceReader
    Trace.IndentSize = 2;
    Trace.WriteLine($"<ReadObject context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
    //if (Reader.LocalName == "SectionProperties")
    //  TestTools.Stop();
    if (Reader.NodeType != XmlNodeType.EndElement)
    {
      if (Reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
      bool isEmptyElement = Reader.IsEmptyElement;
      ReadPropertiesAsAttributes(context, typeInfo, onUnknownMember);
      Reader.Read(); // read end of start element and go to its content;
      SkipWhitespaces();
      if (!isEmptyElement)
      {
        var n = ReadPropertiesAsElements(context, typeInfo);
        if (n == 0)
        {
          var textMemberInfo = typeInfo.TextProperty ?? typeInfo.ContentProperty;
          if (textMemberInfo != null)
            ReadTextMember(context, textMemberInfo);
        }
      }
    }
    result = context;
    //if (Reader.NodeType == XmlNodeType.EndElement)
    if (Reader.IsEndElement(new XmlQualifiedTagName(typeInfo.XmlName, typeInfo.XmlNamespace)))
      Reader.Read();
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadObject>");
#endif
    return result;
  }

  public int ReadPropertiesAsAttributes(object context, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
    //#if TraceReader
    //    Trace.WriteLine($"<ReadPropertiesAsAttributes context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    //    Trace.IndentLevel++;
    //#endif
    var aType = context.GetType();
    var attribs = typeInfo.MembersAsAttributes;
    var props = typeInfo.MembersAsElements;
    var propList = attribs;

    int result = 0;
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
              var str = Reader.ReadString();
              if (str == "preserve")
                Reader.WhitespaceHandling = WhitespaceHandling.Significant;
              else
                Reader.WhitespaceHandling = WhitespaceHandling.None;
            }
            Reader.MoveToNextAttribute();
            continue;
          case "xmlns":
            Reader.ReadAttributeValue();
            var ns = Reader.ReadString();
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
            //if (propValue.GetType().Name == "VariantType")
            //  TestTools.Stop();
            propertyToRead.SetValue(context, propValue);
          }
          catch (Exception ex)
          {
            throw new XmlInternalException($"Could not set value for property {propertyToRead.Member} in type {aType.Name}", Reader, ex);
          }

          result++;
        }
      }

      Reader.MoveToNextAttribute();
    }
    //#if TraceReader
    //    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    //    Trace.IndentLevel--;
    //    Trace.WriteLine($"</ReadPropertiesAsAttributes>");
    //#endif
    return result;
  }

  public int ReadPropertiesAsElements(object context, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadPropertiesAsElements context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    var aType = context.GetType();
    var attribs = typeInfo.MembersAsAttributes;
    var props = typeInfo.MembersAsElements;
    var propList = props;

    int result = 0;
    while (Reader.NodeType == XmlNodeType.Element)
    {

      var qualifiedName = Reader.Name;
      bool isEmptyElement = Reader.IsEmptyElement;
      var memberInfo = propList.FirstOrDefault(item => item.XmlName == qualifiedName.Name);
      if (memberInfo != null)
      {
        object? propValue = null;
        Type propType = memberInfo.MemberType ?? typeof(Object);
        if (memberInfo.XmlConverter?.CanRead == true)
        {
          propValue = memberInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, this);
        }
        else
        {
          var contentInfo = memberInfo.ContentInfo ?? memberInfo.ValueType?.ContentInfo ?? typeInfo.ContentInfo;
          if (propType.IsClass && propType != typeof(string))
          {
            if (memberInfo.IsObject)
              propValue = ReadElementAsMember(context, memberInfo);
            else
            if (contentInfo != null)
            {
              if (memberInfo.IsCollection)
              {
                propValue = ReadMemberCollection(context, memberInfo, contentInfo);
              }
              else
                propValue = null;
             if (propValue == null)
                propValue = ReadElementAsMember(context, memberInfo);
            }
            else
              propValue = ReadElementAsMember(context, memberInfo);
          }
          else
            propValue = ReadElementAsMember(context, memberInfo);
        }
        if (propValue != null)
        {
          SetValue(context, memberInfo, propValue);
          result++;
        }
      }
      else
      {
        if (typeInfo.ContentProperty != null)
        {
          var content = ReadElementAsContent(context, typeInfo.ContentProperty, typeInfo);
          if (content != null)
          {
            SetValue(context, typeInfo.ContentProperty, content);
            result++;
            break;
          }
        }
        if (typeInfo.ContentInfo?.KnownItemTypes != null)
        {
          if (!typeInfo.ContentInfo.KnownItemTypes.TryGetValue(qualifiedName.Name, out var knownItemTypeInfo))
          {
            if (TryGetTypeInfo(qualifiedName, out var itemTypeInfo))
              knownItemTypeInfo = new SerializationItemInfo(qualifiedName.Name, itemTypeInfo);
          }
          if (knownItemTypeInfo != null)
          {
            object? key = qualifiedName.Name;
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
              item = ReadElementWithKnownItemInfo(context, knownItemTypeInfo);

            if (item == null)
              throw new XmlInternalException($"Item of type {knownItemTypeInfo.Type} could not be read at \"{qualifiedName}\"", Reader);
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
                var collectionType = context.GetType();
                var adddMethod = collectionType.GetMethod("Add", new Type[] { knownItemTypeInfo.Type });
                if (adddMethod == null)
                  adddMethod = collectionType.GetMethod("Add");
                if (adddMethod != null)
                  adddMethod.Invoke(context, new object?[] { item });
                else if (context is ICollection collectionObj)
                  throw new XmlInternalException($"Add method for {knownItemTypeInfo.Type} item not found in type {aType.FullName}", Reader);
              }
            }
            result++;
            continue;
          }
        }
        if (TryGetTypeInfo(qualifiedName, out var newTypeInfo))
        {
          break;
        }
        if (onUnknownMember != null)
          onUnknownMember(context, qualifiedName.ToString());
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
              } while (!(qualifiedName == ReadElementTag() && !Reader.IsStartElement()));
              Reader.Read();
            }
            continue;
          }
          throw new XmlInternalException(
            $"No property to read and no type found for element \"{qualifiedName}\" in type {aType.FullName}", Reader);
        }
      }

      SkipWhitespaces();
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadPropertiesAsElements>");
#endif
    return result;
  }

  public object? ReadElementAsContent(object context, SerializationMemberInfo memberInfo, SerializationTypeInfo objectTypeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsContent context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
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

    var result = ReadMemberWithKnownTypeAndMemberInfo(context, typeInfo, memberInfo);
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsContent>");
#endif
    return result;
  }

  public object? ReadElementAsMember(object context, SerializationMemberInfo memberInfo)
  {

    if (Reader.NodeType != XmlNodeType.Element)
      return null;
    //throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    if (Reader.LocalName == "FooterReferences")
      Debugger.Break();
#if TraceReader
    Trace.WriteLine($"<ReadElementAsMember context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (String.IsNullOrEmpty(Reader.NamespaceURI))
    {
      var propInfo = memberInfo.ValueType?.MembersAsElements.FirstOrDefault(memberItem => memberItem.XmlName == Reader.LocalName);
      if (propInfo != null)
        memberInfo = propInfo;
    }
    var qualifiedName = ReadElementTag();
    SerializationTypeInfo? typeInfo = null;
    if (memberInfo.ValueType?.Type != typeof(object))
      typeInfo = memberInfo.ValueType;
    else
    if (!TryGetTypeInfo(qualifiedName, out typeInfo))
      typeInfo = memberInfo.ValueType;
    if (typeInfo == null)
      throw new XmlInternalException($"Unknown type for element \"{qualifiedName}\" on deserialize", Reader);
    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType != null)
    {
      if (expectedType.IsNullable(out var baseType))
        expectedType = baseType;
      if (expectedType == null)
        throw new XmlInternalException($"Element \"{qualifiedName}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but expected type is null", Reader);
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{qualifiedName}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    }

    var result = ReadMemberWithKnownTypeAndMemberInfo(context, typeInfo, memberInfo);
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsMember>");
#endif
    return result;
  }

  public object? ReadMemberWithKnownTypeAndMemberInfo(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberWithKnownTypeAndMemberInfo context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
    if (typeInfo.XmlConverter?.CanRead == true)
      result = typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, null);
    else
    {
      if (typeInfo.Type == typeof(object))
      {
        result = ReadMemberObjectOfAnyType(context, typeInfo, memberInfo);
      }
      else if (typeInfo.KnownConstructor == null)
      {
        if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType == typeof(byte))
        {
          Reader.Read();
          var str = Reader.ReadString();
          result = ConvertMemberValueFromString(context, memberInfo, str);
          Reader.Read();
        }
        else if (typeInfo.Type.IsArray)
        {
          throw new XmlInternalException($"Reading array for type {typeInfo.Type.Name} not implemented", Reader);
        }
        else
        {
          result = ReadMemberObjectOfAnyType(context, typeInfo, memberInfo);
        }
      }
      else
      {
        result = typeInfo.KnownConstructor.Invoke(new object[0]);
        if (result is IXmlSerializable xmlSerializable)
          xmlSerializable.ReadXml(Reader);
        else
        {
          if (!memberInfo.IsContentElement)
            Reader.Read();
          ReadMemberObjectInterior(result, typeInfo, memberInfo);
          if (!memberInfo.IsContentElement)
            Reader.Read();
        }
      }
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberWithKnownTypeAndMemberInfo>");
#endif
    return result;
  }

  public object? ReadMemberObjectOfAnyType(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo,
    OnUnknownMember? onUnknownMember = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberObjectUnknownType context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
    if (Reader.IsEmptyElement)
    {
      Reader.Read();
    }
    else
    {
      Reader.Read();
      if (Reader.NodeType == XmlNodeType.EndElement)
      {
        result = string.Empty;
        Reader.Read();
      }
      else if (Reader.NodeType == XmlNodeType.Text)
      {
        var str = Reader.ReadString();
        result = ConvertMemberValueFromString(context, memberInfo, str);
        Reader.Read();
      }
      else
      {
        var qualifiedName = ReadElementTag();
        if (TryGetTypeInfo(qualifiedName, out var foundTypeInfo) && foundTypeInfo != null)
          typeInfo = foundTypeInfo;
        result = ReadObject();

        //ReadAttributesBase(context, typeInfo, onUnknownMember);
        //Reader.Read(); // read end of start element and go to its content;
        //SkipWhitespaces();
        //var n = ReadElementsBase(context, typeInfo);
        //if (n == 0)
        //  ReadTextMemberValue(context, memberInfo);
        if (Reader.NodeType == XmlNodeType.EndElement)
          Reader.Read();
      }
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberObjectUnknownType>");
#endif
    return result;
  }

  public void ReadMemberObjectInterior(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo,
    OnUnknownMember? onUnknownMember = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberObjectInterior context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    bool isEmptyElement = Reader.IsEmptyElement;
    //if (typeInfo.XmlNamespace == "DocumentModel.Vml" && typeInfo.XmlName == "ShapeDefaults")
    //  TestTools.Stop();
    ReadPropertiesAsAttributes(context, typeInfo, onUnknownMember);
    Reader.Read(); // read end of start element and go to its content;
    SkipWhitespaces();
    if (!isEmptyElement)
    {
      var n = ReadPropertiesAsElements(context, typeInfo);
      if (n == 0 && Reader.NodeType == XmlNodeType.Text)
        ReadTextMember(context, memberInfo);
      if (Reader.NodeType == XmlNodeType.EndElement)
        Reader.Read();
    }
#if TraceReader
    var result = "void";
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberObjectInterior>");
#endif
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
#if TraceReader
    Trace.WriteLine($"<ReadElementAsItem context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var elementTag = ReadElementTag();
    SerializationTypeInfo? typeInfo = null;
    if (collectionInfo.KnownItemTypes.Any())
    {
      if (collectionInfo.KnownItemTypes.TryGetValue(elementTag, out var typeItemInfo))
        typeInfo = typeItemInfo.TypeInfo;
    }
    if (typeInfo == null)
      TryGetTypeInfo(elementTag, out typeInfo);
    if (typeInfo == null)
      throw new XmlInternalException($"Unknown type for element \"{elementTag}\" on deserialize", Reader);
    if (expectedType != null)
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{elementTag}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    var result = ReadElementWithKnownTypeInfo(context, typeInfo);
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsItem>");
#endif
    return result;
  }

  public object? ReadElementWithKnownTypeInfo(object? context, SerializationTypeInfo typeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementWithKnownTypeInfo context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
    if (typeInfo.XmlConverter?.CanRead == true)
      return typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, null);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType == typeof(byte))
      {
        Reader.Read();
        result = ReadValue(context, typeInfo.Type, typeInfo.TypeConverter, null);
        Reader.Read();
      }
      else
      if (typeInfo.Type.IsArray)
      {
        Reader.Read();
      }
      else
        throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
    }
    else
    {
      result = typeInfo.KnownConstructor.Invoke(new object[0]);
      if (result is IXmlSerializable xmlSerializable)
        xmlSerializable.ReadXml(Reader);
      else
        ReadObject(result, typeInfo);
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementWithKnownTypeInfo>");
#endif
    return result;
  }

  public object? ReadElementWithKnownItemInfo(object context, SerializationItemInfo itemTypeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementWithKnownItemInfo context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
    //if (Reader.LocalName == "null")
    //  TestTools.Stop();
    var typeInfo = itemTypeInfo.TypeInfo;
    if (typeInfo.XmlConverter?.CanRead == true)
      return typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, null);
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type.IsArray(out var itemType) && itemType == typeof(byte)
          || typeInfo.Type.GetInterface("IConvertible") != null)
      {
        if (Reader.IsEmptyElement)
        {
          var name = Reader.LocalName;
          Reader.Read();
          if (name == "null")
            result = new object();
          else if (name == "DBNull")
            result = DBNull.Value;
        }
        else
        {
          Reader.Read();
          result = ReadValue(context, typeInfo.Type, typeInfo.TypeConverter, null);
          Reader.Read();
        }
      }
      else
        throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
    }
    else
    {
      result = typeInfo.KnownConstructor.Invoke(new object[0]);
      if (result is IXmlSerializable xmlSerializable)
        xmlSerializable.ReadXml(Reader);
      else
        ReadObject(result, typeInfo);
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementWithKnownItemInfo>");
#endif
    return result;
  }


  public (object? key, object? value) ReadElementAsKVPair(object context, Type? expectedKeyType, Type? expectedValueType,
    DictionaryInfo dictionaryInfo)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var keyName = Reader.LocalName;
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
      else if (Reader.NodeType == XmlNodeType.Element)
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
    Dictionary
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

  public object? ReadMemberCollection(object context, SerializationMemberInfo propertyInfo, ContentItemInfo collectionInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberCollection context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (propertyInfo.ValueType == null)
      throw new XmlInternalException($"Collection type at property {propertyInfo.Member.Name}" +
                                     $" of type {context.GetType().Name} unknown", Reader);
    var collectionType = propertyInfo.MemberType;
    //if (collectionType == null)
    //  throw new XmlInternalException($"Unknown collection type for {propertyInfo.Member.DeclaringType}.{propertyInfo.Member.Name}", reader);

    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);



    List<object> tempList = new List<object>();

    var result = propertyInfo.GetValue(context);
    if (result == null)
    {
      // Check if collection can be written - if not we will not be able to set the property
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
        throw new XmlInternalException($"Unknown item type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.Array;
    }
    else if (collectionType.IsDictionary(out keyType, out valueType))
    {
      if (keyType == null)
        throw new XmlInternalException($"Unknown key type of {collectionType} result", Reader);
      if (valueType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.Dictionary;
      itemType = valueType;
    }
    else if (collectionType.Name.StartsWith("List`") && collectionType.IsList(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.List;
    }
    else if (collectionType.IsCollection(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.Collection;
    }

    if (collectionTypeKind == null)
      result = null;
    else
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} result", Reader);

      if (collectionTypeKind == CollectionTypeKind.Dictionary)
      {
        var dictionaryInfo = propertyInfo.ContentInfo as DictionaryInfo;
        if (dictionaryInfo == null)
          throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlDictionaryAttribute)} attribute", Reader);
        if (Reader.IsStartElement(propertyInfo.XmlName) && !Reader.IsEmptyElement)
        {
          Reader.Read();
          if (keyType == null)
            throw new XmlInternalException($"Unknown key type of {collectionType} result", Reader);
          if (valueType == null)
            throw new XmlInternalException($"Unknown item type of {collectionType} result", Reader);
          ReadDictionaryItems(context, tempList, keyType, valueType, dictionaryInfo);
        }
      }
      else
      {
        if (collectionInfo == null)
          throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlCollectionAttribute)} attribute", Reader);

        if (Reader.IsStartElement(new XmlQualifiedTagName(propertyInfo.XmlName, propertyInfo.XmlNamespace)) && !Reader.IsEmptyElement)
        {
          Reader.Read();
          if (propertyInfo.MemberType != null && Reader.IsStartElement(new XmlQualifiedTagName(propertyInfo.MemberType.Name, propertyInfo.MemberType.Namespace)))
            result = ReadMemberElementAsCollection(context, propertyInfo, collectionInfo);
          else
            ReadCollectionItems(context, tempList, itemType, collectionInfo);
        }
      }

      if (result == null)
      {
        switch (collectionTypeKind)
        {
          case CollectionTypeKind.Array:
            var arrayObject = Array.CreateInstance(itemType, tempList.Count);
            for (int i = 0; i < tempList.Count; i++)
              arrayObject.SetValue(tempList[i], i);
            //propertyInfo.SetValue(context, arrayObject);
            result = arrayObject;
            break;

          case CollectionTypeKind.Collection:
            // We can't use non-generic ICollection interface because implementation of ICollection<T>
            // does implicate implementation of ICollection.
            object? newCollectionObject;
            if (collectionType.IsConstructedGenericType)
            {
              //Type d1 = typeof(Collection<>);
              //Type[] typeArgs = { itemType };
              //Type newListType = d1.MakeGenericType(typeArgs);
              newCollectionObject = Activator.CreateInstance(collectionType);
            }
            else
            {
              var constructor = collectionType.GetConstructor(new Type[0]);
              if (constructor == null)
                throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", Reader);
              newCollectionObject = constructor.Invoke(new object[0]);
            }
            if (newCollectionObject == null)
              throw new XmlInternalException($"Could not create a new instance of {collectionType} result", Reader);

            // ICollection has no Add method so we must localize this method using reflection.
            var addMethod = newCollectionObject.GetType().GetMethod("Add", new Type[] { itemType });
            if (addMethod == null)
              throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} result", Reader);
            for (int i = 0; i < tempList.Count; i++)
              addMethod.Invoke(newCollectionObject, new object[] { tempList[i] });
            //propertyInfo.SetValue(context, newCollectionObject);
            result = newCollectionObject;
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
              throw new XmlInternalException($"Could not create a new instance of {collectionType} result", Reader);
            for (int i = 0; i < tempList.Count; i++)
              newListObject.Add(tempList[i]);
            //propertyInfo.SetValue(context, newListObject);
            result = newListObject;
            break;

          case CollectionTypeKind.Dictionary:
            IDictionary? newDictionaryObject;
            if (collectionType.IsConstructedGenericType)
            {
              Type d1 = typeof(Dictionary<,>);
              if (keyType == null)
                throw new XmlInternalException($"Unknown key type of {collectionType} result", Reader);
              if (valueType == null)
                throw new XmlInternalException($"Unknown item type of {collectionType} result", Reader);
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
              throw new XmlInternalException($"Could not create a new instance of {collectionType} result", Reader);
            foreach (var item in tempList)
            {
              var kvPair = (KeyValuePair<object, object?>)item;
              newDictionaryObject.Add(kvPair.Key, kvPair.Value);
            }
            //propertyInfo.SetValue(context, newDictionaryObject);
            result = newDictionaryObject;
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
            var arrayObject = result as Array;
            if (arrayObject == null)
              throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to Array", Reader);
            if (arrayObject.Length == tempList.Count)
              for (int i = 0; i < tempList.Count; i++)
                arrayObject.SetValue(tempList[i], i);
            else if (!propertyInfo.CanWrite)
              throw new XmlInternalException($"Collection at property {propertyInfo.Member.Name}" +
                                             $" is an array of different length than number of read items but is readonly and can't be changed",
                Reader);
            var itemArray = Array.CreateInstance(itemType, tempList.Count);
            for (int i = 0; i < tempList.Count; i++)
              itemArray.SetValue(tempList[i], i);
            //propertyInfo.SetValue(context, itemArray);
            result = itemArray;
            break;

          case CollectionTypeKind.Collection:
            // We can't cast a collection to non-generic ICollection because implementation of ICollection<T>
            // does implicate implementation of ICollection.
            object? collectionObject = result;
            // ICollection has no Add method so we must localize this method using reflection.
            var addMethod = collectionObject.GetType().GetMethod("Add", new Type[] { itemType });
            if (addMethod == null)
              throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} result", Reader);
            // We must do the same with Clear method.
            var clearMethod = collectionObject.GetType().GetMethod("Clear");
            if (clearMethod == null)
              throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} result", Reader);

            if (tempList.Count > 0)
            {
              clearMethod.Invoke(collectionObject, new object[0]);
              for (int i = 0; i < tempList.Count; i++)
                addMethod.Invoke(collectionObject, new object[] { tempList[i] });
            }
            break;

          case CollectionTypeKind.List:
            IList? listObject = result as IList;
            if (listObject == null)
              throw new XmlInternalException($"Collection value at property {propertyInfo.Member.Name} cannot be typecasted to IList", Reader);
            listObject.Clear();
            for (int i = 0; i < tempList.Count; i++)
              listObject.Add(tempList[i]);
            break;

          case CollectionTypeKind.Dictionary:
            IDictionary? dictionaryObject = result as IDictionary;
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
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberCollection>");
#endif
    return result;
  }

  public object? ReadMemberElementAsCollection(object context, SerializationMemberInfo memberInfo, ContentItemInfo collectionInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberElementAsCollection context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var qualifiedName = ReadElementTag();
    SerializationTypeInfo? typeInfo = null;
    if (memberInfo.ValueType?.Type != typeof(object))
      typeInfo = memberInfo.ValueType;
    else
    if (!TryGetTypeInfo(qualifiedName, out typeInfo))
      typeInfo = memberInfo.ValueType;
    if (typeInfo == null)
      throw new XmlInternalException($"Unknown type for element \"{qualifiedName}\" on deserialize", Reader);

    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType != null)
    {
      if (expectedType.IsNullable(out var baseType))
        expectedType = baseType;
      if (expectedType == null)
        throw new XmlInternalException($"Element \"{qualifiedName}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but expected type is null", Reader);
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{qualifiedName}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    }

    var result = ReadMemberCollectionWithKnownTypeAndMemberInfo(context, typeInfo, memberInfo, collectionInfo);
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberElementAsCollection>");
#endif
    return result;
  }

  public object? ReadMemberCollectionWithKnownTypeAndMemberInfo(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo, ContentItemInfo collectionInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberCollectionWithKnownTypeAndMemberInfo context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
    if (typeInfo.XmlConverter?.CanRead == true)
      result = typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, null);
    else
    {
      if (typeInfo.KnownConstructor == null)
      {
        throw new XmlInternalException($"Collection of type {typeInfo.Type.Name} must have a parameterless constructor", Reader);
      }
      else
      {
        result = typeInfo.KnownConstructor.Invoke(new object[0]);
        if (result is IXmlSerializable xmlSerializable)
          xmlSerializable.ReadXml(Reader);
        else
        {
          ReadPropertiesAsAttributes(result, typeInfo);
          Reader.Read();
          ReadCollectionItems(result, (ICollection)result, collectionInfo.KnownItemTypes.First().TypeInfo.Type, collectionInfo);
          Reader.Read();
        }
      }
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberCollectionWithKnownTypeAndMemberInfo>");
#endif
    return result;
  }

  public int ReadCollectionItems(object context, ICollection collection, Type collectionItemType, ContentItemInfo collectionInfo)
  {
    int itemsRead = 0;
    while (Reader.NodeType == XmlNodeType.Whitespace)
      Reader.Read();
    var collectionType = collection.GetType();
    var addMethod = collectionType.GetMethod("Add", new Type[] { collectionItemType });
    if (addMethod == null)
      throw new InvalidOperationException($"Add method({collectionItemType}) not found in {collectionType}");
    while (Reader.NodeType == XmlNodeType.Element)
    {
      var item = ReadElementAsItem(collection, collectionItemType, collectionInfo);
      if (item != null)
        addMethod.Invoke(collection, new object[] { item });
      SkipWhitespaces();
    }
    return itemsRead;
  }

  public int ReadDictionaryItems(object context, ICollection<object> collection, Type collectionKeyType, Type collectionValueType,
    DictionaryInfo dictionaryInfo)
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

  public void ReadTextMember(object context, SerializationMemberInfo memberInfo)
  {
    object? propValue = ReadValue(context, memberInfo);
    if (propValue != null)
    {
      SetValue(context, memberInfo, propValue);
    }
  }


  public object? ReadValue(object? context, SerializationMemberInfo serializationMemberInfo)
  {
    var propType = serializationMemberInfo.MemberType;
    if (propType == null)
      return null;
    var typeConverter = serializationMemberInfo.GetTypeConverter();
    var propValue = ReadValue(context, propType, typeConverter, serializationMemberInfo);
    return propValue;
  }

  public object? ReadValue(object? context, Type expectedType, TypeConverter? typeConverter, SerializationMemberInfo? memberInfo)
  {
    if (Reader.NodeType == XmlNodeType.None)
      return null;
    object? propValue = null;
    if (expectedType.Name.StartsWith("Nullable`1"))
      expectedType = expectedType.GetGenericArguments()[0];
    if (Reader.NodeType != XmlNodeType.Text && typeConverter is IXmlConverter xmlConverter && xmlConverter.CanConvert(expectedType))
    {
      propValue = xmlConverter.ReadXml(context, Reader, expectedType, null, this);
      return propValue;
    }
    var str = Reader.ReadString();
    str = str.DecodeStringValue();

    if (expectedType == typeof(string))
      propValue = str;
    else if (typeConverter != null)
    {
      if (context != null)
        propValue = typeConverter.ConvertFromInvariantString(new TypeDescriptorContext(context, memberInfo?.Property), str);
      else
        propValue = typeConverter.ConvertFromInvariantString(null, str);
      return propValue;
    }
    else if (expectedType == typeof(char))
    {
      if (str.Length > 0)
        propValue = str[0];
      else
        propValue = '\0';
    }
    else if (expectedType == typeof(bool))
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
            if (context != null)
              propValue = typeConverter.ConvertFromInvariantString(new TypeDescriptorContext(context, memberInfo?.Property), str);
            else
              propValue = typeConverter.ConvertFromInvariantString(null, str);
            break;
          }
          throw new XmlInternalException($"Cannot convert \"{str}\" to {expectedType} value", Reader);
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
    else if (expectedType == typeof(Byte[]))
    {
      propValue = Convert.FromBase64String(str);
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
    else if (expectedType == typeof(Type))
    {
      propValue = new TypeNameConverter(KnownTypes.Keys, KnownNamespaces.XmlNamespaceToPrefix).ConvertFrom(null, null, str);
    }
    //else
    //  throw new XmlInternalException($"Value type \"{expectedType}\" not supported for deserialization", reader);
    return propValue;
  }

  public XmlQualifiedTagName ReadElementTag()
  {
    var result = new XmlQualifiedTagName(Reader.LocalName, Reader.Prefix);
    if (string.IsNullOrEmpty(result.Namespace))
    {
      var type = TypeNaming.GetType(result.Name);
      if (type != null)
        result = new XmlQualifiedTagName(type.FullName ?? "");
    }
    return result;
  }

  public void SetValue(object context, SerializationMemberInfo memberInfo, object? value)
  {
    //if (memberInfo.XmlName == "PropertyId")
    //  TestTools.Stop();
    if (value != null)
    {
      var expectedType = memberInfo.MemberType;
      var valueType = value.GetType();
      if (valueType != expectedType && expectedType != null)
      {
        var typeConverter = memberInfo.TypeConverter;
        if (typeConverter == null && expectedType.IsSimple())
          typeConverter = new ValueTypeConverter(expectedType, KnownTypes.Keys);
        var typeDescriptor = new TypeDescriptorContext(context);
        if (typeConverter?.CanConvertFrom(typeDescriptor, valueType) == true)
          value = typeConverter.ConvertFrom(typeDescriptor, null, value);
      }
    }
    memberInfo.SetValue(context, value);
  }
  #endregion
}