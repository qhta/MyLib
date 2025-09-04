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
  public Dictionary<CodePoint, string>? PredefinedNames { get; set; }

  /// <summary>
  /// A dictionary that maps words or parts of Unicode code point description to their abbreviations used in names.
  /// By "words" we mean single words or composite words (joined with '-'), and by "phrases" we mean multi-word phrases (joined with spaces).
  /// If the key in the input dictionary contains a space, it is considered a phrase.
  /// All spaces in the key are replaced with an underscore ('_') and it is stored in the _KnownPhrases dictionary.
  /// The new key replaces the multi-word phrase as a key in the _AbbreviatedPhrases dictionary.
  /// </summary>
  public Dictionary<string, string>? AbbreviatedPhrases
  {
    get => _AbbreviatedPhrases;
    set
    {
      if (value == null)
        return;
      _AbbreviatedPhrases = new();
      foreach (var item in value)
      {
        _AbbreviatedPhrases[item.Key] = item.Value;
        if (item.Key.Contains(' '))
          _UnbrokenPhrases.Add(item.Key);
      }
    }
  }
  private Dictionary<string, string>? _AbbreviatedPhrases;

  /// <summary>
  /// A list of adjectives that can occur in the Unicode code point description.
  /// Their abbreviations are used as function names in the generated short names.
  /// </summary>
  public HashSet<string>? Adjectives
  {
    get => _Adjectives;
    set
    {
      if (value == null)
        return;
      _Adjectives = new();
      foreach (var item in value)
      {
        _Adjectives.Add(item);
        if (item.Contains(' '))
          _UnbrokenPhrases.Add(item);
      }
    }
  }
  private HashSet<string>? _Adjectives;

  /// <summary>
  /// A list of removable phrases that can occur in the Unicode code point description.
  /// Occurrences of these phrases may be omitted with some exceptions.
  /// </summary>
  public HashSet<string>? Removables
  {
    get => _Removables;
    set
    {
      if (value == null)
        return;
      _Removables = new();
      foreach (var item in value)
      {
        _Removables.Add(item);
        if (item.Contains(' '))
          _UnbrokenPhrases.Add(item);
      }
    }
  }
  private HashSet<string>? _Removables;

  /// <summary>
  /// Dictionary of phrases that are specially treated.
  /// </summary>
  public Dictionary<string, SpecialFunction>? SpecialPhrases
  {
    get => _SpecialPhrases;
    set
    {
      if (value == null)
        return;
      _SpecialPhrases = new();
      foreach (var item in value)
      {
        _SpecialPhrases[item.Key] = item.Value;
        if (item.Key.Contains(' '))
          _UnbrokenPhrases.Add(item.Key);
      }
    }
  }
  private Dictionary<string, SpecialFunction>? _SpecialPhrases;

  /// <summary>
  /// Dictionary of special function names.
  /// </summary>
  public Dictionary<SpecialFunction, string>? SpecialFunctions
  {
    get => _SpecialFunctions;
    set
    {
      if (value == null)
        return;
      _SpecialFunctions = new();
      foreach (var item in value)
      {
        if (!String.IsNullOrEmpty(item.Value))
          _SpecialFunctions[item.Key] = "\\" + item.Value;
        else
          _SpecialFunctions[item.Key] = string.Empty;
      }
    }
  }
  private Dictionary<SpecialFunction, string>? _SpecialFunctions;

  /// A list with known multi-word phrases that should not be split.
  /// Ordered by length descending to avoid partial matches.
  private readonly SortedSet<string> _UnbrokenPhrases = new(new StringLengthComparer());

  private class StringLengthComparer : IComparer<string>
  {
    public int Compare(string? x, string? y)
    {
      if (x == null && y == null) return 0;
      if (x == null) return -1;
      if (y == null) return 1;
      int lengthComparison = y.Length.CompareTo(x.Length); // Note: y compared to x for descending order
      return lengthComparison != 0 ? lengthComparison : StringComparer.Ordinal.Compare(x, y);
    }
  }

  /// <summary>
  /// A HashSet containing all unique names. It must be initiated to all previously generated Unicode point names.
  /// </summary>
  private readonly HashSet<string> AllNames = new HashSet<string>(new NameComparer());

  /// <summary>
  /// A dictionary that maps words or parts of Unicode code point description to their abbreviations used in names.
  /// By "words" we mean single words or composite words (joined with '-'), and by "phrases" we mean multi-word phrases (joined with spaces).
  /// If the key in the input dictionary contains a space, it is considered a phrase.
  /// All spaces in the key are replaced with an underscore ('_') and it is stored in the _KnownPhrases dictionary.
  /// The new key replaces the multi-word phrase as a key in the _AbbreviatedPhrases dictionary.
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
            _UnbrokenPhrases.Add(keyPrase);
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

  #region GenerateNameWithoutWS methods
  /// <summary>
  /// Removes writing system key phrases from the description and generates a short name without writing system prefix.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <returns></returns>
  public string? GenerateNameWithoutWS(UcdCodePointViewModel codePoint)
  {
    if (_WritingSystems==null)
      throw new InvalidOperationException("Writing systems are not defined.");
    var writingSystems = codePoint.GetWritingSystems()?.ToArray();
    if (writingSystems == null || !writingSystems.Any())
    {
      // We don't report this as an error, because some code points may not have writing systems assigned.
      return null;
    }
    LinkedList<WritingSystemViewModel> wsList = new(writingSystems);
    return GenerateNameWithoutWS(codePoint, wsList.First!);
  }

  private string? GenerateNameWithoutWS(UcdCodePointViewModel codePoint, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    if (codePoint.Description== "ARABIC-INDIC CUBE ROOT")
      Debug.Assert(true);
    var ws = wsNode.Value;
    if (!String.IsNullOrEmpty(ws.KeyPhrase))
    {
      var description = codePoint.Description;
      if (description != null)
      {
        var words = SplitDescription(description);
        var keyPhrase = ws.KeyPhrase;
        if (!String.IsNullOrEmpty(keyPhrase))
        {
          if (RemoveKeyPhrase(words, keyPhrase))
            return String.Join(" ", words);
        }
      }
    }
    if (wsNode.Next!=null)
      return GenerateNameWithoutWS(codePoint, wsNode.Next);
    return null;
  }

  #endregion

  #region GenerateShortName methods
  /// <summary>
  /// Generates a short name for a given Unicode code point.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public string? GenerateShortName(UcdCodePointViewModel codePoint)
  {
    var writingSystems = codePoint.GetWritingSystems()?.ToArray();
    if (writingSystems == null || !writingSystems.Any())
    {
      Debug.WriteLine($"GenerateShortName: No writing system declared for code point {codePoint.CP}");
      return null;
    }
    LinkedList<WritingSystemViewModel> wsList = new(writingSystems);
    return GenerateShortName(codePoint, wsList.First!);
  }

  private string? GenerateShortName(UcdCodePointViewModel codePoint, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var ws = wsNode.Value;
    switch (ws.NameGenMethod)
    {
      case NameGenMethod.None:
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
      case NameGenMethod.None:
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
  #endregion

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
      firstCode = _ViewModels.Instance.UcdCodePoints.FirstOrDefault(item => item.GetWritingSystems().FirstOrDefault() == ws)!.CP;
      OrdinalRegions[abbr] = firstCode;
    }
    var thisCode = codePoint.CP;
    var name = $"\\{ws.Abbr}{thisCode - firstCode + 1}";
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
    if (PredefinedNames == null)
      return null;
    if (!PredefinedNames.TryGetValue(codePoint.CP, out var name) || String.IsNullOrEmpty((name)))
      return null;
    if (!name.StartsWith("\\"))
      name = "\\" + name;
    return EnsureUniqueName(name);
  }

  /// <summary>
  /// Creates an abbreviated name for the code point based on its description.
  /// First, it replaces any phrases defined in the _KnownPhrases dictionary with their corresponding values (where each space is replaced with '_').
  /// It prohibits splitting known phrases in the description.
  /// Next, the method splits the description into words (using spaces as separators) and checks each single or composite word against the AbbreviatedPhrases dictionary.
  /// If a word is found in the AbbreviatedPhrases dictionary, it is replaced with its abbreviation.
  /// If a word is not found, and it is a composite word (joined with '-'), it is split into single words and each single word is checked against the AbbreviatedPhrases dictionary.
  /// If a word (neither single nor composite) is not found, it is replaced with its first letter in uppercase.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <returns></returns>
  private string? CreateAbbreviatedName(UcdCodePointViewModel codePoint)
  {
    var name = CreateAbbreviatedName(SplitDescription(codePoint.Description!));
    if (String.IsNullOrEmpty(name))
      return null;
    if (name.Length > 1 && name[0] != '\\' && !char.IsAsciiDigit(name[0]))
      name = "\\" + name;
    return EnsureUniqueName(name);
  }

  private string? CreateAbbreviatedName(List<string> words)
  {
    var abbrevs = new List<string>();
    for (int i = 0; i < words.Count(); i++)
    {
      var word = words[i];
      if (Removables != null && Removables.Contains(word))
      {
        // If the word is in the list of removable phrases, we skip it
        continue;
      }
      if (SpecialPhrases != null && SpecialPhrases.TryGetValue(word, out var function))
      {
        words = words.Skip(i + 1).ToList();
        return DetectAndParseWITHClause(words, function);
      }
      if (AbbreviatedPhrases != null && AbbreviatedPhrases.TryGetValue(word, out var abbrPhrase))
      {
        if (abbrPhrase == String.Empty)
        {
          // If the abbreviation is empty, we skip this word
          continue;
        }
        abbrevs.Add(abbrPhrase);
      }
      else
      {
        var singleWords = word.Split(['-'], StringSplitOptions.RemoveEmptyEntries);
        if (singleWords.Length > 1)
        {
          var abbrevs2 = new List<string>();
          foreach (var singleWord in singleWords)
          {
            if (AbbreviatedPhrases != null && AbbreviatedPhrases.TryGetValue(singleWord, out var abbrWord) && !String.IsNullOrEmpty(abbrWord))
              abbrevs.Add(abbrWord);
            else
              abbrevs2.Add(singleWord.TitleCase());
          }
          abbrevs.Add(String.Join(" ", abbrevs2));
        }
        else
          abbrevs.Add(new string(Char.ToUpper(word.First()), 1));
      }
    }

    var name = String.Join("", abbrevs);
    return name;
  }

  /// <summary>
  /// Creates a procedural name for the code point based on its description and the writing system's key phrase.
  /// Precedes the name with a backslash and ensures it is a unique name.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <param name="wsNode"></param>
  /// <returns></returns>
  private string? CreateProceduralName(UcdCodePointViewModel codePoint, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var name = CreateProceduralName(SplitDescription(codePoint.Description!), wsNode);
    if (String.IsNullOrEmpty(name))
      return null;
    if (name.Length > 1 && name[0] != '\\' && !char.IsAsciiDigit(name[0]))
      name = "\\" + name;
    return EnsureUniqueName(name);
  }

  /// <summary>
  /// Creates a procedural name for the code point based on its description and the writing system's key phrase.
  /// </summary>
  /// <param name="words"></param>
  /// <param name="wsNode"></param>
  /// <returns></returns>
  private string? CreateProceduralName(List<string> words, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var ws = wsNode.Value;
    var keyPhrase = ws.KeyPhrase;
    if (!String.IsNullOrEmpty(keyPhrase))
    {
      RemoveKeyPhrase(words, keyPhrase);

      var name = (wsNode.Next != null) ? GenerateShortName(words, wsNode.Next) :
        CreateAbbreviatedName(words);
      if (!String.IsNullOrEmpty(name))
      {
        if (name.Length > 1 && name[0] != '\\' && !char.IsAsciiDigit(name[0]))
          name = "\\" + name;
      }
      if (!String.IsNullOrEmpty(ws.Abbr))
        return $"\\{ws.Abbr}{{{name}}}";
      if (name == null)
        return null;
      return name;

    }
    if (wsNode.Next != null)
      return CreateProceduralName(words, wsNode.Next);
    return CreateAbbreviatedName(words);
  }

  /// <summary>
  /// Removes the specified key phrase from the list of words.
  /// </summary>
  /// <param name="words"></param>
  /// <param name="keyPhrase"></param>
  /// <returns>True if the key phrase was found</returns>
  private bool RemoveKeyPhrase(List<string> words, string keyPhrase)
  {
    var result = false;
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
      {
        words.RemoveAt(0);
        result = true;
      }
    }
    else if (matchEnd)
    {
      if (words.Last() == keyPhrase)
      {
        words.RemoveAt(words.Count - 1);
        result = true;
      }
    }
    else
    {
      var wordPos = words.IndexOf(keyPhrase);
      if (wordPos >= 0)
      {
        words.RemoveAt(wordPos);
        result = true;
      }
    }
    return result;
  }

  /// <summary>
  /// Splits the description into words, taking into account known multi-word phrases that should not be split.
  /// </summary>
  /// <param name="description"></param>
  /// <returns></returns>
  private List<string> SplitDescription(string description)
  {
    description = PrepareDescription(description);
    var words = description.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < words.Length; i++)
    {
      var word = words[i];
      words[i] = words[i].Replace('_', ' ');
    }
    return words.ToList();
  }

  /// <summary>
  /// Prepares the description by replacing known multi-word phrases with underscores instead of spaces.
  /// </summary>
  /// <param name="description"></param>
  /// <returns></returns>
  private string PrepareDescription(string description)
  {
    foreach (var phrase in _UnbrokenPhrases)
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

  /// <summary>
  /// Parses a list of words when it has a WITH clause.
  /// Divides the phrase into left and right parts and parses them separately.
  /// Precedes the name with a diacritical function.
  /// If there is no WITH clause, it simply parses the left part.
  /// </summary>
  /// <param name="words"></param>
  /// <param name="letterCase"></param>
  /// <returns></returns>
  private string DetectAndParseWITHClause(List<string> words, SpecialFunction letterCase)
  {
    var withPos = words.IndexOf("WITH");
    if (withPos < 0)
    {
      var name = ParseLeftPart(words, letterCase);
      if (String.IsNullOrEmpty(name))
      {
        Debug.WriteLine($"Left phrase cannot be parsed: {String.Join(" ", words)}");
        return string.Empty;
      }
      if (name.Length > 1 && name[0] != '\\' && !char.IsAsciiDigit(name[0]))
        name = "\\" + name;
      return name;
    }
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

  /// <summary>
  /// Parses the left part of a phrase, applying any special functions as needed.
  /// </summary>
  /// <param name="words"></param>
  /// <param name="specialFunction"></param>
  /// <returns></returns>
  private string ParseLeftPart(List<string> words, SpecialFunction specialFunction)
  {
    if (words.Count == 0)
      return String.Empty;
    string name;
    if (words.Count == 1)
    {
      var word = words[0];
      if (AbbreviatedPhrases != null && AbbreviatedPhrases.TryGetValue(word, out var abbrName))
        word = abbrName;
      if (word.Length > 1 && word[0] != '\\' && !char.IsAsciiDigit(word[0]))
      {
        if (specialFunction == NameGen.SpecialFunction.UpperCase)
          name = $"\\^{word.ToUpper()}";
        else if (specialFunction == NameGen.SpecialFunction.LowerCase)
          name = $"\\{word.ToLower()}";
        else if (specialFunction == NameGen.SpecialFunction.TitleCase)
          name = $"\\^{word.TitleCase()}}}";
        else
          name = $"{{\\{word.ToLower()}}}";
      }
      else
      {
        if (specialFunction == NameGen.SpecialFunction.UpperCase)
          name = word.ToUpper();
        else if (specialFunction == NameGen.SpecialFunction.LowerCase)
          name = word.ToLower();
        else if (specialFunction == NameGen.SpecialFunction.TitleCase)
          name = word.TitleCase();
        else
          name = word.ToLower();
      }
    }
    else
    {
      var result = new List<string>();
      for (int i = 0; i < words.Count; i++)
      {
        var word = words[i];
        if (Adjectives != null && Adjectives.Contains(word))
        {
          if (AbbreviatedPhrases != null && !AbbreviatedPhrases.TryGetValue(word, out var abbrName))
          {
            abbrName = word.TitleCase();
            return CreateInlineFunction(words.Skip(i + 1).ToList(), abbrName, specialFunction);
          }
        }
        else if (SpecialPhrases != null && SpecialPhrases.TryGetValue(word, out var function))
        {
          if (function == SpecialFunction.Tone)
            return CreateToneFunction(words.Skip(i + 1).ToList(), specialFunction);
          Debug.WriteLine($"Unexpected special function {function} in this context");
        }
        else
        if (AbbreviatedPhrases != null && AbbreviatedPhrases.TryGetValue(word, out var abbrName))
        {
          if (abbrName == String.Empty)
          {
            // If the abbreviation is empty, we skip this word
            continue;
          }
          //else if (abbrName.EndsWith("{"))
          //{
          //  return CreateInlineFunction(words.Skip(i + 1).ToList(), abbrName, letterCase);
          //}
          else
            result.Add(abbrName);
        }
        else
        if (words.Count > 0)
          result.Add(word.TitleCase());
        else
          result.Add(word.ToLower());
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
      if (AbbreviatedPhrases != null && AbbreviatedPhrases.TryGetValue(name, out var abbrName))
        name = abbrName;
      //if (Char.IsLower(name.First()))
      //  name = name.TitleCase();
    }
    return name;
  }

  private string CreateToneFunction(List<string> words, SpecialFunction letterCase)
  {
    if (words.Count == 0)
      return String.Empty;
    var name = ParseLeftPart(words, letterCase);
    if (name.Length > 1 && name[0] == '\\')
      name = name.Substring(1);
    if (letterCase == NameGen.SpecialFunction.UpperCase)
      name = $"\\^Tone{name.TitleCase()}";
    else if (letterCase == NameGen.SpecialFunction.LowerCase)
      name = $"Tone{name.TitleCase()}";
    return name;
  }

  private string CreateInlineFunction(List<string> words, string function, SpecialFunction letterCase)
  {
    if (words.Count == 0)
      return String.Empty;
    if (function.EndsWith("{"))
      function = function.Substring(0, function.Length - 1);
    if (function.StartsWith("\\"))
      function = function.Substring(1);
    var name = ParseLeftPart(words, letterCase);
    name = $"\\{function}{{{name}}}";
    return name;
  }
}