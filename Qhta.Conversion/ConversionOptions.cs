namespace Qhta.Conversion;

/// <summary>
/// This class groups options for conversion in ValueTypeConverter.
/// </summary>
public class ConversionOptions
{
  /// <summary>
  ///   Specifies whether escape sequences should be used to convert strings.
  /// </summary>
  public bool UseEscapeSequences { get; set; }

  /// <summary>
  ///   Specifies whether Html entities should be used to convert strings.
  /// </summary>
  public bool UseHtmlEntities { get; set; }

  /// <summary>
  ///   Specifies the character to insert between the date and time when serializing a DateTime value.
  /// </summary>
  public char DateTimeSeparator { get; set; }

  /// <summary>
  ///   Specifies whether to display the fractional part of seconds when serializing a DateTime value.
  /// </summary>
  public bool ShowFullTime { get; set; }

  /// <summary>
  ///   Specifies whether to display the time zone when serializing a DateTime value.
  /// </summary>
  public bool ShowTimeZone { get; set; }

  /// <summary>
  ///   Specifies strings representation of the boolean value. First goes TrueString, second goes FalseString.
  ///   First pair is used on serialization, all pairs are accepted on deserialization.
  /// </summary>
  public (string, string)[]? BooleanStrings { get; set; }
}