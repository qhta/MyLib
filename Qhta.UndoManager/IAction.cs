namespace Qhta.UndoManager;

/// <summary>
/// Interface for actions that can be executed with arguments.
/// </summary>
public interface IAction
{
  /// <summary>
  /// Executes the action with the provided arguments.
  /// </summary>
  /// <param name="args"></param>
  public void Execute(object? args);

  /// <summary>
  /// Undoes the action with the provided arguments, restoring the previous state.
  /// </summary>
  /// <param name="args"></param>
  public void Undo(object? args);
}