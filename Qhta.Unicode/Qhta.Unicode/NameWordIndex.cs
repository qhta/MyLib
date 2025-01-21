namespace Qhta.Unicode;

/// <summary>
/// An index of Unicode character name separate words to code points.
/// </summary>
public class NameWordIndex
{
  private UnicodeData Ucd = null!;
  private readonly Dictionary<string, List<CodePoint>> Index = new();

  /// <summary>
  /// Initializes NameStringIndex from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public void Initialize(UnicodeData ucd)
  {
    Ucd = ucd;
    foreach (CharInfo charInfo in ucd.Values)
    {
      Add(charInfo.Name, charInfo.CodePoint);
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

  /// <summary>
  /// Number of words in the index.
  /// </summary>
  public int Count => Index.Count;

  /// <summary>
  /// Get the code points for a name word.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public List<CodePoint> this[string name] => Index.TryGetValue(name, out var value) ? value : new List<CodePoint>();

  /// <summary>
  /// Get the first count words in the index.
  /// </summary>
  /// <param name="count"></param>
  /// <returns></returns>
  public IEnumerable<KeyValuePair<string, List<CodePoint>>> Take(int count) => Index.Take(count);

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
          Add(alias.Alias, charInfo.CodePoint);
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

  /// <summary>
  /// Check if the index contains a word.
  /// </summary>
  /// <param name="word"></param>
  /// <returns></returns>
  public bool Contains(string word) => Index.ContainsKey(word);
}
