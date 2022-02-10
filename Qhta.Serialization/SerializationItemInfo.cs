using System;
using System.Reflection;

namespace Qhta.Serialization
{
  /// <summary>
  /// Info on an array property item
  /// </summary>
  public class SerializationItemInfo 
  {

    public SerializationItemInfo(string elementName, Type itemType)
    {
      ElementName = elementName;
      ItemType = itemType;
    }

    /// <summary>
    /// Name of the Xml/Json element
    /// </summary>
    public string ElementName {get; init; }

    /// <summary>
    /// Type of the array item
    /// </summary>
    public Type ItemType { get; init; }

    public SerializationTypeInfo? TypeInfo { get; set; }

  }

}
