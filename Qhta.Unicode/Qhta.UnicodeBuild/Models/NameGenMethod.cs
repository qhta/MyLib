namespace Qhta.Unicode.Models;

/// <summary>
/// This enum defines the methods used for generating names.
/// </summary>
public enum NameGenMethod: byte
{
  /// <summary>
  /// Name are not generated.
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
  /// Names are generated using an abbreviation method, where names are shortened or abbreviated forms of longer names.
  /// </summary>
  Abbreviation = 3,
  /// <summary>
  /// Names are generated using a method, which produces macros to process content.
  /// </summary>
  Procedural = 4,
}