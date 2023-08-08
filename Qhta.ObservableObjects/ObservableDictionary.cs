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
    where TKey : notnull, IEquatable<TKey>
  {

    private readonly object _syncRoot = new object();

    /// <summary>
    /// Internal ImmutableDictionary
    /// </summary>
    protected ImmutableDictionary<TKey, TValue> _items = null!;

    /// <summary>
    /// Gets ImmutableList to notify that collection is changed
    /// </summary>
    /// <returns></returns>
    protected override ICollection GetNotifyObject()
    {
      return _items;
    }

    #region constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ObservableDictionary()
    {
      _items = ImmutableDictionary.Create<TKey, TValue>();
    }

    /// <summary>
    /// Constructor with initial collection
    /// </summary>
    /// <param name="items"></param>
    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
      _items = ImmutableDictionary.CreateRange(items);
    }
    #endregion

    /// <summary>
    /// Special comparer for TKey type.
    /// </summary>
    public IEqualityComparer<TKey>? KeyComparer
    {
      get => _keyComparer;
      set
      {
        if (_keyComparer != value)
        {
          _keyComparer = value;
          _items = ImmutableDictionary.CreateRange(_keyComparer, _items);
          NotifyPropertyChanged(nameof(KeyComparer));
          NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
      }
    }
    private IEqualityComparer<TKey>? _keyComparer;

    #region Dictionary<T, V> wrappers

    /// <summary>
    /// Specifies whether collection size is fixed.
    /// </summary>
    public bool IsFixedSize { get; set; }

    /// <summary>
    /// Specifies whether collection is read only.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Removes all the items.
    /// </summary>
    public void Clear()
    {
      lock (LockObject)
      {
        //Debug.WriteLine("Clear");
        foreach (var enumerator in enumerators)
        {
          enumerator.Reset();
        }
        _items = _items.Clear();
        wasReset = true;
        NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //NotifyPropertyChanged(nameof(Count));
      }
    }
    internal bool wasReset;

    private List<ObservableDictionaryEnumerator> enumerators = new List<ObservableDictionaryEnumerator>();

    /// <summary>
    /// Returns the count of items.
    /// </summary>
    public int Count
    {
      get
      {
        return _items.Count;
      }
    }

    /// <summary>
    /// Returns the keys collection.
    /// </summary>
    public IEnumerable<TKey> Keys
    {
      get
      {
        return _items.Keys;
      }
    }

    /// <summary>
    /// Returns the values collection.
    /// </summary>
    public IEnumerable<TValue> Values
    {
      get
      {
        return _items.Values;
      }
    }

    /// <summary>
    /// Indexed access to items.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue this[TKey key]
    {
      get
      {
        //Debug.WriteLine($"this[{key}]={_items[key]}");
        return _items[key];
      }
      set
      {
        _items = _items.SetItem(key, value);
        NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      //Debug.WriteLine($"IEnumerable<KeyValuePair<T, V>>.GetEnumerator");
      return this.GetEnumerator();
    }

    /// <summary>
    /// Gets enumerator adding it to enumerator list.
    /// </summary>
    /// <returns></returns>
    public ObservableDictionaryEnumerator GetEnumerator()
    {
      //Debug.WriteLine($"GetEnumerator");
      var enumerator = new ObservableDictionaryEnumerator(this);
      enumerators.Add(enumerator);
      return enumerator;
    }

    /// <summary>
    /// Adds a value with a specific key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
    {
      //Debug.WriteLine($"Add({key}, {value})" + $" {DateTime.Now.ToString(dateTimeFormat)}");
      try
      {
        lock (LockObject)
        {
          _items = _items.Add(key, value);
          int index = _items.Keys.ToImmutableSortedSet().IndexOf(key);
          NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }
      }
      catch (System.ArgumentException ex)
      {
        Debug.WriteLine($"{ex.GetType().Name} thrown in ObservableDictionary:\n {ex.Message}");
      }
    }

    /// <summary>
    /// Checks if the dictionary contains the specific key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(TKey key)
    {
      return _items.ContainsKey(key);
    }

    /// <summary>
    /// Checks if the dictionary contains the specific value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool ContainsValue(TValue value)
    {
      return _items.ContainsValue(value);
    }

    /// <summary>
    /// Removes an item specified by the key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
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
        _items = _items.Remove(key);
        NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        return true;
      }
    }

    /// <summary>
    /// Checks if the dictionary contains an item specified by the key and returns the value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
      return _items.TryGetValue(key, out value);
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    #endregion

    #region IEnumerable, ICollection, IDictionary implementation

    IEnumerator IEnumerable.GetEnumerator()
    {
      //Debug.WriteLine($"IEnumerable.GetEnumerator");
      return this.GetEnumerator();
    }

    /// <summary>
    /// Adds an object value with a specified object key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(object key, object value)
    {
      Add((TKey)key, (TValue)value);
    }

    /// <summary>
    /// Checks if the dictionary contains an object key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(object key)
    {
      return Contains((TKey)key);
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

    /// <summary>
    /// Removes the item with object key.
    /// </summary>
    /// <param name="key"></param>
    public void Remove(object key)
    {
      Remove((TKey)key);
    }

    ICollection IDictionary.Values
    {
      get
      {
        return (_items as IDictionary).Values;
      }
    }

    /// <summary>
    /// Access to object item
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object? this[object key]
    {
      get
      {
        return this[(TKey)key];
      }
      set
      {
        if (value is not null)
          this[(TKey)key] = (TValue)value;
      }
    }

    /// <summary>
    /// Copies items to the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
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

    /// <summary>
    /// Adds a key-value pair.
    /// </summary>
    /// <param name="item"></param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      this.Add(item.Key, item.Value);
    }

    /// <summary>
    /// Copies items to the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      (_items as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes a key-value pair.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      var result = this.Contains(item.Key);
      _items = _items.Remove(item.Key);
      NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      return result;
    }

    /// <summary>
    /// Checks if the dictionary contains a key-value pair.
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
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


    /// <summary>
    /// Enumerator class for observable dictionary.
    /// </summary>
    public class ObservableDictionaryEnumerator : IDictionaryEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
    {
      /// <summary>
      /// Initializing constructor.
      /// </summary>
      /// <param name="items"></param>
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

      /// <summary>
      /// Gets the current dictionary entry.
      /// </summary>
      public DictionaryEntry Entry => new DictionaryEntry(Current.Key, Current.Value);

      /// <summary>
      /// Gets the current entry key.
      /// </summary>
      public object Key => Current.Key;

      /// <summary>
      /// Gets the current entry value.
      /// </summary>
      public object? Value => Current.Value;

      /// <summary>
      /// Gets the current key-value pair.
      /// </summary>
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

      /// <summary>
      /// Moves current index to the next position.
      /// </summary>
      /// <returns></returns>
      public bool MoveNext()
      {
        if (currentIndex < _items.Count - 1)
        {
          currentIndex++;
          //Debug.WriteLine($"ObservableDictionaryEnumerator moved next to {currentIndex}");
          return currentIndex >= 0;
        }
        else
        {
          //Debug.WriteLine($"ObservableDictionaryEnumerator currentIndex={currentIndex}, itemsCount={_items.Count}");
          currentIndex = _items.Count - 1;
          //Debug.WriteLine($"ObservableDictionaryEnumerator cannot moved next to {currentIndex}");
          return false;
        }
      }

      /// <summary>
      /// Resets the current index.
      /// </summary>
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

      /// <summary>
      /// Dispose implementation method.
      /// </summary>
      /// <param name="disposing"></param>
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
