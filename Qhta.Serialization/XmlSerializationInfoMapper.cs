﻿using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Xml;

using Qhta.Conversion;
using Qhta.TypeUtils;

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
  /// <param name="defaultNamespace">default namespace for elements</param>
  public XmlSerializationInfoMapper(SerializationOptions options, string? defaultNamespace)
  {
    Options = options;
    DefaultNamespace = defaultNamespace;
  }

  /// <summary>
  /// A XML namespace of the main type.
  /// </summary>
  public string? DefaultNamespace { get; set; }

  public KnownNamespacesCollection KnownNamespaces { get; set; } = new();

  /// <summary>
  /// Only some of the options are used:
  /// <list type="bullet">
  ///  <item>
  ///     <see cref="SerializationOptions.IgnoreMissingConstructor"/>
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
  public KnownTypesCollection KnownTypes { get; } = new KnownTypesCollection();

  public Dictionary<string, TypeConverter> TypeConverters { get; } = new();

  public SerializationTypeInfo RegisterType(Type aType)
  {
    if (aType.IsNullable(out var baseType))
      aType = baseType;
    #region Checking if a type was already registered
    if (KnownTypes.TryGetValue(aType, out var knownTypeInfo))
    {
      return knownTypeInfo;
    }
    #endregion

    #region Creating and registering a new serialization info
    // first create a new type info
    // and add it to avoid stack overflow with recurrency
    var newTypeInfo = CreateTypeInfo(aType);
    KnownTypes.Add(newTypeInfo);
    if (newTypeInfo.XmlNamespace != null && newTypeInfo.ClrNamespace != null)
      KnownNamespaces.TryAdd(newTypeInfo.XmlNamespace, newTypeInfo.ClrNamespace);

    // Then fill the type info
    FillTypeInfo(newTypeInfo);
    #endregion

    return newTypeInfo;
  }


  /// <summary>
  /// A method to create type info from a type.
  /// If it is a nullable type, then type info is created from it's base type.
  /// </summary>
  /// <param name="aType"></param>
  /// <returns></returns>
  protected SerializationTypeInfo CreateTypeInfo(Type aType)
  {
    if (aType.IsNullable(out var baseType))
      aType = baseType;
    SerializationTypeInfo typeInfo = new SerializationTypeInfo(aType);
    var xmlRootAttrib = aType.GetCustomAttribute<XmlRootAttribute>(false);
    var elemName = xmlRootAttrib?.ElementName;
    if (String.IsNullOrEmpty(elemName))
      elemName = aType.Name;
    var elemNamespace = xmlRootAttrib?.Namespace;
    //var qElemName = new QualifiedName(elemName ?? "", elemNamespace);
    if (elemNamespace != null && DefaultNamespace == null)
      DefaultNamespace = elemNamespace;
    //if (qElemName.IsEmpty())
    //{
    //  var xmlCollectionAttribute = aType.GetCustomAttribute<XmlCollectionAttribute>();
    //  if (xmlCollectionAttribute != null)
    //    qElemName = new QualifiedName(xmlCollectionAttribute.ElementName);
    //  if (qElemName.IsEmpty())
    //  {
    //    var xmlDictionaryAttribute = aType.GetCustomAttribute<XmlDictionaryAttribute>();
    //    if (xmlDictionaryAttribute != null)
    //      qElemName = new QualifiedName(xmlDictionaryAttribute.ElementName);
    //    if (qElemName.IsEmpty())
    //    {
    //      qElemName = new QualifiedName(aType.Name, elemNamespace);
    //    }
    //  }
    //}

    typeInfo.XmlName = elemName;
    typeInfo.XmlNamespace = elemNamespace;
    typeInfo.ClrNamespace = aType.Namespace;
    return typeInfo;
  }

  /// <summary>
  /// This method fills the <see cref="SerializationTypeInfo"/> parameter 
  /// with data taken from a type.
  /// </summary>
  /// <param name="typeInfo"></param>
  /// <exception cref="InternalException">
  ///   Thrown if a type has no parameterless public constructor 
  ///   and an option <see cref="SerializationOptions.IgnoreMissingConstructor"/> is not set.
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
        if (!Options.IgnoreMissingConstructor && aType.IsNullable())
          throw new IOException($"Type {aType.Name} must have a public, parameterless constructor to allow deserialization");
      }
      typeInfo.KnownConstructor = constructor;
    }
    #endregion

    MapPropertiesAndFields(aType, typeInfo);
    SearchShouldSerializeMethods(aType, typeInfo);

    //typeInfo.KnownItemTypes = GetKnownItems(aType);
    typeInfo.XmlConverter = GetXmlConverter(aType);
    typeInfo.TypeConverter = GetTypeConverter(aType);
    typeInfo.KnownSubtypes = GetKnownTypes(aType);
  }

  public virtual void MapPropertiesAndFields(Type aType, SerializationTypeInfo typeInfo)
  {
    var members = aType.GetMembersByInheritance().Where(item => item is FieldInfo || item is PropertyInfo).ToList();
    if (!members.Any())
      return;
    typeInfo.ContentProperty = GetContentProperty(aType);
    typeInfo.TextProperty = GetTextProperty(aType);

    int attrCount = 0;
    int elemCount = 0;
    foreach (var memberInfo in members)
    {
      //if (memberInfo.Name == "GroupNumber")
      //  TestTools.Stop();
      if (memberInfo.GetCustomAttributes(true).OfType<XmlIgnoreAttribute>().Any())
        continue;
      if (memberInfo == typeInfo.ContentProperty?.Member)
        continue;
      if (memberInfo == typeInfo.TextProperty?.Member)
        continue;
      var xmlAttribute = memberInfo.GetCustomAttributes(true).OfType<XmlAttributeAttribute>().FirstOrDefault();
      if (xmlAttribute != null)
        TryAddMemberAsAttribute(typeInfo, memberInfo, xmlAttribute, ++attrCount);
      else
      {
        var elementAttribute = memberInfo.GetCustomAttributes(true).OfType<XmlElementAttribute>().FirstOrDefault();
        if (elementAttribute != null)
          TryAddMemberAsElement(typeInfo, memberInfo, elementAttribute, ++elemCount);
        else
        {
          var dictionaryAttribute = memberInfo.GetCustomAttributes(true).OfType<XmlDictionaryAttribute>().FirstOrDefault();
          if (dictionaryAttribute != null)
            TryAddMemberAsDictionary(typeInfo, memberInfo, dictionaryAttribute, ++elemCount);
          else
          {
            var collectionAttribute = memberInfo.GetCustomAttributes(true).OfType<XmlCollectionAttribute>().FirstOrDefault();
            if (collectionAttribute != null)
              TryAddMemberAsCollection(typeInfo, memberInfo, collectionAttribute, ++elemCount);
            else
            {
              var arrayAttribute = memberInfo.GetCustomAttributes(true).OfType<XmlArrayAttribute>().FirstOrDefault();
              if (arrayAttribute != null)
                TryAddMemberAsCollection(typeInfo, memberInfo, arrayAttribute, ++elemCount);
              else
              {
                if (memberInfo.GetValueType()?.IsDictionary() == true)
                {
                  TryAddMemberAsDictionary(typeInfo, memberInfo, null, ++elemCount);
                }
                else if (memberInfo.GetValueType()?.IsCollection() == true)
                {
                  if (!aType.IsDictionary() || memberInfo.Name != "Keys" && memberInfo.Name != "Values")
                    TryAddMemberAsCollection(typeInfo, memberInfo, null, ++elemCount);
                }
                else if (Options.AcceptAllProperties)
                {
                  if (!(memberInfo.IsIndexer()))
                  {
                    if (memberInfo.GetValueType()?.IsSimple() == true)
                    {
                      if (memberInfo.CanWrite() == true)
                      {
                        if (Options.SimplePropertiesAsAttributes)
                          TryAddMemberAsAttribute(typeInfo, memberInfo, null, ++attrCount);
                        else
                          TryAddMemberAsElement(typeInfo, memberInfo, null, ++elemCount);
                      }
                    }
                    else
                    //if (propInfo.PropertyType.IsDictionary())
                    //  TryAddPropertyAsDictionary(typeInfo, propInfo, null, ++elemCount);
                    //else
                    //if (propInfo.PropertyType.IsCollection())
                    //  TryAddPropertyAsCollection(typeInfo, propInfo, null, ++elemCount);
                    //else
                    if (memberInfo.CanWrite() == true)
                      TryAddMemberAsElement(typeInfo, memberInfo, null, ++elemCount);
                  }
                }
              }
            }
          }
        }
      }
    }
  }


  /// <summary>
  /// Adds property/field to <see cref="SerializationTypeInfo.MembersAsAttributes"/>
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="memberInfo">Selected property/field</param>
  /// <param name="xmlAttribute">Found XmlAttributeAttribute</param>
  /// <param name="defaultOrder">Default order of serialized attribute</param>
  protected virtual bool TryAddMemberAsAttribute(SerializationTypeInfo typeInfo, MemberInfo memberInfo, XmlAttributeAttribute? xmlAttribute, int defaultOrder)
  {
    var attrName = xmlAttribute?.AttributeName;
    var attrNamespace = xmlAttribute?.Namespace;
    if (string.IsNullOrEmpty(attrName))
      attrName = memberInfo.Name;
    if (Options.AttributeNameCase != 0)
      attrName = QXmlSerializer.ChangeCase(attrName, Options.AttributeNameCase);
    var qAttrName = new QualifiedName(attrName, attrNamespace);
    if (typeInfo.MembersAsAttributes.ContainsKey(qAttrName))
      return false;
    var order = memberInfo.GetCustomAttribute<SerializationOrderAttribute>()?.Order ?? defaultOrder;
    var serializationMemberInfo = CreateSerializationMemberInfo(qAttrName, memberInfo, order);
    serializationMemberInfo.IsNullable = memberInfo.GetCustomAttribute<XmlElementAttribute>()?.IsNullable ?? false;
    serializationMemberInfo.DataType = xmlAttribute?.DataType;
    typeInfo.MembersAsAttributes.Add(serializationMemberInfo);
    return true;
  }

  /// <summary>
  /// Adds property/field to <see cref="SerializationTypeInfo.MembersAsElements"/>
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="memberInfo">Selected property/field</param>
  /// <param name="xmlAttribute">Found XmlElementAttribute</param>
  /// <param name="defaultOrder">default order</param>
  protected virtual bool TryAddMemberAsElement(SerializationTypeInfo typeInfo, MemberInfo memberInfo, XmlElementAttribute? xmlAttribute, int defaultOrder)
  {
    var elemName = xmlAttribute?.ElementName;
    var elemNamespace = xmlAttribute?.Namespace;
    if (string.IsNullOrEmpty(elemName))
      elemName = memberInfo.Name;
    if (Options.ElementNameCase != 0)
      elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);
    var qElemName = new QualifiedName(elemName, elemNamespace);
    if (typeInfo.MembersAsElements.ContainsKey(qElemName))
      return false;
    var order = xmlAttribute?.Order ?? memberInfo.GetCustomAttribute<SerializationOrderAttribute>()?.Order ?? defaultOrder;
    var serializationMemberInfo = CreateSerializationMemberInfo(qElemName, memberInfo, order);
    serializationMemberInfo.IsNullable = memberInfo.GetCustomAttribute<XmlElementAttribute>()?.IsNullable ?? false;
    serializationMemberInfo.DataType = xmlAttribute?.DataType;
    typeInfo.MembersAsElements.Add(serializationMemberInfo);
    //KnownNamespaces.TryAdd(serializationMemberInfo.Name.Namespace);
    return true;
  }

  /// <summary>
  /// Adds property/field to <see cref="SerializationTypeInfo.MembersAsElements"/> with <see cref="DictionaryInfo"/>
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="memberInfo">Selected property/field</param>
  /// <param name="attribute">Found XmlDictionaryAttribute</param>
  /// <param name="defaultOrder">default order</param>
  protected virtual bool TryAddMemberAsDictionary(SerializationTypeInfo typeInfo, MemberInfo memberInfo, XmlDictionaryAttribute? attribute, int defaultOrder)
  {
    var elemName = attribute?.ElementName;
    var elemNamespace = attribute?.Namespace;
    if (string.IsNullOrEmpty(elemName))
      elemName = memberInfo.Name;
    if (Options.ElementNameCase != 0)
      elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);
    var qElemName = new QualifiedName(elemName, elemNamespace);
    if (typeInfo.MembersAsElements.ContainsKey(qElemName))
      return false;
    var order = attribute?.Order ?? memberInfo.GetCustomAttribute<SerializationOrderAttribute>()?.Order ?? defaultOrder;
    var serializationMemberInfo = CreateSerializationMemberInfo(qElemName, memberInfo, order);
    typeInfo.MembersAsElements.Add(elemName, serializationMemberInfo);
    //KnownNamespaces.TryAdd(qElemName.XmlNamespace);
    return true;
  }

  /// <summary>
  /// Adds property/field to <see cref="SerializationTypeInfo.MembersAsElements"/> with <see cref="CollectionInfo"/>
  /// </summary>
  /// <param name="typeInfo">Object to add to</param>
  /// <param name="memberInfo">Selected property/field</param>
  /// <param name="attribute">Found XmlCollectionAttribute</param>
  /// <param name="defaultOrder">default order</param>
  protected virtual bool TryAddMemberAsCollection(SerializationTypeInfo typeInfo, MemberInfo memberInfo, XmlArrayAttribute? attribute, int defaultOrder)
  {
    var elemName = attribute?.ElementName;
    var elemNamespace = attribute?.Namespace;
    if (string.IsNullOrEmpty(elemName))
      elemName = memberInfo.Name;
    if (Options.ElementNameCase != 0)
      elemName = QXmlSerializer.ChangeCase(elemName, Options.ElementNameCase);
    var qElemName = new QualifiedName(elemName, elemNamespace);
    if (typeInfo.MembersAsElements.ContainsKey(qElemName))
      return false;
    var order = attribute?.Order ?? memberInfo.GetCustomAttribute<SerializationOrderAttribute>()?.Order ?? defaultOrder;
    var serializationMemberInfo = CreateSerializationMemberInfo(qElemName, memberInfo, order);
    serializationMemberInfo.CollectionInfo = CreateCollectionInfo(memberInfo);
    typeInfo.MembersAsElements.Add(elemName, serializationMemberInfo);
    //KnownNamespaces.TryAdd(serializationMemberInfo.Name.Namespace);
    if (serializationMemberInfo.TypeConverter == null && serializationMemberInfo.XmlConverter == null)
      serializationMemberInfo.TypeConverter = new ArrayConverter();
    return true;
  }

  protected SerializationMemberInfo CreateSerializationMemberInfo(QualifiedName name, MemberInfo memberInfo, int order)
  {
    var serializationMemberInfo = new SerializationMemberInfo(name, memberInfo, order);
    if (memberInfo.GetCustomAttribute<XmlReferenceAttribute>() != null)
      serializationMemberInfo.IsReference = true;
    if (memberInfo is PropertyInfo propInfo)
      serializationMemberInfo.ValueType = RegisterType(propInfo.PropertyType);
    else
    if (memberInfo is FieldInfo fieldInfo)
      serializationMemberInfo.ValueType = RegisterType(fieldInfo.FieldType);
    serializationMemberInfo.DefaultValue =
      (memberInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as DefaultValueAttribute)?.Value;
    var converterTypeName = memberInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
    if (converterTypeName != null)
      serializationMemberInfo.TypeConverter = FindTypeConverter(converterTypeName);
    var converterType = memberInfo.GetCustomAttribute<XmlTypeConverterAttribute>()?.ConverterType;
    if (converterType != null)
      serializationMemberInfo.TypeConverter = CreateTypeConverter(converterType);
    var xmlDataFormat = memberInfo.GetCustomAttribute<XmlDataFormatAttribute>();
    if (xmlDataFormat != null)
    {
      if (xmlDataFormat.Format != null)
        serializationMemberInfo.Format = xmlDataFormat.Format;
      if (xmlDataFormat.Culture != null)
        serializationMemberInfo.Culture = xmlDataFormat.Culture;
    }
    return serializationMemberInfo;
  }

  #region Collection Handling
  protected CollectionInfo CreateCollectionInfo(Type aType)
  {
    return CreateCollectionInfo(aType, aType.GetCustomAttributes(true).OfType<XmlArrayItemAttribute>().ToArray());
  }

  protected CollectionInfo? CreateCollectionInfo(MemberInfo memberInfo)
  {
    var arrayAttribute = memberInfo.GetCustomAttributes(true).OfType<XmlArrayAttribute>().FirstOrDefault();
    var arrayItemsAttributes = memberInfo.GetCustomAttributes(true).OfType<XmlArrayItemAttribute>().ToArray();
    if (arrayAttribute == null && arrayItemsAttributes.Length == 0)
      return null;
    var valueType = memberInfo.GetValueType();
    if (valueType == null)
      return null;
    var valueTypeInfo = RegisterType(valueType);
    var result = CreateCollectionInfo(valueType, arrayItemsAttributes);
    if (memberInfo.GetCustomAttribute<XmlReferencesAttribute>() != null)
      result.StoresReferences = true;
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
        var serializationItemTypeInfo = new SerializationItemInfo(elemName, RegisterType(itemType));
        collectionTypeInfo.KnownItemTypes.Add(serializationItemTypeInfo);
      }
    }
    else if (aType.IsCollection(out var itemType))
    {
      var itemTypeInfo = RegisterType(itemType);
      var elemName = itemType.Name;
      var serializationItemTypeInfo = new SerializationItemInfo(elemName, RegisterType(itemType));
      collectionTypeInfo.KnownItemTypes.Add(serializationItemTypeInfo);
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

  protected DictionaryInfo? CreateDictionaryInfo(MemberInfo memberInfo)
  {
    var dictionaryItemAttributes = memberInfo.GetCustomAttributes(true).OfType<XmlDictionaryItemAttribute>().ToArray();
    if (dictionaryItemAttributes.Length == 0)
      return null;
    var valueType = memberInfo.GetValueType();
    if (valueType == null)
      return null;
    var result = CreateDictionaryInfo(valueType, dictionaryItemAttributes);
    if (memberInfo.GetCustomAttribute<XmlReferencesAttribute>() != null)
      result.StoresReferences = true;
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
        var serializationItemTypeInfo = new SerializationItemInfo(elemName, RegisterType(itemType));
        dictionaryTypeInfo.KnownItemTypes.Add(serializationItemTypeInfo);
      }
    }
    else if (aType.IsDictionary(out var keyType, out var valType))
    {
      dictionaryTypeInfo.KeyTypeInfo = RegisterType(keyType);
      var elemName = "Item";
      var serializationItemTypeInfo = new SerializationItemInfo(elemName, RegisterType(valType));
      dictionaryTypeInfo.KnownItemTypes.Add(serializationItemTypeInfo);
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
  public virtual SerializationMemberInfo? GetTextProperty(Type aType)
  {
    var textProperties = aType.GetProperties().Where(item =>
      item.CanWrite && item.CanRead &&
      item.GetCustomAttributes(true).OfType<XmlTextAttribute>().Any());
    if (!textProperties.Any())
      return null;

    if (textProperties.Count() > 1)
      throw new InternalException($"Type {aType.Name} has multiple properties marked as xml text, but only one is allowed");

    var textProperty = textProperties.First();
    var knownTextProperty = new SerializationMemberInfo("", textProperty);
    knownTextProperty.ValueType = RegisterType(textProperty.PropertyType);
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
  public virtual SerializationMemberInfo? GetContentProperty(Type aType)
  {
    var contentPropertyAttrib = aType.GetCustomAttribute<XmlContentPropertyAttribute>(true);
    if (contentPropertyAttrib != null)
    {
      var contentPropertyName = contentPropertyAttrib.Name;
      var contentProperty = aType.GetProperty(contentPropertyName);
      if (contentProperty == null)
        throw new InternalException($"Content property \"{contentPropertyName}\" in {aType.Name} not found");
      var contentPropertyInfo = new SerializationMemberInfo(contentPropertyName, contentProperty);
      contentPropertyInfo.ValueType = RegisterType(contentProperty.PropertyType);
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
        throw new InternalException($"Converter type not declared in XmlConverterAttribute assigned to a type {aType.Name}");
      Type[] argTypes = new Type[xmlTypeConverterAttrib.Args.Length];
      for (int i = 0; i < argTypes.Length; i++)
        argTypes[i] = xmlTypeConverterAttrib.Args[i].GetType();
      var constructor = converterType.GetConstructor(argTypes);
      if (constructor == null)
        throw new InternalException($"Converter type {converterType.Name} has no appropriate constructor");
      var converter = constructor.Invoke(xmlTypeConverterAttrib.Args) as XmlConverter;
      if (converter == null)
        throw new InternalException($"Converter type {converterType.Name} is not a subclass of XmlConverter");
      return converter;
    }
    return null;
  }

  /// <summary>
  /// Get types which are assigned to the class with KnownType attribute.
  /// </summary>
  /// <param name="aType">Type to reflect</param>
  /// <returns>A dictionary of known item types (or null) if no KnownType attributes found)</returns>
  public virtual KnownTypesCollection? GetKnownTypes(Type aType)
  {
    KnownTypesCollection? knownItems = null;
    var xmlKnownTypeAttributes = aType.GetCustomAttributes<KnownTypeAttribute>(false).ToList();
    if (xmlKnownTypeAttributes.Any())
    {
      knownItems = new();
      foreach (var xmlKnownTypeAttribute in xmlKnownTypeAttributes)
      {
        var itemType = xmlKnownTypeAttribute.Type;
        if (itemType != null)
        {
          var knownTypeInfo = RegisterType(itemType);
          knownItems.Add(knownTypeInfo);
        }
        else
        {
          var methodName = xmlKnownTypeAttribute.MethodName;
          if (methodName != null)
          {
            var methodInfo = aType.GetMethod(methodName, BindingFlags.Static);
            if (methodInfo == null)
              throw new InvalidOperationException(
                $"KnownTypeAttribute assigned to type {aType.Name} specifies a static method name \"{methodName}\" which can't be found");
            if (methodInfo.GetParameters().Length != 0)
              throw new InvalidOperationException(
                $"KnownTypeAttribute assigned to type {aType.Name} specifies a static method name \"{methodName}\" which must be parameterless");
            if (!methodInfo.ReturnType.IsEnumerable(out var resultItemType) || resultItemType != typeof(Type))
              throw new InvalidOperationException(
                $"KnownTypeAttribute assigned to type {aType.Name} specifies a static method name \"{methodName}\" which must return a result implementing IEnumerable<Type> interface");
            var knownTypesResult = methodInfo.Invoke(null, new object[0]) as IEnumerable<Type>;
            if (knownTypesResult == null)
              throw new InvalidOperationException(
                $"KnownTypeAttribute assigned to type {aType.Name} specifies a static method name \"{methodName}\" which returns null");
            foreach (var item in knownTypesResult)
            {
              var knownTypeInfo = RegisterType(item);
              knownItems.Add(knownTypeInfo);
            }
          }
          else
            throw new InvalidOperationException(
              $"KnownTypeAttribute assigned to type {aType.Name} must have either a type or a method name specified");
        }
      }
    }
    return knownItems;
  }

  /// <summary>
  /// Registers types, which are indended to be serialized as Xml children elements.
  /// These types are marked for the type with <see cref="Qhta.Xml.Serialization.XmlItemElementAttribute"/>
  /// </summary>
  /// <param name="aType">Type to reflect</param>
  /// <returns>A dictionary of known item types</returns>
  public virtual KnownItemTypesCollection GetKnownItemTypes(Type aType)
  {
    KnownItemTypesCollection knownItems = new();
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
        SerializationItemInfo knownItemTypeInfo;
        if (!string.IsNullOrEmpty(xmlItemElementAttribute.ElementName))
          knownItemTypeInfo = new SerializationItemInfo(xmlItemElementAttribute.ElementName, itemTypeInfo);
        else
          knownItemTypeInfo = new SerializationItemInfo(itemTypeInfo);
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
          if (!addMethods.Any())
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
        throw new InternalException($"Converter type name in a TypeConverter xmlAttribute must not be null");
      if (!TypeConverters.TryGetValue(converterTypeName, out var converter))
      {
        converter = FindTypeConverter(converterTypeName);
        if (converter == null)
          throw new InternalException($"Type converter \"{converterTypeName}\" not found");
        if (!(converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string))))
          throw new InternalException($"Type converter \"{converterTypeName}\" not found");
        TypeConverters.Add(converterTypeName, converter);
      }
      return converter;
    }
    return null;
  }

  #region Helper methods
  public static int PropOrderComparison(SerializationMemberInfo a, SerializationMemberInfo b)
  {
    if (a.Order > b.Order)
      return 1;
    if (a.Order < b.Order)
      return -1;
    return a.QualifiedName.CompareTo(b.QualifiedName);
  }

  protected TypeConverter? FindTypeConverter(string typeName)
  {
    //var type = Assembly.GetExecutingAssembly().GetType(typeName);
    //if (type == null)
    //  type = Assembly.GetCallingAssembly().GetType(typeName);
    //if (type == null)
    //  type = Assembly.GetEntryAssembly()?.GetType(typeName);
    var type = Type.GetType(typeName);
    //      FindType(typeName);
    if (type != null)
      return CreateTypeConverter(type);
    return null;
  }

  private static Type? FindType(string fullName)
  {
    return
      AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic)
        .SelectMany(a => a.GetTypes())
        .FirstOrDefault(t => t.FullName?.Equals(fullName) == true);
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
    if (Options.CheckMethod.EndsWith('*'))
    {
      var prefix = Options.CheckMethod.Substring(0, Options.CheckMethod.Length - 1);
      var methodInfos = aType
        .GetMethodsByInheritance(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(item => item.Name.StartsWith(prefix)).ToArray();
      if (methodInfos.Length > 0)
      {
        foreach (var attrPropInfo in typeInfo.MembersAsAttributes)
          SearchShouldSerializeMethod(methodInfos, attrPropInfo);
        foreach (var elemPropInfo in typeInfo.MembersAsElements)
          SearchShouldSerializeMethod(methodInfos, elemPropInfo);
      }
    }
  }

  protected void SearchShouldSerializeMethod(MethodInfo[] methodInfos, SerializationMemberInfo propInfo)
  {
    var methodInfo = methodInfos.FirstOrDefault(item => item.Name.EndsWith(propInfo.Member.Name));
    if (methodInfo != null)
    {
      if (methodInfo.ReturnType == typeof(bool))
        if (!methodInfo.GetParameters().Any())
          propInfo.CheckMethod = methodInfo;
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
    if (string.IsNullOrEmpty(text))
      return false;
    foreach (var ch in text)
      if (char.IsLetter(ch) && !Char.IsUpper(ch))
        return false;
    return true;
  }
  #endregion

  public XmlQualifiedTagName GetXmlTag(INamedElement element)
  {
    var xmlName = element.XmlName;
    var xmlNamespace = element.XmlNamespace;
    var clrNamespace = element.ClrNamespace;
    if (xmlNamespace == null && clrNamespace != null)
      KnownNamespaces.ClrToXmlNamespace.TryGetValue(clrNamespace, out xmlNamespace);
    if (xmlNamespace == null)
      return new XmlQualifiedTagName(xmlName);
    if (xmlNamespace == DefaultNamespace)
      return new XmlQualifiedTagName(xmlName);
    return new XmlQualifiedTagName(xmlName, xmlNamespace);
  }

  public QualifiedName ToQualifiedName(XmlQualifiedTagName xmlQualifiedName)
  {
    if (String.IsNullOrEmpty(xmlQualifiedName.XmlNamespace))
      return new QualifiedName(xmlQualifiedName.Name, DefaultNamespace);
    if (KnownNamespaces.PrefixToXmlNamespace.TryGetValue(xmlQualifiedName.XmlNamespace, out var nspace))
      return new QualifiedName(xmlQualifiedName.Name, nspace);
    return new QualifiedName(xmlQualifiedName.Name);
  }

}