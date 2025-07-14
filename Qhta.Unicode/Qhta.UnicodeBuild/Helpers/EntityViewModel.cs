using Qhta.MVVM;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// ///Base class for entities in the application.
/// </summary>
public class EntityViewModel<T>: ViewModel<T>
{
  /// <summary>
  /// Default constructor for the EntityViewModel class.
  /// </summary>
  /// <param name="entity"></param>
  public EntityViewModel(T entity) : base(entity) { }

  /// <summary>
  /// Collection that contains entities of this type.
  /// </summary>
  public EntityCollection<EntityViewModel<T>>? Collection { get; set; }

  /// <summary>
  /// Gets a value indicating whether the collection is fully loaded.
  /// </summary>
  public new bool IsLoaded => Collection?.IsLoaded ?? base.IsLoaded;
}