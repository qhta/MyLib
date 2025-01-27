using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Qhta.Collections;

/// <summary>
/// Bidirectional dictionary. Converts from Type1 to Type2.
/// </summary>
public class BiDiDictionary<Type1, Type2> : List<Tuple<Type1, Type2>>, IDictionary<Type1, Type2> where Type1 : notnull where Type2 : notnull
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public BiDiDictionary()
  {
    Index1 = new Dictionary<Type1, int>();
    Index2 = new Dictionary<Type2, int>();
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
    Index1 = new Dictionary<Type1, int>(comparer1);
    Index2 = new Dictionary<Type2, int>(comparer2);
  }

  IEqualityComparer<Type1>? Comparer1;
  IEqualityComparer<Type2>? Comparer2;
  protected Dictionary<Type1, int> Index1;
  protected Dictionary<Type2, int> Index2;

  public new void Clear()
  {
    base.Clear();
    Index1.Clear();
    Index2.Clear();
  }

  public void Add(Type1 value1, Type2 value2)
  {
    base.Add(new Tuple<Type1, Type2>(value1, value2));
    if (!Index1.ContainsKey(value1))
      Index1.Add(value1, base.Count - 1);
    if (!Index2.ContainsKey(value2))
      Index2.Add(value2, base.Count - 1);
  }

  public bool TryGetIndex1(Type1 key, out int n)
  {
    var ok = Index1.TryGetValue(key, out n);
    return ok;
  }

  public bool TryGetIndex2(Type2 key, out int n)
  {
    var ok = Index2.TryGetValue(key, out n);
    return ok;
  }

  public bool TryGetValue2(Type1 key, out Type2 value)
  {
    if (TryGetIndex1(key, out int n))
    {
      value = base[n].Item2;
      return true;
    }
#pragma warning disable CS8601 // Possible null reference assignment.
    value = default(Type2);
#pragma warning restore CS8601 // Possible null reference assignment.
    return false;
  }

  public bool TryGetValue1(Type2 key, out Type1 value)
  {
    if (TryGetIndex2(key, out int n))
    {
      value = base[n].Item1;
      return true;
    }
#pragma warning disable CS8601 // Possible null reference assignment.
    value = default(Type1);
#pragma warning restore CS8601 // Possible null reference assignment.
    return false;
  }

  public Type1 GetValue1(Type2 key)
  {
    if (TryGetIndex2(key, out int n))
    {
      return base[n].Item1;
    }
    throw new KeyNotFoundException($"{key} not found in BiDiDictionary");
  }

  public Type2 GetValue2(Type1 key)
  {
    if (TryGetIndex1(key, out int n))
    {
      return base[n].Item2;
    }
    throw new KeyNotFoundException($"{key} not found in BiDiDictionary");
  }

  public bool ContainsKey(Type1 key)
  {
    if (TryGetIndex1(key, out _))
    {
      return true;
    }
    return false;
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
    get => throw new NotImplementedException();
    set => throw new NotImplementedException();
  }

  public ICollection<Type1> Keys { get => base.ToArray().Select(item => item.Item1).ToList(); }
  public ICollection<Type2> Values { get => base.ToArray().Select(item => item.Item2).ToList(); }

  public void Add(KeyValuePair<Type1, Type2> item)
  {
    Add(item.Key, item.Value);
  }

  public bool Contains(KeyValuePair<Type1, Type2> item)
  {
    if (TryGetIndex1(item.Key, out int n))
    {
      return base[n].Item2.Equals(item.Value);
    }
    return false;
  }

  public void CopyTo(KeyValuePair<Type1, Type2>[] array, int arrayIndex)
  {
    base.ToArray().Select(item => new KeyValuePair<Type1, Type2>(item.Item1, item.Item2)).ToArray().CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<Type1, Type2> item)
  {
    if (TryGetIndex1(item.Key, out int n))
    {
      if (base[n].Item2.Equals(item.Value))
      {
        base.RemoveAt(n);
        return true;
      }
    }
    return false;
  }

  public bool IsReadOnly { get => false; }

  IEnumerator<KeyValuePair<Type1, Type2>> IEnumerable<KeyValuePair<Type1, Type2>>.GetEnumerator()
  {
    return base.ToArray().Select(item => new KeyValuePair<Type1, Type2>(item.Item1, item.Item2)).GetEnumerator();
  }
}

