namespace Qhta.Unicode.Models;

/// <summary>
/// This enum defines the methods used for generating names.
/// </summary>
public enum NameGenMethod: byte
{
  /// <summary>
  /// Name is not generated for this writing system.
  /// However, if the code point has other writing systems assigned, they are used to generate names.
  /// </summary>
  NoGeneration = 0,
  /// <summary>
  /// Names are taken from a predefined list of names.
  /// </summary>
  Predefined = 1,
  /// <summary>
  /// Names are generated using an ordinal method, where each name is assigned a unique ordinal number.
  /// </summary>
  Ordinal = 2,
  /// <summary>
  /// Names are generated using an abbreviating method, where names are shortened or abbreviated forms of longer names.
  /// </summary>
  Abbreviating = 3,
  /// <summary>
  /// Names are generated using a method, which produces macros to process content.
  /// </summary>
  Procedural = 4,
  ///// <summary>
  ///// Numeric words are converted to numbers.
  ///// </summary>
  //Numeric = 5,
}