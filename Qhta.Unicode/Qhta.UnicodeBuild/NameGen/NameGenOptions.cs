namespace Qhta.UnicodeBuild.NameGen;

/// <summary>
/// Options for <see cref="NameGenerator"/> method.
/// </summary>
public record NameGenOptions
{
  /// <summary>
  /// Name of the file containing predefined names for Unicode points.
  /// </summary>
  public string PredefinedNamesFile { get; set; } = string.Empty;
  /// <summary>
  /// Name of the file containing phrases abbreviations.
  /// </summary>
  public string KnownPhrasesFile { get; set; } = string.Empty;
  /// <summary>
  /// Count of code points to generate names. Displayed in the dialog.
  /// </summary>
  public int CodePointsCount { get; set; }
}
