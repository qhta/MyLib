using System.Collections;
using System.Collections.Specialized;

using Microsoft.EntityFrameworkCore;

using Qhta.ObservableObjects;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Actions;


/// <summary>
/// Arguments for <see cref="RemoveEntityAction"/>.
/// </summary>
public record RemoveEntityArgs(IList EntityCollectionObject, Object EntityViewModel, int Index);

/// <summary>
/// Specialized action for changing state of the entry to the new state.
/// </summary>
public class RemoveEntityAction : IAction
{
  /// <summary>
  /// Executes the action to change a property.
  /// </summary>
  /// <param name="args"></param>
  /// <exception cref="ArgumentException"></exception>
  public void Execute(object? args)
  {
    if (args is not RemoveEntityArgs changeEntityStateArgs)
      throw new ArgumentException("Invalid arguments for RemoveEntityAction action", nameof(args));

    var observableCollection = changeEntityStateArgs.EntityCollectionObject;
    var item = changeEntityStateArgs.EntityViewModel;
    var index = changeEntityStateArgs.Index;
    observableCollection.RemoveAt(index);
    //observableCollection.NotifyCollectionChanged(observableCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new { dataModel }));
  }

  /// <summary>
  /// Specialized action for changing state of the entry to the old state.
  /// </summary>
  /// <param name="args"></param>
  /// <exception cref="ArgumentException"></exception>
  public void Undo(object? args)
  {
    if (args is not RemoveEntityArgs changeEntityStateArgs)
      throw new ArgumentException("Invalid arguments for RemoveEntityAction action", nameof(args));

    var observableCollection = changeEntityStateArgs.EntityCollectionObject;
    var item = changeEntityStateArgs.EntityViewModel;
    var index = changeEntityStateArgs.Index;
    observableCollection.Insert(index, item);
    //observableCollection.NotifyCollectionChanged(observableCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new { dataModel }));
  }

}