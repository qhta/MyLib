using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{

  protected XmlDeserializationEvents _events = new XmlDeserializationEvents();

  protected string? DefaultNamespace {get; set;}

  protected static XmlSerializerNamespaces DefaultNamespaces
  {
    get
    {
      if (s_defaultNamespaces == null)
      {
        XmlSerializerNamespaces nss = new XmlSerializerNamespaces();
        nss.Add("xsi", XmlSchema.InstanceNamespace);
        nss.Add("xsd", XmlSchema.Namespace);
        if (s_defaultNamespaces == null)
        {
          s_defaultNamespaces = nss;
        }
      }
      return s_defaultNamespaces;
    }
  }
  private static volatile XmlSerializerNamespaces? s_defaultNamespaces;

  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace) :
      this(type, overrides, extraTypes, root, defaultNamespace, (string?)null) { }

  
  public QXmlSerializer(Type type, XmlRootAttribute? root) 
    : this(type, null, Type.EmptyTypes, root, null, (string?)null) { }

  
  public QXmlSerializer(Type type, Type[]? extraTypes) 
    : this(type, null, extraTypes, null, null, (string?)null) { }

  
  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides)
    : this(type, overrides, Type.EmptyTypes, null, null, (string?)null) { }

  public QXmlSerializer(Type type)
    : this(type, null, null, null, null, (string?)null) { }


  public QXmlSerializer(Type type, string? defaultNamespace)
  : this (type, null, null, null, defaultNamespace, (string?)null) {}


  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location)
  {
    Init(type, overrides, extraTypes, root, defaultNamespace, location);
  }

  public QXmlSerializer(XmlTypeMapping xmlTypeMapping)
  {
    Init(xmlTypeMapping);
  }

  protected partial void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location);

  protected partial void Init(XmlTypeMapping xmlTypeMapping);

  public void Serialize(TextWriter textWriter, object? o)
  {
    Serialize(textWriter, o, null);
  }

  public void Serialize(TextWriter textWriter, object? o, XmlSerializerNamespaces? namespaces)
  {
    XmlWriter xmlWriter = XmlWriter.Create(textWriter);
    Serialize(xmlWriter, o, namespaces);
  }

  public void Serialize(Stream stream, object? o)
  {
    Serialize(stream, o, null);
  }

  public void Serialize(Stream stream, object? o, XmlSerializerNamespaces? namespaces)
  {
    XmlWriter xmlWriter = XmlWriter.Create(stream);
    Serialize(xmlWriter, o, namespaces);
  }

  public void Serialize(XmlWriter xmlWriter, object? o)
  {
    Serialize(xmlWriter, o, null);
  }

  public void Serialize(XmlWriter xmlWriter, object? o, XmlSerializerNamespaces? namespaces)
  {
    Serialize(xmlWriter, o, namespaces, null);
  }

  public void Serialize(XmlWriter xmlWriter, object? o, XmlSerializerNamespaces? namespaces, string? encodingStyle)
  {
    Serialize(xmlWriter, o, namespaces, encodingStyle, null);
  }

  public void Serialize(XmlWriter xmlWriter, object? o, XmlSerializerNamespaces? namespaces, string? encodingStyle, string? id)
  {
    SerializeObject(xmlWriter, o, namespaces, encodingStyle, id);
  }

  protected partial void SerializeObject(XmlWriter xmlWriter, object? obj, XmlSerializerNamespaces? namespaces, string? encodingStyle, string? id);

  public object? Deserialize(Stream stream)
  {
    XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings() { IgnoreWhitespace = true });
    return Deserialize(xmlReader, null);
  }

  public object? Deserialize(TextReader textReader)
  {
    XmlTextReader xmlReader = new XmlTextReader(textReader);
    xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
    xmlReader.Normalization = true;
    xmlReader.XmlResolver = null;
    return Deserialize(xmlReader, null);
  }

  
  public object? Deserialize(XmlReader xmlReader)
  {
    return Deserialize(xmlReader, null);
  }

  
  public object? Deserialize(XmlReader xmlReader, XmlDeserializationEvents events)
  {
    return DeserializeObject(xmlReader, null, events);
  }

  
  public object? Deserialize(XmlReader xmlReader, string? encodingStyle)
  {
    return DeserializeObject(xmlReader, encodingStyle, _events);
  }

  public object? Deserialize(XmlReader xmlReader, string? encodingStyle, XmlDeserializationEvents events)
  {
    return DeserializeObject(xmlReader, encodingStyle, events);
  }

  protected partial object? DeserializeObject(XmlReader xmlReader, string? encodingStyle, XmlDeserializationEvents events);


  public partial bool CanDeserialize(XmlReader xmlReader);


  public event XmlNodeEventHandler UnknownNode
  {
    add => _events.OnUnknownNode += value;
    remove => _events.OnUnknownNode -= value;
  }

  public event XmlAttributeEventHandler UnknownAttribute
  {
    add => _events.OnUnknownAttribute += value;
    remove => _events.OnUnknownAttribute -= value;
  }

  public event XmlElementEventHandler UnknownElement
  {
    add => _events.OnUnknownElement += value;
    remove => _events.OnUnknownElement -= value;
  }

  public event UnreferencedObjectEventHandler UnreferencedObject
  {
    add => _events.OnUnreferencedObject += value;
    remove => _events.OnUnreferencedObject -= value;
  }

}

