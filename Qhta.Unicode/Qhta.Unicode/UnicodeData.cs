using System.Diagnostics;
using System.Globalization;
using Qhta.Collections;
using Qhta.TextUtils;

namespace Qhta.Unicode;

/// <summary>
/// Unicode character data singleton class.
/// </summary>
public class UnicodeData : Dictionary<int, CharInfo>
{
  private readonly Dictionary<string, CodePoint> NameIndex = new Dictionary<string, CodePoint>();
  private readonly NameWordIndex NameWordIndex = new NameWordIndex();
  private readonly CategoryIndex CategoryIndex = new CategoryIndex();
  private readonly DecompositionIndex DecompositionIndex = new DecompositionIndex();
  private readonly ScriptIndex ScriptIndex = new ScriptIndex();

  const string UcdFileName = "UnicodeData.txt";
  const string AliasesFileName = "NameAliases.txt";
  const string ScriptsFileName = "Scripts.txt";

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
    var taskA = DownloadFileNameAsync(UcdFileName);
    var taskB = DownloadFileNameAsync(AliasesFileName);
    var taskC = DownloadFileNameAsync(ScriptsFileName);
    taskA.Wait();
    string[] lines = System.IO.File.ReadAllLines(UcdFileName);
    foreach (string line in lines)
    {
      string[] parts = line.Split(';');
      CharInfo charInfo = new CharInfo
      {
        CodePoint = parts[0],
        Name = parts[1]!,
        Category = (UcdCategory)Enum.Parse(typeof(UcdCategory), parts[2]),
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
    taskB.Wait();
    LoadAliases(AliasesFileName);
    CategoryIndex.Initialize(this);
    DecompositionIndex.Initialize(this);
    taskC.Wait();
    LoadScripts(ScriptsFileName);
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
  /// Loads aliases from a file. Updates NameIndex and NameWordIndex.
  /// </summary>
  /// <param name="filePath"></param>
  /// <returns></returns>
  private int LoadScripts(string filePath)
  {
    int count = 0;
    string[] lines = System.IO.File.ReadAllLines(filePath);
    foreach (string line in lines)
    {
      if (line.StartsWith("#") || line.Trim().Length == 0)
        continue;
      var k = line.IndexOf('#');
      var str = (k >= 0) ? line.Substring(0, k) : line;
      string[] parts = str.Split(';');
      str = parts[0].Trim();
      k = str.IndexOf("..");
      var str1 = (k >= 0) ? str.Substring(0, k) : str;
      var str2 = (k >= 0) ? str.Substring(k + 2) : string.Empty;
      var codePoint1 = int.Parse(str1, NumberStyles.HexNumber);
      var codePoint2 = (str2.Length > 0) ? int.Parse(str2, NumberStyles.HexNumber) : codePoint1;
      for (int cp = codePoint1; cp <= codePoint2; cp++)
      {
        if (TryGetValue(cp, out CharInfo? charInfo))
        {
          str = parts[1].Trim().Replace('_',' ');
          if (ScriptCodes.UcdScriptNames.TryGetValue1(str, out string script))
          {
            charInfo.Script = script;
            count++;
          }
          else
            throw new Exception($"Invalid script name: \"{str}\"");
        }
      }
    }
    ScriptIndex.Initialize(this);
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
  /// Downloads a file from unicode.org if it does not exist in the current directory.
  /// </summary>
  /// <param name="fileName"></param>
  private static async Task DownloadFileNameAsync(string fileName)
  {
    var filePath = Path.Combine(Environment.CurrentDirectory, fileName);
    var url = "https://www.unicode.org/Public/UNIDATA/" + fileName;
    using (HttpClient client = new HttpClient())
    {
      if (!File.Exists(filePath))
      {
        string content = await client.GetStringAsync(url);
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
  /// Searches for code points by categories.
  /// Pattern can be a list of pattern items separated by '|'.
  /// Pattern is a two-letter string or one-letter string.
  /// If a pattern is a single letter or a two-letter string and the second letter is '*', all categories starting with the first letter are searched.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public IEnumerable<CodePoint> SearchInCategories(string pattern)
  {
    var patterns = pattern.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
    List<CodePoint> result = new();
    foreach (var pat in patterns)
    {
      result.AddRange(SearchInCategories2(pat));
    }
    return result;
  }

  /// <summary>
  /// Searches for code points by categories. Pattern is a two-letter string and can contain '*' as the second char.
  /// Pattern can also be a single letter, in which case all categories starting with that letter are searched
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns></returns>
  private IEnumerable<CodePoint> SearchInCategories2(string pattern)
  {
    pattern = pattern.Trim();
    if (pattern.Length==1)
      pattern = pattern + "*";
    if (pattern.Length!=2)
      throw new Exception("Invalid category pattern: " + pattern);

    if (pattern[1] == '*')
    {
      UcdCategory[] categories = pattern[0] switch
      {
        'C' => new UcdCategory[] { UcdCategory.Cc, UcdCategory.Cf, UcdCategory.Co, UcdCategory.Cs },
        'L' => new UcdCategory[] { UcdCategory.Ll, UcdCategory.Lm, UcdCategory.Lo, UcdCategory.Lt, UcdCategory.Lu },
        'M' => new UcdCategory[] { UcdCategory.Mc, UcdCategory.Me, UcdCategory.Mn },
        'N' => new UcdCategory[] { UcdCategory.Nd, UcdCategory.Nl, UcdCategory.No },
        'P' => new UcdCategory[]
        {
          UcdCategory.Pc, UcdCategory.Pd, UcdCategory.Pe, UcdCategory.Pf, UcdCategory.Pi, UcdCategory.Po, UcdCategory.Ps
        },
        'S' => new UcdCategory[] { UcdCategory.Sc, UcdCategory.Sk, UcdCategory.Sm, UcdCategory.So },
        'Z' => new UcdCategory[] { UcdCategory.Zl, UcdCategory.Zp, UcdCategory.Zs },
        _ => throw new Exception("Invalid category pattern: " + pattern),
      };
      List<CodePoint> result = new();
      foreach (var category in categories)
      {
        result.AddRange(CategoryIndex[category]);
      }
      return result;
    }
    else
    {
      var category = (UcdCategory)Enum.Parse(typeof(UcdCategory), pattern);
      return this.Values.Where(charInfo => charInfo.Category == category).Select(charInfo => charInfo.CodePoint);
    }
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

  /// <summary>
  /// Searches for code points by script code
  /// </summary>
  /// <param name="script"></param>
  /// <returns></returns>
  public IEnumerable<CodePoint> SearchInScripts(string script)
  {
    return ScriptIndex[script];
  }
}
