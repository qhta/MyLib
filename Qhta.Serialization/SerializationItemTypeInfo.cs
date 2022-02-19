using System;
using System.Reflection;

namespace Qhta.Xml.Serialization
{
  /// <summary>
  /// Info of an array property item
  /// </summary>
  public class SerializationItemTypeInfo: ITypeInfo
  {

    public SerializationItemTypeInfo(string elementName, SerializationTypeInfo itemTypeInfo)
    {
      ElementName = elementName;
      TypeInfo = itemTypeInfo;
    }

    /// <summary>
    /// Name of the Xml/Json element
    /// </summary>
    public string ElementName {get; init; }

    /// <summary>
    /// Refers to the existing TypeInfo
    /// </summary>
    public SerializationTypeInfo TypeInfo { get; set; }

    public Type Type => TypeInfo.Type;

    /// <summary>
    /// Used when this info is a dictionary item info
    /// </summary>
    public string? KeyName { get; set; }

  }

}
