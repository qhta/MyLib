using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;

namespace Qhta.DispatchedObjects
{
  /// <summary>
  /// A dispatched version of <see cref="ObservableCollection{TValue}"/>.
  /// Is is based on <see cref="DispatchedObject"/> to notify on changes and to invoke actions.
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  public class DispatchedCollection<TValue> : DispatchedObject, IEnumerable<TValue>, INotifyCollectionChanged, ICollection<TValue>
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DispatchedCollection()
    {
      _Values.CollectionChanged += _Values_CollectionChanged;
    }

    /// <summary>
    /// External collection of values.
    /// </summary>
    public IEnumerable<TValue> Values => _Values;

    /// <summary>
    /// Internal observable collection of values.
    /// It can be accessed in descendant classes.
    /// </summary>
    protected ObservableCollection<TValue> _Values = new ObservableCollection<TValue>();


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
    /// It can be checked in descendant classes.
    /// </summary>
    protected event NotifyCollectionChangedEventHandler? _CollectionChanged;

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
    /// to notify that a <see cref="Count"/> has changed.
    /// </summary>
    /// <param name="args"></param>
    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      if (_InAddRange)
        return;
      if (_CollectionChanged != null)
      {
        var dispatcher = DispatcherBridge;
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
    /// A virtual method to notify that a <see cref="Count"/> property has changed.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void AfterCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      NotifyPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// A flag to signal that <see cref="Add(TValue)"/> operation is invoked in <see cref="AddRange(IEnumerable{TValue}) "/> method.
    /// </summary>
    protected bool _InAddRange;

    /// <summary>
    /// Adds a whole collection of items. 
    /// <see cref="_InAddRange"/> is used to omit large number of notifications in the <see cref="Add(TValue)"/> method.
    /// <see cref="OnCollectionChanged(NotifyCollectionChangedEventArgs)"/> is invoked after adding the last item.
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IEnumerable<TValue> items)
    {
      _InAddRange = true;
      int startIndex = Count;
      foreach (var item in items)
        Add(item);
      _InAddRange = false;
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<TValue>(items), startIndex));
    }

    /// <summary>
    /// Adds a single item.
    /// Invokes protected <see cref="InsertItem(int, TValue)"/> method.
    /// Uses <see cref="DispatchedObject.DispatcherBridge"/>, if it is set, to add the item.
    /// Otherwise the item is added directly.
    /// </summary>
    /// <param name="item"></param>
    public void Add(TValue item)
    {
      InsertItem(Count, item);
      Count++;
    }

    /// <summary>
    /// Inserts a single item at the specified index.
    /// Invokes protected <see cref="InsertItem(int, TValue)"/> method.
    /// Uses <see cref="DispatchedObject.DispatcherBridge"/>, if it is set, to add the item.
    /// Otherwise the item is added directly.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public virtual void Insert(int index, TValue item)
    {
      InsertItem(index, item);
    }

    /// <summary>
    /// Inserts a single item at the specified index.
    /// Uses <see cref="DispatchedObject.DispatcherBridge"/>, if it is set, to add the item.
    /// Otherwise the item is added directly.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected virtual void InsertItem(int index, TValue item)
    {
      var dispatcher = DispatcherBridge;
      if (dispatcher != null)
      {
        dispatcher.Invoke(() =>
        {
          _Values.Insert(index, item);
        });
      }
      else
        _Values.Insert(index, item);
    }

    /// <summary>
    /// Returns the count of items.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// The collection is not read only.
    /// </summary>
    public bool IsReadOnly => ((ICollection<TValue>)_Values).IsReadOnly;

    /// <summary>
    /// Gets the <see cref="_Values"/> collection as an array.
    /// </summary>
    /// <returns></returns>
    public TValue[] ToArray()
    {
      return _Values.ToArray();
    }

    /// <summary>
    /// Enumerates the <see cref="_Values"/> collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TValue> GetEnumerator()
    {
      return (_Values).GetEnumerator();
    }

    /// <summary>
    /// Enumerates the <see cref="_Values"/> collection.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    /// <summary>
    /// Clears the <see cref="_Values"/> collection.
    /// </summary>
    public void Clear()
    {
      ((ICollection<TValue>)_Values).Clear();
    }

    /// <summary>
    /// Checks if the <see cref="_Values"/> collection contains the item.
    /// </summary>
    public bool Contains(TValue item)
    {
      return ((ICollection<TValue>)_Values).Contains(item);
    }

    /// <summary>
    /// Copies the <see cref="_Values"/> collection to an array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(TValue[] array, int arrayIndex)
    {
      ((ICollection<TValue>)_Values).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes an item from the <see cref="_Values"/> collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(TValue item)
    {
      return ((ICollection<TValue>)_Values).Remove(item);
    }
  }

}
