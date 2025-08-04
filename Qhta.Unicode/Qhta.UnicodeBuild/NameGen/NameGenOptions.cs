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
  /// Name of the file containing word abbreviations.
  /// </summary>
  public string AbbreviatedWordsFile { get; set; } = string.Empty;
  /// <summary>
  /// Specifies whether to use <see cref="KnownNumeralsFile"/>.
  /// </summary>
  public bool UseKnownNumerals { get; set; }
  /// <summary>
  /// Name of the file containing numerals phrases and their mappings.
  /// </summary>
  public string KnownNumeralsFile { get; set; } = string.Empty;
  /// <summary>
  /// Count of code points to generate names. Displayed in the dialog.
  /// </summary>
  public int CodePointsCount { get; set; }
}
