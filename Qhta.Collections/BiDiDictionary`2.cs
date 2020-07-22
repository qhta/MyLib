using System;
using System.Collections.Generic;

namespace Qhta.Collections
{
  public class BiDiDictionary<Type1, Type2> : List<Tuple<Type1, Type2>>
  {
    public BiDiDictionary()
    {
      index1 = new Dictionary<Type1, int>();
      index2 = new Dictionary<Type2, int>();
    }

    public BiDiDictionary(IEqualityComparer<Type1> comparer1, IEqualityComparer<Type2> comparer2)
    {
      Comparer1 = comparer1;
      Comparer2 = comparer2;
      index1 = new Dictionary<Type1, int>(comparer1);
      index2 = new Dictionary<Type2, int>(comparer2);
    }

    IEqualityComparer<Type1> Comparer1;
    IEqualityComparer<Type2> Comparer2;
    private Dictionary<Type1, int> index1;
    private Dictionary<Type2, int> index2;

    public void Add(Type1 value1, Type2 value2)
    {
      base.Add(new Tuple<Type1, Type2>(value1, value2));
      if (!index1.ContainsKey(value1))
        index1.Add(value1, base.Count - 1);
      if (!index2.ContainsKey(value2))
        index2.Add(value2, base.Count - 1);
    }

    public bool TryGetIndex1(Type1 key, out int n)
    {
      var ok = index1.TryGetValue(key, out n);
      return ok;
    }

    public bool TryGetIndex2(Type2 key, out int n)
    {
      var ok = index2.TryGetValue(key, out n);
      return ok;
    }

    public bool TryGetValue2(Type1 key, out Type2 value)
    {
      if (TryGetIndex1(key, out int n))
      {
        value = base[n].Item2;
        return true;
      }
      value = default(Type2);
      return false;
    }

    public bool TryGetValue1(Type2 key, out Type1 value)
    {
      if (TryGetIndex2(key, out int n))
      {
        value = base[n].Item1;
        return true;
      }
      value = default(Type1);
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

  }

}
