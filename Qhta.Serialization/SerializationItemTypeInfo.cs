using System.Reflection;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Info of an array property item
/// </summary>
public class SerializationItemTypeInfo: ITypeInfo
{

  public SerializationItemTypeInfo(string elementName, SerializationTypeInfo itemTypeInfo)
  {
    Name = new QualifiedName(elementName);
    TypeInfo = itemTypeInfo;
  }

  /// <summary>
  /// Name of the Xml/Json element
  /// </summary>
  public QualifiedName Name { get; }

  /// <summary>
  /// Refers to the existing TypeInfo
  /// </summary>
  public SerializationTypeInfo TypeInfo { get; set; }

  /// <summary>
  /// Used when this info is a dictionary item info
  /// </summary>
  public DictionaryInfo? DictionaryInfo { get; set; }

  public Type Type => TypeInfo.Type;

  /// <summary>
  /// Used when this info is a dictionary item info
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
  /// Used when this info is a dictionary item info
  /// </summary>
  public string? ValueName
  {
    get => DictionaryInfo?.ValueName;
    set
    {
      if (DictionaryInfo == null)
        DictionaryInfo = new DictionaryInfo();
      DictionaryInfo.ValueName = value;
    }
  }

  /// <summary>
  /// Used when this info is a dictionary item info
  /// </summary>
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
  /// Used when this info is a dictionary item info
  /// </summary>
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
  /// Used to add item to collection or dictionary
  /// </summary>
  public MethodInfo? AddMethod { get; set; }

  public override string ToString()
  {
    return $"{this.GetType().Name}({Type?.Name})";
  }
}