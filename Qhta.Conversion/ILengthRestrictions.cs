namespace Qhta.Conversion;

/// <summary>
/// Defines string (or array) length restrictions.
/// </summary>
public interface ILengthRestrictions
{
  /// <summary>
  /// Minimum length.
  /// </summary>
  public int? MinLength { get; set; }

  /// <summary>
  /// Maximum length.
  /// </summary>
  public int? MaxLength { get; set; }
}