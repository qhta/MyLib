using System.ComponentModel;
using System.Reflection;

namespace Qhta.Serialization
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
    public int Order { get; init; }

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
  }

}
