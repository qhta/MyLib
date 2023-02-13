using Qhta.Conversion;

namespace Qhta.Xml.Serialization;

public class SerializationOptions : IEquatable<SerializationOptions>
{
  /// <summary>
  /// Whether tag namespaces should be written.
  /// </summary>
  public bool EmitNamespaces { get; set; } = true;

  /// <summary>
  /// Whether tag namespace prefixes should be defined in the root start Xml element and used in document content. 
  /// If this option is false and <see cref="EmitNamespaces"/> is true, then namespaces are written as full strings
  /// in "xmlns=" attributes in the whole document.
  /// </summary>
  public bool AutoSetPrefixes { get; set; } = true;

  /// <summary>
  /// Whether unused tag namespaces should be removed on serialization.
  /// </summary>
  public bool RemoveUnusedNamespaces { get; set; } = true;

  /// <summary>
  ///   Types to be deserialized must have parameterless constructor.
  ///   Whether a type found during scanning available types hase no public parameterless constructor,
  ///   the exception is thrown unless this option is set.
  /// </summary>
  public bool IgnoreMissingConstructor { get; set; }

  /// <summary>
  ///   Whether properties not marked with any XmlAttribute are to be serialized.
  /// </summary>
  public bool AcceptAllProperties { get; set; } = true;

  /// <summary>
  ///   Whether fields are to be serialized along with properties.
  /// </summary>
  public bool AcceptFields { get; set; }

  /// <summary>
  ///   Whether simple type properties not marked with any XmlAttribute are to be serialized as attributes.
  /// </summary>
  public bool SimplePropertiesAsAttributes { get; set; } = true;

  /// <summary>
  ///   Whether XML attribute names should change case on serialization.
  /// </summary>
  public SerializationCase AttributeNameCase { get; set; }

  /// <summary>
  ///   Whether XML element names should change case on serialization.
  /// </summary>
  public SerializationCase ElementNameCase { get; set; }

  /// <summary>
  ///   Whether enumeration value names should change case on serialization.
  /// </summary>
  public SerializationCase EnumNameCase { get; set; }

  /// <summary>
  ///   Whether enum values letter case should be ignored on deserialization.
  ///   Boolean case is always ignored.
  /// </summary>
  public bool IgnoreCaseOnEnum { get; set; }

  /// <summary>
  ///   Whether property names serialized as Xml element should be preceded with its class name.
  ///   A dot character ('.') is used as a separator (like in XAML).
  /// </summary>
  public bool PrecedePropertyNameWithClassName { get; set; }

  /// <summary>
  ///   A string to markup items in collections.
  ///   If not specified, items are serialized with their type tags.
  /// </summary>
  public string? ItemTag { get; set; }

  /// <summary>
  ///   Culture used in numbers during serialization/deserialization.
  ///   Default is invariant culture.
  /// </summary>
  public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

  /// <summary>
  ///   Specifies that when the deserializer finds an unknown XML element,
  ///   it skips to the closing element (or ignores if it is an empty element).
  /// </summary>
  public bool IgnoreUnknownElements { get; set; }

  /// <summary>
  ///   Writes xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" in the first element.
  ///   Writes xsi:nil="true" attribute when value is null.
  /// </summary>
  public bool UseNilValue { get; set; } = true;

  /// <summary>
  ///   Writes xmlns:xsd="http://www.w3.org/2001/XMLSchema" in the first element.
  /// </summary>
  public bool UseXsdScheme { get; set; } = true;

  /// <summary>
  ///   String to write as false value;
  /// </summary>
  public string FalseString { get; set; } = "false";

  /// <summary>
  ///   String to write as false value;
  /// </summary>
  public string TrueString { get; set; } = "true";

  /// <summary>
  ///   Conversion options for default TypeConverter
  /// </summary>
  public ConversionOptions ConversionOptions { get; set; } = new()
  {
    DateTimeSeparator = 'T', ShowFullTime = true, ShowTimeZone = true
  };

  /// <summary>
  ///   Format to use for DateTime value;
  /// </summary>
  public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:sszzz";

  /// <summary>
  ///   Generic name of the method used to specify if a property should be serialized.
  ///   Asterisk represents a property name.
  ///   The method should be a parameterless function of type boolean.
  /// </summary>
  public string CheckMethod { get; set; } = "ShouldSerialize*";

  public bool Equals(SerializationOptions? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return 
      EmitNamespaces == other.EmitNamespaces
      && AutoSetPrefixes == other.AutoSetPrefixes
      && RemoveUnusedNamespaces == other.RemoveUnusedNamespaces
      && IgnoreMissingConstructor == other.IgnoreMissingConstructor
      && AcceptAllProperties == other.AcceptAllProperties
      && SimplePropertiesAsAttributes == other.SimplePropertiesAsAttributes
      && AttributeNameCase == other.AttributeNameCase
      && ElementNameCase == other.ElementNameCase
      && EnumNameCase == other.EnumNameCase
      && IgnoreCaseOnEnum == other.IgnoreCaseOnEnum
      && PrecedePropertyNameWithClassName == other.PrecedePropertyNameWithClassName
      && ItemTag == other.ItemTag
      && Culture.Equals(other.Culture)
      && IgnoreUnknownElements == other.IgnoreUnknownElements
      && UseNilValue == other.UseNilValue
      && UseXsdScheme == other.UseXsdScheme
      && FalseString == other.FalseString
      && TrueString == other.TrueString
      && DateTimeFormat == other.DateTimeFormat
      && CheckMethod == other.CheckMethod;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != GetType()) return false;
    return Equals((SerializationOptions)obj);
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}