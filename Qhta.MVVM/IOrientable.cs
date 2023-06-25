namespace Qhta.MVVM
{
  /// <summary>
  /// How is a ListViewModel oriented
  /// </summary>
  public enum OrientationType
  {
    /// <summary>
    /// Items are shown in a horizontal panel.
    /// </summary>
    Horizontal = 0,
    /// <summary>
    /// Items are shown in a vertical panel.
    /// </summary>
    Vertical = 1
  }

  /// <summary>
  /// Interface for ListViewModel to be oriented horizontally or vertically.
  /// </summary>
  public interface IOrientable
  {
    /// <summary>
    /// How is a ListViewModel oriented
    /// </summary>
    OrientationType Orientation { get; }
  }
}
