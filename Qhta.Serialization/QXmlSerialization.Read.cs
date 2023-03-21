#define TraceReader

using System.Reflection;
using System.Reflection.PortableExecutable;

using Qhta.Xml.Reflection;

using Qhta.Xml.Serialization;

namespace Qhta.Xml.Serialization;

public delegate void OnUnknownMember(object readObject, string elementName);

public partial class QXmlSerializer : IXmlConverterReader
{

  /// <summary>
  ///   Main deserialization entry
  /// </summary>
  public object? DeserializeObject(XmlReader xmlReader)
  {
#if TraceReader
    File.Delete("QXmlSerializerReadTrace.xml");
    using (var myWriter = new TextWriterTraceListener("QXmlSerializerReadTrace.xml"))
    {
      var defaultListener = Trace.Listeners[0];
      Trace.Listeners.Remove("Default");
      Trace.Listeners.Add(myWriter);
      Trace.IndentSize = 2;
      Reader = new QXmlReader(xmlReader);
      var result = ReadObject();
      myWriter.Close();
      Trace.Listeners.Add(defaultListener);
      return result;
    }
#else
    Reader = new QXmlReader(xmlReader);
    var result = ReadObject();
    return result;
#endif
  }

  public XmlReaderSettings XmlReaderSettings { get; } = new() { IgnoreWhitespace = true };

  public XmlSerializerNamespaces Namespaces { get; private set; } = new();

  XmlReader IXmlConverterReader.Reader => Reader;

  /// <summary>
  /// Internal XML reader
  /// </summary>
  public QXmlReader Reader { get; private set; } = null!;

  #region Read object methods

  /// <summary>
  /// Reads an object in a specific context. 
  /// On entry, the Reader must be located at the XML start element (or empty element) tag.
  /// This tag represents the object type. Its name is a type name (with namespace) or a type root element name.
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </summary>
  /// <param name="context">An object in which this object is contained. It can be null (when reading a root XML element)</param>
  /// <returns>Read object</returns>
  /// <exception cref="XmlInternalException">Thrown in a tag does not represent a registered type.</exception>
  /// <remarks>
  /// A typeInfo is get from the <see cref="Mapper"/> object. 
  /// If a typeInfo has <see cref="XmlConverter"/> defined, then this converter is used.
  /// Otherwise <see cref="ReadElementWithKnownTypeInfo"/> method is invoked.
  /// </remarks>
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

  /// <summary>
  /// Reads an object instance with a specifiedTypeInfo. An instance must exists and it is filled here.
  /// On entry, the Reader must be located at the XML start element (or empty element) tag.
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </summary>
  /// <param name="instance">Existing object instance.</param>
  /// <param name="typeInfo">Serialized type info according to the instance type.</param>
  /// <param name="onUnknownMember">Optional Delegate to the method invoked when the type member is not recognized.</param>
  /// <returns>The same object instance</returns>
  /// <remarks>
  /// First XML attributes are read by invoking <see cref="ReadXmlAttributes"/>. 
  /// If the Xml element is not empty then element content is read by <see cref="ReadXmlElements"/>
  /// and if no content elements are read then by <see cref="ReadXmlTextElement"/>
  /// </remarks>
  public object? ReadObjectInstance(object instance, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadObjectInstance instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;

    if (Reader.NodeType == XmlNodeType.Element)
    {
      bool isEmptyElement = Reader.IsEmptyElement;
      var startTagName = Reader.Name;
      ReadXmlAttributes(instance, typeInfo, onUnknownMember);
      if (!isEmptyElement)
      {
        Reader.Read(); // move the read cursor past the end of the start element and navigate to its contents
        SkipWhitespaces();
        if (!isEmptyElement)
        {
          var n = ReadXmlElements(instance, typeInfo);
          if (n == 0)
          {
            var textMemberInfo = typeInfo.TextProperty ?? typeInfo.ContentProperty;
            if (textMemberInfo != null)
              ReadXmlTextElement(instance, textMemberInfo);
          }
          if (Reader.IsEndElement(startTagName))
            Reader.Read();
        }
      }
      else
      {
        Reader.Read(); // move the read cursor past the end of the empty element and navigate to next element
        SkipWhitespaces();
      }
    }
    result = instance;
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadObjectInstance>");
#endif
    return result;
  }
  #endregion

