using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  public class ObservableDictionary<T, V> : ObservableCollectionObject, IImmutableDictionary<T, V>, 
    IReadOnlyDictionary<T, V>, IReadOnlyCollection<KeyValuePair<T, V>>, 
    IDictionary<T, V>, ICollection<KeyValuePair<T, V>>, 
    IEnumerable<KeyValuePair<T, V>>, IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
  {

    private readonly object _syncRoot = new object();

    private Dictionary<T, V> _items = new Dictionary<T, V>();

    public IEqualityComparer<T> KeyComparer
    {
      get => _keyComparer;
      set
      {
        if (_keyComparer!=value)
        {
          _keyComparer = value;
          _items = new Dictionary<T, V>(_items, _keyComparer);
          NotifyPropertyChanged(nameof(KeyComparer));
        }
      }
    }
    private IEqualityComparer<T> _keyComparer;

    #region Constructors

    public ObservableDictionary(Dispatcher dispatcher) : this(new KeyValuePair<T, V>[0], dispatcher)
    {
    }

    public ObservableDictionary(IEnumerable<KeyValuePair<T, V>> items, Dispatcher dispatcher) : base(dispatcher)
    {
      _syncRoot = new object();
      var newItems = new Dictionary<T, V>();
      foreach (var item in items)
        newItems.Add(item.Key, item.Value);
      _items = newItems;
    }

    #endregion Constructors
    /*
        #region Thread-Safe Methods

        #region General

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryOperation(Func<IImmutableDictionary<T, V>, IImmutableDictionary<T, V>> operation)
        {
          return TryOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DoOperation(Func<IImmutableDictionary<T, V>, IImmutableDictionary<T, V>> operation)
        {
          return DoOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #region Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<IImmutableDictionary<T, V>, IImmutableDictionary<T, V>> operation, NotifyCollectionChangedEventArgs args)
        {
          try
          {
            if (Helper.TryLock())
            {
              var oldList = _items;
              var newItems = operation(oldList);

              if (newItems == null)
              {
                // user returned null which means he cancelled operation
                return false;
              }

              _items = newItems;

              if (args != null)
                NotifyCollectionChanged(args);
              return true;
            }
          }
          finally
          {
            Helper.Unlock();
          }

          return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<IImmutableDictionary<T, V>, KeyValuePair<Dictionary<T, V>, NotifyCollectionChangedEventArgs>> operation)
        {
          try
          {
            if (Helper.TryLock())
            {
              var oldList = _items;
              var kvp = operation(oldList);
              var newItems = kvp.Key;
              var args = kvp.Value;

              if (newItems == null)
              {
                // user returned null which means he cancelled operation
                return false;
              }

              _items = newItems;

              if (args != null)
                NotifyCollectionChanged(args);
              return true;
            }
          }
          finally
          {
            Helper.Unlock();
          }

          return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<IImmutableDictionary<T, V>, Dictionary<T, V>> operation, NotifyCollectionChangedEventArgs args)
        {
          bool result;

          try
          {
            Helper.Lock();
            var oldItems = _items;
            var newItems = operation(_items);

            if (newItems == null)
            {
              // user returned null which means he cancelled operation
              return false;
            }

            result = (_items = newItems) != oldItems;

            if (args != null)
              NotifyCollectionChanged(args);
          }
          finally
          {
            Helper.Unlock();
          }

          return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<IImmutableDictionary<T, V>, KeyValuePair<IImmutableDictionary<T, V>, NotifyCollectionChangedEventArgs>> operation)
        {
          bool result;

          try
          {
            Helper.Lock();
            var oldItems = _items;
            var kvp = operation(_items);
            var newItems = kvp.Key;
            var args = kvp.Value;

            if (newItems == null)
            {
              // user returned null which means he cancelled operation
              return false;
            }

            result = (_items = newItems) != oldItems;

            if (args != null)
              NotifyCollectionChanged(args);
          }
          finally
          {
            Helper.Unlock();
          }

          return result;
        }

        #endregion Helpers

        #endregion General

        #region Specific

        public bool DoAdd(Func<IImmutableDictionary<T, V>, KeyValuePair<T, V>> valueProvider)
        {
          return DoOperation
            (
            currentItems =>
            {
              var kvp = valueProvider(currentItems);
              var newItems = this.Add(kvp.Key, kvp.Value);
              return new KeyValuePair<IImmutableDictionary<T, V>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp));
            }
            );
        }

        public bool DoAddRange(Func<IImmutableDictionary<T, V>, IEnumerable<KeyValuePair<T, V>>> valueProvider)
        {
          return DoOperation
            (
            currentItems =>
              currentItems.AddRange(valueProvider(currentItems))
            );
        }

        public bool DoClear()
        {
          return DoOperation
            (
            currentItems =>
              currentItems.Clear()
            );
        }

        public bool DoRemove(Func<IImmutableDictionary<T, V>, KeyValuePair<T, V>> valueProvider)
        {
          return DoOperation
            (
            currentItems =>
            {
              var newKVP = valueProvider(currentItems);
              var oldKVP = new KeyValuePair<T, V>(newKVP.Key, currentItems[newKVP.Key]);
              var newItems = currentItems.Remove(newKVP.Key);
              return new KeyValuePair<ImmutableDictionary<T, V>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldKVP));
            }
            );
        }

        public bool DoRemoveRange(Func<IImmutableDictionary<T, V>, IEnumerable<T>> valueProvider)
        {
          return DoOperation
            (
            currentItems =>
              currentItems.RemoveRange(valueProvider(currentItems))
            );
        }

        public bool DoSetItem(Func<IImmutableDictionary<T, V>, KeyValuePair<T, V>> valueProvider)
        {
          return DoOperation
            (
            currentItems =>
            {
              var newKVP = valueProvider(currentItems);
              var oldKVP = new KeyValuePair<T, V>(newKVP.Key, currentItems[newKVP.Key]);
              var newItems = currentItems.SetItem(newKVP.Key, newKVP.Value);
              return new KeyValuePair<ImmutableDictionary<T, V>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldKVP, newKVP));
            }
            );
        }

        public bool DoSetItems(Func<IImmutableDictionary<T, V>, IEnumerable<KeyValuePair<T, V>>> valueProvider)
        {
          return DoOperation
            (
            currentItems =>
              currentItems.SetItems(valueProvider(currentItems))
            );
        }

        public bool TryAdd(Func<IImmutableDictionary<T, V>, KeyValuePair<T, V>> valueProvider)
        {
          return TryOperation
            (
            currentItems =>
            {
              var kvp = valueProvider(currentItems);
              var newItems = _items.Add(kvp.Key, kvp.Value);
              return new KeyValuePair<ImmutableDictionary<T, V>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp));
            }
            );
        }

        public bool TryAddRange(Func<IImmutableDictionary<T, V>, IEnumerable<KeyValuePair<T, V>>> valueProvider)
        {
          return TryOperation
            (
            currentItems =>
              currentItems.AddRange(valueProvider(currentItems))
            );
        }

        public bool TryClear()
        {
          return TryOperation
            (
            currentItems =>
              currentItems.Clear()
            );
        }

        public bool TryRemove(Func<IImmutableDictionary<T, V>, KeyValuePair<T, V>> valueProvider)
        {
          return TryOperation
            (
            currentItems =>
            {
              var newKVP = valueProvider(currentItems);
              var oldKVP = new KeyValuePair<T, V>(newKVP.Key, currentItems[newKVP.Key]);
              var newItems = currentItems.Remove(newKVP.Key);
              return new KeyValuePair<ImmutableDictionary<T, V>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldKVP));
            }
            );
        }

        public bool TryRemoveRange(Func<IImmutableDictionary<T, V>, IEnumerable<T>> valueProvider)
        {
          return TryOperation
            (
            currentItems =>
              currentItems.RemoveRange(valueProvider(currentItems))
            );
        }

        public bool TrySetItem(Func<IImmutableDictionary<T, V>, KeyValuePair<T, V>> valueProvider)
        {
          return TryOperation
            (
            currentItems =>
            {
              var newKVP = valueProvider(currentItems);
              var oldKVP = new KeyValuePair<T, V>(newKVP.Key, currentItems[newKVP.Key]);
              var newItems = currentItems.SetItem(newKVP.Key, newKVP.Value);
              return new KeyValuePair<ImmutableDictionary<T, V>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldKVP, newKVP));
            }
            );
        }

        public bool TrySetItems(Func<IImmutableDictionary<T, V>, IEnumerable<KeyValuePair<T, V>>> valueProvider)
        {
          return TryOperation
            (
            currentItems =>
              currentItems.SetItems(valueProvider(currentItems))
            );
        }

        #endregion Specific

        public ImmutableDictionary<T, V> ToImmutableDictionary()
        {
          return _items;
        }
*/
        #region IEnumerable<KeyValuePair<T, V>>

        public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
        {
          return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
          return GetEnumerator();
        }

        #endregion IEnumerable<T>
    /*
        #endregion Thread-Safe Methods

      */
    #region Non Thead-Safe Methods

    #region IImmutableDictionary<T, V>

    public IImmutableDictionary<T, V> Add(T key, V value)
    {
      try
      {
        var newItems = new Dictionary<T, V>(this);
        newItems.Add(key, value);
        var index = newItems.Keys.ToImmutableList().IndexOf(key);
        _items = newItems;
        NotifyCollectionChanged(NotifyCollectionChangedAction.Add, value, index);
      }
      catch (System.ArgumentException ex)
      {
        Debug.WriteLine("Exception thrown: 'System.ArgumentException' in System.Collections.Immutable.dll");
        Debug.WriteLine(ex.Message);
      }
      return this;
    }

    public IImmutableDictionary<T, V> AddRange(IEnumerable<KeyValuePair<T, V>> pairs)
    {
      var newItems = new Dictionary<T, V>(this);
      foreach (var pair in pairs)
        newItems.Add(pair.Key, pair.Value);
      _items = newItems;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);

      return this;
    }

    public IImmutableDictionary<T, V> Clear()
    {
      var newItems = new Dictionary<T, V>(this);
      newItems.Clear();
      _items = newItems;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return this;
    }

    public bool Contains(KeyValuePair<T, V> pair)
    {
      return _items.ContainsKey(pair.Key);
    }

    public IImmutableDictionary<T, V> Remove(T key)
    {
      var newItems = new Dictionary<T, V>(this);
      newItems.Remove(key);
      _items = newItems;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return this;
    }

    public IImmutableDictionary<T, V> RemoveRange(IEnumerable<T> keys)
    {
      var newItems = new Dictionary<T, V>(this);
      foreach (var key in keys)
        newItems.Remove(key);
      _items = newItems;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return this;
    }

    public IImmutableDictionary<T, V> SetItem(T key, V value)
    {
      var newItems = new Dictionary<T, V>(this);
      newItems[key] = value;
      _items = newItems;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return this;
    }

    public IImmutableDictionary<T, V> SetItems(IEnumerable<KeyValuePair<T, V>> pairs)
    {
      var newItems = new Dictionary<T, V>(this);
      foreach (var pair in pairs)
        newItems[pair.Key]=pair.Value;
      _items = newItems;
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return this;
    }

    public bool TryGetKey(T equalKey, out T actualKey)
    {
      actualKey = equalKey;
      if (KeyComparer != null)
      {
        var newKeys = new List<T>(_items.Keys);
        foreach (var item in newKeys)
        {
          if (KeyComparer.Equals(item, equalKey))
          {
            actualKey = item;
            return true;
          }
        }
        return false;
      }
      else
      {
        var newItems = new Dictionary<T, V>(this);
        return newItems.ContainsKey(equalKey);
      }
    }

    public bool ContainsKey(T key)
    {
      return _items.ContainsKey(key);
    }

    public IEnumerable<T> Keys
    {
      get
      {
        return _items.Keys;
      }
    }

    public bool TryGetValue(T key, out V value)
    {
      return _items.TryGetValue(key, out value);
    }

    public IEnumerable<V> Values
    {
      get
      {
        return _items.Values;
      }
    }

    public int Count
    {
      get
      {
        return _items.Count;
      }
    }

    #endregion IImmutableDictionary<T, V>

    #region IDictionary<T, V>

    void IDictionary<T, V>.Add(T key, V value)
    {
      Add(key, value);
    }

    ICollection<T> IDictionary<T, V>.Keys
    {
      get
      {
        return (_items as IDictionary<T, V>).Keys;
      }
    }

    bool IDictionary<T, V>.Remove(T key)
    {
      var newItems = new Dictionary<T, V>(_items);
      if (newItems.ContainsKey(key))
      {
        newItems.Remove(key);
        _items = newItems;
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
        return true;
      }
      return false;
    }

    ICollection<V> IDictionary<T, V>.Values
    {
      get
      {
        return (_items as IDictionary<T, V>).Values;
      }
    }

    public V this[T key]
    {
      get
      {
        return _items[key];
      }
      set
      {
        _items[key] = value;
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    public void Add(KeyValuePair<T, V> item)
    {
      (_items as IDictionary<T, V>).Add(item);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }

    void ICollection<KeyValuePair<T, V>>.Clear()
    {
      Clear();
    }

    public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
    {
      (_items as IDictionary<T, V>).CopyTo(array, arrayIndex);
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool Remove(KeyValuePair<T, V> item)
    {
      var result = (_items as IDictionary<T, V>).Remove(item);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return result;
    }

    #endregion IDictionary<T, V>

    #region IDictionary

    public void Add(object key, object value)
    {
      Add((T)key, (V)value);
    }

    void IDictionary.Clear()
    {
      Clear();
    }

    public bool Contains(object key)
    {
      return (_items as IDictionary).Contains(key);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (_items as IDictionary).GetEnumerator();
    }

    public bool IsFixedSize
    {
      get
      {
        return false;
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        return (_items as IDictionary).Keys;
      }
    }

    public void Remove(object key)
    {
      (_items as IDictionary).Remove(key);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }

    ICollection IDictionary.Values
    {
      get
      {
        return (_items as IDictionary).Values;
      }
    }

    public object this[object key]
    {
      get
      {
        return this[(T)key];
      }
      set
      {
        this[(T)key] = (V)value;
      }
    }

    public void CopyTo(Array array, int index)
    {
      (_items as IDictionary).CopyTo(array, index);
    }

    public bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public object SyncRoot
    {
      get
      {
        return _syncRoot;
      }
    }

    #endregion IDictionary

    #endregion Non Thead-Safe Methods
  }
}
