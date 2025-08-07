using System.ComponentModel;
using System.Diagnostics;

using Qhta.TextUtils;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Data.Extensions;

namespace Qhta.UnicodeBuild.NameGen;

/// <summary>
/// This class is responsible for generating CodePoint short names based on a set of rules and patterns.
/// </summary>
public class NameGenerator
{
  /// <summary>
  /// Initializes a new instance of the <see cref="NameGenerator"/> class.
  /// </summary>
  public NameGenerator()
  {
    foreach (var item in _ViewModels.Instance.UcdCodePoints.Where(item => !String.IsNullOrEmpty(item.CharName)))
    {
      AllNames.Add(item.CharName!);
    }
    WritingSystems = _ViewModels.Instance.WritingSystems;

  }

  /// <summary>
  /// A dictionary that maps Unicode code points to their predefined names.
  /// </summary>
  public required Dictionary<CodePoint, string> PredefinedNames { get; set; }

  /// <summary>
  /// A dictionary that maps words or parts of Unicode code point description to their abbreviations used in names.
  /// By "words" we mean single words or composite words (joined with '-'), and by "phrases" we mean multi-word phrases (joined with spaces).
  /// If the key in the input dictionary contains a space, it is considered a phrase.
  /// All spaces in the key are replaced with an underscore ('_') and it is stored in the _KnownPhrases dictionary.
  /// The new key replaces the multi-word phrase as a key in the _AbbreviatedWords dictionary.
  /// </summary>
  public required Dictionary<string, string> AbbreviatedWords
  {
    get
    {
      var result = new Dictionary<string, string>();
      foreach (var item in _AbbreviatedWords)
      {
        if (item.Key.Contains('_'))
          result[item.Key.Replace('_', ' ')] = item.Value;
        else
          result[item.Key] = item.Value;
      }
      return result;
    }
    set
    {
      _AbbreviatedWords = new();
      foreach (var item in value)
      {
        if (item.Key.Contains(' '))
          _KnownPhrases.Add(item.Key);
        _AbbreviatedWords[item.Key] = item.Value;
      }
    }
  }

  /// A dictionary that maps words single words or composite words (joined with '-') to their abbreviations.
  private Dictionary<string, string> _AbbreviatedWords = null!;

  /// A List with known multi-word phrases.
  private readonly HashSet<string> _KnownPhrases = new();

  /// <summary>
  /// A HashSet containing all unique names. It must be initiated to all previously generated Unicode point names.
  /// </summary>
  private readonly HashSet<string> AllNames = new HashSet<string>(new NameComparer());

  /// <summary>
  /// A dictionary that maps words or parts of Unicode code point description to their abbreviations used in names.
  /// By "words" we mean single words or composite words (joined with '-'), and by "phrases" we mean multi-word phrases (joined with spaces).
  /// If the key in the input dictionary contains a space, it is considered a phrase.
  /// All spaces in the key are replaced with an underscore ('_') and it is stored in the _KnownPhrases dictionary.
  /// The new key replaces the multi-word phrase as a key in the _AbbreviatedWords dictionary.
  /// </summary>
  public WritingSystemsCollection WritingSystems
  {
    get => _WritingSystems;
    set
    {
      _WritingSystems = value;
      foreach (var item in value)
      {
        var keyPrase = item.KeyPhrase;
        if (!String.IsNullOrEmpty(keyPrase))
        {
          if (keyPrase.Contains(' '))
          {
            keyPrase = keyPrase.Replace(' ', '_');
            keyPrase = keyPrase.Replace("*", "");
            _KnownPhrases.Add(keyPrase);
          }
        }
      }
    }
  }

  private WritingSystemsCollection _WritingSystems = null!;

  /// <summary>
  /// A dictionary containing first codes for each ordinal generated writing system.
  /// </summary>
  private readonly Dictionary<String, CodePoint> OrdinalRegions = new();

  /// <summary>
  /// Generates a short name for a given Unicode code point.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public string? GenerateShortName(UcdCodePointViewModel codePoint)
  {
    var writingSystems = codePoint.GetWritingSystems(
      [WritingSystemType.Area, WritingSystemType.Script, WritingSystemType.Language, WritingSystemType.Notation, WritingSystemType.SymbolSet, WritingSystemType.Subset])?.ToArray();
    if (writingSystems == null || !writingSystems.Any())
    {
      Debug.WriteLine($"GenerateShortName: No writing system declared for code point {codePoint.CP}");
      return null;
    }
    return GenerateShortName(codePoint, writingSystems);
  }

