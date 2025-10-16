using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Qhta.Unicode.Models;

/// <summary>
/// Entity representing a Unicode code point as defined in the Unicode Character Database (UCD).
/// </summary>
[Table("UcdCodePoints")]
public partial class UcdCodePoint
{
  #region Basic fields
  /// <summary>
  /// Identifier for the Unicode code point, which is an integer number of the Code field.
  /// </summary>
  [Column("ID")]
  public int Id { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Visual representation of the Unicode code point, typically one character or two characters for diacritical marks.
  /// </summary>
  public string? Glyph { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Hexadecimal code of the Unicode code point, represented as a string.
  /// </summary>
  public string Code { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// A short name of the Unicode code point, which may be used to identify the character in various contexts.
  /// </summary>
  public string? CharName { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Long, descriptive name of the Unicode code point.
  /// </summary>
  public string? Description { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Two-letter code representing the Unicode category of the code point, such as "Lu" for uppercase letter or "Nd" for decimal number.
  /// </summary>
  public string? Ctg { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Combining class of the Unicode code point, which indicates how the character combines with other characters.
  /// </summary>
  public UcdCombination? Comb { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Bidirectional category of the Unicode code point, which indicates how the character behaves in bidirectional text.
  /// Encoded as a string, such as "L" for left-to-right or "R" for right-to-left.
  /// </summary>
  public string? Bidir { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A string which describes the decomposition of the Unicode code point, if applicable.
  /// </summary>
  public string? Decomposition { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A string which expresses the decimal digit value of the Unicode code point, if applicable.
  /// </summary>
  public string? DecDigitVal { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A string which expresses the digit value of the Unicode code point, if applicable.
  /// </summary>
  public string? DigitVal { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A string which expresses the numeric value of the Unicode code point, if applicable.
  /// </summary>
  public string? NumVal { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A string which expresses if the character is mirrored in bidirectional text.
  /// It is encoded as "Y" for yes or "N" for no.
  /// </summary>
  public string? Mirr { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// An old description of the Unicode code point, which may be used for historical reference or compatibility purposes.
  /// </summary>
  public string? OldDescription { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A comment or additional information about the Unicode code point, which may provide context or usage notes.
  /// </summary>
  public string? Comment { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Code point for the upper case version of the character, if applicable.
  /// </summary>
  public string? Upper { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Code point for the lower case version of the character, if applicable.
  /// </summary>
  public string? Lower { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Code point for the title case version of the character, if applicable.
  /// </summary>
  public string? Title { [DebuggerStepThrough] get; set; }
  #endregion

  #region Classification fields
  /// <summary>
  /// Identifier for the Unicode block that this code point belongs to.
  /// </summary>
  [Column("Block")]
  public int? BlockId { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Identifier for the area that this code point belongs to, if applicable.
  /// </summary>
  [Column("Area")]
  public int? AreaId { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Identifier for the script that this code point belongs to, if applicable.
  /// </summary>
  [Column("Script")]
  public int? ScriptId { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Identifier for the language that this code point belongs to, if applicable.
  /// </summary>
  [Column("Language")]
  public int? LanguageId { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Identifier for the notation that this code point belongs to, if applicable.
  /// </summary>
  [Column("Notation")]
  public int? NotationId { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Identifier for the symbol set that this code point belongs to, if applicable.
  /// </summary>
  [Column("SymbolSet")]
  public int? SymbolSetId { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Identifier for the subset that this code point belongs to, if applicable.
  /// </summary>
  [Column("Subset")]
  public int? SubsetId { [DebuggerStepThrough] get; set; }
  #endregion
  
  /// <summary>
  /// Collection of the aliases of this Unicode code point.
  /// </summary>
  public virtual ICollection<Alias> Aliases { [DebuggerStepThrough] get; set; } = new List<Alias>();
}
