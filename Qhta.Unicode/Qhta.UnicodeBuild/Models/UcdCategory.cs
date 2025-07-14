namespace Qhta.Unicode.Models;

/// <summary>
/// Represents the Unicode character categories as defined by the Unicode Standard.
/// </summary>
/// <remarks>The <see cref="UcdCategory"/> enumeration provides a set of named constants that correspond to the 
/// general categories of characters in the Unicode character database. These categories are used to  classify
/// characters based on their general properties, such as being a letter, number, punctuation,  or symbol. This
/// classification is useful for text processing and analysis tasks.</remarks>
public enum UcdCategory: byte
{
  /// <summary>
  /// Other, Not assigned
  /// </summary>
  Cn = 0,
  /// <summary>
  /// Other, Control
  /// </summary>
  Cc = 1,
  /// <summary>
  /// Other, Format
  /// </summary>
  Cf = 2,
  /// <summary>
  /// Other, Private Use
  /// </summary>
  Co = 3,
  /// <summary>
  /// Other, Surrogate
  /// </summary>
  Cs = 4,
  /// <summary>
  /// Letter, Uppercase
  /// </summary>
  Lu = 5,
  /// <summary>
  /// Letter, Lowercase
  /// </summary>
  Ll = 6,
  /// <summary>
  /// Letter, Titlecase
  /// </summary>
  Lt = 7,
  /// <summary>
  /// Letter, Modifier
  /// </summary>
  Lm = 8,
  /// <summary>
  /// Letter, Other
  /// </summary>
  Lo = 9,
  /// <summary>
  /// Mark, Non-Spacing
  /// </summary>
  Mn = 10,
  /// <summary>
  /// Mark, Spacing Combining
  /// </summary>
  Mc = 11,
  /// <summary>
  /// Mark, Enclosing
  /// </summary>
  Me = 12,
  /// <summary>
  /// Number, Decimal Digit
  /// </summary>
  Nd = 13,
  /// <summary>
  /// Number, Letterlike
  /// </summary>
  Nl = 14,
  /// <summary>
  /// Number, Other
  /// </summary>
  No = 15,
  /// <summary>
  /// Punctuation, Connector
  /// </summary>
  Pc = 16,
  /// <summary>
  /// Punctuation, Dash
  /// </summary>
  Pd = 17,
  /// <summary>
  /// Punctuation, Open
  /// </summary>
  Ps = 18,
  /// <summary>
  /// Punctuation, Close
  /// </summary>
  Pe = 19,
  /// <summary>
  /// Punctuation, Quote Open
  /// </summary>
  Pi = 20,
  /// <summary>
  /// Punctuation, Quote Close
  /// </summary>
  Pf = 21,
  /// <summary>
  /// Punctuation, Other
  /// </summary>
  Po = 22,
  /// <summary>
  /// Symbol, Currency
  /// </summary>
  Sc = 23,
  /// <summary>
  /// Symbol, Modifier
  /// </summary>
  Sk = 24,
  /// <summary>
  /// Symbol, Math
  /// </summary>
  Sm = 25,
  /// <summary>
  /// Symbol, Other
  /// </summary>
  So = 26,
  /// <summary>
  /// Separator, Line
  /// </summary>
  Zl = 27,
  /// <summary>
  /// Separator, Paragraph
  /// </summary>
  Zp = 28,
  /// <summary>
  /// Separator, Space
  /// </summary>
  Zs = 29,

}