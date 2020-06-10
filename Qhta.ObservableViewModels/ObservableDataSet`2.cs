using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Qhta.ObservableObjects;

namespace Qhta.ObservableViewModels
{
  public class ObservableDataSet<TKey, TValue> : ObservableDataSet<TValue>,
      ICollection<TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IList<TValue> where TKey: IComparable<TKey> where TValue: class
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

    public readonly ObservableDictionary<TKey, TValue> PrimaryIndex = new ObservableDictionary<TKey, TValue>();

    public virtual bool ContainsKey(TKey key)
    {
      return PrimaryIndex.ContainsKey(key);
    }

    public virtual bool TryGetValue(TKey key, out TValue aItem)
    {
      return PrimaryIndex.TryGetValue(key, out aItem);
    }

    public override void Add(TValue item)
    {
      var key = CreateKey(item);
      PrimaryIndex.Add(key, item);
      base.Add(item);
      IsModified = true;
    }

    public virtual void Add(TKey key, TValue item)
    {
      PrimaryIndex.Add(key, item);
      base.Add(item);
      IsModified = true;
    }

    public override bool Remove(TValue item)
    {
      bool ok;
      var key = CreateKey(item);
      ok = PrimaryIndex.ContainsKey(key);
      if (ok)
      {
        PrimaryIndex.Remove(key);
        base.Remove(item);
      }
      IsModified = ok;
      return ok;
    }

    public virtual bool Remove(TKey key)
    {
      var ok = PrimaryIndex.TryGetValue(key, out TValue item);
      if (ok)
      {
        PrimaryIndex.Remove(key);
        base.Remove(item);
      }
      IsModified = ok;
      return ok;
    }

    protected virtual TKey CreateKey(TValue item)
    {
      if (primaryKeys.Count == 0)
        return default(TKey);
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
    public override void Clear()
    {
      PrimaryIndex.Clear();
      Items.Clear();
    }

    public virtual int IndexOfKey(TKey key)
    {
      return PrimaryIndex.Keys.ToList().IndexOf(key);
    }

    public virtual TValue this[TKey index] => PrimaryIndex[index];

    public override void Insert(int index, TValue value)
    {
      PrimaryIndex.Add(CreateKey(value), value);
      Items.Insert(index, value);
      IsModified = true;
    }

  }

}

