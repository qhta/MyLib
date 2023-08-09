using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Multithread version of Dictionary<typeparamref name="TKey"/>, <typeparamref name="TValue"/> with CollectionChanged notification.
  /// It should be used instead of ObservableCollection<typeparamref name="TValue"/> in MVVM architecture model when
  /// data source must be a dictionary.
  /// To bind it to CollectionView, 
  /// <c>BindingOperator.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot)</c> must be invoked. 
  /// Instead, it can be assured in XAML using CollectionViewBehavior class from Qhta.WPF.Behaviors assembly.
  /// Syntax is:
  /// <c>xmlns:bhv="clr-namespace:Qhta.WPF.Behaviors;assembly=Qhta.WPF.Behaviors"</c>
  /// <c>bhv:CollectionViewBehavior.EnableCollectionSynchronization="True"</c>
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class ObservableDictionary<TKey, TValue> : ObservableCollectionObject,
    IEnumerable,
    ICollection,
    //IList, This interface must not be implemented
    IEnumerable<KeyValuePair<TKey, TValue>>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IDictionary,
    IDictionary<TKey, TValue>,
    INotifyCollectionChanged, INotifyPropertyChanged
    where TKey : notnull, IEquatable<TKey>
  {

    private readonly object _syncRoot = new object();

    /// <summary>
    /// Internal ImmutableDictionary
    /// </summary>
    protected ImmutableDictionary<TKey, TValue> _items = null!;

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
        _items = _items.Clear();
        NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }

    /// <summary>
    /// Returns the count of items.
    /// </summary>
    public int Count
    {
      get
      {
        var count = _items.Count;
        if (count == 100000)
          Debug.Assert(true);
        Debug.WriteLine($"GetCount({count})" + $" {DateTime.Now.TimeOfDay}");
        return count;
      }
    }

    /// <summary>
    /// Returns the count of items.
    /// </summary>
    internal int _Count
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
      var enumerator = _items.GetEnumerator();
      return enumerator;
    }

    /// <summary>
    /// Gets enumerator adding it to enumerator list.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TValue> GetEnumerator()
    {
      //Debug.WriteLine($"GetEnumerator");
      //var enumerator = new ObservableDictionaryEnumerator(this);
      var enumerator = _items.Values.GetEnumerator();
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
      lock (LockObject)
      {
        int index = _items.Count;
        _items = _items.Add(key, value);
        NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        NotifyPropertyChanged(nameof(Count));
      }
    }


    /// <summary>
    ///   Adds the elements of the specified collection to the end of the ImmutableList.
    /// </summary>
    /// <param name="collection">
    ///   The collection whose elements should be added to the end of the list.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   collection is null.
    /// </exception>
    public void AddRange(IEnumerable<(TKey, TValue)> collection)
    {
      //Debug.WriteLine($"AddRange({collection.Count()})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        _items = _items.AddRange(collection.Select(item => new KeyValuePair<TKey, TValue>(item.Item1, item.Item2)).ToArray());
        NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        NotifyPropertyChanged(nameof(Count));
      }
    }


    /// <summary>
    ///   Adds the elements of the specified collection to the end of the ImmutableList.
    /// </summary>
    /// <param name="collection">
    ///   The collection whose elements should be added to the end of the list.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   collection is null.
    /// </exception>
    public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
      //Debug.WriteLine($"AddRange({collection.Count()})" + $" {DateTime.Now.TimeOfDay}");
      lock (LockObject)
      {
        _items = _items.AddRange(collection);
        NotifyCollectionChanged(_items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        NotifyPropertyChanged(nameof(Count));
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
        NotifyPropertyChanged(nameof(Count));
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
      return ((IDictionary)_items).GetEnumerator();
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
      NotifyPropertyChanged(nameof(Count));
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

  }
}
