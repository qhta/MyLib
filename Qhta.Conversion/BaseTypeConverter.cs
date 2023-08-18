namespace Qhta.Conversion;

/// <summary>
/// Basic type converter - to be extended by other converters.
/// </summary>
public class BaseTypeConverter: TypeConverter
{
  /// <summary>
  ///   Type expected in ConvertFrom method
  /// </summary>
  public virtual Type? ExpectedType { get; set;}

  /// <summary>
  ///   Types known in ConvertFrom method
  /// </summary>
  public virtual Dictionary<string, Type>? KnownTypes { get; set; }

  /// <summary>
  ///   Known namespace prefixes
  /// </summary>
  public virtual Dictionary<string, string>? KnownNamespaces { get; set; }

  /// <summary>
  ///   XsdSimpleType to use when converting to string in ConvertTo
  /// </summary>
  public virtual XsdSimpleType? XsdType { get; set; }

  /// <summary>
  ///   Format to use when converting to/from string in ConvertTo/ConvertFrom
  /// </summary>
  public virtual string? Format { get; set; }

  /// <summary>
  ///   CultureInfo to use when converting to/from string in ConvertTo/ConvertFrom
  /// </summary>
  public virtual CultureInfo? Culture { get; set; }

}