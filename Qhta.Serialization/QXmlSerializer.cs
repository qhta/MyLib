﻿namespace Qhta.Xml.Serialization;

public class QXmlSerializer
{
  private static volatile XmlSerializerNamespaces? s_defaultNamespaces;

  protected XmlDeserializationEvents _events;

  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    SerializationOptions options) :
    this(type, overrides, extraTypes, root, defaultNamespace, null, options)
  {
  }


  public QXmlSerializer(Type type, XmlRootAttribute? root, SerializationOptions options)
    : this(type, null, Type.EmptyTypes, root, null, null, options)
  {
  }


  public QXmlSerializer(Type type, Type[]? extraTypes, SerializationOptions options)
    : this(type, null, extraTypes, null, null, null, options)
  {
  }


  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, SerializationOptions options)
    : this(type, overrides, Type.EmptyTypes, null, null, null, options)
  {
  }

  public QXmlSerializer(Type type, SerializationOptions options)
    : this(type, null, Type.EmptyTypes, null, null, null, options)
  {
  }


  public QXmlSerializer(Type type, string? defaultNamespace, SerializationOptions options)
    : this(type, null, null, null, defaultNamespace, null, options)
  {
  }


  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location, SerializationOptions options)
  {
    Init(type, overrides, extraTypes, root, defaultNamespace, location, options);
  }

  public QXmlSerializer(XmlTypeMapping xmlTypeMapping, SerializationOptions options)
  {
    Init(xmlTypeMapping);
  }

  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace) :
    this(type, overrides, extraTypes, root, defaultNamespace, (string?)null)
  {
  }


  public QXmlSerializer(Type type, XmlRootAttribute? root)
    : this(type, null, Type.EmptyTypes, root, null, (string?)null)
  {
  }


  public QXmlSerializer(Type type, Type[]? extraTypes)
    : this(type, null, extraTypes, null, null, (string?)null)
  {
  }


  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides)
    : this(type, overrides, Type.EmptyTypes, null, null, (string?)null)
  {
  }

  public QXmlSerializer(Type type)
    : this(type, null, null, null, null, (string?)null)
  {
  }


  public QXmlSerializer(Type type, string? defaultNamespace)
    : this(type, null, null, null, defaultNamespace, (string?)null)
  {
  }


  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location)
  {
    Init(type, overrides, extraTypes, root, defaultNamespace, location);
  }

  public QXmlSerializer(XmlTypeMapping xmlTypeMapping)
  {
    Init(xmlTypeMapping);
  }

  protected string? DefaultNamespace => Mapper.DefaultNamespace;

  public KnownTypesCollection KnownTypes => Mapper.KnownTypes;

  public KnownNamespacesCollection KnownNamespaces => Mapper.KnownNamespaces;

  public static Type? RootType { get; protected set; }

  public static Type[]? ExtraTypes { get; protected set; }

  [DebuggerBrowsable(DebuggerBrowsableState.Never)]
  public static SerializationOptions Options { get; protected set; } = new();

  public static XmlSerializationInfoMapper Mapper { get; protected set; } = null!;


  public XmlWriterSettings XmlWriterSettings { get; } = new() { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };

  public XmlReaderSettings XmlReaderSettings { get; } = new() { IgnoreWhitespace = true };

  protected static XmlSerializerNamespaces DefaultNamespaces
  {
    get
    {
      if (s_defaultNamespaces == null)
      {
        var nss = new XmlSerializerNamespaces();
        nss.Add("xsi", XmlSchema.InstanceNamespace);
        nss.Add("xsd", XmlSchema.Namespace);
        if (s_defaultNamespaces == null) s_defaultNamespaces = nss;
      }
      return s_defaultNamespaces;
    }
  }

  protected void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location)
  {
    Init(type, overrides, extraTypes, root, defaultNamespace, location, new SerializationOptions());
  }

  protected void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location, SerializationOptions options)
  {
    var sameInitParams = RootType == type
                         && Equals(ExtraTypes, extraTypes)
                         && Options.Equals(options);
    if (sameInitParams)
      return;

    RootType = type;
    ExtraTypes = extraTypes;
    Options = options;
    Mapper = new XmlSerializationInfoMapper(options, defaultNamespace);

    RegisterType(type);
    if (extraTypes != null)
      foreach (var t in extraTypes)
        RegisterType(t);
    RegisterType(typeof(object));
    KnownNamespaces.AssignPrefixes(Mapper.DefaultNamespace ?? "");

    //KnownTypes.Dump();
  }

  protected void Init(XmlTypeMapping xmlTypeMapping)
  {
    Init(xmlTypeMapping, new SerializationOptions());
  }

  protected void Init(XmlTypeMapping xmlTypeMapping, SerializationOptions options)
  {
    throw new NotImplementedException("Init(XmlTypeMapping");
  }


  public SerializationTypeInfo? RegisterType(Type aType)
  {
    return Mapper.RegisterType(aType);
  }

  //protected void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
  //  string? location)
  //{
  //}

  //protected void Init(XmlTypeMapping xmlTypeMapping)
  //{
  //}

  public void Serialize(TextWriter textWriter, object? o)
  {
    var xmlWriter = XmlWriter.Create(textWriter, XmlWriterSettings);
    SerializeObject(xmlWriter, o);
  }

  public void Serialize(Stream stream, object? o)
  {
    var xmlWriter = XmlWriter.Create(stream);
    SerializeObject(xmlWriter, o);
  }

  public void Serialize(XmlWriter xmlWriter, object? o)
  {
    SerializeObject(xmlWriter, o);
  }

  /// <summary>
  ///   Main serialization entry
  /// </summary>
  protected void SerializeObject(XmlWriter xmlWriter, object? obj)
  {
    if (obj == null)
      return;
    var qxmlSerializerSettings = new QXmlSerializerSettings(Mapper, Options, XmlWriterSettings, XmlReaderSettings);
    var writer = new QXmlSerializationWriter(xmlWriter, qxmlSerializerSettings);
    writer.WriteObject(obj);
    xmlWriter.Flush();
  }

  public object? Deserialize(Stream stream)
  {
    var xmlReader = XmlReader.Create(stream, XmlReaderSettings);
    return Deserialize(xmlReader);
  }

  public object? Deserialize(TextReader textReader)
  {
    var xmlReader = new XmlTextReader(textReader);
    xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
    xmlReader.Normalization = true;
    xmlReader.XmlResolver = null;
    return Deserialize(xmlReader);
  }


  public object? Deserialize(XmlReader xmlReader)
  {
    return DeserializeObject(xmlReader);
  }

  /// <summary>
  ///   Main deserialization entry
  /// </summary>
  protected object? DeserializeObject(XmlReader xmlReader)
  {
    var qxmlSerializerSettings = new QXmlSerializerSettings(Mapper, Options, XmlWriterSettings, XmlReaderSettings);
    var reader = new QXmlSerializationReader(xmlReader, qxmlSerializerSettings);
    return reader.ReadObject();
  }


  //public partial bool CanDeserialize(XmlReader xmlReader);


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