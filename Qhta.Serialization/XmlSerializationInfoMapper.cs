using Qhta.TestHelper;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Qhta.TypeUtils;
using System.Collections;

namespace Qhta.Xml.Serialization
{
  /// <summary>
  /// The purpose of this class is to build serialization info on types and properties
  /// and map xml element or attribute names to this info.
  /// </summary>
  public class XmlSerializationInfoMapper
  {
    /// <summary>
    /// To create a mapper you need serialization options.
    /// </summary>
    /// <param name="options">Instance of the serialization options - set only once</param>
    public XmlSerializationInfoMapper(SerializationOptions options, string? baseNamespace)
    {
      Options = options;
      BaseNamespace = baseNamespace ?? "";
    }

    /// <summary>
    /// A namespace of the main type. If registered type are in the same namespace,
    /// they are indexed without prefixes.
    /// </summary>
    public string BaseNamespace { get; init; }

    /// <summary>
    /// Only some of the options are used:
    /// <list type="bullet">
    ///  <item>
    ///     <see cref="SerializationOptions.IgnoreTypesWithoutParameterlessConstructor"/>
    ///     <see cref="SerializationOptions.LowercaseAttributeName"/>
    ///     <see cref="SerializationOptions.LowercasePropertyName"/>
    ///   </item>
    /// </list>
    /// </summary>
    public SerializationOptions Options { get; init; }

    /// <summary>
    /// The dictionary of the types known to the serializer.
    /// Types are registered by the name of the type or by <see cref="XmlRootAttribute"/>,
    /// <see cref="XmlCollectionAttribute"/>, or <see cref="XmlDictionaryAttribute"/>.
    /// </summary>
    public KnownTypesDictionary KnownTypes { get; init; } = new KnownTypesDictionary();

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
      var typeInfo = GetKnownType(aType);
      string? aName = typeInfo.ElementName;

      if (aName == null)
      { // elementName was not taken from any custom attribute
        var ns = aType.Namespace ?? "";
        if (ns == BaseNamespace)
          ns = null;
        else
        if (ns.StartsWith(BaseNamespace + "."))
          ns = ns.Substring(BaseNamespace.Length + 1);

        XmlQualifiedName qName;
        string typeName = aType.Name;
        if (typeName.Contains('`') && aType.FullName != null)
          typeName = aType.FullName;
        qName = new XmlQualifiedName(typeName, ns);
        aName = qName.ToString();
      }

      if (KnownTypes.TryGetValue(aName, out var oldTypeInfo))
      {
        var bType = oldTypeInfo.Type;
        if (bType != null && aType != bType && !Options.IgnoreMultipleTypeRegistration)
          throw new InternalException($"Name \"{aName}\" already used for {bType.FullName} while registering {aType.FullName} type");
        result = oldTypeInfo;
        return false;
      }

      KnownTypes.Add(aName, typeInfo);
      result = typeInfo;
      return true;
    }

    /// <summary>
    /// Creates the <see cref="SerializationTypeInfo"/> item and adds it to the known types dictionary.
    /// Attributes, properties and more info are reflected from the type.
    /// Ns is a prefix used to create a qualified name of the type, which is registered in the dictionary.
    /// </summary>
    /// <param name="aType">Original type to reflect</param>
    /// <param name="result">Newly created serialization type info or the already registered one.</param>
    /// <returns>true if the type info was newly added, false if it was added previously.</returns>
    public SerializationTypeInfo GetKnownType(Type aType)
    {
      TryAddKnownType(aType, out var result);
      return result;
    }

