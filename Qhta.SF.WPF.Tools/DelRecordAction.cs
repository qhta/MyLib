using System.Collections;
using Qhta.UndoManager;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Arguments for changing a property.
/// </summary>
/// <param name="DataList">Target object</param>
/// <param name="Index">Number index of the record</param>
/// <param name="DataObject">Old value of the property</param>
public record DelRecordArgs(IList DataList, int? Index, object DataObject);

/// <summary>
/// Undo/redo action for deleting records in a data grid.
/// </summary>
public class DelRecordAction: IAction
{
  /// <summary>
  /// Deletes a record in a data grid.
  /// </summary>
  /// <param name="args">Must be <see cref="DelRecordArgs"/></param>
  public void Execute(object? args)
  {
    if (args is not DelRecordArgs delRecordArgs)
      throw new ArgumentNullException(nameof(args));
    delRecordArgs.DataList.Remove(delRecordArgs.DataObject);
  }

  /// <summary>
  /// Undo a delete action.
  /// </summary>
  /// <param name="args">Must be <see cref="DelRecordArgs"/></param>
  public void Undo(object? args)
  {
    if (args is not DelRecordArgs delRecordArgs)
      throw new ArgumentNullException(nameof(args));
    if (delRecordArgs.Index.HasValue && delRecordArgs.Index.Value >= 0 && delRecordArgs.Index.Value < delRecordArgs.DataList.Count)
      delRecordArgs.DataList.Insert((int)delRecordArgs.Index, delRecordArgs.DataObject);
    else
      delRecordArgs.DataList.Add(delRecordArgs.DataObject);
  }
}