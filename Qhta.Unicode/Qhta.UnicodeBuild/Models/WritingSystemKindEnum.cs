using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public enum WritingSystemKindEnum
{
  abjad = 1,  // a writing system in which only consonants are represented, leaving the vowel sounds to be inferred by the reader
  abugida = 2,  // a segmental writing system in which consonant–vowel sequences are written as units; each unit is based on a consonant letter, and vowel notation is secondary, similar to a diacritical mark
  alphabet = 3,  // a standard set of letters written to represent particular sounds in a spoken language
  constructed = 4,  // a language whose phonology, grammar, orthography, and vocabulary, instead of having developed naturally, are consciously devised for some purpose, which may include being devised for a work of fiction
  control = 5,  // a set of characters that control the flow of text
  cuneiform = 6,  // a logo-syllabic writing system that was used to write several languages of the Ancient Near East, named for the characteristic wedge-shaped impressions which form their signs
  encoding = 7,  // a set of characters encoding other characters
  form = 8,  // a set of characters with special form
  graphical = 9,  // a set of graphical shapes for individual use or used to construct grid-based drawings
  hieroglyphic = 10,  // an ancient writing system that combines ideographic, logographic, syllabic and alphabetic elements, with more than 1,000 distinct characters
  ideographic = 11,  // a writing system consisting of symbols that represents ideas or concepts independent of any particular language
  invisible = 12,  // a set of code points that have no visible presentations
  logographic = 13,  // a set of written character that represents a semantic component of a language, such as a word or morpheme
  mixed = 14,  // mixed logographic, syllabary and phonetic
  musical = 15,  // any system used to visually represent music
  numerical = 16,  // a writing system for expressing numbers; that is, a mathematical notation for representing numbers of a given set, using digits or other symbols in a consistent manner
  phonetic = 17,  // a set of symbols used in phonetic notation
  pictographic = 18,  // a set of graphical symbols that conveys meaning through its visual resemblance to a physical objects
  semi_syllabary = 19,  // a writing system that behaves partly as an alphabet and partly as a syllabary
  stenography = 20,  // an abbreviated symbolic writing method that increases speed and brevity of writing as compared to longhand, a more common method of writing a language
  syllabary = 21,  // a set of written symbols that represent the syllables or (more frequently) morae which make up words
  symbolic = 22,  // a set of symbols from a certain domain
}
