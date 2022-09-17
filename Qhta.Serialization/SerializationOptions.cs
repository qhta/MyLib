using System.Globalization;

namespace Qhta.Xml.Serialization;

public class SerializationOptions: IEquatable<SerializationOptions>
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
  /// Writes xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" in the first element.
  /// Writes xsi:nil="true" attribute when value is null.
  /// </summary>
  public bool UseNilValue { get; set; } = true;

  /// <summary>
  /// Writes xmlns:xsd="http://www.w3.org/2001/XMLSchema" in the first element
  /// </summary>
  public bool UseXsdScheme { get; set; } = true;

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
  /// Generic name of the method used to specify if a property should be serialized.
  /// Asterisk represents a property name.
  /// The method should be a parameterless function of type boolean.
  /// </summary>
  public string CheckMethod { get; set; } = "ShouldSerialize*";

  public bool Equals(SerializationOptions? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return IgnoreMissingConstructor == other.IgnoreMissingConstructor 
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
    if (obj.GetType() != this.GetType()) return false;
    return Equals((SerializationOptions)obj);
  }

  public override int GetHashCode()
  {
    var hashCode = new HashCode();
    hashCode.Add(IgnoreMissingConstructor);
    hashCode.Add(AcceptAllProperties);
    hashCode.Add(SimplePropertiesAsAttributes);
    hashCode.Add((int)AttributeNameCase);
    hashCode.Add((int)ElementNameCase);
    hashCode.Add((int)EnumNameCase);
    hashCode.Add(IgnoreCaseOnEnum);
    hashCode.Add(PrecedePropertyNameWithClassName);
    hashCode.Add(ItemTag);
    hashCode.Add(Culture);
    hashCode.Add(IgnoreUnknownElements);
    hashCode.Add(UseNilValue);
    hashCode.Add(UseXsdScheme);
    hashCode.Add(FalseString);
    hashCode.Add(TrueString);
    hashCode.Add(DateTimeFormat);
    hashCode.Add(CheckMethod);
    return hashCode.ToHashCode();
  }
}