    /// <summary>
    /// Creates the <see cref="SerializationTypeInfo"/> item and adds it to the known types dictionary.
    /// Attributes, properties and more info are reflected from the type.
    /// Ns is a prefix used to create a qualified name of the type, which is registered in the dictionary.
    /// </summary>
    /// <param name="aType">Original type to reflect</param>
    /// <param name="result">Newly created serialization type info or the already registered one.</param>
    /// <returns>true if the type info was newly added, false if it was added previously.</returns>
    public bool TryAddKnownType(Type aType, out SerializationTypeInfo result)
    {
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
    /// A switch method to create type info using <see cref="XmlRootAttribute"/>,
    /// <see cref="XmlCollectionAttribute"/>, or <see cref="XmlDictionaryAttribute"/>.
    /// </summary>
    /// <param name="aType"></param>
    /// <returns></returns>
    protected SerializationTypeInfo CreateTypeInfo(Type aType)
    {
      SerializationTypeInfo typeInfo = new SerializationTypeInfo(aType);
      var xmlDictionaryAttribute = aType.GetCustomAttribute<XmlDictionaryAttribute>();
      if (xmlDictionaryAttribute != null)
        typeInfo.ElementName = xmlDictionaryAttribute.ElementName;
      else
      {
        var xmlCollectionAttribute = aType.GetCustomAttribute<XmlCollectionAttribute>();
        if (xmlCollectionAttribute != null)
          typeInfo.ElementName = xmlCollectionAttribute.ElementName;
        else
        {
          var xmlRootAttrib = aType.GetCustomAttribute<XmlRootAttribute>(false);
          if (xmlRootAttrib?.ElementName != null)
            typeInfo.ElementName = xmlRootAttrib.ElementName;
        }
      }
      return typeInfo;
    }

    /// <summary>
    /// This method fills the <see cref="SerializationTypeInfo"/> parameter 
    /// with data taken from a type.
    /// </summary>
    /// <param name="typeInfo"></param>
    /// <param name="aType"></param>
    /// <exception cref="InternalException">
    ///   Thrown if a type has no parameterless public constructor 
    ///   and an option <see cref="SerializationOptions.IgnoreTypesWithoutParameterlessConstructor"/> is not set.
    /// </exception>
    protected void FillTypeInfo(SerializationTypeInfo typeInfo)
    {
      var aType = typeInfo.Type;

      var xmlDictionaryAttribute = aType.GetCustomAttribute<XmlDictionaryAttribute>();
      if (xmlDictionaryAttribute != null)
        typeInfo.CollectionInfo = CreateDictionaryInfo(aType, xmlDictionaryAttribute);
      else
      {
        var xmlCollectionAttribute = aType.GetCustomAttribute<XmlCollectionAttribute>();
        if (xmlCollectionAttribute != null)
          typeInfo.CollectionInfo = CreateCollectionInfo(aType, xmlCollectionAttribute);
      }

      #region Checking and registering a known construtor - optional for simple types
      var constructor = aType.GetConstructor(new Type[0]);
      if (!aType.IsSimple() && !aType.IsAbstract && !aType.IsEnum)
      {
        if (constructor == null)
        {
          if (!Options.IgnoreTypesWithoutParameterlessConstructor && aType.IsNullable())
            throw new InternalException($"Type {aType.Name} must have a public, parameterless constructor to allow deserialization");
        }
        typeInfo.KnownConstructor = constructor;
      }
      #endregion
      typeInfo.PropsAsAttributes = GetPropsAsXmlAttributes(aType);
      typeInfo.PropsAsElements = GetPropsAsXmlElements(aType);
      typeInfo.KnownItemTypes = GetKnownItems(aType);
      typeInfo.XmlConverter = GetXmlConverter(aType);
      typeInfo.KnownContentProperty = GetContentProperty(aType);
      typeInfo.KnownTextProperty = GetTextProperty(aType);
      typeInfo.TypeConverter = GetTypeConverter(aType);

    }


    /// <summary>
    /// Registers properties which are intended to be serialized as XML attributes.
    /// These properties are marked in the type with <see cref="System.Xml.Serialization.XmlAttributeAttribute"/>
    /// or <see cref="Qhta.Xml.Serialization.XmlOrderedAttribAttribute"/> (which is a subclass of the previous one).
    /// </summary>
    /// <param name="aType">Type to reflect</param>
    /// <returns>A dictionary of known properties</returns>
    public virtual KnownPropertiesDictionary GetPropsAsXmlAttributes(Type aType)
    {
      var propList = new KnownPropertiesDictionary();
      var props = aType.GetProperties().Where(item => item.CanWrite && item.CanRead
        && item.GetCustomAttributes(true).OfType<XmlAttributeAttribute>().Count() > 0).ToList();
      if (props.Count() == 0)
        return propList;
      int n = 0;
      foreach (var propInfo in props)
      {
        var xmlAttribute = propInfo.GetCustomAttributes(true).OfType<XmlAttributeAttribute>().FirstOrDefault();
        if (xmlAttribute != null)
        {
          var attrName = xmlAttribute.AttributeName;
          if (string.IsNullOrEmpty(attrName))
            attrName = propInfo.Name;
          if (Options?.LowercaseAttributeName == true)
            attrName = LowercaseName(attrName);

          var order = ++n + 100;
          if (xmlAttribute is XmlOrderedAttribAttribute attr2 && attr2.Order != null)
            order = (int)attr2.Order;
          var serializePropInfo = new SerializationPropertyInfo(attrName, propInfo, order);
          if (TryRegisterType(propInfo.PropertyType, out var typeInfo))
            serializePropInfo.TypeInfo = typeInfo;
          var converterTypeName = propInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
          if (converterTypeName != null)
            serializePropInfo.TypeConverter = FindTypeConverter(converterTypeName);
          var converterType = propInfo.GetCustomAttribute<XmlTypeConverterAttribute>()?.ConverterType;
          if (converterType != null)
            serializePropInfo.TypeConverter = CreateTypeConverter(converterType);
          propList.Add(attrName, serializePropInfo);
        }
      }
      return propList;
    }

    /// <summary>
    /// Registers properties which are intended to be serialized as XML element.
    /// These properties are marked in the type header with <see cref="System.Xml.Serialization.XmlElementAttribute"/>
    /// or <see cref="System.Xml.Serialization.XmlArrayAttribute"/> or <see cref="System.Xml.Serialization.XmlCollectionAttribute"/>
    /// or <see cref="Qhta.Xml.Serialization.XmlDictionaryAttribute"/>. 
    /// For arrays and collections also all <see cref="System.Xml.Serialization.XmlArrayItemAttribute"/> are recognized
    /// to create known item types dictionary for each array.
    /// For dictionaries also all <see cref="Qhta.Xml.Serialization.XmlDictionaryItemAttribute"/> are recognized.
    /// </summary>
    /// <param name="aType">Type to reflect</param>
    /// <returns>A dictionary of known properties</returns>
    public virtual KnownPropertiesDictionary GetPropsAsXmlElements(Type aType)
    {
      var propList = new KnownPropertiesDictionary();
      var props = aType.GetProperties().Where(
           item => item.GetCustomAttributes(true).OfType<XmlElementAttribute>().Count() > 0 && item.CanWrite && item.CanRead
        || item.GetCustomAttributes(true).OfType<XmlArrayAttribute>().Count() > 0 && item.CanRead
        || item.GetCustomAttributes(true).OfType<XmlCollectionAttribute>().Count() > 0 && item.CanRead
        || item.GetCustomAttributes(true).OfType<XmlDictionaryAttribute>().Count() > 0 && item.CanRead
        ).ToList();
      if (props.Count() == 0)
        return propList;
      int n = 1000;
      int order;
      foreach (var propInfo in props)
      {
        SerializationPropertyInfo? serializePropInfo = null;
        string? elementName = null;
        int? attrOrder = null;
        XmlElementAttribute? elementAttribute = propInfo.GetCustomAttributes(true).OfType<XmlElementAttribute>().FirstOrDefault();
        XmlDictionaryAttribute? dictionaryAttribute = null;
        XmlCollectionAttribute? collectionAttribute = null;
        XmlArrayAttribute? arrayAttribute = null;

        if (elementAttribute != null)
        {
          elementName = elementAttribute.ElementName;
          attrOrder = elementAttribute.Order;
        }
        else
        {
          dictionaryAttribute = propInfo.GetCustomAttributes(true).OfType<XmlDictionaryAttribute>().FirstOrDefault();
          if (dictionaryAttribute != null)
          {
            elementName = dictionaryAttribute.ElementName;
            attrOrder = dictionaryAttribute.Order;
          }
          else
          {
            collectionAttribute = propInfo.GetCustomAttributes(true).OfType<XmlCollectionAttribute>().FirstOrDefault();
            if (collectionAttribute != null)
            {
              elementName = collectionAttribute.ElementName;
              attrOrder = collectionAttribute.Order;
            }
            else
            {
              arrayAttribute = propInfo.GetCustomAttributes(true).OfType<XmlArrayAttribute>().FirstOrDefault();
              if (arrayAttribute != null)
              {
                elementName = arrayAttribute.ElementName;
                attrOrder = arrayAttribute.Order;
              }
            }
          }
        }

        if (string.IsNullOrEmpty(elementName))
          elementName = propInfo.Name;
        if (Options?.LowercasePropertyName == true)
          elementName = LowercaseName(elementName);
        serializePropInfo = new SerializationPropertyInfo(elementName, propInfo);
        serializePropInfo.TypeInfo = GetKnownType(propInfo.PropertyType);
        if (dictionaryAttribute != null)
          serializePropInfo.CollectionInfo = CreateDictionaryInfo(propInfo, dictionaryAttribute);
        else
        if (collectionAttribute != null)
          serializePropInfo.CollectionInfo = CreateCollectionInfo(propInfo, collectionAttribute);
        else
        if (arrayAttribute != null)
          serializePropInfo.CollectionInfo = CreateCollectionInfo(propInfo, arrayAttribute);

        if (elementName != null && serializePropInfo != null)
        {
          order = ++n + 100;
          if (attrOrder != null)
            order = (int)attrOrder;
          serializePropInfo.Order = order;
          var xmlConverterAttribute = propInfo.GetCustomAttributes(true).OfType<XmlConverterAttribute>().FirstOrDefault();
          if (xmlConverterAttribute != null)
          {
            var converterType = xmlConverterAttribute.ConverterType;
            if (converterType != null)
            {
              if (!converterType.IsSubclassOf(typeof(XmlConverter)))
                throw new InternalException($"A type {converterType} must be a subclass of {typeof(XmlConverter)}");
              var constructor = converterType.GetConstructor(new Type[] { });
              if (constructor == null)
                throw new InternalException($"Converter type {converterType} does not have parameterless public constructor");
              serializePropInfo.XmlConverter = constructor.Invoke(null) as XmlConverter;
            }
          }
          propList.Add(elementName, serializePropInfo);
        }
      }
      return propList;
    }

    protected CollectionInfo CreateArrayPropertyInfo(PropertyInfo propInfo, XmlArrayAttribute arrayAttribute)
    {
      if (!propInfo.PropertyType.IsArray)
      {
        if (propInfo.PropertyType.IsDictionary())
          return CreateDictionaryInfo(propInfo, arrayAttribute);
      }
      var elementName = arrayAttribute.ElementName;
      if (string.IsNullOrEmpty(elementName))
        elementName = "";
      else if (Options?.LowercasePropertyName == true)
        elementName = LowercaseName(elementName);
      var arrayPropertyInfo = new CollectionInfo();

      var arrayItemAttrib = propInfo.GetCustomAttributes(true).OfType<XmlArrayItemAttribute>().ToList();
      foreach (var arrayItemAttribute in arrayItemAttrib)
      {
        elementName = arrayItemAttribute.ElementName;
        var itemType = arrayItemAttribute.Type;
        if (string.IsNullOrEmpty(elementName))
        {
          if (itemType == null)
            throw new InternalException($"Element name or type must be specified in ArrayItemAttribute in specification of {propInfo?.DeclaringType?.Name} type");
          elementName = itemType.Name;
        }
        else if (Options?.LowercasePropertyName == true)
          elementName = LowercaseName(elementName);
        if (itemType == null)
          itemType = typeof(object);
        var typeInfo = GetKnownType(itemType);
        var itemTypeInfo = new SerializationItemTypeInfo(elementName, typeInfo);
        arrayPropertyInfo.KnownItemTypes.Add(elementName, itemTypeInfo);
      }
      return arrayPropertyInfo;
    }

    protected CollectionInfo CreateCollectionInfo(PropertyInfo propInfo, XmlArrayAttribute arrayAttribute)
    {
      var result = CreateArrayPropertyInfo(propInfo, arrayAttribute);
      var collectionAttribute = arrayAttribute as XmlCollectionAttribute;

      Type? collectionType = null;
      if (collectionAttribute != null)
      {
        collectionType = collectionAttribute.CollectionType;
        if (!collectionType.IsEqualOrSubclassOf(propInfo.PropertyType))
          throw new InternalException($"Declared collection type {collectionType} must be a subclass of {propInfo.PropertyType}" +
            $" in declaration of property {propInfo.DeclaringType}.{propInfo.Name}");
      }
      if (collectionType == null)
        collectionType = propInfo.PropertyType;
      result.CollectionTypeInfo = GetKnownType(collectionType);
      if (result.CollectionTypeInfo == null)
        throw new InternalException($"Unknown collection type {collectionType}" +
          $" in declaration of property {propInfo.DeclaringType}.{propInfo.Name}");


      if (collectionAttribute != null)
      {
        var addMethodName = collectionAttribute?.AddMethod;
        if (addMethodName == null)
          addMethodName = "Add";
        if (collectionType != null)
          result.AddMethodInfo = collectionType.GetMethod(addMethodName);
        if (result.AddMethodInfo == null)
          throw new InternalException($"Unknown add method in {collectionType}" +
            $" in declaration of property {propInfo.DeclaringType}.{propInfo.Name}");
      }
      return result;
    }

    protected CollectionInfo CreateCollectionInfo(Type aType, XmlArrayAttribute arrayAttribute)
    {
      var collectionTypeInfo = new CollectionInfo();
      collectionTypeInfo.ItemTypeInfo = GetKnownType(aType.GenericTypeArguments.FirstOrDefault() ?? typeof(object));
      var arrayItemAttrib = aType.GetCustomAttributes(true).OfType<XmlArrayItemAttribute>().ToList();
      foreach (var arrayItemAttribute in arrayItemAttrib)
      {
        var elementName = arrayItemAttribute.ElementName;
        var itemType = arrayItemAttribute.Type;
        if (string.IsNullOrEmpty(elementName))
        {
          if (itemType == null)
            throw new InternalException($"Element name or type must be specified in ArrayItemAttribute in specification of {aType} type");
          elementName = itemType.Name;
        }
        else if (Options?.LowercasePropertyName == true)
          elementName = LowercaseName(elementName);
        if (itemType == null)
          itemType = typeof(object);
        var serializationItemTypeInfo = new SerializationItemTypeInfo(elementName, GetKnownType(itemType));
        GetKnownType(itemType).KnownItemTypes.Add(elementName, serializationItemTypeInfo);
      }
      return collectionTypeInfo;
    }

    protected DictionaryInfo CreateDictionaryInfo(PropertyInfo propInfo, XmlArrayAttribute arrayAttribute)
    {
      var dictionaryType = propInfo.PropertyType;
      if (!dictionaryType.IsDictionary(out var propertyKeyType, out var propertyValueType))
        throw new InternalException($"Property {propInfo.PropertyType}.{propInfo.Name} has XmlDictionaryItemAttribute but is not a dictionary");

      var elementName = arrayAttribute.ElementName;
      if (string.IsNullOrEmpty(elementName))
        elementName = "";
      else if (Options?.LowercasePropertyName == true)
        elementName = LowercaseName(elementName);
      var dictionaryInfo = new DictionaryInfo();

      if (arrayAttribute is XmlCollectionAttribute collectionAttribute)
      {

        if (collectionAttribute.CollectionType != null)
          dictionaryInfo.CollectionTypeInfo = GetKnownType(collectionAttribute.CollectionType);
        else
          dictionaryInfo.CollectionTypeInfo = GetKnownType(propInfo.PropertyType);

        if (arrayAttribute is XmlDictionaryAttribute dictionaryAttribute)
        {
          dictionaryInfo.KeyName = dictionaryAttribute.KeyName;
          if (dictionaryAttribute.KeyType != null)
            dictionaryInfo.KeyTypeInfo = GetKnownType(dictionaryAttribute.KeyType);
          if (dictionaryAttribute.ValueType != null)
            dictionaryInfo.ValueTypeInfo = GetKnownType(dictionaryAttribute.ValueType);
        }
      }

      if (dictionaryInfo.KeyTypeInfo != null)
      {
        if (!dictionaryInfo.KeyTypeInfo.Type.IsEqualOrSubclassOf(propertyKeyType))
          throw new InternalException($"Declared key type {dictionaryInfo.KeyTypeInfo.Type} must be equal or subclass of {propertyKeyType}");
      }
      else
      {
        dictionaryInfo.KeyTypeInfo = GetKnownType(propertyKeyType);
      }

      if (dictionaryInfo.ValueTypeInfo != null)
      {
        if (!dictionaryInfo.ValueTypeInfo.Type.IsEqualOrSubclassOf(propertyValueType))
          throw new InternalException($"Declared value type {dictionaryInfo.ValueTypeInfo.Type} must be equal or subclass of {propertyValueType}");
      }
      else
      {
        dictionaryInfo.ValueTypeInfo = GetKnownType(propertyValueType);
      }

      var dictionaryItemAttributes = propInfo.GetCustomAttributes(true).OfType<XmlDictionaryItemAttribute>().ToList();
      foreach (var dictionaryItemAttrib in dictionaryItemAttributes)
      {
        elementName = dictionaryItemAttrib.ElementName;
        var itemType = dictionaryItemAttrib.Type;
        if (string.IsNullOrEmpty(elementName))
        {
          if (itemType == null)
            throw new InternalException($"Element name or type must be specified in DictionaryItemAttribute in specification of {propInfo?.DeclaringType?.Name} type");
          elementName = itemType.Name;
        }
        else if (Options?.LowercasePropertyName == true)
          elementName = LowercaseName(elementName);
        if (itemType == null)
          itemType = typeof(object);
        var typeInfo = GetKnownType(itemType);
        var itemTypeInfo = new SerializationItemTypeInfo(elementName, typeInfo);
        dictionaryInfo.KnownItemTypes.Add(elementName, itemTypeInfo);
        itemTypeInfo.KeyName = dictionaryItemAttrib.KeyAttributeName;
        if (dictionaryItemAttrib.KeyAttributeName != null)
        {
          var genericAttributes = propInfo.PropertyType.GetGenericArguments();
          if (genericAttributes != null && genericAttributes.Count() >= 2)
          {
            var keyType = genericAttributes[0];
            var valueType = genericAttributes[1];
            dictionaryInfo.KeyTypeInfo = GetKnownType(keyType);
            dictionaryInfo.ValueTypeInfo = GetKnownType(valueType);
          }
        }

      }

      return dictionaryInfo;
    }

    protected DictionaryInfo CreateDictionaryInfo(Type aType, XmlArrayAttribute arrayAttribute)
    {
      if (!aType.IsDictionary(out var propertyKeyType, out var propertyValueType))
        throw new InternalException($"Type {aType} has XmlDictionaryItemAttribute but is not a dictionary");

      var dictionaryInfo = new DictionaryInfo();
      if (arrayAttribute is XmlCollectionAttribute collectionAttribute)
      {
        if (collectionAttribute.CollectionType != null)
          dictionaryInfo.ItemTypeInfo = GetKnownType(collectionAttribute.CollectionType);
        else
          dictionaryInfo.ItemTypeInfo = GetKnownType(aType);

        if (arrayAttribute is XmlDictionaryAttribute dictionaryAttribute)
        {
          dictionaryInfo.KeyName = dictionaryAttribute.KeyName;
          if (dictionaryAttribute.KeyType != null)
            dictionaryInfo.KeyTypeInfo = GetKnownType(dictionaryAttribute.KeyType);
          if (dictionaryAttribute.ValueType != null)
            dictionaryInfo.ValueTypeInfo = GetKnownType(dictionaryAttribute.ValueType);
        }
      }

      if (dictionaryInfo.KeyTypeInfo != null)
      {
        if (!dictionaryInfo.KeyTypeInfo.Type.IsEqualOrSubclassOf(propertyKeyType))
          throw new InternalException($"Declared key type {dictionaryInfo.KeyTypeInfo.Type} must be equal or subclass of {propertyKeyType}");
      }
      else
      {
        dictionaryInfo.KeyTypeInfo = GetKnownType(propertyKeyType);
      }

      if (dictionaryInfo.ValueTypeInfo != null)
      {
        if (!dictionaryInfo.ValueTypeInfo.Type.IsEqualOrSubclassOf(propertyValueType))
          throw new InternalException($"Declared value type {dictionaryInfo.ValueTypeInfo.Type} must be equal or subclass of {propertyValueType}");
      }
      else
      {
        dictionaryInfo.ValueTypeInfo = GetKnownType(propertyValueType);
      }

      var xmlDictionaryItemAttributes = aType.GetCustomAttributes(true).OfType<XmlDictionaryItemAttribute>().ToList();
      foreach (var xmlDictionaryItemAttrib in xmlDictionaryItemAttributes)
      {
        var elementName = xmlDictionaryItemAttrib.ElementName;
        var itemType = xmlDictionaryItemAttrib.Type;
        if (string.IsNullOrEmpty(elementName))
        {
          if (itemType == null)
            throw new InternalException($"Element name or type must be specified in DictionaryItemAttribute in specification of {aType?.DeclaringType?.Name} type");
          elementName = itemType.Name;
        }
        else if (Options?.LowercasePropertyName == true)
          elementName = LowercaseName(elementName);
        if (itemType == null)
          itemType = typeof(object);
        var itemTypeInfo = GetKnownType(itemType);
        var serializationItemTypeInfo = new SerializationItemTypeInfo(elementName, itemTypeInfo);
        dictionaryInfo.KnownItemTypes.Add(elementName, serializationItemTypeInfo);
        serializationItemTypeInfo.KeyName = xmlDictionaryItemAttrib.KeyAttributeName;
        serializationItemTypeInfo.ValueName = xmlDictionaryItemAttrib.ValueAttributeName;
      }
      return dictionaryInfo;
    }

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
      knownTextProperty.TypeInfo = GetKnownType(textProperty.PropertyType);
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
        contentPropertyInfo.TypeInfo = GetKnownType(contentProperty.PropertyType);
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
      if (xmlItemElementAttributes.Count() > 0)
      {
        foreach (var xmlItemElementAttribute in xmlItemElementAttributes)
        {
          var itemType = xmlItemElementAttribute.Type;
          if (itemType == null)
            itemType = typeof(object);
          if (!KnownTypes.TryGetValue(itemType, out var itemTypeInfo))
          {
            itemTypeInfo = GetKnownType(itemType);
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
      else
      if (a.Order < b.Order)
        return -1;
      else
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


    public virtual bool IsSimple(Type aType)
    {
      bool isSimpleValue = false;
      if (aType == typeof(string))
        isSimpleValue = true;
      else if (aType == typeof(bool))
        isSimpleValue = true;
      else if (aType == typeof(int))
        isSimpleValue = true;
      else if (aType.Name.StartsWith("`Nullable"))
        return IsSimple(aType.GetGenericArguments().First());
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
}
