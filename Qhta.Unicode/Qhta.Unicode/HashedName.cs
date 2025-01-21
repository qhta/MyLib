using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qhta.TextUtils;

namespace Qhta.Unicode;

/// <summary>
/// A Unicode name split into separate words.
/// Each word is hashed and stored in a list.
/// </summary>
public class HashedName
{
  /// <summary>
  /// Gets access to the original name.
  /// </summary>
  public string OriginalName { get; set; }
  /// <summary>
  /// Gets access to the word hashes.
  /// </summary>
  public List<int> WordHashes;

  /// <summary>
  /// Initializes a new instance of the HashedName class using original name
  /// </summary>
  /// <param name="name"></param>
  public HashedName(string name)
  {
    OriginalName = name;
    var words = name.Split(new char[] { ' ', '-' });
    WordHashes = new List<int>();
    foreach (var word in words)
    {
      WordHashes.Add(word.GetHashCode());
    }
  }

  /// <summary>
  /// Determines whether the hashed word contains the pattern.
  /// Wildcard '*' is allowed.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public bool ContainsWords(string pattern)
  {
    var parts = pattern.Split(['*'], StringSplitOptions.RemoveEmptyEntries);
    int curIndex = -1;
    if (!pattern.StartsWith("*"))
    {
      if (!ContainsStringAt(0, parts[0]))
        return false;
      curIndex = 0;
    }
    else
    {
      curIndex = Find(0, parts[0]);
    }
    if (curIndex == -1)
      return false;
    for (int i = 1; i < parts.Length; i++)
    {
      curIndex = Find(curIndex + 1, parts[i]);
      if (curIndex == -1)
        return false;
    }
    return true;
  }

  /// <summary>
  /// Find the pattern in the hashed words starting from the index.
  /// </summary>
  /// <param name="fromIndex"></param>
  /// <param name="pattern"></param>
  /// <returns></returns>
  private int Find(int fromIndex, string pattern)
  {
    var words = pattern.Split(new char[] { ' ', '-' });
    var wordHashes = words.Select(w => w.GetHashCode()).ToArray();
    if (words.Length > WordHashes.Count)
    {
      return -1;
    }
    for (int j=fromIndex; j < WordHashes.Count- words.Length+1; j++)
    {
      if (ContainsHashesAt(j, wordHashes))
        return j;
    }
    return -1;
  }

  /// <summary>
  /// Determines whether the original name contains the string at the index.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="str"></param>
  /// <returns></returns>
  private bool ContainsStringAt(int index, string str)
  {
    var words = str.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
    var wordHashes = words.Select(w => w.GetHashCode()).ToArray();
    return ContainsHashesAt(index, wordHashes);
  }

  /// <summary>
  /// Determines whether the hashed words contain the word hashes at the index.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="wordHashes"></param>
  /// <returns></returns>
  private bool ContainsHashesAt(int index, int[] wordHashes)
  {
    for (int i = 0; i < wordHashes.Length; i++)
    {
      if (wordHashes[i].GetHashCode() != WordHashes[i+index])
      {
        return false;
      }
    }
    return true;
  }

  #region OriginalName basic string functions
  /// <summary>
  /// Determines whether the original name contains the pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public bool Contains(string pattern)
  {
    return OriginalName.Contains(pattern);
  }

  /// <summary>
  /// Determines whether the original name starts with the pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public bool StartsWith(string pattern)
  {
    return OriginalName.StartsWith(pattern);
  }

  /// <summary>
  /// Determines whether the original name ends with the pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public bool StartsEnds(string pattern)
  {
    return OriginalName.EndsWith(pattern);
  }

  /// <summary>
  /// Determines whether the original name is like the pattern.
  /// Wildcard '*' is allowed.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public bool IsLike(string pattern)
  {
    return OriginalName.IsLike(pattern);
  }
  #endregion


  #region Implicit conversions
  /// <summary>
  /// Implicit conversion from HashedName to string.
  /// </summary>
  /// <param name="hashedName"></param>
  public static implicit operator string(HashedName hashedName)
  {
    return hashedName.OriginalName;
  }

  /// <summary>
  /// Implicit conversion from string to HashedName.
  /// </summary>
  /// <param name="name"></param>
  public static implicit operator HashedName?(string? name)
  {
    if (name is null)
      return null;
    return new HashedName(name);
  }
  #endregion

  /// <summary>
  /// Returns the original name.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return OriginalName;
  }
}
