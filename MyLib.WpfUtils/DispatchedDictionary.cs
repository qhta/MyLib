﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyLib.WpfUtils
{
  public class DispatchedDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IDisposable
    , ICollection<TValue>, IEnumerable<TValue>
    , INotifyCollectionChanged, INotifyPropertyChanged
  {

    public Dispatcher _Dispatcher = Application.Current.Dispatcher;

    public string Name { get; private set; }
    #region constructors
    public DispatchedDictionary() : base()
    {
    }

    public DispatchedDictionary(string name) : base()
    {
      Name = name;
    }

    public DispatchedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection)
    {
    }

    public DispatchedDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
    {
    }

    public DispatchedDictionary(int concurrencyLevel, int capacity) : base(concurrencyLevel, capacity)
    {
    }

    public DispatchedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
      : base(collection, comparer)
    {
    }

    public DispatchedDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
      : base(concurrencyLevel, collection, comparer)
    {
    }

    public DispatchedDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
      : base(concurrencyLevel, capacity, comparer)
    {
    }

    #endregion constructors

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~ObservableConcurrentDictionary() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion

    private SynchronizationContext _context;
    private readonly object _lock = new object();
    private readonly ReaderWriterLockSlim ItemsLock = new ReaderWriterLockSlim();

    #region events
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add
      {
        _CollectionChanged+=value;
      }
      remove
      {
        _CollectionChanged-=value;
      }
    }
    private event NotifyCollectionChangedEventHandler _CollectionChanged;

    public event PropertyChangedEventHandler PropertyChanged;
    public void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region overriden properties
    public new ObservableConcurrentDictionaryValues<TValue> Values
    {
      get
      {
        if (_Values==null)
          _Values = new ObservableConcurrentDictionaryValues<TValue>(this, base.Values);
        return _Values;
      }
    }
    #endregion
    private ObservableConcurrentDictionaryValues<TValue> _Values;

    #region overriden methods
    public new TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
    {
      int oldCount, newCount;
      TValue oldItem, newItem;
      lock (_lock)
      {
        oldCount = Count;
        TryGetValue(key, out oldItem);
        newItem = base.AddOrUpdate(key, addValueFactory, updateValueFactory);
        newCount = Count;
      }
      ThrowAddOrUpdateEvents(oldItem, newItem, oldCount, newCount);
      return newItem;
    }

    public new TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
    {
      int oldCount, newCount;
      TValue oldItem, newItem;
      lock (_lock)
      {
        oldCount = Count;
        TryGetValue(key, out oldItem);
        newItem = base.AddOrUpdate(key, addValue, updateValueFactory);
        newCount = Count;
      }
      ThrowAddOrUpdateEvents(oldItem, newItem, oldCount, newCount);
      return newItem;
    }

    ////
    //// Summary:
    ////     Removes all keys and values from the System.Collections.Concurrent.ConcurrentDictionary`2.
    //public void Clear();


    public new TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
      TValue oldItem, newItem;
      lock (_lock)
      {
        TryGetValue(key, out oldItem);
        newItem = base.GetOrAdd(key, valueFactory);
      }
      ThrowGetOrAddEvents(oldItem, newItem);
      return newItem;
    }
    public new TValue GetOrAdd(TKey key, TValue value)
    {
      var result = TryAdd(key, value);
      if (!result)
        return base[key];
      else
        return value;
    }


    public new bool TryAdd(TKey key, TValue value)
    {
      bool result = base.TryAdd(key, value);
      if (result)
        ThrowAddEvent(value);
      return result;
    }

    public new bool TryRemove(TKey key, out TValue value)
    {
      bool result = base.TryRemove(key, out value);
      if (result)
        ThrowRemoveEvent(value);
      return result;
    }
    public new bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
    {
      bool result = base.TryUpdate(key, newValue, comparisonValue);
      if (result)
        ThrowReplaceEvent(comparisonValue, newValue);
      return result;
    }

    private void ThrowAddOrUpdateEvents(TValue oldItem, TValue newItem, int oldCount, int newCount)
    {
      if (newCount!=Count)
        ThrowAddEvent(newItem);
      else
        ThrowReplaceEvent(oldItem, newItem);
    }

    private void ThrowGetOrAddEvents(TValue oldItem, TValue newItem)
    {
      if (newItem.Equals(oldItem))
        ThrowAddEvent(newItem);
    }

    private void ThrowAddEvent(TValue newItem)
    {
      if (_CollectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==_Dispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            newItem
            ));
        else
        {
          var action = new Action<TValue>(ThrowAddEvent);
          _Dispatcher.BeginInvoke(action, new object[] { newItem });
        }
      }
    }

    private void ThrowReplaceEvent(TValue oldItem, TValue newItem)
    {
      if (_CollectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==_Dispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
            newItem, oldItem
            ));
        else
        {
          var action = new Action<TValue, TValue>(ThrowReplaceEvent);
          _Dispatcher.BeginInvoke(action, new object[] { newItem });
        }
      }
    }

    private void ThrowRemoveEvent(TValue oldItem)
    {
      if (_CollectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==_Dispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
            oldItem
            ));
        else
        {
          var action = new Action<TValue>(ThrowRemoveEvent);
          _Dispatcher.BeginInvoke(action, new object[] { oldItem });
        }
      }
    }
    #endregion overriden methods

    #region ICollection<TValue> support

    public bool IsReadOnly => false;

    void ICollection<TValue>.Add(TValue item)
    {
      throw new NotImplementedException();
    }

    bool ICollection<TValue>.Contains(TValue item)
    {
      return Values.Contains(item);
    }

    void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
    {
      Values.CopyTo(array, arrayIndex);
    }

    bool ICollection<TValue>.Remove(TValue item)
    {
      throw new NotImplementedException();
    }

    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
      return Values.GetEnumerator();
    }
    #endregion ICollection<TValue> support
  }

  public class ObservableConcurrentDictionaryValues<TValue> : ICollection<TValue>, INotifyCollectionChanged
  {
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add { Owner.CollectionChanged+=value; }
      remove { Owner.CollectionChanged-=value; }
    }

    public INotifyCollectionChanged Owner { get; private set; }

    public ICollection<TValue> Collection { get; private set; }

    public int Count => Collection.Count;

    public bool IsReadOnly => Collection.IsReadOnly;

    public ObservableConcurrentDictionaryValues(INotifyCollectionChanged owner, ICollection<TValue> collection)
    {
      Owner = owner;
      Collection = collection;
    }

    public void Add(TValue item)
    {
      Collection.Add(item);
    }

    public void Clear()
    {
      Collection.Clear();
    }

    public bool Contains(TValue item)
    {
      return Collection.Contains(item);
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
      Collection.CopyTo(array, arrayIndex);
    }

    public bool Remove(TValue item)
    {
      return Collection.Remove(item);
    }

    public IEnumerator<TValue> GetEnumerator()
    {
      return Collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return Collection.GetEnumerator();
    }
  }

}