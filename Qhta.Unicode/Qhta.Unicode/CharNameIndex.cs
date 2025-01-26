using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

using Qhta.Collections;
using Qhta.TextUtils;

namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific char name. Char name is a single-word string.
/// </summary>
public class CharNameIndex : Dictionary<string, CodePoint>
{
  private UnicodeData Ucd = null!;

  /// <summary>
  /// Initializes index.from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public void Initialize(UnicodeData ucd)
  {
    if (Initialized)
      return;
    Ucd = ucd;
    StringReplacements = new Dictionary<string, string>();
    InitializeDictionary(StringReplacements, "GenCharNameStringRepl.txt");
    WordsAbbreviations = new Dictionary<string, string>();
    InitializeDictionary(WordsAbbreviations, "GenCharNameWordAbbr.txt");
    AdjectiveWords = new List<string>();
    InitializeList(AdjectiveWords, "GenCharNameAdjectives.txt");
    Initialized = true;
    CreateShortNamesToFile("CharNames.txt");
  }
  private static bool Initialized = false;

  /// <summary>
  /// Add a code point to the this.
  /// </summary>
  /// <param name="charName">Name is a single-word string</param>
  /// <param name="codePoint"></param>
  private void Add(string charName, int codePoint)
  {
    if (this.TryGetValue(charName, out var value))
    {
      throw new DuplicateNameException($"CharName \"{charName}\" already exists");
    }
    else
    {
      base.Add(charName, codePoint);
    }
  }

  private void CreateShortNamesToFile(string filename)
  {
    GenerateShortNames();
    using (var writer = File.CreateText(filename))
    {
      foreach (var item in this.OrderBy(item => (int)item.Value))
      {
        writer.WriteLine($"{item.Value};{item.Key}");
      }
    }
  }

  private void GenerateShortNames()
  {
    foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
    {
      if (!IsAccepted(charInfo))
        continue;
      TryAddShortName(charInfo);
    }
  }

  private bool TryAddShortName(CharInfo charInfo)
  {
    var charName = GenerateShortName(charInfo);
    if (charName is null)
      return false;

    if (this.TryGetValue(charName, out var existingCodePoint))
    {
      const int maxAlternative = 2;
      for (int alternative = 1; alternative <= maxAlternative; alternative++)
      {
        var altName2 = GenerateShortName(charInfo, alternative);
        if (altName2 != null && altName2 != charName)
        {
          this.Add(altName2, charInfo.CodePoint);
          break;
        }
        else
        {
          var charInfo1 = Ucd[existingCodePoint];
          var altName1 = GenerateShortName(charInfo1, alternative);
          if (altName1 != null && altName1 != charName)
          {
            this.Remove(charName);
            Add(altName1, existingCodePoint);
            Add(charName, charInfo.CodePoint);
            break;
          }
          else if (alternative == maxAlternative)
          {
            if (charName.StartsWith("hangjungseong"))
            {
              charName = charName.Insert("hangjungseong".Length, "2");
              Add(charName, charInfo.CodePoint);
              break;
            }
            throw new DuplicateNameException($"Conflict between code points {charInfo1.CodePoint} and {charInfo.CodePoint}. CharName \"{charName}\" already exists");
          }
        }
      }
    }
    else
      Add(charName, charInfo.CodePoint);
    return true;
  }

