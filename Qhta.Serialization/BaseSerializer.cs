using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;

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

    #region Creator methods

    protected BaseSerializer()
    {
    }

    protected SerializedTypeInfo AddKnownType(Type aType, string ns)
    {
      //if (aType.Name=="Legend")
      //  Debug.Assert(true);
      var xmlRootAttrib = aType.GetCustomAttribute<XmlRootAttribute>(false);
      XmlQualifiedName qName;
      if (xmlRootAttrib?.ElementName != null)
        qName = new XmlQualifiedName(xmlRootAttrib.ElementName, ns);
      else
        qName = new XmlQualifiedName(aType.Name, ns);
      var aName = qName.ToString();
      if (!KnownTypes.TryGetValue(aName, out var oldTypeInfo))
      {
        var newTypeInfo = new SerializedTypeInfo { Type = aType, ElementName = aName };

        if (!IsSimple(aType) && !aType.IsAbstract)
        {
          var constructor = aType.GetConstructor(new Type[0]);
          if (constructor == null)
            throw new InvalidOperationException($"Type {aType.Name} must have a public, parameterless constructor to allow deserialization.");
          newTypeInfo.KnownConstructor = constructor;
        }
        KnownTypes.Add(aName, newTypeInfo);
        newTypeInfo.PropsAsAttributes = GetPropsAsXmlAttributes(aType);
        newTypeInfo.PropsAsElements = GetPropsAsXmlElements(aType);
        newTypeInfo.PropsAsArrays = GetPropsAsXmlArrays(aType);
        newTypeInfo.KnownItems = GetKnownItems(aType, ns);
        newTypeInfo.TypeConverter = GetTypeConverter(aType);
        foreach (var prop in newTypeInfo.PropsAsAttributes.Values)
          AddKnownType(prop.GetType(), ns);
        foreach (var prop in newTypeInfo.PropsAsElements.Values)
          AddKnownType(prop.GetType(), ns);
        foreach (var prop in newTypeInfo.PropsAsArrays.Values)
          AddKnownType(prop.GetType(), ns);
        return newTypeInfo;
      }
      else
      {
        var bType = oldTypeInfo.Type;
        if (aType.FullName != bType.FullName)
          throw new InvalidOperationException($"Name \"{aName}\" already defined for \"{bType}\" while registering \"{aType}\" type.");
        return oldTypeInfo;
      }
    }
    #endregion

    #region Serialize methods
    public abstract void Serialize(Stream stream, object obj);

    public abstract void Serialize(TextWriter textWriter, object obj);
    #endregion

    #region Write methods
    public abstract void WriteObject(object obj);

    public abstract int WriteAttributesBase(object obj);

    public abstract int WritePropertiesBase(string elementTag, object obj);

    public abstract int WriteCollectionBase(string elementTag, string collectionTag, ICollection collection, KnownTypesDictionary itemTypes = null);

    public abstract void WriteStartElement(string elementTag);

    public abstract void WriteEndElement(string elementTag);

    public abstract void WriteAttributeString(string attrName, string valStr);

    public abstract void WriteValue(object value);

    public abstract void WriteValue(string propTag, object value);

    public abstract string EncodeStringValue(string str);


    #endregion

    #region Deserialize methods
    public abstract object Deserialize(Stream stream);

    public abstract object Deserialize(TextReader textReader);
    #endregion

    #region Read methods
    public abstract object DeserializeObject();

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

    #region Helper methods

    public static int PropOrderComparison(SerializedPropertyInfo a, SerializedPropertyInfo b)
    {
      if (a.Order > b.Order)
        return 1;
      else
      if (a.Order < b.Order)
        return -1;
      else
        return String.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);
    }

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
          if (xmlAttribute is XmlOrderedAttribAttribute attr2 && attr2.Order > 0)
            order = attr2.Order;
          var serializePropInfo = new SerializedPropertyInfo { Order = order, Name = attrName, PropInfo = propInfo };
          propList.Add(serializePropInfo);
        }
      }
      return propList;
    }

    public virtual KnownPropertiesDictionary GetPropsAsXmlElements(Type aType)
    {
      var propList = new KnownPropertiesDictionary();
      var props = aType.GetProperties().Where(item => item.CanWrite && item.CanRead && item.GetCustomAttributes(true)
      .OfType<XmlElementAttribute>().Count() > 0).ToList();
      if (props.Count() == 0)
        return propList;
      int n = 1000;
      foreach (var propInfo in props)
      {
        var xmlAttribute = propInfo.GetCustomAttributes(true).OfType<XmlElementAttribute>().FirstOrDefault();
        if (xmlAttribute != null)
        {
          var attrName = xmlAttribute.ElementName;
          if (string.IsNullOrEmpty(attrName))
            attrName = propInfo.Name;
          if (Options?.LowercasePropertyName == true)
            attrName = LowercaseName(attrName);

          var order = ++n + 100;
          if (xmlAttribute.Order > 0)
            order = xmlAttribute.Order;
          var serializePropInfo = new SerializedPropertyInfo { Order = order, Name = attrName, PropInfo = propInfo };
          propList.Add(serializePropInfo);
        }
      }
      return propList;
    }

    public virtual KnownPropertiesDictionary GetPropsAsXmlArrays(Type aType)
    {
      var propList = new KnownPropertiesDictionary();
      var props = aType.GetProperties().Where(item => item.CanRead && item.GetCustomAttributes(true)
      .OfType<XmlArrayAttribute>().Count() > 0).ToList();
      if (props.Count() == 0)
        return propList;
      int n = 2000;
      foreach (var propInfo in props)
      {
        var xmlAttribute = propInfo.GetCustomAttributes(true).OfType<XmlArrayAttribute>().FirstOrDefault();
        if (xmlAttribute != null)
        {
          var attrName = xmlAttribute.ElementName;
          if (string.IsNullOrEmpty(attrName))
            attrName = null;
          if (Options?.LowercasePropertyName == true)
            attrName = LowercaseName(attrName);

          var order = ++n + 100;
          if (xmlAttribute.Order > 0)
            order = xmlAttribute.Order;
          var serializePropInfo = new SerializedPropertyInfo { Order = order, Name = attrName, PropInfo = propInfo };
          propList.Add(serializePropInfo);
        }
      }
      return propList;
    }

    protected KnownTypesDictionary GetKnownItems(Type aType, string ns)
    {
      KnownTypesDictionary knownItems = null;
      var itemTagAttributes = aType.GetCustomAttributes<XmlItemElementAttribute>(false);
      if (itemTagAttributes.Count() > 0)
      {
        knownItems = new KnownTypesDictionary();
        foreach (var itemTagAttribute in itemTagAttributes)
        {
          var itemType = itemTagAttribute.Type;
          if (itemType == null)
            itemType = typeof(object);
          if (!KnownTypes.TryGetValue(itemType, out var itemTypeInfo))
          {
            itemTypeInfo = AddKnownType(itemType, ns);
          }
          if (!String.IsNullOrEmpty(itemTagAttribute.ElementName))
            knownItems.Add(itemTagAttribute.ElementName, itemTypeInfo);
          else
          {
            knownItems.Add(itemTypeInfo.ElementName, itemTypeInfo);
          }
        }
      }
      return knownItems;
    }

    protected TypeConverter GetTypeConverter(Type aType)
    {
      var typeConverterAttribute = aType.GetCustomAttribute<TypeConverterAttribute>();
      if (typeConverterAttribute != null)
      {
        var converterTypeName = typeConverterAttribute.ConverterTypeName;
        converterTypeName = converterTypeName.Split(',').FirstOrDefault();
        var converterType = aType.Assembly.GetType(converterTypeName);
        if (converterType != null)
        {
          var constructor = converterType.GetConstructor(new Type[0]);
          var result = (TypeConverter)constructor.Invoke(new object[0]);
          if (result.CanConvertTo(typeof(string)) && result.CanConvertFrom(typeof(string)))
            return result;
        }
        //else
        //{
        //  foreach (var type in aType.Assembly.GetTypes())
        //    Debug.WriteLine(type.FullName);
        //}
      }
      return null;
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
        return IsSimple(aType.GetGenericArguments().FirstOrDefault());
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
      foreach (var ch in text)
        if (char.IsLetter(ch) && !Char.IsUpper(ch))
          return false;
      return true;
    }
    #endregion

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
