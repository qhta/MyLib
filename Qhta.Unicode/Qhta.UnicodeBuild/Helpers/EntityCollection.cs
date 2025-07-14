namespace Qhta.UnicodeBuild.Helpers;

using System;
using System.Collections.Generic;

/// <summary>
/// OrderedObservableCollection is a collection that maintains its items in a sorted order based on a key selector function.
/// It is a base class for collections that need to be ordered by a specific property or value of the items.
/// It uses ObservableList&lt;T&gt; as a base class to provide observable collection functionality.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="keySelector"></param>
/// <param name="comparer"></param>
public class EntityCollection<T>(Func<T, object> keySelector, IComparer<object>? comparer = null)
  : Qhta.ObservableObjects.ObservableList<T>, IList<T>
//    : ObservableCollection<T>, IList<T>
{

  private readonly Func<T, object> _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

  /// <summary>
  /// Inserts an item at the specified index in the collection, maintaining the order based on the key selector.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="item"></param>
  protected override void InsertItem(int index, T item)
  {
    var oldIndex = IndexOf(item);
    if (oldIndex >= 0)
    {
      if (oldIndex == index)
      {
        //Debug.WriteLine($"{item} already in collection at {oldIndex}");
        return; // Already in the correct position
      }
      else
      {
        // Remove the item from its old position
        RemoveAt(oldIndex);
        //Debug.WriteLine($"{item} removed from position {oldIndex}");
      }
    }
    index = GetInsertIndex(item);
    base.InsertItem(index, item);
    //Debug.WriteLine($"{item} added at {index}");
  }

  /// <summary>
  /// Sets the item at the specified index in the collection, removing it from its old position and inserting it in the correct order based on the key selector.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="item"></param>
  public override void SetItem(int index, T item)
  {
    RemoveAt(index);
    InsertItem(GetInsertIndex(item), item);
  }

  private int GetInsertIndex(T item)
  {
    var key = _keySelector(item);
    for (int i = 0; i < Count; i++)
    {
      var comparison = comparer?.Compare(key, _keySelector(this[i])) ?? Comparer<object>.Default.Compare(key, _keySelector(this[i]));
      if (comparison < 0)
        return i;
    }
    return Count;
  }

}                         