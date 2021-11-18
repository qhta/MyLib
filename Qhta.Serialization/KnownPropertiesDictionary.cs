using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public class KnownPropertiesDictionary: ICollection<SerializedPropertyInfo>
  {
    private static readonly PropOrderComparer propertyInfoOrderComparer = new PropOrderComparer();

    private SortedDictionary<string, SerializedPropertyInfo> StringIndexedItems = new SortedDictionary<string, SerializedPropertyInfo>();
    private SortedSet<SerializedPropertyInfo> OrderedItems = new SortedSet<SerializedPropertyInfo>(propertyInfoOrderComparer);

    public void Add(string key, SerializedPropertyInfo value)
    {
      ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems).Add(key, value);
      OrderedItems.Add(value);
    }

    public bool ContainsKey(string key)
    {
      return ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems).ContainsKey(key);
    }

    public bool Remove(string key)
    {
      return ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems).Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out SerializedPropertyInfo value)
    {
      return ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems).TryGetValue(key, out value);
    }

    public SerializedPropertyInfo this[string key] { get => ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems)[key]; set => ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems)[key] = value; }

    public ICollection<string> Keys => ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems).Keys;

    public ICollection<SerializedPropertyInfo> Values => ((IDictionary<string, SerializedPropertyInfo>)StringIndexedItems).Values;


    public void Clear()
    {
      (StringIndexedItems).Clear();
    }

    public bool Contains(KeyValuePair<string, SerializedPropertyInfo> item)
    {
      return (StringIndexedItems).Contains(item);
    }

    public void CopyTo(KeyValuePair<string, SerializedPropertyInfo>[] array, int arrayIndex)
    {
      (StringIndexedItems).CopyTo(array, arrayIndex);
    }


    public int Count => (OrderedItems).Count;

    public bool IsReadOnly => false;


    IEnumerator IEnumerable.GetEnumerator()
    {
      return (OrderedItems).GetEnumerator();
    }

    public void Add(SerializedPropertyInfo item)
    {
      (OrderedItems).Add(item);
    }

    public bool Contains(SerializedPropertyInfo item)
    {
      return (OrderedItems).Contains(item);
    }

    public void CopyTo(SerializedPropertyInfo[] array, int arrayIndex)
    {
      (OrderedItems).CopyTo(array, arrayIndex);
    }

    public bool Remove(SerializedPropertyInfo item)
    {
      return (OrderedItems).Remove(item);
    }

    IEnumerator<SerializedPropertyInfo> IEnumerable<SerializedPropertyInfo>.GetEnumerator()
    {
      return ((IEnumerable<SerializedPropertyInfo>)OrderedItems).GetEnumerator();
    }
  }
}
