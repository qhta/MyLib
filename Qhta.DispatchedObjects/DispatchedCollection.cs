using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace Qhta.DispatchedObjects
{
  /// <summary>
  /// A dispatched version of <see cref="ObservableCollection{TValue}"/>.
  /// Is is based on <see cref="DispatchedObject"/> to notify on changes and to invoke actions.
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  public class DispatchedCollection<TValue> : ObservableCollection<TValue>,
    //DispatchedObject, ICollection<TValue>,
    //IList<TValue>, 
    //IList, 
    //IReadOnlyList<TValue>,
    INotifyCollectionChanged, INotifyPropertyChanged
  {
    ///// <summary>
    ///// Default constructor.
    ///// </summary>
    //public DispatchedCollection()
    //{
    //  _Values.CollectionChanged += _Values_CollectionChanged;
    //}

    ///// <summary>
    ///// External collection of values.
    ///// </summary>
    //public IEnumerable<TValue> Values => _Values;

    ///// <summary>
    ///// Internal observable collection of values.
    ///// It can be accessed in descendant classes.
    ///// </summary>
    //protected ObservableCollection<TValue> _Values = new ObservableCollection<TValue>();


    /// <summary>
    /// Implementation of <see cref="INotifyCollectionChanged"/> interface.
    /// </summary>
    public new event NotifyCollectionChangedEventHandler? CollectionChanged
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
    /// It can be checked in descendant classes.
    /// </summary>
    protected event NotifyCollectionChangedEventHandler? _CollectionChanged;

    /// <summary>
    /// Property changed event which implements <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public new event PropertyChangedEventHandler? PropertyChanged
    {
      add
      {
        _PropertyChanged += value;
      }
      remove
      {
        _PropertyChanged -= value;
      }
    }
    /// <summary>
    /// A handler of <see cref="PropertyChanged"/> event which can be checked in descending classes.
    /// </summary>
    protected event PropertyChangedEventHandler? _PropertyChanged;

    /// <summary>
    /// A method to notify that a property has changed. 
    /// Uses <see cref="DispatchedObject.DispatcherBridge"/>, if it is set, to invoke <see cref="PropertyChanged"/> event.
    /// Otherwise the event is invoked directly.
    /// </summary>
    /// <param name="propertyName"></param>
    public virtual void NotifyPropertyChanged(string propertyName)
    {
      if (_PropertyChanged != null)
      {
        var dispatcher = DispatchedObject.DispatcherBridge;
        if (dispatcher != null)
          lock (this)
            dispatcher.Invoke(() => _PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        else
          _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// A protected method which enables external objects to notify that the collection has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void _Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChanged(e);
    }

    /// <summary>
    /// A protected method that is called to notify that the collection has changed.
    /// Uses <see cref="DispatchedObject.DispatcherBridge"/>, if it is set, to invoke <see cref="CollectionChanged"/> event.
    /// Otherwise the event is invoked directly.
    /// After that a <see cref="AfterCollectionChanged(NotifyCollectionChangedEventArgs)"/> is called 
    /// to notify that items Count has changed.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      if (_InAddRange)
        return;
      if (_CollectionChanged != null)
      {
        var dispatcher = DispatchedObject.DispatcherBridge;
        if (dispatcher != null)
        {
          var action = new Action<NotifyCollectionChangedEventArgs>(OnCollectionChanged);
          dispatcher.Invoke(() =>
          {
            _CollectionChanged.Invoke(this, args);
            AfterCollectionChanged(args);
          });
        }
        else
        {
          _CollectionChanged.Invoke(this, args);
          AfterCollectionChanged(args);
        }
      }
      else
        AfterCollectionChanged(args);
    }

    /// <summary>
    /// A virtual method to notify that a Count property has changed.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void AfterCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      NotifyPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// A flag to signal that Add(TValue) operation is invoked in <see cref="AddRange(IEnumerable{TValue}) "/> method.
    /// </summary>
    protected bool _InAddRange;

    /// <summary>
    /// Adds a whole collection of items. 
    /// <see cref="_InAddRange"/> is used to omit large number of notifications in Add(TValue) method.
    /// <see cref="OnCollectionChanged(NotifyCollectionChangedEventArgs)"/> is invoked after adding the last item.
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IEnumerable<TValue> items)
    {
      lock (this)
      {
        _InAddRange = true;
        int startIndex = Count;
        foreach (var item in items)
          InsertItem(startIndex++, item);
        _InAddRange = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<TValue>(items), startIndex));
      }
    }

    /// <summary>
    /// Adds a single item.
    /// Invokes protected InsertItem method.
    /// </summary>
    /// <param name="item"></param>
    public new int Add(TValue item)
    {
      lock (this)
      {
        var count = Count;
        InsertItem(count, item);
        return count;
      }
    }

    /// <summary>
    /// Inserts a single item at the specified index.
    /// Invokes protected InsertItem method.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public new void Insert(int index, TValue item)
    {
      lock (this)
        InsertItem(index, item);
    }

    //    /// <summary>
    //    /// Inserts a single item at the specified index.
    //    /// Uses <see cref="DispatchedObject.DispatcherBridge"/>, if it is set, to add the item.
    //    /// Otherwise the item is added directly.
    //    /// </summary>
    //    /// <param name="index"></param>
    //    /// <param name="item"></param>
    //    protected virtual void InsertItem(int index, TValue item)
    //    {
    //      var dispatcher = DispatcherBridge;
    //      if (dispatcher != null)
    //      {
    //        dispatcher.Invoke(() =>
    //        {
    //          _Values.Insert(index, item);
    //        });
    //      }
    //      else
    //        _Values.Insert(index, item);
    //    }

    //    /// <summary>
    //    /// Returns the count of items.
    //    /// </summary>
    //    public int Count => _Count;//(_InAddRange) ? _Count : _Values.Count;
    //    private int _Count;

    //    /// <summary>
    //    /// The collection is not read only.
    //    /// </summary>
    //    public bool IsReadOnly => base.IsReadOnly;

    //    /// <summary>
    //    /// Gets the collection as an array.
    //    /// </summary>
    //    /// <returns></returns>
    //    public TValue[] ToArray()
    //    {
    //      return _Values.ToArray();
    //    }

    /// <summary>
    /// Enumerates the collection.
    /// </summary>
    /// <returns></returns>
    public new IEnumerator<TValue> GetEnumerator()
    {
      lock (this)
        return base.GetEnumerator();
    }

    //    /// <summary>
    //    /// Enumerates the collection.
    //    /// </summary>
    //    /// <returns></returns>
    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //      return this.GetEnumerator();
    //    }

    /// <summary>
    /// Clears the collection.
    /// </summary>
    public new void Clear()
    {
      lock (this)
        base.Clear();
    }

    /// <summary>
    /// Checks if the collection contains the item.
    /// </summary>
    public new bool Contains(TValue item)
    {
      lock (this)
        return base.Contains(item);
    }

    /// <summary>
    /// Copies the collection to an array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public new void CopyTo(TValue[] array, int arrayIndex)
    {
      lock (this)
        base.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes an item from the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public new bool Remove(TValue item)
    {
      lock (this)
        return base.Remove(item);
    }

    /// <inheritdoc/>
    public new int IndexOf(TValue item)
    {
      lock (this)
        return base.IndexOf(item);
    }

    /// <inheritdoc/>
    public new void RemoveAt(int index)
    {
      lock (this)
        base.RemoveAt(index);
    }

    /// <inheritdoc/>
    public new TValue? this[int index]
    {
      get
      {
        lock (this)
          return (index < Count) ? base[index] : default(TValue);
      }
      set => throw new NotImplementedException();

    }

    /// <inheritdoc/>
    public int Add(object? value)
    {
      if (value is TValue tValue)
        return this.Add(tValue);
      return -1;
    }

    /// <inheritdoc/>
    public bool Contains(object value)
    {
      if (value is TValue tValue)
        return this.Contains(tValue);
      return false;
    }

    /// <inheritdoc/>
    public int IndexOf(object value)
    {
      if (value is TValue tValue)
        return this.IndexOf(tValue);
      return -1;
    }

    /// <inheritdoc/>
    public void Insert(int index, object value)
    {
      if (value is TValue tValue)
        this.Insert(index, tValue);
    }

    /// <inheritdoc/>
    public void Remove(object value)
    {
      if (value is TValue tValue)
        this.Remove(tValue);
    }

    ///// <inheritdoc/>
    //public bool IsFixedSize => ((IList)_Values).IsFixedSize;

    //object? IList.this[int index]
    //{
    //  get => (index<_Values.Count) ? ((IList)_Values)[index] : null;
    //  set => ((IList)_Values)[index] = value;
    //}

    ///// <inheritdoc/>
    //public void CopyTo(Array array, int index)
    //{
    //  ((ICollection)_Values).CopyTo(array, index);
    //}

    ///// <inheritdoc/>
    //public bool IsSynchronized => ((ICollection)_Values).IsSynchronized;

    ///// <inheritdoc/>
    //public object SyncRoot => ((ICollection)_Values).SyncRoot;

    /// <summary>
    /// A method to invoke and action.
    /// Uses <see cref="DispatchedObject.DispatcherBridge"/>, if it is set, to invoke an action .
    /// Otherwise the action is invoked directly.
    /// </summary>
    /// <param name="action"></param>
    public virtual void Dispatch(Action action)
    {
      var dispatcher = DispatchedObject.DispatcherBridge;
      if (dispatcher != null)
        lock (this)
          dispatcher.Invoke(action);
      else
        action.Invoke();
    }
  }

}
