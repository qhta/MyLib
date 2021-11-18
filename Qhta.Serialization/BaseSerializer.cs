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
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;

namespace Qhta.Serialization
{
  /// <summary>
  /// This basic serializer uses some <see cref="System.Xml.Serialization"/> classes,
  /// but can be uses as a base not only for <see cref="XmlSerializer"/>
  /// but for other serializers as well.
  /// </summary>
  public abstract class BaseSerializer: IDisposable
  {
    public Dictionary<XmlQualifiedName, Type> KnownTypes { get; set; } = new Dictionary<XmlQualifiedName, Type>();

    public SerializationOptions Options { get; set; } = new SerializationOptions();

    #region Creator methods

    public BaseSerializer()
    {
    }

    public BaseSerializer(Type type)
    {
      AddKnownType(type, "");
    }

    public BaseSerializer(Type type, string defaultNamespace)
    {
      AddKnownType(type, defaultNamespace);
    }

    public BaseSerializer(Type type, Type[] extraTypes)
    {
      AddKnownType(type, "");
      foreach (var item in extraTypes)
        AddKnownType(item, "");
    }

    protected void AddKnownType(Type aType, string ns)
    {
      var xmlRootAttrib = aType.GetCustomAttribute<XmlRootAttribute>();
      XmlQualifiedName aName;
      if (xmlRootAttrib?.ElementName != null)
        aName = new XmlQualifiedName(xmlRootAttrib.ElementName, ns);
      else
        aName = new XmlQualifiedName(aType.Name, ns);
      if (!KnownTypes.ContainsKey(aName))
      {
        KnownTypes.Add(aName, aType);
        foreach (var property in aType.GetProperties())
          AddKnownType(property.GetType(), ns);
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

    public abstract int WritePropertiesBase(string tag, object obj);

    public abstract int WriteCollectionBase(string propTag, ICollection collection);

    public abstract void WriteStartElement(string propTag);

    public abstract void WriteEndElement(string propTag);

    public abstract void WriteAttributeString(string attrName, string valStr);

    public abstract void WriteValue(object value);

    public abstract void WriteValue(string propTag, object value);

    #endregion

    #region Deserialize methods
    public abstract object Deserialize(Stream stream);

    public abstract object Deserialize(TextReader textReader);
    #endregion

    #region Read methods
    public abstract object DeserializeObject();

    public abstract void ReadObject(object obj);

    public abstract int ReadAttributesBase(object obj);

    public abstract int ReadExtraAttributes(object obj, IDictionary<string, object> properties);

    public abstract int ReadPropertiesBase(string tag, object obj);

    public abstract int ReadCollectionBase(string propTag, IList collection);

    public abstract bool ReadStartElement(string propTag);

    public abstract bool ReadEndElement(string propTag);

    //public abstract string ReadAttributeString(string attrName);

    //public abstract object ReadValue();

    //public abstract object ReadValue(string propTag);
    #endregion

    #region Helper methods

    public static int PropOrderComparison(SerializePropertyInfo a, SerializePropertyInfo b)
    {
      if (a.Order > b.Order)
        return 1;
      else
      if (a.Order < b.Order)
        return -1;
      else
        return String.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);
    }

    public virtual List<SerializePropertyInfo> GetPropsAsXmlAttributes(object obj)
    {
      var propList = new List<SerializePropertyInfo>();
      var props = obj.GetType().GetProperties().Where(item => item.CanWrite && item.CanRead && item.GetCustomAttributes(true)
      .OfType<XmlAttributeAttribute>().Count() > 0).ToList();
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
          if (Options.HasFlag(SerializationOptions.LowercaseAttributeName))
            attrName = LowercaseName(attrName);

          var order = ++n + 100;
          if (xmlAttribute is XmlAttribute2Attribute attr2 && attr2.Order > 0)
            order = attr2.Order;
          var serializePropInfo = new SerializePropertyInfo { Order = order, Name = attrName, PropInfo = propInfo };
          propList.Add(serializePropInfo);
        }
      }
      return propList;
    }

    public virtual List<SerializePropertyInfo> GetPropsAsXmlElements(object obj)
    {
      var propList = new List<SerializePropertyInfo>();
      var props = obj.GetType().GetProperties().Where(item => item.CanWrite && item.CanRead && item.GetCustomAttributes(true)
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
          if (Options.HasFlag(SerializationOptions.LowercasePropertyName))
            attrName = LowercaseName(attrName);

          var order = ++n + 100;
          if (xmlAttribute.Order > 0)
            order = xmlAttribute.Order;
          var serializePropInfo = new SerializePropertyInfo { Order = order, Name = attrName, PropInfo = propInfo };
          propList.Add(serializePropInfo);
        }
      }
      return propList;
    }

    public virtual List<SerializePropertyInfo> GetPropsAsXmlArrays(object obj)
    {
      var propList = new List<SerializePropertyInfo>();
      var props = obj.GetType().GetProperties().Where(item => item.CanRead && item.GetCustomAttributes(true)
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
          if (Options.HasFlag(SerializationOptions.LowercasePropertyName))
            attrName = LowercaseName(attrName);

          var order = ++n + 100;
          if (xmlAttribute.Order > 0)
            order = xmlAttribute.Order;
          var serializePropInfo = new SerializePropertyInfo { Order = order, Name = attrName, PropInfo = propInfo };
          propList.Add(serializePropInfo);
        }
      }
      return propList;
    }

    public virtual bool IsSimpleValue(object propValue)
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

    public static string LowercaseName(string str)
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
