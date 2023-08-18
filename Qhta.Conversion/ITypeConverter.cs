namespace Qhta.Conversion;

/// <summary>
/// Defines interface to implement in internal converter.
/// </summary>
public interface ITypeConverter
{
  /// <summary>
  /// Expected type of output value in convert-back methods.
  /// </summary>
  public Type? ExpectedType { get; set; }

  /// <summary>
  /// Known types for convert-back methods.
  /// </summary>
  public Type[]? KnownTypes { get; set; }

  /// <summary>
  /// XsdType for convert-forth methods.
  /// </summary>
  public XsdSimpleType? XsdType { get; set; }

  /// <summary>
  /// Format of string values.
  /// </summary>
  public string? Format { get; set; }

  /// <summary>
  /// Culture used in conversion.
  /// </summary>
  public CultureInfo? Culture { get; set; }
}