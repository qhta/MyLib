using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public class KnownTypesDictionary: IDictionary<string, SerializedTypeInfo>
  {
    private SortedDictionary<string, SerializedTypeInfo> StringIndexedItems = new SortedDictionary<string, SerializedTypeInfo>();
    private Dictionary<string, SerializedTypeInfo> TypeIndexedItems = new Dictionary<string, SerializedTypeInfo>();

    public void Add(string key, SerializedTypeInfo value)
    {
      ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems).Add(key, value);
      //if (value.Type.Name=="WikiLink")
      //  Debug.Assert(true);
      if (!TypeIndexedItems.ContainsKey(value.Type.FullName))
        TypeIndexedItems.Add(value.Type.FullName, value);
    }

    public bool ContainsKey(string key)
    {
      return ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems).ContainsKey(key);
    }

    public bool Remove(string key)
    {
      return ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems).Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out SerializedTypeInfo value)
    {
      return ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems).TryGetValue(key, out value);
    }

    public SerializedTypeInfo this[string key] { get => ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems)[key]; set => ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems)[key] = value; }

    public ICollection<string> Keys => ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems).Keys;

    public ICollection<SerializedTypeInfo> Values => ((IDictionary<string, SerializedTypeInfo>)StringIndexedItems).Values;

    public void Add(KeyValuePair<string, SerializedTypeInfo> item)
    {
      ((ICollection<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).Add(item);
    }

    public void Clear()
    {
      ((ICollection<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).Clear();
    }

    public bool Contains(KeyValuePair<string, SerializedTypeInfo> item)
    {
      return ((ICollection<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).Contains(item);
    }

    public void CopyTo(KeyValuePair<string, SerializedTypeInfo>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, SerializedTypeInfo> item)
    {
      return ((ICollection<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).Remove(item);
    }

    public int Count => ((ICollection<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).IsReadOnly;

    public IEnumerator<KeyValuePair<string, SerializedTypeInfo>> GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<string, SerializedTypeInfo>>)StringIndexedItems).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)StringIndexedItems).GetEnumerator();
    }

    public void Add(Type key, SerializedTypeInfo value)
    {
      ((IDictionary<Type, SerializedTypeInfo>)TypeIndexedItems).Add(key, value);
    }

    public bool ContainsKey(Type key)
    {
      return TypeIndexedItems.ContainsKey(key.FullName);
    }

    public bool Remove(Type key)
    {
      return TypeIndexedItems.Remove(key.FullName);
    }

    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out SerializedTypeInfo value)
    {
      return TypeIndexedItems.TryGetValue(key.FullName, out value);
    }

    public SerializedTypeInfo this[Type key] { get => (TypeIndexedItems)[key.FullName]; set => (TypeIndexedItems)[key.FullName] = value; }

  }
}
