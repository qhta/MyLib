namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which can specify missing order number.
/// Especially XmlAttribute has no "Order" property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SerializationOrderAttribute : Attribute
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public SerializationOrderAttribute()
  {
  }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="order"></param>
  public SerializationOrderAttribute(int order)
  {
    Order = order;
  }

  /// <summary>
  /// An order number for a member (XML attribute or XML element) to be emitted.
  /// </summary>
  public int Order { get; set; }
}