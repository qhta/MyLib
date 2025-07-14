namespace Qhta.Unicode.Models;

/// <summary>
/// Specifies the type of alias used of the code point.
/// </summary>
public enum AliasType: byte
{
  /// <summary>
  /// An alias that is used for control purposes, such as formatting or layout control characters.
  /// </summary>
  Control = 0,
  /// <summary>
  /// An alias that is used to represent a shortened or abbreviated form of a code point, often used for convenience or space-saving purposes.
  /// </summary>
  Abbreviation = 1,
  /// <summary>
  /// An alias that is used to represent an alternative name or form of a code point, which may be used in different contexts or for different purposes.
  /// </summary>
  Alternate = 2,
  /// <summary>
  /// An alias that is used to provide a correction or clarification for a code point, often used to fix errors or inconsistencies in the original code point definition.
  /// </summary>
  Correction = 3,
  /// <summary>
  /// An alias that represents a name or label for a code point that does not actually exist in the Unicode standard.
  /// These aliases are typically used for illustrative, explanatory, or placeholder purposes in documentation or tools, but they do not correspond to any real, assigned Unicode code point.
  /// </summary>
  Figment = 4,
}