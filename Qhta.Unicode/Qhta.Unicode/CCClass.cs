namespace Qhta.Unicode;

public enum CCClass
{
  NR = 0,                 // Not_Reordered - Spacing and enclosing marks; also many vowel and consonant signs, even if nonspacing
  Ov = 1,                 // Overlay - Marks which overlay a base letter or symbol
  HR = 6,                 // Han_Reading - Diacritic reading marks for CJK unified ideographs
  Nu = 7,                 // Nukta - Diacritic nukta marks in Brahmi-derived scripts
  KV = 8,                 // Kana_Voicing - Hiragana/Katakana voicing marks
  Vi = 9,                 // Virama - Viramas
  AtBL = 200,             // Attached_Below_Left - Marks attached at the bottom left
  AtB = 202,              // Attached_Below - Marks attached directly below
  AtBR = 204,             // Attached_Below_Right - Marks attached at the bottom right
  AtL = 208,              // Attached_Left - Marks attached to the left
  AtR = 210,              // Attached_Right - Marks attached to the right
  AtTL = 212,             // Attached_Top_Left - Marks attached at the top left
  AtA = 214,              // Attached_Above - Marks attached directly above
  AtAR = 216,             // Attached_Above_Right - Marks attached at the top right
  BL = 218,               // Below_Left - Distinct marks at the bottom left
  B = 220,                // Below - Distinct marks directly below
  BR = 222,               // Below_Right - Distinct marks at the bottom right
  L = 224,                // Left - Distinct marks to the left
  R = 226,                // Right - Distinct marks to the right
  AL = 228,               // Above_Left - Distinct marks at the top left
  A = 230,                // Above - Distinct marks directly above
  AR = 232,               // Above_Right - Distinct marks at the top right
  DB = 233,               // Double_Below - Distinct marks subtending two bases
  DA = 234,               // Double_Above - Distinct marks extending above two bases
  IS = 240,               // Iota_Subscript - Greek iota subscript only

}