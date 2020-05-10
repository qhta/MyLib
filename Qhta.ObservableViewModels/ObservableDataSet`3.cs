using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Qhta.ObservableImmutable;

namespace Qhta.ObservableViewModels
{
  /// <summary>
  /// DataSet with Primary and Secondary index.
  /// SecondaryKey type must be different than PrimaryKey type.
  /// </summary>
  /// <typeparam name="TPrimary"></typeparam>
  /// <typeparam name="TSecondaryKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class ObservableDataSet<TPrimary, TSecondaryKey, TValue>: ObservableDataSet<TPrimary, TValue> 
    where TPrimary : IComparable<TPrimary> where TSecondaryKey : IComparable<TSecondaryKey> where TValue: class
  {
    private ObservableDataIndex<TSecondaryKey, TValue> SecondaryIndex = 
      new ObservableDataIndex<TSecondaryKey, TValue>();

    public override void Add(TValue item)
    {
      SecondaryIndex.Add(item);
      base.Add(item);
    }

    public override bool Remove(TValue item)
    {
      SecondaryIndex.Remove(item);
      return base.Remove(item);
    }

    public override void Clear()
    {
      SecondaryIndex.Clear();
      base.Clear();
    }
    public virtual bool TryGetValue(TSecondaryKey index, out TValue item)
    {
      return SecondaryIndex.TryGetValue(index, out item);
    }


    public virtual int IndexOfKey(TSecondaryKey key)
    {
      if (SecondaryIndex.TryGetValue(key, out var item))
      {
        return IndexOf(item);
      }
      return -1;
    }
  }
}
