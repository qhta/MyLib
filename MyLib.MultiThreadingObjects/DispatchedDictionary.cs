using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyLib.MultiThreadingObjects
{
  public class DispatchedDictionary<TKey, TValue> : DispatchedObject, IDisposable
    , ICollection<TValue>, IEnumerable<TValue>
    , INotifyCollectionChanged, INotifyPropertyChanged
  {

    protected ConcurrentDictionary<TKey, TValue> Dictionary;

    #region constructors
    public DispatchedDictionary()
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>();
    }

    public DispatchedDictionary(string name)
    {
      Name = name;
      Dictionary = new ConcurrentDictionary<TKey, TValue>();
    }

    public DispatchedDictionary(IEnumerable<TValue> collection)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>();
    }

    public DispatchedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(collection);
    }

    public DispatchedDictionary(IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(comparer);
    }

    public DispatchedDictionary(int concurrencyLevel, int capacity)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity);
    }

    public DispatchedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
    }

    public DispatchedDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, collection, comparer);
    }

    public DispatchedDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer);
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

    public bool HasItems
    {
      get { return _HasItems; }
      set
      {
        if (_HasItems!=value)
        {
          _HasItems=value;
          NotifyPropertyChanged("HasItems");
        }
      }
    }
    private bool _HasItems;

    private readonly object _lock = new object();

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

    #endregion

    #region overriden properties
    public DispatchedDictionaryValues Values
    {
      get
      {
        if (_Values==null)
          _Values = new DispatchedDictionaryValues(this);
        return _Values;
      }
    }
    #endregion
    private DispatchedDictionaryValues _Values;

    #region overriden methods
    public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
    {
      int oldCount, newCount;
      TValue oldItem, newItem;
      lock (_lock)
      {
        oldCount = Dictionary.Count;
        Dictionary.TryGetValue(key, out oldItem);
        newItem = Dictionary.AddOrUpdate(key, addValueFactory, updateValueFactory);
        newCount = Dictionary.Count;
      }
      ThrowAddOrUpdateEvents(oldItem, newItem, oldCount, newCount);
      return newItem;
    }

    public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
    {
      int oldCount, newCount;
      TValue oldItem, newItem;
      lock (_lock)
      {
        oldCount = Dictionary.Count;
        Dictionary.TryGetValue(key, out oldItem);
        newItem = Dictionary.AddOrUpdate(key, addValue, updateValueFactory);
        newCount = Dictionary.Count;
        ThrowAddOrUpdateEvents(oldItem, newItem, oldCount, newCount);
      }
      return newItem;
    }


    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
      TValue oldItem, newItem;
      lock (_lock)
      {
        Dictionary.TryGetValue(key, out oldItem);
        newItem = Dictionary.GetOrAdd(key, valueFactory);
      }
      ThrowGetOrAddEvents(oldItem, newItem);
      return newItem;
    }

    public TValue GetOrAdd(TKey key, TValue value)
    {
      var result = TryAdd(key, value);
      if (!result)
      {
        //Debug.WriteLine($"{Name}.GetOrAdd {value} just exists");
        return Dictionary[key];
      }
      else
      {
        //Debug.WriteLine($"{Name}.GetOrAdd {value} added");
        return value;
      }
    }


    public bool TryAdd(TKey key, TValue value)
    {
      bool result = Dictionary.TryAdd(key, value);
      HasItems = true;
      if (result)
        ThrowAddEvent(value);
      return result;
    }

    public bool TryRemove(TKey key, out TValue value)
    {
      bool result = Dictionary.TryRemove(key, out value);
      if (result)
        ThrowRemoveEvent(value);
      return result;
    }
    public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
    {
      bool result = Dictionary.TryUpdate(key, newValue, comparisonValue);
      if (result)
        ThrowReplaceEvent(comparisonValue, newValue);
      return result;
    }

    private void ThrowAddOrUpdateEvents(TValue oldItem, TValue newItem, int oldCount, int newCount)
    {
      if (newCount!=Dictionary.Count)
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
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            newItem
            ));
        else
        {
          var action = new Action<TValue>(ThrowAddEvent);
          ApplicationDispatcher.Invoke(action, new object[] { newItem });
        }
      }
    }

    private void ThrowReplaceEvent(TValue oldItem, TValue newItem)
    {
      if (_CollectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
            newItem, oldItem
            ));
        else
        {
          var action = new Action<TValue, TValue>(ThrowReplaceEvent);
          ApplicationDispatcher.Invoke(action, new object[] { newItem });
        }
      }
    }

    private void ThrowRemoveEvent(TValue oldItem)
    {
      if (_CollectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
            oldItem
            ));
        else
        {
          var action = new Action<TValue>(ThrowRemoveEvent);
          ApplicationDispatcher.Invoke(action, new object[] { oldItem });
        }
      }
    }
    #endregion overriden methods

    #region ICollection<TValue> support

    public bool IsReadOnly => false;

    public int Count => Dictionary.Count;

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

    public void Clear()
    {
      ((ICollection<TValue>)Values).Clear();
    }

    public IEnumerator GetEnumerator()
    {
      return ((ICollection<TValue>)Values).GetEnumerator();
    }
    #endregion ICollection<TValue> support

    public class DispatchedDictionaryValues : ICollection<TValue>, INotifyCollectionChanged
    {
      public event NotifyCollectionChangedEventHandler CollectionChanged
      {
        add { Owner.CollectionChanged+=value; }
        remove { Owner.CollectionChanged-=value; }
      }

      public DispatchedDictionary<TKey, TValue> Owner { get; private set; }

      public int Count => Owner.Count;

      public bool IsReadOnly => true;

      public DispatchedDictionaryValues(DispatchedDictionary<TKey, TValue> owner)
      {
        Owner = owner;
      }

      public bool Contains(TValue item)
      {
        return Owner.Dictionary.Values.Contains(item);
      }

      public void CopyTo(TValue[] array, int arrayIndex)
      {
        Owner.Dictionary.Values.CopyTo(array, arrayIndex);
      }

      public IEnumerator<TValue> GetEnumerator()
      {
        return Owner.Dictionary.Values.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return Owner.Dictionary.Values.GetEnumerator();
      }

      public void Add(TValue item)
      {
        throw new NotImplementedException();
      }

      public void Clear()
      {
        throw new NotImplementedException();
      }

      public bool Remove(TValue item)
      {
        throw new NotImplementedException();
      }
    }

  }

}
