using System.Runtime.Serialization;

namespace Qhta.Unicode;

/// <summary>
/// Enumerates the Unicode block types
/// </summary>

public enum BlockType
{
  /// <summary>Undefined block type.</summary>
  [EnumMember(Value = "unkn")] Unknown,
  /// <summary>Standard set of letters written to represent particular sounds in a spoken language.</summary>
  [EnumMember(Value = "alph")] Alphabet,
  /// <summary>Writing system in which only consonants are represented, leaving the vowel sounds to be inferred by the reader.</summary>
  [EnumMember(Value = "abjad")] Abjad,
  /// <summary>Segmental writing system in which consonant–vowel sequences are written as units; each unit is based on a consonant letter, and vowel notation is secondary, similar to a diacritical mark.</summary>
  [EnumMember(Value = "abug")] Abugida,
  /// <summary>Diacritical symbols combining with other symbols.</summary>
  [EnumMember(Value = "comb")] Combining,
  /// <summary>Cuneiform symbols.</summary>
  [EnumMember(Value = "cune")] Cuneiform,
  /// <summary>Ideographic symbols.</summary>
  [EnumMember(Value = "ideo")] Ideographic,
  /// <summary>Block that contains only numeral symbols</summary>
  [EnumMember(Value = "num")] Numerals,
  /// <summary>Block that changes only the form of presentation of some other code point.</summary>
  [EnumMember(Value = "form")] Format,
  /// <summary>Phonetic extensions.</summary>
  [EnumMember(Value = "phon")] Phonetic,
  /// <summary>Phonetic extensions.</summary>
  [EnumMember(Value = "punc")] Punctuation,
  /// <summary>Block of which only first and last symbol has a name.</summary>
  [EnumMember(Value = "range")] Range,
  /// <summary>Block in which individual code points are numbered sequentially</summary>
  [EnumMember(Value = "seq")] Sequential,
  /// <summary>Writing system that behaves partly as an alphabet and partly as a syllabary.</summary>
  [EnumMember(Value = "semi")] SemiSyllabary,
  /// <summary>Set of written symbols (called syllabograms) that represent either syllables or moras—a unit of prosody that is often but not always a syllable in length.</summary>
  [EnumMember(Value = "syl")] Syllabary,
  /// <summary>Block which contains symbols not used in any natural language.</summary>
  [EnumMember(Value = "symb")] Symbols,
  /// <summary>Block with special syntax of description.</summary>
  [EnumMember(Value = "block")] Block,
  /// <summary>Block with special syntax of description.</summary>
  [EnumMember(Value = "spec")] Special,

}