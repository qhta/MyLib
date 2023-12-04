namespace Qhta.WPF.Utils;

/// <summary>
/// Helper class to drag data. Contains source DependencyObject and dragged data.
/// </summary>
public class DragData
{
  /// <summary>
  /// Source DependencyObject.
  /// </summary>
  public DependencyObject? Source { get; set; }

  /// <summary>
  /// Dragged data.
  /// </summary>
  public object? Data { get; set; }
}
