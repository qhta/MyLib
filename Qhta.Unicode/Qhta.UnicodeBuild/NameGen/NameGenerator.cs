using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

using Microsoft.Win32;

using Qhta.TextUtils;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Windows.Controls.Input;

namespace Qhta.UnicodeBuild.NameGen;

/// <summary>
/// This class is responsible for generating CodePoint short names based on a set of rules and patterns.
/// </summary>
public class NameGenerator
{

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
      _KnownPhrases = new();
      foreach (var item in value)
      {
        if (item.Key.Contains(' '))
        {
          var newKey = item.Key.Replace(' ', '_');
          _KnownPhrases[item.Key] = newKey;
          _AbbreviatedWords[newKey] = item.Value;
        }
        else
          _AbbreviatedWords[item.Key] = item.Value;
      }
    }
  }

  /// <summary>
  /// A dictionary that maps numeral words or parts of Unicode code point description to their digital values.
  /// Values are stored in _AbbreviatedWords and _KnownPhrases dictionary with the same rules as for AbbreviatedWords.
  /// </summary>
  public Dictionary<string, string>? KnownNumerals
  {
    get => _KnownNumerals;
    set
    {
      if (KnownNumerals != _KnownNumerals)
      {
        _KnownNumerals = value;
        if (value!=null)
        {
          foreach (var item in value)
          {
            if (item.Key.Contains(' '))
            {
              var newKey = item.Key.Replace(' ', '_');
              _KnownPhrases[item.Key] = newKey;
              _AbbreviatedWords[newKey] = item.Value;
            }
            else
              _AbbreviatedWords[item.Key] = item.Value;
          }
        }
      }
    }
  }

  private Dictionary<string, string>? _KnownNumerals;

  /// A dictionary that maps words single words or composite words (joined with '-') to their abbreviations.
  private Dictionary<string, string> _AbbreviatedWords = null!;

  /// A dictionary that maps words multi-word phrases to their new key phrases stored with underscores ('_').
  private Dictionary<string, string> _KnownPhrases = null!;

  /// <summary>
  /// A HashSet containing all unique names. It must be initiated to all previously generated Unicode point names.
  /// </summary>
  public required HashSet<string> AllNames { get; set; }

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
        if (wsNode.Next!=null)
          return GenerateShortName(codePoint, wsNode.Next);
        break;
      case NameGenMethod.Ordinal:
        return GetOrdinalName(codePoint, ws);
      case NameGenMethod.Predefined:
        return GetPredefinedName(codePoint, ws);
      case NameGenMethod.Abbreviating:
        return GetAbbreviatedName(codePoint, ws);
      case NameGenMethod.Procedural:
        return GetProceduralName(codePoint, wsNode);
      default:
        var predefinedName = GetPredefinedName(codePoint, ws);
        if (predefinedName != null)
          return predefinedName;
        var abbreviatedName = GetAbbreviatedName(codePoint, ws);
        if (abbreviatedName != null)
          return abbreviatedName;
        return GetProceduralName(codePoint, wsNode);
    }
    return null;
  }

  private string? GenerateShortName(string phrase, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var ws = wsNode.Value;
    switch (ws.NameGenMethod)
    {
      case NameGenMethod.NoGeneration:
        if (wsNode.Next != null)
          return GenerateShortName(phrase, wsNode.Next);
        break;
      case NameGenMethod.Ordinal:
        Debug.WriteLine($"String \"\" can't be converted using ordinal method");
        break;
      case NameGenMethod.Predefined:
        Debug.WriteLine($"String \"\" can't be converted using predefined names");
        break;
      case NameGenMethod.Abbreviating:
        return GetAbbreviatedName(phrase, ws);
      case NameGenMethod.Procedural:
        return GetProceduralName(phrase, wsNode);
      default:
        var abbreviatedName = GetAbbreviatedName(phrase, ws);
        if (abbreviatedName != null)
          return abbreviatedName;
        return GetProceduralName(phrase, wsNode);
    }
    return null;
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
  private string? GetOrdinalName(UcdCodePointViewModel codePoint, WritingSystemViewModel ws)
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
    return $"{ws.Abbr}_{thisCode - firstCode + 1}";
  }

  /// <summary>
  /// Simply tries to get a predefined name for the code point from the PredefinedNames dictionary.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <param name="ws"></param>
  /// <returns></returns>
  private string? GetPredefinedName(UcdCodePointViewModel codePoint, WritingSystemViewModel ws)
  {
    return PredefinedNames.GetValueOrDefault(codePoint.CP);
  }

  /// <summary>
  /// Creates an abbreviated name for the code point based on its description.
  /// First, it replaces any phrases defined in the _KnownPhrases dictionary with their corresponding values (where each space is replaced with '_').
  /// It prohibits splitting known phrases in the description.
  /// Next, the method splits the description into words (using spaces as separators) and checks each single or composite word against the AbbreviatedWords dictionary.
  /// If a word is found in the AbbreviatedWords dictionary, it is replaced with its abbreviation.
  /// If a word is not found, and it is a composite word (joined with '-'), it is split into single words and each single word is checked against the AbbreviatedWords dictionary.
  /// If a word (neither single nor composite) is not found, it is replaced with its first letter in uppercase.
  /// At the end, if the generated name already exists in the AllNames set, it appends a number (starting with 2) to make it unique.
  /// </summary>
  /// <param name="codePoint"></param>
  /// <param name="ws"></param>
  /// <returns></returns>
  private string? GetAbbreviatedName(UcdCodePointViewModel codePoint, WritingSystemViewModel ws) => GetAbbreviatedName(codePoint.Description, ws);

  private string? GetAbbreviatedName(string? description, WritingSystemViewModel ws)
  {
    if (String.IsNullOrEmpty(description))
      return null;
    foreach (var item in _KnownPhrases)
    {
      if (String.IsNullOrEmpty(item.Key) || String.IsNullOrEmpty(item.Value))
        continue;
      description = description.Replace(item.Key, item.Value, StringComparison.OrdinalIgnoreCase);
    }
    var words = description!.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    var sb = new StringBuilder();
    foreach (var word in words)
    {
      if (AbbreviatedWords.TryGetValue(word, out var abbrPhrase) && !String.IsNullOrEmpty(abbrPhrase))
        sb.Append(abbrPhrase);
      else
      {
        var singleWords = word.Split(['-'], StringSplitOptions.RemoveEmptyEntries);
        if (singleWords.Length > 1)
        {
          foreach (var singleWord in singleWords)
          {
            if (AbbreviatedWords.TryGetValue(singleWord, out var abbrWord) && !String.IsNullOrEmpty(abbrWord))
              sb.Append(abbrWord);
            else
              sb.Append(singleWord);
          }
        }
        else
          sb.Append(Char.ToUpper(word.First()));
      }
    }
    var result = sb.ToString();
    if (AllNames.Contains(result))
    {
      for (int i = 2; ; i++)
      {
        var newResult = $"{result}{i}";
        if (!AllNames.Contains(newResult))
        {
          result = newResult;
          break;
        }
      }
    }
    AllNames.Add(result);
    return result;
  }


  private string? GetProceduralName(UcdCodePointViewModel codePoint, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    string description = codePoint.Description!;
    return GetProceduralName(description, wsNode);
  }

  private string? GetProceduralName(string phrase, LinkedListNode<WritingSystemViewModel> wsNode)
  {
    var ws = wsNode.Value;
    var keyPhrase = ws.KeyPhrase;
    if (!String.IsNullOrEmpty(keyPhrase))
    {
      if (!keyPhrase.Contains('*'))
        keyPhrase = '*' + keyPhrase + '*';
      if (phrase.IsLike(keyPhrase, out var wildKey) && wildKey!=null)
      {
        var wKeys = wildKey.Split(['*'], StringSplitOptions.RemoveEmptyEntries);
        if (wKeys.Length == 1)
        {
          var intern = (wsNode.Next != null) ? GenerateShortName(wildKey.Trim(), wsNode.Next) : phrase;
          if (!String.IsNullOrEmpty(ws.Abbr))
            return $"\\{ws.Abbr}{{{intern}}}";
          else
            return $"\\{{{intern}}}";
        }
        else
        {
          foreach (var wKey in wKeys)
          {

          }
        }
      }
      return phrase;
    }
    if (wsNode.Next != null)
      return GetProceduralName(phrase, wsNode.Next);
    return null;
  }


}