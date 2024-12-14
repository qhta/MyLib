namespace Qhta.OpenXmlTools;

/// <summary>
/// Options for FindAndReplace methods.
/// </summary>
public record FindAndReplaceOptions
{
  /// <summary>
  /// Search option - whole words only.
  /// </summary>
  public bool FindWholeWordsOnly;

  /// <summary>
  /// Search and replace option - case-insensitive.
  /// </summary>
  public bool MatchCaseInsensitive;
}