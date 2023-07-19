namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which can specify "Format" and "Culture" for a property or field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class XmlDataFormatAttribute : Attribute
{
  /// <summary>
  /// Format that must be suitable for a property or field data type.
  /// </summary>
  public string? Format { get; set; }

  /// <summary>
  /// Culture info for data conversion.
  /// </summary>
  public CultureInfo? Culture { get; set; }
}