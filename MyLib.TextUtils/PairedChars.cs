using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TextUtils
{

  public class CharPair
  {
    public CharPair(char item1, char item2)
    {
      Item1 = item1;
      Item2 = item2;
    }

    public Char Item1 { get; private set; }
    public Char Item2 { get; private set; }
  }

  public class PairedChars: List<CharPair>
  {

    public PairedChars()
    {
    }

    public PairedChars(char[,] chars)
    {
      for (int i = 0; i < chars.GetUpperBound(0); i++)
        Add(new CharPair(chars[i, 0], chars[i, 1]));
    }

    public static implicit operator PairedChars (char[,] value)
    {
      return new PairedChars(value);
    }

    public static implicit operator char[,] (PairedChars value)
    {
      char[,] result = new char[value.Count, 2];
      for (int i = 0; i < value.Count; i++)
      {
        result[i, 0] = value[i].Item1;
        result[i, 1] = value[i].Item2;
      }
      return result;
    }

    public void Add(char item1, char item2)
    {
      Add(new CharPair(item1, item2));
    }

    public void Remove(char item1, char item2)
    {
      CharPair found = this.FirstOrDefault(item => item.Item1 == item1 && item.Item2 == item2);
      if (found != null)
        Remove(found);
    }

    public bool TryGetItem2(char item1, out char item2)
    {
      CharPair found = this.FirstOrDefault(item => item.Item1 == item1);
      if (found != null)
      {
        item2 = found.Item2;
        return true;
      }
      item2 = '\0';
      return false;
    }

    public bool TryGetItem1(char item2, out char item1)
    {
      CharPair found = this.FirstOrDefault(item => item.Item2 == item2);
      if (found != null)
      {
        item1 = found.Item1;
        return true;
      }
      item1 = '\0';
      return false;
    }
  }
}
