using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using Qhta.ObservableObjects;

namespace Qhta.ObservableViewModels
{
  public class ObservableDataIndex<TValue> : ObservableObject,
      ICollection<TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IList<TValue>
  {

    public ObservableDataIndex(Type keyType, string property) : this(keyType, new string[] { property })
    {

    }

    public ObservableDataIndex(Type keyType, string[] properties) : base()
    {
      Init(keyType, properties);
    }

    public ObservableDataIndex(Type keyType, string property, Dispatcher dispatcher) : this(keyType, new string[] {property }, dispatcher)
    {

    }

    public ObservableDataIndex(Type keyType, string[] properties, Dispatcher dispatcher) : base(dispatcher)
    {
      Init(keyType, properties);
    }

    private void Init(Type keyType, string[] properties)
    { 
      Items = new ObservableDictionary<object, TValue>(_dispatcher);
      foreach (var propName in properties)
      {
        var propInfo = typeof(TValue).GetProperty(propName);
        if (propInfo == null)
          throw new InvalidOperationException($"Property {propName} not found in type {typeof(TValue).Name}");
        if (!propInfo.CanRead)
          throw new KeyNotFoundException($"Primary key {propInfo.Name} has no get accessor");
        propertyInfos.Add(propInfo);
      }
      if (propertyInfos.Count == 1)
      {
        if (propertyInfos[0].PropertyType != keyType)
          throw new InvalidCastException($"Typeof property {propertyInfos[0].Name} in type {typeof(TValue).Name} " +
            $"different from {keyType.Name} as declared in {this.GetType().Name}");
      }
      else
      {
        bool found = false;
        foreach (var constructorInfo in keyType.GetConstructors())
        {
          var constructorParamList = constructorInfo.GetParameters().Select(item => item.ParameterType).ToList();
          if (constructorParamList.Count == propertyInfos.Count)
          {
            int k = 0;
            for (int i = 0; i < constructorParamList.Count; i++)
            {
              if (constructorParamList[i] == propertyInfos[i])
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
          var typeNames = String.Join(", ", propertyInfos.Select(item => item.PropertyType.Name));
          throw new InvalidCastException($"No appropriate constructor found in {keyType.Name}." +
            $" A public constructor with the following parameters should be declared: {typeNames}");
        }
      }
    }
    protected List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
    protected ConstructorInfo keyConstructor;

    public ObservableDictionary<object, TValue> Items { get; private set; }

    #region Notification
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add => Items.CollectionChanged += value;
      remove => Items.CollectionChanged -= value;
    }
    #endregion

    public virtual int Count => Items.Count;

    public virtual bool IsReadOnly => false;

    public virtual bool ContainsKey(object key)
    {
      return Items.ContainsKey(key);
    }

    public virtual bool TryGetValue(object key, out TValue aItem)
    {
      return Items.TryGetValue(key, out aItem);
    }

    public virtual void Add(object key, TValue item)
    {
      Items.Add(key, item);
      IsModified = true;
    }

    public virtual bool RemoveKey(object key)
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

    protected virtual object CreateKey(TValue item)
    {
      if (propertyInfos.Count == 1)
      {
        var key = (object)propertyInfos[0].GetValue(item);
        return key;
      }
      else
      {
        object[] keyValues = new object[propertyInfos.Count];
        for (int i = 0; i < propertyInfos.Count; i++)
          keyValues[i] = propertyInfos[i].GetValue(item);
        object key = (object)keyConstructor.Invoke(keyValues);
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

    public virtual int IndexOfKey(object key)
    {
      return Items.Keys.ToList().IndexOf(key);
    }


    public virtual object KeyOfIndex(int index)
    {
      return Items.Keys.ToList()[index];
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

