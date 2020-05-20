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
    IEnumerable<TValue>,
    ICollection<TValue>,
    IList<TValue>,
    IEnumerable,
    ICollection,
    IList,
    INotifyCollectionChanged, INotifyPropertyChanged where TValue : class
  {
    public ObservableDataSet() { }

    public readonly ObservableList<TValue> Items = new ObservableList<TValue>();

    #region LazyLoad functionality

    /// <summary>
    /// This property determines if the content should be populated at the enumerate operation (when needed).
    /// </summary>
    public bool LazyLoad
    {
      get => lazyLoad;
      set
      {
        if (lazyLoad != value)
        {
          lazyLoad = value;
          NotifyPropertyChanged(nameof(LazyLoad));
        }
      }
    }
    private bool lazyLoad;

    /// <summary>
    /// Populates the content instances.
    /// This function must be implemented in the specific class
    /// if LazyLoad functionality is active.
    /// </summary>
    public virtual void Populate() { }

    /// <summary>
    /// Set in the time of Populate.
    /// </summary>
    public bool IsPopulating
    {
      get => isPopulating;
      protected set
      {
        if (isPopulating != value)
        {
          isPopulating = value;
          NotifyPropertyChanged(nameof(IsPopulating));
        }
      }
    }
    private bool isPopulating;

    /// <summary>
    /// Set after Populate.
    /// </summary>
    public bool IsPopulated
    {
      get => isPopulated;
      protected set
      {
        if (isPopulated != value)
        {
          isPopulated = value;
          NotifyPropertyChanged(nameof(IsPopulated));
        }
      }
    }
    private bool isPopulated;

    /// <summary>
    /// A method to populate contents when it is needed.
    /// </summary>
    /// <returns></returns>
    public virtual void PopulateWhenNeeded()
    {
      ////if (IsPopulating)
      ////  return;
      ////IsPopulating = true;
      var lazyLoadSave = LazyLoad;
      lazyLoad = false;
      if (Count != Items.Count)
      {
        Populate();
        IsPopulated = true;
      }
      lazyLoad = lazyLoadSave;
      IsPopulating = false;
    }

    /// <summary>
    /// The number of items.
    /// This function should be changed in the specific class
    /// if LazyLoad functionality is active.
    /// </summary>
    /// <returns></returns>
    public virtual int Count => (LazyLoad) ? 0 : Items.Count;
    #endregion

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
      ok = oldIndex >= 0;
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

    public virtual bool Contains(TValue item)
    {
      PopulateWhenNeeded();
      return Items.Contains(item);
    }

    public virtual void CopyTo(TValue[] array, int arrayIndex)
    {
      PopulateWhenNeeded();
      Items.ToArray().CopyTo(array, arrayIndex);
    }

    public virtual IEnumerator<TValue> GetEnumerator()
    {
      PopulateWhenNeeded();
      return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public virtual int IndexOf(TValue value)
    {
      PopulateWhenNeeded();
      return Items.IndexOf(value);
    }

    public virtual TValue this[int index]
    {
      get
      {
        PopulateWhenNeeded();
        return Items[index];
      }
      set => throw new NotImplementedException();
    }

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

    public virtual object SyncRoot => Items.SyncRoot;

    public virtual bool IsSynchronized => Items.IsSynchronized;

    public virtual bool IsModified { get; protected set; }

    object IList.this[int index] { get => this[index]; set => this[index]=(TValue)value; }

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

    public int Add(object value)
    {
      if (value is TValue item)
      {
        this.Add(item);
        return Items.IndexOf(item);
      }
      return -1;
    }

    public bool Contains(object value)
    {
      if (value is TValue item)
      {
        return Items.Contains(item);
      }
      return false;
    }

    public int IndexOf(object value)
    {
      if (value is TValue item)
      {
        return Items.IndexOf(item);
      }
      return -1;
    }

    public void Insert(int index, object value)
    {
      throw new NotImplementedException();
    }

    public void Remove(object value)
    {
      if (value is TValue item)
      {
        this.Remove(item);
      }
    }

    public void CopyTo(Array array, int index)
    {
      if (array is TValue[] items)
      {
        this.CopyTo(items, index);
      }
    }
  }

}

