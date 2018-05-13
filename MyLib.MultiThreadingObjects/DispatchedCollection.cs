using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyLib.MultiThreadingObjects
{
  public class DispatchedCollection<TValue>: DispatchedObject, IDisposable
    , ICollection<TValue>, IEnumerable<TValue>
    , INotifyCollectionChanged
  {

    protected List<TValue> List;

    #region constructors
    public DispatchedCollection()
    {
      List = new List<TValue>();
      _Values = new DispatchedCollectionValues(this);
    }

    public DispatchedCollection(string name)
    {
      Name = name;
      List = new List<TValue>();
    }

    public DispatchedCollection(IEnumerable<TValue> collection)
    {
      List = new List<TValue>();
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
    public DispatchedCollectionValues Values =>_Values;
    #endregion
    private DispatchedCollectionValues _Values;

    #region overriden methods

    public void AddRange(IEnumerable<TValue> values)
    {
      lock (_lock)
      {
        int index = List.Count;
        List.AddRange(values);
        HasItems = true;
        ThrowInsertEvent(index, values);
        //for (int i=0; i<values.Count(); i++)
        //  ThrowInsertEvent(index+i, List[index+i]);
      }
    }


    public void Add(TValue value)
    {
      lock (_lock)
      {
        int index = List.Count;
        List.Add(value);
        HasItems = true;
        ThrowInsertEvent(index, value);
      }
    }

    public void Insert(int index, TValue value)
    {
      lock (_lock)
      {
        List.Insert(index, value);
        HasItems = true;
        ThrowInsertEvent(index, value);
      }
    }
    public bool Remove(TValue value)
    {
      lock (_lock)
      {
        bool result = List.Remove(value);
        if (result)
          ThrowRemoveEvent(value);
        return result;
      }
    }

    //private void ThrowAddEvent(TValue newItem)
    //{
    //  if (_CollectionChanged != null)
    //  {
    //    if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
    //      _CollectionChanged.Invoke(this,
    //        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
    //        newItem
    //        ));
    //    else
    //    {
    //      var action = new Action<TValue>(ThrowAddEvent);
    //      ApplicationDispatcher.Invoke(action, new object[] { newItem });
    //    }
    //  }
    //}

    private void ThrowInsertEvent(int index, TValue newItem)
    {
      if (_CollectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            new List<TValue> { newItem }, index
            ));
        else
        {
          var action = new Action<int, TValue>(ThrowInsertEvent);
          ApplicationDispatcher.Invoke(action, new object[] { index, newItem });
        }
      }
    }

    private void ThrowInsertEvent(int index, IEnumerable<TValue> newItems)
    {
      if (_CollectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
          _CollectionChanged.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            new List<TValue>(newItems), index
            ));
        else
        {
          var action = new Action<int, IEnumerable<TValue>>(ThrowInsertEvent);
          ApplicationDispatcher.Invoke(action, new object[] { index, newItems });
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

    public int Count => List.Count;

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

    public IEnumerator<TValue> GetEnumerator()
    {
      return ((ICollection<TValue>)Values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((ICollection<TValue>)Values).GetEnumerator();
    }

    #endregion ICollection<TValue> support

    public class DispatchedCollectionValues : ICollection<TValue>, INotifyCollectionChanged
    {
      public event NotifyCollectionChangedEventHandler CollectionChanged
      {
        add { Owner.CollectionChanged+=value; }
        remove { Owner.CollectionChanged-=value; }
      }

      public DispatchedCollection<TValue> Owner { get; private set; }

      public int Count => Owner.Count;

      public bool IsReadOnly => true;

      public DispatchedCollectionValues(DispatchedCollection<TValue> owner)
      {
        Owner = owner;
      }

      public bool Contains(TValue item)
      {
        return Owner.List.Contains(item);
      }

      public void CopyTo(TValue[] array, int arrayIndex)
      {
        Owner.List.CopyTo(array, arrayIndex);
      }

      public IEnumerator<TValue> GetEnumerator()
      {
        return Owner.List.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return Owner.List.GetEnumerator();
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
