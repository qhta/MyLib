using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using Qhta.TestHelper;

namespace Qhta.Serialization
{
  /// <summary>
  /// This basic serializer uses some <see cref="System.Xml.Serialization"/> classes,
  /// but can be uses as a base not only for <see cref="XmlSerializer"/>
  /// but for other serializers as well.
  /// </summary>
  public abstract class BaseSerializer : IXSerializer, IDisposable
  {
    public KnownTypesDictionary KnownTypes { get; init; } = new KnownTypesDictionary();

    public SerializationOptions Options { get; init; } = new SerializationOptions();

    #region Create and get SerializationTypeInfo methods

    protected XmlSerializationInfoMapper SerializationInfoMapper { get; init; } = new XmlSerializationInfoMapper();

    protected SerializationTypeInfo? AddKnownType(Type aType, string? ns=null)
    {
      return SerializationInfoMapper.AddKnownType(KnownTypes, aType, ns);
    }


    //{
    //  if (aType.Name=="Property")
    //    TestUtils.Stop();
    //  var xmlRootAttrib = aType.GetCustomAttribute<XmlRootAttribute>(false);
    //  XmlQualifiedName qName;
    //  if (xmlRootAttrib?.ElementName != null)
    //    qName = new XmlQualifiedName(xmlRootAttrib.ElementName, ns);
    //  else
    //    qName = new XmlQualifiedName(aType.Name, ns);
    //  var aName = qName.ToString();
    //  if (!KnownTypes.TryGetValue(aName, out var oldTypeInfo))
    //  {
    //    var newTypeInfo = new SerializationTypeInfo(aName, aType);

    //    if (!IsSimple(aType) && !aType.IsAbstract && !aType.IsEnum)
    //    {
    //      var constructor = aType.GetConstructor(new Type[0]);
    //      if (constructor == null)
    //      {
    //        if (Options.IgnoreTypesWithoutParameterlessConstructor)
    //          return null;
    //        throw new InternalException($"Type {aType.Name} must have a public, parameterless constructor to allow deserialization");
    //      }
    //      newTypeInfo.KnownConstructor = constructor;
    //    }
    //    KnownTypes.Add(aName, newTypeInfo);
    //    newTypeInfo.PropsAsAttributes = GetPropsAsXmlAttributes(aType);
    //    newTypeInfo.PropsAsElements = GetPropsAsXmlElements(aType);
    //    newTypeInfo.KnownItems = GetKnownItems(aType, ns);
    //    newTypeInfo.TypeConverter = GetTypeConverter(aType);
    //    foreach (var prop in newTypeInfo.PropsAsAttributes.Values)
    //      AddKnownType(prop.GetType(), ns);
    //    foreach (var prop in newTypeInfo.PropsAsElements.Values)
    //      AddKnownType(prop.GetType(), ns);

    //    //var contentPropertyAttrib = aType.GetCustomAttribute<ContentPropertyAttribute>(true);
    //    //if (contentPropertyAttrib != null)
    //    //{
    //    //  var contentPropertyName = contentPropertyAttrib.Name;
    //    //  var contentProperty = aType.GetProperty(contentPropertyName);
    //    //  if (contentProperty == null)
    //    //    throw new InternalException($"Content property \"{contentPropertyName}\" in {aType.Name} not found");
    //    //  var contentPropertyTypeInfo = AddKnownType(contentProperty.PropertyType);
    //    //  newTypeInfo.KnownContentProperty = new SerializationPropertyInfo(contentPropertyName, contentProperty, contentPropertyTypeInfo);
    //    //}

    //    //var textProperty = aType.GetProperties().Where(item =>
    //    //    item.CanWrite && item.CanRead &&
    //    //    item.GetCustomAttributes(true).OfType<XmlTextAttribute>().Count() > 0).FirstOrDefault();
    //    //if (textProperty != null)
    //    //{
    //    //  var textPropertyTypeInfo = AddKnownType(textProperty.PropertyType);
    //    //  newTypeInfo.KnownTextProperty = new SerializationPropertyInfo("", textProperty, textPropertyTypeInfo);
    //    //}
    //    return newTypeInfo;
    //  }
    //  else
    //  {
    //    var bType = oldTypeInfo.Type;
    //    if (bType != null && aType.Name != bType.Name)
    //      throw new InternalException($"Name \"{aName}\" already defined for \"{bType}\" while registering \"{aType}\" type");
    //    return oldTypeInfo;
    //  }
    //}
    #endregion

