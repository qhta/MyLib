using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Unicode;

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
public class NameAlias
{
  public int CodePoint { get; set; }
  public HashedName Alias { get; set; } = string.Empty;
  public NameAliasType Type { get; set; }
}
