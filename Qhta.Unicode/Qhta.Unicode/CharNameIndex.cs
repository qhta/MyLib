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
public class CharNameIndex : BiDiDictionary<CodePoint, string>
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

    InitializeCodePointDictionary(KnownCharNames, "KnownCharNames.txt");
    InitializeCodePointDictionary(GreekAlphabet, "GreekAlphabet.txt");
    InitializeCodePointDictionary(HebrewAlphabet, "HebrewAlphabet.txt");
    InitializeStringDictionary(StringReplacements, "StringRepl.txt");
    InitializeStringDictionary(WordsAbbreviations, "WordAbbr.txt");
    InitializeStringList(AdjectiveWords, "Adjectives.txt");
    InitializeStringIntDictionary(WordsToRemove, "WordsToRemove.txt");
    CreateCharNamesToFile("CharNames.txt");
    Initialized = true;
  }
  private static bool Initialized = false;

  /// <summary>
  /// Add a code point to the this.
  /// </summary>
  /// <param name="charName">Name is a single-word string</param>
  /// <param name="codePoint"></param>
  private void AddCheck(int codePoint, string charName)
  {
    if (this.TryGetValue1(charName, out var value))
    {
      throw new DuplicateNameException($"CharName \"{charName}\" already exists");
    }
    else
    {
      base.Add(codePoint, charName);
    }
  }

  private void CreateCharNamesToFile(string filename)
  {
    CreateCharNames();
    using (var writer = File.CreateText(filename))
    {
      foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
      {
        if (TryGetValue(charInfo.CodePoint, out var charName))
          writer.WriteLine($"{charInfo.CodePoint};{charName}");
      }
    }
  }

  private void CreateCharNames()
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

    if (this.TryGetValue1(charName, out var existingCodePoint))
    {
      if (existingCodePoint == 0x2004)
        Debug.Assert(true);
      const int maxAlternative = 2;
      for (int alternative = 1; alternative <= maxAlternative; alternative++)
      {
        var altName2 = GenerateShortName(charInfo, alternative);
        if (altName2 != null && altName2 != charName)
        {
          this.AddCheck(charInfo.CodePoint, altName2);
          break;
        }
        else
        {
          this.TryGetValue1(charName, out existingCodePoint);
          var charInfo1 = Ucd[existingCodePoint];
          var altName1 = GenerateShortName(charInfo1, alternative);
          if (altName1 != null && altName1 != charName)
          {
            this.Remove(existingCodePoint);
            AddCheck(existingCodePoint, altName1);
            AddCheck(charInfo.CodePoint, charName);
            break;
          }
          else if (alternative == maxAlternative)
          {
            if (charName.StartsWith("hangjungseong"))
            {
              charName = charName.Insert("hangjungseong".Length, "2");
              AddCheck(charInfo.CodePoint, charName);
              break;
            }
            throw new DuplicateNameException($"Conflict between code points {charInfo1.CodePoint} and {charInfo.CodePoint}. CharName \"{charName}\" already exists");
          }
        }
      }
    }
    else
      AddCheck(charInfo.CodePoint, charName);
    return true;
  }

  /// <summary>
  /// Check if a character is accepted in this name generation process.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <returns></returns>
  public bool IsAccepted(CharInfo charInfo)
  {
    if (charInfo.CodePoint == 0x2004)
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
    if (charInfo.CodePoint == 0x2004)
      Debug.Assert(true);


    string? charName = null;
    string longName = charInfo.Name.ToString().ToUpper();

    if (KnownCharNames.TryGetValue(charInfo.CodePoint, out var knownCharName))
      charName = knownCharName;
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
    if (decomposition == null)
      return CreateShortenName(charInfo);
    var sb = new StringBuilder();
    foreach (var cp in decomposition.CodePoints)
    {
      var item = Ucd[cp.Value];
      var str = GenerateShortName(item, alternative);
      sb.Append(str);
    }
    var sequence = sb.ToString();
    var funcName = (decomposition.Type == DecompositionType.Super) ? "sup" : decomposition.Type.ToString()!.ToLower();
    return funcName + sequence;
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
      //if (alternative == 0)
      //  return result;
      return result+"mod";
    }

    if (comb >= 0)
    {
      if (alternative == 0)
      {
        if (longName.Contains("LIGATURE"))
          return "zw" + result;
        if (longName.Contains("ABOVE") || longName.EndsWith("BELOW") || longName.EndsWith("OVERLAY") || longName.EndsWith("OVERLINE") || longName.EndsWith("BREVE"))
          return "zw"+result;
        if (longName.EndsWith("BREVE") || longName.EndsWith("TILDE") || longName.EndsWith("MACRON") || longName.EndsWith("CARON") || longName.EndsWith("DIERESIS"))
        {
          if (charInfo.Decomposition?.Type == DecompositionType.Compat &&
              charInfo.Decomposition?.CodePoints.FirstOrDefault() == 0x0020)
            return result +"mod";
          return "zw" + result;
        }
        if (longName.Contains("CANDRABINDU") || longName.Contains("HORN"))
          return "zw" + result;
        if (longName.Split(' ').Last().Length == 1)
          return "zw" + result;

        if (longName.EndsWith("ACCENT"))
          return "zw" + result.Substring(0,result.Length-6);
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

  private static void InitializeCodePointDictionary(BiDiDictionary<CodePoint, string> dictionary, string fileName)
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

  private static void InitializeStringDictionary(Dictionary<string, string> dictionary, string fileName)
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

  private static void InitializeStringIntDictionary(Dictionary<string, int> dictionary, string fileName)
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
            dictionary.Add(parts[0], int.Parse(parts[1]));
          }
        }
      }
    }
  }

  private static void InitializeStringList(List<string> list, string fileName)
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


  private static readonly BiDiDictionary<CodePoint, string> KnownCharNames = new();
  private static readonly BiDiDictionary<CodePoint, string> GreekAlphabet = new();
  private static readonly BiDiDictionary<CodePoint, string> HebrewAlphabet = new();
  private static readonly Dictionary<string, string> StringReplacements = new();
  private static readonly Dictionary<string, string> WordsAbbreviations = new();
  private static readonly List<string> AdjectiveWords = new();
  private static readonly Dictionary<string, int> WordsToRemove = new();



}
