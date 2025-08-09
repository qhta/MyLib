namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Specifies how to find a value in the context of the current selection.
/// </summary>
public enum FindInSequence
{
  /// <summary>
  /// Find the next occurrence of the value.
  /// </summary>
  FindNext,
  /// <summary>
  /// Find the first occurrence of the value.
  /// </summary>
  FindFirst,
  /// <summary>
  /// Find all occurrences of the value.
  /// </summary>
  FindAll,
}