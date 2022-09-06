using System.Globalization;

namespace Qhta.Xml.Serialization;

public class SerializationOptions
{
  /// <summary>
  /// Types to be deserialized must have parameterless constructor.
  /// If a type found during scanning available types hase no public parameterless constructor, 
  /// the exception is thrown unless this option is set.
  /// </summary>
  public bool IgnoreMissingConstructor { get; set; }

  /// <summary>
  /// If properties not marked with any XmlAttribute are accepted to serialize.
  /// </summary>
  public bool AcceptAllProperties { get; set; } = true;

  /// <summary>
  /// If simple type properties not marked with any XmlAttribute are accepted to serialize as attributes.
  /// </summary>
  public bool SimplePropertiesAsAttributes { get; set; }

  /// <summary>
  /// If XML attribute names should change case on serialization.
  /// </summary>
  public SerializationCase AttributeNameCase { get; set; }

  /// <summary>
  /// If XML element names should change case on serialization.
  /// </summary>
  public SerializationCase ElementNameCase { get; set; }

  /// <summary>
  /// If enumeration value names should change case on serialization.
  /// </summary>
  public SerializationCase EnumNameCase { get; set; }

  /// <summary>
  /// If enum values letter case should be ignored on deserialization.
  /// Boolean case is always ignored.
  /// </summary>
  public bool IgnoreCaseOnEnum { get; set; }

  /// <summary>
  /// If property names serialized as Xml element should be preceded with its class name.
  /// A dot character ('.') is used as a separator (like in XAML).
  /// </summary>
  public bool PrecedePropertyNameWithClassName { get; set; }

  /// <summary>
  /// A string to markup items in collections.
  /// If not specified, items are serialized directly.
  /// </summary>
  public string? ItemTag { get; set; }

  /// <summary>
  /// Culture used in numbers during serialization/deserialization. 
  /// Default is invariant culture.
  /// </summary>
  public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

  /// <summary>
  /// Specifies that when the deserializer finds an unknown XML element, 
  /// it skips to the closing element (or ignores if it is an empty element).
  /// </summary>
  public bool IgnoreUnknownElements { get; set; }

  /// <summary>
  /// Writes xsi:nil="true" attribute when value is null.
  /// </summary>
  public bool UseNilValue { get; set; } = true;

  /// <summary>
  /// String to write as false value;
  /// </summary>
  public string FalseString { get; set; } = "false";

  /// <summary>
  /// String to write as false value;
  /// </summary>
  public string TrueString { get; set; } = "true";

  /// <summary>
  /// Format to use for DateTime value;
  /// </summary>
  public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:sszzz";

  /// <summary>
  /// Start of name of the method used to specify if a property should be serialized.
  /// The name ends with the property name.
  /// The method should be a parameterless function of type boolean.
  /// </summary>
  public string ShouldSerializeMethodPrefix { get; set; } = "ShouldSerialize";
}