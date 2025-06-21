using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks.Sources;
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
    Ordinals.LoadFromFile("Ordinals.txt");
    MaxWords = WordsAbbreviations.Keys.Max(key => key.Split(' ').Length);
    foreach (var key in WordsAbbreviations.Keys.Where(key => key.Contains(' ')))
    {
      var key2 = key.Replace(' ', '_');
      StringReplacements.Add(key, key2);
    }
    foreach (var key in Letters.Keys.Where(key => key.Contains(' ')))
    {
      if (!StringReplacements.ContainsKey(key))
      {
        var key2 = key.ToUpper().Replace(' ', '_');
        StringReplacements.Add(key, key2);
      }
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
    foreach (var entry in ScriptNames)
    {
<<<<<<< HEAD
      WordsAbbreviations.TryAdd(entry.Key, entry.Value);
=======
      NameStarts.Add(entry.Key, entry.Value);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    }
    Adjectives.LoadFromFile("Adjectives.txt");
    WordsToRemove.LoadFromFile("WordsToRemove.txt");
    Functors.LoadFromFile("WordAbbr.txt", "Functors");
    Functors.LoadFromFile("WordAbbr.txt", "DiacriticPos");
    Functors.LoadFromFile("WordAbbr.txt", "Forms");
    Enclosings.LoadFromFile("WordAbbr.txt", "Enclosing");
    Enclosed.LoadFromFile("WordAbbr.txt", "Enclosed");
    Functors.LoadFromFile("WordAbbr.txt", "Enclosing");
    NameStarts.LoadFromFile("WordAbbr.txt", "SymbolGroups");
<<<<<<< HEAD
=======
    NameStarts.LoadFromFile("WordAbbr.txt", "Forms");
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    NameStarts.LoadFromFile("WordAbbr.txt", "Games");
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
      foreach (var entry in this.OrderBy(item => item.Key))
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
    if (codePoint == 0x0279)
      Debug.Assert(true);
<<<<<<< HEAD
    var charName = CreateShortName(charInfo, 0);
    if (charName == String.Empty)
=======
    var charName = CreateShortName(charInfo);
    if (charName is null)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      return false;

    if (this.TryGetValue1(charName, out var existingCodePoint))
    {
      const int maxAlternative = 2;
      for (int alternative = 1; alternative <= maxAlternative; alternative++)
      {
        var altName2 = CreateShortName(charInfo, alternative);
<<<<<<< HEAD
        if (altName2 != charName && !this.TryGetValue1(altName2, out existingCodePoint))
=======
        if (altName2 != null && altName2 != charName && !this.TryGetValue1(altName2, out existingCodePoint))
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        {
          this.AddCheck(charInfo.CodePoint, altName2);
          break;
        }
        else
        {
          this.TryGetValue1(charName, out existingCodePoint);
          var charInfo1 = Ucd[existingCodePoint];
          var altName1 = CreateShortName(charInfo1, alternative);
<<<<<<< HEAD
          if (altName1 != charName)
=======
          if (altName1 != null && altName1 != charName)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
          {
            var ok = this.Remove(existingCodePoint);
            if (ok)
              ok = Index2.Remove(altName1);
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
  /// Create a short name for a character or get it if already exists.
  /// </summary>
<<<<<<< HEAD
  private string GetOrCreateShortName(CharInfo charInfo, int alternative)
=======
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  public string? GetOrCreateShortName(CharInfo charInfo, int alternative = 0)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  {
    if (Ucd.CharNameIndex.TryGetValue(charInfo.CodePoint, out var charName))
      return charName;
    return CreateShortName(charInfo, alternative);
  }

  /// <summary>
<<<<<<< HEAD
  /// Create a short name for a character or get it if already exists.
  /// </summary>
  private string GetOrCreateShortName(Tokens tokens, int alternative)
  {
    var longName = tokens.ToString();
    if (Ucd.NameIndex.TryGetValue(longName, out var cp))
      return GetOrCreateShortName(Ucd[cp], alternative);
    return CreateMainCharName(tokens, alternative);
  }

  /// <summary>
  /// Create a short name for a character info.
=======
  /// Create a short name for a character.
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
<<<<<<< HEAD
  public string CreateShortName(CharInfo charInfo, int alternative)
=======
  public string? CreateShortName(CharInfo charInfo, int alternative = 0)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  {
    if (charInfo.CodePoint == 0x08CB)
      Debug.Assert(true);

<<<<<<< HEAD
    string longName = charInfo.Name.ToString().ToUpper().Replace(" -A", " AA");

    var tokens = SplitWords(longName, alternative);
    return CreateShortName(charInfo, tokens, alternative);
=======
    string? charName = null;
    string longName = charInfo.Name.ToString().ToUpper().Replace(" -A", " AA");

    var cp = charInfo.CodePoint;
    //if (cp >= 'a' && cp <= 'z' || cp >= 'A' && cp <= 'Z')
    //  charName = ScriptNames["LATIN"] + ":" + (char)cp;
    //else 
    if (KnownCharNames.TryGetValue(charInfo.CodePoint, out var knownCharName))
      charName = knownCharName;
    else if (GreekAlphabet.TryGetValue(charInfo.CodePoint, out var greekLetterName))
      charName = greekLetterName;
    else if (HebrewAlphabet.TryGetValue(charInfo.CodePoint, out var hebrewLetterName))
      charName = hebrewLetterName;
    else if (TryParseSignWritingName(charInfo, out charName, alternative))
    { }
    else if (TryParseByzantineMusicalSymbol(charInfo, out charName, alternative))
    { }
    else if (TryParseCuneiformSymbol(charInfo, out charName, alternative))
    { }
    else if (TryParseMusicalSymbol(charInfo, out charName, alternative))
    { }
    else if (TryParseChessFigureName(charInfo, out charName, alternative))
    { }
    else if (TryParseModifierLetterName(charInfo, out charName, alternative))
    { }
    else if (TryParseCombiningCharName(charInfo, out charName, alternative))
    { }
    else if (TryParseDecompositionName(charInfo, out charName, alternative))
    { }
    else if (TryParseNamedBlockCpName(charInfo, out charName, alternative))
    { }
    else if (charInfo.Category == UcdCategory.Cc || charInfo.CodePoint == 0x020 || longName.StartsWith("<") && TryCreateAliasCharName(charInfo, out charName))
    { }
    else if (charInfo.Category.ToString()[0] == 'Z')
      charName = CreateShortenName(charInfo, alternative);
    else if (TryParseNameStartingString(charInfo, out charName, alternative))
    { }
    //else if (charInfo.Decomposition?.Type == DecompositionType.Super || charInfo.Decomposition?.Type == DecompositionType.Sub)
    //  charName = CreateDecompositionName(charInfo, alternative);
    else if (charInfo.Category.ToString()[0] == 'L' || longName.Contains("LETTER") || longName.Contains("CAPITAL"))
      charName = CreateShortenName(charInfo, longName, alternative);
    else
      charName = CreateShortenName(charInfo, alternative);


    if (charName == String.Empty)
      charName = null;
    //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
    return charName;
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  }

  /// <summary>
  /// Create a short name for a character info and a list of token.
  /// </summary>
  private string CreateShortName(CharInfo charInfo, Tokens tokens, int alternative)
  {
    if (charInfo.CodePoint == 0x01C4)
      Debug.Assert(true);
    // ReSharper disable once InlineOutVariableDeclaration
    string charName;
    if (tokens.Count == 0)
      return string.Empty;

    if (KnownCharNames.TryGetValue(charInfo.CodePoint, out charName))
      return charName;
    if (TryParseNamedBlockCpName(charInfo, tokens, alternative, out charName))
      return charName;
    return string.Empty;
    if (GreekAlphabet.TryGetValue(charInfo.CodePoint, out charName))
      return charName;
    if (HebrewAlphabet.TryGetValue(charInfo.CodePoint, out charName))
      return charName;
    if (TryParseSignWritingName(charInfo, tokens, alternative, out charName))
      return charName;
    if (TryParseByzantineMusicalSymbol(charInfo, tokens, alternative, out charName))
      return charName;
    if (TryParseCuneiformSymbol(charInfo, tokens, alternative, out charName))
      return charName;
    if (TryParseMusicalSymbol(charInfo, tokens, alternative, out charName))
      return charName;
    if (TryParseChessFigureName(charInfo, tokens, alternative, out charName))
      return charName;
    if (TryParseModifierLetterName(charInfo, tokens, alternative, out charName))
      return charName;
    if (TryParseCombiningCharName(charInfo, tokens, alternative, out charName))
      return charName;
    if (TryParseDecompositionName(charInfo, alternative, out charName))
      return charName;
    if (TryParseNameStartingString(charInfo, tokens, alternative, out charName))
      return charName;

    if (charInfo.Category == UcdCategory.Cc || charInfo.CodePoint == 0x020 || tokens.StartsWith("<") && TryCreateAliasCharName(charInfo, alternative, out charName))
      return charName;
    if (charInfo.Category.ToString()[0] == 'Z')
      return CreateMainCharName(tokens, alternative);

    //else if (charInfo.Decomposition?.Type == DecompositionType.Super || charInfo.Decomposition?.Type == DecompositionType.Sub)
    //  charName = CreateDecompositionName(charInfo, alternative);
    if (charInfo.Category.ToString()[0] == 'L' || tokens.Contains("LETTER") || tokens.Contains("CAPITAL"))
      return CreateLetterName(tokens, alternative);
    return CreateMainCharName(tokens, alternative);

  }

  private bool TryCreateAliasCharName(CharInfo charInfo, int alternative, out string charName)
  {
    charName = String.Empty;
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
    return false;
  }

<<<<<<< HEAD
  private string CreateRangeCharName(Tokens tokens, int alternative)
  {
    var charName = WordsAbbreviations[tokens.First()] + tokens.Last().ToUpper();
    return charName;
  }

  private HashSet<(BlockType, UcdCategory)> blockCategories = new();

  private bool TryParseNamedBlockCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    if (charInfo.CodePoint == 0x01C4)
      Debug.Assert(true);
    charName = String.Empty;
    if (NamedBlocks.TryGetValue(charInfo.CodePoint, charInfo.Name.ToString(), out var namedBlock) && namedBlock!=null)
=======
  private bool TryParseNamedBlockCpName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x0B75)
      Debug.Assert(true);
    charName = null;
    if (NamedBlocks.TryGetValue(charInfo.CodePoint, charInfo.Name.ToString(), out var namedBlock))
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    {
      if (namedBlock.BlockType == BlockType.Unknown)
        Debug.WriteLine($"{charInfo.CodePoint}:{charInfo.Name}");
      var entry = (namedBlock.BlockType,charInfo.Category);
      if (!blockCategories.Contains(entry))
      {
<<<<<<< HEAD
        blockCategories.Add(entry);
        Debug.WriteLine($"{entry.BlockType};{entry.Category}");
      }
      return false;
      switch (namedBlock.BlockType)
      {
        case BlockType.Alphabet:
          return TryParseAlphabetCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Abjad:
        case BlockType.Abugida:
        case BlockType.Syllabary:
        case BlockType.SemiSyllabary:
          return TryParseOtherScriptCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Phonetic:
          return TryParsePhoneticCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Ideographic:
          return TryParseIdeographicCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Cuneiform:
          return TryParseCuneiformCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Symbols:
        case BlockType.Punctuation:
          return TryParseSymbolCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Combining:
          return TryParseCombiningCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Enclosed:
          return TryParseEnclosedSymbolName(charInfo, tokens, alternative, out charName);
=======
        case BlockType.Enclosed:
          return TryParseEnclosedSymbolName(charInfo, out charName, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        case BlockType.Sequential:
          return TryParseSequentialCpName(charInfo, tokens, namedBlock.KeyString!, alternative, out charName);
        case BlockType.Numerals:
          return TryParseNumeralCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Blockdraw:
          return TryParseBlockDrawingsName(charInfo, namedBlock.KeyString, alternative, out charName);
        case BlockType.Range:
          return TryParseRangeCpName(charInfo, tokens, alternative, out charName);
        case BlockType.Special:
        case BlockType.Unknown:
          switch (namedBlock.Name)
          {
            case "Control Chars":
              return TryCreateAliasCharName(charInfo, alternative, out charName);
            case "Spacing Modifier Letters":
              return TryParseModifierLetterName(charInfo, tokens, alternative, out charName);
            case "Chess Symbols":
              return TryParseChessFigureName(charInfo, tokens, alternative, out charName);
            case "Vulgar Fractions":
              return TryParseVulgarFractionName(charInfo, alternative, out charName);
            case "Sutton SignWriting":
<<<<<<< HEAD
              return TryParseSignWritingName(charInfo, tokens, alternative, out charName);
=======
              return TryParseSignWritingName(charInfo, out charName, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
            default:
              throw new NotImplementedException($"Special parse method for block \"{namedBlock.Name}\" not implemented");
          }
        default:
          charName = CreateStartCharName(tokens, alternative);
          return true;
      }
    }
    return false;
  }

  private bool TryParseAlphabetCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
<<<<<<< HEAD
    charName = String.Empty;
    switch (charInfo.Category)
    {
      case UcdCategory.Ll:
      case UcdCategory.Lu:
        return TryParseLetterCpName(charInfo, tokens, alternative, out charName);
      case UcdCategory.Lt:
        return TryParseLigatureCpName(charInfo, tokens, alternative, out charName);
      case UcdCategory.Lm:
        return TryParseModifierLetterName(charInfo, tokens, alternative, out charName);
      case UcdCategory.Lo:
        return TryParseLetterCpName(charInfo, tokens, alternative, out charName);
      case UcdCategory.Nd:
      case UcdCategory.Nl:
      case UcdCategory.No:
        return TryParseNumeralCpName(charInfo, tokens, alternative, out charName);
        case UcdCategory.Mn:
        return TryParseCombiningCharName(charInfo, tokens, alternative, out charName);
      default:
        return TryParseSymbolCpName(charInfo, tokens, alternative, out charName);
    }
  }
=======
    if (charInfo.CodePoint == 0x0816)
      Debug.Assert(true);
    charName = null;
    var longName = charInfo.Name.ToString();
    if (longName.StartsWith("MODIFIER LETTER ") || charInfo.Category == UcdCategory.Lm)
    {
      string trailStr;
      if (longName.StartsWith("MODIFIER LETTER SMALL CAPITAL"))
        trailStr = "LATIN" + longName.Substring("MODIFIER".Length);
      else
      if (longName.StartsWith("MODIFIER LETTER SMALL"))
        trailStr = "LATIN SMALL LETTER" + longName.Substring("MODIFIER LETTER SMALL".Length);
      else
      if (longName.StartsWith("MODIFIER LETTER CAPITAL"))
        trailStr = "LATIN CAPITAL LETTER" + longName.Substring("MODIFIER LETTER CAPITAL".Length);
      else
      if (longName.StartsWith("MODIFIER LETTER CYRILLIC SMALL"))
        trailStr = "CYRILLIC SMALL LETTER" + longName.Substring("MODIFIER LETTER CYRILLIC SMALL".Length);
      else if (longName.StartsWith("MODIFIER LETTER "))
        trailStr = longName.Substring("MODIFIER LETTER ".Length);
      else
        trailStr = longName;
      if (Ucd.NameIndex.TryGetValue(trailStr, out var modCP) && !Equals(modCP, charInfo.CodePoint))
      {
        charName = GetOrCreateShortName(Ucd[modCP], alternative);
        if (charName == null)
          return false;
        if (charInfo.Decomposition?.Type == DecompositionType.Super)
          charName = @"sup\" + charName;
        else if (charInfo.Decomposition?.Type == DecompositionType.Sub)
          charName = @"sub\" + charName;
        else
          charName = @"sp\" + charName;
      }
      else
      {
        charName = CreateShortenName(charInfo, trailStr, alternative);
        charName = @"sp\" + charName;
      }
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148


  private bool TryParseOtherScriptCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }


  private bool TryParseIdeographicCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParseCuneiformCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParseLetterCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParseLigatureCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParseDigraphCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParseSymbolCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    //Debug.WriteLine($"{charInfo.CodePoint}: {tokens}");
    //if (tokens.Contains("LETTER") || tokens.Contains("CAPITAL") || tokens.Contains("LIGATURE") || tokens.Contains("DIGRAPH"))
    //  return TryParseLetterCpName(charInfo, tokens, alternative, out charName);
    charName = String.Empty;
    return false;
  }

  private bool TryParseCombiningCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParsePhoneticCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParseRangeCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = String.Empty;
    return false;
  }

  private bool TryParseModifierLetterName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    if (charInfo.CodePoint == 0x02C7)
      Debug.Assert(true);
    charName = String.Empty;
    if (tokens.StartsWith("MODIFIER", "LETTER") || charInfo.Category == UcdCategory.Lm)
    {
      Tokens? trail = null;
      if (tokens.StartsWith("MODIFIER", "LETTER", "SMALL", "CAPITAL"))
        trail = tokens.Substring(4).Prepend("LATIN", "SMALL", "CAPITAL", "LETTER");
      else
      if (tokens.StartsWith("MODIFIER", "LETTER", "SMALL"))
        trail = tokens.Substring(3).Prepend("LATIN", "SMALL", "LETTER");
      else
      if (tokens.StartsWith("MODIFIER", "LETTER", "CAPITAL"))
        trail = tokens.Substring(3).Prepend("LATIN", "CAPITAL", "LETTER");
      else
      if (tokens.StartsWith("MODIFIER", "LETTER", "CYRILLIC"))
        trail = tokens.Substring(3).Prepend("CYRILLIC", "SMALL", "LETTER");
      else
      if (tokens.StartsWith("MODIFIER"))
        trail = tokens.Substring(2);
      if (trail != null)
        charName = GetOrCreateShortName(trail, alternative);
      else
        charName = CreateMainCharName(tokens, alternative);
      if (charInfo.Decomposition?.Type == DecompositionType.Super)
        charName = @"sup\" + charName;
      else if (charInfo.Decomposition?.Type == DecompositionType.Sub)
        charName = @"sub\" + charName;
      else
        charName = @"sp\" + charName;
      return true;
    }
    return false;
  }

<<<<<<< HEAD
  private bool TryParseDecompositionName(CharInfo charInfo, int alternative, out string charName)
  {
    if (charInfo.CodePoint == 0x02B0)
=======
  private bool TryParseDecompositionName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x02B9)
      Debug.Assert(true);
    charName = null;
    if (charInfo.Decomposition?.Type == DecompositionType.Super || charInfo.Decomposition?.Type == DecompositionType.Sub
        || charInfo.Decomposition?.Type == DecompositionType.Wide || charInfo.Decomposition?.Type == DecompositionType.Narrow)
    {
      var subNames = new List<string>();
      foreach (var modCP in charInfo.Decomposition.CodePoints)
      {
        if (modCP == "4E8C")
          Debug.Assert(true);

        if (!Ucd.ContainsKey(modCP))
          return false;
        var subName = GetOrCreateShortName(Ucd[modCP], alternative);
        if (subName != null)
          subNames.Add(subName);

      }
      var isSimple = subNames.All(s => s.Length == 1);
      if (isSimple)
        charName = string.Join("", subNames);
      else
        charName = string.Join(@"\", subNames);
      if (charInfo.Decomposition?.Type == DecompositionType.Super)
      {
        charName = @"sup\" + charName;
        return true;
      }
      else
      if (charInfo.Decomposition?.Type == DecompositionType.Sub)
      {
        charName = @"sub\" + charName;
        return true;
      }
      else
      if (charInfo.Decomposition?.Type == DecompositionType.Wide)
      {
        charName = @"Wide\" + charName;
        return true;
      }
      else
      if (charInfo.Decomposition?.Type == DecompositionType.Narrow)
      {
        charName = @"Narrow\" + charName;
        return true;
      }
    }
    else
    if (charInfo.Decomposition?.Type == DecompositionType.Compat && charInfo.Decomposition.CodePoints.FirstOrDefault() == 0x020)
    {
      var longName = charInfo.Name.ToString();
      if (longName.StartsWith("GREEK "))
      {
        longName = longName.Substring("GREEK ".Length);
        charName = CreateMainCharName(charInfo, longName, alternative);
        charName = @"sp\" + charName;
        return true;
      }
    }
    return false;
  }

  private bool TryParseCombiningCharName(CharInfo charInfo, out string? charName, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x08CB)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      Debug.Assert(true);
    charName = String.Empty;
    if (charInfo.Decomposition?.Type == DecompositionType.Super || charInfo.Decomposition?.Type == DecompositionType.Sub
        || charInfo.Decomposition?.Type == DecompositionType.Wide || charInfo.Decomposition?.Type == DecompositionType.Narrow)
    {
      var subNames = new List<string>();
      foreach (var modCP in charInfo.Decomposition.CodePoints)
      {
        if (modCP == "4E8C")
          Debug.Assert(true);

        if (!Ucd.ContainsKey(modCP))
          return false;
        var subName = GetOrCreateShortName(Ucd[modCP], alternative);
        if (subName != String.Empty)
          subNames.Add(subName);

      }
      var isSimple = subNames.All(s => s.Length == 1);
      if (isSimple)
        charName = string.Join("", subNames);
      else
        charName = string.Join(@"\", subNames);
      if (charInfo.Decomposition?.Type == DecompositionType.Super)
      {
        charName = @"sup\" + charName;
        return true;
      }
      else
      if (charInfo.Decomposition?.Type == DecompositionType.Sub)
      {
        charName = @"sub\" + charName;
        return true;
      }
      else
      if (charInfo.Decomposition?.Type == DecompositionType.Wide)
      {
        charName = @"Wide\" + charName;
        return true;
      }
      else
      if (charInfo.Decomposition?.Type == DecompositionType.Narrow)
      {
        charName = @"Narrow\" + charName;
        return true;
      }
    }
    else
    if (charInfo.Decomposition?.Type == DecompositionType.Compat && charInfo.Decomposition.CodePoints.FirstOrDefault() == 0x020)
    {
      var longName = charInfo.Name.ToString();
      var tokens = SplitWords(longName, alternative);
      if (tokens.StartsWith("GREEK"))
      {
        charName = CreateMainCharName(tokens.Substring(1), alternative);
        charName = @"sp\" + charName;
        return true;
      }
    }
    return false;
  }

  private bool TryParseCombiningCharName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    if (charInfo.CodePoint == 0x08CB)
      Debug.Assert(true);
    charName = String.Empty;
    bool isSmallLetter = false;
    Tokens? trail = null;
    if (tokens.StartsWith("COMBINING"))
    {
      if (tokens.StartsWith("COMBINING", "LATIN", "LETTER", "SMALL CAPITAL"))
      {
        trail = tokens.Substring(4);
        charName = GetOrCreateShortName(trail, alternative);
        charName += WordsAbbreviations["ABOVE"];
        return true;
      }
      else if (tokens.StartsWith("COMBINING", "LATIN", "SMALL LETTER"))
      {
        trail = tokens.Substring(3);
        if (trail.Count == 1)
        {
          charName = trail.ToLower() + WordsAbbreviations["ABOVE"];
          return true;
        }
        // ReSharper disable once UseIndexFromEndExpression
        if (trail.EndsWith("BELOW") && trail[trail.Count - 2].Length == 1)
        {
          // ReSharper disable once UseIndexFromEndExpression
          charName = trail[trail.Count - 2].ToLower() + WordsAbbreviations["BELOW"];
          return true;
        }
        trail = ConcatAboveIfPossible(trail);
        isSmallLetter = true;
      }
      else if (tokens.StartsWith("COMBINING", "GREEK", "MUSICAL"))
        trail = tokens.Substring(3).Append("ABOVE");
      else if (tokens.StartsWith("COMBINING", "GREEK"))
        trail = tokens.Substring(2);
      else if (tokens.StartsWith("COMBINING", "CYRILLIC", "SMALL LETTER"))
      {
        trail = ConcatAboveIfPossible(tokens.Substring(3)).Prepend("CYRILLIC");
        isSmallLetter = true;
      }
      else if (tokens.StartsWith("COMBINING", "CYRILLIC", "LETTER"))
        trail = ConcatAboveIfPossible(tokens.Substring(3)).Prepend("CYRILLIC");
      else if (tokens.StartsWith("COMBINING", "CYRILLIC"))
        trail = ConcatAboveIfPossible(tokens.Substring(2)).Prepend("CYRILLIC");
      else if (tokens.StartsWith("COMBINING", "KATAKANA-HIRAGANA"))
        trail = ConcatAboveIfPossible(tokens.Substring(2)).Prepend("KATAKANA-HIRAGANA").Append("ABOVE");
      else if (tokens.StartsWith("COMBINING"))
        trail = tokens.Substring(1);

    }
    else if (charInfo.Category == UcdCategory.Mn)
    {
      if (tokens.StartsWith("HEBREW", "ACCENT"))
        trail = tokens.Substring(2).Prepend("HEBREW").Append("ACCENT");
      else if (tokens.StartsWith("HEBREW", "MARK"))
        trail = tokens.Substring(2).Prepend("HEBREW").Append("MARK");
      else if (tokens.StartsWith("HEBREW", "POINT"))
        trail = tokens.Substring(2).Prepend("HEBREW").Append("POINT");
      else
        trail = tokens;
    }
    if (trail != null)
    {
      var sb = new StringBuilder();
      for (int i = 0; i < trail.Count; i++)
      {
        var word = trail[i];
        if (word == "ACCENT")
        {
<<<<<<< HEAD
          if (tokens.StartsWith("MUSICAL", "SYMBOL"))
=======
          if (ss.First() == "MUSICAL SYMBOL")
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
            sb.Append("Accent");
          continue;
        }
        if (word == "MARK" || word == "SIGN")
          continue;
        if (word == "MODIFIER" || word == "COMBINING")
          continue;
        if (word == "SUPERSCRIPT")
          continue;
        if (word == "DIGIT" || word == "LETTER")
        {
          tokens.Add("ABOVE");
          continue;
        }
        if (word == "WITH" || word == "AND")
        {
          sb.Append(@"\");
          continue;
        }
<<<<<<< HEAD
=======
        if (word == "WITH" || word == "AND")
        {
          sb.Append(@"\");
          continue;
        }
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        if (TryFindScriptName(word, alternative, out var lang))
        {
          var isEmpty = sb.Length == 0;
          sb.Append(lang);
          if (isEmpty)
            sb.Append(@"\");
          continue;
        }

        if (WordsAbbreviations.TryGetValue(word, out var replacement))
        {
          sb.Append(replacement);
<<<<<<< HEAD
          if (i < tokens.Count - 1 && Functors.ContainsKey(word))
=======
          if (i < ss.Count - 1 && Functors.ContainsKey(word))
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
            sb.Append(@"\");
        }
        else if (isSmallLetter && word.Length == 1)
        {
          sb.Append(word.ToLower());
          isSmallLetter = false;
        }

        else if (Numerals.TryGetValue1(word, out var n))
          sb.Append(n.ToString());
        else
          sb.Append(TitleCase(word, alternative));
      }
      charName = sb.ToString();
      if (alternative > 0
          && !charName.EndsWith(WordsAbbreviations["ABOVE"])
          && !charName.EndsWith(WordsAbbreviations["BELOW"])
          && !charName.EndsWith(WordsAbbreviations["OVERLAY"])
          && !charName.EndsWith(WordsAbbreviations["ACCENT"])
          )
        charName = charName + WordsAbbreviations["ACCENT"];
      return true;
    }



    //  else
    //  if (longName.StartsWith("SYRIAC LETTER SUPERSCRIPT "))
    //    trailStr = longName.Substring("SYRIAC LETTER SUPERSCRIPT ".Length) + " ABOVE";
    //  else
    //  if (longName.StartsWith("SYRIAC DOTTED "))
    //    trailStr = longName.Substring("SYRIAC DOTTED ".Length) + " BELOW";


    return false;
  }

  private Tokens ConcatAboveIfPossible(Tokens tokens)
  {
    if (tokens.EndsWith("BELOW") || tokens.EndsWith("ABOVE") || tokens.EndsWith("OVERLAY"))
      return tokens;
    if (tokens.Contains("ABOVE"))
      return tokens;
    if (tokens.StartsWith("ENCLOSING"))
      return tokens;
    return tokens.Append("ABOVE");
  }

  private bool TryParseVulgarFractionName(CharInfo charInfo, int alternative, out string charName)
  {
    charName = String.Empty;
    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    var ss = SplitWords(longName, alternative);
    if (codePoint == 0x00BC)
      Debug.Assert(true);
<<<<<<< HEAD
    return TryParseFractionName(ss, alternative, out charName);
  }

  private bool TryParseFractionName(Tokens longName, int alternative, out string charName)
  {
    charName = String.Empty;
    if (longName.StartsWith("FRACTION ") || longName.StartsWith("VULGAR FRACTION "))
    {
      var ss = longName;
=======
    return TryParseFractionName(charInfo, longName, out charName, alternative);
  }

  private bool TryParseFractionName(CharInfo charInfo, string longName, out string? charName, int alternative = 0)
  {
    charName = null;
    var codePoint = charInfo.CodePoint;
    if (codePoint == 0x2150)
      Debug.Assert(true);
    if (longName.StartsWith("FRACTION ") || longName.StartsWith("VULGAR FRACTION "))
    {
      var ss = SplitWords(longName, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      var sb = new StringBuilder();
      for (int i = 1; i < ss.Count; i++)
      {
        var word = ss[i];
        if (word == "DIGIT" || word == "NUMBER" || word == "NUMERATOR")
          continue;
        if (word == "FOR")
          continue;
        var ordWord = word.EndsWith("THS") ? word.Substring(0, word.Length - 1) : word;

        if (Ordinals.TryGetValue1(ordWord, out var ordAbbr))
          sb.Append(TitleCase(ordAbbr, alternative));
        else
        if (Numerals.TryGetValue1(word, out var num))
          sb.Append(word.TitleCase().Replace(" ", ""));
        else
        if (WordsAbbreviations.TryGetValue(word, out var abbr))
          sb.Append(abbr);
        else
          sb.Append(word.TitleCase());
      }
      charName = sb.ToString();
      charName = WordsAbbreviations["FRACTION"] + @"\" + charName;
      return true;
    }
    return false;
  }

  private bool TryParseBlockDrawingsName(CharInfo charInfo, string? keyString, int alternative, out string charName)
  {
    charName = String.Empty;
    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    if (codePoint == 0x2581)
      Debug.Assert(true);
    var ss = SplitWords(longName, alternative);
    var sb = new StringBuilder();
<<<<<<< HEAD

=======
    //if (ss.Count > 2)
    //{
    //  if (Numerals.TryGetValue1(ss[1], out _))
    //  {
    //    var firstWord = ss[0];
    //    ss.RemoveAt(0);
    //    ss.Add(firstWord);
    //  }
    //}
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
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
        for (int i = keyWords.Length - 1; i >= 0; i--)
        {
          var str = keyWords[i];
          if (ss.Contains(str))
          {
            ss.Remove(str);
          }
        }
        ss.Insert(0, keyString);
      }
      else if (keyString.Contains("BLOCK"))
      {
        var str = ss.FirstOrDefault(s => s.Contains(keyString));
        if (str != null)
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

      var ordWord = word.EndsWith("THS") ? word.Substring(0, word.Length - 1) : word;

      if (Ordinals.TryGetValue1(ordWord, out var ordAbbr))
        sb.Append(TitleCase(ordAbbr, alternative));

      else if (WordsAbbreviations.TryGetValue(word, out var replacement))
      {
        sb.Append(replacement);
        if (i < ss.Count - 1)
          if (word == "BLOCK" || Functors.ContainsKey(word) || NameStarts.ContainsKey(word))
            sb.Append(@"\");
      }
      else
        sb.Append(word.TitleCase());
    }
    charName = sb.ToString();
    return true;
  }

<<<<<<< HEAD
  private bool TryParseSignWritingName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
=======
  private bool TryParseSignWritingName(CharInfo charInfo, out string? charName, int alternative = 0)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  {
    charName = string.Empty;
    string keyString = "SIGNWRITING";
    if (tokens.StartsWith(keyString))
    {
<<<<<<< HEAD
      charName = CreateMainCharName(tokens.Substring(1), alternative);
      charName = WordsAbbreviations[keyString] + @"\" + charName;
=======
      charName = CreateShortenName(charInfo, longName, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      return true;
    }
    return false;
  }

  private bool TryParseByzantineMusicalSymbol(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = string.Empty;
    string keyString = "BYZANTINE MUSICAL SYMBOL";
    if (tokens.StartsWith(keyString))
    {
      charName = CreateMainCharName(tokens.Substring(1), alternative);
      charName = WordsAbbreviations[keyString] + @"\" + charName;
      return true;
    }
    return false;
  }

  private bool TryParseMusicalSymbol(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = string.Empty;
    string keyString = "MUSICAL SYMBOL";
    if (tokens.StartsWith(keyString))
    {
      charName = CreateMainCharName(tokens.Substring(1), alternative);
      charName = WordsAbbreviations[keyString] + @"\" + charName;
      return true;
    }
    return false;
  }

  private bool TryParseCuneiformSymbol(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = string.Empty;
    string keyString = "CUNEIFORM";
    if (tokens.StartsWith(keyString))
    {
      charName = CreateMainCharName(tokens.Substring(1), alternative);
      charName = WordsAbbreviations[keyString] + @"\" + charName;
      return true;
    }
    return false;
  }

  private bool TryParseNameStartingString(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
<<<<<<< HEAD
    if (charInfo.CodePoint == 0x01C4)
      Debug.Assert(true);
    charName = string.Empty;
    if (TryFindScriptName(tokens[0], alternative, out var script))
    {
      charName = CreateLetterName(tokens.Substring(1), alternative);
      if (charName == string.Empty)
        return false;
      if (script != string.Empty)
        charName = script + @"\" + charName;
      return true;
    }
    if (TryFindStartOfName(tokens[0], alternative, out var start))
    {
      charName = CreateMainCharName(tokens.Substring(1), alternative);
      if (charName == string.Empty)
        return false;
      if (start != string.Empty)
        charName = start + @"\" + charName;
      return true;
=======
    if (charInfo.CodePoint == 0x1100)
      Debug.Assert(true);
    var longName = charInfo.Name.ToString();
    var ss = SplitWords(longName, alternative);
    foreach (var entry in NameStarts)
    {
      var key = entry.Key;
      if (ss[0] == key)
      {
        var prefix = entry.Value;
        var rest = longName.Substring(key.Length + 1);
        if (int.TryParse(rest, NumberStyles.HexNumber, null, out _))
        {
          rest = charInfo.CodePoint;
          charName = prefix + "{" + rest + "}";
          return true;
        }
        var shortName1 = CreateShortenName(charInfo, rest, alternative);
        if (shortName1.StartsWith("Sideways") && charInfo.Category == UcdCategory.Ll)
          shortName1 = shortName1.Insert("Sideways".Length, @"\");
        charName = prefix + @"\" + shortName1;
        return true;
      }
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    }
    return false;
  }

  private bool TryParseSequentialCpName(CharInfo charInfo, Tokens tokens, string keyString, int alternative, out string charName)
  {
    charName = string.Empty;

    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    if (codePoint == 0x1CF42)
      Debug.Assert(true);
    var word = tokens.First();
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

  private bool TryParseNumeralCpName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = string.Empty;

    var codePoint = charInfo.CodePoint;
    var longName = charInfo.Name.ToString();
    if (codePoint == 0x1ED2D)
      Debug.Assert(true);
    var sb = new StringBuilder();
    var isTens = tokens.Contains("TENS");
    for (int i = 0; i < tokens.Count; i++)
    {
      var word = tokens[i];
      if (TryFindScriptName(word, alternative, out var lang))
      {
        var isEmpty = sb.Length == 0;
        sb.Append(lang);
        if (isEmpty)
          sb.Append(@"\");
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

  private bool TryParseChessFigureName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    charName = string.Empty;

    var codePoint = charInfo.CodePoint;
<<<<<<< HEAD
    if (codePoint == 0x1CCBA)
      Debug.Assert(true);
    if (tokens.Contains("CHESS"))
    {
      var k = tokens.IndexOf("TURNED");
      if (k >= 0)
      {
        var item = tokens[k];
        tokens.RemoveAt(k);
        tokens.Add(item);
      }
      k = 0;
      var isQuad = tokens[k].EndsWith("QUADRANT");
=======
    var longName = charInfo.Name.ToString().Replace('-', ' ');
    if (codePoint == 0x1CCBA)
      Debug.Assert(true);
    if (longName.Contains("CHESS"))
    {
      var ss = SplitWords(longName, alternative);
      var k = ss.IndexOf("TURNED");
      if (k >= 0)
      {
        var item = ss[k];
        ss.RemoveAt(k);
        ss.Add(item);
      }
      k = 0;
      var isQuad = ss[k].EndsWith("QUADRANT");
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      //{
      //  var item = ss[k];
      //  ss.RemoveAt(k);
      //  ss.Add(item);
      //  //ss[0] = "LARGE " + ss[0];
      //}
      var sb = new StringBuilder();
<<<<<<< HEAD
      for (int i = 0; i < tokens.Count; i++)
      {
        var word = tokens[i];
=======
      for (int i = 0; i < ss.Count; i++)
      {
        var word = ss[i];
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        if (word == "TURNED")
        {
          if (WordsAbbreviations.TryGetValue("ROTATED ONE HUNDRED EIGHTY DEGREES", out var replacement))
            sb.Append(replacement);
          else if (WordsAbbreviations.TryGetValue(word, out replacement))
            sb.Append(replacement);
          else sb.Append(TitleCase(word, alternative));
        }
        else if (Numerals.TryGetValue1(word, out var number))
          sb.Append(number);
        else if (WordsAbbreviations.TryGetValue(word, out var replacement))
        {
          if (!isQuad)
            replacement = replacement.Replace("Chess", @"Chess\");
          sb.Append(replacement);
        }
        else
          sb.Append(TitleCase(word, alternative));
      }
      charName = sb.ToString();
      return true;
    }
    return false;
  }


<<<<<<< HEAD
  private bool TryParseEnclosedSymbolName(CharInfo charInfo, Tokens tokens, int alternative, out string charName)
  {
    if (charInfo.CodePoint == 0x1F10D)
      Debug.Assert(true);
    charName = string.Empty;

    if (charInfo.Decomposition?.Type == DecompositionType.Compat)
    {
      if (tokens.EndsWith("FULL STOP"))
      {
        var rest = tokens.Substring(0, tokens.Count - 1);
        charName = CreateMainCharName(rest, alternative);
        charName = charName + @"\" + WordsAbbreviations["FULL STOP"];
        return true;
      }
      if (tokens.EndsWith(" COMMA"))
      {
        var rest = tokens.Substring(0, tokens.Count - 1);
        charName = CreateMainCharName(rest, alternative);
=======
  private bool TryParseEnclosedSymbolName(CharInfo charInfo, out string? charName, int alternative)
  {
    if (charInfo.CodePoint == 0x1F10D)
      Debug.Assert(true);
    charName = null;
    var longName = charInfo.Name.ToString();
    if (charInfo.Decomposition?.Type == DecompositionType.Compat)
    {
      if (longName.EndsWith(" FULL STOP"))
      {
        var rest = longName.Substring(0, longName.Length - " FULL STOP".Length);
        charName = CreateShortenName(charInfo, rest, alternative);
        charName = charName + @"\" + WordsAbbreviations["FULL STOP"];
        return true;
      }
      if (longName.EndsWith(" COMMA"))
      {
        var rest = longName.Substring(0, longName.Length - " COMMA".Length);
        charName = CreateShortenName(charInfo, rest, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        charName = charName + @"\" + WordsAbbreviations["COMMA"];
        return true;
      }
    }
    if (charInfo.Decomposition?.Type == DecompositionType.Square)
    {
<<<<<<< HEAD
      if (tokens.StartsWith("SQUARE"))
      {
        var rest = tokens.Substring(1);
=======
      if (longName.StartsWith("SQUARE "))
      {
        var rest = longName.Substring("SQUARE ".Length);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        charName = ParseEnclosedRest(rest);
        charName = WordsAbbreviations["SQUARE"] + @"\" + charName;
        return true;
      }
    }

<<<<<<< HEAD
    if (tokens.StartsWith("SQUARED"))
    {
      var rest = tokens.Substring(1);
=======
    if (longName.StartsWith("SQUARED "))
    {
      var rest = longName.Substring("SQUARED ".Length);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      charName = ParseEnclosedRest(rest);
      charName = WordsAbbreviations["SQUARED"] + @"\" + charName;
      return true;
    }

    foreach (var entry in Enclosings)
    {
<<<<<<< HEAD
      var key = entry.Key;
      if (tokens.StartsWith(key))
      {
        var rest = tokens.Substring(1);
=======
      var key = entry.Key + " ";
      if (longName.StartsWith(key))
      {
        var rest = longName.Substring(key.Length);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        charName = ParseEnclosedRest(rest);
        charName = entry.Value + @"\" + charName;
        return true;
      }
    }
    return false;

<<<<<<< HEAD
    string ParseEnclosedRest(Tokens rest)
=======
    string ParseEnclosedRest(string rest)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    {
      foreach (var entry in Enclosed)
      {
        var key = entry.Key;
<<<<<<< HEAD
        if (rest[0] == key)
=======
        if (rest.Equals(key))
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        {
          return entry.Value;
        }
      }
<<<<<<< HEAD
      var charName = CreateMainCharName(rest, alternative);
=======
      var charName = CreateShortenName(charInfo, rest, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      if (charName.Length <= 3 && !rest.Contains("SMALL") && !charName.Contains(@"\"))
        charName = charName.ToUpper();
      return charName;
    }
  }


  /// <summary>
  /// Create a short name for a charInfo.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateShortenName(CharInfo charInfo, int alternative)
  {
    var longName = charInfo.Name.ToString();
    var tokens = SplitWords(longName, alternative);
    return CreateLetterName(tokens, alternative);
  }

  /// <summary>
  /// Create a short name for a long character name.
  /// </summary>
  private string CreateLetterName(Tokens tokens, int alternative)
  {
<<<<<<< HEAD
    var k = tokens.IndexOf("WITH");
    if (k >= 0)
    {
      var lead = tokens.Substring(0, k);
      var trail = tokens.Substring(k + 1);
      var leadStr = CreateMainCharName(lead, alternative);
      var trailStr = GetOrCreateWithModifierName(trail, alternative);
      if (trailStr.StartsWith(@"\z\"))
        trailStr = trailStr.Substring(1);
      return trailStr + leadStr;
    }
    return CreateStartCharName(tokens, alternative);
=======
    if (charInfo.CodePoint == 0x0B75)
      Debug.Assert(true);
    if (charInfo.Category.ToString()[0] == 'L' || longName.Contains("SLASH") || longName.Contains("BACKSLASH"))
    {
      var k = longName.IndexOf(" WITH ");
      if (k >= 0)
      {
        var lead = longName.Substring(0, k);
        var trail = longName.Substring(k + " WITH ".Length);
        var leadStr = CreateMainCharName(charInfo, lead, alternative);
        var trailStr = GetOrCreateWithModifierName(charInfo, trail, alternative);
        if (trailStr.StartsWith(@"\z\"))
          trailStr = trailStr.Substring(1);
        return trailStr + leadStr;
      }
    }
    return CreateStartCharName(charInfo, longName, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  }

  /// <summary>
  /// Get or create a short name for a modifier character name.
  /// </summary>
<<<<<<< HEAD
  private string GetOrCreateWithModifierName(Tokens tokens, int alternative)
  {

    var k = tokens.IndexOf("WITH");
    if (k >= 0)
    {
      var lead = tokens.Substring(0, k);
      var trail = tokens.Substring(k + 1);
      return @"\" + CreateMainCharName(lead, alternative) + GetOrCreateWithModifierName(trail, alternative);
    }
    k = tokens.IndexOf("AND");
    if (k >= 0)
    {
      var lead = tokens.Substring(0, k);
      var trail = tokens.Substring(k + 1);
      return @"\" + CreateMainCharName(lead, alternative) + GetOrCreateWithModifierName(trail, alternative);
    }
    var combName1 = "COMBINING " + tokens;
=======
  /// <param name="longName"></param>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string GetOrCreateWithModifierName(CharInfo charInfo, string longName, int alternative = 0)
  {

    var k = longName.IndexOf(" WITH ");
    if (k >= 0)
    {
      var lead = longName.Substring(0, k);
      var trail = longName.Substring(k + " WITH ".Length);
      return @"\" + CreateMainCharName(charInfo, lead, alternative) + GetOrCreateWithModifierName(charInfo, trail, alternative);
    }
    k = longName.IndexOf(" AND ");
    if (k >= 0)
    {
      var lead = longName.Substring(0, k);
      var trail = longName.Substring(k + " AND ".Length);
      return @"\" + CreateMainCharName(charInfo, lead, alternative) + GetOrCreateWithModifierName(charInfo, trail, alternative);
    }
    var combName1 = "COMBINING " + longName;
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    var combName2 = combName1 + " ACCENT";
    if (Ucd.NameIndex.TryGetValue(combName2, out var modCP) || Ucd.NameIndex.TryGetValue(combName1, out modCP))
    {
      var modName = GetOrCreateShortName(Ucd[modCP], alternative);
<<<<<<< HEAD
      if (modName.Length > 1)
        return @"\" + modName;
    }
    var result = CreateStartCharName(tokens, alternative);
=======
      if (modName != null && modName.Length > 1)
        return @"\" + modName;
    }
    var result = CreateStartCharName(charInfo, longName, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    if (result.Length > 1)
      return @"\" + result;
    return result;
  }

  /// <summary>
<<<<<<< HEAD
  /// Create a short name for start of character name.
  /// </summary>
  /// <returns></returns>
  private string CreateStartCharName(Tokens tokens, int alternative)
  {
    if (tokens.Count == 0)
      return String.Empty;
    var word = tokens[0];

    if (word != "YI" && TryFindNameStart(word, alternative, out var lang))
    {
      var rest = tokens.Substring(1);
      var shortName1 = CreateMainCharName(rest, alternative);
=======
  /// Create a short name for a simple character name.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="longName"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateStartCharName(CharInfo charInfo, string longName, int alternative = 0)
  {
    var ss = SplitWords(longName, alternative);
    var word = ss[0];

    if (word != "YI" && TryFindNameStart(word, alternative, out var lang))
    {
      var rest = longName.Substring(word.Length + 1);
      var shortName1 = CreateMainCharName(charInfo, rest, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
      if (lang != String.Empty)
        lang += @"\";
      return lang + shortName1;
    }
<<<<<<< HEAD
    return CreateMainCharName(tokens, alternative);
  }

  /// <summary>
  /// Create a short name for a main part of character name.
  /// </summary>
  private string CreateMainCharName(Tokens tokens, int alternative)
  {
    if (TryParseFractionName(tokens, alternative, out var charName))
      return charName;
    else
      return CreateSimpleCharName(tokens, alternative);
=======
    return CreateMainCharName(charInfo, longName, alternative);
  }

  /// <summary>
  /// Create a short name for a simple character name.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="longName"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateMainCharName(CharInfo charInfo, string longName, int alternative = 0)
  {
    if (TryParseFractionName(charInfo, longName, out var charName, alternative))
      return charName!;
    else
      return CreateSimpleCharName(charInfo, longName, alternative);
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148

  }

  /// <summary>
  /// Create a short name for a simple character name.
  /// </summary>
<<<<<<< HEAD
  private string CreateSimpleCharName(Tokens tokens, int alternative)
=======
  /// <param name="charInfo"></param>
  /// <param name="longName"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  private string CreateSimpleCharName(CharInfo charInfo, string longName, int alternative = 0)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  {
    //if (charInfo.CodePoint== 0xAB66)
    //  Debug.Assert(true);

<<<<<<< HEAD
    if (tokens.Count == 0)
      return String.Empty;
    var sb = new StringBuilder();

    var firstWord = tokens[0];
=======
    var sb = new StringBuilder();

    var ss = SplitWords(longName, alternative);
    var firstWord = ss[0];
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    bool isCapital = false;

    bool isSmall = false;
    if (firstWord == "SQUARE")
    {
      isCapital = tokens.Contains("CAPITAL");
      isSmall = tokens.Contains("SMALL") && !tokens.Contains("SMALL HIGH");
    }
<<<<<<< HEAD
    var isLetter = tokens.Contains("LETTER");
    var isLigature = tokens.Contains("LIGATURE");
    for (int i = 0; i < tokens.Count; i++)
=======
    var isLetter = ss.Contains("LETTER");
    var isLigature = ss.Contains("LIGATURE");
    for (int i = 0; i < ss.Count; i++)
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
    {
      var word = tokens[i];//.Replace('-', '_');

      if (word != "YI" && TryFindScriptName(word, alternative, out var lang))
      {
        var isEmpty = sb.Length == 0;
        sb.Append(lang);
        if (lang != string.Empty)
          sb.Append(@"\");
        continue;
      }

<<<<<<< HEAD
      if (tokens.Count > 1)
        if (!(word == "TO" && isLetter))
          if (TryFindWordToRemove(word, tokens, alternative))
=======
      if (ss.Count > 1)
        if (!(word == "TO" && isLetter))
          if (TryFindWordToRemove(word, longName, alternative))
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
            continue;


      if (word == "CAPITAL")
      {
        isCapital = true;
        continue;
      }

      if (word == "SMALL")
      {
        isSmall = true;
        if (isLetter || isLigature)
          continue;
        if (WordsAbbreviations.TryGetValue(word, out var small))
        {
<<<<<<< HEAD
          if (!(i < tokens.Count - 1 && tokens[i + 1].Length == 1))
=======
          if (!(i < ss.Count - 1 && ss[i + 1].Length == 1))
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
          {
            sb.Append(small);
            sb.Append(@"\");
          }
        }
        continue;
      }

      if (word == "SMALL CAPITAL")
      {
        isCapital = true;
        isSmall = true;
        continue;
      }

      if (Numerals.TryGetValue1(word, out var num))
      {
        sb.Append(TitleCase(num.ToString(), alternative));
        continue;
      }

      //if (int.TryParse(word, out _))
      //{
      //  sb.Append("-");
      //  sb.Append(word);
      //  continue;
      //}

      var ordWord = word.EndsWith("THS") ? word.Substring(0, word.Length - 1) : word;

      if (Ordinals.TryGetValue1(ordWord, out var ordAbbr))
      {
        sb.Append(TitleCase(ordAbbr, alternative));
        continue;
      }

      if (Letters.TryGetValue(word, out var letter))
      {
        sb.Append("{");
        if (isCapital)
          sb.Append(CapitalCase(letter, alternative));
        else if (isSmall)
          sb.Append(SmallCase(letter, alternative));
<<<<<<< HEAD
        else
          sb.Append(letter);
=======
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
        sb.Append("}");
        continue;
      }

      if (word != "EN" && word != "EM")
      {
        //if (AdjectiveWords.Contains(word))
        //{
        //  sb.Append(word.ToLower());
        //  continue;
        //}
        if (WordsAbbreviations.TryGetValue(word, out var replacement))
        {
          sb.Append(replacement);
          if (word == "SIDEWAYS" && isLetter && isSmall)
            sb.Append(@"\");
          else
<<<<<<< HEAD
          if (i < tokens.Count - 1 && Functors.ContainsKey(word))
=======
          if (i < ss.Count - 1 && Functors.ContainsKey(word))
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
            sb.Append(@"\");
          continue;
        }

      }

      if (isCapital)
      {
        if (isSmall)
          sb.Append(WordsAbbreviations["SMALL CAPITAL"] + @"\");
        if (word.Length == 2)
          sb.Append(UpperCase(word, alternative));
        else
        {
          if (alternative > 1)
            sb.Append(WordsAbbreviations["CAPITAL"] + @"\");
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
    var charName = sb.ToString();
    if (charName.EndsWith(@"\"))
      charName = charName.Substring(0, charName.Length - 1);
    return charName;
<<<<<<< HEAD
=======

  }
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148

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
        word = word.Replace("-", " ");
    }
    return word.TitleCase(true).Replace(" ", "");
  }

  private static string CapitalCase(string word, int alternative)
  {
    if (word.Contains('-'))
    {
      if (alternative == 0)
        word = word.Replace("-", "");
    }
    //var chars = word.ToCharArray();
    //chars[0] = char.ToUpper(chars[0]);
    return word.ToUpper();
  }

  private static string SmallCase(string word, int alternative)
  {
    if (word.Contains('-'))
    {
      if (alternative == 0)
        word = word.Replace("-", "");
    }
    return word.ToLower();
  }

  private static bool TryFindWordToRemove(string word, Tokens longName, int alternative)
  {
    if (alternative > 1)
      return false;
    if (word == "WITH")
      return true;
    if (word == "AND" && longName.First() != "LOGICAL")
      return true;
    if (WordsToRemove.TryGetValue(word, out int val))
    {
      if (alternative <= val)
        return true;
    }
    return false;
  }

  private static Tokens SplitWords(string longName, int alternative)
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
    return new Tokens(words);
  }

  private static string ReplaceStrings(string longName, int alternative)
  {
    var wordSequences = GetWordSequences(longName, alternative);
    for (int i = 0; i < wordSequences.Count; i++)
    {
      var sequence = wordSequences[i];
      if (sequence == "NINETY THOUSAND")
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
    if (alternative == 0 && (word == "LATIN" || word == "GREEK" || word == "COPTIC" || word == "HEBREW"))
    {
      scCode = "";
      return true;
    }
    if (ScriptNames.TryGetValue(word, out scCode))
    {
      return true;
    }
    return false;
  }

<<<<<<< HEAD
  private static bool TryFindStartOfName(string word, int alternative, out string scCode)
  {
    if (NameStarts.TryGetValue(word, out scCode!))
    {
      return true;
    }
    scCode = string.Empty;
    return false;
  }

=======
>>>>>>> 2700dffbc7a58be67bd3cc15b1f6088d1371f148
  private static bool TryFindNameStart(string word, int alternative, out string? scCode)
  {
    if (alternative == 0 && (word == "LATIN" || word == "GREEK" || word == "COPTIC" || word == "HEBREW"))
    {
      scCode = "";
      return true;
    }
    if (NameStarts.TryGetValue(word, out scCode))
    {
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
  private static readonly List<string> Adjectives = new();
  private static readonly Dictionary<string, string> Enclosings = new();
  private static readonly Dictionary<string, string> Enclosed = new();
  private static readonly Dictionary<string, string> Functors = new();
  private static readonly Dictionary<string, int> WordsToRemove = new();
  private static readonly BiDiDictionary<string, string> Letters = new();
  private static readonly BiDiDictionary<int, string> Numerals = new();
  private static readonly BiDiDictionary<string, string> Ordinals = new();
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


