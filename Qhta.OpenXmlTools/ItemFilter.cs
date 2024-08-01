namespace Qhta.OpenXmlTools;

/// <summary>
/// Specifies which items to count or get.
/// </summary>
public enum ItemFilter
{
  /// <summary>
  /// Count or get only defined items.
  /// </summary>
  Defined,
  /// <summary>
  /// Count or get all items.
  /// </summary>
  All,
  /// <summary>
  /// Count or get only built-in items.
  /// </summary>
  BuiltIn,
}
