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
  /// Flag to indicate if the application is currently recording a group of actions;
  /// </summary>
  public static bool IsGrouping { get; set; } = false;

  /// <summary>
  /// Flag to indicate if the Undo functionality is available.
  /// </summary>
  public static bool IsUndoAvailable => UndoStack.Any() && !IsUndoing && !IsRedoing && !IsRecording;

  /// <summary>
  /// Flag to indicate if the Redo functionality is available.
  /// </summary>
  public static bool IsRedoAvailable => RedoStack.Any() && !IsUndoing && !IsRedoing && !IsRecording;

  /// <summary>
  /// Flag to indicate if the recording functionality is available.
  /// </summary>
  public static bool IsRecordingAvailable => !IsUndoing && !IsRecording;
  
  /// <summary>
  /// Flag to indicate if the grouping functionality is available.
  /// </summary>
  public static bool IsGroupingAvailable => !IsGrouping && !IsRecording;

  /// <summary>
  /// Records an action with its arguments for undo functionality.
  /// </summary>
  /// <param name="action"></param>
  /// <param name="args"></param>
  public static void Record(IAction action, object args)
  {
    if (!Enabled || !IsRecordingAvailable)
      return;
    if (IsGrouping)
    {
      actionGroup?.Group.Add(new UndoRedoEntry(action, args));
    }
    else
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
    entry.Action.Undo(entry.Args);
    RedoStack.Push(entry);
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

  private static ActionGroup? actionGroup = null;

  /// <summary>
  /// Starts recording a group of actions.
  /// </summary>
  public static void StartGrouping()
  {
    if (!Enabled || !IsGroupingAvailable)
      return;
    IsGrouping = true;
    actionGroup = new ActionGroup();
  }

  /// <summary>
  /// Starts recording a group of actions.
  /// </summary>
  public static void StopGrouping()
  {
    if (!Enabled || !IsGrouping)
      return;
    IsGrouping = false;
    if (actionGroup == null)
      throw new InvalidOperationException("No action group is currently being recorded.");
    UndoStack.Push(new UndoRedoEntry(actionGroup, null));
  }
}
