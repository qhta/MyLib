using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

using Microsoft.Win32;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.NameGen;

/// <summary>
/// This class is responsible for generating CodePoint short names based on a set of rules and patterns.
/// </summary>
public class NameGenerator
{


  /// <summary>
  /// PredefinedNameList is a dictionary that maps Unicode code points to their predefined names.
  /// </summary>
  public required Dictionary<CodePoint, string> PredefinedNameList { get; set; }

  /// <summary>
  /// The dictionary containing first codes for each ordinal generated writing system.
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
    for (int i = 0; i < writingSystems.Count(); i++)
    {
      var ws = writingSystems[i];
      switch (ws.NameGenMethod)
      {
        case NameGenMethod.NoGeneration:
          return null;
        case NameGenMethod.Ordinal:
          return GetNameUsingOrdinalMethod(codePoint, ws);
        default:
          if (PredefinedNameList.TryGetValue(codePoint.CP, out var predefinedName))
            return predefinedName;
          break;
      }
    }
    return null;
  }

  private string? GetNameUsingOrdinalMethod(UcdCodePointViewModel codePoint, WritingSystemViewModel ws)
  {
    if (ws.Abbr == null)
      throw new ArgumentNullException(nameof(ws.Abbr), "Writing system abbreviation cannot be null.");

    var wsCodeName = ws.Abbr;

    if (!OrdinalRegions.TryGetValue(wsCodeName, out var firstCode))
    {
      firstCode = _ViewModels.Instance.UcdCodePoints.FirstOrDefault(item => item.GetWritingSystem((WritingSystemType)ws.Type!) == ws)?.CP;
      if (firstCode == null)
        throw new InvalidOperationException($"No first code point found for {ws.Name}");
      OrdinalRegions[wsCodeName] = firstCode;
    }
    var thisCode = codePoint.CP;
    return $"{ws.Abbr}_{thisCode-firstCode+1}";
  }

}