using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Unicode;

/// <summary>
/// A Unicode type of name alias
/// </summary>
public enum NameAliasType
{
  /// <summary>
  /// ISO 6429 names for C0 and C1 control functions, and other commonly occurring names for control codes
  /// </summary>
  Control,
  /// <summary>
  /// Commonly occurring abbreviations (or acronyms) for control codes, format characters, spaces, and variation selectors
  /// </summary>
  Abbreviation,
  /// <summary>
  /// A few widely used alternate names for format characters
  /// </summary>
  Alternate,
  /// <summary>
  /// Corrections for serious problems in the character names
  /// </summary>
  Correction,
  /// <summary>
  /// Several documented labels for C1 control code points which were never actually approved in any standard
  /// </summary> 
  Figment
}

/// <summary>
/// A Unicode name alias. Contains a code point and an alias name.
/// </summary>
public class NameAlias
{
  /// <summary>
  /// The code point of the character
  /// </summary>
  public int CodePoint { get; set; }
  /// <summary>
  /// 
  /// </summary>
  public HashedName Alias { get; set; } = null!;
  /// <summary>
  /// The type of the alias
  /// </summary>
  public NameAliasType Type { get; set; }
}
