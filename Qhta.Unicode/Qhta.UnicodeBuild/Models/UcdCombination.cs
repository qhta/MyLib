namespace Qhta.Unicode.Models;

public enum UcdCombination
{
  /// <summary>
  ///  	Spacing, split, enclosing, reordrant, and Tibetan subjoin
  /// </summary>
  Standard = 0,
  /// <summary>
  /// Overlays and interior
  /// </summary>
  OverlaysAndInterior = 1,
  /// <summary>
  /// Tibetan subjoined letters
  /// </summary>
  TibetanSubjoinedLetters = 6,
  /// <summary>
  /// Nuktas
  /// </summary>
  Nuktas = 7,
  /// <summary>
  /// Hiragana/Katakana voiced marks
  /// </summary>
  KanaVoicedMarks = 8,
  /// <summary>
  /// Viramas
  /// </summary>
  Viramas = 9,
  /// <summary>
  /// Start of fixed position classes
  /// </summary>
  FixedPositionClassesStart = 10,
  /// <summary>
  /// End of fixed position classes
  /// </summary>
  FixedPositionClassesEnd = 199,
  /// <summary>
  /// Below left attached
  /// </summary>
  BelowLeftAttached = 200,
  /// <summary>
  /// Below attached
  /// </summary>
  BelowAttached = 202,
  /// <summary>
  /// Below right attached
  /// </summary>
  BelowRightAttached = 204,
  /// <summary>
  /// Left attached (reordrant around single base character)
  /// </summary>
  LeftAttached = 208,
  /// <summary>
  /// Right attached
  /// </summary>
  RightAttached = 210,
  /// <summary>
  /// Above left attached
  /// </summary>
  AboveLeftAttached = 212,
  /// <summary>
  /// Above attached
  /// </summary>
  AboveAttached = 214,
  /// <summary>
  /// Above right attached
  /// </summary>
  AboveRightAttached = 216,
  /// <summary>
  /// Below left
  /// </summary>
  BelowLeft = 218,
  /// <summary>
  /// Below
  /// </summary>
  Below = 220,
  /// <summary>
  /// Below right
  /// </summary>
  BelowRight = 222,
  /// <summary>
  /// Left (reordrant around single base character)
  /// </summary>
  LeftReordrant = 224,
  /// <summary>
  /// Right
  /// </summary>
  Right = 226,
  /// <summary>
  /// Above left
  /// </summary>
  AboveLeft = 228,
  /// <summary>
  /// Above
  /// </summary>
  Above = 230,
  /// <summary>
  /// Above right
  /// </summary>
  AboveRight = 232,
  /// <summary>
  /// Double above
  /// </summary>
  AboveDouble = 234,
  /// <summary>
  /// Below (iota subscript)
  /// </summary>
  BelowIotaSubscript = 240,

}