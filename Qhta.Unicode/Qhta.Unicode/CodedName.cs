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
public class CodedName
{
  public string PublicName { get; set; }
  public List<int> EncodedName;

  public CodedName(string name)
  {
    PublicName = name;
    var words = name.Split(new char[] { ' ', '-' });
    EncodedName = new List<int>();
    foreach (var word in words)
    {
      EncodedName.Add(word.GetHashCode());
    }
  }

  public bool IsLike(string pattern)
  {
    return PublicName.IsLike(pattern);
  }

  public bool ContainsWords(string pattern)
  {
    var parts = pattern.Split(['*'], StringSplitOptions.RemoveEmptyEntries);
    int curIndex = -1;
    if (!pattern.StartsWith('*'))
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

  public int Find(int fromIndex, string pattern)
  {
    var words = pattern.Split(new char[] { ' ', '-' });
    var wordHashes = words.Select(w => w.GetHashCode()).ToArray();
    if (words.Length > EncodedName.Count)
    {
      return -1;
    }
    for (int j=fromIndex; j < EncodedName.Count- words.Length+1; j++)
    {
      if (ContainsWordsAt(j, wordHashes))
        return j;
    }
    return -1;
  }

  private bool ContainsStringAt(int index, string str)
  {
    var words = str.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
    var wordHashes = words.Select(w => w.GetHashCode()).ToArray();
    return ContainsWordsAt(index, wordHashes);
  }

  private bool ContainsWordsAt(int index, int[] wordHashes)
  {
    for (int i = 0; i < wordHashes.Length; i++)
    {
      if (wordHashes[i].GetHashCode() != EncodedName[i+index])
      {
        return false;
      }
    }
    return true;
  }

  public bool Contains(string pattern)
  {
    return PublicName.Contains(pattern);
  }

  public static implicit operator string(CodedName codedName)
  {
    return codedName.PublicName;
  }

  public static implicit operator CodedName(string name)
  {
    return new CodedName(name);
  }
}
