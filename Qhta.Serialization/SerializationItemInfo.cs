namespace Qhta.Xml.Serialization;

/// <summary>
///   Info of an array property item
/// </summary>
public class SerializationItemInfo : ITypeNameInfo, INamedElement
{
  private string? _ClrNameNamespace;
  private string? _XmlName;
  private string? _XmlNameNamespace;

  public SerializationItemInfo()
  {
  }

  public SerializationItemInfo(SerializationTypeInfo itemTypeInfo)
  {
    TypeInfo = itemTypeInfo;
    XmlName = itemTypeInfo.XmlName;
    XmlNamespace = itemTypeInfo.XmlNamespace;
  }

  public SerializationItemInfo(string elementName, SerializationTypeInfo itemTypeInfo)
  {
    TypeInfo = itemTypeInfo;
    XmlName = elementName;
    XmlNamespace = itemTypeInfo.XmlNamespace;
  }

  public SerializationItemInfo(string elementName, string xmlNamespace, SerializationTypeInfo itemTypeInfo)
  {
    TypeInfo = itemTypeInfo;
    XmlName = elementName;
    XmlNamespace = xmlNamespace;
  }

  /// <summary>
  ///   Refers to the existing TypeInfo
  /// </summary>
  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo TypeInfo { get; set; } = null!;

  /// <summary>
  ///   Used when this info is a dictionary item info
  /// </summary>
  public DictionaryInfo? DictionaryInfo { get; set; }

  /// <summary>
  ///   Used when this info is a dictionary item info
  /// </summary>
  public string? KeyName
  {
    get => DictionaryInfo?.KeyName;
    set
    {
      if (DictionaryInfo == null)
        DictionaryInfo = new DictionaryInfo();
      DictionaryInfo.KeyName = value;
    }
  }

  /// <summary>
  ///   Used when this info is a dictionary item info
  /// </summary>
  [XmlReference]
  public SerializationTypeInfo? KeyTypeInfo
  {
    get => DictionaryInfo?.KeyTypeInfo;
    set
    {
      if (DictionaryInfo == null)
        DictionaryInfo = new DictionaryInfo();
      DictionaryInfo.KeyTypeInfo = value;
    }
  }

  /// <summary>
  ///   Used when this info is a dictionary item info
  /// </summary>
  [XmlReference]
  public SerializationTypeInfo? ValueTypeInfo
  {
    get => DictionaryInfo?.ValueTypeInfo;
    set
    {
      if (DictionaryInfo == null)
        DictionaryInfo = new DictionaryInfo();
      DictionaryInfo.ValueTypeInfo = value;
    }
  }

  /// <summary>
  ///   Preset value of the item
  /// </summary>
  public object? Value { get; set; }

  /// <summary>
  ///   Used to add item to collection or dictionary
  /// </summary>
  public MethodInfo? AddMethod { get; set; }

  /// <summary>
  ///   Name of the Xml element
  /// </summary>
  [XmlAttribute]
  public string XmlName
  {
    get => _XmlName ?? TypeInfo?.XmlName ?? "";
    set => _XmlName = value;
  }

  /// <summary>
  ///   XmlNamespace of the element
  /// </summary>
  [XmlAttribute]
  public string? XmlNamespace
  {
    get => _XmlNameNamespace ?? TypeInfo?.XmlNamespace;
    set => _XmlNameNamespace = value;
  }

  /// <summary>
  ///   ClrNamespace of the element
  /// </summary>
  [XmlAttribute]
  public string? ClrNamespace
  {
    get => _ClrNameNamespace ?? TypeInfo?.ClrNamespace;
    set => _ClrNameNamespace = value;
  }

  public Type Type => TypeInfo.Type;

  public override string ToString()
  {
    return $"{GetType().Name}({Type?.Name})";
  }
}