//#define TraceReader

// ReSharper disable All
namespace Qhta.Xml.Serialization;

/// <summary>
/// Reading methods.
/// </summary>
public partial class QXmlSerializer
{
  /// <summary>
  /// Delegate invoked if a reader meets an element or attribute which can't be recognized.
  /// </summary>
  /// <param name="readObject"></param>
  /// <param name="elementName"></param>
  public delegate void UnknownMemberDelegate(object readObject, string elementName);

  /// <summary>
  /// Optional event raised when a reader meets an element or attribute which can't be recognized.
  /// </summary>
  public event UnknownMemberDelegate? OnUnknownMember;

  /// <summary>
  ///   Collection of referenced objects during serialization/deserialization.
  /// </summary>
  public readonly ReferencedObjects ReferencedObjects = new();

  /// <summary>
  ///   Main deserialization entry.
  /// </summary>
  /// <param name="xmlReader">reader to use</param>
  /// <param name="instance">optional existing object to read</param>
  public object? DeserializeObject(XmlReader xmlReader, object? instance = null)
  {
#if TraceReader
    File.Delete("QXmlSerializerReadTrace.xml");
    using (var myWriter = new Qhta.TestHelper.TraceTextWriter("QXmlSerializerReadTrace.xml", true))
    {
      var defaultListener = Trace.Listeners[0];
      Trace.Listeners.Remove("Default");
      Trace.Listeners.Add(myWriter.TraceListener);
      Trace.IndentSize = 2;
      Reader = new QXDocReader(xmlReader);
      var result = ReadObject();
      Trace.Listeners.Add(defaultListener);
      return result;
    }
#else
    Reader = new QXDocReader(xmlReader);
    if (instance != null)
    {
      ReadObject(null, instance);
      return instance;
    }
    else
    {
      var result = ReadObject();
      return result;
    }
#endif
  }

  /// <summary>
  ///   Secondary deserialization entry
  /// </summary>
  /// <param name="xmlReader">reader to use</param>
  /// <param name="instance">optional existing object to read</param>
  public object? DeserializeObject(IXmlReader xmlReader, object? instance = null)
  {
#if TraceReader
    File.Delete("QXmlSerializerReadTrace.xml");
    using (var myWriter = new Qhta.TestHelper.TraceTextWriter("QXmlSerializerReadTrace.xml", true))
    {
      var defaultListener = Trace.Listeners[0];
      Trace.Listeners.Remove("Default");
      Trace.Listeners.Add(myWriter.TraceListener);
      Trace.IndentSize = 2;
      Reader = (QXDocReader)xmlReader;
      var result = ReadObject();
      Trace.Listeners.Add(defaultListener);
      return result;
    }
#else
    Reader = (QXDocReader)xmlReader;
    if (instance != null)
    {
      ReadObject(null, instance);
      return instance;
    }
    else
    {
      var result = ReadObject();
      return result;
    }
#endif
  }

  /// <summary>
  /// System settings for XmlReader.
  /// </summary>
  public XmlReaderSettings XmlReaderSettings { [DebuggerStepThrough] get; } = new() { IgnoreWhitespace = true };

  /// <summary>
  /// System Namespaces.
  /// </summary>
  public XmlSerializerNamespaces Namespaces { [DebuggerStepThrough] get; private set; } = new();

  /// <summary>
  /// System XML reader wrapper.
  /// </summary>
  public QXDocReader Reader { [DebuggerStepThrough] get; private set; } = null!;

  #region Read object methods

  /// <summary>
  /// Reads an object in a specific context. 
  /// </summary>
  /// <param name="context">An object in which this object is contained. It can be null (when reading a root XML element)</param>
  /// <returns>Read object</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown in a tag does not represent a registered type.</exception>
  /// <entrystate>
  /// On entry, the Reader must be located at the XML start element (or empty element) tag.
  /// This tag represents the object type. Its name is a type name (with namespace) or a type root element name.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// A typeInfo is get from the <see cref="Mapper"/> object. 
  /// If a typeInfo has <see cref="XmlConverter"/> defined, then this converter is used.
  /// Otherwise, <see cref="ReadObjectWithKnownTypeInfo"/> method is invoked.
  /// </remarks>
  public object? ReadObject(object? context = null)
  {
    SkipToElement();
    if (Reader.EOF)
      return null;

