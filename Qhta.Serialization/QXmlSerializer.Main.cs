namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{

  protected partial void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location) => Init(type, overrides, extraTypes, root, defaultNamespace, location, new SerializationOptions());

  protected void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location, SerializationOptions options)
  {
    DefaultNamespace = defaultNamespace;
    Options = options;
    SerializationInfoMapper = new XmlSerializationInfoMapper(options, defaultNamespace ?? type.Namespace);
    RegisterType(type);
    if (extraTypes != null)
      foreach (Type t in extraTypes)
        RegisterType(t);
  }

  protected partial void Init(XmlTypeMapping xmlTypeMapping)
    => Init(xmlTypeMapping, new SerializationOptions());

  protected void Init(XmlTypeMapping xmlTypeMapping, SerializationOptions options)
  {
    throw new NotImplementedException("Init(XmlTypeMapping");
  }

  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace, SerializationOptions options) :
    this(type, overrides, extraTypes, root, defaultNamespace, null, options)
  { }


  public QXmlSerializer(Type type, XmlRootAttribute? root, SerializationOptions options)
    : this(type, null, Type.EmptyTypes, root, null, null, options) { }


  public QXmlSerializer(Type type, Type[]? extraTypes, SerializationOptions options)
    : this(type, null, extraTypes, null, null, null, options) { }


  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, SerializationOptions options)
    : this(type, overrides, Type.EmptyTypes, null, null, null, options) { }

  public QXmlSerializer(Type type, SerializationOptions options)
    : this(type, null, Type.EmptyTypes, null, null, null, options) { }


  public QXmlSerializer(Type type, string? defaultNamespace, SerializationOptions options)
    : this(type, null, null, null, defaultNamespace, null, options) { }


  public QXmlSerializer(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location, SerializationOptions options)
  {
    Init(type, overrides, extraTypes, root, defaultNamespace, location, options);
  }

  public QXmlSerializer(XmlTypeMapping xmlTypeMapping, SerializationOptions options)
  {
    Init(xmlTypeMapping);
  }

  public KnownTypesDictionary KnownTypes => SerializationInfoMapper.KnownTypes;

  public string? BaseNamespace => SerializationInfoMapper.BaseNamespace;


  [DebuggerBrowsable(DebuggerBrowsableState.Never)]
  public SerializationOptions Options { get; private set; } = new SerializationOptions();

  public XmlSerializationInfoMapper SerializationInfoMapper { get; private set; } = null!;

  public SerializationTypeInfo? RegisterType(Type aType)
  {
    return SerializationInfoMapper.RegisterType(aType);
  }

  //public SerializationTypeInfo? AddKnownType(Type aType)
  //{
  //  return SerializationInfoMapper.GetKnownType(aType);
  //}


}