  /// <summary>
  /// Generates a short name for a given Unicode code point based on the provided writing systems.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <param name="writingSystems"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public string? GenerateShortName(UcdCodePointViewModel codePoint, WritingSystemViewModel[] writingSystems)
  {
    LinkedList<WritingSystemViewModel> wsList = new(writingSystems);
    if (wsList.Count == 0)
    {
      Debug.WriteLine($"GenerateShortName: No writing system declared for code point {codePoint.CP}");
      return null;
    }
    return GenerateShortName(codePoint, wsList.First!);
  }

  private string? GenerateShortName(UcdCodePointViewModel codePoint, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var ws = wsNode.Value;
    switch (ws.NameGenMethod)
    {
      case NameGenMethod.NoGeneration:
        if (wsNode.Next != null)
          return GenerateShortName(codePoint, wsNode.Next);
        break;
      case NameGenMethod.Ordinal:
        return CreateOrdinalName(codePoint, ws);
      case NameGenMethod.Predefined:
        return CreatePredefinedName(codePoint, ws);
      case NameGenMethod.Abbreviating:
        return CreateAbbreviatedName(codePoint);
      case NameGenMethod.Procedural:
        return CreateProceduralName(codePoint, wsNode);
      default:
        var predefinedName = CreatePredefinedName(codePoint, ws);
        if (predefinedName != null)
          return predefinedName;
        return CreateProceduralName(codePoint, wsNode);
    }
    return null;
  }

  private string? GenerateShortName(List<string> words, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var ws = wsNode.Value;
    switch (ws.NameGenMethod)
    {
      case NameGenMethod.NoGeneration:
        if (wsNode.Next != null)
          return GenerateShortName(words, wsNode.Next);
        break;
      case NameGenMethod.Ordinal:
        Debug.WriteLine($"String \"\" can't be converted using ordinal method");
        break;
      case NameGenMethod.Predefined:
        Debug.WriteLine($"String \"\" can't be converted using predefined names");
        break;
      case NameGenMethod.Abbreviating:
        return CreateAbbreviatedName(words);
      case NameGenMethod.Procedural:
        return CreateProceduralName(words, wsNode);
      default:
        var abbreviatedName = CreateAbbreviatedName(words);
        if (abbreviatedName != null)
          return abbreviatedName;
        return CreateProceduralName(words, wsNode);
    }
    return null;
  }


  /// <summary>
  /// This method ensures that the provided name is unique by checking against the AllNames set.
  /// If the name already exists, it appends a number to the name (starting from 2) until a unique name is found.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  private string EnsureUniqueName(string name)
  {
    if (AllNames.Contains(name))
    {
      for (int i = 2; ; i++)
      {
        var newName = $"{name}{i}";
        if (!AllNames.Contains(newName))
        {
          name = newName;
          break;
        }
      }
    }
    AllNames.Add(name);
    return name;
  }

  /// <summary>
  /// Gets the ordinal name for a given Unicode code point based on the writing system.
  /// Ordinal names are generated by calculating the difference between the current code point and the first code point of the writing system,
  /// then adding 1 to that difference and appending it to the writing system abbreviation.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <param name="ws"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException">Thrown if the writing system has no abbreviation.</exception>
  private string? CreateOrdinalName(UcdCodePointViewModel codePoint, WritingSystemViewModel ws)
  {
    if (ws.Abbr == null)
      throw new ArgumentNullException(nameof(ws.Abbr), "Writing system abbreviation cannot be null.");

    var abbr = ws.Abbr;

    if (!OrdinalRegions.TryGetValue(abbr, out var firstCode))
    {
      firstCode = _ViewModels.Instance.UcdCodePoints.FirstOrDefault(item => item.GetWritingSystem((WritingSystemType)ws.Type!) == ws)!.CP;
      OrdinalRegions[abbr] = firstCode;
    }
    var thisCode = codePoint.CP;
    var name = $"{ws.Abbr}{thisCode - firstCode + 1}";
    name = EnsureUniqueName(name);
    return name;
  }

