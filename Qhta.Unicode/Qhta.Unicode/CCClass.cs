namespace Qhta.Unicode;

/// <summary>
/// Unicode canonical combining class.
/// </summary>
public enum CCClass
{
  /// <summary>
  /// Not_Reordered - Spacing and enclosing marks; also many vowel and consonant signs, even if non-spacing
  /// </summary>
  NR = 0,
  /// <summary>
  /// Overlay - Marks which overlay a base letter or symbol
  /// </summary>
  Ov = 1,
  /// <summary>
  /// Han_Reading - Diacritic reading marks for CJK unified ideographs
  /// </summary>
  HR = 6,
  /// <summary>
  /// Nukta - Diacritic nukta marks in Brahmi-derived scripts
  /// </summary>
  Nu = 7,
  /// <summary>
  /// Kana_Voicing - Hiragana/Katakana voicing marks
  /// </summary>
  KV = 8,
  /// <summary>
  /// Virama - Viramas
  /// </summary>
  Vi = 9,
  /// <summary>
  /// Attached_Below_Left - Marks attached at the bottom left
  /// </summary>
  AtBL = 200,
  /// <summary>
  /// Attached_Below - Marks attached directly below
  /// </summary>
  AtB = 202,
  /// <summary>
  /// Attached_Below_Right - Marks attached at the bottom right
  /// </summary>
  AtBR = 204,
  /// <summary>
  /// Attached_Left - Marks attached to the left
  /// </summary>
  AtL = 208,
  /// <summary>
  /// Attached_Right - Marks attached to the right
  /// </summary>
  AtR = 210,
  /// <summary>
  /// Attached_Above_Left - Marks attached at the top left
  /// </summary>
  AtTL = 212,
  /// <summary>
  /// Attached_Above - Marks attached directly above
  /// </summary>
  AtA = 214,
  /// <summary>
  /// Attached_Above_Right - Marks attached at the top right
  /// </summary>
  AtAR = 216,
  /// <summary>
  /// Below_Left - Distinct marks at the bottom left
  /// </summary>
  BL = 218,
  /// <summary>
  /// Below - Distinct marks directly below
  /// </summary>
  B = 220,
  /// <summary>
  /// Below_Right - Distinct marks at the bottom right
  /// </summary>
  BR = 222,
  /// <summary>
  /// Left - Distinct marks to the left
  /// </summary>
  L = 224,
  /// <summary>
  /// Right - Distinct marks to the right
  /// </summary>
  R = 226,
  /// <summary>
  /// Above_Left - Distinct marks at the top left
  /// </summary>
  AL = 228,
  /// <summary>
  /// Above - Distinct marks directly above
  /// </summary>
  A = 230,
  /// <summary>
  /// Above_Right - Distinct marks at the top right
  /// </summary>
  AR = 232,
  /// <summary>
  /// Double_Below - Distinct marks subtending two bases
  /// </summary>
  DB = 233,
  /// <summary>
  /// Double_Above - Distinct marks extending above two bases
  /// </summary>
  DA = 234,
  /// <summary>
  /// Iota_Subscript - Greek iota subscript only
  /// </summary>
  IS = 240,
}