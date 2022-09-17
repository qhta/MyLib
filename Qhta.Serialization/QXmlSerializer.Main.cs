namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{
  private const string xsiNamespace = @"http://www.w3.org/2001/XMLSchema-instance";
  private const string xsdNamespace = @"http://www.w3.org/2001/XMLSchema";

  public XmlSerializerNamespaces Namespaces { get; private set; } = new();

  protected partial void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location) => Init(type, overrides, extraTypes, root, defaultNamespace, location, new SerializationOptions());

  protected void Init(Type type, XmlAttributeOverrides? overrides, Type[]? extraTypes, XmlRootAttribute? root, string? defaultNamespace,
    string? location, SerializationOptions options)
  {
    var sameInitParams = RootType == type
                         && Array.Equals(ExtraTypes, extraTypes)
                         && Options.Equals(options);
    if (sameInitParams)
      return;

    RootType = type;
    ExtraTypes = extraTypes;
    Options = options;
    Mapper = new XmlSerializationInfoMapper(options, defaultNamespace);

    RegisterType(type);
    if (extraTypes != null)
      foreach (Type t in extraTypes)
        RegisterType(t);
    KnownNamespaces.AssignPrefixes(Mapper.DefaultNamespace ?? "");
    KnownTypes.Dump();
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

  protected string? DefaultNamespace => Mapper.DefaultNamespace;

  public KnownTypesCollection KnownTypes => Mapper.KnownTypes;

  public KnownNamespacesCollection KnownNamespaces => Mapper.KnownNamespaces;

  public static Type? RootType { get; protected set; }

  public static Type[]? ExtraTypes { get; protected set; }

  [DebuggerBrowsable(DebuggerBrowsableState.Never)]
  public static SerializationOptions Options { get; protected set; } = new SerializationOptions();

  public static XmlSerializationInfoMapper Mapper { get; protected set; } = null!;


  public SerializationTypeInfo? RegisterType(Type aType)
  {
    return Mapper.RegisterType(aType);
  }

  //public SerializationTypeInfo? AddKnownType(Type aType)
  //{
  //  return SerializationInfoMapper.GetKnownType(aType);
  //}


}