using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  public class ObservableDictionary<T, V> : ObservableCollectionObject,
    IEnumerable,
    ICollection,
    IList,
    IEnumerable<KeyValuePair<T, V>>,
    ICollection<KeyValuePair<T, V>>,
    IDictionary,
    IDictionary<T, V>,
    INotifyCollectionChanged, INotifyPropertyChanged
  {

    private readonly object _syncRoot = new object();

    private Dictionary<T, V> _items = new Dictionary<T, V>();

    public IEqualityComparer<T> KeyComparer
    {
      get => _keyComparer;
      set
      {
        if (_keyComparer != value)
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

    #region Dictionary<T, V> wrappers

    public bool IsFixedSize
    {
      get
      {
        return false;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public void Clear()
    {
      lock (LockObject)
      {
        //Debug.WriteLine("Clear");
        foreach (var enumerator in enumerators)
        {
          enumerator.Reset();
        }
        _items.Clear();
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    private List<ObservableDictionaryEnumerator> enumerators = new List<ObservableDictionaryEnumerator>();

    public int Count
    {
      get
      {
        return _items.Count;
      }
    }

    public IEnumerable<T> Keys
    {
      get
      {
        return _items.Keys;
      }
    }

    public IEnumerable<V> Values
    {
      get
      {
        return _items.Values;
      }
    }

    public V this[T key]
    {
      get
      {
        Debug.WriteLine($"this[{key}]={_items[key]}");
        return _items[key];
      }
      set
      {
        _items[key] = value;
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    IEnumerator<KeyValuePair<T, V>> IEnumerable<KeyValuePair<T, V>>.GetEnumerator()
    {
      Debug.WriteLine($"IEnumerable<KeyValuePair<T, V>>.GetEnumerator");
      return this.GetEnumerator();
    }

    public ObservableDictionaryEnumerator GetEnumerator()
    {
      Debug.WriteLine($"GetEnumerator");
      var enumerator = new ObservableDictionaryEnumerator(this);
      enumerators.Add(enumerator);
      return enumerator;
    }

    public void Add(T key, V value)
    {
      //Debug.WriteLine($"Add({key}, {value})" + $" {DateTime.Now.ToString(dateTimeFormat)}");
      try
      {
        lock (LockObject)
        {
          _items.Add(key, value);
          int index = _items.Keys.ToImmutableSortedSet().IndexOf(key);
          NotifyCollectionChanged(NotifyCollectionChangedAction.Add, value, index);
        }
      }
      catch (System.ArgumentException ex)
      {
        //Debug.WriteLine($"{ex.GetType().Name} thrown in ObservableDictionary:\n {ex.Message}");
      }
    }

    public bool ContainsKey(T key)
    {
      return _items.ContainsKey(key);
    }

    public bool ContainsValue(V value)
    {
      return _items.ContainsValue(value);
    }

    public bool Remove(T key)
    {
      lock (LockObject)
      {
        var list = _items.Keys.ToImmutableSortedSet();
        //Debug.WriteLine(String.Join(", ", list));
        int index = list.IndexOf(key);
        if (index < 0)
          return false;
        V value = _items[key];
        _items.Remove(key);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);
        return true;
      }
    }

    public bool TryGetValue(T key, out V value)
    {
      return _items.TryGetValue(key, out value);
    }

    #endregion

    #region IEnumerable, ICollection, IDictionary implementation

    IEnumerator IEnumerable.GetEnumerator()
    {
      Debug.WriteLine($"IEnumerable.GetEnumerator");
      return this.GetEnumerator();
    }

    public void Add(object key, object value)
    {
      Add((T)key, (V)value);
    }

    public bool Contains(object key)
    {
      return (_items as IDictionary).Contains(key);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return this.GetEnumerator();
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
    #endregion

    #region IDictionary<T, V>

    ICollection<T> IDictionary<T, V>.Keys
    {
      get
      {
        return (_items as IDictionary<T, V>).Keys;
      }
    }

    ICollection<V> IDictionary<T, V>.Values
    {
      get
      {
        return (_items as IDictionary<T, V>).Values;
      }
    }
    #endregion

    public void Add(KeyValuePair<T, V> item)
    {
      this.Add(item.Key, item.Value);
    }

    public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
    {
      (_items as IDictionary<T, V>).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<T, V> item)
    {
      var result = (_items as IDictionary<T, V>).Remove(item);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return result;
    }

    public bool Contains(KeyValuePair<T, V> pair)
    {
      return _items.ContainsKey(pair.Key);
    }

    #region IList explicit implementation

    object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    int IList.Add(object value)
    {
      throw new NotImplementedException();
    }

    int IList.IndexOf(object value)
    {
      throw new NotImplementedException();
    }

    void IList.Insert(int index, object value)
    {
      throw new NotImplementedException();
    }

    void IList.RemoveAt(int index)
    {
      throw new NotImplementedException();
    }
    #endregion


    public class ObservableDictionaryEnumerator : IDictionaryEnumerator, IEnumerator<KeyValuePair<T, V>>
    {
      public ObservableDictionaryEnumerator(ObservableDictionary<T, V> items)
      {
        _items = items;
      }
      ObservableDictionary<T, V> _items;
      int currentIndex = -1;

      public DictionaryEntry Entry => throw new NotImplementedException();

      public object Key => throw new NotImplementedException();

      public object Value => throw new NotImplementedException();

      public object Current
      {
        get
        {
          Debug.WriteLine($"Current[{currentIndex}]");
          if (currentIndex >= 0 && currentIndex < _items.Count)
            return _items.ToImmutableArray()[currentIndex];
          return null;
        }
      }

      KeyValuePair<T, V> IEnumerator<KeyValuePair<T, V>>.Current => throw new NotImplementedException();

      object IEnumerator.Current => this.Current;

      public bool MoveNext()
      {
        if (currentIndex < _items.Count)
        {
          currentIndex++;
          Debug.WriteLine($"ObservableDictionaryEnumerator moved next to {currentIndex}");
          return true;
        }
        else
        {
          currentIndex = _items.Count - 1;
          Debug.WriteLine($"ObservableDictionaryEnumerator cannot moved next to {currentIndex}");
          return false;
        }
      }

      public void Reset()
      {
        Debug.WriteLine("Reset");
        currentIndex = -1;
      }

      bool IEnumerator.MoveNext()
      {
        return this.MoveNext();
      }

      void IEnumerator.Reset()
      {
        this.Reset();
      }

      #region IDisposable Support
      private bool disposedValue = false; // To detect redundant calls

      protected virtual void Dispose(bool disposing)
      {
        if (!disposedValue)
        {
          if (disposing)
          {
            //_items.enumerators.Remove(this);
          }

          // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
          // TODO: set large fields to null.

          disposedValue = true;
        }
      }

      // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
      // ~ObservableDictionaryEnumerator() {
      //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      //   Dispose(false);
      // }

      // This code added to correctly implement the disposable pattern.
      void IDisposable.Dispose()
      {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
      }
      #endregion

    }
  }
}
