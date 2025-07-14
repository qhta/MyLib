using Qhta.MVVM;
using Qhta.UndoManager;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  /// <summary>
  /// Command to undo the last action.
  /// </summary>
  public IRelayCommand UndoCommand = new RelayCommand (UndoMgr.Undo, () => UndoMgr.IsUndoAvailable);

  /// <summary>
  /// Command to redo the last action.
  /// </summary>
  public IRelayCommand RedoCommand = new RelayCommand(UndoMgr.Redo, () => UndoMgr.IsRedoAvailable);
}