  /// <summary>
  /// Simply tries to get a predefined name for the code point from the PredefinedNames dictionary.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <param name="ws"></param>
  /// <returns></returns>
  private string? CreatePredefinedName(UcdCodePointViewModel codePoint, WritingSystemViewModel ws)
  {
    if (!PredefinedNames.TryGetValue(codePoint.CP, out var name))
      return null;
    return EnsureUniqueName(name);
  }

  /// <summary>
  /// Creates an abbreviated name for the code point based on its description.
  /// First, it replaces any phrases defined in the _KnownPhrases dictionary with their corresponding values (where each space is replaced with '_').
  /// It prohibits splitting known phrases in the description.
  /// Next, the method splits the description into words (using spaces as separators) and checks each single or composite word against the AbbreviatedWords dictionary.
  /// If a word is found in the AbbreviatedWords dictionary, it is replaced with its abbreviation.
  /// If a word is not found, and it is a composite word (joined with '-'), it is split into single words and each single word is checked against the AbbreviatedWords dictionary.
  /// If a word (neither single nor composite) is not found, it is replaced with its first letter in uppercase.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <returns></returns>
  private string? CreateAbbreviatedName(UcdCodePointViewModel codePoint)
  {
    var name = CreateAbbreviatedName(SplitDescription(codePoint.Description!));
    if (name != null)
      name = EnsureUniqueName(name);
    return name;
  }

  private string? CreateAbbreviatedName(List<string> words)
  {
    var abbrevs = new List<string>();
    for (int i = 0; i < words.Count(); i++)
    {
      var word = words[i];
      //if (word == "SQUARE BRACKET")
      //{
      //  Debug.Assert(true);
      //}
      if (AbbreviatedWords.TryGetValue(word, out var abbrPhrase))
      {
        if (abbrPhrase == String.Empty)
        {
          // If the abbreviation is empty, we skip this word
          continue;
        }
        if (abbrPhrase == "<UC>")
        {
          words = words.Skip(i + 1).ToList();
          return GetLetter(words, LetterCase.UpperCase);
        }
        else
        if (abbrPhrase == "<LC>")
        {
          words = words.Skip(i + 1).ToList();
          return GetLetter(words, LetterCase.LowerCase);
        }
        else
          abbrevs.Add(abbrPhrase);
      }
      else
      {
        var singleWords = word.Split(['-'], StringSplitOptions.RemoveEmptyEntries);
        if (singleWords.Length > 1)
        {
          foreach (var singleWord in singleWords)
          {
            if (AbbreviatedWords.TryGetValue(singleWord, out var abbrWord) && !String.IsNullOrEmpty(abbrWord))
              abbrevs.Add(abbrWord);
            else
              abbrevs.Add(singleWord);
          }
        }
        else
          abbrevs.Add(new string(Char.ToUpper(word.First()), 1));
      }
    }
    if (abbrevs.Count > 1)
      for (int i = 0; i < abbrevs.Count; i++)
      {
        var abbr = abbrevs[i];
        abbr = abbr.TitleCase();
        abbrevs[i] = abbr;
      }
    var name = String.Join("", abbrevs);
    return name;
  }

  private string? CreateProceduralName(UcdCodePointViewModel codePoint, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var name = CreateProceduralName(SplitDescription(codePoint.Description!), wsNode);
    if (name != null)
      name = EnsureUniqueName(name);
    return name;
  }

  private string? CreateProceduralName(List<string> words, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var ws = wsNode.Value;
    var keyPhrase = ws.KeyPhrase;
    if (!String.IsNullOrEmpty(keyPhrase))
    {
      var matchStart = keyPhrase.EndsWith('*');
      var matchEnd = keyPhrase.StartsWith('*');
      if (matchStart)
        keyPhrase = keyPhrase.Substring(0, keyPhrase.Length - 1);
      if (matchEnd)
        keyPhrase = keyPhrase.Substring(1, keyPhrase.Length - 1);
      keyPhrase = keyPhrase.Trim();
      if (matchStart && matchEnd)
        matchStart = matchEnd = false;
      if (matchStart)
      {
        if (words.First() == keyPhrase)
          words.RemoveAt(0);
      }
      else
      if (matchEnd)
      {
        if (words.Last() == keyPhrase)
          words.RemoveAt(words.Count - 1);
      }
      else
      {
        var wordPos = words.IndexOf(keyPhrase);
        if (wordPos >= 0)
        {
          words.RemoveAt(wordPos);
        }
      }

      var intern = (wsNode.Next != null) ? GenerateShortName(words, wsNode.Next) :
        CreateAbbreviatedName(words);
      if (!String.IsNullOrEmpty(ws.Abbr))
        return $"\\{ws.Abbr}{{{intern}}}";
      if (intern == null)
        return null;
      return intern;

    }
    if (wsNode.Next != null)
      return CreateProceduralName(words, wsNode.Next);
    return CreateAbbreviatedName(words);
  }

