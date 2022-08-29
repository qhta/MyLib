using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.Xml.Serialization;

public class KnownPropertiesDictionary : IEnumerable<SerializationPropertyInfo>
{
  private static readonly PropOrderComparer propertyInfoOrderComparer = new PropOrderComparer();

  private SortedDictionary<string, SerializationPropertyInfo> StringIndexedItems = new SortedDictionary<string, SerializationPropertyInfo>();
  private SortedSet<SerializationPropertyInfo> OrderedItems = new SortedSet<SerializationPropertyInfo>(propertyInfoOrderComparer);

  public void Add(string key, SerializationPropertyInfo value)
  {
    ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems).Add(key, value);
    OrderedItems.Add(value);
  }

  public bool ContainsKey(string key)
  {
    return ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems).ContainsKey(key);
  }

  public bool Remove(string key)
  {
    return ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems).Remove(key);
  }

  public bool TryGetValue(string key, /*[MaybeNullWhen(false)]*/ out SerializationPropertyInfo value)
  {
    return ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems).TryGetValue(key, out value);
  }

  public SerializationPropertyInfo this[string key] { get => ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems)[key]; set => ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems)[key] = value; }

  public ICollection<string> Keys => ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems).Keys;

  public ICollection<SerializationPropertyInfo> Values => ((IDictionary<string, SerializationPropertyInfo>)StringIndexedItems).Values;


  public void Clear()
  {
    (StringIndexedItems).Clear();
  }

  public bool Contains(KeyValuePair<string, SerializationPropertyInfo> item)
  {
    return (StringIndexedItems).Contains(item);
  }

  public void CopyTo(KeyValuePair<string, SerializationPropertyInfo>[] array, int arrayIndex)
  {
    (StringIndexedItems).CopyTo(array, arrayIndex);
  }


  public int Count => (OrderedItems).Count;

  public bool IsReadOnly => false;


  IEnumerator IEnumerable.GetEnumerator()
  {
    return (OrderedItems).GetEnumerator();
  }

  //public void Add(SerializationPropertyInfo item)
  //{
  //  (OrderedItems).Add(item);
  //}

  //public bool Contains(SerializationPropertyInfo item)
  //{
  //  return (OrderedItems).Contains(item);
  //}

  //public void CopyTo(SerializationPropertyInfo[] array, int arrayIndex)
  //{
  //  (OrderedItems).CopyTo(array, arrayIndex);
  //}

  //public bool Remove(SerializationPropertyInfo item)
  //{
  //  return (OrderedItems).Remove(item);
  //}

  IEnumerator<SerializationPropertyInfo> IEnumerable<SerializationPropertyInfo>.GetEnumerator()
  {
    return ((IEnumerable<SerializationPropertyInfo>)OrderedItems).GetEnumerator();
  }
}