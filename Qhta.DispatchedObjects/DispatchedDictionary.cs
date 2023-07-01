using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Threading;

namespace Qhta.DispatchedObjects
{
  /// <summary>
  /// A dispatched version of <see cref="ConcurrentDictionary{TKey, TValue}"/>. 
  /// It implements <see cref="INotifyCollectionChanged"/> event (as <see cref="DispatchedCollection{TValue}"/> does).
  /// Is is based on <see cref="DispatchedObject"/> to notify on changes and to invoke actions.
  /// Internally it uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> to support multithreading applications
  /// and <see cref="DispatchedDictionaryValues"/> to support dispatched observable collection.
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class DispatchedDictionary<TKey, TValue> : DispatchedObject, IDisposable,
    IEnumerable<TValue>,
    ICollection<TValue>,
    ICollection,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
    IDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>,
    INotifyCollectionChanged
  {

    /// <summary>
    /// Internal concurrent dictionary instance;
    /// </summary>
    protected ConcurrentDictionary<TKey, TValue> Dictionary;

    /// <summary>
    /// External collection of values. Modified internally.
    /// </summary>
    public DispatchedDictionaryValues Values => _Values;
    /// <summary>
    /// Internal collection of values.
    /// It can be accessed in desendant classes.
    /// </summary>
    protected DispatchedDictionaryValues _Values;

    #region constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DispatchedDictionary()
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>();
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// Constructor which enables to set a <see cref="DispatchedObject.Name"/> property.
    /// </summary>
    /// <param name="debugName"></param>
    public DispatchedDictionary(string debugName)
    {
      Name = debugName;
      Dictionary = new ConcurrentDictionary<TKey, TValue>();
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// Initializing constructor based on a collection of values.
    /// </summary>
    /// <param name="collection"></param>
    public DispatchedDictionary(IEnumerable<TValue> collection)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>();
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// Initializing constructor based on a collection of key-value pairs.
    /// </summary>
    /// <param name="collection"></param>
    public DispatchedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(collection);
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// Constructor that enables to setup a comparer in the internal dictionary.
    /// </summary>
    /// <param name="comparer"></param>
    public DispatchedDictionary(IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(comparer);
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// Constructor that enables to setup a concurrency level and capacity in internal dictionary.
    /// </summary>
    /// <param name="concurrencyLevel"></param>
    /// <param name="capacity"></param>
    public DispatchedDictionary(int concurrencyLevel, int capacity)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity);
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// Initializing constructor based on a collection of key-value pairs.
    /// It enables to setup a comparer in the internal dictionary.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="comparer"></param>
    public DispatchedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// Initializing constructor based on a collection of key-value pairs.
    /// It enables to setup a concurrency level and comparer in the internal dictionary.
    /// </summary>
    /// <param name="concurrencyLevel"></param>
    /// <param name="collection"></param>
    /// <param name="comparer"></param>
    public DispatchedDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, collection, comparer);
      _Values = new DispatchedDictionaryValues(this);
    }

