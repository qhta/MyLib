using System.Globalization;

namespace Qhta.Xml.Serialization
{
  public class SerializationOptions
  {
    /// <summary>
    /// Types to be deserialized must have parameterless constructor.
    /// If a type found during scanning available types hase no public parameterless constructor, 
    /// the exception is thrown unless this option is set.
    /// </summary>
    public bool IgnoreTypesWithoutParameterlessConstructor { get; set; }

    /// <summary>
    /// Usually types are registered for serialization/deserialization with unique names.
    /// So if an application tries to register the same type with two different names,
    /// the exception is thrown unless this option is set.
    /// Note that when a type is registered with multiple names, 
    /// it can be recognized with different names on deserialization.
    /// On serialization, only one name is used.
    /// </summary>
    public bool IgnoreMultipleTypeRegistration { get; set; }

    /// <summary>
    /// If properties not marked with any XmlAttribute are accepted to serialize.
    /// </summary>
    public bool AcceptAllNotIgnoredProperties { get; set; } = true;

    /// <summary>
    /// If simple type properties not marked with any XmlAttribute are accepted to serialize as attributes.
    /// </summary>
    public bool AcceptNotIgnoredPropertiesAsAttributes { get; set; }

    /// <summary>
    /// If attribute names should be changed to lowercase on serialization.
    /// </summary>
    public bool LowercaseAttributeName { get; set; }

    /// <summary>
    /// If property names should be changed to lowercase on serialization.
    /// </summary>
    public bool LowercasePropertyName { get; set; }

    /// <summary>
    /// If pProperty names should be preceded with element name on serialization.
    /// </summary>
    public bool PrecedePropertyNameWithElementName { get; set; }

    /// <summary>
    /// A string to markup items in collections as XmlElement names.
    /// </summary>
    public string? ItemTag { get; set; }

    /// <summary>
    /// If enum values should be changed to lowercase on serialization.
    /// Boolean is treated as enum.
    /// </summary>
    public bool LowerCaseEnum { get; set; }

    /// <summary>
    /// If enum values letter case should be ignored on deserialization.
    /// Boolean case is always ignored.
    /// </summary>
    public bool IgnoreCaseEnum { get; set; }

    /// <summary>
    /// Culture is important on serialization/deserialization numbers and dates.
    /// Default is invariant culture.
    /// </summary>
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    /// <summary>
    /// Specifies that when the deserializer finds an unknown XML element, 
    /// it skips to the closing one (or ignores if it is an empty element).
    /// </summary>
    public bool IgnoreUnknownElements { get; set; }

    /// <summary>
    /// Writes xsi:nil="true" attribute when value is null.
    /// </summary>
    public bool UseNilAttribute { get; set; } = true;

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

  }
}
