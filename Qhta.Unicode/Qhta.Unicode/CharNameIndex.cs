using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
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

    KnownCharNames.LoadFromFile("KnownCharNames.txt");
    GreekAlphabet.LoadFromFile("GreekAlphabet.txt");
    HebrewAlphabet.LoadFromFile("HebrewAlphabet.txt");
    ScriptNames.LoadFromFile("ScriptNames.txt");
    WordsAbbreviations.LoadFromFile("WordAbbr.txt");
    Letters.LoadFromFile("Letters.txt");
    Numerals.LoadFromFile("Numerals.txt");
    MaxWords = WordsAbbreviations.Keys.Max(key => key.Split(' ').Length);
    foreach (var key in WordsAbbreviations.Keys.Where(key => key.Contains(' ')))
    {
      var key2 = key.Replace(' ', '_');
      StringReplacements.Add(key, key2);
    }
    foreach (var key in Letters.Keys.Where(key => key.Contains(' ')))
    {
      var key2 = key.ToUpper().Replace(' ', '_');
      StringReplacements.Add(key, key2);
    }
    foreach (var key in Numerals.Values.Where(key => key.Contains(' ')))
    {
      var key2 = key.ToUpper().Replace(' ', '_');
      StringReplacements.Add(key, key2);
    }
    foreach (var key in ScriptNames.Keys.Where(key => key.Contains(' ')))
    {
      var key2 = key.ToUpper().Replace(' ', '_');
      StringReplacements.TryAdd(key, key2);
    }
    NameStarts.LoadFromFile("NameStarts.txt");
    AdjectiveWords.LoadFromFile("Adjectives.txt");
    MoveToStartWords.LoadFromFile("MoveToStartWords.txt");
    MoveToEndWords.LoadFromFile("MoveToEndWords.txt");
    WordsToRemove.LoadFromFile("WordsToRemove.txt");

    SignWritingAbbreviations.LoadFromFile("SignWritingAbbr.txt");
    NamedBlocks.LoadFromFile("NamedBlocks.txt");
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

    using (var writer = File.CreateText("SwWords.txt"))
    {
      foreach (var entry in SwWords)
      {
        writer.WriteLine($"{entry.Key} {entry.Value}");
      }
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
    var codePoint = charInfo.CodePoint;
    if (codePoint == 0x1EF2D)
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
  /// Create a short name for a character.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  public string? GenerateShortName(CharInfo charInfo, int alternative = 0)
  {
    if (charInfo.CodePoint == 0xFF01)
      Debug.Assert(true);


    string? charName = null;
    string longName = charInfo.Name.ToString().ToUpper().Replace(" -A", " AA")/*.Replace('-', ' ')*/;

    if (KnownCharNames.TryGetValue(charInfo.CodePoint, out var knownCharName))
      charName = knownCharName;
    else if (GreekAlphabet.TryGetValue(charInfo.CodePoint, out var greekLetterName))
      charName = greekLetterName;
    else if (HebrewAlphabet.TryGetValue(charInfo.CodePoint, out var hebrewLetterName))
      charName = hebrewLetterName;
    else if (TryParseNamedBlockCpName(charInfo, out charName, alternative))
    { }
    else if (charInfo.Category == UcdCategory.Cc || charInfo.CodePoint == 0x020 || longName.StartsWith("<") && TryCreateAliasCharName(charInfo, out charName))
    {}
    else if (charInfo.Category.ToString()[0] == 'Z')
      charName = CreateShortenName(charInfo, alternative);
    else if (TryParseSignWrittingName(charInfo, out charName, alternative))
    { }
    else if (TryParseByzantineMusicalSymbol(charInfo, out charName, alternative))
    { }
    else if (TryParseCuneiformSymbol(charInfo, out charName, alternative))
    { }
    else if (TryParseMusicalSymbol(charInfo, out charName, alternative))
    { }
    else if (TryParseNameStartingString(charInfo, out charName, alternative))
    { }
    else if (charInfo.Decomposition?.Type == DecompositionType.Super || charInfo.Decomposition?.Type == DecompositionType.Sub)
      charName = CreateDecompositionName(charInfo, alternative);
    else if (TryParseModifierLetterName(charInfo, out charName, alternative))
    { }
    else if (TryParseCombiningCharName(charInfo, out charName, alternative))
    { }

    else if (charInfo.Category.ToString()[0] == 'L' || longName.Contains("LETTER") || longName.Contains("CAPITAL"))
      charName = CreateShortenName(charInfo, longName, alternative);
    else
      charName = CreateShortenName(charInfo, alternative);


    if (charName == String.Empty)
      charName = null;
    //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
    return charName;
  }

  private bool TryCreateAliasCharName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    charName = null;
    if (charInfo.Aliases != null)
    {
      var alias = charInfo.Aliases.FirstOrDefault(item => item.Type == NameAliasType.Abbreviation);
      charName = alias?.Name!;
      //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
      if (alias != null)
      {
        charName = alias!.Name;
        return true;
      }
    }
    var longName = charInfo.Name.ToString();
    if (longName.StartsWith("<") && longName.EndsWith(">"))
    {
      charName = CreateShortenName(charInfo, longName.Substring(1, longName.Length - 2).ToUpper());
      return true;
    }
    return false;
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

  private bool TryParseNamedBlockCpName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    charName = null;
    if (NamedBlocks.TryGetValue(charInfo.CodePoint, charInfo.Name.ToString(), out var namedBlock))
    {
      switch (namedBlock!.BlockType)
      {
        case BlockType.Sequential:
          return TryParseSequentialCpName(charInfo, namedBlock.KeyString!, out charName, alternative);
        case BlockType.Numerals:
          return TryParseNumeralCpName(charInfo, out charName, alternative);
        case BlockType.Block:
          return TryParseBlockDrawingsName(charInfo, namedBlock.KeyString, out charName, alternative);
        case BlockType.Special:
          switch (namedBlock.Name)
          {
            case "Control Chars":
              return TryCreateAliasCharName(charInfo, out charName, alternative);
            case "Chess Symbols":
              return TryParseChessFigureName(charInfo, out charName, alternative);
            case "Vulgar Fractions":
              return TryParseVulgarFractionName(charInfo, out charName, alternative);

            default:
              throw new NotImplementedException($"Special parse method for block \"{namedBlock.Name}\" not implemented");
          }
        default:
          charName = CreateShortenName(charInfo, alternative);
          return true;
      }
    }
    return false;
  }

  private bool TryParseModifierLetterName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    var longName = charInfo.Name.ToString();
    var mod = longName.IndexOf("MODIFIER LETTER ");
    if (mod >= 0)
    {
      longName = longName.Replace("MODIFIER LETTER ", "");
      var result = CreateShortenName(charInfo, longName, alternative);
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
      var result = CreateShortenName(charInfo, longName, alternative);
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
    charName = null;
    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    if (codePoint == 0x00BC)
      Debug.Assert(true);
    var ss = SplitWords(longName, alternative);
    var sb = new StringBuilder();
    for (int i = 2; i < ss.Count; i++)
    {
      var word = ss[i];
      sb.Append(word.TitleCase());
    }
    charName = sb.ToString();
    return true;
  }

  private bool TryParseBlockDrawingsName(CharInfo charInfo, string? keyString, out string? charName, int alternative = 0)
  {
    charName = null;
    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    if (codePoint == 0x00BC)
      Debug.Assert(true);
    var ss = SplitWords(longName, alternative);
    var sb = new StringBuilder();
    if (keyString != null)
    {
      var k = ss.IndexOf(keyString);
      if (k >= 0)
      {
        ss.RemoveAt(k);
        ss.Insert(0, keyString);
      }
      else if (keyString.Contains('*'))
      {
        keyString = keyString.Replace("*", " ");
        var keyWords = keyString.Split(' ');
        for (int i = keyWords.Length-1; i>=0; i--)
        {
          var str = keyWords[i];
          if (ss.Contains(str))
          {
            ss.Remove(str);
          }
        }
        ss.Insert(0,keyString);
      }
      else if (keyString.Contains("BLOCK"))
      {
        var str = ss.FirstOrDefault(s => s.Contains(keyString));
        if (str!=null)
        {
          ss.Remove(str);
          ss.Insert(0, str);
        }
      }
    }
    var arrowString = ss.FirstOrDefault(s => s.Contains("ARROW"));
    if (arrowString != null)
    {
      ss.Remove(arrowString);
      ss.Add(arrowString);
    }
    for (int i = 0; i < ss.Count; i++)
    {
      var word = ss[i];
      if (word == "AND" || word == "TO" || word == "WITH" || word == "CONTAINING")
        continue;
      else if (word == "SINGLE")
        sb.Append("1");
      else if (word == "DOUBLE")
        sb.Append("2");
      else if (WordsAbbreviations.TryGetValue(word, out var replacement))
        sb.Append(replacement);
      else
        sb.Append(word.TitleCase());
    }
    charName = sb.ToString();
    return true;
  }

  private bool TryParseSignWrittingName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    string keyString = "SIGNWRITING";
    var longName = charInfo.Name.ToString().ToUpper().Replace('-', ' ');
    var k = longName.IndexOf(keyString);
    if (k >= 0)
    {
      var lead = longName.Substring(0, k + keyString.Length).Trim();
      //var rest = longName.Substring(k + keyString.Length).Trim();
      if (!NameStarts.TryGetValue(lead, out var prefix))
      {
        if (!NameStarts.TryGetValue(keyString, out prefix))
          prefix = keyString.ToLower();
        //rest += " " + lead.Substring(0, lead.Length - keyString.Length);
      }
      var number = charInfo.CodePoint - 0x1D800 + 1;
      var shortName = number.ToString();
      charName = prefix + shortName;
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseByzantineMusicalSymbol(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    string keyString = "BYZANTINE MUSICAL SYMBOL";
    var longName = charInfo.Name.ToString();
    var k = longName.IndexOf(keyString);
    if (k >= 0)
    {
      var lead = keyString;
      if (!WordsAbbreviations.TryGetValue(lead, out var prefix))
        prefix = keyString.ToLower();
      var number = charInfo.CodePoint - 0x1D000 + 1;
      var shortName = number.ToString();
      charName = prefix + shortName;
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseMusicalSymbol(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    string keyString = "MUSICAL SYMBOL";
    var longName = charInfo.Name.ToString();
    var k = longName.IndexOf(keyString);
    if (k >= 0)
    {
      var lead = keyString;
      if (!WordsAbbreviations.TryGetValue(lead, out var prefix))
        prefix = keyString.ToLower();
      var number = charInfo.CodePoint - 0x1D100 + 1;
      var shortName = number.ToString();
      charName = prefix + shortName;
      return true;
    }
    charName = null;
    return false;
  }

  private bool TryParseCuneiformSymbol(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    string keyString = "CUNEIFORM";
    var longName = charInfo.Name.ToString();
    var k = longName.IndexOf(keyString);
    if (k >= 0)
    {
      var lead = keyString;
      if (!WordsAbbreviations.TryGetValue(lead, out var prefix))
        prefix = keyString.ToLower();
      var number = charInfo.CodePoint - 0x12000 + 1;
      var shortName = number.ToString();
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
        var shortName1 = CreateShortenName(charInfo, rest, alternative);
        charName = prefix + shortName1;
        return true;
      }
    }
    charName = null;
    return false;
  }

  private bool TryParseSequentialCpName(CharInfo charInfo, string keyString, out string? charName, int alternative)
  {
    charName = null;
    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    if (codePoint == 0x1CF42)
      Debug.Assert(true);
    var ss = SplitWords(longName, alternative);
    var word = ss.First();
    if (WordsAbbreviations.TryGetValue(word, out var replacement))
    {
      var sb = new StringBuilder();
      sb.Append(replacement);
      SequentialDictionary.TryAdd(keyString, 0);
      var number = SequentialDictionary[keyString] + 1;
      SequentialDictionary[keyString] = number;
      //sb.Append('-');
      sb.Append(number);
      charName = sb.ToString();
      return true;
    }
    return false;
  }

  private bool TryParseNumeralCpName(CharInfo charInfo, out string? charName, int alternative)
  {
    charName = null;
    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    if (codePoint == 0x1ED2D)
      Debug.Assert(true);
    var ss = SplitWords(longName, alternative);
    var sb = new StringBuilder();
    var isTens = ss.Contains("TENS");
    for (int i = 0; i < ss.Count; i++)
    {
      var word = ss[i];
      if (TryFindScriptName(word, alternative, out var scCode))
      {
        sb.Append(scCode);
      }
      else if (Numerals.TryGetValue1(word, out var num))
      {
        if (isTens)
          num *= 10;
        sb.Append(Numerals[num].TitleCase().Replace(" ", ""));
      }
      else if (WordsAbbreviations.TryGetValue(word, out var replacement))
        sb.Append(replacement);
      else if (int.TryParse(word, out _)) 
        sb.Append(word);
    }
    charName = sb.ToString();
    return true;
  }

  private bool TryParseChessFigureName(CharInfo charInfo, out string? charName, int alternative)
  {
    charName = null;
    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString().Replace('-', ' ');
    if (codePoint == 0x11A3F)
      Debug.Assert(true);
    var ss = SplitWords(longName, alternative);
    var k = ss.IndexOf("TURNED");
    if (k >= 0)
    {
      var item = ss[k];
      ss.RemoveAt(k);
      ss.Add(item);
    }
    k = 0;
    if (ss[k].EndsWith("QUADRANT"))
    {
      var item = ss[k];
      ss.RemoveAt(k);
      ss.Add(item);
      ss[0] = "LARGE " + ss[0];
    }
    var sb = new StringBuilder();
    for (int i = 0; i < ss.Count; i++)
    {
      var word = ss[i];
      if (word == "TURNED")
      {
        if (WordsAbbreviations.TryGetValue("ROTATED ONE HUNDRED EIGHTY DEGREES", out var replacement))
          sb.Append(replacement);
        else
        if (WordsAbbreviations.TryGetValue(word, out replacement))
          sb.Append(replacement);
        else sb.Append(TitleCase(word, alternative));
      }
      else
      if (Numerals.TryGetValue1(word, out var number))
        sb.Append(number);
      else
      if (WordsAbbreviations.TryGetValue(word, out var replacement))
        sb.Append(replacement);
      else
        sb.Append(TitleCase(word, alternative));
    }
    charName = sb.ToString();
    return true;
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
    return CreateShortenName(charInfo, longName, alternative);
  }

  /// <summary>
  /// Create a short name for a long character name.
  /// </summary>
  /// <param name="longName"></param>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateShortenName(CharInfo charInfo, string longName, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x037E)
      Debug.Assert(true);
    var sb = new StringBuilder();
    var ss = SplitWords(longName, alternative);
    var isCapital = longName.Contains("CAPITAL");
    var isSmall = longName.Contains("SMALL") && !longName.Contains("SMALL HIGH");
    var isLetter = ss.Contains("LETTER");
    var isLigature = ss.Contains("LIGATURE");
    foreach (var word in MoveToStartWords)
      TryMoveToStart(word, ss);
    if (charInfo.Category.ToString().StartsWith("L"))
      foreach (var word in MoveToEndWords)
        TryMoveToEnd(word, ss);
    for (int i = 0; i < ss.Count; i++)
    {
      var word = ss[i];//.Replace('-', '_');

      if (word != "YI" && TryFindScriptName(word, alternative, out var scCode))
      {
        if (sb.Length==0)
          scCode = scCode.ToLower();
        sb.Append(scCode);
        continue;
      }
      if (!(word =="TO" && charInfo.Category.ToString().StartsWith("L")))
        if (TryFindWordToRemove(word, longName, alternative))
          continue;


      if (word == "CAPITAL")
        continue;

      if (word == "SMALL")
      {
        if (isLetter || isLigature)
          continue;
        if (WordsAbbreviations.TryGetValue(word, out var small))
          sb.Append(small);
        continue;
      }
      if (word == "SMALL CAPITAL")
      {
        continue;
      }

      if (Numerals.TryGetValue1(word, out var abbr))
      {
        sb.Append(TitleCase(word, alternative));
        continue;
      }

      if (Letters.TryGetValue(word, out var letter))
      {
        if (isCapital)
          sb.Append(CapitalCase(letter, alternative));
        else
          sb.Append(SmallCase(letter, alternative));
        continue;
      }

      if (word != "EN" && word != "EM")
      {
        if (WordsAbbreviations.TryGetValue(word, out var replacement))
        {
          if (sb.Length == 0)
            replacement = replacement.ToLower();
          sb.Append(replacement);
          continue;
        }

        if (AdjectiveWords.Contains(word))
        {
          sb.Append(word.ToLower());
          continue;
        }
      }

      if (isCapital)
      {
        if (isSmall)
          sb.Append(WordsAbbreviations["SMALL CAPITAL"]);
        if (word.Length == 2)
          sb.Append(UpperCase(word, alternative));
        else
        {
          if (alternative > 1)
            sb.Append(WordsAbbreviations["CAPITAL"]);
          if (word.Length <= 2) 
            sb.Append(UpperCase(word, alternative));
          else
            sb.Append(TitleCase(word, alternative));
        }
        if (!isLigature)
          isCapital = false;
      }
      else
      if (isSmall)
      {
        sb.Append(LowerCase(word, alternative));
      }
      else
        sb.Append(TitleCase(word, alternative));
      if (!isLigature)
        isSmall = false;
    }
    return sb.ToString();
  }

  private static bool TryMoveToStart(string key, List<string> ss)
  {
    var k = ss.IndexOf(key);
    if (k >= 0)
    {
      var item = ss[k];
      ss.RemoveAt(k);
      ss.Insert(0, item);
      return true;
    }
    return false;
  }

  private static bool TryMoveToEnd(string key, List<string> ss)
  {
    var k = ss.IndexOf(key);
    if (k >= 0)
    {
      var item = ss[k];
      ss.RemoveAt(k);
      k = ss.IndexOf("WITH");
      if (k < 0)
        k = ss.Count();
      ss.Insert(k,item);
      return true;
    }
    return false;
  }

  private static string LowerCase(string word, int alternative)
  {
    if (word.Contains('-'))
    {
      if (alternative == 0)
        word = word.Replace("-", "");
    }
    return word.ToLower();
  }

  private static string UpperCase(string word, int alternative)
  {
    if (word.Contains('-'))
    {
      if (alternative == 0)
        word = word.Replace("-", "");
    }
    return word.ToUpper();
  }

  private static string TitleCase(string word, int alternative)
  {
    if (word.Contains('-'))
    {
      if (alternative == 0)
        word = word.Replace("-", "");
    }
    return word.TitleCase();
  }

  private static string CapitalCase(string word, int alternative)
  {
    if (word.Contains('-'))
    {
      if (alternative == 0)
        word = word.Replace("-", "");
    }
    var chars = word.ToCharArray();
    chars[0] = char.ToUpper(chars[0]);
    return new string(chars);
  }

  private static string SmallCase(string word, int alternative)
  {
    if (word.Contains('-'))
    {
      if (alternative == 0)
        word = word.Replace("-", "");
    }
    var chars = word.ToCharArray();
    chars[0] = char.ToLower(chars[0]);
    return new string(chars);
  }

  private static bool TryFindWordToRemove(string word, string longName, int alternative)
  {
    if (alternative > 1)
      return false;
    if (word == "WITH")
      return true;
    if (word == "AND" && !longName.StartsWith("LOG"))
      return true;
    if (WordsToRemove.TryGetValue(word, out int val))
    {
      if (alternative <= val)
        return true;
    }
    return false;
  }

  private static List<String> SplitWords(string longName, int alternative)
  {
    var k = longName.LastIndexOf('-');
    if (k >= 0)
    {
      var rest = longName.Substring(k + 1);
      if (int.TryParse(rest, out _) || (rest.Length > 2 && int.TryParse(rest, NumberStyles.HexNumber, null, out _)))
        longName = longName.Substring(0, k) + " " + rest;
    }
    longName = ReplaceStrings(longName, alternative);
    var words = longName.Split([' '], StringSplitOptions.RemoveEmptyEntries).ToList();
    for (int i = 0; i < words.Count; i++)
    {
      words[i] = words[i].Replace('_', ' ');
    }
    return words;
  }

  private static string ReplaceStrings(string longName, int alternative)
  {
    var wordSequences = GetWordSequences(longName, alternative);
    for (int i = 0; i < wordSequences.Count; i++)
    {
      var sequence = wordSequences[i];
      if (sequence== "NINETY THOUSAND")
        Debug.Assert(true);
      if (StringReplacements.TryGetValue(sequence, out var replacement))
      {
        longName = longName.Replace(sequence, replacement);
      }
    }
    return longName;
  }

  private static List<string> GetWordSequences(string longName, int alternative)
  {
    var wordSequences = new List<string>();
    var words = longName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    for (int length = 1; length <= words.Length; length++)
    {
      for (int start = 0; start <= words.Length - length; start++)
      {
        if (length > 1)
        {
          var sequence = string.Join(" ", words, start, length);
          wordSequences.Add(sequence);
        }
      }
    }
    wordSequences.Sort((s1, s2) => s2.Length.CompareTo(s1.Length));
    return wordSequences;
  }

  private static bool TryFindScriptName(string word, int alternative, out string scCode)
  {
    word = word.Replace('_', ' ').Replace(" LETTER", "");
    if (alternative == 0 && (word == "LATIN" || word == "GREEK" || word == "COPTIC" || word == "HEBREW"))
    {
      scCode = "";
      return true;
    }
    if (ScriptNames.TryGetValue(word, out scCode))
    {
      //if (WordsAbbreviations.TryGetValue(word.ToUpper(), out var abbr))
      //  scCode = abbr;
      return true;
    }
    return false;
  }

  private static readonly BiDiDictionary<CodePoint, string> KnownCharNames = new();
  private static readonly BiDiDictionary<CodePoint, string> GreekAlphabet = new();
  private static readonly BiDiDictionary<CodePoint, string> HebrewAlphabet = new();
  private static readonly BiDiDictionary<string, string> ScriptNames = new();
  private static readonly SortedDictionary<string, string> WordsAbbreviations = new(new WordsAbbreviationsComparer());
  private static readonly SortedList<string, string> StringReplacements = new(new StringReplacementsComparer());
  private static int MaxWords;
  private static readonly Dictionary<string, string> NameStarts = new();
  private static readonly List<string> AdjectiveWords = new();
  private static readonly List<string> MoveToStartWords = new();
  private static readonly List<string> MoveToEndWords = new();
  private static readonly Dictionary<string, int> WordsToRemove = new();
  private static readonly BiDiDictionary<string, string> Letters = new();
  private static readonly BiDiDictionary<int, string> Numerals = new();
  private static readonly Dictionary<string, string> SignWritingAbbreviations = new();
  private static readonly Dictionary<string, int> SwWords = new();
  internal static readonly NamedBlocks NamedBlocks = new();
  private Dictionary<string, int> SequentialDictionary = new();

  private class WordsAbbreviationsComparer : IComparer<string>
  {
    /// <summary>
    /// Compare two strings by lexicographical order,
    /// but if one string is contained in the other, the longer string goes first.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(string? x, string? y)
    {
      if (x == "EXTENDED ARABIC-INDIC" && y == "EXTENDED ARABIC-INDIC")
        Debug.Assert(true);
      return String.Compare(x!, y, StringComparison.InvariantCulture);
    }
  }

  private class StringReplacementsComparer : IComparer<string>
  {
    /// <summary>
    /// Compare two strings by reverse lexicographical order,
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(string? x, string? y)
    {
      return -String.Compare(x!, y, StringComparison.InvariantCulture);
    }
  }
}


