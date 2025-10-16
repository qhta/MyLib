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
  Script = 2,
  /// <summary>
  /// A language is a structured system of communication that consists of grammar and vocabulary, used by a community for communication.
  /// </summary>
  Language = 3,
  /// <summary>
  /// Notation is a system of symbols used to represent information in some convention, often used in mathematics, music, or other specialized fields.
  /// </summary>
  Notation = 4,
  /// <summary>
  /// SymbolSet is a collection of symbols that can be used in various contexts. Unlike notation, there are no specific rules for use. 
  /// </summary>
  SymbolSet = 5,
  /// <summary>
  /// A subset is a collection of symbols that belongs to a larger set, often used to represent a specific part of that set.
  /// </summary>
  Subset = 6, // a subset of symbols belonging to a set
}
