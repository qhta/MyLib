namespace Qhta.SF.Tools;

/// <summary>
/// Class representing a selectable item with a display name and selection state - for UI purposes.
/// </summary>
public class SelectableItemStub: ISelectableItem
{
  /// <inheritdoc />
  public string DisplayName { get; set; } = String.Empty;

  /// <inheritdoc />
  public bool IsSelected { get; set; }
}