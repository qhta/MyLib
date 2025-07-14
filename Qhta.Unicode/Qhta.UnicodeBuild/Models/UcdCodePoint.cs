using System.ComponentModel.DataAnnotations.Schema;

namespace Qhta.Unicode.Models;

/// <summary>
/// Entity representing a Unicode code point as defined in the Unicode Character Database (UCD).
/// </summary>
[Table("UcdCodePoints")]
public partial class UcdCodePoint
{
  /// <summary>
  /// Identifier for the Unicode code point, which is an integer number of the Code field.
  /// </summary>
  [Column("Ord")]
  public int Id { get; set; }

  /// <summary>
  /// Visual representation of the Unicode code point, typically one character or two characters for diacritical marks.
  /// </summary>
  public string? Glyph { get; set; }

  /// <summary>
  /// Hexadecimal code of the Unicode code point, represented as a string.
  /// </summary>
  public string Code { get; set; } = null!;

  /// <summary>
  /// A short name of the Unicode code point, which may be used to identify the character in various contexts.
  /// </summary>
  public string? CharName { get; set; }

  /// <summary>
  /// Long, descriptive name of the Unicode code point.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Two-letter code representing the Unicode category of the code point, such as "Lu" for uppercase letter or "Nd" for decimal number.
  /// </summary>
  public string? Ctg { get; set; }

  /// <summary>
  /// Combining class of the Unicode code point, which indicates how the character combines with other characters.
  /// </summary>
  public UcdCombination? Comb { get; set; }

  /// <summary>
  /// Bidirectional category of the Unicode code point, which indicates how the character behaves in bidirectional text.
  /// Encoded as a string, such as "L" for left-to-right or "R" for right-to-left.
  /// </summary>
  public string? Bidir { get; set; }

  /// <summary>
  /// A string which describes the decomposition of the Unicode code point, if applicable.
  /// </summary>
  public string? Decomposition { get; set; }

  /// <summary>
  /// A string which expresses the decimal digit value of the Unicode code point, if applicable.
  /// </summary>
  public string? DecDigitVal { get; set; }

  /// <summary>
  /// A string which expresses the digit value of the Unicode code point, if applicable.
  /// </summary>
  public string? DigitVal { get; set; }

  /// <summary>
  /// A string which expresses the numeric value of the Unicode code point, if applicable.
  /// </summary>
  public string? NumVal { get; set; }

  /// <summary>
  /// A string which expresses if the character is mirrored in bidirectional text.
  /// It is encoded as "Y" for yes or "N" for no.
  /// </summary>
  public string? Mirr { get; set; }

  /// <summary>
  /// An old description of the Unicode code point, which may be used for historical reference or compatibility purposes.
  /// </summary>
  public string? OldDescription { get; set; }

  /// <summary>
  /// A comment or additional information about the Unicode code point, which may provide context or usage notes.
  /// </summary>
  public string? Comment { get; set; }

  /// <summary>
  /// Code point for the upper case version of the character, if applicable.
  /// </summary>
  public string? Upper { get; set; }

  /// <summary>
  /// Code point for the lower case version of the character, if applicable.
  /// </summary>
  public string? Lower { get; set; }

  /// <summary>
  /// Code point for the title case version of the character, if applicable.
  /// </summary>
  public string? Title { get; set; }

  #region Classification fields
  /// <summary>
  /// Identifier for the Unicode block that this code point belongs to.
  /// </summary>
  public int? Block { get; set; }

  /// <summary>
  /// Identifier for the area writing system that this code point is associated with, if applicable.
  /// </summary>
  public int? Area { get; set; }

  /// <summary>
  /// Identifier for the script that this code point is written in, if applicable.
  /// </summary>
  public int? Script { get; set; }

  /// <summary>
  /// Identifier for the language that this code point is associated with, if applicable.
  /// </summary>
  public int? Language { get; set; }

  /// <summary>
  /// Identifier for the notation that this code point is associated with, if applicable.
  /// </summary>
  public int? Notation { get; set; }

  /// <summary>
  /// Identifier for the symbol set that this code point belongs to, if applicable.
  /// </summary>
  public int? SymbolSet { get; set; }

  /// <summary>
  /// Identifier for the subset that this code point belongs to, if applicable.
  /// </summary>
  public int? Subset { get; set; }

  /// <summary>
  /// Identifier for the artefact that this code point is associated with, if applicable.
  /// </summary>
  public int? Artefact { get; set; }

  #endregion

  /// <summary>
  /// Collection of the aliases of this Unicode code point.
  /// </summary>
  public virtual ICollection<Alias> Aliases { get; set; } = new List<Alias>();
}
