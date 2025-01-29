using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text;

using Qhta.Collections;

namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific char function.
/// Char function is a function name with a parameter in braces
/// or a rich text string in braces.
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
      foreach (var entry in this.OrderBy(item => item.Value))
      {
        writer.WriteLine($"{entry.Key};{entry.Value}");
      }
      //foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
      //{
      //  if (TryGetValue(charInfo.CodePoint, out var charFunc))
      //    writer.WriteLine($"{charInfo.CodePoint};{charFunc}");
      //}
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
    if (charInfo.CodePoint == 0xFB21)
      Debug.Assert(true);
    var sb = new StringBuilder();
    var needsSp = false;
    var codePoints = decomposition.CodePoints.ToArray().ToList();
    string longName = charInfo.Name;
    var isParenthesized = false;

    if (longName.StartsWith("PARENTHESIZED"))
    {
      if (codePoints.First() == '(' && codePoints.Last() == ')')
      {
        codePoints.RemoveAt(0);
        codePoints.RemoveAt(codePoints.Count - 1);
        isParenthesized = true;
      }
    }
    foreach (var cp in codePoints)
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

      if (str.Length > 1 && !str.StartsWith("{"))
      {
        str = @"\" + str;
        needsSp = false;
      }
      if (needsSp)
        sb.Append(" ");
      if (str.StartsWith("{") && str.EndsWith("}"))
        sb.Append(str.Substring(1, str.Length - 2));
      else
        sb.Append(str);
      needsSp = str.First() == '\\';
    }
    var sequence = sb.ToString();
    if (sequence.StartsWith("{") && sequence.StartsWith("}"))
      return sequence.Substring(1, sequence.Length - 1);
    if (decomposition.Type == DecompositionType.None)
      return "{" + sequence + "}";

    string funcName;
    if (decomposition.Type == DecompositionType.Super)
      funcName = "sup";
    else if (decomposition.Type == DecompositionType.Vertical)
      funcName = "vert";
    else if (decomposition.Type == DecompositionType.Font)
    {
      funcName = "";
      var words = longName.Split([' '], StringSplitOptions.RemoveEmptyEntries);
      foreach (var word in words)
      {
        if (word == "ALTERNATIVE")
          funcName = @"\alt";
        else if (word == "BLACK-LETTER")
          funcName = @"\fraktur";
        else if (word == "DOUBLE-STRUCK")
          funcName += @"\dstruck";
        else if (word == "MATHEMATICAL" || word == "CONSTANT")
          funcName += @"\math";
        else if (word == "BOLD")
          funcName += @"\b";
        else if (word == "ITALIC")
          funcName += @"\i";
        else if (word == "SCRIPT")
          funcName += @"\script";
        else if (word == "OUTLINED")
          funcName += @"\outl";
        else if (word == "WIDE")
          funcName += @"\wide";
        else if (word == "SEGMENTED")
          funcName += @"\segm";
      }
      if (funcName == "")
        funcName = "font";
      else
        funcName = funcName.Substring(1);
    }
    else if (decomposition.Type == DecompositionType.Nobreak)
      funcName = "nobreak";
    else if (decomposition.Type == DecompositionType.Initial)
      funcName = "init";
    else if (decomposition.Type == DecompositionType.Medial)
      funcName = "med";
    else if (decomposition.Type == DecompositionType.Final)
      funcName = "final";
    else if (decomposition.Type == DecompositionType.Isolated)
      funcName = "isol";
    else if (decomposition.Type == DecompositionType.Compat)
    {
      if (isParenthesized)
        funcName = "parenthesized";
      else
      if (longName.Contains("LIGATURE"))
        funcName = "ligature";
      else
      if (longName.Contains("DIGRAPH") || longName.Contains("LETTER"))
        funcName = "digraph";
      else
      if (longName.Contains("ROMAN"))
        funcName = "roman";
      else
        funcName = "compat";
    }
    else
      funcName = decomposition.Type.ToString().ToLower();
    var result = funcName + "{" + sequence + "}";
    return result;
  }


}
