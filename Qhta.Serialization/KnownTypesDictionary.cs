using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Qhta.Serialization
{
  public class KnownTypesDictionary : IDictionary<string, SerializationTypeInfo>
  {
    private SortedDictionary<string, SerializationTypeInfo> StringIndexedItems = new SortedDictionary<string, SerializationTypeInfo>();
    private Dictionary<string, SerializationTypeInfo> TypeIndexedItems = new Dictionary<string, SerializationTypeInfo>();

    #region IDictionary implementation

    public void Add(string key, Type type)
    {
      this.Add(key, new SerializationTypeInfo(key, type));
    }

    public void Add(string key, SerializationTypeInfo value)
    {
      ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems).Add(key, value);
      if (!TypeIndexedItems.ContainsKey(value.Type.FullName ?? ""))
        TypeIndexedItems.Add(value.Type.FullName ?? "", value);
    }

    public bool ContainsKey(string key)
    {
      return ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems).ContainsKey(key);
    }

    public bool Remove(string key)
    {
      return ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems).Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out SerializationTypeInfo value)
    {
      return ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems).TryGetValue(key, out value);
    }

    public SerializationTypeInfo this[string key] { get => ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems)[key]; set => ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems)[key] = value; }

    public ICollection<string> Keys => ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems).Keys;

    public ICollection<SerializationTypeInfo> Values => ((IDictionary<string, SerializationTypeInfo>)StringIndexedItems).Values;

    public void Add(KeyValuePair<string, SerializationTypeInfo> item)
    {
      ((ICollection<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).Add(item);
    }

    public void Clear()
    {
      ((ICollection<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).Clear();
    }

    public bool Contains(KeyValuePair<string, SerializationTypeInfo> item)
    {
      return ((ICollection<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).Contains(item);
    }

    public void CopyTo(KeyValuePair<string, SerializationTypeInfo>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, SerializationTypeInfo> item)
    {
      return ((ICollection<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).Remove(item);
    }

    public int Count => ((ICollection<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).IsReadOnly;

    public IEnumerator<KeyValuePair<string, SerializationTypeInfo>> GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<string, SerializationTypeInfo>>)StringIndexedItems).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)StringIndexedItems).GetEnumerator();
    }

    public void Add(Type key, SerializationTypeInfo value)
    {
      ((IDictionary<Type, SerializationTypeInfo>)TypeIndexedItems).Add(key, value);
    }

    public bool ContainsKey(Type key)
    {
      return TypeIndexedItems.ContainsKey(key.FullName ?? "");
    }

    public bool Remove(Type key)
    {
      return TypeIndexedItems.Remove(key.FullName ?? "");
    }

    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out SerializationTypeInfo value)
    {
      return TypeIndexedItems.TryGetValue(key.FullName ?? "", out value);
    }

    public SerializationTypeInfo this[Type key] { get => (TypeIndexedItems)[key.FullName ?? ""]; set => (TypeIndexedItems)[key.FullName ?? ""] = value; }
    #endregion


  }
}
