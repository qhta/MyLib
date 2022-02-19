using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Qhta.Serialization
{
  public class TypesDictionary<ItemType>: IEnumerable<ItemType> where ItemType : ITypeInfo
  {
    private Dictionary<Type, ItemType> TypeIndexedItems = new Dictionary<Type, ItemType>();
    private SortedDictionary<string, ItemType> StringIndexedItems = new SortedDictionary<string, ItemType>();


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
      return (StringIndexedItems).Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out ItemType value)
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

    public int Count => ((ICollection<KeyValuePair<string, ItemType>>)StringIndexedItems).Count;

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

    public bool TryGetValue(Type type, [MaybeNullWhen(false)] out ItemType value)
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
  }
}
