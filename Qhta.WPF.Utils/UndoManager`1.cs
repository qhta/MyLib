using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Qhta.WPF.Utils
{
  public class UndoManager<ObjectType>
  {

    private Stack<ObjectType> undoStack = null;
    private Stack<ObjectType> redoStack = null;


    public void StartProtection()
    {
      if (undoStack==null)
      {
        undoStack = new Stack<ObjectType>();
        redoStack = new Stack<ObjectType>();
      }
    }

    public void Done()
    {
      undoStack = null;
      redoStack = null;
    }

    public void SaveState(ObjectType current)
    {
      undoStack.Push(current);
      redoStack.Clear();
    }

    public bool CanUndo => undoStack!=null && undoStack.Count>0;
    public bool CanRedo => redoStack!=null && redoStack.Count>0;

    public ObjectType UndoChanges(ObjectType current)
    {
      if (undoStack.Count>0)
      {
        var saved = undoStack.Pop();
        redoStack.Push(current);
        return saved;
      }
      throw new InvalidOperationException($"UndoManager<{typeof(ObjectType)}> undo stack is empty");
    }

    public ObjectType RedoChanges()
    {
      if (redoStack.Count>0)
      {
        var saved = redoStack.Pop();
        undoStack.Push(saved);
        return saved;
      }
      throw new InvalidOperationException($"UndoManager<{typeof(ObjectType)}> redo stack is empty");
    }

  }
}
