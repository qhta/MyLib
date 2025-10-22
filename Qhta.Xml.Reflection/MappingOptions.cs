namespace Qhta.Xml.Reflection;

/// <summary>
/// Class containing mapping options.
/// </summary>
public class MappingOptions
{
  /// <summary>
  ///   Types to be deserialized must have parameterless constructor.
  ///   Whether a type found during scanning available types hase no public parameterless constructor,
  ///   the exception is thrown unless this option is set.
  /// </summary>
  public bool IgnoreMissingConstructor { get; set; }

  /// <summary>
  ///   Whether properties not marked with any XmlAttribute/XmlElement attributes are to be serialized.
  /// </summary>
  public bool AcceptAllProperties { get; set; } = true;

  /// <summary>
  ///   Whether only properties marked with DataMember attribute are to be serialized.
  ///  XmlAttribute/XmlElement attributes are also recognized.
  /// </summary>
  public bool AcceptDataMembers { get; set; } = true;

  /// <summary>
  ///   Whether fields are to be serialized along with properties.
  /// </summary>
  public bool AcceptFields { get; set; }

  /// <summary>
  ///   Whether simple type properties not marked with any XmlAttribute are to be serialized as attributes.
  /// </summary>
  public bool SimplePropertiesAsAttributes { get; set; } = true;

  /// <summary>
  ///   Whether members with unique types that are serialized as element are to be serialized as contentElements (without element tag).
  /// </summary>
  public bool UniqueMemberTypesAsContentElements { get; set; } = false;

  /// <summary>
  ///   Whether XML attribute names should change case on serialization.
  /// </summary>
  public NameCase AttributeNameCase { get; set; }

  /// <summary>
  ///   Whether XML element names should change case on serialization.
  /// </summary>
  public NameCase ElementNameCase { get; set; }

  /// <summary>
  ///   Generic name of the method used to specify if a property should be serialized.
  ///   Asterisk represents a property name.
  ///   The method should be a parameterless function of type boolean.
  /// </summary>
  public string CheckMethod { get; set; } = "ShouldSerialize*";

}