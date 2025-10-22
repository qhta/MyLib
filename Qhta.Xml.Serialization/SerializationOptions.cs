namespace Qhta.Xml.Serialization;

/// <summary>
/// Class containing serialization options as separate boolean values.
/// </summary>
public class SerializationOptions: MappingOptions
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
  /// Whether auto-set prefix for default namespace should be written.
  /// </summary>
  public bool EmitDefaultNamespacePrefix { get; set; } = true;

  /// <summary>
  /// Whether unused tag namespaces should be removed on serialization.
  /// </summary>
  public bool RemoveUnusedNamespaces { get; set; } = true;

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

  ///// <summary>
  /////   A string to markup items in collections.
  /////   If not specified, items are serialized with their type tags.
  ///// </summary>
  //public string? ItemTag { get; set; }

  /// <summary>
  ///   Culture used in numbers during serialization/deserialization.
  ///   Default is invariant culture.
  /// </summary>
  public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

  /// <summary>
  ///   Specifies that when the serializer finds an unregistered type,
  ///   it tries to serialize it anyhow.
  /// </summary>
  public bool AllowUnregisteredTypes { get; set; }

  /// <summary>
  ///   Specifies that when the deserializer finds an unknown XML element,
  ///   it skips to the closing element (or ignores if it is an empty element).
  /// </summary>
  public bool IgnoreUnknownElements { get; set; }

  /// <summary>
  ///   Writes xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" in the first element.
  ///   Writes xsi:nil="true" attribute when value is null.
  ///   If this flag is false, then a null value is simply omitted.
  /// </summary>
  public bool UseNilValue { get; set; } = false;

  /// <summary>
  ///   Writes xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" in the first element.
  ///   Writes xsi:type="typeName" attribute when serializing a property with multiple derived types.
  ///   If this flag is true, then a property-named Xml element is itself a class instance element
  ///   where class name is written as first xsi:type attribute.
  ///   If this flag is false, then property name is serialized as a compound Xml element 
  ///   with contains a whole class instance Xml element.
  /// </summary>
  public bool UseXsiType { get; set; } = true;

  /// <summary>
  ///   Writes xmlns:xsd="http://www.w3.org/2001/XMLSchema" in the first element.
  /// </summary>
  public bool UseXsdScheme { get; set; } = true;

  /// <summary>
  /// Specifies whether the capacity of lists should be written as an attribute during serialization.
  /// </summary>
  public bool WriteListCapacity { get; set; } = false;

  /// <summary>
  /// Specifies whether an exception should be thrown when a circular reference is detected during serialization.
  /// </summary>
  public bool ThrowOnCircularReferences { get; set; } = false;

  /// <summary>
  /// Limit divides text to separate lines when Base64Binary converter was used.
  /// </summary>
  public int LineLengthLimit { get; set; } = 80;

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
  /// Default unit for numbers.
  /// </summary>
  public string DefaultUnit { get; set; } = "pt";

  /// <summary>
  /// Specifies if WriteStartElement and WriteEndElement pairs are checked.
  /// </summary>
  public bool TraceElementStack { get; set; } = false;

  /// <summary>
  /// Specifies if WriteStartAttribute and WriteEndAttribute pairs are checked.
  /// </summary>
  public bool TraceAttributeStack { get; set; } = false;
}