    var qualifiedName = Reader.Name;
    if (!TryGetTypeInfo(qualifiedName, out var typeInfo))
      throw new XmlInvalidOperationException($"Element {qualifiedName} not recognized while deserialization.", Reader);
    if (typeInfo.XmlConverter != null)
      return typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, this);
    return ReadObjectWithKnownTypeInfo(context, typeInfo);
  }


  /// <summary>
  /// Reads an existing object in a specific context. 
  /// </summary>
  /// <param name="context">An object in which this object is contained. It can be null (when reading a root XML element)</param>
  /// <param name="instance">Existing object which content is filled</param>
  /// <returns>Read object</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown in a tag does not represent a registered type.</exception>
  /// <entrystate>
  /// On entry, the Reader must be located at the XML start element (or empty element) tag.
  /// This tag represents the object type. Its name is a type name (with namespace) or a type root element name.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// A typeInfo is get from the <see cref="Mapper"/> object. 
  /// If a typeInfo has <see cref="XmlConverter"/> defined, then this converter is used.
  /// Otherwise, <see cref="ReadObjectWithKnownTypeInfo"/> method is invoked.
  /// </remarks>
  public object? ReadObject(object? context, object instance)
  {
    SkipToElement();
    if (Reader.EOF)
      return null;

    var qualifiedName = Reader.Name;
    if (!TryGetTypeInfo(qualifiedName, out var typeInfo))
      throw new XmlInvalidOperationException($"Element {qualifiedName} not recognized while deserialization.", Reader);

    if (typeInfo.XmlConverter != null)
      typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, instance, this);
    return ReadObjectInstance(instance, typeInfo);
  }

  /// <summary>
  /// Reads element as object. If type info has XmlConverter defined, this converter is used.
  /// Simple types and array of bytes are read by using <see cref="ReadValue"/> method.
  /// Other types are read by using <see cref="ReadObjectInstance"/>.
  /// </summary>
  /// <param name="context">An object in which this object is contained. It can be null (when reading a root XML element)</param>
  /// <param name="typeInfo">A type info for expected object</param>
  /// <returns>Read object</returns>
  /// <exception cref="XmlInvalidOperationException"></exception>
  /// <entrystate>
  /// On entry, the Reader must be located at the XML start element (or empty element) tag that represents the object.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// Internal method for <see cref="ReadObject(object?)"/>. 
  /// Also used in <see cref="ReadElementAsItem"/>.
  /// </remarks>
  public object? ReadObjectWithKnownTypeInfo(object? context, SerializationTypeInfo typeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadObjectWithKnownTypeInfo instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (typeInfo.Type.Name == "DocumentQualityCollection")
      Debug.Assert(true);
    object? result = null;
    if (typeInfo.XmlConverter?.CanRead == true)
      return typeInfo.XmlConverter.ReadXml(context, Reader, typeInfo.Type, null, null);
    var startTag = Reader.Name;
    if (typeInfo.KnownConstructor == null)
    {
      if (typeInfo.Type.IsSimple() || typeInfo.Type == typeof(byte[]) || typeInfo.Type == typeof(Uri) || typeInfo.TypeConverter != null)
      {
        if (!Reader.IsEmptyElement)
        {
          Reader.Read();
          result = ReadValueWithTypeConverter(context, typeInfo.Type, typeInfo.TypeConverter, null);
          Reader.ReadEndElement(startTag);
        }
        else
        {
          Reader.Read();
          if (typeInfo.Type == typeof(byte[]))
            result = Array.Empty<byte>();
          else
            result = System.Activator.CreateInstance(typeInfo.Type);
        }
      }
      else
      if (typeInfo.Type.IsArray)
      {
        Reader.Read();
      }
      else
        throw new XmlInvalidOperationException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
    }
    else
    {
      result = typeInfo.KnownConstructor.Invoke([]);
      if (result is IXmlSerializable xmlSerializable)
        xmlSerializable.ReadXml(Reader.BaseXmlReader);
      else
        result = ReadObjectInstance(result, typeInfo);
    }
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadObjectWithKnownTypeInfo>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an object instance with a specifiedTypeInfo. An instance must exists and it is filled here.
  /// </summary>
  /// <param name="instance">Existing object instance.</param>
  /// <param name="instanceTypeInfo">Serialized type info according to the instance type.</param>
  /// <returns>The same object instance</returns>
  /// <entrystate>
  /// On entry, the Reader must be located at the XML start element (or empty element) tag that represents the instance.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// First XML attributes are read by invoking <see cref="ReadXmlAttributes"/>. 
  /// If the Xml element is not empty then element content is read by <see cref="ReadXmlElements"/>
  /// and if no content elements are read then by <see cref="ReadXmlTextElement"/>
  /// </remarks>
  public object? ReadObjectInstance(object instance, SerializationTypeInfo instanceTypeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadObjectInstance instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (instanceTypeInfo.Type.Name == "QualityMetrics")
      Debug.Assert(true);
    if (instanceTypeInfo.Type.Name == "QualityMeasure")
      Debug.Assert(true);
    object? result = null;
    if (Reader.NodeType == XmlNodeType.Element)
    {
      bool isEmptyElement = Reader.IsEmptyElement;
      var startTagName = Reader.Name;
      ReadXmlAttributes(instance, instanceTypeInfo);
      if (!isEmptyElement)
      {
        Reader.Read(); // move the read cursor past the end of the start element and navigate to its contents
        //SkipWhitespaces();
        if (!isEmptyElement)
        {
          int elementsRead = 0;
          if (Reader.NodeType == XmlNodeType.Element)
            elementsRead = ReadXmlElements(instance, instanceTypeInfo);
          if (elementsRead == 0)
          {
            var textMemberInfo = instanceTypeInfo.TextProperty ?? instanceTypeInfo.ContentProperty;
            if (textMemberInfo != null)
            {
              if (Reader.NodeType == XmlNodeType.Text || Reader.NodeType == XmlNodeType.SignificantWhitespace)
                ReadXmlTextElement(instance, textMemberInfo);
              else
                ReadXmlTextElement(instance, textMemberInfo);
            }
          }
          Reader.ReadEndElement(startTagName);
        }
      }
      else
      {
        Reader.Skip(); // move the read cursor past the end of the empty element and navigate to next element
        SkipWhitespaces();
      }
    }
    if (instanceTypeInfo.IdProperty != null)
    {
      var idString = ReferencedObjectHelper.GetReferenceString(instance, instanceTypeInfo.IdProperty);
      if (idString != null)
        ReferencedObjects.AddReference(instanceTypeInfo.Type, idString, instance);
    }
    result = instance;
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadObjectInstance>");
    if (result.ToString()=="Quest.QualityScale")
      Debug.Assert(true);
#endif
    return result;
  }

  #endregion

  #region Read element attributes method
  /// <summary>
  /// Reads all XML attributes contained in an XML opening tag, converts attribute texts to values and assigns them to the instance.
  /// </summary>
  /// <param name="instance">Existing object instance.</param>
  /// <param name="instanceTypeInfo">Serialized type info according to the instance type.</param>
  /// <returns>The number of read attributes</returns>
  /// <entrystate>
  /// On entry, the Reader should be located at the XML start element (or empty element) tag that represents the instance.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the last XML attribute (inside the entry element).
  /// </exitstate>
  /// <remarks>
  ///  xmlns prefixes are recognized as xml namespace declarations. The xml:space attribute is recognized correctly.
  ///  The Xsd prefix causes the attribute to be omitted. To read each attribute value a ReadValue method is invoked
  /// </remarks>
  public int ReadXmlAttributes(object instance, SerializationTypeInfo instanceTypeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadPropertiesAsAttributes instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    var members = instanceTypeInfo.KnownMembers;
    int result = 0;
    Reader.MoveToFirstAttribute();
    var attrCount = Reader.AttributeCount;
    for (int i = 0; i < attrCount; i++)
    {
      string attrPrefix = Reader.Prefix;
      string attrName = Reader.LocalName;
      if (instance.GetType().Name == "QualityMetrics" && attrName == "Value")
        Debug.Assert(true);
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
        if (OnUnknownMember == null)
          throw new XmlInvalidOperationException($"Unknown property for attribute \"{attrName}\" in type {instance.GetType().Name}", Reader);
        OnUnknownMember(instance, attrName);
      }
      else
      {
        Reader.ReadAttributeValue();
        if (propertyToRead.IsReference)
        {
          string? referenceValue = ReadReferenceValue(instance, propertyToRead);
          if (referenceValue != null)
          {
            try
            {
              var referencedObject = ReferencedObjects.GetReference(propertyToRead.ValueType.Type, referenceValue);
              if (referencedObject != null)
                propertyToRead.SetValue(instance, referencedObject);
            }
            catch (Exception ex)
            {
              throw new XmlInvalidOperationException($"Could not set value for property {propertyToRead.Member} in type {instance.GetType().Name}", Reader, ex);
            }

            result++;
          }
        }
        else
        {
          object? propValue = ReadValue(instance, propertyToRead);
          if (propValue != null)
          {
            try
            {
              propertyToRead.SetValue(instance, propValue);
            }
            catch (Exception ex)
            {
              throw new XmlInvalidOperationException($"Could not set value for property {propertyToRead.Member} in type {instance.GetType().Name}", Reader, ex);
            }

            result++;
          }
        }
      }
      if (!Reader.MoveToNextAttribute()) break;
    }
#if TraceReader
    Trace.WriteLine($"<Return result=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadPropertiesAsAttributes>");
#endif
    return result;
  }
  #endregion

  #region Read XML elements methods
  /// <summary>
  /// Reads all XML elements contained in the instance XML open-end tags.
  /// </summary>
  /// <param name="instance">Existing object instance.</param>
  /// <param name="instanceTypeInfo">Serialized type info according to the instance type.</param>
  /// <param name="instanceAsMemberInfo">Optional info when instance is a member of other instance</param>
  /// 
  /// <returns>The number of elements read</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown when the XML element is unrecognized.</exception>
  /// <entrystate>
  /// On entry, the Reader is located just after the XML start element that represents the instance.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located just before the corresponding XML ending element.
  /// </exitstate>
  /// <remarks>
  /// The method iterates for all XML elements and exits when no Xml element is met.
  /// A member info is searched in serialization type info known instance members.
  /// If a member is found, then <see cref="ReadElementAsInstanceMember"/> is invoked.
  /// Otherwise, the Xml element should represent direct instance content.
  /// There are two cases for a direct instance content. 
  /// One is when the instance has known ContentProperty. In this case <see cref="ReadElementAsContentProperty"/> is invoked.
  /// The second case is when the instance is a collection. In this case <see cref="ReadElementAsCollectionItem"/> is called.
  /// </remarks>
  public int ReadXmlElements(object instance, SerializationTypeInfo instanceTypeInfo, SerializationMemberInfo? instanceAsMemberInfo = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadXmlElements instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    var aType = instance.GetType();
    var members = instanceTypeInfo.KnownMembers;
    int result = 0;
    while (Reader.NodeType == XmlNodeType.Element)
    {
      var startTag = Reader.Name;
      bool isEmptyElement = Reader.IsEmptyElement;
      SerializationMemberInfo? memberInfo;
      SerializationTypeInfo? currentMemberTypeInfo = null;
      {
        //if (startTag.Name == "ShapeDefaults")
        //  Debug.Assert(true);
        //if (startTag.Name == "VmlShapeLayout")
        //  Debug.Assert(true);
        memberInfo = members.FirstOrDefault(item => item.XmlName == startTag.LocalName);
        if (memberInfo != null)
        {

          var value = ReadElementAsProperty(instance, instanceTypeInfo, memberInfo);
          if (value != null)
          {
            if (memberInfo.CanWrite && value.GetType().IsAssignableFrom(memberInfo.ValueType.Type))
              SetValue(instance, memberInfo, value);
            else
            {
              if (memberInfo.IsCollection)
                AddValue(instance, memberInfo, value);
              else
                throw new InvalidOperationException($"Cannot set value for property {memberInfo.Member} in type {aType.FullName}");
            }
          }
          result++;
          continue;
        }

        //if (memberInfo.ValueType.IsCollection)
        //{

        //  var value = ReadElementAsContentProperty(instance, memberInfo);
        //  if (value != null)
        //    SetValue(instance, memberInfo, value);
        //  result++;
        //  continue;
        //}
        //else
        //{

        //}
      }
      {
        memberInfo = members.FirstOrDefault(item => item.ValueType.XmlName == startTag.LocalName
        && item.ValueType.XmlNamespace == startTag.Namespace);
        if (memberInfo == null)
        {
          if (KnownTypes.TryGetValue(startTag, out var memberTypeInfo))
          {
            memberInfo = members.FirstOrDefault(item => item.MemberType != null && memberTypeInfo.Type.IsEqualOrSubclassOf(item.MemberType));
            if (memberInfo != null)
            {
              currentMemberTypeInfo = memberTypeInfo;
            }
            else if (instanceAsMemberInfo?.MemberType != null && memberTypeInfo.Type.IsEqualOrSubclassOf(instanceAsMemberInfo.MemberType))
              Debug.Assert(false);
          }
          else
            Debug.Assert(false);
        }
      }
      if (memberInfo != null /*&& !memberInfo.IsAttribute*/)
      {
        var value = ReadElementAsInstanceMember(instance, memberInfo, currentMemberTypeInfo);
        if (value != null)
        {
          SetValue(instance, memberInfo, value);
          result++;
        }
      }
      else
      {
        if (instanceTypeInfo.ContentProperty != null)
        {
          var content = ReadElementAsContentProperty(instance, instanceTypeInfo.ContentProperty);
          if (content != null)
          {
            SetValue(instance, instanceTypeInfo.ContentProperty, content);
            result++;
          }
        }
        else
        {
          if (instanceTypeInfo.IsCollection && instanceTypeInfo.ContentInfo is CollectionContentInfo contentItemInfo &&
             ReadElementAsCollectionItem(instance, instanceTypeInfo, contentItemInfo) != null)
          {
            // Returned content can't be set to any member, but is is added to collection inside the called method.
            result++;
          }
          else
          if (instanceTypeInfo.IsDictionary && instanceTypeInfo.ContentInfo is DictionaryContentInfo dictionaryInfo &&
             ReadElementAsDictionaryItem(instance, instanceTypeInfo, dictionaryInfo) != null)
          {
            // Returned content can't be set to any member, but is is added to collection inside the called method.
            result++;
          }
          else
          {
            //if (startTag.Name == "CompatibilitySetting")
            //  Debug.Assert(true);
            if (OnUnknownMember != null)
              OnUnknownMember.Invoke(instance, startTag.ToString());
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
                  } while (!Reader.IsEndElement(startTag));
                  Reader.Read();
                }
                continue;
              }
              throw new XmlInvalidOperationException(
                $"No instance member to read and no type found for element \"{startTag}\" in type {aType.FullName}", Reader);
            }
          }
        }
      }
      SkipWhitespaces();
    }
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadXmlElements>");
#endif
    return result;
  }

  /// <summary>
  /// Tries to read an XmlElement which has a property syntax as an instance Property.
  /// </summary>
  /// <param name="instance"></param>
  /// <param name="instanceTypeInfo"></param>
  /// <param name="memberInfo"></param>
  /// <returns>Object read (or null)</returns>
  public object? ReadElementAsProperty(object instance, SerializationTypeInfo instanceTypeInfo, SerializationMemberInfo memberInfo)
  {
    object? memberValue = null;
    var propertyTag = Reader.Name;
    if (Reader.HasAttributes)
    {
      var typeName = Reader.GetAttribute(new XmlQualifiedTagName("type", MappingHelper.xsiNamespace));
      if (typeName != null)
      {
        Debug.Assert(false);
        //var nodeType = Reader.NodeType;
        //var valueType = Reader.ValueType;
        //var nodeValue = Reader.Value;
        //var isSealed = memberInfo.MemberType?.IsSealed ?? false;
        memberValue = ReadElementAsInstanceMember(instance, memberInfo);
      }
      else
      {
        if (!memberInfo.ValueType.HasKnownConstructor)
        {
          throw new InvalidOperationException($"Type {memberInfo.ValueType} has no public, parameterless constructor");
        }
        else
        {
          memberValue = memberInfo.ValueType.KnownConstructor!.Invoke([]);
          memberValue = ReadMemberObjectInterior(memberValue, memberInfo, memberInfo.ValueType);
        }
      }
    }
    else
    if (Reader.IsEmptyElement)
    {
      if (memberInfo.ValueType.HasKnownConstructor)
        memberValue = memberInfo.ValueType.KnownConstructor?.Invoke([]);
      Reader.Read();
    }
    else
    {
      Reader.ReadStartElement(propertyTag);
      if (Reader.NodeType == XmlNodeType.Text)
      {
        if (memberInfo.ValueType.TypeConverter != null)
          memberValue = ReadValueWithTypeConverter(instance, memberInfo.ValueType.Type, memberInfo.ValueType.TypeConverter, memberInfo);
        else
          memberValue = ReadValue(instance, memberInfo);
      }
      else
      {
        if (!memberInfo.CanWrite)
        {
          memberValue = memberInfo.GetValue(instance);
          if (memberValue == null)
            throw new InvalidOperationException($"Readonly member {memberInfo.XmlName} must be not null");
        }
        else
        if (memberInfo.ValueType.Type.IsArray)
        {
          var collectionInfo = memberInfo.GetCollectionInfo();
          if (collectionInfo == null)
            throw new InvalidOperationException($"Collection {memberInfo.ValueType} has no collectionInfo");

          var itemType = memberInfo.ValueType.Type.GetElementType() ?? typeof(object);
          Type collectionType = typeof(Collection<>).MakeGenericType(itemType);

          // Create an instance of the constructed type
          var collection = Activator.CreateInstance(collectionType)
                 ?? throw new InvalidOperationException($"Could not create an instance of {collectionType}");
          var addMethod = collectionType.GetMethod("Add");
          if (addMethod == null)
            throw new InvalidOperationException($"Could not find Add method of {collectionType}");
          foreach (var knownItem in collectionInfo.KnownItemTypes)
            knownItem.AddMethod = addMethod;

          ReadElementsAsPropertiesOrMembers(collection, memberInfo.ValueType, collectionInfo);
          var collectionAsList = (System.Collections.ICollection)collection;
          Array array = Array.CreateInstance(itemType, collectionAsList.Count);

          // Copy items from the collection to the array
          collectionAsList.CopyTo(array, 0);
          memberInfo.SetValue(instance, array);
        }
        else
        {
          if (!memberInfo.ValueType.HasKnownConstructor)
            throw new InvalidOperationException($"Type {memberInfo.ValueType} has no public, parameterless constructor");
          memberValue = memberInfo.ValueType.KnownConstructor!.Invoke([]);
          if (memberInfo.IsCollection)
          {
            Debug.Assert(memberValue != null);
            var collectionInfo = memberInfo.GetCollectionInfo();
            if (collectionInfo == null)
              throw new InvalidOperationException($"Collection {memberInfo.ValueType} has no collectionInfo");
            memberValue = ReadElementsAsPropertiesOrMembers(memberValue!, memberInfo.ValueType, collectionInfo);
          }
          else
          {
            var contentInfo = memberInfo.ContentInfo;
            memberValue = ReadElementsAsPropertiesOrMembers(memberValue, memberInfo.ValueType, contentInfo);
          }
        }
      }
      Reader.ReadEndElement(propertyTag);
    }
    return memberValue;
  }

  /// <summary>
  /// Method invoked for a sequence of child elements of an instance. These child elements can be item elements or properties.
  /// </summary>
  /// <param name="instance"></param>
  /// <param name="instanceTypeInfo"></param>
  /// <param name="contentInfo"></param>
  /// <exception cref="InvalidOperationException"></exception>
  public object? ReadElementsAsPropertiesOrMembers(object instance, SerializationTypeInfo instanceTypeInfo,
   ContentInfo? contentInfo)
  {
    if (instance.GetType().Name == "QualityNodeCollection")
      Debug.Assert(true);
    object? result = instance;
    while (Reader.NodeType == XmlNodeType.Element)
    {
      var innerTag = Reader.Name;
      if (innerTag.LocalName == "QualityMeasure")
        Debug.Assert(true);
      if (contentInfo != null && FindItemTypeInfo(contentInfo, innerTag, out var itemTypeInfo, out var typeInfo))
      {
        if (itemTypeInfo.AddMethod == null)
          throw new InvalidOperationException($"{instanceTypeInfo} has no add method for {itemTypeInfo} values");
        var item = ReadElementWithKnownItemInfo(instance, itemTypeInfo, typeInfo);
        if (item != null)
          itemTypeInfo.AddMethod.Invoke(instance, [item]);
      }
      else
      if (instanceTypeInfo.KnownMembers.TryGetValue(innerTag.LocalName, out var innerMemberInfo))
      {
        var memberValue = ReadElementAsProperty(instance, instanceTypeInfo, innerMemberInfo);
        if (memberValue != null && innerMemberInfo.CanWrite)
          innerMemberInfo.SetValue(instance, memberValue);
      }
      else
      {
        if (!KnownTypes.TryGetValue(innerTag, out var innerTypeInfo))
          throw new InvalidOperationException($"Type {innerTag} not registered");
        //if (!innerTypeInfo.Type.IsEqualOrSubclassOf(instanceTypeInfo.Type))
        //  throw new InvalidOperationException($"Type {innerTypeInfo.Type} is not compatible with {instanceTypeInfo.Type}");
        var memberValue = ReadObjectWithKnownTypeInfo(instance, innerTypeInfo);
        if (memberValue != null)
        {
          if (contentInfo != null && contentInfo.KnownItemTypes.TryGetValue(innerTag, out var itemTypeInfo2))
          {
            if (itemTypeInfo2.AddMethod == null)
              throw new InvalidOperationException($"{instanceTypeInfo} has no add method for {itemTypeInfo2} values");
            itemTypeInfo2.AddMethod.Invoke(instance, [memberValue]);
          }
          else
            result = memberValue;
        }
      }
    }
    //        }
    //        else
    //        {
    //          var innerTag = Reader.Name;
    //          if (memberInfo.ValueType.KnownMembers.TryGetValue(innerTag.Name, out var innerMemberInfo))
    //          {
    //            ReadElementAsProperty(memberValue!, memberInfo.ValueType, innerMemberInfo);
    //          }
    //          else
    //          if (KnownTypes.TryGetValue(innerTag, out var innerTypeInfo))
    //          {
    //            if (innerTypeInfo.Type.IsEqualOrSubclassOf(memberInfo.ValueType.Type))
    //              memberValue = ReadObjectWithKnownTypeInfo(instance, memberInfo.ValueType);
    //            else
    //              throw new InvalidOperationException($"Type {innerTypeInfo.Type} is not compatible with {memberInfo.ValueType.Type}");
    //          }
    //          else
    //          {
    //            throw new InvalidOperationException($"Type {innerTag} not registered");
    //          }
    //        }
    //      }
    //    }
    //    else
    //      Debug.Assert(false);
    //    if (Reader.IsEndElement(propertyTag))
    //      Reader.ReadEndElement(propertyTag);
    //  }
    return result;
  }

  private bool FindItemTypeInfo(ContentInfo contentInfo, XmlQualifiedTagName innerTag, 
    [NotNullWhen(true)] out SerializationItemInfo? itemTypeInfo, [NotNullWhen(true)] out SerializationTypeInfo? typeInfo)
  {
    foreach (var itemType in contentInfo.KnownItemTypes)
    {
      if (itemType.TypeInfo.KnownSubtypes != null)
        foreach (var subType in itemType.TypeInfo.KnownSubtypes)
        {
          var tagName = innerTag.QualifiedName;
          var subTypeName = subType.QualifiedName;
          if (subTypeName.Equals(tagName))
          {
            itemTypeInfo = itemType;
            typeInfo = subType;
            return true;
          }
        }
    }
    itemTypeInfo = null;
    typeInfo = null;
    return false;
  }
  /// <summary>
  /// Reads Xml element as known member of the instance.
  /// </summary>
  /// <param name="instance">Instance which member is to be read</param>
  /// <param name="memberInfo">Member info of the member</param>
  /// <param name="currentMemberTypeInfo">Optional current TypeInfo of the member (if known).</param>
  /// <returns>Read member value (can be null)</returns>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// If a member has an XmlConverter assigner, then it is used to read the value.
  /// Otherwise, a member can be read as a collection (or dictionary) by invoking <see cref="ReadElementAsMemberCollection"/>,
  /// however a collection may have its own properties, and then should read as a "normal" object.
  /// Other, "normal" object are read by invoking <see cref="ReadElementAsMemberObject"/>.
  /// </remarks>
  public object? ReadElementAsInstanceMember(object instance, SerializationMemberInfo memberInfo, SerializationTypeInfo? currentMemberTypeInfo = null)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsInstanceMember instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result;
    Type memberType = currentMemberTypeInfo?.Type ?? memberInfo.MemberType ?? typeof(Object);
    if (memberInfo.XmlConverter?.CanRead == true)
    {
      result = memberInfo.XmlConverter.ReadXml(instance, Reader, memberInfo.ValueType.Type ?? typeof(object), null, this);
    }
    else
    {
      SerializationTypeInfo instanceTypeInfo = memberInfo.ParentType;
      if (!memberInfo.IsObject && (memberInfo.IsCollection || memberInfo.IsDictionary)
                               && (memberInfo.ContentInfo ?? memberInfo.ValueType.ContentInfo ?? instanceTypeInfo.ContentInfo) is CollectionContentInfo contentInfo)
        result = ReadElementAsMemberCollection(instance, memberInfo, contentInfo);
      else
        result = ReadElementAsMemberObject(instance, memberInfo, currentMemberTypeInfo);
    }
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsInstanceMember>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an XML element as an object which will be assigned to the instance content property.
  /// </summary>
  /// <param name="instance">Instance which content property is to be read</param>
  /// <param name="memberInfo">Member info of the member which represents a ContentProperty</param>
  /// <returns>Read value (can be null)</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on invalid type mapping.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the object.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// After validation of types, simply <see cref="ReadMemberObjectWithKnownType"/>
  /// </remarks>
  public object? ReadElementAsContentProperty(object instance, SerializationMemberInfo memberInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsContentProperty instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInvalidOperationException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var name = Reader.Name;
    SerializationTypeInfo? valueTypeInfo = null;
    valueTypeInfo = memberInfo.ValueType;
    if (valueTypeInfo == null)
      throw new XmlInvalidOperationException($"Unknown type for element \"{name}\" on deserialize", Reader);
    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType.IsNullable(out var baseType))
      expectedType = baseType;
    if (expectedType == null)
      throw new XmlInvalidOperationException($"Element \"{name}\" is mapped to {valueTypeInfo.Type.Name}" +
                                             $" but expected type is null", Reader);
    if (!valueTypeInfo.Type.IsEqualOrSubclassOf(expectedType))
      throw new XmlInvalidOperationException($"Element \"{name}\" is mapped to {valueTypeInfo.Type.Name}" +
                                             $" but {expectedType.Name} or its subclass expected", Reader);

    var result = ReadMemberObjectWithKnownType(instance, memberInfo, valueTypeInfo);
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsContentProperty>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an XML element as an object which is added to the instance as a collection (or dictionary).
  /// </summary>
  /// <param name="instance">Instance which must be a collection (or dictionary) to add a value</param>
  /// <param name="instanceTypeInfo">TypeInfo of the instance</param>
  /// <param name="contentItemInfo">Info of the known item types.</param>
  /// <returns>Read item object (or null if item type could not be recognized)</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on collection (or dictionary) error.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the collection item.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// First, serialization item info is searched for the Xml element name. 
  /// If it is not found, then it can be created temporarily basing on start tag name.
  /// Next an item element is read. 
  /// If the collection is dictionary, then a pair of key-item is read using <see cref="ReadElementAsKVPair"/> method.
  /// Otherwise, an item is read by invoking <see cref="ReadElementWithKnownItemInfo"/>.
  /// At the end the item (or key-item pair) is added to the instance with an Add method.
  /// The Add method can be declared in knownItemTypeInfo or localized in the type of instance using type reflection.
  /// </remarks>
  public object? ReadElementAsCollectionItem(object instance, SerializationTypeInfo instanceTypeInfo, CollectionContentInfo contentItemInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsCollectionItem instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? item = null;

    #region serialization item info is searched
    var startTagName = Reader.Name;
    if (!FindItemTypeInfo(contentItemInfo, startTagName.LocalName, out var knownItemTypeInfo, out var typeInfo))
    {
      if (TryGetTypeInfo(startTagName, out var itemTypeInfo))
        knownItemTypeInfo = new SerializationItemInfo(startTagName.LocalName, itemTypeInfo);
    }
    #endregion

    if (knownItemTypeInfo != null)
    {
      if (typeInfo==null)
        typeInfo = knownItemTypeInfo.TypeInfo;
      #region item (or key-item pair) is read
      object? key = startTagName.LocalName;
      if (knownItemTypeInfo.DictionaryInfo != null)
      {
        var collectionInfo = instanceTypeInfo.ContentInfo;
        if (collectionInfo is DictionaryContentInfo dictionaryInfo)
        {
          (key, item) = ReadElementAsKVPair(instance, null, null, dictionaryInfo);
        }
        else
          throw new InvalidOperationException($"TypeInfo({instanceTypeInfo.Type}).CollectionInfo must be a DictionaryInfo");
      }
      else
        item = ReadElementWithKnownItemInfo(instance, knownItemTypeInfo, typeInfo);
      if (item == null)
        throw new XmlInvalidOperationException($"Item of type {knownItemTypeInfo.Type} could not be read at \"{startTagName}\"", Reader);
      if (item.GetType() == typeof(object))
        item = null;
      if (key == null)
        throw new XmlInvalidOperationException($"Key for {item} must be not null", Reader);
      #endregion

      #region item (or key-item pair) is added to the collection
      if (knownItemTypeInfo.AddMethod != null)
      {
        if (instance is IDictionary dictionaryObj)
          knownItemTypeInfo.AddMethod.Invoke(instance, [key, item]);
        else
          knownItemTypeInfo.AddMethod.Invoke(instance, [item]);
      }
      else
      {
        if (instance is IDictionary dictionaryObj)
          dictionaryObj.Add(key, item);
        else
        {
          var collectionType = instance.GetType();
          var addMethod = collectionType.GetMethod("Add", [knownItemTypeInfo.Type]);
          if (addMethod == null)
            addMethod = collectionType.GetMethod("Add");
          if (addMethod != null)
            addMethod.Invoke(instance, [item]);
          else if (instance is ICollection collectionObj)
            throw new XmlInvalidOperationException($"Add method for {knownItemTypeInfo.Type} value not found in {instanceTypeInfo.Type?.FullName}", Reader);
        }
      }
      #endregion
    }
#if TraceReader
    Trace.WriteLine($"<Return item=\"{item}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsCollectionItem>");
#endif
    return item;
  }

  /// <summary>
  /// Reads an XML element as an object which is added to the instance as a collection (or dictionary).
  /// </summary>
  /// <param name="instance">Instance which must be a collection (or dictionary) to add a value</param>
  /// <param name="instanceTypeInfo">TypeInfo of the instance</param>
  /// <param name="dictionaryInfo">Info of the known item types.</param>
  /// <returns>Read item object (or null if item type could not be recognized)</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on collection (or dictionary) error.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the collection item.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// First, serialization item info is searched for the Xml element name. 
  /// If it is not found, then it can be created temporarily basing on start tag name.
  /// Next an item element is read. 
  /// If the collection is dictionary, then a pair of key-item is read using <see cref="ReadElementAsKVPair"/> method.
  /// Otherwise, an item is read by invoking <see cref="ReadElementWithKnownItemInfo"/>.
  /// At the end the item (or key-item pair) is added to the instance with an Add method.
  /// The Add method can be declared in knownItemTypeInfo or localized in the type of instance using type reflection.
  /// </remarks>
  public object? ReadElementAsDictionaryItem(object instance, SerializationTypeInfo instanceTypeInfo, DictionaryContentInfo dictionaryInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsDictionaryItem instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? key;
    object? value = null;

    #region serialization item info is searched
    var startTagName = Reader.Name;
    #endregion

    #region key-item pair is read
    (key, value) = ReadElementAsKVPair(instance, null, null, dictionaryInfo);
    if (key == null)
      throw new XmlInvalidOperationException($"Key for {value} must be not null", Reader);
    #endregion

    #region key-item pair is added to dictionary
    if (instance is IDictionary dictionaryObj)
      dictionaryObj.Add(key, value);
    //else
    //{
    //  var collectionType = instance.GetType();
    //  var addMethod = collectionType.GetMethod("Add", new Type[] { knownItemTypeInfo.Type });
    //  if (addMethod == null)
    //    addMethod = collectionType.GetMethod("Add");
    //  if (addMethod != null)
    //    addMethod.Invoke(instance, new object?[] { value });
    //  else if (instance is ICollection collectionObj)
    //    throw new XmlInvalidOperationException($"Add method for {knownItemTypeInfo.Type} value not found in {instanceTypeInfo.Type?.FullName}", Reader);
    //}
    #endregion
#if TraceReader
    Trace.WriteLine($"<Return value=\"{value}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsDictionaryItem>");
#endif
    return value;
  }

  /// <summary>
  /// Reads an XML element which is recognized as a member object of the instance.
  /// </summary>
  /// <param name="instance">Instance which member is to be read</param>
  /// <param name="memberInfo">Member info of the member</param>
  /// <param name="currentMemberTypeInfo">Optional current TypeInfo of the member (if known).</param>
  /// <returns>Read object (nay be null)</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on errors.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  public object? ReadElementAsMemberObject(object instance, SerializationMemberInfo memberInfo, SerializationTypeInfo? currentMemberTypeInfo = null)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      return null;
    //throw new XmlInvalidOperationException($"XmlReader must be at XmlElement on deserialize object", Reader);
