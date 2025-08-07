namespace Qhta.UnicodeBuild.NameGen;

/// <summary>
/// Letter case conversion to apply to generated names.
/// </summary>
public enum LetterCase
{
  /// <summary>
  /// Letters belong to unicameral scripts and their case is not sensitive.
  /// </summary>
  UniCameral = 0,
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
}