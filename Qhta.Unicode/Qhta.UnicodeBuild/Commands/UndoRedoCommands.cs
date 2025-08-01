using Qhta.MVVM;
using Qhta.UndoManager;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to undo the last action.
/// </summary>
public class UndoCommand : RelayCommand
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UndoCommand"/> class.
  /// </summary>
  public UndoCommand(): base(UndoMgr.Undo, () => UndoMgr.IsUndoAvailable)
  {
  }
}

/// <summary>
/// Command to redo the last action.
/// </summary>
public class RedoCommand : RelayCommand
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RedoCommand"/> class.
  /// </summary>
  public RedoCommand() : base(UndoMgr.Redo, () => UndoMgr.IsRedoAvailable)
  {
  }
}
