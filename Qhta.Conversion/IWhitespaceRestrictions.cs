namespace Qhta.Conversion;

/// <summary>
/// Declares how to treat whitespaces.
/// </summary>
public enum WhitespaceBehavior
{
  /// <summary>
  /// Preserve whitespaces.
  /// </summary>
  Preserve = 0,
  /// <summary>
  /// Replace whitespaces with spaces.
  /// </summary>
  Replace = 1,
  /// <summary>
  /// Collapce whitespaces to a single space.
  /// </summary>
  Collapse = 2
}

/// <summary>
/// Specifies how to treat whitespaces and whether they were fixed on convert-back.
/// </summary>
public interface IWhitespaceRestrictions
{
  /// <summary>
  /// Specifies how to treat whitespaces.
  /// </summary>
  public WhitespaceBehavior Whitespaces { get; set; }

  /// <summary>
  /// Specifies whether whitespaces were fixed on convert-back method.
  /// </summary>
  public bool WhitespacesFixed { get; set; }
}