    /// <summary>
    /// <summary>
    /// Constructor that enables to setup a concurrency level, capacity in internal dictionary and comparer in the internal dictionary.
    /// </summary>
    /// </summary>
    /// <param name="concurrencyLevel"></param>
    /// <param name="capacity"></param>
    /// <param name="comparer"></param>
    public DispatchedDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer);
      _Values = new DispatchedDictionaryValues(this);
    }

    #endregion constructors

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    /// <summary>
    /// Internal Dispose implementation.
    /// </summary>
    /// <param name="disposing"></param>
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

    /// <summary>
    ///  External method to correctly implement the disposable pattern.
    /// </summary>
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion

    #region new properties
    /// <summary>
    /// Determines if the dictionary has items. Set internally.
    /// </summary>
    public bool HasItems
    {
      get { return _HasItems; }
      protected set
      {
        if (_HasItems != value)
        {
          _HasItems = value;
          NotifyPropertyChanged(nameof(HasItems));
        }
      }
    }
    private bool _HasItems;

    private readonly object _lock = new object();
    #endregion

    #region notification events

    /// <summary>
    /// Implementation of <see cref="INotifyCollectionChanged"/> interface.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
      add
      {
        _CollectionChanged += value;
      }
      remove
      {
        _CollectionChanged -= value;
      }
    }
    /// <summary>
    /// Internal <see cref="NotifyCollectionChangedEventHandler"/> event.
    /// It can be checked in descendent classes.
    /// </summary>
    protected event NotifyCollectionChangedEventHandler? _CollectionChanged;

    #endregion

    #region overriden ConcurrentDictionary methods

    /// <summary>
    /// Overrides <see cref="ConcurrentDictionary{TKey, TValue}.AddOrUpdate(TKey, Func{TKey, TValue}, Func{TKey, TValue, TValue})"/> method.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="addValueFactory"></param>
    /// <param name="updateValueFactory"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Overrides <see cref="ConcurrentDictionary{TKey, TValue}.AddOrUpdate(TKey, TValue, Func{TKey, TValue, TValue})"/> method.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="addValue"></param>
    /// <param name="updateValueFactory"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Overrides <see cref="ConcurrentDictionary{TKey, TValue}.GetOrAdd(TKey, Func{TKey, TValue})"/> method.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="valueFactory"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Overrides <see cref="ConcurrentDictionary{TKey, TValue}.GetOrAdd(TKey, TValue)"/> method.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Overrides <see cref="ConcurrentDictionary{TKey, TValue}.TryAdd(TKey, TValue)"/> method.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryAdd(TKey key, TValue value)
    {
      bool result = Dictionary.TryAdd(key, value);
      HasItems = true;
      if (result)
        ThrowAddEvent(value);
      return result;
    }

    /// <summary>
    /// Overrides <see cref="ConcurrentDictionary{TKey, TValue}.TryRemove(TKey, out TValue)"/> method.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryRemove(TKey key, out TValue value)
    {
      bool result = Dictionary.TryRemove(key, out value);
      if (result)
        ThrowRemoveEvent(value);
      return result;
    }

    /// <summary>
    /// Overrides <see cref="ConcurrentDictionary{TKey, TValue}.TryUpdate(TKey, TValue, TValue)"/> nethod.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newValue"></param>
    /// <param name="comparisonValue"></param>
    /// <returns></returns>
    public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
    {
      bool result = Dictionary.TryUpdate(key, newValue, comparisonValue);
      if (result)
        ThrowReplaceEvent(comparisonValue, newValue);
      return result;
    }

    private void ThrowAddOrUpdateEvents(TValue oldItem, TValue newItem, int oldCount, int newCount)
    {
      if (newCount != Dictionary.Count)
        ThrowAddEvent(newItem);
      else
        ThrowReplaceEvent(oldItem, newItem);
    }

    private void ThrowGetOrAddEvents(TValue oldItem, TValue newItem)
    {
      if (object.Equals(newItem,oldItem))
        ThrowAddEvent(newItem);
    }

    private void ThrowAddEvent(TValue newItem)
    {
      if (_CollectionChanged != null)
      {
        var dispatcher = DispatcherBridge;
        if (dispatcher != null)
        {
          var action = new Action<TValue>(ThrowAddEvent);
          dispatcher.Invoke(() =>
          {
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            newItem
            ));
          });
        }
        else
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            newItem
            ));
      }
    }

    private void ThrowReplaceEvent(TValue oldItem, TValue newItem)
    {
      if (_CollectionChanged != null)
      {
        var dispatcher = DispatcherBridge;
        if (dispatcher != null)
        {
          var action = new Action<TValue, TValue>(ThrowReplaceEvent);
          dispatcher.Invoke(() =>
          {
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
            newItem, oldItem
            ));
          });
        }

        else
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
            newItem, oldItem
            ));
      }
    }

    private void ThrowRemoveEvent(TValue oldItem)
    {
      if (_CollectionChanged != null)
      {
        var dispatcher = DispatcherBridge;
        if (dispatcher != null)
        {
          var action = new Action<TValue>(ThrowRemoveEvent);
          dispatcher.Invoke(() =>
          {
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
            oldItem
            ));
          });
        }
        else
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
            oldItem
            ));
      }
    }
    #endregion overriden methods

    #region ICollection<TValue> support

    /// <summary>
    /// A collection is not read only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Returns the number of key-value pairs.
    /// </summary>
    public int Count => Dictionary.Count;

    /// <summary>
    /// A value can not be added without a key.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="NotImplementedException"></exception>
    void ICollection<TValue>.Add(TValue item)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if the values collection contains an item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool ICollection<TValue>.Contains(TValue item)
    {
      return Values.Contains(item);
    }

    /// <summary>
    /// Copies the collection of values to an array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
    {
      Values.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// A value can not be removed without a key.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    bool ICollection<TValue>.Remove(TValue item)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Enumerates the collection of values.
    /// </summary>
    /// <returns></returns>
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
      return Values.GetEnumerator();
    }

    /// <summary>
    /// Clears the collection of values.
    /// </summary>
    public void Clear()
    {
      ((ICollection<TValue>)Values).Clear();
    }

    /// <summary>
    /// Enumerates the collection of values.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator()
    {
      return ((ICollection<TValue>)Values).GetEnumerator();
    }
    #endregion ICollection<TValue> support

    #region internal class of DispatchedDictionaryValues
    /// <summary>
    /// Internal collection of dispatched dictionary values.
    /// </summary>
    public class DispatchedDictionaryValues : ICollection<TValue>, INotifyCollectionChanged
    {
      /// <summary>
      /// Implementation of <see cref="INotifyCollectionChanged"/> interface.
      /// </summary>
      public event NotifyCollectionChangedEventHandler CollectionChanged
      {
        add { Owner.CollectionChanged += value; }
        remove { Owner.CollectionChanged -= value; }
      }

      /// <summary>
      /// Owner of the collection is the <see cref="DispatchedDictionary{TKey, TValue}"/> object.
      /// </summary>
      public DispatchedDictionary<TKey, TValue> Owner { get; private set; }

      #region common interface properties
      /// <summary>
      /// Returns the number of values.
      /// </summary>
      public int Count => Owner.Count;

      /// <summary>
      /// The collection is not read only.
      /// </summary>
      public bool IsReadOnly => true;
      #endregion

      /// <summary>
      /// Constructor based on the owner.
      /// </summary>
      /// <param name="owner"></param>
      public DispatchedDictionaryValues(DispatchedDictionary<TKey, TValue> owner)
      {
        Owner = owner;
      }

      /// <summary>
      /// Checks if the collection contains an item.
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public bool Contains(TValue item)
      {
        return Owner.Dictionary.Values.Contains(item);
      }

      /// <summary>
      /// Copies collection to an array.
      /// </summary>
      /// <param name="array"></param>
      /// <param name="arrayIndex"></param>
      public void CopyTo(TValue[] array, int arrayIndex)
      {
        Owner.Dictionary.Values.CopyTo(array, arrayIndex);
      }

      /// <summary>
      /// Enumerates on the collection items.
      /// </summary>
      /// <returns></returns>
      public IEnumerator<TValue> GetEnumerator()
      {
        return Owner.Dictionary.Values.GetEnumerator();
      }

      /// <summary>
      /// Enumerates on the collection items.
      /// </summary>
      /// <returns></returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        return Owner.Dictionary.Values.GetEnumerator();
      }

      /// <summary>
      /// An item can not be added directly.
      /// </summary>
      /// <param name="item"></param>
      /// <exception cref="NotImplementedException"></exception>
      public void Add(TValue item)
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// The collection can not be cleared directly.
      /// </summary>
      /// <exception cref="NotImplementedException"></exception>
      public void Clear()
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// An item can not be removed directly.
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
      public bool Remove(TValue item)
      {
        throw new NotImplementedException();
      }
    }
    #endregion

    #region implementation of the collection of key-value pairs
    /// <summary>
    /// Enumerates on the internal dictionary as on the collection of item-value pairs.
    /// </summary>
    /// <returns></returns>
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<TKey, TValue>>)Dictionary).GetEnumerator();
    }

    /// <summary>
    /// Copies the internal dictionary to an array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index)
    {
      ((ICollection)Dictionary).CopyTo(array, index);
    }

    /// <summary>
    /// Checks if the internal dictionary is synchronized.
    /// </summary>
    public bool IsSynchronized => ((ICollection)Dictionary).IsSynchronized;

    /// <summary>
    /// An object to lock synchronization critical section.
    /// </summary>
    public object SyncRoot => ((ICollection)Dictionary).SyncRoot;
    #endregion

    #region implementation of the dictionary of key-value pairs.
    /// <summary>
    /// Adds a key-value pair to the internal dictionary.
    /// Add a value to the internal values collection.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
    {
      if ((Dictionary).TryAdd(key, value))
        _Values.Add(value);
    }

    /// <summary>
    /// Checks if the internal dictionary contains a key;
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(TKey key)
    {
      return (Dictionary).ContainsKey(key);
    }

    /// <summary>
    /// Removes a key-value pair from the internal dictionary.
    /// On success it removes a value from the internal values collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Remove(TKey key)
    {
      if ((Dictionary).TryRemove(key, out var value))
        return _Values.Remove(value);
      else return false;
    }

    /// <summary>
    /// Tries to get a value of the key from the internal dictionary.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
      return (Dictionary).TryGetValue(key, out value);
    }

    /// <summary>
    /// Returns a value of the specific key from the internal dictionary. Only get accessor is implemented.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public TValue this[TKey key]
    {
      get
      {
        if (Dictionary.TryGetValue(key, out var value))
          return value;
        throw new KeyNotFoundException($"Key {key} not found");
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Access to the collection of keys in the internal dictionary.
    /// </summary>
    public ICollection<TKey> Keys => (Dictionary).Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => ((IDictionary<TKey, TValue>)Dictionary).Values;

    /// <summary>
    /// Adds a key-value pair to the internal dictionary.
    /// </summary>
    /// <param name="item"></param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      (Dictionary).TryAdd(item.Key, item.Value);
    }

    /// <summary>
    /// Checks if the internal dictionary contains a key-value pair.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return Dictionary.TryGetValue(item.Key, out var value) && item.Equals(value);
    }

    /// <summary>
    /// Copies the internal dictionary as the collection of key-value pairs to an array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the key-value pair from the internal dictionary.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Remove(item);
    }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IReadOnlyDictionary<TKey, TValue>)Dictionary).Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => ((IReadOnlyDictionary<TKey, TValue>)Dictionary).Values;
    #endregion
  }

}
