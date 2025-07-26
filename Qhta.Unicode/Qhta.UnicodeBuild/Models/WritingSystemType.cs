namespace Qhta.Unicode.Models;

/// <summary>
/// Enumeration representing different types of writing systems.
/// </summary>
public enum WritingSystemType: byte
{
  /// <summary>
  /// A range of code points that are not defined individually
  /// </summary>
  Area = 1,
  /// <summary>
  /// A set of writing systems or scripts that share common characteristics
  /// </summary>
  Family = 2,
  /// <summary>
  /// A script is a set of symbols used to write a language, which may include letters, numbers, punctuation, and other characters.
  /// </summary>
  Script = 3,
  /// <summary>
  /// A language is a structured system of communication that consists of grammar and vocabulary, used by a community for communication.
  /// </summary>
  Language = 4,
  /// <summary>
  /// Notation is a system of symbols used to represent information in some convention, often used in mathematics, music, or other specialized fields.
  /// </summary>
  Notation = 5,
  /// <summary>
  /// SymbolSet is a collection of symbols that can be used in various contexts. Unlike notation, there are no specific rules for use. 
  /// </summary>
  SymbolSet = 6,
  /// <summary>
  /// A subset is a collection of symbols that belongs to a larger set, often used to represent a specific part of that set.
  /// </summary>
  Subset = 7, // a subset of symbols belonging to a set
}
