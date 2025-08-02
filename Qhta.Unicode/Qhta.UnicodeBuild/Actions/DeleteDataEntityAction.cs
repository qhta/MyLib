using System.Collections.Specialized;

using Microsoft.EntityFrameworkCore;

using Qhta.ObservableObjects;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Actions;


/// <summary>
/// Arguments for <see cref="DeleteDataEntityAction"/>.
/// </summary>
public record DeleteDataEntityArgs(Object DataModel, EntityState OldState, EntityState NewState);

/// <summary>
/// Specialized action for changing state of the entry to the new state.
/// </summary>
public class DeleteDataEntityAction : IAction
{
  /// <summary>
  /// Executes the action to change a property.
  /// </summary>
  /// <param name="args"></param>
  /// <exception cref="ArgumentException"></exception>
  public void Execute(object? args)
  {
    if (args is not DeleteDataEntityArgs changeEntityStateArgs)
      throw new ArgumentException("Invalid arguments for DeleteDataEntityAction action", nameof(args));

    var dbContext = _ViewModels.Instance.DbContext;
    var dataModel = changeEntityStateArgs.DataModel;
    var entry = dbContext.Entry(dataModel);


    entry.State = changeEntityStateArgs.NewState;
    foreach (var collectionEntry in entry.Collections)
    {
      // If the collection is not loaded, we need to load it first
      if (!collectionEntry.IsLoaded)
        collectionEntry.Load();
      if (collectionEntry.CurrentValue is ObservableCollectionObject observableCollection)
      {
        observableCollection.NotifyCollectionChanged(observableCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new { dataModel }));
      }
    }

  }

  /// <summary>
  /// Specialized action for changing state of the entry to the old state.
  /// </summary>
  /// <param name="args"></param>
  /// <exception cref="ArgumentException"></exception>
  public void Undo(object? args)
  {
    if (args is not DeleteDataEntityArgs changeEntityStateArgs)
      throw new ArgumentException("Invalid arguments for DeleteDataEntityAction action", nameof(args));

    var dbContext = _ViewModels.Instance.DbContext;
    var dataModel = changeEntityStateArgs.DataModel;
    var entry = dbContext.Entry(dataModel);
    if (entry.State == EntityState.Detached)
      dbContext.Attach(dataModel);

    entry.State = changeEntityStateArgs.OldState;
    foreach (var collectionEntry in entry.Collections)
    {
      // If the collection is not loaded, we need to load it first
      if (!collectionEntry.IsLoaded)
        collectionEntry.Load();
      if (collectionEntry.CurrentValue is ObservableCollectionObject observableCollection)
      {
        observableCollection.NotifyCollectionChanged(observableCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new { dataModel }));
      }
    }
  }

}