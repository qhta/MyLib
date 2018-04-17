using System;
using System.Collections;
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
  public class DispatchedCollection<T>: DispatchedObject, ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
  {
    protected DispatchedDictionary<int, T> Collection = new DispatchedDictionary<int, T>();

    public int Count => ((ICollection<T>)Collection).Count;

    public bool IsReadOnly => ((ICollection<T>)Collection).IsReadOnly;

    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add
      {
        ((INotifyCollectionChanged)Collection).CollectionChanged+=value;
      }

      remove
      {
        ((INotifyCollectionChanged)Collection).CollectionChanged-=value;
      }
    }

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

    public void Add(T item)
    {
      //if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
      //{
      Collection.TryAdd(item.GetHashCode(), item);
      HasItems = true;
      //}
      //else if (ApplicationDispatcher!=null)
      //{
      //  var action = new Action<T>(Add);
      //  ApplicationDispatcher.BeginInvoke(action, new object[] { item });
      //}
    }

    public void Clear()
    {
      ((ICollection<T>)Collection).Clear();
    }

    public bool Contains(T item)
    {
      return ((ICollection<T>)Collection).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      ((ICollection<T>)Collection).CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return ((ICollection<T>)Collection).GetEnumerator();
    }

    public void Insert(int index, T item)
    {
      Add(item);
      //if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
      //{
        //Collection.Insert(index, item);
      //}
      //else if (ApplicationDispatcher!=null)
      //{
      //  var action = new Action<int, T>(Insert);
      //  ApplicationDispatcher.BeginInvoke(action, new object[] { index, item });
      //}
    }

    public bool Remove(T item)
    {
      return ((ICollection<T>)Collection).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((ICollection<T>)Collection).GetEnumerator();
    }
  }
}
