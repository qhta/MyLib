namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Specifies what to do with a column filter inside filter dialog.
/// </summary>
public enum FilterEditOperation
{
  /// <summary>
  /// Add an And filter above this filter and make this filter left operand.
  /// </summary>
  AddAndAbove,

  /// <summary>
  /// Add an And filter above this filter and make this filter left operand.
  /// </summary>
  AddOrAbove,

}
