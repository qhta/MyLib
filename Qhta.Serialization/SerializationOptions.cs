using System.Globalization;

namespace Qhta.Serialization
{
  public class SerializationOptions
  {
    /// <summary>
    /// Types to be deserialized must have parameterless constructor.
    /// If a type found during scanning available types, the exception is thrown
    /// unless this option is set.
    /// </summary>
    public bool IgnoreTypesWithoutParameterlessConstructor { get; set; }

    /// <summary>
    /// If attribute names should be changed to lowercase on serialization
    /// </summary>
    public bool LowercaseAttributeName { get; set; }

    /// <summary>
    /// If property names should be changed to lowercase on serialization
    /// </summary>
    public bool LowercasePropertyName { get; set; }

    /// <summary>
    /// If pProperty names should be preceded with element name on serialization
    /// </summary>
    public bool PrecedePropertyNameWithElementName { get; set; }

    /// <summary>
    /// A string to markup items in collections as XmlElement names
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
    /// </summary>
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
  }
}
