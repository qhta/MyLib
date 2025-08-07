using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Qhta.MVVM;
using Qhta.UndoManager;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Represents a view model for an entity of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>This class provides a default constructor that initializes the view model with a new instance of the
/// entity type if no entity is provided. It inherits from <see cref="ViewModel{T}"/>.</remarks>
/// <typeparam name="T">The type of the entity, which must be a class with a parameterless constructor.</typeparam>
public class EntityViewModel<T>: ViewModel<T>
  where T : class, new()
{
  /// <summary>
  /// Default constructor that initializes the ViewModel with a new instance of the entity type.
  /// </summary>
  /// <param name="entity"></param>
  public EntityViewModel(T? entity = null)
    : base(entity ?? new T())
  {
  }

  /// <summary>
  /// Changes the value of a specified property on the current instance.
  /// </summary>
  /// <remarks>If the new value is equal to the current value, no change is made. The change is recorded for
  /// undo functionality, and property change notifications are triggered.</remarks>
  /// <param name="propertyName">The name of the property to change. Must be a valid property name of the current instance's type.</param>
  /// <param name="newValue">The new value to assign to the property. Can be null if the property type allows it.</param>
  /// <param name="pairedPropertyName">The name of the paired property to notify that was changed too.</param>
  /// <returns>True if change was done, false if no change was needed</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="propertyName"/> does not correspond to a valid property on the current instance's type.</exception>
  public bool ChangeThisProperty(string propertyName, object? newValue, string? pairedPropertyName = null)
  {
    var type = this.GetType();
    var property = type.GetProperty(propertyName);
    if (property == null)
      throw new ArgumentException($"Property '{propertyName}' not found on entity type {type.Name}", nameof(propertyName));
    
    var oldValue = property.GetValue(this);
    if (!UndoMgr.IsUndoing)
      if (Equals(oldValue, newValue)) return false; // No change needed
    // Record the change for undo functionality
    UndoMgr.Record(new ChangePropertyAction(), new ChangePropertyArgs(this, propertyName, oldValue, newValue));
    ChangeModelProperty(propertyName, newValue);
    NotifyPropertyChanged(propertyName);
    if (pairedPropertyName != null && pairedPropertyName != propertyName)
      NotifyPropertyChanged(pairedPropertyName);
    return true;
  }

  /// <summary>
  /// Changes the value of a specified model property and notifies a paired property was changed.
  /// </summary>
  /// <param name="propertyName">The name of the property of the underlying model to change. Must be a valid property name of the model instance's type.</param>
  /// <param name="newValue">The new value to assign to the property. Can be null if the property type allows it.</param>

  /// <returns>True if change was done, false if no change was needed</returns>
  private bool ChangeModelProperty(string propertyName, object? newValue)
  {
    var type = Model.GetType();
    var property = type.GetProperty(propertyName);
    if (property == null)
      throw new ArgumentException($"Property '{propertyName}' not found on entity type {type.Name}", nameof(propertyName));

    var oldValue = property.GetValue(Model);
    if (!UndoMgr.IsUndoing)
      if (Equals(oldValue, newValue)) return false; // No change needed
    // Record the change for undo functionality
    UndoMgr.Record(new ChangePropertyAction(), new ChangePropertyArgs(Model, propertyName, oldValue, newValue));
    property.SetValue(Model, newValue);
    NotifyPropertyChanged(propertyName);
    return true;
  }
}
