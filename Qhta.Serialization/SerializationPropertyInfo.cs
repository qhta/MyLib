using System.ComponentModel;
using System.Reflection;

namespace Qhta.Xml.Serialization
{
  /// <summary>
  /// Represents the information on property needed for serialization/deserialization
  /// </summary>
  public class SerializationPropertyInfo
  {

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    /// <param name="name"Attribute or element name used for serialization></param>
    /// <param name="propertyInfo">Applied property info</param>
    /// <param name="order">Needed to sort the order of properties for serialization</param>
    public SerializationPropertyInfo(string name, PropertyInfo propertyInfo, int order=int.MaxValue)
    {
      Name = name;
      PropInfo = propertyInfo;
      Order = order;
    }

    /// <summary>
    /// Needed to sort the order of properties for serialization
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Attribute or element name used for serialization
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Applied property info
    /// </summary>
    public PropertyInfo PropInfo { get; init; }

    /// <summary>
    /// Applied type info
    /// </summary>
    public SerializationTypeInfo? TypeInfo { get; set; }

    /// <summary>
    /// Used for conversion value from/to string
    /// </summary>
    public TypeConverter? TypeConverter { get; set; }

    /// <summary>
    /// Used for conversion value from/to xml
    /// </summary>
    public XmlConverter? XmlConverter { get; set; }

    /// <summary>
    /// Optional collection info filled if a property is an array, collection or dictionary
    /// </summary>
    public CollectionInfo? CollectionInfo { get; set; }

    public override string ToString()
    {
      return $"{this.GetType().Name}({PropInfo.Name})";
    }
  }

}
