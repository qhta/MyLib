namespace Qhta.Unicode;

/// <summary>
/// Unicode character BiDi class.
/// </summary>
public enum BiDiClass
{
  // Strong Types
  /// <summary>
  /// Left-to-Right - any strong left-to-right character
  /// </summary>
  L,
  /// <summary>
  /// Right-to-Left - any strong right-to-left (non-Arabic-type) character
  /// </summary>
  R,
  /// <summary>
  /// Arabic Letter - any strong right-to-left (Arabic-type) character
  /// </summary>
  AL,
  // Weak Types
  /// <summary>
  /// European Number - any ASCII digit or Eastern Arabic-Indic digit
  /// </summary>
  EN,
  /// <summary>
  /// European_Separator - plus and minus signs
  /// </summary>
  ES,
  /// <summary>
  /// European_Terminator - a terminator in a numeric format context, includes currency signs
  /// </summary>
  ET,
  /// <summary>
  /// Arabic Number - any Arabic-Indic digit
  /// </summary>
  AN,
  /// <summary>
  /// Common_Separator - commas, colons, and slashes
  /// </summary>
  CS,
  /// <summary>
  /// Non-Spacing Mark - any non-spacing mark
  /// </summary>
  NSM,
  /// <summary>
  /// Boundary Neutral - most format characters, control codes, or non-characters
  /// </summary>
  BN,
  // Neutral Types
  /// <summary>
  /// Paragraph Separator - various newline characters
  /// </summary>
  B,
  /// <summary>
  /// 
  /// </summary>
  S,
  /// <summary>
  /// White_Space - spaces
  /// </summary>
  WS,
  /// <summary>
  /// Other_Neutral - most other symbols and punctuation marks
  /// </summary>
  ON,
  // Explicit Formatting Types
  /// <summary>
  /// Left-to-Right Embedding - U+202A: the LR embedding control
  /// </summary>
  LRE,
  /// <summary>
  /// Left-to-Right Override - U+202D: the LR override control
  /// </summary>
  LRO,
  /// <summary>
  /// Right-to-Left Embedding - U+202B: the RL embedding control
  /// </summary>
  RLE,
  /// <summary>
  /// Right-to-Left Override - U+202E: the RL override control
  /// </summary>
  RLO,
  /// <summary>
  /// Pop_Directional_Format - U+202C: terminates an embedding or override control
  /// </summary>
  PDF,
  /// <summary>
  /// Left-to-Right Isolate - U+2066: the LR isolate control
  /// </summary>
  LRI,
  /// <summary>
  /// Right-to-Left Isolate - U+2067: the RL isolate control
  /// </summary>
  RLI,
  /// <summary>
  /// First_Strong_Isolate - U+2068: the first strong isolate control
  /// </summary>
  FSI,
  /// <summary>
  /// Pop_Directional_Isolate - U+2069: terminates an isolate control
  /// </summary>
  PDI,
}