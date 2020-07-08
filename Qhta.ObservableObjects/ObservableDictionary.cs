using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Multithread version of Dictionary<typeparamref name="TKey"/>, <typeparamref name="TValue"/> with CollectionChanged notification.
  /// It should be used instead of ObservableCollection<typeparamref name="TValue"/> in MVVM architecture model when
  /// data source must be a dictionary.
  /// To bind it to CollectionView, <c>BindingOperator.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot)</c>
  /// must be invoked. It can be assured in XAML using CollectionViewBehavior class from Qhta.WPF.Utils assembly.
  /// Syntax is:
  /// <c>xmlns:utils="clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.Utils"</c>
  /// <c>utils:CollectionViewBehavior.EnableCollectionSynchronization="True"</c>
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class ObservableDictionary<TKey, TValue> : ObservableCollectionObject,
    IEnumerable,
    ICollection,
    //IList,// This interface must not be implemented due to error in PresentationFramework. The error occurs after invoking Clear() method,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IDictionary,
    IDictionary<TKey, TValue>,
        //ISerializable,
        //IDeserializationCallback,
    INotifyCollectionChanged, INotifyPropertyChanged
  {

    private readonly object _syncRoot = new object();

    private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();

    public IEqualityComparer<TKey> KeyComparer
    {
      get => _keyComparer;
      set
      {
        if (_keyComparer != value)
        {
          _keyComparer = value;
          _items = new Dictionary<TKey, TValue>(_items, _keyComparer);
          NotifyPropertyChanged(nameof(KeyComparer));
        }
      }
    }
    private IEqualityComparer<TKey> _keyComparer;

    #region Constructors

    public ObservableDictionary(Dispatcher dispatcher) : this(new KeyValuePair<TKey, TValue>[0], dispatcher)
    {
    }

    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items, Dispatcher dispatcher) : base(dispatcher)
    {
      _syncRoot = new object();
      var newItems = new Dictionary<TKey, TValue>();
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
        wasReset = true;
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
        //NotifyPropertyChanged(nameof(Count));
      }
    }
    internal bool wasReset;

    private List<ObservableDictionaryEnumerator> enumerators = new List<ObservableDictionaryEnumerator>();

    public int Count
    {
      get
      {
        return _items.Count;
      }
    }

    public IEnumerable<TKey> Keys
    {
      get
      {
        return _items.Keys;
      }
    }

    public IEnumerable<TValue> Values
    {
      get
      {
        return _items.Values;
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        //Debug.WriteLine($"this[{key}]={_items[key]}");
        return _items[key];
      }
      set
      {
        _items[key] = value;
        NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      }
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      //Debug.WriteLine($"IEnumerable<KeyValuePair<T, V>>.GetEnumerator");
      return this.GetEnumerator();
    }

    public ObservableDictionaryEnumerator GetEnumerator()
    {
      Debug.WriteLine($"GetEnumerator");
      var enumerator = new ObservableDictionaryEnumerator(this);
      enumerators.Add(enumerator);
      return enumerator;
    }

    public void Add(TKey key, TValue value)
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
      catch (System.ArgumentException)
      {
        //Debug.WriteLine($"{ex.GetType().Name} thrown in ObservableDictionary:\n {ex.Message}");
      }
    }

    public bool ContainsKey(TKey key)
    {
      return _items.ContainsKey(key);
    }

    public bool ContainsValue(TValue value)
    {
      return _items.ContainsValue(value);
    }

    public bool Remove(TKey key)
    {
      lock (LockObject)
      {
        var list = _items.Keys.ToImmutableSortedSet();
        //Debug.WriteLine(String.Join(", ", list));
        int index = list.IndexOf(key);
        if (index < 0)
          return false;
        TValue value = _items[key];
        _items.Remove(key);
        NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);
        return true;
      }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return _items.TryGetValue(key, out value);
    }

    #endregion

    #region IEnumerable, ICollection, IDictionary implementation

    IEnumerator IEnumerable.GetEnumerator()
    {
      //Debug.WriteLine($"IEnumerable.GetEnumerator");
      return this.GetEnumerator();
    }

    public void Add(object key, object value)
    {
      Add((TKey)key, (TValue)value);
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
        return this[(TKey)key];
      }
      set
      {
        this[(TKey)key] = (TValue)value;
      }
    }

    public void CopyTo(Array array, int index)
    {
      (_items as IDictionary).CopyTo(array, index);
    }
    #endregion

    #region IDictionary<T, V>

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get
      {
        return (_items as IDictionary<TKey, TValue>).Keys;
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get
      {
        return (_items as IDictionary<TKey, TValue>).Values;
      }
    }
    #endregion

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      this.Add(item.Key, item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      (_items as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      var result = (_items as IDictionary<TKey, TValue>).Remove(item);
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
      return result;
    }

    public bool Contains(KeyValuePair<TKey, TValue> pair)
    {
      return _items.ContainsKey(pair.Key);
    }

    //#region IList explicit implementation

    //object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    //int IList.Add(object value)
    //{
    //  throw new NotImplementedException();
    //}

    //int IList.IndexOf(object value)
    //{
    //  throw new NotImplementedException();
    //}

    //void IList.Insert(int index, object value)
    //{
    //  throw new NotImplementedException();
    //}

    //void IList.RemoveAt(int index)
    //{
    //  throw new NotImplementedException();
    //}
    //#endregion


    public class ObservableDictionaryEnumerator : IDictionaryEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
    {
      public ObservableDictionaryEnumerator(ObservableDictionary<TKey, TValue> items)
      {
        _items = items;
        if (items.wasReset)
        {
          currentIndex = -2;
          items.wasReset = false;
        }
      }

      private ObservableDictionary<TKey, TValue> _items;
      private int currentIndex = -1;

      public DictionaryEntry Entry => throw new NotImplementedException();

      public object Key => throw new NotImplementedException();

      public object Value => throw new NotImplementedException();

      public KeyValuePair<TKey, TValue> Current
      {
        get
        {
          //Debug.WriteLine($"Current[{currentIndex}]");
          if (currentIndex >= 0 && currentIndex < _items.Count)
            return _items._items.ToImmutableArray()[currentIndex];
          return default(KeyValuePair<TKey, TValue>);
        }
      }

      object IEnumerator.Current => this.Current;

      public bool MoveNext()
      {
        if (currentIndex < _items.Count-1)
        {
          currentIndex++;
          //Debug.WriteLine($"ObservableDictionaryEnumerator moved next to {currentIndex}");
          return currentIndex>=0;
        }
        else
        {
          //Debug.WriteLine($"ObservableDictionaryEnumerator currentIndex={currentIndex}, itemsCount={_items.Count}");
          currentIndex = _items.Count - 1;
          //Debug.WriteLine($"ObservableDictionaryEnumerator cannot moved next to {currentIndex}");
          return false;
        }
      }

      public void Reset()
      {
        //Debug.WriteLine("Reset");
        currentIndex = -2;
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
