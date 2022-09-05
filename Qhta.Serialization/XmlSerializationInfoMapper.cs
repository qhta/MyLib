namespace Qhta.Xml.Serialization;

/// <summary>
/// The purpose of this class is to build serialization info on types and properties
/// and map xml element or attribute names to this info.
/// </summary>
public partial class XmlSerializationInfoMapper
{
  /// <summary>
  /// To create a mapper you need serialization options.
  /// </summary>
  /// <param name="options">Instance of the serialization options - set only once</param>
  /// <param name="baseNamespace">default namespace for elements</param>
  public XmlSerializationInfoMapper(SerializationOptions options, string? baseNamespace)
  {
    Options = options;
    BaseNamespace = baseNamespace ?? "";
  }

  /// <summary>
  /// A namespace of the main type. If registered type are in the same namespace,
  /// they are indexed without prefixes.
  /// </summary>
  public string BaseNamespace { get; }

  /// <summary>
  /// Only some of the options are used:
  /// <list type="bullet">
  ///  <item>
  ///     <see cref="SerializationOptions.IgnoreTypesWithoutParameterlessConstructor"/>
  ///     <see cref="SerializationOptions.AttributeNameCase"/>
  ///     <see cref="SerializationOptions.ElementNameCase"/>
  ///   </item>
  /// </list>
  /// </summary>
  public SerializationOptions Options { get; }

  /// <summary>
  /// The dictionary of the types known to the serializer.
  /// Types are registered by the name of the type or by <see cref="XmlRootAttribute"/>,
  /// <see cref="XmlCollectionAttribute"/>, or <see cref="XmlDictionaryAttribute"/>.
  /// </summary>
  public KnownTypesDictionary KnownTypes { get; } = new KnownTypesDictionary();

  public SerializationTypeInfo RegisterType(Type aType)
  {
    TryRegisterType(aType, out var result);
    return result;
  }

  /// <summary>
  /// <see cref="SerializationTypeInfo"/> is created for a given type and is registered with a type name 
  /// or name taken from <see cref="XmlRootAttribute"/>,
  /// <see cref="XmlCollectionAttribute"/>, or <see cref="XmlDictionaryAttribute"/>.
  /// </summary>
  /// <param name="aType">Registered type</param>
  /// <param name="result">Newly created serialization type info or the already registered one.</param>
  /// <returns>true if the type info was newly added, false if it was added previously.</returns>
  /// <exception cref="InternalException">thrown if a name is already used for a different type</exception>
  public bool TryRegisterType(Type aType, out SerializationTypeInfo result)
  {
    if (aType.IsNullable(out var baseType))
      aType = baseType;
    #region Checking if a type was already registered
    if (KnownTypes.TryGetValue(aType, out var knownTypeInfo))
    {
      result = knownTypeInfo;
      return false;
    }
    #endregion

    #region Creating and registering a new serialization info
    // first create a new type info
    // and add it to avoid stack overflow with recurrency
    var newTypeInfo = CreateTypeInfo(aType);
    KnownTypes.Add(newTypeInfo);
    // Then fill the type info
    FillTypeInfo(newTypeInfo);
    #endregion

    result = newTypeInfo;
    return true;
  }



  /// <summary>
  /// A method to create type info from a type.
  /// Its name is created looking first for <see cref="XmlRootAttribute"/>.
  /// If it is impossible then next it is create looking for <see cref="XmlCollectionAttribute"/>.
  /// If it is impossible then next it is create looking for<see cref="XmlDictionaryAttribute"/>.
  /// If it is impossible then element name is created from aType name and namespace.
  /// </summary>
  /// <param name="aType"></param>
  /// <returns></returns>
  protected SerializationTypeInfo CreateTypeInfo(Type aType)
  {
    if (aType.IsNullable(out var baseType))
      aType = baseType;
    SerializationTypeInfo typeInfo = new SerializationTypeInfo(aType);
    var xmlRootAttrib = aType.GetCustomAttribute<XmlRootAttribute>(false);
    if (xmlRootAttrib?.ElementName != null)
      typeInfo.Name = new QualifiedName(xmlRootAttrib.ElementName);
    if (typeInfo.Name.IsEmpty())
    {
      var xmlCollectionAttribute = aType.GetCustomAttribute<XmlCollectionAttribute>();
      if (xmlCollectionAttribute != null)
        typeInfo.Name = new QualifiedName(xmlCollectionAttribute.ElementName);
      if (typeInfo.Name.IsEmpty())
      {
        var xmlDictionaryAttribute = aType.GetCustomAttribute<XmlDictionaryAttribute>();
        if (xmlDictionaryAttribute != null)
          typeInfo.Name = new QualifiedName(xmlDictionaryAttribute.ElementName);
        if (typeInfo.Name.IsEmpty())
          typeInfo.Name = new QualifiedName(aType.Name, aType.Namespace);
      }
    }
    return typeInfo;
  }

