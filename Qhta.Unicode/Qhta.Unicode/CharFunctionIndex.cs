using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

using Qhta.Collections;
using Qhta.TextUtils;

namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific char function. Char function is a function Func with a parameter in braces.
/// </summary>
public class CharFunctionIndex : BiDiDictionary<CodePoint, string>
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
    //StringReplacements = new Dictionary<string, string>();
    //InitializeDictionary(StringReplacements, "GenCharNameStringRepl.txt");
    //WordsAbbreviations = new Dictionary<string, string>();
    //InitializeDictionary(WordsAbbreviations, "GenCharFuncWordAbbr.txt");
    //AdjectiveWords = new List<string>();
    //InitializeList(AdjectiveWords, "GenCharFuncAdjectives.txt");
    CreateCharFunctionsToFile("CharFunctions.txt");
    Initialized = true;
  }
  private static bool Initialized = false;

  /// <summary>
  /// Add a code point to the this.
  /// </summary>
  /// <param name="charFunc">Func is a single-word string</param>
  /// <param name="codePoint"></param>
  private void Add(string charFunc, int codePoint)
  {
    if (this.TryGetValue1(charFunc, out var value))
    {
      throw new DuplicateNameException($"CharFunc \"{charFunc}\" already exists");
    }
    else
    {
      base.Add(codePoint, charFunc);
    }
  }

  private void CreateCharFunctionsToFile(string fileFunc)
  {
    CreateCharFunctions();
    using (var writer = File.CreateText(fileFunc))
    {
      foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
      {
        if (TryGetValue(charInfo.CodePoint, out var charFunc))
          writer.WriteLine($"{charInfo.CodePoint};{charFunc}");
      }
    }
  }

  private void CreateCharFunctions()
  {
    foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
    {
      TryAddShortFunc(charInfo);
    }
  }

  private bool TryAddShortFunc(CharInfo charInfo)
  {
    var charFunc = GenerateFunction(charInfo);
    if (charFunc is null)
      return false;

    if (this.TryGetValue1(charFunc, out var existingCodePoint))
    {
      var charInfo1 = Ucd[existingCodePoint];
      Debug.WriteLine($"Conflict between code points {charInfo1.CodePoint} and {charInfo.CodePoint}. CharFunc \"{charFunc}\" already exists");
    }
    else
      Add(charInfo.CodePoint, charFunc);
    return true;
  }

  /// <summary>
  /// Create a short Func for a character.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some Functions can have alternatives</param>
  /// <returns></returns>
  public string? GenerateFunction(CharInfo charInfo, int alternative = 0)
  {
    if (charInfo.CodePoint == 0x2393)
      Debug.Assert(true);

    string? charFunc = CreateDecompositionFunction(charInfo, alternative);

    //Debug.WriteLine($"{charInfo.CodePoint};{charFunc}");
    return charFunc;
  }


  /// <summary>
  /// Create a function for a character decomposition.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <param name="alternative">Some Functions can have alternatives</param>
  /// <returns></returns>
  private string? CreateDecompositionFunction(CharInfo charInfo, int alternative = 0)
  {
    var decomposition = charInfo.Decomposition;
    if (decomposition == null)
      return null;
    var sb = new StringBuilder();
    var needsSp = false;
    foreach (var cp in decomposition.CodePoints)
    {
      if (!Ucd.TryGetValue(cp.Value, out var item))
      {
        Debug.WriteLine($"CodePoint {cp} not found in UnicodeData");
        return null;
      }

      if (item.CodePoint >= '0' && item.CodePoint <= '9')
      {
        if (needsSp)
          sb.Append(" ");
        sb.Append((char)(int)item.CodePoint);
        needsSp = false;
        continue;
      }
      var str = GenerateFunction(item, alternative);
      if (str is null)
        if (!Ucd.CharNameIndex.TryGetValue(item.CodePoint, out str))
          str = "'" + cp.ToString("X4");

      if (str.Length >1)
      {
        str = @"\" + str;
        needsSp = false;
      }
      if (needsSp)
        sb.Append(" ");
      sb.Append(str);
      needsSp = str.First()=='\\';
    }
    var sequence = sb.ToString();
    var funcName = (decomposition.Type == DecompositionType.Super) ? "sup" : decomposition.Type.ToString().ToLower();
    var result = funcName + "{" + sequence + "}";
    return result;
  }


}
