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
  /// Optional tooltip text that provides additional information about the item when hovered over in a user interface.
  /// </summary>
  public string? ToolTip { get; }

  /// <summary>
  /// Actual value of the item, which can be of any type. This is the underlying data associated with the item.
  /// </summary>
  public object? ActualValue { get; }

  /// <summary>
  /// Determines whether the item is currently selected in the user interface.
  /// </summary>
  public bool IsSelected { get; set; }

  /// <summary>
  /// Determines whether the item should be considered as empty value.
  /// </summary>
  public bool IsEmpty => ActualValue is null;

  /// <summary>
  /// Determines whether the item should be considered as any non-empty value.
  /// </summary>
  public bool IsNonEmpty => ActualValue is NonEmptyValue;
}
