namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for object that defines IsSelected property.
  /// </summary>
  public interface ISelectable
  {
    /// <summary>
    /// Specifies whether the object is selected.
    /// </summary>
    bool IsSelected { get; set; }
  }
}
