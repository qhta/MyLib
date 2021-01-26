using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Qhta.RegularExpressions
{
  public class Inequalities: IList<Inequality>
  {
    private List<Inequality> items = new List<Inequality>();

    public int IndexOf(Inequality item)
    {
      return ((IList<Inequality>)items).IndexOf(item);
    }

    public void Insert(int index, Inequality item)
    {
      ((IList<Inequality>)items).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      ((IList<Inequality>)items).RemoveAt(index);
    }

    public Inequality this[int index] { get => ((IList<Inequality>)items)[index]; set => ((IList<Inequality>)items)[index] = value; }

    public void Add(Inequality item)
    {
      ((ICollection<Inequality>)items).Add(item);
    }

    public void Clear()
    {
      ((ICollection<Inequality>)items).Clear();
    }

    public bool Contains(Inequality item)
    {
      return ((ICollection<Inequality>)items).Contains(item);
    }

    public void CopyTo(Inequality[] array, int arrayIndex)
    {
      ((ICollection<Inequality>)items).CopyTo(array, arrayIndex);
    }

    public bool Remove(Inequality item)
    {
      return ((ICollection<Inequality>)items).Remove(item);
    }

    public int Count => ((ICollection<Inequality>)items).Count;

    public bool IsReadOnly => ((ICollection<Inequality>)items).IsReadOnly;

    public IEnumerator<Inequality> GetEnumerator()
    {
      return ((IEnumerable<Inequality>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)items).GetEnumerator();
    }

    public override string ToString()
    {
      var strings = new List<string>();
      foreach (var item in this)
        strings.Add($"{item.Property}={item.Expected}");
      return String.Join(", ", strings);
    }
  }
}