  private List<string> SplitDescription(string description)
  {
    description = PrepareDescription();
    var words = description.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < words.Length; i++)
    {
      var word = words[i];
      words[i] = words[i].Replace('_', ' ');
    }

    return words.ToList();

    string PrepareDescription()
    {
      foreach (var phrase in _KnownPhrases)
      {
        var itemPos = description.IndexOf(phrase);
        if (itemPos < 0)
          continue;
        var chars = description.ToCharArray();
        for (int i = itemPos; i < itemPos + phrase.Length; i++)
          if (chars[i] == ' ')
            chars[i] = '_';
        description = new string(chars);
      }
      return description;
    }
  }

  private string GetLetter(List<string> words, LetterCase letterCase)
  {
    var withPos = words.IndexOf("WITH");
    if (withPos < 0)
      return ParseLeftPart(words, letterCase);
    if (withPos == 0)
    {
      Debug.WriteLine($"WITH can't appear at the beginning of the phrase {String.Join(" ", words)}");
      return String.Empty;
    }

    var leftPart = words.Take(withPos).ToList();
    var rightPart = words.Skip(withPos + 1).ToList();
    var charName = ParseLeftPart(leftPart, letterCase);
    var diacriticalFunction = ParseRightPart(rightPart);
    return $"\\{diacriticalFunction}{{{charName}}}";
  }

  private string ParseLeftPart(List<string> words, LetterCase letterCase)
  {
    if (words.Count == 0)
      return String.Empty;
    string name;
    if (words.Count == 1)
    {
      name = words[0];
      if (letterCase == LetterCase.UpperCase)
        name = name.ToUpper();
      else if (letterCase == LetterCase.LowerCase)
        name = name.ToLower();
      else
        name = name.TitleCase();
      if (name.Length > 1)
        name = "\\" + name;
    }
    else
    {
      var result = new List<string>();
      for (int i=0; i< words.Count; i++)
      {
        var word = words[i];
        if (AbbreviatedWords.TryGetValue(word, out var abbrName))
        {
          if (abbrName == String.Empty)
          {
            // If the abbreviation is empty, we skip this word
            continue;
          }
          else if (abbrName=="<ND>")
          {
            return CreateDotlessFunction(words.Skip(i + 1).ToList(), letterCase);
          }
          else if (abbrName == "<TN>")
          {
            return CreateToneFunction(words.Skip(i + 1).ToList(), letterCase);
          }
          else
            result.Add(abbrName);
        }
        else
          result.Add(word.TitleCase());
      }
      name = "\\" + String.Join(" ", result);
    }
    return name;
  }

  private string ParseRightPart(List<string> words)
  {
    if (words.Count == 0)
      return String.Empty;
    string name;
    if (words.Count > 1)
    {
       name = String.Join(" ", words);
    }
    else
    {
      name = words[0];
      if (AbbreviatedWords.TryGetValue(name, out var abbrName))
        name = abbrName;
      if (Char.IsLower(name.First()))
        name = name.TitleCase();
    }
    return name;
  }

  private string CreateDotlessFunction(List<string> words, LetterCase letterCase)
  {
    if (words.Count == 0)
      return String.Empty;
    var name = ParseLeftPart(words, letterCase);
    if (name.Length > 1 && name[0] == '\\')
      name = name.Substring(1);
    return $"\\Dotless{{{name}}}";
  }

  private string CreateToneFunction(List<string> words, LetterCase letterCase)
  {
    if (words.Count == 0)
      return String.Empty;
    var name = ParseLeftPart(words, letterCase);
    if (name.Length > 1 && name[0] == '\\')
      name = name.Substring(1);
    if (name.Length == 0 || Char.IsDigit(name.First()))
    {
      name = $"\\Tone{name}";
      if (letterCase == LetterCase.UpperCase)
        name = name.ToUpperInvariant();
      else if (letterCase == LetterCase.LowerCase)
        name = name.ToLowerInvariant();
      return name;
    }
    return $"\\Tone{{{name}}}";

  }
}