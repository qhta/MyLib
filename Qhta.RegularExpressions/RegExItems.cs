using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions
{
  public class RegExItems: IList<RegExItem>
  {
    private List<RegExItem> Items = new List<RegExItem>();

    public void Add(RegExItem item)
    {
      ((ICollection<RegExItem>)Items).Add(item);
    }

    public void Clear()
    {
      ((ICollection<RegExItem>)Items).Clear();
    }

    public bool Contains(RegExItem item)
    {
      return ((ICollection<RegExItem>)Items).Contains(item);
    }

    public void CopyTo(RegExItem[] array, int arrayIndex)
    {
      ((ICollection<RegExItem>)Items).CopyTo(array, arrayIndex);
    }

    public bool Remove(RegExItem item)
    {
      return ((ICollection<RegExItem>)Items).Remove(item);
    }

    public int Count => ((ICollection<RegExItem>)Items).Count;

    public bool IsReadOnly => ((ICollection<RegExItem>)Items).IsReadOnly;

    public IEnumerator<RegExItem> GetEnumerator()
    {
      return ((IEnumerable<RegExItem>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)Items).GetEnumerator();
    }

    public int IndexOf(RegExItem item)
    {
      return ((IList<RegExItem>)Items).IndexOf(item);
    }

    public void Insert(int index, RegExItem item)
    {
      ((IList<RegExItem>)Items).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      ((IList<RegExItem>)Items).RemoveAt(index);
    }

    public RegExItem this[int index] { get => ((IList<RegExItem>)Items)[index]; set => ((IList<RegExItem>)Items)[index] = value; }

  }
}
