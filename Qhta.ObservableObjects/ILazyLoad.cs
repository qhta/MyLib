namespace Qhta.ObservableObjects;

/// <summary>
/// Interface for object with lazy-loading functionality.
/// </summary>
public interface ILazyLoad
{
  /// <summary>
  /// Determines whether the object is loaded.
  /// </summary>
  public bool IsLoaded { get; set; }
}