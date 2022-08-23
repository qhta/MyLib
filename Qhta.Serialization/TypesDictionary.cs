using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Qhta.Xml.Serialization
{
  public class TypesDictionary<ItemType>: IEnumerable<ItemType>, IDictionary<string, ItemType>, IDictionary<Type, ItemType> where ItemType : ITypeInfo
  {
    private Dictionary<Type, ItemType> TypeIndexedItems = new Dictionary<Type, ItemType>();
    private SortedDictionary<string, ItemType> StringIndexedItems = new SortedDictionary<string, ItemType>();

    public ItemType? FindTypeInfo(Type itemType)
    {
      var result = (this as IEnumerable<ItemType>).FirstOrDefault(item => itemType == item.Type);
      if (result == null)
        result = (this as IEnumerable<ItemType>).FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
      return result;
    }

    public IEnumerable<string> KnownTags(Type type)
    {
      foreach (var item in StringIndexedItems)
        if (type == item.Value.Type)
          yield return item.Key;
    }

    public void Add(ItemType value)
    {
      if (!TypeIndexedItems.ContainsKey(value.Type))
        TypeIndexedItems.Add(value.Type, value);
    }

    public void Add(string key, ItemType value)
    {
      if (!TypeIndexedItems.ContainsKey(value.Type))
        TypeIndexedItems.Add(value.Type, value);
      (StringIndexedItems).Add(key, value);
    }

    public bool ContainsKey(string key)
    {
      return (StringIndexedItems).ContainsKey(key);
    }

    public bool Remove(string key)
    {
      if (StringIndexedItems.TryGetValue(key, out var typeInfo))
        TypeIndexedItems.Remove(typeInfo.Type);
      return StringIndexedItems.Remove(key);
    }

    public bool TryGetValue(string key, /*[MaybeNullWhen(false)]*/ out ItemType value)
    {
      return (StringIndexedItems).TryGetValue(key, out value);
    }

    public ItemType this[string key] { get => (StringIndexedItems)[key]; set => (StringIndexedItems)[key] = value; }

    public ICollection<string> Keys => (StringIndexedItems).Keys;

    public ICollection<ItemType> Values => (TypeIndexedItems).Values;

    public void Add(KeyValuePair<string, ItemType> item)
    {
      ((ICollection<KeyValuePair<string, ItemType>>)StringIndexedItems).Add(item);
    }

    public void Clear()
    {
      StringIndexedItems.Clear();
      TypeIndexedItems.Clear();
    }

    public bool Contains(KeyValuePair<string, ItemType> item)
    {
      return ((ICollection<KeyValuePair<string, ItemType>>)StringIndexedItems).Contains(item);
    }

    public void CopyTo(KeyValuePair<string, ItemType>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<string, ItemType>>)StringIndexedItems).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, ItemType> item)
    {
      return ((ICollection<KeyValuePair<string, ItemType>>)StringIndexedItems).Remove(item);
    }

    public int Count => Math.Max(StringIndexedItems.Count, TypeIndexedItems.Count);

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, ItemType>>)StringIndexedItems).IsReadOnly;

    public IEnumerator<KeyValuePair<string, ItemType>> GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<string, ItemType>>)StringIndexedItems).GetEnumerator();
    }

    //IEnumerator IEnumerable.GetEnumerator()
    //{
    //  return ((IEnumerable)StringIndexedItems).GetEnumerator();
    //}

    public void Add(Type key, ItemType value)
    {
      ((IDictionary<Type, ItemType>)TypeIndexedItems).Add(key, value);
    }

    public bool ContainsKey(Type type)
    {
      return TypeIndexedItems.ContainsKey(type);
    }

    public bool Remove(Type type)
    {
      return TypeIndexedItems.Remove(type);
    }

    public bool TryGetValue(Type type, /*[MaybeNullWhen(false)]*/ out ItemType value)
    {
      return TypeIndexedItems.TryGetValue(type, out value);
    }

    IEnumerator<ItemType> IEnumerable<ItemType>.GetEnumerator()
    {
      return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)Values).GetEnumerator();
    }

    public ItemType this[Type key] { get => ((IDictionary<Type, ItemType>)TypeIndexedItems)[key]; set => ((IDictionary<Type, ItemType>)TypeIndexedItems)[key] = value; }

    ICollection<Type> IDictionary<Type, ItemType>.Keys => ((IDictionary<Type, ItemType>)TypeIndexedItems).Keys;

    public void Add(KeyValuePair<Type, ItemType> item)
    {
      ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).Add(item);
    }

    public bool Contains(KeyValuePair<Type, ItemType> item)
    {
      return ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).Contains(item);
    }

    public void CopyTo(KeyValuePair<Type, ItemType>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<Type, ItemType> item)
    {
      return ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).Remove(item);
    }

    IEnumerator<KeyValuePair<Type, ItemType>> IEnumerable<KeyValuePair<Type, ItemType>>.GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<Type, ItemType>>)TypeIndexedItems).GetEnumerator();
    }
  }
}
