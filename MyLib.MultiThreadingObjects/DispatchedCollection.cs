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
  public class DispatchedCollection<TValue>: DispatchedObject, IEnumerable<TValue>, INotifyCollectionChanged, ICollection<TValue>
  {
    public DispatchedCollection()
    {
      _Values.CollectionChanged+=_Values_CollectionChanged;
    }

    public IEnumerable<TValue> Values => _Values;
    private ObservableCollection<TValue> _Values = new ObservableCollection<TValue>();


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
    protected event NotifyCollectionChangedEventHandler _CollectionChanged;

    private void _Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChanged(e);
    }

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (inAddRange)
        return;
      if (_CollectionChanged != null)
      {
        if (DispatchedObject.ApplicationDispatcher==null ||Dispatcher.CurrentDispatcher==DispatchedObject.ApplicationDispatcher)
        {
          _CollectionChanged.Invoke(this, e);
          AfterCollectionChanged(e);
        }
        else
        {
          var action = new Action<NotifyCollectionChangedEventArgs>(OnCollectionChanged);
          DispatchedObject.ApplicationDispatcher.Invoke(action, new object[] { e });
        }
      }
      else
        AfterCollectionChanged(e);
    }

    protected virtual void AfterCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      NotifyPropertyChanged(nameof(Count));
    }

    bool inAddRange;

    public void AddRange(IEnumerable<TValue> items)
    {
      inAddRange = true;
      int startIndex = Count;
      foreach (var item in items)
        Add(item);
      inAddRange = false;
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<TValue>(items), startIndex));
    }

    public void Add(TValue item)
    {
      if (DispatchedObject.ApplicationDispatcher==null || Dispatcher.CurrentDispatcher==DispatchedObject.ApplicationDispatcher)
        _Values.Add(item);
      else
      {
        var action = new Action<TValue>(Add);
        DispatchedObject.ApplicationDispatcher.Invoke(action, new object[] { item });
      }
    }

    public int Count => _Values.Count;

    public bool IsReadOnly => ((ICollection<TValue>)_Values).IsReadOnly;

    public TValue[] ToArray()
    {
      return _Values.ToArray();
    }

    public IEnumerator<TValue> GetEnumerator()
    {
      return (_Values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public void Clear()
    {
      ((ICollection<TValue>)_Values).Clear();
    }

    public bool Contains(TValue item)
    {
      return ((ICollection<TValue>)_Values).Contains(item);
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
      ((ICollection<TValue>)_Values).CopyTo(array, arrayIndex);
    }

    public bool Remove(TValue item)
    {
      return ((ICollection<TValue>)_Values).Remove(item);
    }
  }

}
