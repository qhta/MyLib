namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for object that defines HasRowDetails and IsExpanded properties.
  /// </summary>
  public interface IExpandable
  {

    /// <summary>
    /// Determines whether an object is expanded (e.g. as a tree view item).
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// Determines whether an object has details.
    /// </summary>
    bool HasRowDetails { get; set; }
  }
}
