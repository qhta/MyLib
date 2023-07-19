namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which can specify data of collection property, field or class that are needed for serialization/deserialization.
/// Replaces <see cref="System.Xml.Serialization.XmlArrayAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field |AttributeTargets.Class)]
public class XmlCollectionAttribute : XmlArrayAttribute
{
  /// <summary>
  ///  Default constructor.
  /// </summary>
  public XmlCollectionAttribute()
  {
  }

  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="elementName"></param>
  /// <param name="collectionType"></param>
  public XmlCollectionAttribute(string? elementName, Type? collectionType = null)
  {
    ElementName = elementName;
    CollectionType = collectionType;
  }

  /// <summary>
  /// Collection type to create.
  /// </summary>
  public Type? CollectionType { get; }

  /// <summary>
  /// A method to add an item to the collection.
  /// </summary>
  public string? AddMethod { get; set; }

  /// <summary>
  /// A type of XmlConverter which can be defined for a collection.
  /// </summary>
  public Type? XmlConverter { get; set; }
}