  /// <summary>
  /// This method fills the <see cref="SerializationTypeInfo"/> parameter 
  /// with data taken from a type.
  /// </summary>
  /// <param name="typeInfo"></param>
  /// <exception cref="InternalException">
  ///   Thrown if a type has no parameterless public constructor 
  ///   and an option <see cref="SerializationOptions.IgnoreTypesWithoutParameterlessConstructor"/> is not set.
  /// </exception>
  protected void FillTypeInfo(SerializationTypeInfo typeInfo)
  {
    var aType = typeInfo.Type;

    if (aType.IsDictionary())
      typeInfo.CollectionInfo = CreateDictionaryInfo(aType);
    else
    if (aType.IsCollection())
      typeInfo.CollectionInfo = CreateCollectionInfo(aType);

    #region Checking and registering a known constructor - optional for simple types
    var constructor = aType.GetConstructor(Type.EmptyTypes);
    if (!aType.IsSimple() && !aType.IsAbstract && !aType.IsEnum && !aType.IsNullable())
    {
      if (constructor == null)
      {
        if (!Options.IgnoreTypesWithoutParameterlessConstructor && aType.IsNullable())
          throw new IOException($"Type {aType.Name} must have a public, parameterless constructor to allow deserialization");
      }
      typeInfo.KnownConstructor = constructor;
    }
    #endregion

    CategorizeProperties(aType, typeInfo);
    SearchShouldSerializeMethods(aType, typeInfo);

    typeInfo.KnownItemTypes = GetKnownItems(aType);
    typeInfo.XmlConverter = GetXmlConverter(aType);
    typeInfo.KnownContentProperty = GetContentProperty(aType);
    typeInfo.KnownTextProperty = GetTextProperty(aType);
    typeInfo.TypeConverter = GetTypeConverter(aType);
  }

  public virtual void CategorizeProperties(Type aType, SerializationTypeInfo typeInfo)
  {
    var props = aType.GetPropertiesByInheritance().Where(item => item.CanRead);
    if (!props.Any())
      return;
    int attrCount = 0;
    int elemCount = 0;
    foreach (var propInfo in props)
    {
      //if (propInfo.Name == "MajorFont")
      //  Debug.Assert(true);
      var xmlAttribute = propInfo.GetCustomAttributes(true).OfType<XmlAttributeAttribute>().FirstOrDefault();
      if (xmlAttribute != null)
        TryAddPropertyAsAttribute(typeInfo, propInfo, xmlAttribute, attrCount++);
      else
      {
        var elementAttribute = propInfo.GetCustomAttributes(true).OfType<XmlElementAttribute>().FirstOrDefault();
        if (elementAttribute != null)
          TryAddPropertyAsElement(typeInfo, propInfo, elementAttribute, elemCount++);
        else
        {
          var dictionaryAttribute = propInfo.GetCustomAttributes(true).OfType<XmlDictionaryAttribute>().FirstOrDefault();
          if (dictionaryAttribute != null)
            TryAddPropertyAsDictionary(typeInfo, propInfo, dictionaryAttribute, elemCount++);
          else
          {
            var collectionAttribute = propInfo.GetCustomAttributes(true).OfType<XmlCollectionAttribute>().FirstOrDefault();
            if (collectionAttribute != null)
              TryAddPropertyAsCollection(typeInfo, propInfo, collectionAttribute, elemCount++);
            else
            {
              if (propInfo.PropertyType.IsDictionary())
              {
                TryAddPropertyAsDictionary(typeInfo, propInfo, null, elemCount++);
              }
              else if (propInfo.PropertyType.IsCollection())
              {
                if (!aType.IsDictionary() || propInfo.Name != "Keys" && propInfo.Name != "Values")
                  TryAddPropertyAsCollection(typeInfo, propInfo, null, elemCount++);
              }
              else
              if (Options.AcceptAllNotIgnoredProperties
                  && !propInfo.GetCustomAttributes(true).OfType<XmlIgnoreAttribute>().Any())
              {
                if (propInfo.GetIndexParameters().Length == 0)
                {
                  if (propInfo.PropertyType.IsSimple())
                  {
                    if (propInfo.CanWrite)
                    {
                      if (Options.AcceptSimplePropertiesAsAttributes)
                        TryAddPropertyAsAttribute(typeInfo, propInfo, null, attrCount++);
                      else
                        TryAddPropertyAsElement(typeInfo, propInfo, null, elemCount++);
                    }
                  }
                  else
                    //if (propInfo.PropertyType.IsDictionary())
                    //  TryAddPropertyAsDictionary(typeInfo, propInfo, null, elemCount++);
                    //else
                    //if (propInfo.PropertyType.IsCollection())
                    //  TryAddPropertyAsCollection(typeInfo, propInfo, null, elemCount++);
                    //else
                  if (propInfo.CanWrite)
                    TryAddPropertyAsElement(typeInfo, propInfo, null, elemCount++);
                }
              }
            }
          }
        }
      }
    }
  }


