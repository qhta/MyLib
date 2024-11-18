namespace Qhta.OpenXmlTools;

/// <summary>
/// Specifies which items to count or get.
/// </summary>
public enum ItemFilter
{
  /// <summary>
  /// RowsCount or get only defined items.
  /// </summary>
  Defined,
  /// <summary>
  /// RowsCount or get all items.
  /// </summary>
  All,
  /// <summary>
  /// RowsCount or get only built-in items.
  /// </summary>
  BuiltIn,
}
