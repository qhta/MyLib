using System.Reflection;
using System.Xml.Serialization;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Represents the information about property needed for serialization/deserialization.
/// </summary>
public class SerializationPropertyInfo
{

  /// <summary>
  /// Constructor with parameters.
  /// </summary>
  /// <param name="name"Attribute or element name used for serialization></param>
  /// <param name="propertyInfo">Applied property info</param>
  /// <param name="order">Needed to sort the order of properties for serialization</param>
  public SerializationPropertyInfo(string name, PropertyInfo propertyInfo, int order=int.MaxValue)
  {
    Name = name;
    PropInfo = propertyInfo;
    IsNullable = PropInfo.GetCustomAttribute<XmlElementAttribute>()?.IsNullable;
    Order = order;
  }

  /// <summary>
  /// Needed to sort the order of properties for serialization.
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// Attribute or element name used for serialization.
  /// </summary>
  public string Name { get; init; }

  /// <summary>
  /// Applied property info.
  /// </summary>
  public PropertyInfo PropInfo { get;}

  public bool? IsNullable { get; init;}

  /// <summary>
  /// Applied type info
  /// </summary>
  public SerializationTypeInfo? TypeInfo { get; set; }

  /// <summary>
  /// Used for conversion value from/to string.
  /// </summary>
  public TypeConverter? TypeConverter { get; set; }

  /// <summary>
  /// Used for conversion value from/to xml.
  /// </summary>
  public XmlConverter? XmlConverter { get; set; }

  /// <summary>
  /// A method used to specify if a property should be serialized at run-time.
  /// The method should be a parameterless function of type boolean.
  /// </summary>
  public MethodInfo? ShouldSerializeMethod { get; set; }

  /// <summary>
  /// Optional collection info filled if a property is an array, collection or dictionary.
  /// </summary>
  public CollectionInfo? CollectionInfo { get; set; }

  public override string ToString()
  {
    return $"{this.GetType().Name}({PropInfo.Name})";
  }
}