  /// <summary>
  /// Check if a character is accepted in this name generation process.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <returns></returns>
  public bool IsAccepted(CharInfo charInfo)
  {
    if (charInfo.CodePoint == 0x2393)
      Debug.Assert(true);

    if (charInfo.CodePoint > 0xFFFF)
      return false;

    string longName = charInfo.Name.ToString().ToUpper();

    if (longName.StartsWith("MODIFIER LETTER ") && !longName.Contains("CHINESE"))
      return true;

    if (charInfo.CodePoint >= 0xFE20 && charInfo.CodePoint <= 0xFE2F)
      return true;

    if (longName.StartsWith("GREEK") && !GreekAlphabet.ContainsKey(charInfo.CodePoint))
      return false;

    if (longName.StartsWith("HEBREW") && !HebrewAlphabet.ContainsKey(charInfo.CodePoint))
      return false;

    if (charInfo.CodePoint >= 0xA000 && !longName.Contains("LIGATURE"))
      return false;

    if (longName.Contains("CJK ")
        || longName.Contains("IDEOGRAPH")
        || longName.StartsWith("VARIATION")
        || longName.StartsWith("HEXAGRAM")
        || longName.StartsWith("KANGXI")
        || longName.StartsWith("PHAGS-PA")
        || longName.StartsWith("RUSSIAN")
        || longName.Contains("EGYPT"))
      return false;

    if (longName.StartsWith("APL FUNCTIONAL SYMBOL"))
      return false;

    if (longName.StartsWith("SQUARE "))
    {
      var decomposition = charInfo.Decomposition;
      if (decomposition is not null)
      {
        var hasAscii = false;
        foreach (var item in decomposition.CodePoints)
          if (item.Value < 0x80)
            hasAscii = true;
        if (!hasAscii)
          return false;
      }

    }

    foreach (var entry in ScriptCodes.UcdScriptNames)
    {
      var scriptName = entry.Item2;
      var words = scriptName.Split(' ');
      string key = words[0];
      if (key is "Latin" or "Greek" or "Hebrew")
        continue;

      if (longName.Contains(scriptName.ToUpper()))
        return false;
    }
    return true;
  }

  /// <summary>
  /// Create a short name for a character.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  public string? GenerateShortName(CharInfo charInfo, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x2393)
      Debug.Assert(true);


    string? charName = null;
    string longName = charInfo.Name.ToString().ToUpper();

