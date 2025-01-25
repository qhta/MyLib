using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

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
    InitializeMapping(StringReplacements, "GenCharNameStringRepl.txt");
    WordsAbbreviations = new Dictionary<string, string>();
    InitializeMapping(WordsAbbreviations, "GenCharNameWordAbbr.txt");
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
    foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
    {
      var charName = CreateShortName(charInfo);
      if (charName is not null)
      {
        if (this.TryGetValue(charName, out var existingCodePoint))
        {
          const int maxAlternative = 2;
          for (int alternative = 1; alternative <= maxAlternative; alternative++)
          {
            var altName2 = CreateShortName(charInfo, alternative);
            if (altName2 != null && altName2 != charName)
            {
              this.Add(altName2, charInfo.CodePoint);
              break;
            }
            else
            {
              var charInfo1 = Ucd[existingCodePoint];
              var altName1 = CreateShortName(charInfo1, alternative);
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
                //charName = charName + alternative.ToString();
                //Add(charName, charInfo.CodePoint);
              }
            }
          }
        }
        else
          Add(charName, charInfo.CodePoint);
      }
    }
    using (var writer = File.CreateText(filename))
    {
      foreach (var item in this.OrderBy(item => (int)item.Value))
      {
        writer.WriteLine($"{item.Value};{item.Key}");
      }
    }
  }

  /// <summary>
  /// Create a short name for a character.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some names can have alternatives</param>
  /// <returns></returns>
  public string? CreateShortName(CharInfo charInfo, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x166D)
      Debug.Assert(true);

    string longName = charInfo.Name;
    string? charName = null;

    if (charInfo.CodePoint == 0x007F)
      charName = "DEL";
    else if (charInfo.Category == UcdCategory.Cc || charInfo.CodePoint == 0x020 || longName.StartsWith("<"))
      charName = CreateAbbreviationCharName(charInfo);
    else if (charInfo.Category.ToString()[0] == 'Z')
      charName = CreateShortenName(charInfo, alternative);
    else if (longName.StartsWith("MODIFIER LETTER ") || charInfo.Category == UcdCategory.Mn)
      charName = CreateModifierLetterName(longName, alternative);
    else if (longName.StartsWith("COMBINING "))
      charName = CreateCombiningCharName(longName, alternative);
    else if (longName.StartsWith("VULGAR FRACTION "))
      charName = CreateVulgarFractionName(longName, alternative);
    else if (longName.StartsWith("PRESENTATION FORM "))
      charName = CreatePresentationFormName(longName, alternative);
    else if (longName.StartsWith("FULLWIDTH "))
      charName = CreateFullWidthName(longName, alternative);
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
      return CreateShortenName(longName.Substring(1, longName.Length - 2).ToUpper());
    return charName;
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
    var words = longName.Split([' ', ',', '-','_'], StringSplitOptions.RemoveEmptyEntries).ToList();
    for (int i = words.Count - 1; i >= 0; i--)
    {
      var word = words[i];
      if (TryFindWordToRemove(word, alternative))
        words.RemoveAt(i);
    }
    if (words.Count == 1)
    {
      if (WordsAbbreviations.TryGetValue(words[0], out var shortWord))
        return shortWord;
      return words[0].ToLower();
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

      if (word == "LETTER" || word == "LIGATURE")
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
    return CreateShortenName(longName, alternative);
  }

  private string CreateModifierLetterName(string longName, int alternative = 0)
  {
    var k = longName.IndexOf("MODIFIER LETTER ");
    if (k >= 0)
      longName = longName.Replace("MODIFIER LETTER ", "");
    var result = CreateShortenName(longName, alternative);
    if (k>=0)
      result = "mod" + result;
    return result;
  }

  private string CreateCombiningCharName(string longName, int alternative = 0)
  {
    longName = longName.Replace("COMBINING ","");
    longName = longName.Replace(" AND ", " ");
    longName = longName.Replace('-', ' ');
    return "comb" + CreateShortenName(longName, alternative);
  }

  private string CreateVulgarFractionName(string longName, int alternative = 0)
  {
    longName = longName.Replace("VULGAR FRACTION ", "");
    return CreateShortenName(longName, alternative);
  }

  private string CreatePresentationFormName(string longName, int alternative = 0)
  {
    longName = longName.Replace("PRESENTATION FORM FOR VERTICAL ", "VERTICAL");
    return CreateShortenName(longName, alternative);
  }

  private string? CreateFullWidthName(string longName, int alternative = 0)
  {
    longName = longName.Replace("FULLWIDTH ", "");
    return "wide" + CreateShortenName(longName, alternative);
  }

  private string CreateShortenName(string longName, int alternative = 0)
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
      if (word == "LETTER")
        continue;
      if (word == "LIGATURE")
        continue;
      if (word == "SYMBOL")
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

      if (WordsAbbreviations.TryGetValue(word, out var replacement) && replacement.Length > 2)
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
    if (word== "CANADIAN")
      word = "Canadian Aboriginal";
    else
      word = word.Replace('_', ' ').TitleCase(true);
    if (ScriptCodes.UcdScriptNames.TryGetValue1(word, out scCode))
    {
      if (alternative == 0)
      {
        if (scCode is "Latn" or "Grek" or "Hebr" or "Arab")
          scCode = "";
      }
      return true;
    }
    return false;
  }

  private static readonly Dictionary<string, int> WordsToRemove = new()
  {
    {"SIGN", 1},
    {"MARK", 1},
    {"DIGIT", 1},
    {"WITH", 0 },
    {"COMMERCIAL", 0},
    {"THAN", 0},
  };

  private static readonly List<string> AdjectiveWords = new()
  {
    "AFRICAN",
    "FINAL",
    "INITIAL",
    "ISOLATED",
  };

  private static Dictionary<string, string> StringReplacements = null!;
  private static Dictionary<string, string> WordsAbbreviations = null!;

  private static void InitializeMapping(Dictionary<string, string> mapping, string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split(';');
          if (parts.Length == 2)
          {
            mapping.Add(parts[0], parts[1]);
          }
        }
      }
    }

  }
}
