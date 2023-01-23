using System;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.TextUtils
{

  /// <summary>
  /// A pair of characters. May be used to parse text.
  /// </summary>
  public class CharPair
  {
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="item1">first char</param>
    /// <param name="item2">second char</param>
    public CharPair(char item1, char item2)
    {
      Item1 = item1;
      Item2 = item2;
    }

    /// <summary>
    /// First char.
    /// </summary>
    public Char Item1 { get; private set; }
    /// <summary>
    /// Second char.
    /// </summary>
    public Char Item2 { get; private set; }
  }

  /// <summary>
  /// A list of CharPairs
  /// </summary>
  public class PairedChars: List<CharPair>
  {
    /// <summary>
    /// Default constructor
    /// </summary>
    public PairedChars()
    {
    }

    /// <summary>
    /// Constructor based on 2-dimension array of chars.
    /// </summary>
    /// <param name="chars"></param>
    public PairedChars(char[,] chars)
    {
      for (int i = 0; i < chars.GetUpperBound(0); i++)
        Add(new CharPair(chars[i, 0], chars[i, 1]));
    }

    /// <summary>
    /// Implicit conversion from a 2-dimension array of chars.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator PairedChars (char[,] value)
    {
      return new PairedChars(value);
    }

    /// <summary>
    /// Implicit conversion to a 2-dimension array of chars
    /// </summary>
    /// <param name="value"></param>
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

    /// <summary>
    /// Add a pair of chars
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    public void Add(char item1, char item2)
    {
      Add(new CharPair(item1, item2));
    }

    /// <summary>
    /// Remove a pair of chars.
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    public void Remove(char item1, char item2)
    {
      CharPair? found = this.FirstOrDefault(item => item.Item1 == item1 && item.Item2 == item2);
      if (found != null)
        Remove(found);
    }

    /// <summary>
    /// Try to get first char when second is known.
    /// </summary>
    /// <param name="item2"></param>
    /// <param name="item1"></param>
    /// <returns></returns>
    public bool TryGetItem2(char item1, out char item2)
    {
      CharPair? found = this.FirstOrDefault(item => item.Item1 == item1);
      if (found != null)
      {
        item2 = found.Item2;
        return true;
      }
      item2 = '\0';
      return false;
    }

    /// <summary>
    /// Try to get second char when first is known.
    /// </summary>
    /// <param name="item2"></param>
    /// <param name="item1"></param>
    /// <returns></returns>
    public bool TryGetItem1(char item2, out char item1)
    {
      CharPair? found = this.FirstOrDefault(item => item.Item2 == item2);
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
