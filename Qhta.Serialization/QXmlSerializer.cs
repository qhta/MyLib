namespace Qhta.Xml.Serialization;

/// <summary>
/// Flexible Xml serializer.
/// </summary>
public partial class QXmlSerializer : IXmlSerializer
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type">The type of the object that this XmlSerializer can serialize.</param>
  /// <param name="extraTypes"></param>
  /// <param name="defaultNamespace"></param>
  /// <param name="options"></param>
  public QXmlSerializer(Type type, Type[]? extraTypes, string? defaultNamespace,SerializationOptions options)
  {
    Init(type, extraTypes, defaultNamespace, options);
  }


  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type">The type of the object that this XmlSerializer can serialize.</param>
  /// <param name="options"></param>
  public QXmlSerializer(Type type, SerializationOptions options)
  {
    Init(type, null, null, options);
  }


  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type">The type of the object that this XmlSerializer can serialize.</param>
  /// <param name="extraTypes"></param>
  /// <param name="options"></param>
  public QXmlSerializer(Type type, Type[]? extraTypes, SerializationOptions options)
  {
    Init(type, extraTypes, null, options);
  }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="defaultNamespace"></param>
  /// <param name="options"></param>
  public QXmlSerializer(Type type, string? defaultNamespace, SerializationOptions options)
  {
    Init(type, null, defaultNamespace, options);
  }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="extraTypes"></param>
  /// <param name="defaultNamespace"></param>
  public QXmlSerializer(Type type, Type[]? extraTypes, string? defaultNamespace)
  {
    Init(type, extraTypes, defaultNamespace);
  }


  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="extraTypes"></param>
  public QXmlSerializer(Type type, Type[]? extraTypes)
  {
    Init(type, extraTypes, null);
  }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type"></param>
  public QXmlSerializer(Type type)
  {
    Init(type, null, null);
  }


  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="defaultNamespace"></param>
  public QXmlSerializer(Type type, string? defaultNamespace)
  {
    Init(type, null, defaultNamespace);
  }

  /// <summary>
  /// Gets the Mapper.DefaultNamespace.
  /// </summary>
  public string? DefaultNamespace => Mapper.DefaultNamespace;

  /// <summary>
  /// Gets the Mapper.KnownTypes.
  /// </summary>
  public KnownTypesCollection KnownTypes => Mapper.KnownTypes;

  /// <summary>
  /// Gets the Mapper.KnownNamespaces.
  /// </summary>
  public KnownNamespacesCollection KnownNamespaces => Mapper.KnownNamespaces;

  /// <summary>
  /// Gets the defined ExtraTypes.
  /// </summary>
  public static Type[]? ExtraTypes { get; protected set; }

  /// <summary>
  /// Gets the defined SerializationOptions.
  /// </summary>
  public static SerializationOptions Options { get; protected set; } = new();

  /// <summary>
  /// Gets the XmlSerializationInfoMapper.
  /// </summary>
  public static XmlSerializationInfoMapper Mapper { get; protected set; } = null!;

  /// <summary>
  /// Try get known type searched by the type name.
  /// </summary>
  /// <param name="typeName">Name of type to find.</param>
  /// <param name="type">Found type (or null if not found).</param>
  /// <returns>True if type found, false otherwise.</returns>
  public bool TryGetKnownType(string typeName, [NotNullWhen(true)] out Type type)
  {
    var qualifiedTypeName = Mapper.ToQualifiedName(typeName);
    if (Mapper.KnownTypes.TryGetValue(qualifiedTypeName, out var serializationTypeInfo))
    {
      type = serializationTypeInfo.Type;
      return true;
    }
    type = null!;
    return false;
  }

  /// <summary>
  /// Gets the type converter searching by the type.
  /// </summary>
  /// <param name="type">Type to search.</param>
  /// <param name="typeConverter">Found type converter (or null if not found).</param>
  /// <returns>True if type found, false otherwise.</returns>
  public bool TryGetTypeConverter(Type type, [NotNullWhen(true)] out TypeConverter typeConverter)
  {
    var qualifiedTypeName = Mapper.ToQualifiedName(type.FullName ?? "");
    if (Mapper.KnownTypes.TryGetValue(qualifiedTypeName, out var serializationTypeInfo))
    {
      typeConverter = serializationTypeInfo?.TypeConverter!;
      return typeConverter != null;
    }
    typeConverter = null!;
    return false;
  }

  /// <summary>
  /// Initializes the serializator.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="extraTypes"></param>
  /// <param name="defaultNamespace"></param>
  protected void Init(Type type, Type[]? extraTypes, string? defaultNamespace)
  {
    Init(type, extraTypes, defaultNamespace, new SerializationOptions());
  }

  /// <summary>
  /// Initializes the serializator.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="extraTypes"></param>
  /// <param name="defaultNamespace"></param>
  /// <param name="options"></param>
  protected void Init(Type type, Type[]? extraTypes, string? defaultNamespace, SerializationOptions options)
  {
    ExtraTypes = extraTypes;
    Options = options;
    Mapper = new XmlSerializationInfoMapper(options, defaultNamespace);

    RegisterType(type);
    if (extraTypes != null)
      foreach (var t in extraTypes)
        RegisterType(t);
    RegisterType(typeof(object));
    if (Options.AutoSetPrefixes)
      Mapper.AutoSetPrefixes(Options.EmitDefaultNamespacePrefix ? null : DefaultNamespace);
  }

  /// <summary>
  /// Registers a type using XmlSerializationInfoMapper
  /// </summary>
  /// <param name="type">Type to register.</param>
  /// <returns>Created or previously registered SerializationTypeInfo.</returns>
  public SerializationTypeInfo? RegisterType(Type type)
  {
    return Mapper.RegisterType(type);
  }

  /// <summary>
  /// Serializes an object to the TextWriter.
  /// </summary>
  /// <param name="textWriter">The target of serialization.</param>
  /// <param name="obj">Serialized object.</param>
  public void Serialize(TextWriter textWriter, object? obj)
  {
    if (Options.EmitNamespaces && Options.AutoSetPrefixes)
    {
      var bufWriter = new StringWriter();
      var xmlWriter = XmlDictionaryWriter.Create(bufWriter, XmlWriterSettings);
      SerializeObject(xmlWriter, obj);
      var str = bufWriter.ToString();
      if (Options.RemoveUnusedNamespaces)
      {
        if (!Writer.XsiNamespaceUsed)
          str = str.ReplaceFirst($" xmlns:xsi=\"{QXmlSerializationHelper.xsiNamespace}\"", "");
        if (!Writer.XsdNamespaceUsed)
          str = str.ReplaceFirst($" xmlns:xsd=\"{QXmlSerializationHelper.xsdNamespace}\"", "");
        foreach (var item in KnownNamespaces)
        {
          var ns = item.XmlNamespace;
          if (!Writer.NamespacesUsed.Contains(ns))
          {
            var prefix = KnownNamespaces.XmlNamespaceToPrefix[ns];
            var searchStr = $" xmlns:{prefix}=\"{ns}\"";
            str = str.ReplaceFirst(searchStr, "");
          }
        }
      }
      str = str.ReplaceFirst("encoding=\"utf-16\"", "encoding=\"utf-8\"");
      textWriter.Write(str);
    }
    else
    {
      var xmlWriter = XmlTextWriter.Create(textWriter, XmlWriterSettings);
      SerializeObject(xmlWriter, obj);
    }
  }

  /// <summary>
  /// Serializes an object to the Stream.
  /// </summary>
  /// <param name="stream">The target of serialization.</param>
  /// <param name="obj">Serialized object.</param>
  public void Serialize(Stream stream, object? obj)
  {
    var xmlWriter = XmlWriter.Create(stream);
    SerializeObject(xmlWriter, obj);
  }

  /// <summary>
  /// Serializes an object to the XmlWriter.
  /// </summary>
  /// <param name="xmlWriter">The target of serialization.</param>
  /// <param name="obj">Serialized object.</param>
  public void Serialize(XmlWriter xmlWriter, object? obj)
  {
    SerializeObject(xmlWriter, obj);
  }

  /// <summary>
  /// Deserialized and object from the stream.
  /// </summary>
  /// <param name="stream">Source of serialized data.</param>
  /// <returns>Deserialized object.</returns>
  public object? Deserialize(Stream stream)
  {
    var xmlReader = XmlReader.Create(stream, XmlReaderSettings);
    return Deserialize(xmlReader);
  }

  /// <summary>
  /// Deserialized and object from the TextReader.
  /// </summary>
  /// <param name="textReader">Source of serialized data.</param>
  /// <returns>Deserialized object.</returns>
  public object? Deserialize(TextReader textReader)
  {
    var xmlReader = new XmlTextReader(textReader);
    xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
    xmlReader.Normalization = true;
    xmlReader.XmlResolver = null;
    return Deserialize(xmlReader);
  }

  /// <summary>
  /// Deserialized and object from the XmlReader.
  /// </summary>
  /// <param name="xmlReader">Source of serialized data.</param>
  /// <returns>Deserialized object.</returns>
  public object? Deserialize(XmlReader xmlReader)
  {
    return DeserializeObject(xmlReader);
  }

}