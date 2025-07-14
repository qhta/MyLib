using Qhta.UndoManager;

namespace Qhta.UnicodeBuild.Actions;

/// <summary>
/// Arguments for changing a property.
/// </summary>
/// <param name="Target">Target object</param>
/// <param name="PropertyName">Name of the changed property</param>
/// <param name="OldValue">Old value of the property</param>
/// <param name="NewValue">New value of the property</param>
public record ChangePropertyArgs(Object Target, string PropertyName, object? OldValue, object? NewValue);

/// <summary>
/// Action for changing a property.
/// </summary>
public class ChangePropertyAction: IAction
{
  /// <summary>
  /// Executes the action to change a property.
  /// </summary>
  /// <param name="args"></param>
  /// <exception cref="NotImplementedException"></exception>
  public void Execute(object? args)
  {
    if (args is not ChangePropertyArgs changePropertyArgs)
      throw new ArgumentException("Invalid arguments for ChangeProperty action", nameof(args));
    var property = changePropertyArgs.Target.GetType().GetProperty(changePropertyArgs.PropertyName);
    if (property == null)
      throw new ArgumentException($"Property '{changePropertyArgs.PropertyName}' not found on target object", nameof(changePropertyArgs.PropertyName));
    property.SetValue(changePropertyArgs.Target, changePropertyArgs.NewValue);
  }

  /// <summary>
  /// Undoes the action to change a property, restoring the old value.
  /// </summary>
  /// <param name="args"></param>
  /// <exception cref="ArgumentException"></exception>
  public void Undo(object? args)
  {
    if (args is not ChangePropertyArgs changePropertyArgs)
      throw new ArgumentException("Invalid arguments for ChangeProperty action", nameof(args));
    var property = changePropertyArgs.Target.GetType().GetProperty(changePropertyArgs.PropertyName);
    if (property == null)
      throw new ArgumentException($"Property '{changePropertyArgs.PropertyName}' not found on target object", nameof(changePropertyArgs.PropertyName));
    property.SetValue(changePropertyArgs.Target, changePropertyArgs.OldValue);
  }
}