    #region Serialize methods
    public abstract void Serialize(Stream stream, object obj);

    public abstract void Serialize(TextWriter textWriter, object obj);

    public abstract string Serialize(object obj);

    #endregion

    #region Write methods
    public abstract void WriteObject(IXWriter writer, object obj);

    public abstract void WriteObjectInterior(IXWriter writer, string? tag, object obj);

    public abstract int WriteAttributesBase(IXWriter writer, object obj);

    public abstract int WritePropertiesBase(IXWriter writer, string? elementTag, object obj);

    public abstract int WriteCollectionBase(IXWriter writer, string? elementTag, string? collectionTag, ICollection collection, KnownTypesDictionary? itemTypes = null);

    public abstract void WriteStartElement(IXWriter writer, string elementTag);

    public abstract void WriteEndElement(IXWriter writer, string elementTag);

    public abstract void WriteAttributeString(IXWriter writer, string attrName, string valStr);

    public abstract void WriteValue(IXWriter writer, object? value);

    public abstract void WriteValue(IXWriter writer, string propTag, object value);

    public abstract string EncodeStringValue(string str);


    #endregion

    #region Deserialize methods
    public abstract object? Deserialize(Stream stream);

    public abstract object? Deserialize(TextReader textReader);

    public abstract object? Deserialize(string str);

    #endregion

    #region Read methods
    //public abstract void ReadObject(object obj);

    //public abstract int ReadAttributesBase(object obj);

    //public abstract int ReadExtraAttributes(object obj, IDictionary<string, object> properties);

    //public abstract int ReadPropertiesBase(string tag, object obj);

    //public abstract int ReadCollectionBase(string propTag, IList collection);

    //public abstract bool ReadStartElement(string propTag);

    //public abstract bool ReadEndElement(string propTag);

    //public abstract string ReadAttributeString(string attrName);

    //public abstract object ReadValue();

    //public abstract object ReadValue(string propTag);
    #endregion

    //#region Helper methods

    //public static int PropOrderComparison(SerializationPropertyInfo a, SerializationPropertyInfo b)
    //{
    //  if (a.Order > b.Order)
    //    return 1;
    //  else
    //  if (a.Order < b.Order)
    //    return -1;
    //  else
    //    return String.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);
    //}

    //public virtual KnownPropertiesDictionary GetPropsAsXmlAttributes(Type aType)
    //{
    //  var propList = new KnownPropertiesDictionary();
    //  var props = aType.GetProperties().Where(item => item.CanWrite && item.CanRead
    //    && item.GetCustomAttributes(true).OfType<XmlAttributeAttribute>().Count() > 0).ToList();
    //  if (props.Count() == 0)
    //    return propList;
    //  int n = 0;
    //  foreach (var propInfo in props)
    //  {
    //    var xmlAttribute = propInfo.GetCustomAttributes(true).OfType<XmlAttributeAttribute>().FirstOrDefault();
    //    if (xmlAttribute != null)
    //    {
    //      var attrName = xmlAttribute.AttributeName;
    //      if (string.IsNullOrEmpty(attrName))
    //        attrName = propInfo.Name;
    //      if (Options?.LowercaseAttributeName == true)
    //        attrName = LowercaseName(attrName);

    //      var order = ++n + 100;
    //      if (xmlAttribute is XmlOrderedAttribAttribute attr2 && attr2.Order != null)
    //        order = (int)attr2.Order;
    //      var propTypeInfo = AddKnownType(propInfo.PropertyType);
    //      var serializePropInfo = new SerializationPropertyInfo(attrName, propInfo, propTypeInfo, Options, order);
    //      var converterTypeName = propInfo.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
    //      if (converterTypeName!= null)
    //        serializePropInfo.TypeConverter = FindTypeConverter(converterTypeName);
    //      var converterType = propInfo.GetCustomAttribute<XmlConverterAttribute>()?.ConverterType;
    //      if (converterType != null)
    //        serializePropInfo.TypeConverter = CreateTypeConverter(converterType);
    //      propList.Add(serializePropInfo);
    //    }
    //  }
    //  return propList;
    //}

