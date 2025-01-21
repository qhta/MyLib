using System.Globalization;

using Qhta.TextUtils;

namespace Qhta.Unicode;

/// <summary>
/// Unicode character data singleton class.
/// </summary>
public class UnicodeData : Dictionary<int, CharInfo>
{
  private readonly Dictionary<string, CodePoint> NameIndex = new Dictionary<string, CodePoint>();
  private readonly NameWordIndex NameWordIndex = new NameWordIndex();
  private readonly DecompositionIndex DecompositionIndex = new DecompositionIndex();
  const string ucdFileName = "UnicodeData.txt";
  const string aliasesFileName = "NameAliases.txt";

  /// <summary>
  /// Private constructor for the singleton instance.
  /// </summary>
  private UnicodeData() { }

  /// <summary>
  /// Singleton instance of UnicodeData.
  /// </summary>
  public static UnicodeData Instance
  {
    get
    {
      if (_Instance == null)
      {
        _Instance = new UnicodeData();
        _Instance.Initialize();
      }
      return _Instance;
    }
  }
  private static UnicodeData? _Instance = null!;

  /// <summary>
  /// Initializes the UnicodeData object by reading UnicodeData.txt and NameAliases.txt files.
  /// </summary>
  /// <exception cref="Exception"></exception>
  private void Initialize()
  {
    DownloadFileName(ucdFileName);
    DownloadFileName(aliasesFileName);
    string[] lines = System.IO.File.ReadAllLines(ucdFileName);
    foreach (string line in lines)
    {
      string[] parts = line.Split(';');
      CharInfo charInfo = new CharInfo
      {
        CodePoint = parts[0],
        Name = parts[1]!,
        Category = (Category)Enum.Parse(typeof(Category), parts[2]),
        CCClass = (CCClass)Enum.ToObject(typeof(CCClass), byte.Parse(parts[3])),
        BiDiClass = (BiDiClass)Enum.Parse(typeof(BiDiClass), parts[4]),
        Decomposition = NullOrNonempty(parts[5]),
        DecDigit = NullOrNonempty(parts[6]),
        Digit = NullOrNonempty(parts[7]),
        NumVal = NullOrNonempty(parts[8]),
        BidiMirrored = parts[9] == "Y",
      };
      if (parts[11] != string.Empty)
        charInfo.SimpleUppercaseMapping = parts[11];
      if (parts[12] != string.Empty)
        charInfo.SimpleLowercaseMapping = parts[12];
      if (parts[13] != string.Empty)
        charInfo.SimpleTitlecaseMapping = parts[13];
      Add(charInfo.CodePoint, charInfo);
      if (!NameIndex.ContainsKey(charInfo.Name))
        NameIndex.Add(charInfo.Name, charInfo.CodePoint);
      else if (!charInfo.Name.OriginalName.StartsWith("<"))
        throw new Exception("Duplicate name: " + charInfo.Name);
    }
    NameWordIndex.Initialize(this);
    LoadAliases(aliasesFileName);
    DecompositionIndex.Initialize(this);
  }

  /// <summary>
  /// Returns null if the string is empty, otherwise returns the string.
  /// Needed for the UnicodeData parsing.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private string? NullOrNonempty(string str)
  {
    return str.Length > 0 ? str : null;
  }

  /// <summary>
  /// Loads aliases from a file. Updates NameIndex and NameWordIndex.
  /// </summary>
  /// <param name="filePath"></param>
  /// <returns></returns>
  private int LoadAliases(string filePath)
  {
    int count = 0;
    string[] lines = System.IO.File.ReadAllLines(filePath);
    foreach (string line in lines)
    {
      if (line.StartsWith("#") || line.Trim().Length == 0)
        continue;
      count++;
      string[] parts = line.Split(';');
      int codePoint = int.Parse(parts[0], NumberStyles.HexNumber);
      string alias = parts[1];
      NameAliasType type = (NameAliasType)Enum.Parse(typeof(NameAliasType), parts[2], true);
      if (TryGetValue(codePoint, out CharInfo? charInfo))
      {
        var aliases = charInfo.Aliases ??= new List<NameAlias>();
        aliases.Add(new NameAlias { CodePoint = codePoint, Alias = alias!, Type = type });
      }
      NameIndex.Add(alias, codePoint);
    }
    NameWordIndex.LoadAliases(this);
    return count;
  }

  /// <summary>
  /// Downloads a file from unicode.org if it does not exist in the current directory.
  /// </summary>
  /// <param name="fileName"></param>
  private static void DownloadFileName(string fileName)
  {
    var filePath = Path.Combine(Environment.CurrentDirectory, fileName);
    var url = "https://www.unicode.org/Public/UNIDATA/" + fileName;
    using (HttpClient client = new HttpClient())
    {
      if (!File.Exists(filePath))
      {
        string content = client.GetStringAsync(url).Result;
        System.IO.File.WriteAllText(filePath, content);
      }
    }
  }

  /// <summary>
  /// Searches for code points by name.
  /// If pattern does not contain '*', searches the name index for exact match.
  /// If pattern contains '*', first searches the NameWordIndex for all words.
  /// When no results are found, searches all names using IsLike function on pattern
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public IEnumerable<CodePoint> SearchInNames(string pattern)
  {
    if (!pattern.Contains('*'))
    {
      if (NameIndex.TryGetValue(pattern, out var codePoint))
        return new CodePoint[] { codePoint };
      return Enumerable.Empty<CodePoint>();
    }

    var foundPoints = NameWordIndex.Search(pattern).ToList();
    if (foundPoints.Any())
      return foundPoints;
    var result = new SortedSet<CodePoint>();
    foreach (var charInfo in this.Values)
    {
      if (charInfo.GetAllNames().Any(name => name.OriginalName.IsLike(pattern)))
        result.Add(charInfo.CodePoint);
    }
    return result;
  }

  /// <summary>
  /// Searches for code points by decomposition type
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public IEnumerable<CodePoint> SearchInDecomposition(DecompositionType type)
  {
    return DecompositionIndex[type];
  }
}
