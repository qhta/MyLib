namespace Qhta.UndoManager;

/// <summary>
/// Entry record for storing action and its arguments in the undo/redo stacks.
/// </summary>
/// <param name="Action"></param>
/// <param name="Args"></param>
internal record UndoRedoEntry(IAction Action, object? Args);
