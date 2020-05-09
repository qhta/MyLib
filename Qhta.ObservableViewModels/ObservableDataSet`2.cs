using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Qhta.ObservableImmutable;

namespace Qhta.ObservableViewModels
{
  public class ObservableDataSet<TKey, TValue> : ObservableObject,
      ICollection<TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IList<TValue> where TKey: IComparable<TKey>
  {
    public ObservableDataSet()
    {
      foreach (var propInfo in typeof(TValue).GetProperties())
      {
        if (propInfo.GetCustomAttribute<KeyAttribute>() != null)
        {
          primaryKeys.Add(propInfo);
        }
      }
      if (primaryKeys.Count==0)
      {
        throw new KeyNotFoundException($"Primary key not found in class {typeof(TValue).Name}. Use [Key] attribute where necessary");
      }
      foreach (var propInfo in primaryKeys)
      {
        if (!propInfo.CanRead)
          throw new KeyNotFoundException($"Primary key {propInfo.Name} has no get accessor");
      }
      if (primaryKeys.Count == 1)
      {
        if (primaryKeys[0].PropertyType != typeof(TKey))
          throw new InvalidCastException($"Type of primary key property in class {typeof(TValue).Name} " +
            $"different from {typeof(TKey).Name} as declared in {this.GetType().Name}");
      }
      else
      {
        bool found = false;
        foreach (var constructorInfo in typeof(TKey).GetConstructors())
        {
          var constructorParamList = constructorInfo.GetParameters().Select(item => item.ParameterType).ToList();
          if (constructorParamList.Count == primaryKeys.Count)
          {
            int k = 0;
            for (int i = 0; i < constructorParamList.Count; i++)
            {
              if (constructorParamList[i] == primaryKeys[i])
                k++;
            }
            if (k == constructorParamList.Count)
            {
              primaryKeyConstructor = constructorInfo;
              found = true;
              break;
            }
          }
        }
        if (!found)
        {
          var typeNames = String.Join(", ", primaryKeys.Select(item => item.PropertyType.Name));
          throw new InvalidCastException($"No appropriate constructor found in {typeof(TKey).Name}." +
            $" A public constructor with the following parameters should be declared: {typeNames}");
        }
      }
    }
    protected List<PropertyInfo> primaryKeys = new List<PropertyInfo>();
    protected ConstructorInfo primaryKeyConstructor;

    public readonly ObservableDictionary<TKey, TValue> Items = new ObservableDictionary<TKey, TValue>();

    #region Notification
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add => Items.CollectionChanged += value;
      remove => Items.CollectionChanged -= value;
    }

    public bool NotifyCollectionChangedEnabled { get => Items.NotifyCollectionChangedEnabled; set => Items.NotifyCollectionChangedEnabled = value; }

    public virtual void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
      => Items.NotifyCollectionChanged(args);

    public void BulkChangeStart(NotifyCollectionChangedAction action) => Items.BulkChangeStart(action);
    public void BulkChangeEnd() => Items.BulkChangeEnd();

    #endregion

    public virtual int Count => Items.Count;

    public virtual bool IsReadOnly => false;

    public virtual bool ContainsKey(TKey key)
    {
      return Items.ContainsKey(key);
    }

    public virtual bool TryGetValue(TKey key, out TValue aItem)
    {
      return Items.TryGetValue(key, out aItem);
    }

    public virtual void Add(TKey key, TValue item)
    {
      Items.Add(key, item);
      IsModified = true;
    }

    public virtual bool Remove(TKey key)
    {
      var ok = Items.TryGetValue(key, out TValue item);
      if (ok)
      {
        Items.Remove(key);
      }
      return ok;
    }

    public virtual void Add(TValue item)
    {
      Add(CreateKey(item), item);
    }

    protected virtual TKey CreateKey(TValue item)
    {
      if (primaryKeys.Count == 1)
      {
        var key = (TKey)primaryKeys[0].GetValue(item);
        return key;
      }
      else
      {
        object[] keyValues = new object[primaryKeys.Count];
        for (int i = 0; i < primaryKeys.Count; i++)
          keyValues[i] = primaryKeys[i].GetValue(item);
        TKey key = (TKey)primaryKeyConstructor.Invoke(keyValues);
        return key;
      }
    }
    public virtual void Clear()
    {
      if (Items.Count > 0)
      {
        Items.Clear();
      }
    }

    public virtual bool Contains(TValue item) => Items.Values.Contains(item);

    public virtual void CopyTo(TValue[] array, int arrayIndex)
    {
      Items.Values.ToArray().CopyTo(array, arrayIndex);
    }

    public virtual bool Remove(TValue item)
    {
      return this.Remove(CreateKey(item));
    }

    public virtual IEnumerator<TValue> GetEnumerator()
    {
      return Items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public virtual int IndexOf(TValue value)
    {
      return Items.Values.ToList().IndexOf(value);
    }

    public virtual int IndexOfKey(TKey key)
    {
      return Items.Keys.ToList().IndexOf(key);
    }


    public virtual TKey KeyOfIndex(int index)
    {
      return Items.Keys.ToList()[index];
    }

    public virtual TValue this[TKey index] => Items[index];

    public virtual TValue SelectedItem
    {
      get => selectedItem;
      set
      {
        if (!selectedItem.Equals(value))
        {
          selectedItem = value;
          base.NotifyPropertyChanged(nameof(SelectedItem));
        }
      }
    }

    public bool IsFixedSize => false;

    public virtual object SyncRoot => throw new NotImplementedException();

    public virtual bool IsSynchronized => throw new NotImplementedException();

    public virtual bool IsModified { get; private set; }

    public TValue this[int index] { get => Items.Values.ToArray()[index]; set => throw new NotImplementedException(); }

    private TValue selectedItem;

    public bool ShouldSerializeSelectedItem() => false;

    //public virtual int Add(TValue value)
    //{
    //  if (value is TValue abbreviation)
    //    Add(abbreviation);
    //  return IndexOf(value);
    //}

    public virtual bool Contains(object value)
    {
      if (value is TValue subdocument)
        return Items.Contains(subdocument);
      return false;
    }

    //public virtual int IndexOf(TValue value)
    //{
    //  if (value is TValue subdocument)
    //    return Items.Values.ToList().IndexOf(subdocument);
    //  return -1;
    //}

    public virtual void Insert(int index, TValue value)
    {
      throw new NotImplementedException("TValue.Insert not implemented");
    }

    public virtual void Remove(object value)
    {
      if (value is TValue abbreviation)
        Items.Remove(abbreviation);
    }

    public virtual void RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

    public virtual void CopyTo(Array array, int index)
    {
      // This method is invoked by PresentationFramework
      // if BindingOperations.EnableCollectionSynchronization was called
      Items.Values.ToArray().CopyTo(array, index);
    }
  }

}

