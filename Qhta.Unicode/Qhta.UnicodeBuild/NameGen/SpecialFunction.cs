namespace Qhta.UnicodeBuild.NameGen;

/// <summary>
/// Special functions used in description parsing.
/// </summary>
public enum SpecialFunction
{
  /// <summary>
  /// No change is applied.
  /// </summary>
  NoChange = 0,
  /// <summary>
  /// Letters are converted to upper case.
  /// </summary>
  UpperCase,
  /// <summary>
  /// Letters are converted to lower case.
  /// </summary>
  LowerCase,
  /// <summary>
  /// Letters are converted to title case.
  /// </summary>
  TitleCase,
  /// <summary>
  /// A letter is assumed to be tonal symbol.
  /// </summary>
  Tone,
  /// <summary>
  /// Ligature is expected.
  /// </summary>
  Ligature,
}