#if TraceReader
    Trace.WriteLine($"<ReadElementAsMemberObject instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    string? propertyElementName = null;
    var tagName = ReadElementTag();
    if (TagIsProperty(tagName))
    {
      var propInfo = memberInfo.ValueType.MembersAsElements.FirstOrDefault(memberItem => memberItem.XmlName == Reader.LocalName);
      if (propInfo != null)
      {
        memberInfo = propInfo;
        Reader.Read();
        propertyElementName = Reader.LocalName;
      }
    }
    tagName = ReadElementTag();
    SerializationTypeInfo? valueTypeInfo = currentMemberTypeInfo;
    if (valueTypeInfo == null)
    {
      if (memberInfo.ValueType.Type != typeof(object))
        valueTypeInfo = memberInfo.ValueType;
      else
      if (!TryGetTypeInfo(tagName, out valueTypeInfo))
        valueTypeInfo = memberInfo.ValueType;
      if (valueTypeInfo == null)
        throw new XmlInvalidOperationException($"Unknown type for element \"{tagName}\" on deserialize", Reader);
    }
    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType.IsNullable(out var baseType))
      expectedType = baseType;
    if (expectedType == null)
      throw new XmlInvalidOperationException($"Element \"{tagName}\" is mapped to {valueTypeInfo.Type.Name}" +
                                             $" but expected type is null", Reader);
    if (!valueTypeInfo.Type.IsEqualOrSubclassOf(expectedType))
      throw new XmlInvalidOperationException($"Element \"{tagName}\" is mapped to {valueTypeInfo.Type.Name}" +
                                             $" but {expectedType.Name} or its subclass expected", Reader);
    var result = ReadMemberObjectWithKnownType(instance, memberInfo, valueTypeInfo);
    if (propertyElementName != null)
      Reader.ReadEndElement(propertyElementName);
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsMemberObject>");
#endif
    return result;
  }
  #endregion

  #region Read member object with or without known type
  /// <summary>
  /// Reads an object as a member of some instance with known type.
  /// </summary>
  /// <param name="instance">Instance which member should be read.</param>
  /// <param name="memberInfo">Serialization member info.</param>
  /// <param name="valueTypeInfo">Serialization type info for expected value.</param>
  /// <returns>Read object (can be null)</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on errors.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// If expected value type is Object, then <see cref="ReadObject(object?)"/> is invoked.
  /// If it is a simple type or a byte array, then a value is read as a string and then converted 
  /// by using <see cref="ConvertMemberValueFromString(object, SerializationMemberInfo, string?)"/>.
  /// If expected value type has parameterless constructor, then the value object is created
  /// and its content is read by using <see cref="ReadMemberObjectInterior"/>.
  /// Arrays of other item type then byte are not read (exception is thrown).
  /// Types which have no parameterless constructor are read by  using <see cref="ReadObject(object?)"/>.
  /// </remarks>
  public object? ReadMemberObjectWithKnownType(object instance, SerializationMemberInfo memberInfo, SerializationTypeInfo valueTypeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberObjectWithKnownType instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (valueTypeInfo.Type.Name == "QualityMetrics")
      Debug.Assert(true);
    object? result = null;
    if (valueTypeInfo.XmlConverter?.CanRead == true)
      result = valueTypeInfo.XmlConverter.ReadXml(instance, Reader, valueTypeInfo.Type, null, null);
    else
    {
      if (valueTypeInfo.Type == typeof(object))
      {
        result = ReadObject(instance);
      }
      else if (valueTypeInfo.KnownConstructor == null)
      {
        if (valueTypeInfo.Type.IsSimple() || valueTypeInfo.Type.IsArray(out var itemType) && itemType == typeof(byte))
        {
          Reader.Read();
          var str = Reader.ReadString();
          result = ConvertMemberValueFromString(instance, memberInfo, str);
          Reader.Read();
        }
        else if (valueTypeInfo.Type.IsArray)
        {
          // TODO: implement any array deserialization.
          throw new XmlInvalidOperationException($"Reading array for type {valueTypeInfo.Type.Name} not implemented", Reader);
        }
        else
        {
          var elementTag = Reader.Name;
          if (TagIsProperty(elementTag))
          {
            Reader.ReadStartElement(elementTag);
            result = ReadObject(instance);
            Reader.ReadEndElement(elementTag);
          }
          else
            result = ReadObject(instance);
        }
      }
      else
      {
        result = valueTypeInfo.KnownConstructor.Invoke([]);
        if (result is IXmlSerializable xmlSerializable)
          xmlSerializable.ReadXml(Reader.BaseXmlReader);
        else
        if (Reader.NodeType == XmlNodeType.Text || Reader.NodeType == XmlNodeType.SignificantWhitespace)
        {
          var str = Reader.ReadString();
          if (valueTypeInfo.TypeConverter != null)
            result = valueTypeInfo.TypeConverter.ConvertFromInvariantString(str);
          else
            result = str;
        }
        else
        if (Reader.NodeType == XmlNodeType.Element && KnownTypes.TryGetValue(Reader.Name, out var elementType)
          && elementType != valueTypeInfo)
          result = ReadElementAsMemberObject(result, memberInfo, elementType);
        else
          result = ReadMemberObjectInterior(result, memberInfo, valueTypeInfo);
      }
    }
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberObjectWithKnownType>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an object as a member of some instance with known type.
  /// </summary>
  /// <param name="instance">Instance which member should be read.</param>
  /// <param name="memberInfo">Serialization member info.</param>
  /// <param name="valueTypeInfo">Serialization type info for expected value.</param>
  /// <returns>Read object (can be null)</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on errors.</exception>
  /// <remarks>Internal method for <see cref="ReadMemberObjectWithKnownType"/>.</remarks>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the object.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  public object? ReadMemberObjectInterior(object instance, SerializationMemberInfo memberInfo, SerializationTypeInfo valueTypeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberObjectInterior instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInvalidOperationException($"XmlReader must be at XmlElement on deserialize object", Reader);
    bool isEmptyElement = Reader.IsEmptyElement;
    ReadXmlAttributes(instance, valueTypeInfo);
    Reader.Read(); // read end of start element and go to its content;
    SkipWhitespaces();
    object result = instance;
    if (!isEmptyElement)
    {
      var n = ReadXmlElements(instance, valueTypeInfo, memberInfo);
      if (n == 0 && (Reader.NodeType == XmlNodeType.Text || Reader.NodeType == XmlNodeType.SignificantWhitespace))
      {
        if (valueTypeInfo.TypeConverter != null)
        {
          var str = Reader.ReadString();
          result = valueTypeInfo.TypeConverter.ConvertFrom(str) ?? instance;
        }
        else
          ReadXmlTextElement(instance, memberInfo);
      }
      if (Reader.NodeType == XmlNodeType.EndElement)
        Reader.Read();
    }
    //}
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberObjectInterior>");
#endif
    return result;
  }

  /// <summary>
  /// A helper method to convert from string to the object using TypeConverter.
  /// Converter is taken from memberInfo.
  /// </summary>
  /// <param name="context">Context object, instance which member is to be read.</param>
  /// <param name="memberInfo">Serialization info for member to be read</param>
  /// <param name="str">Value string to be converted</param>
  /// <returns>Converted object (or entry string if conversion is impossible)</returns>
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

  /// <summary>
  /// Reads an element of collection when a serialization item is known.
  /// Item info is used to get type info of the expected object.
  /// If type info has XmlConverter defined, this converter is used.
  /// Simple types and array of bytes are read by using <see cref="ReadValue"/> method.
  /// Other types are read by using <see cref="ReadObjectInstance"/>.
  /// </summary>
  /// <param name="context">Context object, instance which member is to be read.</param>
  /// <param name="itemInfo">Serialization item info that holds expected type info</param>
  /// <param name="typeInfo">Serialization type info for the expected item</param>
  /// <returns>Read object (can be null)</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on errors</exception>
  /// <remarks>
  /// Internal method for <see cref="ReadElementAsCollectionItem"/>. 
  /// </remarks>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the object.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  protected object? ReadElementWithKnownItemInfo(object context, SerializationItemInfo itemInfo, SerializationTypeInfo typeInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementWithKnownItemInfo instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    object? result = null;
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
        if (Reader.NodeType == XmlNodeType.Text)
        {
          result = ReadValueWithTypeConverter(context, typeInfo.Type, typeInfo.TypeConverter, null);
        }
        else
        if (Reader.NodeType == XmlNodeType.Element)
        {
          var elementTag = Reader.Name;
          Reader.Read();
          result = ReadValueWithTypeConverter(context, typeInfo.Type, typeInfo.TypeConverter, null);
          Reader.ReadEndElement(elementTag);
        }
        else
          throw new XmlInvalidOperationException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", Reader);
      }
      else
        throw new XmlInvalidOperationException($"Can't ReadElementWithKnownItemInfo on {Reader.NodeType}", Reader);
    }
    else
    {
      result = typeInfo.KnownConstructor.Invoke([]);
      if (result is IXmlSerializable xmlSerializable)
        xmlSerializable.ReadXml(Reader.BaseXmlReader);
      else
        result = ReadObjectInstance(result, typeInfo);
    }
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementWithKnownItemInfo>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an element of a dictionary. Element is a key-value pair.
  /// First a reader local name is checked if id 
  /// </summary>
  /// <param name="context">Context object, instance which member is to be read.</param>
  /// <param name="expectedKeyType">Expected key type to validate. If null, then any key type is allowed.</param>
  /// <param name="expectedValueType">Expected value type to validate. If null, then any value type is allowed.</param>
  /// <param name="dictionaryInfo">Dictionary info to recognize an element that represents a key.</param>
  /// <returns>Read a pair of key-value, both of object type.</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on errors.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the object.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  protected (object? key, object? value) ReadElementAsKVPair(object context, Type? expectedKeyType, Type? expectedValueType,
    DictionaryContentInfo dictionaryInfo)
  {
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInvalidOperationException($"XmlReader must be at XmlElement on deserialize object", Reader);
    string keyName = dictionaryInfo.KeyName ?? "Key";
    object? key = null;
    var itemTag = Reader.Name;
    key = Reader.GetAttribute(keyName);
    Reader.Read();
    var valueTag = Reader.Name;
    SerializationTypeInfo? typeInfo = dictionaryInfo.ValueTypeInfo;
    if (dictionaryInfo.KnownItemTypes.Count > 0)
    {
      if (dictionaryInfo.KnownItemTypes.TryGetValue(valueTag, out var typeItemInfo))
      {
        typeInfo = typeItemInfo.TypeInfo;
      }
    }

    if (typeInfo == null)
      throw new XmlInvalidOperationException($"Unknown type for element \"{keyName}\" on deserialize", Reader);
    var foundValueType = typeInfo.Type;
    if (expectedValueType != null)
      if (!foundValueType.IsEqualOrSubclassOf(expectedValueType))
      {
        if (!expectedValueType.IsCollection(foundValueType))
          throw new XmlInvalidOperationException($"Element \"{keyName}\" is mapped to {typeInfo.Type}" +
                                         $" but {expectedValueType} or its subclass expected", Reader);
      }

    var value = ReadObject(context);
    Reader.ReadEndElement(itemTag);
    return (key, value);
  }

  /// <summary>
  /// Internal type kind of collection
  /// </summary>
  internal enum CollectionTypeKind
  {
    Array,
    Collection,
    List,
    Dictionary
  }

  internal record KVPair
  {
    public object? Key { get; set; }
    public object? Value { get; set; }

    public KVPair()
    {
    }

    public KVPair(object? key, object? item)
    {
      Key = key;
      Value = item;
    }

    public void Deconstruct(out object? key, out object? item)
    {
      key = Key;
      item = Value;
    }
  }

  /// <summary>
  /// Reads an XML element, which represents a member of the instance and is recognized as a collection (or dictionary).
  /// </summary>
  /// <param name="instance">Instance which content property is to be read</param>
  /// <param name="memberInfo">Member info of the member which represents a ContentProperty</param>
  /// <param name="contentInfo">Info of the known item types.</param>
  /// <returns>Read collection (or null).</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on errors.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// Member must be of array, collection, list or dictionary type.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// First the kind of collection type is determined and collection attributes are validated.
  /// In the next stage collection items are read to a temporary list. 
  /// If the collection is a dictionary, then <see cref="ReadDictionaryItems"/> method is used.
  /// Otherwise, 
  ///   if the XML element represents the instance of collection, then <see cref="ReadMemberCollectionObject"/> is invoked,
  ///   else <see cref="ReadCollectionItems"/> is used.
  /// In the final stage, either a temporary list is assigned to the result, or items from the temporary list are added to the existing collection.
  /// In both cases, the kind of collection type causes different set/add methods.
  /// </remarks>
  protected object? ReadElementAsMemberCollection(object instance, SerializationMemberInfo memberInfo, CollectionContentInfo contentInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsMemberCollection instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInvalidOperationException($"XmlReader must be at XmlElement on deserialize object", Reader);

    if (memberInfo.ValueType == null)
      throw new XmlInvalidOperationException($"Collection type at property {memberInfo.Member.Name}" +
                                     $" of type {instance.GetType().Name} unknown", Reader);
    var collectionType = memberInfo.MemberType;

    var result = memberInfo.GetValue(instance);
    if (result == null)
    {
      // Check if collection can be written - if not we will not be able to set the property
      if (!memberInfo.CanWrite)
        throw new XmlInvalidOperationException($"Collection at property {memberInfo.Member.Name}" +
                                       $" of type {instance.GetType().Name} is  but readonly", Reader);
    }

    CollectionTypeKind? collectionTypeKind = null;
    Type? itemType;
    Type? valueType = typeof(object);
    Type? keyType = typeof(object);
    if (collectionType == null)
      throw new XmlInvalidOperationException($"Collection type is null", Reader);

    #region determining kind of collection type and validating collection attributes
    if (collectionType.IsArray)
    {
      itemType = collectionType.GetElementType();
      if (itemType == null)
        throw new XmlInvalidOperationException($"Unknown value type of {collectionType} value", Reader);
      collectionTypeKind = CollectionTypeKind.Array;
    }
    else if (collectionType.IsDictionary(out keyType, out valueType))
    {
      if (keyType == null)
        throw new XmlInvalidOperationException($"Unknown key type of {collectionType}", Reader);
      if (valueType == null)
        throw new XmlInvalidOperationException($"Unknown value type of {collectionType}", Reader);
      collectionTypeKind = CollectionTypeKind.Dictionary;
      itemType = valueType;
    }
    else if (collectionType.Name.StartsWith("List`") && collectionType.IsList(out itemType))
    {
      if (itemType == null)
        throw new XmlInvalidOperationException($"Unknown value type of {collectionType}", Reader);
      collectionTypeKind = CollectionTypeKind.List;
    }
    else if (collectionType.IsCollection(out itemType))
    {
      if (itemType == null)
        throw new XmlInvalidOperationException($"Unknown value type of {collectionType}", Reader);
      collectionTypeKind = CollectionTypeKind.Collection;
    }
    else
      throw new XmlInvalidOperationException($"Unrecognized context type kind of {collectionType}", Reader);
    #endregion

    #region reading content items to the temporary list
    List<object> tempList = new List<object>();
    if (itemType == null)
      throw new XmlInvalidOperationException($"Unknown value type of {collectionType} value", Reader);

    if (collectionTypeKind == CollectionTypeKind.Dictionary)
    {
      var dictionaryInfo = memberInfo.ContentInfo as DictionaryContentInfo;
      if (dictionaryInfo == null)
        throw new XmlInvalidOperationException($"Collection of type {collectionType} must be marked with {nameof(XmlDictionaryAttribute)} attribute", Reader);
      if (Reader.IsStartElement(memberInfo.XmlName) && !Reader.IsEmptyElement)
      {
        Reader.Read();
        if (keyType == null)
          throw new XmlInvalidOperationException($"Unknown key type of {collectionType} value", Reader);
        if (valueType == null)
          throw new XmlInvalidOperationException($"Unknown value type of {collectionType} value", Reader);
        ReadDictionaryItems(instance, tempList, keyType, valueType, dictionaryInfo);
      }
    }
    else
    {
      if (contentInfo == null)
        throw new XmlInvalidOperationException($"Collection of type {collectionType} must be marked with {nameof(XmlCollectionAttribute)} attribute", Reader);

      if (Reader.IsStartElement(new XmlQualifiedTagName(memberInfo.XmlName, memberInfo.XmlNamespace)) && !Reader.IsEmptyElement)
      {
        Reader.Read();
        if (memberInfo.MemberType != null && Reader.IsStartElement(new XmlQualifiedTagName(memberInfo.MemberType.Name, memberInfo.MemberType.Namespace)))
          result = ReadMemberCollectionObject(instance, memberInfo, contentInfo);
        else
          ReadCollectionItems(tempList, itemType, contentInfo);
      }
    }
    #endregion

    #region final stage
    if (result == null)
    {
      #region assigning the temporary list to the result.
      switch (collectionTypeKind)
      {
        case CollectionTypeKind.Array:
          var arrayObject = Array.CreateInstance(itemType, tempList.Count);
          for (int i = 0; i < tempList.Count; i++)
            arrayObject.SetValue(tempList[i], i);
          result = arrayObject;
          break;

        case CollectionTypeKind.Collection:
          // We can't use non-generic ICollection interface because implementation of ICollection<T>
          // does implicate implementation of ICollection.
          object? newCollectionObject;
          if (collectionType.IsConstructedGenericType)
          {
            newCollectionObject = Activator.CreateInstance(collectionType);
          }
          else
          {
            var constructor = collectionType.GetConstructor(new Type[0]);
            if (constructor == null)
              throw new XmlInvalidOperationException($"Collection type {collectionType} must have a parameterless public constructor", Reader);
            newCollectionObject = constructor.Invoke(new object[0]);
          }
          if (newCollectionObject == null)
            throw new XmlInvalidOperationException($"Could not create a new instance of {collectionType} value", Reader);

          // ICollection has no Add method so we must localize this method using reflection.
          var addMethod = newCollectionObject.GetType().GetMethod("Add", new Type[] { itemType });
          if (addMethod == null)
            throw new XmlInvalidOperationException($"Could not get \"Add\" method of {collectionType} value", Reader);
          for (int i = 0; i < tempList.Count; i++)
            addMethod.Invoke(newCollectionObject, new object[] { tempList[i] });
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
              throw new XmlInvalidOperationException($"Collection type {collectionType} must have a parameterless public constructor", Reader);
            newListObject = constructor.Invoke(new object[0]) as IList;
          }
          if (newListObject == null)
            throw new XmlInvalidOperationException($"Could not create a new instance of {collectionType} value", Reader);
          for (int i = 0; i < tempList.Count; i++)
            newListObject.Add(tempList[i]);
          result = newListObject;
          break;

        case CollectionTypeKind.Dictionary:
          IDictionary? newDictionaryObject;
          if (collectionType.IsConstructedGenericType)
          {
            Type d1 = typeof(Dictionary<,>);
            if (keyType == null)
              throw new XmlInvalidOperationException($"Unknown key type of {collectionType} value", Reader);
            if (valueType == null)
              throw new XmlInvalidOperationException($"Unknown value type of {collectionType} value", Reader);
            Type[] typeArgs = { keyType, valueType };
            Type newListType = d1.MakeGenericType(typeArgs);
            newDictionaryObject = Activator.CreateInstance(newListType) as IDictionary;
          }
          else
          {
            var constructor = collectionType.GetConstructor([]);
            if (constructor == null)
              throw new XmlInvalidOperationException($"Collection type {collectionType} must have a parameterless public constructor", Reader);
            newDictionaryObject = constructor.Invoke([]) as IDictionary;
          }
          if (newDictionaryObject == null)
            throw new XmlInvalidOperationException($"Could not create a new instance of {collectionType} value", Reader);
          foreach (var item in tempList)
          {
            var kvPair = (KeyValuePair<object, object?>)item;
            newDictionaryObject.Add(kvPair.Key, kvPair.Value);
          }
          //propertyInfo.SetValue(context, newDictionaryObject);
          result = newDictionaryObject;
          break;

        default:
          throw new XmlInvalidOperationException($"Collection type {collectionType} is not implemented for creation", Reader);
      }
      #endregion
    }
    else
    {
      #region add items of the temporary list to the existing collection
      switch (collectionTypeKind)
      {
        case CollectionTypeKind.Array:
          var arrayObject = result as Array;
          if (arrayObject == null)
            throw new XmlInvalidOperationException($"Collection value at property {memberInfo.Member.Name} cannot be typecasted to Array", Reader);
          if (arrayObject.Length == tempList.Count)
            for (int i = 0; i < tempList.Count; i++)
              arrayObject.SetValue(tempList[i], i);
          else if (!memberInfo.CanWrite)
            throw new XmlInvalidOperationException($"Collection at property {memberInfo.Member.Name}" +
                                           $" is an array of different length than number of read items but is readonly and can't be changed",
              Reader);
          var itemArray = Array.CreateInstance(itemType, tempList.Count);
          for (int i = 0; i < tempList.Count; i++)
            itemArray.SetValue(tempList[i], i);
          result = itemArray;
          break;

        case CollectionTypeKind.Collection:
          // We can't cast a collection to non-generic ICollection because implementation of ICollection<T>
          // does implicate implementation of ICollection.
          object? collectionObject = result;
          // ICollection has no Add method so we must localize this method using reflection.
          var addMethod = collectionObject.GetType().GetMethod("Add", new Type[] { itemType });
          if (addMethod == null)
            throw new XmlInvalidOperationException($"Could not get \"Add\" method of {collectionType} value", Reader);
          // We must do the same with Clear method.
          var clearMethod = collectionObject.GetType().GetMethod("Clear");
          if (clearMethod == null)
            throw new XmlInvalidOperationException($"Could not get \"Add\" method of {collectionType} value", Reader);

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
            throw new XmlInvalidOperationException($"Collection value at property {memberInfo.Member.Name} cannot be typecasted to IList", Reader);
          listObject.Clear();
          for (int i = 0; i < tempList.Count; i++)
            listObject.Add(tempList[i]);
          break;

        case CollectionTypeKind.Dictionary:
          IDictionary? dictionaryObject = result as IDictionary;
          if (dictionaryObject == null)
            throw new XmlInvalidOperationException($"Collection value at property {memberInfo.Member.Name} cannot be typecasted to IDictionary", Reader);
          dictionaryObject.Clear();
          for (int i = 0; i < tempList.Count; i++)
          {
            KVPair? kvPair = tempList[i] as KVPair;
            if (kvPair != null && kvPair.Key != null)
              dictionaryObject.Add(kvPair.Key, kvPair.Value);
          }
          break;

        default:
          throw new XmlInvalidOperationException($"Collection type {collectionType} is not implemented for set content", Reader);
      }
      #endregion
    }
    #endregion
    Reader.Read();
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsMemberCollection>");
#endif
    return result;
  }

  /// <summary>
  /// Reads an XML element, which represents a member of the instance that is a collection serialized as the whole object
  /// (with class element). 
  /// </summary>
  /// <param name="instance">Instance which content property is to be read</param>
  /// <param name="memberInfo">Member info of the member which represents a ContentProperty</param>
  /// <param name="contentInfo">Info of the known item types.</param>
  /// <returns>Read collection (or null).</returns>
  /// <exception cref="XmlInvalidOperationException">Thrown on errors.</exception>
  /// <entrystate>
  /// On entry, the Reader is located at the XML start element (or empty element) that represents the instance member.
  /// Member must be of array, collection, list or dictionary type.
  /// </entrystate>
  /// <exitstate>
  /// On exit, the Reader is located after the corresponding XML ending element (or after the entry empty element).
  /// </exitstate>
  /// <remarks>
  /// After checking expected type, the collection object is read. It is realized by using <see cref="XmlConverter"/>
  /// or by invoking <see cref="ReadXmlAttributes"/> and <see cref="ReadCollectionItems"/>.
  /// </remarks>
  protected object? ReadMemberCollectionObject(object instance, SerializationMemberInfo memberInfo, CollectionContentInfo contentInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadMemberCollectionObject instance=\"{instance}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInvalidOperationException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var qualifiedName = ReadElementTag();
    SerializationTypeInfo? typeInfo = null;
    if (memberInfo.ValueType.Type != typeof(object))
      typeInfo = memberInfo.ValueType;
    else
    if (!TryGetTypeInfo(qualifiedName, out typeInfo))
      typeInfo = memberInfo.ValueType;
    if (typeInfo == null)
      throw new XmlInvalidOperationException($"Unknown type for element \"{qualifiedName}\" on deserialize", Reader);

    #region expected type checking
    var expectedType = memberInfo.MemberType ?? typeof(object);

    if (expectedType != null)
    {
      if (expectedType.IsNullable(out var baseType))
        expectedType = baseType;
      if (expectedType == null)
        throw new XmlInvalidOperationException($"Element \"{qualifiedName}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but expected type is null", Reader);
      if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
        throw new XmlInvalidOperationException($"Element \"{qualifiedName}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {expectedType.Name} or its subclass expected", Reader);
    }
    #endregion

    #region collection object reading
    object? result = null;
    if (typeInfo.XmlConverter?.CanRead == true)
      result = typeInfo.XmlConverter.ReadXml(instance, Reader, typeInfo.Type, null, null);
    else
    {
      if (typeInfo.KnownConstructor == null)
      {
        throw new XmlInvalidOperationException($"Collection of type {typeInfo.Type.Name} must have a parameterless constructor", Reader);
      }
      else
      {
        result = typeInfo.KnownConstructor.Invoke([]);
        if (result is IXmlSerializable xmlSerializable && Reader.BaseXmlReader != null)
          xmlSerializable.ReadXml(Reader.BaseXmlReader);
        else
        {
          ReadXmlAttributes(result, typeInfo);
          Reader.Read();
          ReadCollectionItems((ICollection)result, contentInfo.KnownItemTypes.First().TypeInfo.Type, contentInfo);
          Reader.Read();
        }
      }
    }
    #endregion

#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadMemberCollectionObject>");
#endif
    return result;
  }

  /// <summary>
  /// Reads collection items.
  /// </summary>
  /// <param name="context">Collection instance.</param>
  /// <param name="itemType">Type of collection items.</param>
  /// <param name="contentInfo">Info of the known item types.</param>
  /// <returns>Number of read items</returns>
  /// <exception cref="InvalidOperationException">Thrown on errors.</exception>
  /// <remarks>
  /// Add method is get from collection instance. Items are read using <see cref="ReadElementAsItem"/>
  /// and added with the reflected add method.
  /// </remarks>
  public int ReadCollectionItems(object context, Type itemType, CollectionContentInfo contentInfo)
  {
    int itemsRead = 0;
    while (Reader.NodeType == XmlNodeType.Whitespace)
      Reader.Read();
    var collectionType = context.GetType();
    var addMethod = collectionType.GetMethod("Add", [itemType]);
    if (addMethod == null)
      throw new InvalidOperationException($"Add method({itemType}) not found in {collectionType}");
    while (Reader.NodeType == XmlNodeType.Element)
    {
      var item = ReadElementAsItem(context, itemType, contentInfo);
      if (item != null)
        addMethod.Invoke(context, [item]);
      SkipWhitespaces();
    }
    return itemsRead;
  }

  /// <summary>
  /// Reads a single collection item.
  /// </summary>
  /// <param name="context">Collection instance.</param>
  /// <param name="itemType">Type of collection items.</param>
  /// <param name="contentInfo">Info of the known item types.</param>
  /// <returns>Read item object</returns>
  /// <exception cref="InvalidOperationException">Thrown on errors.</exception>
  /// <remarks>
  /// After item type validation, <see cref="ReadObjectWithKnownTypeInfo"/> is called.
  /// </remarks>
  public object? ReadElementAsItem(object context, Type? itemType, CollectionContentInfo contentInfo)
  {
#if TraceReader
    Trace.WriteLine($"<ReadElementAsItem instance=\"{context}\" ReaderName=\"{Reader.Name}\">");
    Trace.IndentLevel++;
#endif
    if (Reader.NodeType != XmlNodeType.Element)
      throw new XmlInvalidOperationException($"XmlReader must be at XmlElement on deserialize object", Reader);
    var elementTag = ReadElementTag();
    SerializationTypeInfo? typeInfo = null;
    if (contentInfo.KnownItemTypes.Any())
    {
      if (contentInfo.KnownItemTypes.TryGetValue(elementTag, out var typeItemInfo))
        typeInfo = typeItemInfo.TypeInfo;
    }
    if (typeInfo == null)
      TryGetTypeInfo(elementTag, out typeInfo);
    if (typeInfo == null)
      throw new XmlInvalidOperationException($"Unknown type for element \"{elementTag}\" on deserialize", Reader);
    if (itemType != null)
      if (!typeInfo.Type.IsEqualOrSubclassOf(itemType))
        throw new XmlInvalidOperationException($"Element \"{elementTag}\" is mapped to {typeInfo.Type.Name}" +
                                       $" but {itemType.Name} or its subclass expected", Reader);
    var result = ReadObjectWithKnownTypeInfo(context, typeInfo);
#if TraceReader
    Trace.WriteLine($"<Return item=\"{result}\" ReaderName=\"{(Reader.IsEndElement() ? "/" : null)}{Reader.Name}\"/>");
    Trace.IndentLevel--;
    Trace.WriteLine($"</ReadElementAsItem>");
#endif
    return result;
  }

  /// <summary>
  /// Reads collection of dictionary items.
  /// </summary>
  /// <param name="context">Original dictionary instance.</param>
  /// <param name="collection">Collection to hold key-value pairs.</param>
  /// <param name="expectedKeyType">Serialization info of expected key type.</param>
  /// <param name="expectedValueType">Serialization info of expected value type.</param>
  /// <param name="dictionaryInfo">Original dictionary info</param>
  /// <returns>Number of read key-value pairs.</returns>
  /// <remarks>
  /// To read a single key-value pair, <see cref="ReadElementAsKVPair"/> is used.
  /// </remarks>
  public int ReadDictionaryItems(object context, ICollection<object> collection, Type expectedKeyType, Type expectedValueType,
    DictionaryContentInfo dictionaryInfo)
  {
    int itemsRead = 0;
    while (Reader.NodeType == XmlNodeType.Element)
    {
      (object? key, object? val) = ReadElementAsKVPair(context, expectedKeyType, expectedValueType, dictionaryInfo);
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
  /// Reads Xml text element and assigns it to the instance.
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
  /// <returns>Read value (can be null)</returns>
  public object? ReadValue(object? context, SerializationMemberInfo memberInfo)
  {
    var memberType = memberInfo.MemberType;
    if (memberType == null)
      return null;
    var typeConverter = memberInfo.GetTypeConverter();
    var value = ReadValueWithTypeConverter(context, memberType, typeConverter, memberInfo);
    return value;
  }

  /// <summary>
  /// Reads a text value according to member info and using expected type and type converter.
  /// </summary>
  /// <param name="context">Context of read (instance or collection) passed to <see cref="XmlConverter"/></param>
  /// <param name="expectedType">Expected type of the value to read. Used to convert read string to the object value.</param>
  /// <param name="typeConverter">
  /// If it is <see cref="XmlConverter"/>, it is used to read the XML. 
  /// Otherwise, is can be used to convert string value to the object value.
  /// </param>
  /// <param name="memberInfo">Serialization member info of the read member</param>
  /// <returns>Read value (can be null)</returns>
  /// <remarks>This method has its own algorithm to convert simple type values from string.</remarks>
  public object? ReadValueWithTypeConverter(object? context, Type expectedType, TypeConverter? typeConverter, SerializationMemberInfo? memberInfo)
  {
    if (Reader.NodeType == XmlNodeType.None)
      return null;
    object? propValue = null;
    if (expectedType.Name.StartsWith("Nullable`1"))
      expectedType = expectedType.GetGenericArguments()[0];
    if (Reader.NodeType != XmlNodeType.Text && Reader.NodeType != XmlNodeType.SignificantWhitespace
      && typeConverter is IXmlConverter xmlConverter && xmlConverter.CanConvert(expectedType))
    {
      propValue = xmlConverter.ReadXml(context, Reader, expectedType, null, this);
      return propValue;
    }
    var str = Reader.ReadString();
    if (str.StartsWith("1.833"))
      Debug.Assert(true);
    if (Reader.NodeType == XmlNodeType.Text)
      Reader.Read();
    str = str.DecodeStringValue();

    if (expectedType == typeof(string))
      propValue = str;
    else if (typeConverter != null)
    {
      if (context != null)
        propValue = typeConverter.ConvertFromInvariantString(new TypeDescriptorContext(context, memberInfo?.Property), str);
      else
        propValue = typeConverter.ConvertFromInvariantString(null!, str);
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
              propValue = typeConverter.ConvertFromInvariantString(null!, str);
            break;
          }
          throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to {expectedType} value", Reader);
      }
    }
    else if (expectedType.IsEnum)
    {
#if NET6_0_OR_GREATER
      if (!Enum.TryParse(expectedType, str, Options.IgnoreCaseOnEnum, out propValue))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to enum value of type {expectedType.Name}", Reader);
#else
      try
      {
        propValue = Enum.Parse(expectedType, str, Options.IgnoreCaseOnEnum);
      }
      catch
      {
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to enum value of type {expectedType.Name}", Reader);
      }
#endif
    }
    else if (expectedType == typeof(int))
    {
      if (!int.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to int value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(uint))
    {
      if (!uint.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to uint value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(long))
    {
      if (!long.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to int64 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(ulong))
    {
      if (!ulong.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to uint64 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(byte))
    {
      if (!byte.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to byte value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(sbyte))
    {
      if (!sbyte.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to signed byte value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(short))
    {
      if (!short.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to int16 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(ushort))
    {
      if (!ushort.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to uint16 value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(float))
    {
      if (!float.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to float value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(double))
    {
      if (!double.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to double value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(decimal))
    {
      if (!decimal.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to decimal value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(DateTime))
    {
      if (!DateTime.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to date/time value", Reader);
      propValue = val;
    }
#if NET6_0_OR_GREATER
    else if (expectedType == typeof(DateOnly))
    {
      if (!DateOnly.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to date-only value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(TimeOnly))
    {
      if (!TimeOnly.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to time-only value", Reader);
      propValue = val;
    }
#endif
    else if (expectedType == typeof(TimeSpan))
    {
      if (!TimeSpan.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to time span value", Reader);
      propValue = val;
    }
    else if (expectedType == typeof(Guid))
    {
      if (!Guid.TryParse(str, out var val))
        throw new XmlInvalidOperationException($"Cannot convert \"{str}\" to guid value", Reader);
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
        throw new XmlInvalidOperationException($"Array type converter not supported", Reader);
    }
    else if (expectedType == typeof(Type))
    {
      propValue = new TypeNameConverter(KnownTypes.Keys, KnownNamespaces.XmlNamespaceToPrefix).ConvertFrom(null, null, str);
    }
    //else
    //  throw new XmlInvalidOperationException($"Value type \"{expectedType}\" not supported for deserialization", reader);
    return propValue;
  }

  /// <summary>
  /// Reads a reference value from XML text according to member info.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="memberInfo"></param>
  /// <returns></returns>
  public string? ReadReferenceValue(object? context, SerializationMemberInfo memberInfo)
  {
    var value = ReadValueWithTypeConverter(context, typeof(string), null, memberInfo);
    return value as string;
  }
  #endregion

  #region Helper methods

  /// <summary>
  /// Tries to get a registered type info for a specified type. 
  /// A type should be registered in <see cref="Mapper"/> object.
  /// </summary>
  /// <param name="type">Searched type.</param>
  /// <param name="typeInfo">Found typeInfo (or null when not found).</param>
  /// <returns>True when typeInfo found, false otherwise.</returns>
  public bool TryGetTypeInfo(Type type, [NotNullWhen(true)] out SerializationTypeInfo? typeInfo)
  {
    return Mapper.KnownTypes.TryGetValue(type, out typeInfo);
  }

  /// <summary>
  /// Tries to get a registered type info for a specified tag name. 
  /// A type should be registered in <see cref="Mapper"/> object.
  /// </summary>
  /// <param name="tag">Searched XML tag.</param>
  /// <param name="typeInfo">Found typeInfo (or null when not found).</param>
  /// <returns>True when typeInfo found, false otherwise.</returns>
  public bool TryGetTypeInfo(XmlQualifiedTagName tag, [NotNullWhen(true)] out SerializationTypeInfo? typeInfo)
  {
    if (String.IsNullOrEmpty(tag.Namespace))
    {
      var aType = TypeNaming.GetType(tag.LocalName);
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
      clrName = new QualifiedName(clrName.LocalName, ns);
      if (Mapper.KnownTypes.TryGetValue(clrName, out typeInfo))
        return true;
    }
    if (String.IsNullOrEmpty(tag.Prefix))
    {
      clrName = new QualifiedName(clrName.LocalName, "System");
      if (Mapper.KnownTypes.TryGetValue(clrName, out typeInfo))
        return true;
    }
    if (Mapper.KnownTypes.TryGetValue(clrName.LocalName, out typeInfo))
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
      var type = TypeNaming.GetType(result.LocalName);
      if (type != null)
        result = new XmlQualifiedTagName(type.FullName ?? "");
    }
    return result;
  }

  /// <summary>
  /// Checks if tag syntax is for property name, i.e. Tag.Namespace is null or empty string.
  /// </summary>
  /// <param name="tag"></param>
  /// <returns></returns>
  public bool TagIsProperty(XmlQualifiedTagName tag)
    => String.IsNullOrEmpty(tag.Namespace);

  /// <summary>
  /// Helper method to set a value to the object instance using member info.
  /// If a member has type converter, the value is translated (when it is possible).
  /// </summary>
  /// <param name="instance">The object instance to which the value is to be assigned.</param>
  /// <param name="memberInfo">Serialization member info of the value.</param>
  /// <param name="value">The value to set (can be null).</param>
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
          value = typeConverter.ConvertFrom(typeDescriptor, null!, value);
      }
    }
    memberInfo.SetValue(instance, value);
  }

  /// <summary>
  /// Helper method to set a value to the object instance using member info.
  /// If a member has type converter, the value is translated (when it is possible).
  /// </summary>
  /// <param name="instance">The object instance to which the value is to be assigned.</param>
  /// <param name="memberInfo">Serialization member info of the value.</param>
  /// <param name="value">The value to set (can't be null).</param>
  public bool AddValue(object instance, SerializationMemberInfo memberInfo, object value)
  {
    var contentInfo = memberInfo.ContentInfo;
    if (contentInfo != null)
    {
      foreach (var knownItem in contentInfo.KnownItemTypes)
      {
        var expectedType = knownItem.TypeInfo.Type;
        var valueType = value.GetType();
        if (valueType != expectedType)
        {
          var typeConverter = memberInfo.TypeConverter;
          if (typeConverter == null && expectedType.IsSimple())
            typeConverter = new ValueTypeConverter(expectedType, KnownTypes.Keys);
          var typeDescriptor = new TypeDescriptorContext(instance);
          if (typeConverter?.CanConvertFrom(typeDescriptor, valueType) == true)
          {
            value = typeConverter.ConvertFrom(typeDescriptor, null!, value)!;
          }
        }
        var collection = memberInfo.GetValue(instance);
        if (collection != null)
        {
          var addMethods = collection.GetType().GetMethods().Where(method => method.Name == "Add").ToArray();
          if (AddWithMethods(collection, addMethods, value))
          {
            return true;
          }
        }
      }
    }
    return false;
  }

  private bool AddWithMethods(object collection, MethodInfo[] addMethods, object value)
  {
    foreach (var addMethod in addMethods)
    {
      var parameters = addMethod.GetParameters();
      if (parameters.Length == 1)
      {
        var paramType = parameters[0].ParameterType;
        if (paramType.IsAssignableFrom(value.GetType()))
        {
          addMethod.Invoke(collection, [value]);
          return true;
        }
      }
      else if (parameters.Length > 1 && value is object[] valueArray && parameters.Length == valueArray.Length)
      {
        bool match = true;
        for (int i = 0; i < parameters.Length; i++)
        {
          var paramType = parameters[i].ParameterType;
          if (!paramType.IsAssignableFrom(valueArray[i].GetType()))
          {
            match = false;
            break;
          }
        }
        if (match)
        {
          addMethod.Invoke(collection, valueArray);
          return true;
        }
      }
    }
    return false;
  }
  #endregion
}