namespace Qhta.Unicode;

/// <summary>
/// Unicode character decomposition type.
/// </summary>
public enum DecompositionType
{
  /// <summary>
  /// Not stated
  /// </summary>
  None,
  /// <summary>
  /// Font variant (for example, a blackletter form)
  /// </summary>
  Font,
  /// <summary>
  /// No-break version of a space or hyphen
  /// </summary>
  Nobreak,
  /// <summary>
  /// Initial presentation form (Arabic)
  /// </summary>
  Initial,
  /// <summary>
  /// Medial presentation form (Arabic)
  /// </summary>
  Medial,
  /// <summary>
  /// Final presentation form (Arabic)
  /// </summary>
  Final,
  /// <summary>
  /// Isolated presentation form (Arabic)
  /// </summary>
  Isolated,
  /// <summary>
  /// Encircled form
  /// </summary>
  Circle,
  /// <summary>
  /// Superscript form
  /// </summary>
  Super,
  /// <summary>
  /// Subscript form
  /// </summary>
  Sub,
  /// <summary>
  /// Vertical layout presentation form
  /// </summary>
  Vertical,
  /// <summary>
  /// Wide (or zenkaku) compatibility character
  /// </summary>
  Wide,
  /// <summary>
  /// Narrow (or hankaku) compatibility character
  /// </summary>
  Narrow,
  /// <summary>
  /// Small variant form (CNS compatibility)
  /// </summary>
  Small,
  /// <summary>
  /// CJK squared font variant
  /// </summary>
  Square,
  /// <summary>
  /// Vulgar fraction form
  /// </summary>
  Fraction,
  /// <summary>
  /// Otherwise unspecified compatibility character
  /// </summary>
  Compat,
}