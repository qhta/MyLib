using System.Xaml;

namespace Qhta.WPF.Utils;

/// <summary>
/// Implements undo operation for a specific object type.
/// </summary>
/// <typeparam name="ObjectType"></typeparam>
public class UndoManager<ObjectType>
{

  private Stack<string>? undoStack = null;
  private Stack<string>? redoStack = null;


  /// <summary>
  /// Starts registering undo and redo operation.
  /// </summary>
  public void StartProtection()
  {
    if (undoStack == null)
    {
      undoStack = new Stack<string>();
      redoStack = new Stack<string>();
    }
  }

  /// <summary>
  /// Ends managing.
  /// </summary>
  public void Done()
  {
    undoStack = null;
    redoStack = null;
  }

  /// <summary>
  /// Save the current state.
  /// </summary>
  /// <param name="current"></param>
  public void SaveState(ObjectType current)
  {
    var str = XamlServices.Save(current);
    undoStack?.Push(str);
    redoStack?.Clear();
  }

  /// <summary>
  /// Checks if undo stack is not empty.
  /// </summary>
  public bool CanUndo => undoStack != null && undoStack.Count > 0;

  /// <summary>
  /// Checks if redo stack is not empty.
  /// </summary>
  public bool CanRedo => redoStack != null && redoStack.Count > 0;

  /// <summary>
  /// Undoes changes on specified object type.
  /// </summary>
  /// <param name="current"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public ObjectType UndoChanges(ObjectType current)
  {
    if (undoStack != null)
    {
      if (undoStack.Count > 0)
      {
        var savedStr = undoStack.Pop();
        var str = XamlServices.Save(current);
        redoStack?.Push(str);
        var savedObj = XamlServices.Load(new StringReader(savedStr));
        return (ObjectType)savedObj;
      }
    }
    throw new InvalidOperationException($"UndoManager<{typeof(ObjectType)}> undo stack is empty");
  }

  /// <summary>
  /// Redoes changes.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public ObjectType RedoChanges()
  {
    if (redoStack != null)
    {
      if (redoStack.Count > 0)
      {
        var savedStr = redoStack.Pop();
        undoStack?.Push(savedStr);
        var savedObj = XamlServices.Load(new StringReader(savedStr));
        return (ObjectType)savedObj;
      }
    }
    throw new InvalidOperationException($"UndoManager<{typeof(ObjectType)}> redo stack is empty");
  }

}
