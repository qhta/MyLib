namespace Qhta.OpenXmlTools;

/// <summary>
/// Options for FindAndReplace methods.
/// </summary>
public record FindAndReplaceOptions
{
  /// <summary>
  /// Find option - whole words only.
  /// </summary>
  public bool FindWholeWordsOnly;

  /// <summary>
  /// Find and replace option - case-insensitive.
  /// </summary>
  public bool MatchCaseInsensitive;
}