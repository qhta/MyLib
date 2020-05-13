using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Qhta.ObservableImmutable;

namespace Qhta.ObservableViewModels
{
  public class ObservableDataSet<TValue> : ObservableCollectionObject,
      ICollection<TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IList<TValue> where TValue : class
  {
    public ObservableDataSet() { }

    public readonly ObservableList<TValue> Items = new ObservableList<TValue>();

    public virtual int Count => Items.Count;

    public virtual bool IsReadOnly => false;

    public virtual void Add(TValue item)
    {
      int oldCount = Items.Count;
      Items.Add(item);
      IsModified = true;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, oldCount);
    }

    public virtual bool Remove(TValue item)
    {
      bool ok;
      var oldIndex = Items.IndexOf(item);
      ok = oldIndex>=0;
      if (ok)
      {
        Items.Remove(item);
      }
      IsModified = ok;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, oldIndex);
      return ok;
    }

    public virtual void Clear()
    {
      Items.Clear();
    }

    public virtual bool Contains(TValue item) => Items.Contains(item);

    public virtual void CopyTo(TValue[] array, int arrayIndex)
    {
      Items.ToArray().CopyTo(array, arrayIndex);
    }

    public virtual IEnumerator<TValue> GetEnumerator()
    {
      return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public virtual int IndexOf(TValue value)
    {
      return Items.IndexOf(value);
    }

    public TValue this[int index] { get => Items[index]; set => throw new NotImplementedException(); }

    public virtual TValue SelectedItem
    {
      get => selectedItem;
      set
      {
        if (!selectedItem.Equals(value))
        {
          selectedItem = value;
          base.NotifyPropertyChanged(nameof(SelectedItem));
        }
      }
    }

    public bool IsFixedSize => false;

    public virtual object SyncRoot => throw new NotImplementedException();

    public virtual bool IsSynchronized => throw new NotImplementedException();

    public virtual bool IsModified { get; protected set; }

    private TValue selectedItem;

    public bool ShouldSerializeSelectedItem() => false;

    public virtual void Insert(int index, TValue item)
    {
      Items.Insert(index, item);
      IsModified = true;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }

    public virtual void RemoveAt(int index)
    {
      var item = Items[index];
      Items.RemoveAt(index);
      IsModified = true;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
    }

    public virtual void CopyTo(Array array, int index)
    {
      Items.ToArray().CopyTo(array, index);
    }
  }

}

