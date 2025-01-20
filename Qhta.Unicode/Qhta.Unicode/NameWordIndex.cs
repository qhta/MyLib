using System.Diagnostics;

using Qhta.TextUtils;


namespace Qhta.Unicode;

/// <summary>
/// An index of Unicode character name separate words to code points.
/// </summary>
public class NameWordIndex
{
  private readonly UnicodeData Ucd;
  private readonly Dictionary<string, List<int>> Index = new();

  /// <summary>
  /// This constructor creates a NameStringIndex from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public NameWordIndex(UnicodeData ucd)
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
    for (int i = 0; i < nameItems.Length; i++)
    {
      string nameItem = nameItems[i];
      if (Index.TryGetValue(nameItem, out var value))
      {
        value.Add(codePoint);
      }
      else
      {
        Index.Add(nameItem, [codePoint]);
      }
    }
  }

  public int Count => Index.Count;

  public List<int> this[string name] => Index.TryGetValue(name, out var value) ? value : new List<int>();

  public IEnumerable<KeyValuePair<string, List<int>>> Take(int count) => Index.Take(count);

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
    var words = pattern.Split(['*', ' ', '-'], StringSplitOptions.RemoveEmptyEntries);
    if (Index.TryGetValue(words[0], out var value))
    {
      foreach (var codePoint in value)
      {
        var charInfo = Ucd[codePoint];
        foreach (var name in charInfo.GetAllNames())
        {
          if (name.ContainsWords(pattern))
          {
            result.Add(codePoint);
            break;
          }
        }
      }
    }
    return result;
  }
}
