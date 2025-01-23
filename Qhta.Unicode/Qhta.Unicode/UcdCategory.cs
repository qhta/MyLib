namespace Qhta.Unicode;

/// <summary>
/// Unicode character category. The values are the same as UnicodeCategory in System.Globalization.
/// </summary>
public enum UcdCategory
{
  /// <summary>Uppercase letter.</summary>
  Lu,
  /// <summary>Lowercase letter.</summary>
  Ll,
  /// <summary>Titlecase letter.</summary>
  Lt,
  /// <summary>Modifier letter character, which is freestanding spacing character that indicates modifications of a preceding letter.</summary>
  Lm,
  /// <summary>Letter that is not an uppercase letter, a lowercase letter, a titlecase letter, or a modifier letter.</summary>
  Lo,
  /// <summary>Non-spacing character that indicates modifications of a base character.</summary>
  Mn,
  /// <summary>Spacing character that indicates modifications of a base character and affects the width of the glyph for that base character.</summary>
  Mc,
  /// <summary>Enclosing mark character, which is a non-spacing combining character that surrounds all previous characters up to and including a base character.</summary>
  Me,
  /// <summary>Decimal digit character, that is, a character in the range 0 through 9.</summary>
  Nd,
  /// <summary>Number represented by a letter, instead of a decimal digit, for example, the Roman numeral for five, which is "V". The indicator is</summary>
  Nl,
  /// <summary>Number that is neither a decimal digit nor a letter number, for example, the fraction 1/2. The indicator is</summary>
  No,
  /// <summary>Space character, which has no glyph but is not a control or format character.</summary>
  Zs,
  /// <summary>Character that is used to separate lines of text.</summary>
  Zl,
  /// <summary>Character used to separate paragraphs.</summary>
  Zp,
  /// <summary>Control code character, with a Unicode value of U+007F or in the range U+0000 through U+001F or U+0080 through U+009F.</summary>
  Cc,
  /// <summary>Format character that affects the layout of text or the operation of text processes, but is not normally rendered.</summary>
  Cf,
  /// <summary>High surrogate or a low surrogate character. Surrogate code values are in the range U+D800 through U+DFFF.</summary>
  Cs,
  /// <summary>Private-use character, with a Unicode value in the range U+E000 through U+F8FF.</summary>
  Co,
  /// <summary>Connector punctuation character that connects two characters.</summary>
  Pc,
  /// <summary>Dash or hyphen character.</summary>
  Pd,
  /// <summary>Opening character of one of the paired punctuation marks, such as parentheses, square brackets, and braces.</summary>
  Ps,
  /// <summary>Closing character of one of the paired punctuation marks, such as parentheses, square brackets, and braces.</summary>
  Pe,
  /// <summary>Opening or initial quotation mark character.</summary>
  Pi,
  /// <summary>Closing or final quotation mark character.</summary>
  Pf,
  /// <summary>Punctuation character that is not a connector, a dash, open punctuation, close punctuation, an initial quote, or a final quote.</summary>
  Po,
  /// <summary>Mathematical symbol character, such as "+" or "= ".</summary>
  Sm,
  /// <summary>Currency symbol character.</summary>
  Sc,
  /// <summary>Modifier symbol character, which indicates modifications of surrounding characters. For example, the fraction slash indicates that the number to the left is the numerator and the number to the right is the denominator. The indicator is</summary>
  Sk,
  /// <summary>Symbol character that is not a mathematical symbol, a currency symbol or a modifier symbol.</summary>
  So,
  /// <summary>Character that is not assigned to any Unicode category.</summary>
  Cn,
}
