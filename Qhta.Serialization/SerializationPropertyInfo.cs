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
    /// <param name="info">Applied property info</param>
    /// <param name="order">Needed to sort the order of properties for serialization</param>
    public SerializationPropertyInfo(string name, PropertyInfo info, int order=int.MaxValue)
    {
      Name = name;
      PropInfo = info;
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

  }

}
