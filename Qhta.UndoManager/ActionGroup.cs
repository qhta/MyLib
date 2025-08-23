
using System.Diagnostics;

namespace Qhta.UndoManager;

/// <summary>
/// Specialized action group for grouping actions.
/// </summary>
public class ActionGroup: IAction
{
  internal readonly List<UndoRedoEntry> Group = new();

  /// <summary>
  /// Executes all actions in the group with the provided arguments.
  /// </summary>
  /// <param name="args"></param>
  public virtual void Execute(object? args)
  {
    for (int i=0; i<Group.Count; i++)
    {
      var entry = Group[i];
      try
      {
        entry.Action.Execute(entry.Args);
      }
      catch (Exception e)
      {
        Debug.WriteLine($"Error executing action {entry.Action.GetType().Name}: {e.Message}");
        // Optionally, you could log the error or handle it in a specific way
      }
    }
  }

  /// <summary>
  /// Undoes all actions in the group in the backward direction.
  /// </summary>
  /// <param name="args"></param>
  /// <exception cref="NotImplementedException"></exception>
  public virtual void Undo(object? args)
  {
    for (int i = Group.Count-1; i>=0; i--)
    {
      var entry = Group[i];
      try
      {
        entry.Action.Undo(entry.Args);
      }
      catch (Exception e)
      {
        Debug.WriteLine($"Error undoing action {entry.Action.GetType().Name}: {e.Message}");
        // Optionally, you could log the error or handle it in a specific way
      }
    }
  }
}