    //protected TypeConverter? FindTypeConverter(string typeName)
    //{
    //  var type = Assembly.GetExecutingAssembly().GetType(typeName);
    //  if (type == null)
    //    type = Assembly.GetCallingAssembly().GetType(typeName);
    //  if (type == null)
    //    throw new InternalException($"Type converter \"{typeName}\" not found");
    //  return CreateTypeConverter(type);
    //}

    //protected TypeConverter? CreateTypeConverter(Type type)
    //{
    //  if (!type.IsSubclassOf(typeof(TypeConverter)))
    //    throw new InternalException($"Declared type converter \"{type.Name}\" must be a subclass of {typeof(TypeConverter).FullName}");
    //  var constructor = type.GetConstructor(new Type[0]);
    //  if (constructor == null)
    //    throw new InternalException($"Declared type converter \"{type.Name}\" must have a public parameterless constructor");
    //  return constructor.Invoke(null) as TypeConverter;
    //}
    //public virtual KnownPropertiesDictionary GetPropsAsXmlElements(Type aType)
    //{
    //  var propList = new KnownPropertiesDictionary();
    //  var props = aType.GetProperties().Where(
    //       item => item.GetCustomAttributes(true).OfType<XmlElementAttribute>().Count() > 0 && item.CanWrite && item.CanRead
    //    || item.GetCustomAttributes(true).OfType<XmlArrayAttribute>().Count() > 0 && item.CanRead).ToList();
    //  if (props.Count() == 0)
    //    return propList;
    //  int n = 1000;
    //  int order;
    //  foreach (var propInfo in props)
    //  {
    //    var elementAttribute = propInfo.GetCustomAttributes(true).OfType<XmlElementAttribute>().FirstOrDefault();
    //    if (elementAttribute != null)
    //    {
    //      var elementName = elementAttribute.ElementName;
    //      if (string.IsNullOrEmpty(elementName))
    //        elementName = propInfo.Name;
    //      if (Options?.LowercasePropertyName == true)
    //        elementName = LowercaseName(elementName);

    //      order = ++n + 100;
    //      if (elementAttribute.Order > 0)
    //        order = elementAttribute.Order;
    //      var propTypeInfo = AddKnownType(propInfo.PropertyType);
    //      var serializePropInfo = new SerializationPropertyInfo(elementName, propInfo, propTypeInfo, Options, order);
    //      propList.Add(serializePropInfo);
    //    }
    //    var arrayAttribute = propInfo.GetCustomAttributes(true).OfType<XmlArrayAttribute>().FirstOrDefault();
    //    if (arrayAttribute != null)
    //    {
    //      var elementName = arrayAttribute.ElementName;
    //      if (string.IsNullOrEmpty(elementName))
    //        elementName = "";
    //      else if (Options?.LowercasePropertyName == true)
    //        elementName = LowercaseName(elementName);
    //      order = ++n + 100;
    //      if (arrayAttribute.Order > 0)
    //        order = arrayAttribute.Order;
    //      var propTypeInfo = AddKnownType(propInfo.PropertyType);
    //      var serializeArrayInfo = new SerializationArrayInfo(elementName, propInfo, propTypeInfo, Options, order);
    //      propList.Add(serializeArrayInfo);

