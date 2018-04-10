using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace MyLib.WpfUtils
{
  //TODO: ObservableCollection<T>.Move() is not implemented...
  [Serializable]
  [ComVisible(false)]
  [DebuggerDisplay("Count = {Count}")]
  public class SynchronizedObservableCollection<T> : IDisposable, IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
  {
    #region Constructor
    public SynchronizedObservableCollection()
    {
      _context = SynchronizationContext.Current;
      _items = new List<T>();
    }
    #endregion

    #region Private Fields

    public SynchronizationContext Context
    {
      get
      {
        if (_context==null)
          _context = SynchronizationContext.Current;
        return _context;
      }
    }
    private SynchronizationContext _context;

    public IList<T> Items
    {
      get
      {
        if (_items==null)
          _items = new List<T>();
        return _items;
      }
    }

    private IList<T> _items;
    private readonly ReaderWriterLockSlim ItemsLock = new ReaderWriterLockSlim();
    private readonly object _lock = new object();
    [NonSerialized] private Object _syncRoot;

    private readonly SimpleMonitor _monitor = new SimpleMonitor();
    #endregion

    #region Events
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add { PropertyChanged += value; }
      remove { PropertyChanged -= value; }
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;
    protected event PropertyChangedEventHandler PropertyChanged;
    #endregion

    #region Private Properties
    bool IList.IsFixedSize
    {
      get
      {
        //TODO: Do I need a lock here?
        // There is no IList<T>.IsFixedSize, so we must assume that only
        // readonly collections are fixed size, if our internal item 
        // collection does not implement IList.  Note that Array implements
        // IList, and therefore T[] and U[] will be fixed-size.
        var list = Items as IList;
        if (list != null)
        {
          return list.IsFixedSize;
        }

        return Items.IsReadOnly;
      }
    }

    bool ICollection<T>.IsReadOnly
    {
      get { return Items.IsReadOnly; }
    }

    bool IList.IsReadOnly
    {
      get { return Items.IsReadOnly; }
    }

    //TODO: Does this mean what I think it does?
    bool ICollection.IsSynchronized
    {
      get { return true; }
    }

    object ICollection.SyncRoot
    {
      get
      {
        if (_syncRoot == null)
        {
          ItemsLock.EnterReadLock();

          try
          {
            var c = Items as ICollection;
            if (c != null)
            {
              _syncRoot = c.SyncRoot;
            }
            else
            {
              Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
            }
          }
          finally
          {
            ItemsLock.ExitReadLock();
          }
        }

        return _syncRoot;
      }
    }
    #endregion

    #region Public Properties
    public int Count
    {
      get
      {
        ItemsLock.EnterReadLock();

        try
        {
          return Items.Count;
        }
        finally
        {
          ItemsLock.ExitReadLock();
        }
      }
    }

    public T this[int index]
    {
      get
      {
        ItemsLock.EnterReadLock();

        try
        {
          CheckIndex(index);

          return Items[index];
        }
        finally
        {
          ItemsLock.ExitReadLock();
        }
      }
      set
      {
        T oldValue;

        ItemsLock.EnterWriteLock();

        try
        {
          CheckIsReadOnly();
          CheckIndex(index);
          CheckReentrancy();

          oldValue = this[index];

          Items[index] = value;

        }
        finally
        {
          ItemsLock.ExitWriteLock();
        }

        OnPropertyChanged("Item[]");
        OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldValue, value, index);
      }
    }

    object IList.this[int index]
    {
      get { return this[index]; }
      set
      {
        try
        {
          this[index] = (T)value;
        }
        catch (InvalidCastException)
        {
          throw new ArgumentException("'value' is the wrong type");
        }
      }
    }

    #endregion

    #region Private Methods
    private IDisposable BlockReentrancy()
    {
      _monitor.Enter();

      return _monitor;
    }

    // ReSharper disable once UnusedParameter.Local
    private void CheckIndex(int index)
    {
      if (index < 0 || index >= Items.Count)
      {
        throw new ArgumentOutOfRangeException();
      }
    }

    private void CheckIsReadOnly()
    {
      if (Items.IsReadOnly)
      {
        throw new NotSupportedException("Collection is readonly");
      }
    }

    private void CheckReentrancy()
    {
      if (_monitor.Busy && CollectionChanged != null && CollectionChanged.GetInvocationList().Length > 1)
      {
        throw new InvalidOperationException("SynchronizedObservableCollection reentrancy not allowed");
      }
    }

    private static bool IsCompatibleObject(object value)
    {
      // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
      // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
      return ((value is T) || (value == null && default(T) == null));
    }

    private void OnPropertyChanged(string propertyName)
    {
      OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
    {
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
    {
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
    {
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
    }

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      var collectionChanged = CollectionChanged;
      if (collectionChanged == null)
      {
        return;
      }

      using (BlockReentrancy())
      {
        Context.Send(state => collectionChanged(this, e), null);
      }
    }

    private void OnCollectionReset()
    {
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      var propertyChanged = PropertyChanged;
      if (propertyChanged == null)
      {
        return;
      }

      Context.Send(state => propertyChanged(this, e), null);
    }
    #endregion

    #region Public Methods
    public void Add(T item)
    {
      ItemsLock.EnterWriteLock();

      var index = Items.Count;

      try
      {
        CheckIsReadOnly();
        CheckReentrancy();

        Items.Insert(index, item);
      }
      finally
      {
        ItemsLock.ExitWriteLock();
      }

      OnPropertyChanged("Count");
      OnPropertyChanged("Item[]");
      if (item.GetType().Name=="Synset")
      {
        string s = item.GetType().GetProperty("Text").GetValue(item) as string;
        if (s=="physical entity")
          Debug.WriteLine($"{s} added");
      }
      OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }

    int IList.Add(object value)
    {
      ItemsLock.EnterWriteLock();

      var index = Items.Count;
      T item;

      try
      {
        CheckIsReadOnly();
        CheckReentrancy();

        item = (T)value;

        Items.Insert(index, item);
      }
      catch (InvalidCastException)
      {
        throw new ArgumentException("'value' is the wrong type");
      }
      finally
      {
        ItemsLock.ExitWriteLock();
      }

      OnPropertyChanged("Count");
      OnPropertyChanged("Item[]");
      OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);

      return index;
    }

    public void Clear()
    {
      ItemsLock.EnterWriteLock();

      try
      {
        CheckIsReadOnly();
        CheckReentrancy();

        Items.Clear();
      }
      finally
      {
        ItemsLock.ExitWriteLock();
      }

      OnPropertyChanged("Count");
      OnPropertyChanged("Item[]");
      OnCollectionReset();
    }

    public void CopyTo(T[] array, int index)
    {
      ItemsLock.EnterReadLock();

      try
      {
        Items.CopyTo(array, index);
      }
      finally
      {
        ItemsLock.ExitReadLock();
      }
    }

    void ICollection.CopyTo(Array array, int index)
    {
      ItemsLock.EnterReadLock();

      try
      {
        if (array == null)
        {
          throw new ArgumentNullException("array", "'array' cannot be null");
        }

        if (array.Rank != 1)
        {
          throw new ArgumentException("Multidimension arrays are not supported", "array");
        }

        if (array.GetLowerBound(0) != 0)
        {
          throw new ArgumentException("Non-zero lower bound arrays are not supported", "array");
        }

        if (index < 0)
        {
          throw new ArgumentOutOfRangeException("index", "'index' is out of range");
        }

        if (array.Length - index < Items.Count)
        {
          throw new ArgumentException("Array is too small");
        }

        var tArray = array as T[];
        if (tArray != null)
        {
          Items.CopyTo(tArray, index);
        }
        else
        {
          //
          // Catch the obvious case assignment will fail.
          // We can found all possible problems by doing the check though.
          // For example, if the element type of the Array is derived from T,
          // we can't figure out if we can successfully copy the element beforehand.
          //
          var targetType = array.GetType().GetElementType();
          var sourceType = typeof(T);
          if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
          {
            throw new ArrayTypeMismatchException("Invalid array type");
          }

          //
          // We can't cast array of value type to object[], so we don't support 
          // widening of primitive types here.
          //
          var objects = array as object[];
          if (objects == null)
          {
            throw new ArrayTypeMismatchException("Invalid array type");
          }

          var count = Items.Count;
          try
          {
            for (var i = 0; i < count; i++)
            {
              objects[index++] = Items[i];
            }
          }
          catch (ArrayTypeMismatchException)
          {
            throw new ArrayTypeMismatchException("Invalid array type");
          }
        }
      }
      finally
      {
        ItemsLock.ExitReadLock();
      }
    }

    public bool Contains(T item)
    {
      ItemsLock.EnterReadLock();

      try
      {
        return Items.Contains(item);
      }
      finally
      {
        ItemsLock.ExitReadLock();
      }
    }

    bool IList.Contains(object value)
    {
      if (IsCompatibleObject(value))
      {
        ItemsLock.EnterReadLock();

        try
        {
          return Items.Contains((T)value);
        }
        finally
        {
          ItemsLock.ExitReadLock();
        }
      }

      return false;
    }

    public void Dispose()
    {
      ItemsLock.Dispose();
    }

    public IEnumerator<T> GetEnumerator()
    {
      ItemsLock.EnterReadLock();

      try
      {
        return Items.ToList().GetEnumerator();
      }
      finally
      {
        ItemsLock.ExitReadLock();
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      ItemsLock.EnterReadLock();

      try
      {
        return ((IEnumerable)Items.ToList()).GetEnumerator();
      }
      finally
      {
        ItemsLock.ExitReadLock();
      }
    }

    public int IndexOf(T item)
    {
      ItemsLock.EnterReadLock();

      try
      {
        return Items.IndexOf(item);
      }
      finally
      {
        ItemsLock.ExitReadLock();
      }
    }

    int IList.IndexOf(object value)
    {
      if (IsCompatibleObject(value))
      {
        ItemsLock.EnterReadLock();

        try
        {
          return Items.IndexOf((T)value);
        }
        finally
        {
          ItemsLock.ExitReadLock();
        }
      }

      return -1;
    }

    public void Insert(int index, T item)
    {
      ItemsLock.EnterWriteLock();

      try
      {
        CheckIsReadOnly();
        CheckIndex(index);
        CheckReentrancy();

        Items.Insert(index, item);
      }
      finally
      {
        ItemsLock.ExitWriteLock();
      }

      OnPropertyChanged("Count");
      OnPropertyChanged("Item[]");
      OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }

    void IList.Insert(int index, object value)
    {
      try
      {
        Insert(index, (T)value);
      }
      catch (InvalidCastException)
      {
        throw new ArgumentException("'value' is the wrong type");
      }
    }

    public bool Remove(T item)
    {
      int index;
      T value;

      ItemsLock.EnterUpgradeableReadLock();

      try
      {
        CheckIsReadOnly();

        index = Items.IndexOf(item);
        if (index < 0)
        {
          return false;
        }

        ItemsLock.EnterWriteLock();

        try
        {
          CheckReentrancy();

          value = Items[index];

          Items.RemoveAt(index);
        }
        finally
        {
          ItemsLock.ExitWriteLock();
        }
      }
      finally
      {
        ItemsLock.ExitUpgradeableReadLock();
      }

      OnPropertyChanged("Count");
      OnPropertyChanged("Item[]");
      OnCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);

      return true;
    }

    void IList.Remove(object value)
    {
      if (IsCompatibleObject(value))
      {
        Remove((T)value);
      }
    }

    public void RemoveAt(int index)
    {
      T value;

      ItemsLock.EnterWriteLock();

      try
      {
        CheckIsReadOnly();
        CheckIndex(index);
        CheckReentrancy();

        value = Items[index];

        Items.RemoveAt(index);
      }
      finally
      {
        ItemsLock.ExitWriteLock();
      }

      OnPropertyChanged("Count");
      OnPropertyChanged("Item[]");
      OnCollectionChanged(NotifyCollectionChangedAction.Remove, value, index);
    }
    #endregion

    #region SimpleMonitor Class
    private class SimpleMonitor : IDisposable
    {
      private int _busyCount;

      public bool Busy
      {
        get { return _busyCount > 0; }
      }

      public void Enter()
      {
        ++_busyCount;
      }

      public void Dispose()
      {
        --_busyCount;
      }
    }
    #endregion
  }
}
