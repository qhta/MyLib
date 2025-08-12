namespace Qhta.SF.Tools;

/// <summary>
/// An interface representing an item that can be selected in a user interface.
/// </summary>
public interface ISelectableItem
{
  /// <summary>
  /// Displayed name of the item, typically used in user interfaces to represent the item.
  /// </summary>
  public string DisplayName { get; set; }

  /// <summary>
  /// Determines whether the item is currently selected in the user interface.
  /// </summary>
  public bool IsSelected { get; set; }
}