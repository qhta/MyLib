namespace Qhta.Unicode;

/// <summary>
/// An index of Unicode character name separate words to code points.
/// </summary>
public class NameWordIndex: Dictionary<string, List<CodePoint>>
{
  private UnicodeData Ucd = null!;

  /// <summary>
  /// Initializes index from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public void Initialize(UnicodeData ucd)
  {
    Ucd = ucd;
    foreach (CharInfo charInfo in ucd.Values)
    {
      Add(charInfo.Name, charInfo.CodePoint);
      if (charInfo.OldName!=null)
        Add(charInfo.OldName, charInfo.CodePoint);
    }
  }

  /// <summary>
  /// Add a name words to the index.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="codePoint"></param>
  private void Add(string name, int codePoint)
  {
    string[] nameItems = name.Split([' ', '-']);
    for (int i = 0; i < nameItems.Length; i++)
    {
      string nameItem = nameItems[i];
      if (this.TryGetValue(nameItem, out var value))
      {
        value.Add(codePoint);
      }
      else
      {
        this.Add(nameItem, [codePoint]);
      }
    }
  }

  /// <summary>
  /// Load aliases from CharInfo entries into the index.
  /// </summary>
  /// <param name="ucd"></param>
  public void LoadAliases(UnicodeData ucd)
  {
    foreach (CharInfo charInfo in ucd.Values)
    {
      if (charInfo.Aliases is not null)
      {
        foreach (NameAlias alias in charInfo.Aliases)
        {
          Add(alias.Name, charInfo.CodePoint);
        }
      }
    }
  }

  /// <summary>
  /// Searches for code points by name words.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public IEnumerable<CodePoint> Search(string pattern)
  {
    var result = new SortedSet<CodePoint>();
    var words = pattern.Split(['*', ' ', '-'], StringSplitOptions.RemoveEmptyEntries);
    if (this.TryGetValue(words[0], out var value))
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

  /// <summary>
  /// Check if the index contains a word.
  /// </summary>
  /// <param name="word"></param>
  /// <returns></returns>
  public bool Contains(string word) => this.ContainsKey(word);
}
