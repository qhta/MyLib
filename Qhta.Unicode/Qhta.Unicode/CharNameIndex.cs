using System;
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
    InitializeStringDictionary(NameStarts, "NameStarts.txt");
    InitializeStringList(AdjectiveWords, "Adjectives.txt");
    InitializeStringIntDictionary(WordsToRemove, "WordsToRemove.txt");
    InitializeIntStringDictionary(NumberNames, "NumberNames.txt");
    CreateCharNamesToFile("CharNames.txt");
    Initialized = true;
  }
  private static bool Initialized = false;

  /// <summary>
  /// Add a code point to the this.
  /// </summary>
  /// <param name="charName">Name is a single-word string</param>
  /// <param name="codePoint"></param>
  private void AddCheck(CodePoint codePoint, string charName)
  {
    if (this.TryGetValue1(charName, out var value))
    {
      //if (charName == "gujrvowelcandrae")
      //  Debug.Assert(true);
      if (Index1.TryGetValue(codePoint, out var existingValue))
        throw new DuplicateNameException($"CodePoint {codePoint} has charName \"{charName}\"");

      if (this.TryGetValue1(charName, out var codePoint1))
        throw new DuplicateNameException($"Can't add charName \"{charName}\" for {codePoint}.It is already registered for {codePoint1}");
    }
    base.Add(codePoint, charName);
  }

  private void CreateCharNamesToFile(string filename)
  {
    CreateCharNames();
    using (var writer = File.CreateText(filename))
    {
      foreach (var entry in this.OrderBy(item => item.Value.Length))
      {
        writer.WriteLine($"{entry.Key};{entry.Value}");
      }
      //foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
      //{
      //  if (TryGetValue(charInfo.CodePoint, out var charName))
      //    writer.WriteLine($"{charInfo.CodePoint};{charName}");
      //}
    }
  }

  private void CreateCharNames()
  {
    foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
    {
      //if (!IsAccepted(charInfo))
      //  continue;
      TryAddShortName(charInfo);
    }
  }

  private bool TryAddShortName(CharInfo charInfo)
  {
    if (charInfo.CodePoint == 0x00B2)
      Debug.Assert(true);
    var charName = GenerateShortName(charInfo);
    if (charName is null)
      return false;
    //if (charName.Contains("Gamma"))
    //  Debug.Assert(true);

    if (this.TryGetValue1(charName, out var existingCodePoint))
    {
      const int maxAlternative = 2;
      for (int alternative = 1; alternative <= maxAlternative; alternative++)
      {
        var altName2 = GenerateShortName(charInfo, alternative);
        if (altName2 != null && altName2 != charName && !this.TryGetValue1(altName2, out existingCodePoint))
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
  private bool IsAccepted(CharInfo charInfo)
  {

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
      var scriptName = entry.Value;
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
    if (charInfo.CodePoint == 0x1D9BD)
      Debug.Assert(true);


    string? charName = null;
    string longName = charInfo.Name.ToString().ToUpper().Replace(" -A", " AA").Replace('-', ' ');

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
    else if (TryParseModifierLetterName(charInfo, out charName, alternative))
    { }
    else if (TryParseCombiningCharName(charInfo, out charName, alternative))
    { }
    else if (TryParseVulgarFractionName(charInfo, out charName, alternative))
    { }
    else if (TryParseVariationSelectorName(charInfo, out charName, alternative))
    { }
    else if (TryParseChessFigureName(charInfo, out charName, alternative))
    { }
    else if (TryParseNameStartingString(charInfo, out charName, alternative))
    { }
    else if (charInfo.Category.ToString()[0] == 'L' || longName.Contains("LETTER") || longName.Contains("CAPITAL"))
      charName = CreateShortenName(charInfo.CodePoint, longName, alternative);
    else
      charName = CreateShortenName(charInfo, alternative);


    if (charName == String.Empty)
      charName = null;
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
      return CreateShortenName(charInfo.CodePoint, longName.Substring(1, longName.Length - 2).ToUpper());
    return charName;
  }

  /// <summary>
  /// Create a short name for a character decomposition.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string? CreateDecompositionName(CharInfo charInfo, int alternative = 0)
  {
    var decomposition = charInfo.Decomposition;
    if (decomposition == null)
      return CreateShortenName(charInfo);
    var sb = new StringBuilder();
    foreach (var cp in decomposition.CodePoints)
    {
      if (!Ucd.TryGetValue(cp.Value, out var item))
      {
        Debug.WriteLine($"CodePoint {cp} not found in UnicodeData");
        return null;
      }
      var str = GenerateShortName(item, alternative);
      sb.Append(str);
    }
    var sequence = sb.ToString();
    var funcName = (decomposition.Type == DecompositionType.Super) ? "sup" : decomposition.Type.ToString()!.ToLower();
    return funcName + sequence;
  }

  private bool TryParseModifierLetterName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    var mod = longName.IndexOf("MODIFIER LETTER ");
    if (mod >= 0)
    {
      longName = longName.Replace("MODIFIER LETTER ", "");
      var result = CreateShortenName(charInfo.CodePoint, longName, alternative);
      if (result.EndsWith("tonebar"))
        result = result.Substring(0, result.Length - 3);
      charName = result + "mod";
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseCombiningCharName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    var comb = longName.IndexOf("COMBINING ");
    if (comb >= 0)
    {
      longName = longName.Replace("COMBINING ", "");
      var result = CreateShortenName(charInfo.CodePoint, longName, alternative);
      if (alternative == 0)
      {
        if (longName.StartsWith("LIGATURE"))
          charName = "zw" + result + "ligature";
        else if (longName.Contains("ABOVE") || longName.EndsWith("BELOW") || longName.EndsWith("OVERLAY") || longName.EndsWith("OVERLINE"))
          charName = "zw" + result;
        else if (longName.EndsWith("BREVE") || longName.EndsWith("TILDE") || longName.EndsWith("MACRON") || longName.EndsWith("CARON") || longName.EndsWith("DIERESIS"))
        {
          if (charInfo.Decomposition?.Type == DecompositionType.Compat &&
              charInfo.Decomposition?.CodePoints.FirstOrDefault() == 0x0020)
            charName = result + "mod";
          else
            charName = "zw" + result;
        }
        else if (longName.Contains("CANDRABINDU") || longName.Contains("HORN"))
          charName = "zw" + result;
        else if (longName.Split(' ').Last().Length == 1)
          charName = "zw" + result;
        else if (longName.EndsWith("ACCENT"))
          charName = "zw" + result.Substring(0, result.Length - 6);
        else
          charName = "zw" + result;
      }
      else
        charName = "zw" + result;
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseVulgarFractionName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    var longName = charInfo.Name.ToString().ToUpper().Replace('-', ' ');
    if (longName.StartsWith("VULGAR FRACTION "))
    {
      longName = longName.Replace("VULGAR FRACTION ", "");
      charName = CreateShortenName(charInfo.CodePoint, longName, alternative);
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseVariationSelectorName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    string keyString = "VARIATION SELECTOR";
    var longName = charInfo.Name.ToString().ToUpper().Replace('-', ' ');
    var k = longName.IndexOf(keyString);
    if (k >= 0)
    {
      var lead = longName.Substring(0, k + keyString.Length).Trim();
      var rest = longName.Substring(k + keyString.Length).Trim();
      if (NumberNames.TryGetValue1(rest.TitleCase(), out var num))
        rest = num.ToString();
      if (!NameStarts.TryGetValue(lead, out var prefix))
        prefix = lead.Acronym();
      charName = prefix + rest;
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseChessFigureName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    string keyString = "CHESS";
    var longName = charInfo.Name.ToString().ToUpper().Replace('-', ' ');
    var k = longName.IndexOf(keyString);
    if (k >= 0)
    {
      var lead = longName.Substring(0, k + keyString.Length).Trim();
      var rest = longName.Substring(k + keyString.Length).Trim();
      if (!NameStarts.TryGetValue(lead, out var prefix))
      {
        if (!NameStarts.TryGetValue(keyString, out prefix))
          prefix = keyString.ToLower();
        rest += " " + lead.Substring(0, lead.Length - keyString.Length);
      }
      keyString = "ROTATED ";
      k = rest.IndexOf(keyString);
      if (k >= 0)
      {
        rest = rest.Substring(0, k) + rest.Substring(k + keyString.Length);
        foreach (var entry in NumberNames.Where(item => item.Key >= 45))
          rest = rest.Replace(entry.Value.ToUpper(), entry.Key.ToString());
      }
      else
      {
        keyString = "TURNED ";
        k = rest.IndexOf(keyString);
        if (k >= 0)
        {
          rest = rest.Substring(0, k) + rest.Substring(k + keyString.Length) + " 180 DEGREES";
        }
      }
      var shortName = CreateShortenName(charInfo.CodePoint, rest, alternative);
      charName = prefix + shortName;
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseNameStartingString(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    var longName = charInfo.Name.ToString().ToUpper().Replace('-', ' ');
    foreach (var entry in NameStarts)
    {
      var key = entry.Key + " ";
      if (longName.StartsWith(key))
      {
        var prefix = entry.Value;
        var rest = longName.Substring(key.Length);
        //if (charInfo.CodePoint >= 0x10000)
        //{
        //  if (!int.TryParse(rest, NumberStyles.HexNumber, null, out _))
        //    rest = charInfo.CodePoint;
        //  charName = prefix + "{" + rest + "}";
        //  return true;
        //}
        if (int.TryParse(rest, NumberStyles.HexNumber, null, out _))
        {
          rest = charInfo.CodePoint;
          charName = prefix + "{" + rest + "}";
          return true;
        }
        var shortName1 = CreateShortenName(charInfo.CodePoint, rest, alternative);
        charName = prefix + shortName1;
        return true;
      }
    }
    charName = null;
    return false;
  }

  /// <summary>
  /// Create a short name for a charInfo.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateShortenName(CharInfo charInfo, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    longName = ReplaceStrings(longName, alternative);
    var sb = new StringBuilder();
    var words = longName.Split([' ', ',', '-', '_'], StringSplitOptions.RemoveEmptyEntries).ToList();
    if (words.Count == 1)
    {
      if (StringReplacements.TryGetValue(words[0], out var repl))
        return repl;
      return words[0].ToLower();
    }
    return CreateShortenName(charInfo.CodePoint, longName, alternative);
  }

  /// <summary>
  /// Create a short name for a long character name.
  /// </summary>
  /// <param name="longName"></param>
  /// <param name="codePoint"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateShortenName(CodePoint codePoint, string longName, int alternative = 0)
  {
    if (codePoint == 0x250D)
      Debug.Assert(true);
    longName = ReplaceStrings(longName, alternative);
    var sb = new StringBuilder();
    var ss = longName.Split([' ', ',', '-']);
    var wasCapital = longName.Contains("CAPITAL");
    var wasSmall = longName.Contains("SMALL");
    var wasLetter = longName.Contains("LETTER");
    //var wasWith = false;
    for (int i = 0; i < ss.Length; i++)
    {
      var word = ss[i];//.Replace('-', '_');

      if (word != "YI" && TryFindScriptName(word, alternative, out var scCode))
      {
        scCode = scCode.ToLower();
        sb.Append(scCode);
        continue;
      }
      if (TryFindWordToRemove(word, longName, alternative))
        continue;


      if (word == "CAPITAL")
        continue;

      if (word == "SMALL")
      {
        if (wasLetter)
          continue;
        if (WordsAbbreviations.TryGetValue(word, out var small))
          sb.Append(small);
        continue;
      }

      if (word != "EN" && word != "EM")
      {
        if (WordsAbbreviations.TryGetValue(word, out var replacement))
        {
          if (AdjectiveWords.Contains(word))
          {
            sb.Append(word.ToLower());
            continue;
          }
          sb.Append(replacement);
          continue;
        }

        if (AdjectiveWords.Contains(word))
        {
          sb.Append(word.ToLower());
          continue;
        }
      }

      if (wasCapital)
      {
        if (wasSmall)
          sb.Append("smcap");
        if (word.Length == 2)
          sb.Append(word.ToUpper());
        else
        {
          if (alternative > 1)
            sb.Append("cap");
          sb.Append(word.TitleCase());
        }
        wasCapital = false;
      }
      else
      if (wasSmall)
      {
        if (alternative > 0 || !wasLetter)
        {
          if (WordsAbbreviations.TryGetValue("SMALL", out var abbr))
            sb.Append(abbr);
          else
            sb.Append("small");
        }
        sb.Append(word.ToLower());
      }
      else
        sb.Append(word.ToLower());
      wasSmall = false;
    }
    return sb.ToString();
  }

  private static bool TryFindWordToRemove(string word, string longName, int alternative)
  {
    if (alternative > 1)
      return false;
    if (word == "WITH" && longName.Contains("ARROW"))
      return false;
    if (word == "AND" && !longName.StartsWith("LOG"))
      return true;
    if (WordsToRemove.TryGetValue(word, out int val))
    {
      if (alternative <= val)
        return true;
    }
    return false;
  }

  private static string ReplaceStrings(string longName, int alternative)
  {
    if (longName.Contains("ARROW"))
      return longName;
    foreach (var entry in StringReplacements)
    {
      longName = longName.Replace(entry.Key, entry.Value);
    }
    return longName;
  }

  private static bool TryFindScriptName(string word, int alternative, out string scCode)
  {
    if (word == "CANADIAN")
      word = "Canadian Aboriginal";
    else
      word = word.Replace('_', ' ').TitleCase(true);
    if (alternative == 0 && (word == "Latin" || word == "Greek" || word == "Hebrew"))
    {
      scCode = "";
      return true;
    }
    if (ScriptCodes.UcdScriptNames.TryGetValue1(word, out scCode))
    {
      //if (WordsAbbreviations.TryGetValue(word.ToUpper(), out var abbr))
      //  scCode = abbr;
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

  private static void InitializeIntStringDictionary(BiDiDictionary<int, string> dictionary, string fileName)
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
            dictionary.Add(int.Parse(parts[0]), parts[1]);
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
  private static readonly Dictionary<string, string> NameStarts = new();
  private static readonly List<string> AdjectiveWords = new();
  private static readonly Dictionary<string, int> WordsToRemove = new();
  private static readonly BiDiDictionary<int, string> NumberNames = new();


}
