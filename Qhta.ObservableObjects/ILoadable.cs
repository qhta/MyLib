using System;

namespace Qhta.ObservableObjects;

/// <summary>
/// Interface for observable objects that can be loaded.
/// </summary>
public interface ILoadable
{
  /// <summary>
  /// Determines whether the object is loaded.
  /// </summary>
  public bool IsLoaded { get; set; }

  /// <summary>
  /// Event that is raised when the object is fully loaded.
  /// </summary>
  public EventHandler? Loaded { get; set; }
}