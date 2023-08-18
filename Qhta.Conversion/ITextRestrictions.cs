namespace Qhta.Conversion;

/// <summary>
/// Defines Patterns and Enumerations and case insensitive option for string converter.
/// </summary>
public interface ITextRestrictions
{
  /// <summary>
  /// Regular expression patterns for acceptable strings.
  /// </summary>
  public string[]? Patterns { get; set; }

  /// <summary>
  /// List of acceptable strings.
  /// </summary>
  public string[]? Enumerations { get; set; }

  /// <summary>
  /// Specifies whether check is case-insensitive.
  /// </summary>
  public bool CaseInsensitive { get; set; }
}