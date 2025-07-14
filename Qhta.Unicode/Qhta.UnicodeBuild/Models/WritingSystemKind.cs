namespace Qhta.Unicode.Models;

/// <summary>
/// Specifies the kind of writing system used to represent language or symbols.
/// </summary>
/// <remarks>This enumeration categorizes various writing systems based on their structural and functional
/// characteristics. It includes systems such as alphabets, abjads, syllabaries, and more specialized forms like musical
/// notation and stenography.</remarks>
public enum WritingSystemKind
{
  /// <summary>
  /// A writing system in which only consonants are represented, leaving the vowel sounds to be inferred by the reader.
  /// </summary>
  Abjad = 1,
  /// <summary>
  /// A segmental writing system in which consonant–vowel sequences are written as units; each unit is based on a consonant letter, and vowel notation is secondary, similar to a diacritical mark.
  /// </summary>
  Abugida = 2,
  /// <summary>
  /// A standard set of letters written to represent particular sounds in a spoken language, typically including both consonants and vowels.
  /// </summary>
  Alphabet = 3,
  /// <summary>
  /// A language whose phonology, grammar, orthography, and vocabulary, instead of having developed naturally, are consciously devised for some purpose, which may include being devised for a work of fiction.
  /// </summary>
  Constructed = 4,
  /// <summary>
  /// A set of characters that control the flow of text, such as line breaks, tabs, and other formatting characters that do not represent written language.
  /// </summary>
  Control = 5,
  /// <summary>
  /// A logo-syllabic writing system that was used to write several languages of the Ancient Near East, named for the characteristic wedge-shaped impressions which form their signs.
  /// </summary>
  Cuneiform = 6,
  /// <summary>
  /// A set of characters encoding other characters, often used to represent text in a specific format or encoding scheme.
  /// </summary>
  Encoding = 7,
  /// <summary>
  /// A set of characters that are used to represent a specific form of writing, such as a script or a style of calligraphy, often with unique stylistic features.
  /// </summary>
  Form = 8,
  /// <summary>
  /// A set of graphical shapes that can be used individually or combined to create grid-based drawings, often used in design and art.
  /// </summary>
  Graphical = 9,
  /// <summary>
  /// An ancient writing system that uses pictorial symbols to represent sounds, words, or concepts, primarily associated with the ancient Egyptians.
  /// </summary>
  Hieroglyphic = 10,
  /// <summary>
  /// A writing system consisting of characters that represent ideas or concepts, rather than specific sounds or words, allowing for a more abstract representation of language.
  /// </summary>
  Ideographic = 11,
  /// <summary>
  /// A set of code points that have no visible presentations, often used for control characters or formatting purposes.
  /// </summary>
  Invisible = 12,
  /// <summary>
  /// A set of written characters that represents a semantic component of a language, such as a word or morpheme.
  /// </summary>
  Logographic = 13,
  /// <summary>
  /// Mixed writing systems that combine elements of different types, such as logographic, syllabary, and phonetic systems, to represent language.
  /// </summary>
  Mixed = 14,
  /// <summary>
  /// Musical notation is a system used to visually represent music through symbols, allowing musicians to read and perform compositions.
  /// </summary>
  Musical = 15,
  /// <summary>
  /// A writing system for expressing numbers; that is, a mathematical notation for representing numbers of a given set, using digits or other symbols in a consistent manner.
  /// </summary>
  Numerical = 16,
  /// <summary>
  /// A writing system that uses symbols to represent the sounds of speech, often used
  /// to transcribe languages phonetically, allowing for the representation of pronunciation without regard to the standard orthography of the language.
  /// </summary>
  Phonetic = 17,
  /// <summary>
  /// A set of graphical symbols that convey meaning through their visual resemblance to physical objects, often used in signage or pictorial communication.
  /// </summary>
  Pictographic = 18,
  /// <summary>
  /// A writing system that behaves partly as an alphabet and partly as a syllabary, where some characters represent syllables and others represent individual sounds.
  /// </summary>
  SemiSyllabary = 19,
  /// <summary>
  /// An abbreviated symbolic writing method that increases speed and brevity of writing as compared to longhand, a more common method of writing a language.
  /// </summary>
  Stenography = 20,
  /// <summary>
  /// A writing system that uses symbols to represent syllables, typically used in languages where syllables are the basic units of meaning, rather than individual sounds or words.
  /// </summary>
  Syllabary = 21,
  /// <summary>
  /// A writing system that uses a set of symbols to represent a specific domain or set of concepts, often used in specialized fields such as mathematics, chemistry, or computer science.
  /// </summary>
  Symbolic = 22,  // a set of symbols from a certain domain
  /// <summary>
  /// A writing system that combines ideographic and phonetic elements, often used to represent both the meaning and pronunciation of words in a language.
  /// </summary>
  Pictophonetic = 23,
}
