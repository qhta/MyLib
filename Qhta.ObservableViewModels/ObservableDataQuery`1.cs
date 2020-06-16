using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Qhta.ObservableObjects;

namespace Qhta.ObservableViewModels
{
  public class ObservableDataQuery<TValue> : ObservableDataSet<TValue>,
    IEnumerable<TValue>,
    ICollection<TValue>,
    IList<TValue>,
    IEnumerable,
    ICollection,
    IList,
    INotifyCollectionChanged, INotifyPropertyChanged where TValue : class
  {
    #region LazyLoad
    public override void PopulateWhenNeeded()
    {
      Source.PopulateWhenNeeded();
    }
    #endregion

    public ObservableDataQuery(ObservableDataSet<TValue> source, Dispatcher dispatcher) : this(source, null, dispatcher) { }

    public ObservableDataQuery(ObservableDataSet<TValue> source, Func<TValue, bool> filter, Dispatcher dispatcher): base(dispatcher)
    {
      Source = source;
      if (source != null)
        source.CollectionChanged += Source_CollectionChanged;
      _filter = filter;
    }

    private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
    }

    public readonly ObservableDataSet<TValue> Source;
    public Func<TValue, bool> Filter
    {
      get => _filter;
      set
      {
        if (_filter != value)
        {
          _filter = value;
          //Debug.WriteLine($"ObservableDataQuery.SetFilter(Filtered={_filter != null})");
          NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
        }
      }
    }
    private Func<TValue, bool> _filter;


    public new IEnumerable<TValue> Items
    {
      get
      {
        //Debug.WriteLine($"ObservableDataQuery.Items(Filtered={_filter!=null})");
        if (Filter == null)
          return Source.ToList();
        else
          return Source.ToList().Where(item => Filter(item));
      }
    }

    /// <summary>
    /// The number of filtered items.
    /// </summary>
    /// <returns></returns>
    public override int Count => Items.Count();


    public override bool IsReadOnly => false;

    public override void Add(TValue item)
    {
      Source.Add(item);
    }

    public override bool Remove(TValue item)
    {
      return Source.Remove(item);
    }

    public override void Clear()
    {
      Source.Clear();
    }

    public override bool Contains(TValue item)
    {
      PopulateWhenNeeded();
      return Source.Contains(item);
    }

    public override void CopyTo(TValue[] array, int arrayIndex)
    {
      var outputLength = array.Length;
      var items = Items.ToArray();
      var itemsLength = items.Length;
      if (itemsLength <= outputLength - arrayIndex)
        items.CopyTo(array, arrayIndex);
      else
        items.AsSpan(arrayIndex, outputLength - arrayIndex).CopyTo(array);
    }

    public override IEnumerator<TValue> GetEnumerator()
    {
      PopulateWhenNeeded();
      return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public override int IndexOf(TValue value)
    {
      PopulateWhenNeeded();
      return Items.ToList().IndexOf(value);
    }

    public override TValue this[int index]
    {
      get
      {
        PopulateWhenNeeded();
        return Items.ToList()[index];
      }
      set => throw new NotImplementedException();
    }


    public override object SyncRoot => Source.SyncRoot;

    public override bool IsSynchronized => Source.IsSynchronized;

    public override bool IsModified { get => Source.IsModified; protected internal set=>Source.IsModified=value; }

    object IList.this[int index] { get => this[index]; set => this[index] = (TValue)value; }

    public override void Insert(int index, TValue item)
    {
      throw new InvalidOperationException($"ObservablaDataQuery.Insert cannot be invoked");
    }

    public override void RemoveAt(int index)
    {
      throw new InvalidOperationException($"ObservablaDataQuery.RemoveAt cannot be invoked");
    }

    int IList.Add(object value)
    {
      if (value is TValue item)
      {
        this.Add(item);
        return Source.IndexOf(item);
      }
      return -1;
    }

    bool IList.Contains(object value)
    {
      if (value is TValue item)
      {
        return Items.Contains(item);
      }
      return false;
    }

    int IList.IndexOf(object value)
    {
      throw new InvalidOperationException($"ObservablaDataQuery.IndexOf cannot be invoked");
    }

    void IList.Insert(int index, object value)
    {
      throw new InvalidOperationException($"ObservablaDataQuery.Insert cannot be invoked");
    }

    void IList.Remove(object value)
    {
      if (value is TValue item)
      {
        this.Remove(item);
      }
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array is TValue[] items)
      {
        this.CopyTo(items, index);
      }
    }
  }
}
