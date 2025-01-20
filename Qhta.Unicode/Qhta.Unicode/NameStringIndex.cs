using System.Diagnostics;
using Qhta.TextUtils;


namespace Qhta.Unicode;

/// <summary>
/// An index of Unicode character name separate words to code points.
/// </summary>
public class NameStringIndex : Dictionary<string, List<int>>
{
  private readonly UnicodeData Ucd;

  /// <summary>
  /// This constructor creates a NameStringIndex from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public NameStringIndex(UnicodeData ucd)
  {
    Ucd = ucd;
    foreach (CharInfo charInfo in ucd.Values)
    {
      Add(charInfo.Name, charInfo.CodePoint);
    }
  }

  private void Add(string name, int codePoint)
  {
    string[] nameItems = name.Split([' ', '-']);
    for (int i = 0; i<nameItems.Length; i++)
    {
      string nameItem = nameItems[i];
      if (ContainsKey(nameItem))
      {
        this[nameItem].Add(codePoint);
      }
      else
      {
       base.Add(nameItem, [codePoint]);
      }
    }
  }

  public void LoadAliases(UnicodeData ucd)
  {
    foreach (CharInfo charInfo in ucd.Values)
    {
      if (charInfo.Aliases is not null)
      {
        foreach (NameAlias alias in charInfo.Aliases)
        {
          Add(alias.Alias, charInfo.CodePoint);
        }
      }
    }
  }

  public IEnumerable<int> Search(string pattern)
  {
    var result = new SortedSet<int>();
    var parts = pattern.Split(['*', ' ', '-'], StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < parts.Length; i++)
    {
      string part = parts[i];
      if (ContainsKey(part))
      {
        foreach (var codePoint in this[part])
        {
          var charInfo = Ucd[codePoint];
          if (charInfo.Name.IsLike(pattern))
            result.Add(codePoint);
          if (charInfo.Aliases!=null)
          {
            foreach (var alias in charInfo.Aliases)
            {
              if (alias.Alias.IsLike(pattern))
                result.Add(codePoint);
            }
          }
        }
      }
    }
    return result;
  }
}
