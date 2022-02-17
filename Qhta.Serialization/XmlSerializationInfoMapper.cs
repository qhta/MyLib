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

namespace Qhta.Serialization
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
    public string BaseNamespace {get; init; }

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
    /// Types are registered by the name of the type or by the <see cref="XmlRootAttribute"/>
    /// </summary>
    public KnownTypesDictionary KnownTypes { get; init; } = new KnownTypesDictionary();

    public SerializationTypeInfo? RegisterType(Type aType)
    {
      var typeInfo = AddKnownType(aType);
      if (aType.IsAbstract)
        return null;
      #region Checking if a type qualified name was already used
      var ns = aType.Namespace ?? "";
      if (ns==BaseNamespace)      
        ns = null;
      else
      if (ns.StartsWith(BaseNamespace+"."))
        ns = ns.Substring(BaseNamespace.Length+1);

      var xmlRootAttrib = aType.GetCustomAttribute<XmlRootAttribute>(false);
      XmlQualifiedName qName;
      if (xmlRootAttrib?.ElementName != null)
        qName = new XmlQualifiedName(xmlRootAttrib.ElementName, ns);
      else
      {
        string typeName = aType.Name;
        if (typeName.Contains("`") && aType.FullName!=null)
          typeName = aType.FullName;
        qName = new XmlQualifiedName(typeName, ns);
      }
      var aName = qName.ToString();

      if (KnownTypes.TryGetValue(aName, out var oldTypeInfo))
      {
        var bType = oldTypeInfo.Type;
        if (bType != null && aType != bType && !Options.IgnoreMultipleTypeRegistration)
          throw new InternalException($"Name \"{aName}\" already defined for {bType.FullName} while registering {aType.FullName} type");
        return oldTypeInfo;
      }
      #endregion

      typeInfo.ElementName = qName.Name;
      KnownTypes.Add(aName, typeInfo);
      return typeInfo;
    }

    /// <summary>
    /// Creates the <see cref="SerializationTypeInfo"/> item and adds it to the known types dictionary.
    /// Attributes, properties and more info are reflected from the type.
    /// Ns is a prefix used to create a qualified name of the type, which is registered in the dictionary.
    /// </summary>
    /// <param name="aType">Original type to reflect</param>
    /// <param name="ns">Namespace used to create a <see cref="XmlQualifiedName"/> of the type</param>
    /// <returns>Newly created serialization type info or the already registered one.</returns>
    /// <exception cref="InternalException">
    ///   Thrown in several cases:
    ///   <list type="number">
    ///     <item>
    ///       A type has no parameterless public constructor 
    ///       and an option <see cref="SerializationOptions.IgnoreTypesWithoutParameterlessConstructor"/> is not set.
    ///     </item>
    ///     <item>
    ///       A qualified name was used for the other type.
    ///     </item>
    ///     <item>
    ///       A type was registered with a different name
    ///       and an option <see cref="SerializationOptions.IgnoreMultipleTypeRegistration"/> is not set.
    ///     </item>
    ///   </list>
    /// </exception>
    public SerializationTypeInfo AddKnownType(Type aType)
    {
      #region Checking if a type was already registered
      if (KnownTypes.TryGetValue(aType, out var knownTypeInfo))
        return knownTypeInfo;
      #endregion

      #region Creating and registering a new serialization info
      var newTypeInfo = new SerializationTypeInfo(aType);
      #endregion

      #region Checking and registering a known construtor - optional for simple types
      var constructor = aType.GetConstructor(new Type[0]);
      if (!IsSimple(aType) && !aType.IsAbstract && !aType.IsEnum)
      {
        if (constructor == null)
        {
          if (!Options.IgnoreTypesWithoutParameterlessConstructor)
            throw new InternalException($"Type {aType.Name} must have a public, parameterless constructor to allow deserialization");
        }
        newTypeInfo.KnownConstructor = constructor;
      }
      #endregion

      newTypeInfo.PropsAsAttributes = GetPropsAsXmlAttributes(aType);
      newTypeInfo.PropsAsElements = GetPropsAsXmlElements(aType);
      newTypeInfo.KnownItems = GetKnownItems(aType);
      newTypeInfo.KnownContentProperty = GetContentProperty(aType);
      newTypeInfo.KnownTextProperty = GetTextProperty(aType);
      newTypeInfo.TypeConverter = GetTypeConverter(aType);

      return newTypeInfo;
    }

    /// <summary>
    /// Registers properties which are intended to be serialized as XML attributes.
    /// These properties are marked in the type with <see cref="System.Xml.Serialization.XmlAttributeAttribute"/>
    /// or <see cref="Qhta.Serialization.XmlOrderedAttribAttribute"/> (which is a subclass of the previous one).
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
          serializePropInfo.TypeInfo = RegisterType(propInfo.PropertyType);
          var converterTypeName = propInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
          if (converterTypeName != null)
            serializePropInfo.TypeConverter = FindTypeConverter(converterTypeName);
          var converterType = propInfo.GetCustomAttribute<XmlConverterAttribute>()?.ConverterType;
          if (converterType != null)
            serializePropInfo.TypeConverter = CreateTypeConverter(converterType);
          propList.Add(serializePropInfo);
        }
      }
      return propList;
    }

    /// <summary>
    /// Registers properties which are intended to be serialized as XML element.
    /// These properties are marked in the type header with <see cref="System.Xml.Serialization.XmlElementAttribute"/>
    /// or <see cref="System.Xml.Serialization.XmlArrayAttribute"/> or <see cref="Qhta.Serialization.XmlDictionaryAttribute"/>. 
    /// For arrays also all <see cref="System.Xml.Serialization.XmlArrayItemAttribute"/> are recognized
    /// to create known item types dictionary for each array.
    /// For dictionaries also all <see cref="Qhta.Serialization.XmlDictionaryItemAttribute"/> are recognized.
    /// </summary>
    /// <param name="aType">Type to reflect</param>
    /// <returns>A dictionary of known properties</returns>
    public virtual KnownPropertiesDictionary GetPropsAsXmlElements(Type aType)
    {
      var propList = new KnownPropertiesDictionary();
      var props = aType.GetProperties().Where(
           item => item.GetCustomAttributes(true).OfType<XmlElementAttribute>().Count() > 0 && item.CanWrite && item.CanRead
        || item.GetCustomAttributes(true).OfType<XmlArrayAttribute>().Count() > 0 && item.CanRead
        || item.GetCustomAttributes(true).OfType<XmlDictionaryAttribute>().Count() > 0 && item.CanRead
        ).ToList();
      if (props.Count() == 0)
        return propList;
      int n = 1000;
      int order;
      foreach (var propInfo in props)
      {
        var elementAttribute = propInfo.GetCustomAttributes(true).OfType<XmlElementAttribute>().FirstOrDefault();
        if (elementAttribute != null)
        {
          var elementName = elementAttribute.ElementName;
          if (string.IsNullOrEmpty(elementName))
            elementName = propInfo.Name;
          if (Options?.LowercasePropertyName == true)
            elementName = LowercaseName(elementName);

          order = ++n + 100;
          if (elementAttribute.Order > 0)
            order = elementAttribute.Order;
          var serializePropInfo = new SerializationPropertyInfo(elementName, propInfo, order);
          serializePropInfo.TypeInfo = AddKnownType(propInfo.PropertyType);
          propList.Add(serializePropInfo);
        }
        var dictionaryAttribute = propInfo.GetCustomAttributes(true).OfType<XmlDictionaryAttribute>().FirstOrDefault();
        if (dictionaryAttribute != null)
          propList.Add(CreateSerializationDictionaryInfo(propInfo, dictionaryAttribute, ++n + 1000));
        else
        {
          var arrayAttribute = propInfo.GetCustomAttributes(true).OfType<XmlArrayAttribute>().FirstOrDefault();
          if (arrayAttribute != null)
            propList.Add(CreateSerializationArrayInfo(propInfo, arrayAttribute, ++n + 1000));
        }
      }
      return propList;
    }

    protected SerializationArrayInfo CreateSerializationArrayInfo(PropertyInfo propInfo, XmlArrayAttribute arrayAttribute, int defaultOrder)
    {
      var elementName = arrayAttribute.ElementName;
      if (string.IsNullOrEmpty(elementName))
        elementName = "";
      else if (Options?.LowercasePropertyName == true)
        elementName = LowercaseName(elementName);
      if (arrayAttribute.Order > 0)
        defaultOrder = arrayAttribute.Order;
      var serializeArrayInfo = new SerializationArrayInfo(elementName, propInfo, defaultOrder);
      serializeArrayInfo.TypeInfo = AddKnownType(propInfo.PropertyType);

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
        //if (itemType.Name=="Category")
        //  TestUtils.Stop();
        var typeInfo = AddKnownType(itemType);
        var itemTypeInfo = new SerializationItemTypeInfo(elementName, typeInfo);
        serializeArrayInfo.KnownItemTypes.Add(elementName, itemTypeInfo);
      }
      return serializeArrayInfo;
    }

    protected SerializationDictionaryInfo CreateSerializationDictionaryInfo(PropertyInfo propInfo, XmlDictionaryAttribute dictionaryAttribute, int defaultOrder)
    {
      var elementName = dictionaryAttribute.ElementName;
      if (string.IsNullOrEmpty(elementName))
        elementName = "";
      else if (Options?.LowercasePropertyName == true)
        elementName = LowercaseName(elementName);
      if (dictionaryAttribute.Order > 0)
        defaultOrder = dictionaryAttribute.Order;
      var serializeDictionaryInfo = new SerializationDictionaryInfo(elementName, propInfo, dictionaryAttribute.KeyType ?? typeof(object), defaultOrder);
      serializeDictionaryInfo.KeyName = dictionaryAttribute.KeyName;
      serializeDictionaryInfo.ValueType = dictionaryAttribute.ValueType;

      serializeDictionaryInfo.TypeInfo = AddKnownType(propInfo.PropertyType);

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
        var typeInfo = AddKnownType(itemType);
        var itemTypeInfo = new SerializationItemTypeInfo(elementName, typeInfo);
        serializeDictionaryInfo.KnownItemTypes.Add(elementName, itemTypeInfo);
        itemTypeInfo.KeyName = dictionaryItemAttrib.KeyName;
      }
      return serializeDictionaryInfo;
    }

    /// <summary>
    /// Registers a property which is indended to be serialized as a text content of the Xml element.
    /// This is the first property which is marked with <see cref="System.Xml.Serialization.XmlTextAttribute"/>.
    /// Note that only the first found property is used. If others are found, an exception is thrown.
    /// </summary>
    /// <param name="aType">Type to reflect</param>
    /// <returns>A serialization property info or null if not found</returns>
    /// <exception cref="InternalException">
    ///   If a property pointed out with <see cref="Qhta.Serialization.XmlContentPropertyAttribute"/> is not found.
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
      knownTextProperty.TypeInfo = AddKnownType(textProperty.PropertyType);
      return knownTextProperty;
    }

    /// <summary>
    /// Registers a property which is indended to get/set Xml content of the Xml element.
    /// This property are marked in the type header with <see cref="Qhta.Serialization.XmlContentPropertyAttribute"/>.
    /// Note that <see cref="System.Windows.Markup.ContentPropertyAttribute"/> is not used to avoid the need of System.Xaml package.
    /// </summary>
    /// <param name="aType">Type to reflect</param>
    /// <returns>A serialization property info or null if not found</returns>
    /// <exception cref="InternalException">
    ///   If a property pointed out with <see cref="Qhta.Serialization.XmlContentPropertyAttribute"/> is not found.
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
        contentPropertyInfo.TypeInfo = AddKnownType(contentProperty.PropertyType);
        return contentPropertyInfo;
      }
      return null;
    }

    /// <summary>
    /// Registers types, which are indended to be serialized as Xml children elements.
    /// These types are marked for the type with <see cref="Qhta.Serialization.XmlItemElementAttribute"/>
    /// </summary>
    /// <param name="aType">Type to reflect</param>
    /// <returns>A dictionary of known item types</returns>
    public virtual KnownTypesDictionary GetKnownItems(Type aType)
    {
      KnownTypesDictionary knownItems = new();
      var itemTagAttributes = aType.GetCustomAttributes<XmlItemElementAttribute>(false);
      if (itemTagAttributes.Count() > 0)
      {
        foreach (var itemTagAttribute in itemTagAttributes)
        {
          var itemType = itemTagAttribute.Type;
          if (itemType == null)
            itemType = typeof(object);
          if (!KnownTypes.TryGetValue(itemType, out var itemTypeInfo))
          {
            itemTypeInfo = AddKnownType(itemType);
          }
          if (itemTypeInfo != null)
            if (!string.IsNullOrEmpty(itemTagAttribute.ElementName))
              knownItems.Add(itemTagAttribute.ElementName, itemTypeInfo);
            //else
            //{
            //  if (itemTypeInfo.ElementName != null)
            //    knownItems.Add(itemTypeInfo.ElementName, itemTypeInfo);
            //}
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
