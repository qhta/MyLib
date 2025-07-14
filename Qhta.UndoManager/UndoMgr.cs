using Qhta.Collections;

namespace Qhta.UndoManager;

/// <summary>
/// Manager class for Undo/Redo functionality in application.
/// Records actions and manages undo/redo stacks.
/// Only registered actions can be undone or redone.
/// </summary>
public static class UndoMgr
{
  /// <summary>
  /// Dictionary to hold available actions and their corresponding undo/redo actions.
  /// </summary>
  private static readonly BiDiDictionary<IAction, IAction> AvailableActions = new();

  /// <summary>
  /// Entry record for storing action and its arguments in the undo/redo stacks.
  /// </summary>
  /// <param name="Action"></param>
  /// <param name="Args"></param>
  private record UndoRedoEntry(IAction Action, object? Args);

  /// <summary>
  /// Undo stack to keep track of executed actions for undo functionality.
  /// </summary>
  private static readonly Stack<UndoRedoEntry> UndoStack = new();

  /// <summary>
  /// Redo stack to keep track of undone actions for redo functionality.
  /// </summary>
  private static readonly Stack<UndoRedoEntry> RedoStack = new();

  /// <summary>
  /// Determines whether the Record/Undo/Redo functionality is enabled.
  /// </summary>
  public static bool Enabled { get; set; } = true;

  /// <summary>
  /// Flag to indicate if the application is currently undoing an action.
  /// </summary>
  public static bool IsUndoing { get; set; } = false;

  /// <summary>
  /// Flag to indicate if the application is currently redoing an action.
  /// </summary>
  public static bool IsRedoing { get; set; } = false;

  /// <summary>
  /// Flag to indicate if the application is currently recording an action.
  /// </summary>
  public static bool IsRecording { get; set; } = false;

  /// <summary>
  /// Flag to indicate if the Undo functionality is available.
  /// </summary>
  public static bool IsUndoAvailable { get; } = UndoStack.Any() && !IsUndoing && !IsRecording && !IsRecording;

  /// <summary>
  /// Flag to indicate if the Redo functionality is available.
  /// </summary>
  public static bool IsRedoAvailable { get; } = RedoStack.Any() && !IsUndoing && !IsRecording && !IsRecording;

  /// <summary>
  /// Flag to indicate if the recording functionality is available.
  /// </summary>
  public static bool IsRecordingAvailable { get; } = AvailableActions.Any() && !IsUndoing && !IsRecording && !IsRecording;

  /// <summary>
  /// Registers an action with its corresponding undo action.
  /// </summary>
  /// <param name="action"></param>
  /// <param name="undoAction"></param>
  public static void RegisterActions(IAction action, IAction undoAction)
  {
    if (AvailableActions.ContainsKey(action))
      AvailableActions[action] = undoAction;
    else
      AvailableActions.Add(action, undoAction);
  }

  /// <summary>
  /// Records an action with its arguments for undo functionality.
  /// </summary>
  /// <param name="action"></param>
  /// <param name="args"></param>
  public static void Record(IAction action, params object[] args)
  {
    if (!Enabled || !IsRecordingAvailable)
      return;
    if (!AvailableActions.ContainsKey(action))
      return;
    UndoStack.Push(new UndoRedoEntry(action, args));
  }

  /// <summary>
  /// Reverts the last operation performed, if undo is available.
  /// </summary>
  /// <remarks>This method checks if undo functionality is enabled and if there is an operation available to
  /// undo. If both conditions are met, it executes the undo operation and prepares the operation for potential
  /// redo.</remarks>
  public static void Undo()
  {
    if (!Enabled || !IsUndoAvailable)
      return;
    IsUndoing = true;
    var entry = UndoStack.Pop();
    if (AvailableActions.TryGetValue(entry.Action, out var undoAction))
    {
      undoAction.Execute(entry.Args);
      RedoStack.Push(entry);
    }
    IsUndoing = false;
  }

  /// <summary>
  /// Re-executes the last undone action if redo is available.
  /// </summary>
  /// <remarks>This method checks if redo operations are enabled,
  /// and then it executes the action and updates the undo and redo stacks accordingly.</remarks>
  public static void Redo()
  {
    if (!Enabled || !IsRedoAvailable)
      return;
    IsRedoing = true;
    var entry = RedoStack.Pop();
    entry.Action.Execute(entry.Args);
    UndoStack.Push(entry);
    IsRedoing = false;
  }
}