  /// <summary>
  /// Adds property to typeInfo.PropsAsAttributes
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="propInfo">Selected property Info</param>
  /// <param name="xmlAttribute">Found XmlAttributeAttribute</param>
  /// <param name="order">default order</param>
  protected virtual bool TryAddPropertyAsAttribute(SerializationTypeInfo typeInfo, PropertyInfo propInfo, XmlAttributeAttribute? xmlAttribute, int order)
  {
    var attrName = xmlAttribute?.AttributeName;
    if (string.IsNullOrEmpty(attrName))
      attrName = propInfo.Name;
    if (Options.AttributeNameCase != 0)
      attrName = QXmlSerializer.ChangeCase(attrName, Options.AttributeNameCase);
    if (typeInfo.PropsAsAttributes.ContainsKey((attrName)))
      return false;
    if (xmlAttribute is XmlOrderedAttribAttribute attr2 && attr2.Order != null)
      order = (int)attr2.Order;
    var serializePropInfo = new SerializationPropertyInfo(attrName, propInfo, order);
    if (propInfo.GetCustomAttribute<XmlReferenceAttribute>()!=null)
      serializePropInfo.IsReference = true;
    if (TryRegisterType(propInfo.PropertyType, out var propTypeInfo))
      serializePropInfo.TypeInfo = propTypeInfo;
    var converterTypeName = propInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
    if (converterTypeName != null)
      serializePropInfo.TypeConverter = FindTypeConverter(converterTypeName);
    var converterType = propInfo.GetCustomAttribute<XmlTypeConverterAttribute>()?.ConverterType;
    if (converterType != null)
      serializePropInfo.TypeConverter = CreateTypeConverter(converterType);
    typeInfo.PropsAsAttributes.Add(attrName, serializePropInfo);
    return true;
  }