    if (charInfo.CodePoint == 0x007F)
      charName = "DEL";
    else if (GreekAlphabet.TryGetValue(charInfo.CodePoint, out var greekLetterName))
      charName = greekLetterName;
    else if (HebrewAlphabet.TryGetValue(charInfo.CodePoint, out var hebrewLetterName))
      charName = hebrewLetterName;
    else if (charInfo.Category == UcdCategory.Cc || charInfo.CodePoint == 0x020 || longName.StartsWith("<"))
      charName = CreateAbbreviationCharName(charInfo);
    else if (charInfo.Category.ToString()[0] == 'Z')
      charName = CreateShortenName(charInfo, alternative);
    else if (charInfo.Decomposition?.Type == DecompositionType.Super || charInfo.Decomposition?.Type == DecompositionType.Sub)
      charName = CreateDecompositionName(charInfo, alternative);
    else if (charInfo.Category == UcdCategory.Mn || longName.StartsWith("MODIFIER LETTER ") || longName.StartsWith("COMBINING "))
      charName = CreateDiacriticCharName(charInfo, alternative);
    else if (longName.StartsWith("VULGAR FRACTION "))
      charName = CreateVulgarFractionName(longName, alternative);
    else if (longName.StartsWith("PRESENTATION FORM "))
      charName = CreatePresentationFormName(longName, alternative);
    else if (longName.StartsWith("HEXAGRAM FOR "))
      charName = CreateHexagramForName(longName, alternative);
    else if (longName.StartsWith("FULLWIDTH "))
      charName = CreateFullWidthName(longName, alternative);
    else if (longName.StartsWith("HALFWIDTH "))
      charName = CreateHalfWidthName(longName, alternative);
    else if (charInfo.Category.ToString()[0] == 'L')
      charName = CreateLetterName(longName, alternative);
    else
      charName = CreateShortenName(charInfo, alternative);
    //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
    return charName;
  }

  private string? CreateAbbreviationCharName(CharInfo charInfo)
  {
    string? charName = null;
    if (charInfo.Aliases != null)
    {
      var alias = charInfo.Aliases.FirstOrDefault(item => item.Type == NameAliasType.Abbreviation);
      charName = alias?.Name!;
      //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
      if (alias != null)
        return alias!.Name;
    }
    var longName = charInfo.Name.ToString();
    if (longName.StartsWith("<") && longName.EndsWith(">"))
      return CreateShortenLetterName(longName.Substring(1, longName.Length - 2).ToUpper());
    return charName;
  }

  /// <summary>
  /// Create a short name for a character decomposition.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateDecompositionName(CharInfo charInfo, int alternative = 0)
  {
    var decomposition = charInfo.Decomposition;
    if (decomposition is null)
      return CreateShortenName(charInfo);
    var sb = new StringBuilder();
    foreach (var cp in decomposition.CodePoints)
    {
      var item = Ucd[cp.Value];
      var str = GenerateShortName(item, alternative);
      if (decomposition.Type == DecompositionType.Super)
        str = "sup" + str;
      else
      if (decomposition.Type == DecompositionType.Sub)
        str = "sub" + str;
      else
        str = decomposition.Type.ToString().ToLower() + str;
      sb.Append(str);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Create a short name for a long character name.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateShortenName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    longName = ReplaceStrings(longName, alternative);
    var sb = new StringBuilder();
    if (charInfo.Category.ToString() == "Sk")
      longName = longName.Replace(" AND ", " ");
    var words = longName.Split([' ', ',', '-', '_'], StringSplitOptions.RemoveEmptyEntries).ToList();
    if (words.Count == 1)
    {
      if (StringReplacements.TryGetValue(words[0], out var repl))
        return repl;
      return words[0].ToLower();
    }

    for (int i = words.Count - 1; i >= 0; i--)
    {
      var word = words[i];
      if (TryFindWordToRemove(word, alternative))
        words.RemoveAt(i);
    }

    for (int i = 0; i < words.Count; i++)
    {
      var word = words[i];

      if (TryFindScriptName(word, alternative, out var scCode))
      {
        scCode = scCode.ToLower();
        sb.Append(scCode);
        continue;
      }

      if (word == "LETTER" || word == "LIGATURE" || word == "DIGRAPH")
        continue;

      if (charInfo.Category == UcdCategory.Sc && i == 0)
      {
        sb.Append(word.ToLower());
        continue;
      }

      if (word == "EN" || word == "EM")
      {
        if (i == 0)
          sb.Append(word.ToLower());
        else
          sb.Append(char.ToLower(word[1]));
      }
      else
      if (WordsAbbreviations.TryGetValue(word, out var shortWord))
        sb.Append(shortWord);
      else
        //if (i < words.Count - 1)
        //  sb.Append(char.ToLower(word[0]));
        //else
        sb.Append(word.ToLower());
    }
    return sb.ToString();
  }

  private string CreateLetterName(string longName, int alternative = 0)
  {
    return CreateShortenLetterName(longName, alternative);
  }

  private string CreateDiacriticCharName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    var mod = longName.IndexOf("MODIFIER LETTER ");
    var comb = longName.IndexOf("COMBINING ");
    if (mod >= 0)
      longName = longName.Replace("MODIFIER LETTER ", "");
    else if (comb >= 0)
      longName = longName.Replace("COMBINING ", "");
    var result = CreateShortenLetterName(longName, alternative);
    if (mod >= 0)
    {
      if (result.EndsWith("tonebar"))
        result = result.Substring(0, result.Length - 3);
      if (alternative == 0)
        return result;
      return "mod" + result;
    }

    if (comb >= 0)
    {
      if (alternative == 0)
      {
        if (longName.Contains("LIGATURE"))
          return "lig" + result;
        if (longName.Contains("ABOVE") || longName.EndsWith("BELOW") || longName.EndsWith("OVERLAY") || longName.EndsWith("OVERLINE") || longName.EndsWith("BREVE"))
          return result;
        if (longName.EndsWith("BREVE") || longName.EndsWith("TILDE") || longName.EndsWith("MACRON") || longName.EndsWith("CARON") || longName.EndsWith("DIERESIS"))
        {
          if (charInfo.Decomposition?.Type == DecompositionType.Compat &&
              charInfo.Decomposition?.CodePoints.FirstOrDefault() == 0x0020)
            return "mod" + result;
          return result + "accent";
        }
        if (longName.Contains("CANDRABINDU") || longName.Contains("HORN"))
          return result;
        if (longName.Split(' ').Last().Length==1)
          return "zw" + result + "accent";
      }
      else if (alternative == 1)
      {
        if (longName.EndsWith("ACCENT"))
          return result;
      }
      result = "zw" + result;
    }
    return result;
  }

  private string CreateVulgarFractionName(string longName, int alternative = 0)
  {
    longName = longName.Replace("VULGAR FRACTION ", "");
    return CreateShortenLetterName(longName, alternative);
  }

  private string CreatePresentationFormName(string longName, int alternative = 0)
  {
    longName = longName.Replace("PRESENTATION FORM FOR VERTICAL ", "");
    return "vert" + CreateShortenLetterName(longName, alternative);
  }


  private string CreateHexagramForName(string longName, int alternative = 0)
  {
    longName = longName.Replace("HEXAGRAM FOR ", "");
    return "hexagram" + CreateShortenLetterName(longName, alternative);
  }

  private string? CreateFullWidthName(string longName, int alternative = 0)
  {
    longName = longName.Replace("FULLWIDTH ", "");
    return "wide" + CreateShortenLetterName(longName, alternative);
  }

  private string? CreateHalfWidthName(string longName, int alternative = 0)
  {
    longName = longName.Replace("HALFWIDTH ", "");
    return "narrow" + CreateShortenLetterName(longName, alternative);
  }
  private string CreateShortenLetterName(string longName, int alternative = 0)
  {
    longName = ReplaceStrings(longName, alternative);
    var sb = new StringBuilder();
    var ss = longName.Split([' ', ',', '-']);
    var wasCapital = false;
    var wasSmall = false;
    var wasWith = false;
    for (int i = 0; i < ss.Length; i++)
    {
      var word = ss[i];//.Replace('-', '_');

      if (word != "YI" && TryFindScriptName(word, alternative, out var scCode))
      {
        scCode = scCode.ToLower();
        sb.Append(scCode);
        continue;
      }
      if (word == "LETTER" || word == "LIGATURE" || word == "DIGRAPH" || word == "SYMBOL")
        continue;

      if (word == "WITH")
      {
        wasWith = true;
        continue;
      }
      if (wasWith && word == "AND")
        continue;
      if (TryFindWordToRemove(word, alternative))
        continue;

      if (word == "CAPITAL" || word == "CAPITAL_LETTER" || word == "CAPITAL_LIGATURE")
      {
        wasCapital = true;
        continue;
      }

      if (word == "SMALL_LETTER" || word == "SMALL_LIGATURE")
      {
        wasSmall = true;
        continue;
      }

      if (word.Contains('_') && word.Contains("SMALL") && word.Contains("CAPITAL"))
      {
        wasCapital = true;
        wasSmall = true;
        continue;
      }

      if (WordsAbbreviations.TryGetValue(word, out var replacement))
      {
        sb.Append(replacement);
        continue;
      }

      if (AdjectiveWords.Contains(word))
      {
        sb.Append(word.ToLower());
        continue;
      }

      if (wasCapital)
      {
        if (wasSmall)
          sb.Append("smcap");
        if (word.Length == 2)
          sb.Append(word.ToUpper());
        else
        {
          if (alternative > 0)
            sb.Append("cap");
          sb.Append(word.TitleCase());
        }
        wasCapital = false;
      }
      else
      if (wasSmall)
      {
        if (alternative > 0)
          if (WordsAbbreviations.TryGetValue("SMALL", out var abbr))
            sb.Append(abbr);
          else
            sb.Append("small");
        sb.Append(word.ToLower());
      }
      else
        sb.Append(word.ToLower());
      wasSmall = false;
    }
    return sb.ToString();
  }

  private static bool TryFindWordToRemove(string word, int alternative)
  {
    if (WordsToRemove.TryGetValue(word, out int val))
      if (alternative <= val)
        return true;
    return false;
  }

  private static string ReplaceStrings(string longName, int alternative)
  {
    foreach (var entry in StringReplacements)
      longName = longName.Replace(entry.Key, entry.Value);

    return longName;
  }

  private static bool TryFindScriptName(string word, int alternative, out string scCode)
  {
    if (word == "CANADIAN")
      word = "Canadian Aboriginal";
    else
      word = word.Replace('_', ' ').TitleCase(true);
    if (ScriptCodes.UcdScriptNames.TryGetValue1(word, out scCode))
    {
      if (alternative == 0)
        scCode = "";
      else
      {
        if (WordsAbbreviations.TryGetValue(word.ToUpper(), out var abbr))
          scCode = abbr;
      }
      return true;
    }
    return false;
  }

  private static void InitializeDictionary(Dictionary<string, string> dictionary, string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split([';', ',']);
          if (parts.Length == 2)
          {
            dictionary.Add(parts[0], parts[1]);
          }
        }
      }
    }
  }

  private static void InitializeList(List<string> list, string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          list.Add(line.Trim());
        }
      }
    }
  }


  private static Dictionary<string, string> StringReplacements = null!;
  private static Dictionary<string, string> WordsAbbreviations = null!;
  private static List<string> AdjectiveWords = null!;

  private static readonly Dictionary<string, int> WordsToRemove = new()
  {
    {"SIGN", 1},
    {"MARK", 1},
    {"DIGIT", 1},
    {"SYMBOL",0},
    {"WITH", 0 },
    {"COMMERCIAL", 0},
    {"THAN", 0},
    {"LOGICAL", 0},
    {"FOR", 0},
    {"FORM", 0},
    {"THE", 0},
    {"TO", 0}
  };

  private static readonly BiDiDictionary<int, string> GreekAlphabet = new()
  {
    { 0x0391, "Alpha" },
    { 0x0392, "Beta" },
    { 0x0393, "Gamma" },
    { 0x0394, "Delta" },
    { 0x0395, "Epsilon" },
    { 0x0396, "Zeta" },
    { 0x0397, "Eta" },
    { 0x0398, "Theta" },
    { 0x0399, "Iota" },
    { 0x039A, "Kappa" },
    { 0x039B, "Lamda" },
    { 0x039C, "Mu" },
    { 0x039D, "Nu" },
    { 0x039E, "Xi" },
    { 0x039F, "Omicron" },
    { 0x03A0, "Pi" },
    { 0x03A1, "Rho" },
    { 0x03A3, "Sigma" },
    { 0x03A4, "Tau" },
    { 0x03A5, "Upsilon" },
    { 0x03A6, "Phi" },
    { 0x03A7, "Chi" },
    { 0x03A8, "Psi" },
    { 0x03A9, "Omega"},

    { 0x3B1, "alpha" },
    { 0x3B2, "beta" },
    { 0x3B3, "gamma" },
    { 0x3B4, "delta" },
    { 0x3B5, "epsilon" },
    { 0x3B6, "zeta" },
    { 0x3B7, "eta" },
    { 0x3B8, "theta" },
    { 0x3B9, "iota" },
    { 0x3BA, "kappa" },
    { 0x3BB, "lamda" },
    { 0x3BC, "mu" },
    { 0x3BD, "nu" },
    { 0x3BE, "xi" },
    { 0x3BF, "omicron" },
    { 0x3C0, "pi" },
    { 0x3C1, "rho" },
    { 0x3C2, "finsigma" },
    { 0x3C3, "sigma" },
    { 0x3C4, "tau" },
    { 0x3C5, "upsilon" },
    { 0x3C6, "phi" },
    { 0x3C7, "chi" },
    { 0x3C8, "psi" },
    { 0x3C9, "omega"},
  };

  private static readonly BiDiDictionary<int, string> HebrewAlphabet = new()
  {
    { 0x5D0, "Alef" },
    { 0x5D1, "Bet" },
    { 0x5D2, "Gimel" },
    { 0x5D3, "Dalet" },
    { 0x5D4, "He" },
    { 0x5D5, "Vav" },
    { 0x5D6, "Zayin" },
    { 0x5D7, "Het" },
    { 0x5D8, "Tet" },
    { 0x5D9, "Yod" },
    { 0x5DA, "finKaf" },
    { 0x5DB, "Kaf" },
    { 0x5DC, "Lamed" },
    { 0x5DD, "finMem" },
    { 0x5DE, "Mem" },
    { 0x5DF, "finNun" },
    { 0x5E0, "Nun" },
    { 0x5E1, "Samekh" },
    { 0x5E2, "Ayin" },
    { 0x5E3, "finPe" },
    { 0x5E4, "Pe" },
    { 0x5E5, "finTsadi" },
    { 0x5E6, "Tsadi" },
    { 0x5E7, "Qof" },
    { 0x5E8, "Resh" },
    { 0x5E9, "Shin" },
    { 0x5EA, "Tav" },
  };

}
