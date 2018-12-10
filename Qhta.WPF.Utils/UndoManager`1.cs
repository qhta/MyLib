using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xaml;

namespace Qhta.WPF.Utils
{
  public class UndoManager<ObjectType>
  {

    private Stack<string> undoStack = null;
    private Stack<string> redoStack = null;


    public void StartProtection()
    {
      if (undoStack==null)
      {
        undoStack = new Stack<string>();
        redoStack = new Stack<string>();
      }
    }

    public void Done()
    {
      undoStack = null;
      redoStack = null;
    }

    public void SaveState(ObjectType current)
    {
      var str = XamlServices.Save(current);
      undoStack.Push(str);
      redoStack.Clear();
    }

    public bool CanUndo => undoStack!=null && undoStack.Count>0;
    public bool CanRedo => redoStack!=null && redoStack.Count>0;

    public ObjectType UndoChanges(ObjectType current)
    {
      if (undoStack.Count>0)
      {
        var savedStr = undoStack.Pop();
        var str = XamlServices.Save(current);
        redoStack.Push(str);
        var savedObj = XamlServices.Load(new StringReader(savedStr));
        return (ObjectType)savedObj;
      }
      throw new InvalidOperationException($"UndoManager<{typeof(ObjectType)}> undo stack is empty");
    }

    public ObjectType RedoChanges()
    {
      if (redoStack.Count>0)
      {
        var savedStr = redoStack.Pop();
        undoStack.Push(savedStr);
        var savedObj = XamlServices.Load(new StringReader(savedStr));
        return (ObjectType)savedObj;
      }
      throw new InvalidOperationException($"UndoManager<{typeof(ObjectType)}> redo stack is empty");
    }

  }
}
