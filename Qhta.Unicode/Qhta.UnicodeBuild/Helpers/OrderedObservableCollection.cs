using System.Diagnostics;

namespace Qhta.UnicodeBuild.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class OrderedObservableCollection<T>(Func<T, object> keySelector, IComparer<object>? comparer = null)
  : ObservableCollection<T>
{
  public bool IsLoaded { get; set; }

  private readonly Func<T, object> _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

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

  protected override void SetItem(int index, T item)
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