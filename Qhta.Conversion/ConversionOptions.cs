namespace Qhta.Conversion;

public class ConversionOptions
{
  /// <summary>
  /// The character to insert between the date and time when serializing a DateTime value.
  /// </summary>
  public char DateTimeSeparator { get; set; }

  /// <summary>
  /// Specifies whether to display the fractional part of seconds when serializing a DateTime value.
  /// </summary>
  public bool ShowSecondsFractionalPart { get; set; }

  /// <summary>
  /// Specifies whether to display the time zone when serializing a DateTime value.
  /// </summary>
  public bool ShowTimeZone { get; set; }

  /// <summary>
  /// Specifies strings representation of the boolean value. First goes TrueString, false goes FalseString.
  /// First pair is used on serialization, all pairs are accepted on deserialization.
  /// </summary>
  public (string, string)[]? BooleanStrings { get; set; }

}