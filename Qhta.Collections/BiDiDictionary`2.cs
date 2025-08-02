using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Qhta.Collections;

/// <summary>
/// Bidirectional dictionary. Converts from Type1 to Type2.
/// </summary>
public class BiDiDictionary<Type1, Type2> : ICollection<KeyValuePair<Type1, Type2>>, IDictionary<Type1, Type2> where Type1 : notnull where Type2 : notnull
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public BiDiDictionary()
  {
    Index1 = new Dictionary<Type1, Type2>();
    Index2 = new SortedDictionary<Type2, Type1>();
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BiDiDictionary{Type1, Type2}"/> class.
  /// </summary>
  /// <param name="comparer1">The comparer1.</param>
  /// <param name="comparer2">The comparer2.</param>
  public BiDiDictionary(IEqualityComparer<Type1> comparer1, IEqualityComparer<Type2> comparer2)
  {
    Comparer1 = comparer1;
    Comparer2 = comparer2;
    Index1 = new Dictionary<Type1, Type2>(comparer1);
    Index2 = new Dictionary<Type2, Type1>(comparer2);
  }

  IEqualityComparer<Type1>? Comparer1;
  IEqualityComparer<Type2>? Comparer2;
  protected Dictionary<Type1, Type2> Index1;
  protected IDictionary<Type2, Type1> Index2;

  public void Clear()
  {
    Index1.Clear();
    Index2.Clear();
  }

  public void Add(Type1 value1, Type2 value2)
  {
#if NET6_0_OR_GREATER
    Index1.TryAdd(value1, value2);
    Index2.TryAdd(value2, value1);
#else
    // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
    if (!Index1.ContainsKey(value1))
      Index1.Add(value1, value2);
    // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
    if (!Index2.ContainsKey(value2))
      Index2.Add(value2, value1);
#endif
  }

  //public bool TryGetIndex1(Type1 key, out int n)
  //{
  //  var ok = Index1.TryGetValue(key, out n);
  //  return ok;
  //}

  //public bool TryGetIndex2(Type2 key, out int n)
  //{
  //  var ok = Index2.TryGetValue(key, out n);
  //  return ok;
  //}

  public bool TryGetValue2(Type1 key, out Type2 value)
  {
    if (Index1.TryGetValue(key, out var value2))
    {
      value = value2;
      return true;
    }
    value = default!;
    return false;
  }

  public bool TryGetValue1(Type2 key, out Type1 value)
  {
    if (Index2.TryGetValue(key, out var value2))
    {
      value = value2;
      return true;
    }
    value = default!;
    return false;
  }

  public Type1 GetValue1(Type2 key)
  {
    return Index2[key];
  }

  public Type2 GetValue2(Type1 key)
  {
    return Index1[key];
  }

  public bool ContainsKey(Type1 key)
  {
    return Index1.ContainsKey(key);
  }

  public bool Remove(Type1 key)
  {
    var remove2 = TryGetValue(key, out var value);
    if (Index1.Remove(key))
    {
      if (remove2)
        return Index2.Remove(value);
      return false;
    }
    return false;
  }

#if NET6_0_OR_GREATER
  public bool TryGetValue(Type1 key, [NotNullWhen(true)] out Type2 value)
#else
  public bool TryGetValue(Type1 key, out Type2 value)
#endif
  {
    return TryGetValue2(key, out value);
  }

  public Type2 this[Type1 key]
  {
    get
    {
      if (TryGetValue(key, out var val))
        return val;
      throw new KeyNotFoundException();
    }
    set
    {
      Remove(key);
      Add(key, value);
    }
  }

  public ICollection<Type1> Keys => Index1.ToArray().Select(item => item.Key).ToList();
  public ICollection<Type2> Values => Index1.ToArray().Select(item => item.Value).ToList();

  public void Add(KeyValuePair<Type1, Type2> item)
  {
    Add(item.Key, item.Value);
  }

  public bool Contains(KeyValuePair<Type1, Type2> item)
  {
    if (TryGetValue(item.Key, out var val))
    {
      return val.Equals(item.Value);
    }
    return false;
  }

  public void CopyTo(KeyValuePair<Type1, Type2>[] array, int arrayIndex)
  {
    Index1.Select(item => new KeyValuePair<Type1, Type2>(item.Key, item.Value)).ToArray().CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<Type1, Type2> item)
  {
    if (Index1.TryGetValue(item.Key, out var val) && val.Equals(item.Value))
    {
      Index1.Remove(item.Key);
      Index2.Remove(item.Value);
      return true;
    }
    return false;
  }

  public int Count => Index1.Count;

  public bool IsReadOnly => false;

  public IEnumerator<KeyValuePair<Type1, Type2>> GetEnumerator()
  {
    return Index1.ToArray().Select(item => new KeyValuePair<Type1, Type2>(item.Key, item.Value)).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)Index1).GetEnumerator();
  }

  public Type2? GetValueOrDefault(Type1 key) =>
    GetValueOrDefault(key, default!);

  public Type2 GetValueOrDefault(Type1 key, Type2 defaultValue)
  {
    return TryGetValue(key, out Type2? value) ? value : defaultValue;
  }

  public Type1? GetIndexOrDefault(Type2 key) =>
    GetIndexOrDefault(key, default!);

  public Type1 GetIndexOrDefault(Type2 key, Type1 defaultValue)
  {
    return TryGetValue1(key, out Type1? value) ? value : defaultValue;
  }
}

