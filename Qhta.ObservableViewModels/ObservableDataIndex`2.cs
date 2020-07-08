using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using Qhta.ObservableObjects;

namespace Qhta.ObservableViewModels
{
  public class ObservableDataIndex<TKey, TValue> : ObservableObject,
      ICollection<TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IList<TValue>
  {
    public ObservableDataIndex(Dispatcher dispatcher): this (null, dispatcher) { }

    public ObservableDataIndex(string aName, Dispatcher dispatcher): base(dispatcher)
    {
      Items = new ObservableDictionary<object, TValue>(_dispatcher);
      //Debug.WriteLine($"ObservableDataIndex({aName}).Create");
      Dictionary<string, List<PropertyInfo>> allIndexes = new Dictionary<string, List<PropertyInfo>>();
      foreach (var propInfo in typeof(TValue).GetProperties())
      {
        //Debug.WriteLine($"ObservableDataIndex({aName}).FoundProperty {propInfo.Name}");
        IndexAttribute indexAttribute;
        if ((indexAttribute = propInfo.GetCustomAttribute<IndexAttribute>(true)) != null)
        {
          if (!propInfo.CanRead)
            throw new KeyNotFoundException($"Indexed property {propInfo.Name} has no get accessor");
          var indexName = indexAttribute.Name ?? "";
          if (allIndexes.TryGetValue(indexName, out var properties))
            properties.Add(propInfo);
          else
          {
            var props = new List<PropertyInfo>();
            props.Add(propInfo);
            allIndexes.Add(indexName, props);
          }
        }
      }
      if (allIndexes.Count == 0)
      {
        throw new KeyNotFoundException($"Index key not found in class {typeof(TValue).Name}. Use [Index] attribute where necessary");
      }
      if (allIndexes.Count == 1)
      {
        indexKeys = allIndexes.Values.First();
      }
      else
      {
        if (aName==null)
          throw new KeyNotFoundException($"Ambiguous index key found in class {typeof(TValue).Name}. Use index constructor with a specific index name");
        if (allIndexes.TryGetValue(aName, out var indexes))
          indexKeys = indexes;
        else
          throw new KeyNotFoundException($"Index key {aName} found in class {typeof(TValue).Name}. Use [Index] attribute with a specific index name");
      }

      if (indexKeys.Count == 1)
      {
          if (indexKeys[0].PropertyType != typeof(TKey))
          throw new InvalidCastException($"Type of primary key property in class {typeof(TValue).Name} " +
            $"different from {typeof(TKey).Name} as declared in {this.GetType().Name}");
      }
      else
      {
        bool found = false;
        foreach (var constructorInfo in typeof(TKey).GetConstructors())
        {
          var constructorParamList = constructorInfo.GetParameters().Select(item => item.ParameterType).ToList();
          if (constructorParamList.Count == indexKeys.Count)
          {
            int k = 0;
            for (int i = 0; i < constructorParamList.Count; i++)
            {
              if (constructorParamList[i] == indexKeys[i])
                k++;
            }
            if (k == constructorParamList.Count)
            {
              keyConstructor = constructorInfo;
              found = true;
              break;
            }
          }
        }
        if (!found)
        {
          var typeNames = String.Join(", ", indexKeys.Select(item => item.PropertyType.Name));
          throw new InvalidCastException($"No appropriate constructor found in {typeof(TKey).Name}." +
            $" A public constructor with the following parameters should be declared: {typeNames}");
        }
      }
    }

    protected List<PropertyInfo> indexKeys = new List<PropertyInfo>();
    protected ConstructorInfo keyConstructor;

    public readonly ObservableDictionary<object, TValue> Items;

    #region Notification
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add => Items.CollectionChanged += value;
      remove => Items.CollectionChanged -= value;
    }
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

    public virtual bool RemoveKey(TKey key)
    {
      var ok = Items.TryGetValue(key, out TValue item);
      if (ok)
        Items.Remove(key);
      return ok;
    }

    public virtual void Add(TValue item)
    {
      Add(CreateKey(item), item);
    }

    protected virtual TKey CreateKey(TValue item)
    {
      if (indexKeys.Count == 1)
      {
        var key = (TKey)indexKeys[0].GetValue(item);
        return key;
      }
      else
      {
        object[] keyValues = new object[indexKeys.Count];
        for (int i = 0; i < indexKeys.Count; i++)
          keyValues[i] = indexKeys[i].GetValue(item);
        var key = (TKey)keyConstructor.Invoke(keyValues);
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
      var ok = Items.Values.Contains(item);
      Items.Remove(CreateKey(item));
      return ok;
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
      return (TKey)Items.Keys.ToList()[index];
    }

    public virtual TValue this[object index]
    {
      get
      {
        if (Items.TryGetValue(index, out TValue item))
          return item;
        return default(TValue);
      }
    }

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

    public virtual bool IsModified { get; private set; }

    public TValue this[int index] { get => Items.Values.ToArray()[index]; set => throw new NotImplementedException(); }

    private TValue selectedItem;

    public bool ShouldSerializeSelectedItem() => false;

    public virtual bool Contains(object value)
    {
      if (value is TValue subdocument)
        return Items.Contains(subdocument);
      return false;
    }

    public virtual void Insert(int index, TValue value)
    {
      throw new NotImplementedException("TValue.Insert not implemented");
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

    public virtual IEnumerable<TValue> Values => Items.Values;
  }
}