  /// <summary>
  /// Adds property to typeInfo.PropsAsElements
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="propInfo">Selected property Info</param>
  /// <param name="xmlAttribute">Found XmlElementAttribute</param>
  /// <param name="order">default order</param>
  protected virtual bool TryAddPropertyAsElement(SerializationTypeInfo typeInfo, PropertyInfo propInfo, XmlElementAttribute? xmlAttribute, int order)
  {
    var elemName = xmlAttribute?.ElementName;
    if (string.IsNullOrEmpty(elemName))
      elemName = propInfo.Name;
    if (Options.ElementNameCase != 0)
      elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);
    if (typeInfo.PropsAsElements.ContainsKey(elemName))
      return false;
    if (xmlAttribute?.Order > 0)
      order = xmlAttribute.Order;
    var serializePropInfo = new SerializationPropertyInfo(elemName, propInfo, order);
    if (propInfo.GetCustomAttribute<XmlReferenceAttribute>() != null)
      serializePropInfo.IsReference = true;
    if (TryRegisterType(propInfo.PropertyType, out var propTypeInfo))
      serializePropInfo.TypeInfo = propTypeInfo;
    var converterTypeName = propInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
    if (converterTypeName != null)
      serializePropInfo.TypeConverter = FindTypeConverter(converterTypeName);
    var converterType = propInfo.GetCustomAttribute<XmlTypeConverterAttribute>()?.ConverterType;
    if (converterType != null)
      serializePropInfo.TypeConverter = CreateTypeConverter(converterType);
    typeInfo.PropsAsElements.Add(elemName, serializePropInfo);
    return true;
  }

  /// <summary>
  /// Adds property to typeInfo.PropsAsElements with DictionaryInfo
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="propInfo">Selected property Info</param>
  /// <param name="xmlAttribute">Found XmlDictionaryAttribute</param>
  /// <param name="order">default order</param>
  protected virtual bool TryAddPropertyAsDictionary(SerializationTypeInfo typeInfo, PropertyInfo propInfo, XmlDictionaryAttribute? xmlAttribute, int order)
  {
    var elemName = xmlAttribute?.ElementName;
    if (string.IsNullOrEmpty(elemName))
      elemName = propInfo.Name;
    if (Options.ElementNameCase != 0)
      elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);
    if (typeInfo.PropsAsElements.ContainsKey(elemName))
      return false;
    if (xmlAttribute?.Order > 0)
      order = xmlAttribute.Order;
    var serializePropInfo = new SerializationPropertyInfo(elemName, propInfo, order);
    if (propInfo.GetCustomAttribute<XmlReferenceAttribute>() != null)
      serializePropInfo.IsReference = true;
    serializePropInfo.TypeInfo?.PropsAsElements?.Clear();
    serializePropInfo.CollectionInfo = CreateDictionaryInfo(propInfo);
    if (TryRegisterType(propInfo.PropertyType, out var propTypeInfo))
      serializePropInfo.TypeInfo = propTypeInfo;
    var converterTypeName = propInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
    if (converterTypeName != null)
      serializePropInfo.TypeConverter = FindTypeConverter(converterTypeName);
    var converterType = propInfo.GetCustomAttribute<XmlTypeConverterAttribute>()?.ConverterType;
    if (converterType != null)
      serializePropInfo.TypeConverter = CreateTypeConverter(converterType);
    typeInfo.PropsAsElements.Add(elemName, serializePropInfo);
    return true;
  }

  /// <summary>
  /// Adds property to typeInfo.PropsAsElements with CollectionInfo
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="propInfo">Selected property Info</param>
  /// <param name="xmlAttribute">Found XmlCollectionAttribute</param>
  /// <param name="order">default order</param>
  protected virtual bool TryAddPropertyAsCollection(SerializationTypeInfo typeInfo, PropertyInfo propInfo, XmlCollectionAttribute? xmlAttribute, int order)
  {
    var elemName = xmlAttribute?.ElementName;
    if (string.IsNullOrEmpty(elemName))
      elemName = propInfo.Name;
    if (Options.ElementNameCase != 0)
      elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);
    if (typeInfo.PropsAsElements.ContainsKey(elemName))
      return false;
    if (xmlAttribute?.Order > 0)
      order = xmlAttribute.Order;
    var serializePropInfo = new SerializationPropertyInfo(elemName, propInfo, order);
    if (propInfo.GetCustomAttribute<XmlReferenceAttribute>() != null)
      serializePropInfo.IsReference = true;
    serializePropInfo.CollectionInfo = CreateCollectionInfo(propInfo);
    if (TryRegisterType(propInfo.PropertyType, out var propTypeInfo))
    {
      propTypeInfo.CollectionInfo = serializePropInfo.CollectionInfo;
      serializePropInfo.TypeInfo = propTypeInfo;
    }
    var converterTypeName = propInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
    if (converterTypeName != null)
      serializePropInfo.TypeConverter = FindTypeConverter(converterTypeName);
    var converterType = propInfo.GetCustomAttribute<XmlTypeConverterAttribute>()?.ConverterType;
    if (converterType != null)
      serializePropInfo.TypeConverter = CreateTypeConverter(converterType);
    typeInfo.PropsAsElements.Add(elemName, serializePropInfo);
    return true;
  }

  #region Collection Handling
  protected CollectionInfo CreateCollectionInfo(Type aType)
  {
    return CreateCollectionInfo(aType, aType.GetCustomAttributes(true).OfType<XmlArrayItemAttribute>().ToArray());
  }

  protected CollectionInfo CreateCollectionInfo(PropertyInfo propertyInfo)
  {
    var result = CreateCollectionInfo(propertyInfo.PropertyType, propertyInfo.GetCustomAttributes(true).OfType<XmlArrayItemAttribute>().ToArray());
    if (propertyInfo.GetCustomAttribute<XmlReferencesAttribute>() != null)
      result.IsReferences = true;
    return result;
  }

  protected CollectionInfo CreateCollectionInfo(Type aType, IEnumerable<XmlArrayItemAttribute> arrayItemAttribs)
  {
    var collectionTypeInfo = new CollectionInfo();
    if (arrayItemAttribs.Count() != 0)
    {
      foreach (var arrayItemAttribute in arrayItemAttribs)
      {
        var elemName = arrayItemAttribute.ElementName;
        var itemType = arrayItemAttribute.Type;
        if (string.IsNullOrEmpty(elemName))
        {
          if (itemType == null)
            throw new InternalException(
              $"Element name or type must be specified in ArrayItemAttribute in specification of {aType} type");
          elemName = itemType.Name;
        }
        else
        if (Options.ElementNameCase != 0)
          elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);

        if (itemType == null)
          itemType = typeof(object);
        var serializationItemTypeInfo = new SerializationItemTypeInfo(elemName, RegisterType(itemType));
        RegisterType(itemType).KnownItemTypes.Add(elemName, serializationItemTypeInfo);
      }
    }
    else if (aType.IsCollection(out var itemType))
    {
      var itemTypeInfo = RegisterType(itemType);
      collectionTypeInfo.KnownItemTypes.Add(new SerializationItemTypeInfo(itemTypeInfo.Name, itemTypeInfo));
    }
    return collectionTypeInfo;
  }
  #endregion

  #region DictionaryHandling
  protected DictionaryInfo CreateDictionaryInfo(Type aType)
  {
    if (aType.Name == "KnownTypesDictionary")
      TestTools.Stop();
    return CreateDictionaryInfo(aType, aType.GetCustomAttributes(true).OfType<XmlDictionaryItemAttribute>().ToArray());
  }

  protected DictionaryInfo CreateDictionaryInfo(PropertyInfo propertyInfo)
  {
    var result = CreateDictionaryInfo(propertyInfo.PropertyType, propertyInfo.GetCustomAttributes(true).OfType<XmlDictionaryItemAttribute>().ToArray());
    if (propertyInfo.GetCustomAttribute<XmlReferencesAttribute>() != null)
      result.IsReferences = true;
    return result;
  }
  protected DictionaryInfo CreateDictionaryInfo(Type aType, IEnumerable<XmlDictionaryItemAttribute> dictItemAttribs)
  {
    if (aType.Name == "KnownTypesDictionary")
      TestTools.Stop();
    var dictionaryTypeInfo = new DictionaryInfo();
    if (dictItemAttribs.Count() != 0)
    {
      foreach (var dictItemAttribute in dictItemAttribs)
      {
        var elemName = dictItemAttribute.ElementName;
        var itemType = dictItemAttribute.Type;
        if (string.IsNullOrEmpty(elemName))
        {
          if (itemType == null)
            throw new InternalException(
              $"Element name or type must be specified in ArrayItemAttribute in specification of {aType} type");
          elemName = itemType.Name;
        }
        else
        if (Options.ElementNameCase != 0)
          elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);

        if (itemType == null)
          itemType = typeof(object);
        var serializationItemTypeInfo = new SerializationItemTypeInfo(elemName, RegisterType(itemType));
        RegisterType(itemType).KnownItemTypes.Add(elemName, serializationItemTypeInfo);
      }
    }
    else if (aType.IsDictionary(out var keyType, out var valType))
    {
      dictionaryTypeInfo.KeyTypeInfo = RegisterType(keyType);
      var itemTypeInfo = RegisterType(valType);
      dictionaryTypeInfo.KnownItemTypes.Add(new SerializationItemTypeInfo(itemTypeInfo.Name, itemTypeInfo));
    }
    return dictionaryTypeInfo;
  }
  #endregion

  /// <summary>
  /// Registers a property which is indended to be serialized as a text content of the Xml element.
  /// This is the first property which is marked with <see cref="System.Xml.Serialization.XmlTextAttribute"/>.
  /// Note that only the first found property is used. If others are found, an exception is thrown.
  /// </summary>
  /// <param name="aType">Type to reflect</param>
  /// <returns>A serialization property info or null if not found</returns>
  /// <exception cref="InternalException">
  ///   If a property pointed out with <see cref="Qhta.Xml.Serialization.XmlContentPropertyAttribute"/> is not found.
  /// </exception>
  public virtual SerializationPropertyInfo? GetTextProperty(Type aType)
  {
    var textProperties = aType.GetProperties().Where(item =>
      item.CanWrite && item.CanRead &&
      item.GetCustomAttributes(true).OfType<XmlTextAttribute>().Count() > 0);
    if (textProperties.Count() == 0)
      return null;

    if (textProperties.Count() > 1)
      throw new InternalException($"Type {aType.Name} has multiple properties marked as xml text, but only one is allowed");

    var textProperty = textProperties.First();
    var knownTextProperty = new SerializationPropertyInfo("", textProperty);
    knownTextProperty.TypeInfo = RegisterType(textProperty.PropertyType);
    return knownTextProperty;
  }

  /// <summary>
  /// Registers a property which is indended to get/set Xml content of the Xml element.
  /// This property are marked in the type header with <see cref="Qhta.Xml.Serialization.XmlContentPropertyAttribute"/>.
  /// Note that <see cref="System.Windows.Markup.ContentPropertyAttribute"/> is not used to avoid the need of System.Xaml package.
  /// </summary>
  /// <param name="aType">Type to reflect</param>
  /// <returns>A serialization property info or null if not found</returns>
  /// <exception cref="InternalException">
  ///   If a property pointed out with <see cref="Qhta.Xml.Serialization.XmlContentPropertyAttribute"/> is not found.
  /// </exception>
  public virtual SerializationPropertyInfo? GetContentProperty(Type aType)
  {
    var contentPropertyAttrib = aType.GetCustomAttribute<XmlContentPropertyAttribute>(true);
    if (contentPropertyAttrib != null)
    {
      var contentPropertyName = contentPropertyAttrib.Name;
      var contentProperty = aType.GetProperty(contentPropertyName);
      if (contentProperty == null)
        throw new InternalException($"Content property \"{contentPropertyName}\" in {aType.Name} not found");
      var contentPropertyInfo = new SerializationPropertyInfo(contentPropertyName, contentProperty);
      contentPropertyInfo.TypeInfo = RegisterType(contentProperty.PropertyType);
      return contentPropertyInfo;
    }
    return null;
  }

  /// <summary>
  /// Registers a converter to read/write using XmlReader/XmlWriter.
  /// This converter is declared in the type header with <see cref="XmlConverterAttribute"/>.
  /// </summary>
  /// <param name="aType">Type to reflect</param>
  /// <returns>An instance of <see cref="XmlConverter"/></returns>
  public virtual XmlConverter? GetXmlConverter(Type aType)
  {
    var xmlTypeConverterAttrib = aType.GetCustomAttribute<XmlConverterAttribute>(true);
    if (xmlTypeConverterAttrib != null)
    {
      var converterType = xmlTypeConverterAttrib.ConverterType;
      if (converterType == null)
        throw new InternalException($"Converter type not declared in {typeof(XmlConverterAttribute).Name} in {aType.Name}");
      Type[] argTypes = new Type[xmlTypeConverterAttrib.Args.Length];
      for (int i = 0; i < argTypes.Length; i++)
        argTypes[i] = xmlTypeConverterAttrib.Args[i].GetType();
      var constructor = converterType.GetConstructor(argTypes);
      if (constructor == null)
        throw new InternalException($"Converter type {converterType.Name} has no appropriate constructor");
      var converter = constructor.Invoke(xmlTypeConverterAttrib.Args) as XmlConverter;
      if (converter == null)
        throw new InternalException($"Converter type {converterType.Name} is not a subclass of {typeof(XmlConverter).Name}");
      return converter;
    }
    return null;
  }

  /// <summary>
  /// Registers types, which are indended to be serialized as Xml children elements.
  /// These types are marked for the type with <see cref="Qhta.Xml.Serialization.XmlItemElementAttribute"/>
  /// </summary>
  /// <param name="aType">Type to reflect</param>
  /// <returns>A dictionary of known item types</returns>
  public virtual KnownItemTypesDictionary GetKnownItems(Type aType)
  {
    KnownItemTypesDictionary knownItems = new();
    var xmlItemElementAttributes = aType.GetCustomAttributes<XmlItemElementAttribute>(false).ToList();
    var xmlDictionaryItemAttributes = aType.GetCustomAttributes<XmlDictionaryItemAttribute>(false);
    xmlItemElementAttributes.AddRange(xmlDictionaryItemAttributes);
    if (xmlItemElementAttributes.Any())
    {
      foreach (var xmlItemElementAttribute in xmlItemElementAttributes)
      {
        var itemType = xmlItemElementAttribute.Type;
        if (itemType == null)
          itemType = typeof(object);
        if (!KnownTypes.TryGetValue(itemType, out var itemTypeInfo))
        {
          itemTypeInfo = RegisterType(itemType);
        }
        if (itemTypeInfo == null)
          throw new InternalException($"Unknown type {itemType} for deserialization");
        SerializationItemTypeInfo knownItemTypeInfo;
        if (!string.IsNullOrEmpty(xmlItemElementAttribute.ElementName))
          knownItemTypeInfo = new SerializationItemTypeInfo(xmlItemElementAttribute.ElementName, itemTypeInfo);
        else
          knownItemTypeInfo = new SerializationItemTypeInfo(null, itemTypeInfo);
        var addMethodName = xmlItemElementAttribute.AddMethod;
        if (xmlItemElementAttribute is XmlDictionaryItemAttribute xmlDictionaryItemAttribute)
        {
          if (xmlDictionaryItemAttribute.KeyAttributeName != null)
          {
            knownItemTypeInfo.KeyName = xmlDictionaryItemAttribute.KeyAttributeName;
          }
          if (addMethodName == null)
            addMethodName = "Add";
        }
        if (addMethodName != null)
        {
          var addMethods = aType.GetMethods().Where(m => m.Name == addMethodName).ToList();
          if (addMethods.Count() == 0)
            throw new InternalException($"Add method \"{xmlItemElementAttribute.AddMethod}\" in type {aType} not found for deserialization");
          else
          {
            MethodInfo? addMethodInfo = null;
            foreach (var addMethod in addMethods)
            {
              var parameters = addMethod.GetParameters();
              if (parameters.Length == 1 && !(xmlItemElementAttribute is XmlDictionaryItemAttribute))
              {
                if (itemType.IsEqualOrSubclassOf(parameters[0].ParameterType))
                {
                  addMethodInfo = addMethod;
                  break;
                }
              }
              else
              if (parameters.Length == 2 && (xmlItemElementAttribute is XmlDictionaryItemAttribute))
              {
                if (itemType.IsEqualOrSubclassOf(parameters[1].ParameterType))
                {
                  addMethodInfo = addMethod;
                  break;
                }
              }
            }
            if (addMethodInfo != null)
              knownItemTypeInfo.AddMethod = addMethodInfo;
            else
              throw new InternalException($"No compatible \"{xmlItemElementAttribute.AddMethod}\"method found in type {itemType}.");
          }
        }
        knownItems.Add(knownItemTypeInfo);
      }
    }
    return knownItems;
  }

  /// <summary>
  /// Gets a type converted for a type. It can be pointed out with a <see cref="System.ComponentModel.TypeConverterAttribute"/>
  /// in a header of the type. This attribute holds the converter type name. 
  /// To use the converter, it must be defined in the same assembly as the type
  /// and must have a parameterless public constructor.
  /// </summary>
  /// <param name="aType">A type fo reflect</param>
  /// <returns>Type converter instance</returns>
  /// <exception cref="InternalException">
  ///    Thrown in two cases:
  ///    <list type="number">
  ///      <item>Type converter of the specified name could not be found</item>
  ///      <item>Type converter can not convert to/from string type</item>
  ///    </list>
  /// </exception>
  public TypeConverter? GetTypeConverter(Type aType)
  {
    var typeConverterAttribute = aType.GetCustomAttribute<TypeConverterAttribute>();
    if (typeConverterAttribute != null)
    {
      var converterTypeName = typeConverterAttribute.ConverterTypeName;
      if (converterTypeName == null)
        throw new InternalException($"Converter type name in a TypeConverter attribute must not be null");
      var result = FindTypeConverter(converterTypeName);
      if (result == null)
        throw new InternalException($"Type converter \"{converterTypeName}\" not found");
      if (!(result.CanConvertTo(typeof(string)) && result.CanConvertFrom(typeof(string))))
        throw new InternalException($"Type converter \"{converterTypeName}\" not found");
      return result;
    }
    return null;
  }

  #region Helper methods
  public static int PropOrderComparison(SerializationPropertyInfo a, SerializationPropertyInfo b)
  {
    if (a.Order > b.Order)
      return 1;
    if (a.Order < b.Order)
      return -1;
    return String.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);
  }

  protected TypeConverter? FindTypeConverter(string typeName)
  {
    var type = Assembly.GetExecutingAssembly().GetType(typeName);
    if (type == null)
      type = Assembly.GetCallingAssembly().GetType(typeName);
    if (type == null)
      type = Assembly.GetEntryAssembly()?.GetType(typeName);
    if (type != null)
      return CreateTypeConverter(type);
    return null;
  }

  protected TypeConverter? CreateTypeConverter(Type type)
  {
    if (!type.IsSubclassOf(typeof(TypeConverter)))
      throw new InternalException($"Declared type converter \"{type.Name}\" must be a subclass of {typeof(TypeConverter).FullName}");
    var constructor = type.GetConstructor(new Type[0]);
    if (constructor == null)
      throw new InternalException($"Declared type converter \"{type.Name}\" must have a public parameterless constructor");
    return constructor.Invoke(null) as TypeConverter;
  }

  protected void SearchShouldSerializeMethods(Type aType, SerializationTypeInfo typeInfo)
  {
    var methodInfos = aType
      .GetMethodsByInheritance(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      .Where(item => item.Name.StartsWith(Options.ShouldSerializeMethodPrefix)).ToArray();
    if (methodInfos.Length > 0)
    {
      foreach (var attrPropInfo in typeInfo.PropsAsAttributes)
        SearchShouldSerializeMethod(methodInfos, attrPropInfo);
      foreach (var elemPropInfo in typeInfo.PropsAsElements)
        SearchShouldSerializeMethod(methodInfos, elemPropInfo);
    }
  }

  protected void SearchShouldSerializeMethod(MethodInfo[] methodInfos, SerializationPropertyInfo propInfo)
  {
    var methodInfo = methodInfos.FirstOrDefault(item => item.Name.EndsWith(propInfo.PropInfo.Name));
    if (methodInfo != null)
    {
      if (methodInfo.ReturnType == typeof(bool))
        if (!methodInfo.GetParameters().Any())
          propInfo.ShouldSerializeMethod = methodInfo;
    }
  }

  public virtual bool IsSimple(Type aType)
  {
    if (aType.IsNullable(out var baseType))
      return IsSimple(baseType);
    bool isSimpleValue = false;
    if (aType == typeof(string))
      isSimpleValue = true;
    else if (aType == typeof(bool))
      isSimpleValue = true;
    else if (aType == typeof(int))
      isSimpleValue = true;
    return isSimpleValue;
  }

  public virtual bool IsSimple(object propValue)
  {
    bool isSimpleValue = false;
    if (propValue is string)
      isSimpleValue = true;
    else if (propValue is bool)
      isSimpleValue = true;
    else if (propValue is int)
      isSimpleValue = true;
    return isSimpleValue;
  }

  public virtual string LowercaseName(string str)
  {
    if (IsUpper(str))
      return str.ToLower();
    return ToLowerFirst(str);
  }

  public static string ToLowerFirst(string text)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    char[] ss = text.ToCharArray();
    ss[0] = char.ToLower(ss[0]);
    return new string(ss);
  }
  public static bool IsUpper(string text)
  {
    if (text == null)
      return false;
    foreach (var ch in text)
      if (char.IsLetter(ch) && !Char.IsUpper(ch))
        return false;
    return true;
  }
  #endregion

}