    //      var arrayItemAttributes = propInfo.GetCustomAttributes(true).OfType<XmlArrayItemAttribute>().ToList();
    //      foreach (var arrayItemAttribute in arrayItemAttributes)
    //      {
    //        elementName = arrayItemAttribute.ElementName;
    //        var elementType = arrayItemAttribute.Type;
    //        if (string.IsNullOrEmpty(elementName))
    //        {
    //          if (elementType == null)
    //            throw new InternalException($"Element name or type must be specified in ArrayItemAttribute in specification of {aType.Name} type");
    //          elementName = elementType.Name;
    //        }
    //        else if (Options?.LowercasePropertyName == true)
    //          elementName = LowercaseName(elementName);
    //        if (elementType == null)
    //          elementType = typeof(object);
    //        serializeArrayInfo.Add(elementName, elementType);
    //      }
    //    }
    //  }
    //  return propList;
    //}

    ////public virtual KnownPropertiesDictionary GetPropsAsXmlArrays(Type aType)
    ////{
    ////  var propList = new KnownPropertiesDictionary();
    ////  var props = aType.GetProperties().Where(item => item.CanRead && item.GetCustomAttributes(true)
    ////  .OfType<XmlArrayAttribute>().Count() > 0).ToList();
    ////  if (props.Count() == 0)
    ////    return propList;
    ////  int n = 2000;
    ////  foreach (var propInfo in props)
    ////  {
    ////    var arrayAttribute = propInfo.GetCustomAttributes(true).OfType<XmlArrayAttribute>().FirstOrDefault();
    ////    if (arrayAttribute != null)
    ////    {
    ////      var attrName = arrayAttribute.ElementName;
    ////      if (string.IsNullOrEmpty(attrName))
    ////        attrName = null;
    ////      if (Options?.LowercasePropertyName == true)
    ////        attrName = LowercaseName(attrName);

    ////      var order = ++n + 100;
    ////      if (arrayAttribute.Order > 0)
    ////        order = arrayAttribute.Order;
    ////      var serializePropInfo = new SerializedPropertyInfo(attrName, propInfo, order);
    ////      propList.Add(serializePropInfo);
    ////    }
    ////  }
    ////  return propList;
    ////}

    //protected KnownTypesDictionary GetKnownItems(Type aType, string? ns)
    //{
    //  KnownTypesDictionary knownItems = new();
    //  var itemTagAttributes = aType.GetCustomAttributes<XmlItemElementAttribute>(false);
    //  if (itemTagAttributes.Count() > 0)
    //  {
    //    foreach (var itemTagAttribute in itemTagAttributes)
    //    {
    //      var itemType = itemTagAttribute.Type;
    //      if (itemType == null)
    //        itemType = typeof(object);
    //      if (!KnownTypes.TryGetValue(itemType, out var itemTypeInfo))
    //      {
    //        itemTypeInfo = AddKnownType(itemType, ns);
    //      }
    //      if (itemTypeInfo!=null)
    //        if (!string.IsNullOrEmpty(itemTagAttribute.ElementName))
    //          knownItems.Add(itemTagAttribute.ElementName, itemTypeInfo);
    //        else
    //        {
    //          if (itemTypeInfo.ElementName!=null)
    //            knownItems.Add(itemTypeInfo.ElementName, itemTypeInfo);
    //        }
    //    }
    //  }
    //  return knownItems;
    //}

    //protected TypeConverter? GetTypeConverter(Type aType)
    //{
    //  var typeConverterAttribute = aType.GetCustomAttribute<TypeConverterAttribute>();
    //  if (typeConverterAttribute != null)
    //  {
    //    var converterTypeName = typeConverterAttribute.ConverterTypeName;
    //    if (converterTypeName == null)
    //      throw new InternalException($"Converter type name in a TypeConverter attribute must not be null");
    //    converterTypeName = converterTypeName.Split(',').First();
    //    var converterType = aType.Assembly.GetType(converterTypeName);
    //    if (converterType != null)
    //    {
    //      var constructor = converterType.GetConstructor(new Type[0]);
    //      if (constructor == null)
    //        throw new InternalException($"A TypeConverter {converterType} must have a public unparameterized contstructor");
    //      var result = (TypeConverter)(constructor.Invoke(new object[0]));
    //      if (result.CanConvertTo(typeof(string)) && result.CanConvertFrom(typeof(string)))
    //        return result;
    //    }
    //  }
    //  return null;
    //}

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
    //#endregion

      #region IDisposable implementation
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BaseSerializer()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}