  #region Read element attributes method
  /// <summary>
  /// Reads all XML attributes contained in an XML opening tag, converts attribute texts to values and assigns them to the instance.
  /// On entry, the Reader should be located at the XML start element (or empty element) tag.
  /// On exit, the Reader is located after the last XML attribute (inside the entry element).
  /// </summary>
  /// <param name="instance">Existing object instance.</param>
  /// <param name="typeInfo">Serialized type info according to the instance type.</param>
  /// <param name="onUnknownMember">Optional delegate to the method invoked when the type member is not recognized.</param>
  /// <returns>The number of read attributes</returns>
  /// <remarks>
  ///  xmlns prefixes are recognized as xml namespace declarations. The xml:space attribute is recognized correctly.
  ///  The Xsd prefix causes the attribute to be omitted. To read each attribue value a <see cref="ReadValue"/> method is invoked
  /// </remarks>
  public int ReadXmlAttributes(object instance, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
    //#if TraceReader
    //    Trace.WriteLine($"<ReadPropertiesAsAttributes context=\"{context}\" ReaderName=\"{Reader.Name}\">");
    //    Trace.IndentLevel++;
    //#endif
    var members = typeInfo.KnownMembers;

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

      var propertyToRead = members.FirstOrDefault(item => item.XmlName == attrName);
      if (propertyToRead == null)
      {
        if (onUnknownMember == null)
          throw new XmlInternalException($"Unknown property for attribute \"{attrName}\" in type {instance.GetType().Name}", Reader);
        onUnknownMember(instance, attrName);
      }
      else
      {
        Reader.ReadAttributeValue();
        object? propValue = ReadValue(instance, propertyToRead);
        if (propValue != null)
        {
          try
          {
            propertyToRead.SetValue(instance, propValue);
          }
          catch (Exception ex)
          {
            throw new XmlInternalException($"Could not set value for property {propertyToRead.Member} in type {instance.GetType().Name}", Reader, ex);
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
  #endregion

  #region Read XML elements methods
  /// <summary>
  /// Reads all XML elements contained in the instance XML open-end tags.
  /// On entry, the Reader is located just after the XML start element that represents the instance.
  /// On exit, the Reader is located just before the corresponding XML ending element.
  /// </summary>
  /// <param name="instance">Existing object instance.</param>
  /// <param name="typeInfo">Serialized type info according to the instance type.</param>
  /// <param name="onUnknownMember">Optional delegate to the method invoked when the type member is not recognized.</param>
  /// <returns>The number of elements read</returns>
  /// <exception cref="XmlInternalException">Thrown when the XML element is unrecognized.</exception>
  /// <remarks>
  /// The method iterates for all XML elements and exits when no Xml element is met.
  /// A member info is searched in serialization type info known instance members.
  /// If a member is found, then <see cref="ReadElementAsInstanceMember"/> is invoked.
  /// Otherwise, the Xml element should represent direct instance content.
  /// There are two cases for a direct instance content. 
  /// One is when the instance has known ContentProperty. In this case <see cref="ReadElementAsContentProperty"/> is invoked.
  /// The second case is when the instance is a collection. In this case <see cref="ReadElementAsCollectionContent"/> is called.
  /// </remarks>
  public int ReadXmlElements(object instance, SerializationTypeInfo typeInfo, OnUnknownMember? onUnknownMember = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadXmlElements instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    var aType = instance.GetType();
    var members = typeInfo.KnownMembers;

    int result = 0;
    while (Reader.NodeType == XmlNodeType.Element)
    {
      var startTagName = Reader.Name;
      bool isEmptyElement = Reader.IsEmptyElement;
      var memberInfo = members.FirstOrDefault(item => item.XmlName == startTagName.Name);
      if (memberInfo != null)
      {
        var value = ReadElementAsInstanceMember(instance, typeInfo, memberInfo);
        if (value != null)
        {
          SetValue(instance, memberInfo, value);
          result++;
        }
      }
      else
      {
        if (typeInfo.ContentProperty != null)
        {
          var content = ReadElementAsContentProperty(instance, typeInfo, typeInfo.ContentProperty);
          if (content != null)
          {
            SetValue(instance, typeInfo.ContentProperty, content);
            result++;
          }
        }
        else
        {
          if (ReadElementAsCollectionContent(instance, typeInfo)!=null)
          {
            // Returned content can't be set to any member, but is is added to collection inside the called method.
            result++;
          }
          else
          {
            if (onUnknownMember != null)
              onUnknownMember(instance, startTagName.ToString());
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
                  } while (!Reader.IsEndElement(startTagName));
                  Reader.Read();
                }
                continue;
              }
              throw new XmlInternalException(
                $"No instance member to read and no type found for element \"{startTagName}\" in type {aType.FullName}", Reader);
            }
          }
        }
      }
      SkipWhitespaces();
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadXmlElements>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an Xml element as known member of the instance.
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </summary>
  /// <param name="instance">Instance which member is to be read</param>
  /// <param name="instanceTypeInfo">TypeInfo of the instance</param>
  /// <param name="memberInfo">Member info of the member</param>
  /// <returns>Read member value (may be null)</returns>
  /// <remarks>
  /// If a member has an XmlConverter assigner, then it is used to read the value.
  /// Otherwise a member may be read as a collection by invoking <see cref="ReadElementAsMemberCollection"/>,
  /// however a collection may have its own properties, and then should read as a "normal" object.
  /// Other, "normal" object are read by invoking <see cref="ReadElementAsMemberObject"/>.
  /// </remarks>
  public object? ReadElementAsInstanceMember(object instance, SerializationTypeInfo instanceTypeInfo, SerializationMemberInfo memberInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsInstanceMember instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result;
    Type memberType = memberInfo.MemberType ?? typeof(Object);
    if (memberInfo.XmlConverter?.CanRead == true)
    {
      result = memberInfo.XmlConverter.ReadXml(instance, Reader, memberInfo.ValueType?.Type ?? typeof(object), null, this);
    }
    else
    {
      var contentInfo = (memberInfo.ContentInfo ?? memberInfo.ValueType?.ContentInfo ?? instanceTypeInfo.ContentInfo) as ContentItemInfo;
      if (!memberInfo.IsObject && memberInfo.IsCollection && contentInfo != null)
        result = ReadElementAsMemberCollection(instance, memberInfo, contentInfo);
      else
        result = ReadElementAsMemberObject(instance, memberInfo);
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsInstanceMember>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an XML element as an object which will be assigned to the instance content property.
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the object.
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </summary>
  /// <param name="instance">Instance which content property is to be read</param>
  /// <param name="instanceTypeInfo">TypeInfo of the instance</param>
  /// <param name="memberInfo">Member info of the member which represents a ContentProperty</param>
  /// <returns>Read value (may be null)</returns>
  /// <exception cref="XmlInternalException">Thrown on invalid type mapping.</exception>
  /// <remarks>
  /// After validation of types, simply <see cref="ReadObjectWithKnownTypeAndMemberInfo"/>
  /// </remarks>
  public object? ReadElementAsContentProperty(object instance, SerializationTypeInfo instanceTypeInfo, SerializationMemberInfo memberInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsContentProperty instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var name = Reader.Name;
    SerializationTypeInfo? valueTypeInfo = null;
    valueTypeInfo = memberInfo.ValueType;
    if (valueTypeInfo == null)
      throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", Reader);
    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType != null)
    {
      if (expectedType.IsNullable(out var baseType))
        expectedType = baseType;
      if (expectedType == null)
        throw new XmlInternalException($"Element \"{name}\" is mapped to {valueTypeInfo.Type.Name}" +
                                       $" but expected type is null", Reader);
      if (!valueTypeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {valueTypeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    }

    var result = ReadObjectWithKnownTypeAndMemberInfo(instance, valueTypeInfo, memberInfo);
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsContentProperty>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an XML element as an object which is added to the instance as a collection (or dictionary).
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the object.
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </summary>
  /// <param name="instance">Instance which must be a collection (or dictionary) to add a value</param>
  /// <param name="instanceTypeInfo">TypeInfo of the instance</param>
  /// <returns>Read value (may be null)</returns>
  /// <exception cref="XmlInternalException">Thrown on collection (or dictionary) error.</exception>
  public object? ReadElementAsCollectionContent(object instance, SerializationTypeInfo typeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsCollectionContent instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
    if (typeInfo.ContentInfo is ContentItemInfo contentItemInfo)
    {
      var qualifiedName = Reader.Name;
      if (!contentItemInfo.KnownItemTypes.TryGetValue(qualifiedName.Name, out var knownItemTypeInfo))
      {
        if (TryGetTypeInfo(qualifiedName, out var itemTypeInfo))
          knownItemTypeInfo = new SerializationItemInfo(qualifiedName.Name, itemTypeInfo);
      }
      if (knownItemTypeInfo != null)
      {
        object? key = qualifiedName.Name;
        if (knownItemTypeInfo.DictionaryInfo != null)
        {
          var collectionInfo = typeInfo.ContentInfo;
          if (collectionInfo is DictionaryInfo dictionaryInfo)
          {
            (key, result) = ReadElementAsKVPair(instance, null, knownItemTypeInfo.Type, dictionaryInfo);
          }
          else
            throw new InternalException($"TypeInfo({typeInfo.Type}).CollectionInfo must be a DictionaryInfo");
        }
        else
          result = ReadElementWithKnownItemInfo(instance, knownItemTypeInfo);
        if (result == null)
          throw new XmlInternalException($"Item of type {knownItemTypeInfo.Type} could not be read at \"{qualifiedName}\"", Reader);
        if (result.GetType() == typeof(object))
          result = null;
        if (key == null)
          throw new XmlInternalException($"Key for {result} must be not null", Reader);

        if (knownItemTypeInfo.AddMethod != null)
        {
          if (instance is IDictionary dictionaryObj)
            knownItemTypeInfo.AddMethod.Invoke(instance, new object?[] { key, result });
          else
            knownItemTypeInfo.AddMethod.Invoke(instance, new object?[] { result });
        }
        else
        {
          if (instance is IDictionary dictionaryObj)
            dictionaryObj.Add(key, result);
          else
          {
            var collectionType = instance.GetType();
            var adddMethod = collectionType.GetMethod("Add", new Type[] { knownItemTypeInfo.Type });
            if (adddMethod == null)
              adddMethod = collectionType.GetMethod("Add");
            if (adddMethod != null)
              adddMethod.Invoke(instance, new object?[] { result });
            else if (instance is ICollection collectionObj)
              throw new XmlInternalException($"Add method for {knownItemTypeInfo.Type} result not found in {typeInfo.Type?.FullName}", Reader);
          }
        }
      }
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsCollectionContent>");
#endif
    return result;
  }


  /// <summary>
  /// 
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </summary>
  /// <param name="context"></param>
  /// <param name="memberInfo"></param>
  /// <returns></returns>
  /// <exception cref="XmlInternalException"></exception>
  public object? ReadElementAsMemberObject(object context, SerializationMemberInfo memberInfo)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      return null;
    //throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
#if TraceReader
    Trace.WriteLine($"<ReadElementAsMemberObject instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    string? propertyElementName = null;
    if (String.IsNullOrEmpty(Reader.NamespaceURI))
    {
      var propInfo = memberInfo.ValueType?.MembersAsElements.FirstOrDefault(memberItem => memberItem.XmlName == Reader.LocalName);
      if (propInfo != null)
        memberInfo = propInfo;
      propertyElementName = Reader.LocalName;
      Reader.Read();
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

    var result = ReadObjectWithKnownTypeAndMemberInfo(context, typeInfo, memberInfo);
    if (propertyElementName != null)
      Reader.ReadEndElement(propertyElementName);
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsMemberObject>");
#endif
    return result;
  }
  #endregion

  #region Read element as member object
  public object? ReadObjectWithKnownTypeAndMemberInfo(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadObjectWithKnownTypeAndMemberInfo instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
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
          //if (!memberInfo.IsContentElement)
          //  Reader.Read();
          ReadMemberObjectInterior(result, typeInfo, memberInfo);
          //if (!memberInfo.IsContentElement)
          //  Reader.Read();
        }
      }
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadObjectWithKnownTypeAndMemberInfo>");
#endif
    return result;
  }

  public object? ReadMemberObjectOfAnyType(object context, SerializationTypeInfo typeInfo, SerializationMemberInfo memberInfo,
    OnUnknownMember? onUnknownMember = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberObjectUnknownType instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
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
    Trace.WriteLine($"<ReadMemberObjectInterior instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    //if (Reader.NodeType == XmlNodeType.Text)
    //{
    //  ReadTextMember(context, memberInfo);
    //  if (Reader.NodeType == XmlNodeType.EndElement)
    //    Reader.Read();
    //}
    //else
    //{
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);
    bool isEmptyElement = Reader.IsEmptyElement;
    ReadXmlAttributes(context, typeInfo, onUnknownMember);
    Reader.Read(); // read end of start element and go to its content;
    SkipWhitespaces();
    if (!isEmptyElement)
    {
      var n = ReadXmlElements(context, typeInfo);
      if (n == 0 && Reader.NodeType == XmlNodeType.Text)
        ReadXmlTextElement(context, memberInfo);
      if (Reader.NodeType == XmlNodeType.EndElement)
        Reader.Read();
    }
    //}
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
    Trace.WriteLine($"<ReadElementAsItem instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
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
    Trace.WriteLine($"<ReadElementWithKnownTypeInfo instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
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
        ReadObjectInstance(result, typeInfo);
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
    Trace.WriteLine($"<ReadElementWithKnownItemInfo instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
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
        ReadObjectInstance(result, typeInfo);
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
          throw new XmlInternalException($"Key name unknown for dictionary result \"{Reader.Name}\" on deserialize", Reader);
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
    ReadObjectInstance(kvPair.Value, typeInfo, (object readObject, string elementName) =>
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
  // <param name="instanceTypeInfo">TypeInfo of the instance</param>

  /// <summary>
  /// Reads an XML element, which represents a member of the instance and is recognized as a collection (or dictionary)
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </summary>
  /// <param name="instance">Instance which content property is to be read</param>
  /// <param name="memberInfo">Member info of the member which represents a ContentProperty</param>
  /// <returns>Read value (may be null)</returns>
  /// <exception cref="XmlInternalException">Thrown on invalid type mapping.</exception>
  /// <param name="instance">Instance, which member is to be set</param>
  /// <param name="memberInfo">Serialization info of the member, which is to be read</param>
  /// <param name="collectionInfo">Info of the known item types.</param>
  /// <returns>Read collection (or null).</returns>
  /// <exception cref="XmlInternalException">Thrown on errors.</exception>
  public object? ReadElementAsMemberCollection(object instance, SerializationMemberInfo memberInfo, ContentItemInfo collectionInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsMemberCollection instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (memberInfo.ValueType == null)
      throw new XmlInternalException($"Collection type at property {memberInfo.Member.Name}" +
                                     $" of type {instance.GetType().Name} unknown", Reader);
    var collectionType = memberInfo.MemberType;
    //if (collectionType == null)
    //  throw new XmlInternalException($"Unknown collection type for {propertyInfo.Member.DeclaringType}.{propertyInfo.Member.Name}", reader);

    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", Reader);



    List<object> tempList = new List<object>();

    var result = memberInfo.GetValue(instance);
    if (result == null)
    {
      // Check if collection can be written - if not we will not be able to set the property
      if (!memberInfo.CanWrite)
        throw new XmlInternalException($"Collection at property {memberInfo.Member.Name}" +
                                       $" of type {instance.GetType().Name} is  but readonly", Reader);
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
        throw new XmlInternalException($"Unknown result type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.Array;
    }
    else if (collectionType.IsDictionary(out keyType, out valueType))
    {
      if (keyType == null)
        throw new XmlInternalException($"Unknown key type of {collectionType} result", Reader);
      if (valueType == null)
        throw new XmlInternalException($"Unknown result type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.Dictionary;
      itemType = valueType;
    }
    else if (collectionType.Name.StartsWith("List`") && collectionType.IsList(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown result type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.List;
    }
    else if (collectionType.IsCollection(out itemType))
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown result type of {collectionType} result", Reader);
      collectionTypeKind = CollectionTypeKind.Collection;
    }

    if (collectionTypeKind == null)
      result = null;
    else
    {
      if (itemType == null)
        throw new XmlInternalException($"Unknown result type of {collectionType} result", Reader);

      if (collectionTypeKind == CollectionTypeKind.Dictionary)
      {
        var dictionaryInfo = memberInfo.ContentInfo as DictionaryInfo;
        if (dictionaryInfo == null)
          throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlDictionaryAttribute)} attribute", Reader);
        if (Reader.IsStartElement(memberInfo.XmlName) && !Reader.IsEmptyElement)
        {
          Reader.Read();
          if (keyType == null)
            throw new XmlInternalException($"Unknown key type of {collectionType} result", Reader);
          if (valueType == null)
            throw new XmlInternalException($"Unknown result type of {collectionType} result", Reader);
          ReadDictionaryItems(instance, tempList, keyType, valueType, dictionaryInfo);
        }
      }
      else
      {
        if (collectionInfo == null)
          throw new XmlInternalException($"Collection of type {collectionType} must be marked with {nameof(XmlCollectionAttribute)} attribute", Reader);

        if (Reader.IsStartElement(new XmlQualifiedTagName(memberInfo.XmlName, memberInfo.XmlNamespace)) && !Reader.IsEmptyElement)
        {
          Reader.Read();
          if (memberInfo.MemberType != null && Reader.IsStartElement(new XmlQualifiedTagName(memberInfo.MemberType.Name, memberInfo.MemberType.Namespace)))
            result = ReadMemberElementAsCollection(instance, memberInfo, collectionInfo);
          else
            ReadCollectionItems(instance, tempList, itemType, collectionInfo);
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
                throw new XmlInternalException($"Unknown result type of {collectionType} result", Reader);
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
              throw new XmlInternalException($"Collection value at property {memberInfo.Member.Name} cannot be typecasted to Array", Reader);
            if (arrayObject.Length == tempList.Count)
              for (int i = 0; i < tempList.Count; i++)
                arrayObject.SetValue(tempList[i], i);
            else if (!memberInfo.CanWrite)
              throw new XmlInternalException($"Collection at property {memberInfo.Member.Name}" +
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
              throw new XmlInternalException($"Collection value at property {memberInfo.Member.Name} cannot be typecasted to IList", Reader);
            listObject.Clear();
            for (int i = 0; i < tempList.Count; i++)
              listObject.Add(tempList[i]);
            break;

          case CollectionTypeKind.Dictionary:
            IDictionary? dictionaryObject = result as IDictionary;
            if (dictionaryObject == null)
              throw new XmlInternalException($"Collection value at property {memberInfo.Member.Name} cannot be typecasted to IDictionary", Reader);
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
    Trace.WriteLine($"</ReadElementAsMemberCollection>");
#endif
    return result;
  }

  public object? ReadMemberElementAsCollection(object context, SerializationMemberInfo memberInfo, ContentItemInfo collectionInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberElementAsCollection instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
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
    Trace.WriteLine($"<ReadMemberCollectionWithKnownTypeAndMemberInfo instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
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
          ReadXmlAttributes(result, typeInfo);
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
  #endregion

  #region Read text value methods

  /// <summary>
  /// Reads Xml text element and assignes it to the instance.
  /// To assign the value, <see cref="SetValue"/> method is invoked, 
  /// which allows to translate value to the valid type.
  /// </summary>
  /// <param name="instance">The object instance to which the value is to be assigned.</param>
  /// <param name="memberInfo">Serialization member info of the value.</param>
  public void ReadXmlTextElement(object instance, SerializationMemberInfo memberInfo)
  {
    object? propValue = ReadValue(instance, memberInfo);
    if (propValue != null)
    {
      SetValue(instance, memberInfo, propValue);
    }
  }

  /// <summary>
  /// Reads a value from XML text according to member info. Expected type and type converter is extracted from the member info.
  /// </summary>
  /// <param name="context">Context of read (instance or collection) passed to <see cref="XmlConverter"/></param>
  /// <param name="memberInfo">Serialization member info of the read member</param>
  /// <returns>Read value (may be null)</returns>
  public object? ReadValue(object? context, SerializationMemberInfo memberInfo)
  {
    var memberType = memberInfo.MemberType;
    if (memberType == null)
      return null;
    var typeConverter = memberInfo.GetTypeConverter();
    var value = ReadValue(context, memberType, typeConverter, memberInfo);
    return value;
  }

  /// <summary>
  /// Reads a text value according to member info and using expected type and type converter.
  /// </summary>
  /// <param name="context">Context of read (instance or collection) passed to <see cref="XmlConverter"/></param>
  /// <param name="expectedType">Expected type of the value to read. Used to convert read string to the object value.</param>
  /// <param name="typeConverter">
  /// If it is <see cref="XmlConverter"/>, it is used to read the XML. 
  /// Otherwise is can be used to convert string value to the object value.
  /// </param>
  /// <param name="memberInfo">Serialization member info of the read member</param>
  /// <returns>Read value (may be null)</returns>
  /// <remarks>This method has its own algorithm to convert simple type values from string.</remarks>
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
  #endregion

  #region Helper methods

  /// <summary>
  /// Tries to get a registered typeinfo for a specified type. 
  /// A type should be registered in <see cref="Mapper"/> object.
  /// </summary>
  /// <param name="type">Searched type.</param>
  /// <param name="typeInfo">Found typeInfo (or null when not found).</param>
  /// <returns>True when typeInfo found, false otherwise.</returns>
  public bool TryGetTypeInfo(Type type, [NotNullWhen(true)][MaybeNullWhen(false)] out SerializationTypeInfo? typeInfo)
  {
    return Mapper.KnownTypes.TryGetValue(type, out typeInfo);
  }

  /// <summary>
  /// Tries to get a registered typeinfo for a specified tag name. 
  /// A type should be registered in <see cref="Mapper"/> object.
  /// </summary>
  /// <param name="tag">Searched XML tag.</param>
  /// <param name="typeInfo">Found typeInfo (or null when not found).</param>
  /// <returns>True when typeInfo found, false otherwise.</returns>
  public bool TryGetTypeInfo(XmlQualifiedTagName tag, [NotNullWhen(true)][MaybeNullWhen(false)] out SerializationTypeInfo? typeInfo)
  {
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

  /// <summary>
  /// Skip everything to the next XML element.
  /// </summary>
  public void SkipToElement()
  {
    while (!Reader.EOF && Reader.NodeType != XmlNodeType.Element)
      Reader.Read();
  }

  /// <summary>
  /// Skip whitespaces
  /// </summary>
  public void SkipWhitespaces()
  {
    while (Reader.NodeType == XmlNodeType.Whitespace)
      Reader.Read();
  }

  /// <summary>
  /// Helper method to read element tag. 
  /// If a tag has no namespace, then a local name is translated to registered type name.
  /// </summary>
  /// <returns>Read or translated tag name.</returns>
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

  /// <summary>
  /// Helper method to set a value to the object instance using member info.
  /// If a member has type converter, the value is translated (when it is possible).
  /// </summary>
  /// <param name="instance">The object instance to which the value is to be assigned.</param>
  /// <param name="memberInfo">Serialization member info of the value.</param>
  /// <param name="value">The value to set (may be null).</param>
  public void SetValue(object instance, SerializationMemberInfo memberInfo, object? value)
  {
    if (value != null)
    {
      var expectedType = memberInfo.MemberType;
      var valueType = value.GetType();
      if (valueType != expectedType && expectedType != null)
      {
        var typeConverter = memberInfo.TypeConverter;
        if (typeConverter == null && expectedType.IsSimple())
          typeConverter = new ValueTypeConverter(expectedType, KnownTypes.Keys);
        var typeDescriptor = new TypeDescriptorContext(instance);
        if (typeConverter?.CanConvertFrom(typeDescriptor, valueType) == true)
          value = typeConverter.ConvertFrom(typeDescriptor, null, value);
      }
    }
    memberInfo.SetValue(instance, value);
  }
  #endregion
}