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
}