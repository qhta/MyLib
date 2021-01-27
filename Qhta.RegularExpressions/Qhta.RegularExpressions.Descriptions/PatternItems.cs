using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions.Descriptions
{
  public class PatternItems: IList<PatternItem>
  {
    private List<PatternItem> items = new List<PatternItem>();

    public int IndexOf(PatternItem item)
    {
      return ((IList<PatternItem>)items).IndexOf(item);
    }

    public void Insert(int index, PatternItem item)
    {
      ((IList<PatternItem>)items).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      ((IList<PatternItem>)items).RemoveAt(index);
    }

    public PatternItem this[int index] { get => ((IList<PatternItem>)items)[index]; set => ((IList<PatternItem>)items)[index] = value; }

    public void Add(PatternItem item)
    {
      ((ICollection<PatternItem>)items).Add(item);
    }

    public void Clear()
    {
      ((ICollection<PatternItem>)items).Clear();
    }

    public bool Contains(PatternItem item)
    {
      return ((ICollection<PatternItem>)items).Contains(item);
    }

    public void CopyTo(PatternItem[] array, int arrayIndex)
    {
      ((ICollection<PatternItem>)items).CopyTo(array, arrayIndex);
    }

    public bool Remove(PatternItem item)
    {
      return ((ICollection<PatternItem>)items).Remove(item);
    }

    public int Count => ((ICollection<PatternItem>)items).Count;

    public bool IsReadOnly => ((ICollection<PatternItem>)items).IsReadOnly;

    public IEnumerator<PatternItem> GetEnumerator()
    {
      return ((IEnumerable<PatternItem>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)items).GetEnumerator();
    }

    public override bool Equals(object obj)
    {
      bool result = true;
      if (obj is PatternItems other)
      {
        int n = this.Count;
        if (other.Count < n)
          n = other.Count;
        for (int i = 0; i < n; i++)
        {
          var item1 = this[i];
          var item2 = other[i];
          if (!item1.Equals(item2))
            result = false;
        }
        if (this.Count != other.Count)
          return false;
        return result;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
