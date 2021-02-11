using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RegExTaggerTest
{
  [XmlRoot("TestData")]
  public class TestData: IList<TestItem>
  {
    private List<TestItem> Data = new List<TestItem>();

    public void Add(TestItem item)
    {
      ((ICollection<TestItem>)Data).Add(item);
    }

    public void AddSearchPattern(TestPattern pattern)
    {
      ((ICollection<TestItem>)Data).Add(new TestItem { Search = pattern });
    }

    public void AddReplacePattern(TestPattern pattern)
    {
      ((ICollection<TestItem>)Data).Last().Replace = pattern;
    }

    public void Clear()
    {
      ((ICollection<TestItem>)Data).Clear();
    }

    public bool Contains(TestItem item)
    {
      return ((ICollection<TestItem>)Data).Contains(item);
    }

    public void CopyTo(TestItem[] array, int arrayIndex)
    {
      ((ICollection<TestItem>)Data).CopyTo(array, arrayIndex);
    }

    public bool Remove(TestItem item)
    {
      return ((ICollection<TestItem>)Data).Remove(item);
    }

    public int Count => ((ICollection<TestItem>)Data).Count;

    public bool IsReadOnly => ((ICollection<TestItem>)Data).IsReadOnly;

    public IEnumerator<TestItem> GetEnumerator()
    {
      return ((IEnumerable<TestItem>)Data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)Data).GetEnumerator();
    }

    public int IndexOf(TestItem item)
    {
      return ((IList<TestItem>)Data).IndexOf(item);
    }

    public void Insert(int index, TestItem item)
    {
      ((IList<TestItem>)Data).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      ((IList<TestItem>)Data).RemoveAt(index);
    }

    public TestItem this[int index] { get => ((IList<TestItem>)Data)[index]; set => ((IList<TestItem>)Data)[index] = value; }
  }
}
