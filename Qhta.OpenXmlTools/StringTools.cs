using System.Text;
using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Extending methods for strings.
/// </summary>
public static class StringTools
{

  /// <summary>
  /// Encodes a string using html entities.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string HtmlEncode(this string str)
  {
    var sb = new StringBuilder();
    foreach (var c in str)
    {
      if (c == '&')
        sb.Append("&amp;");
      else
      if (c == '<')
        sb.Append("&lt;");
      else
      if (c == '>')
        sb.Append("&lt;");
      else sb.Append(c);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Indent a string.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="startIndent"></param>
  /// <param name="indentUnit"></param>
  /// <param name="lineSeparator"></param>
  /// <returns></returns>
  public static string IndentString(this string str, int startIndent=0, string indentUnit="  ", string lineSeparator = "\r\n")
  {
    var indentLevel = startIndent;
    var sl = new List<string>();
    var k = 0;
    k = str.IndexOf("><", k);
    if (k<0)
      k = str.Length;
    var i = 0;
    while (k < str.Length)
    {
      sl.Add(str.Substring(i, k + 1 - i));
      i = k + 1;
      k = str.IndexOf("><", i);
      if (k < 0)
        k = str.Length;
    }
    sl.Add(str.Substring(i));
    for (var j = 0; j < sl.Count; j++)
    {
      var s = sl[j];
      if (s.StartsWith("</"))
        indentLevel--;
      if (indentLevel > 0)
        sl[j] = indentUnit.Duplicate(indentLevel) + sl[j];
      if (s.StartsWith("<") && !s.Contains("</") && !s.EndsWith("/>"))
        indentLevel++;
    }
    return string.Join(lineSeparator, sl);
  }
  
  /// <summary>
  /// Get the number of non-whitespace characters at the beginning of the string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static int LeftIndentLength(this string str)
  {
    int index = 0;
    while (index < str.Length && char.IsWhiteSpace(str[index]))
      index++;
    return index;
  }

  /// <summary>
  /// Checks if a string has a substring at a given position.
  /// </summary>
  /// <param name="s"></param>
  /// <param name="pos"></param>
  /// <param name="substring"></param>
  /// <returns></returns>
  public static bool HasSubstringAt(this string s, int pos, string substring) =>
    (pos >= 0 && pos + substring.Length <= s.Length) && s.Substring(pos, substring.Length).Equals(substring);


  /// <summary>
  /// Determines if a string consists of digits only.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static bool IsNumber(this string str)
  {
    return str.ToCharArray().All(char.IsDigit);
  }

  /// <summary>
  /// Replaces all the whitespaces with a single space.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string NormalizeWhitespaces(this string str)
  {
    var chars = str.ToList();
    var wasChar = false;
    for (int i = 0; i < chars.Count; i++)
    {
      if (char.IsWhiteSpace(chars[i]))
      {
        if (wasChar)
        {
          chars.RemoveAt(i);
          i--;
        }
        else
        {
          chars[i] = ' ';
          wasChar = true;
        }
      }
      else
        wasChar = false;
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Removes all the whitespaces.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string RemoveWhitespaces(this string str)
  {
    var chars = str.ToList();
    for (int i = 0; i < chars.Count; i++)
    {
      if (char.IsWhiteSpace(chars[i]))
      {
        chars.RemoveAt(i);
        i--;
      }
    }
    return new string(chars.ToArray());
  }


  /// <summary>
  /// Insert a soft hyphen character into long words in a string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string FixLongWords(this string str)
  {
    var newTextValue = str;
    var words = str.Split(' ');
    foreach (var word in words)
    {
      if (word.Length > 30)
      {
        var newWord = FixLongWord(word);
        newTextValue = newTextValue.Replace(word, newWord);
      }
    }
    return newTextValue;
  }

  private static string FixLongWord(string word)
  {
    var chars = word.ToList();
    for (int i = 10; i < chars.Count - 10; i++)
    {
      if (chars[i] == '/' && char.IsLetter(chars[i + 1]))
        chars.Insert(i + 1, '\u00AD');
      else if (char.IsUpper(chars[i]) && char.IsLower(chars[i - 1]))
        chars.Insert(i, '\u00AD');
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Removes a numbering from the string (if it begins the string).
  /// </summary>
  /// <param name="str"></param>
  /// <param name="result"></param>
  /// <returns></returns>
  public static bool TryRemoveNumbering(this string str, out string result)
  {
    var numStr = str.GetNumberingString();
    if (numStr != null)
    {
      //if (str.Length > numStr.Length && str[numStr.Length - 1] == '.')
      //  str = str.Remove(numStr.Length - 1, 1);
      result = str.Substring(numStr.Length).TrimStart();
      return true;
    }
    result = str;
    return false;
  }

  /// <summary>
  /// Get the list of sentences from the string.
  /// Sentences are separated by '.', '!', '?' or ':'
  /// followed by a space (or standing at the end of the string).
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static List<string> GetSentences(this string str)
  {
    var sentences = new List<string>();
    var k = 0;
    str = str.Trim();
    while (k >= 0 && k < str.Length)
    {
      k = str.IndexOfAny(['.', '!', '?', ':'], k);
      if (k == -1)
        break;
      if (k == str.Length - 1)
      {
        var s1 = str.Substring(k + 1).TrimStart();
        sentences.Add(s1);
        break;
      }
      if (k + 1 < str.Length && str[k + 1] == ' ')

      {
        var s1 = str.Substring(0, k + 1);
        sentences.Add(s1);
        str = str.Substring(k + 1).TrimStart();
      }
      else
        k = k + 1;
    }
    return sentences;
  }

  /// <summary>
  /// Replace characters in a string with a code between F000 and F0DD to corresponding unicode characters according to symbol encoding.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string ReplaceSymbolEncoding(this string str)
  {
    if (str.Length==1)
      Debug.Assert(true);
    var chars = str.ToCharArray();
    for (int i = 0; i < chars.Length; i++)
    {
      var charCode = (int)chars[i];
      if (charCode >= 0xF000 && charCode <= 0xF0FF)
      {
        charCode = charCode - 0xF000;
        if (SymbolEncodingDictionary.TryGetValue(charCode, out char c))
        {
          chars[i] = c;
        }
      }
    }
    return new string(chars);
  }

  private static readonly Dictionary<int, char> SymbolEncodingDictionary = new()
  {
    { 33, '!' },
    { 34, '∀' },
    { 35, '#' },
    { 36, '∃' },
    { 37, '%' },
    { 38, '&' },
    { 39, '∍' },
    { 40, '(' },
    { 41, ')' },
    { 42, '*' },
    { 43, '+' },
    { 44, ',' },
    { 45, '-' },
    { 46, '.' },
    { 47, '/' },
    { 48, '0' },
    { 49, '1' },
    { 50, '2' },
    { 51, '3' },
    { 52, '4' },
    { 53, '5' },
    { 54, '6' },
    { 55, '7' },
    { 56, '8' },
    { 57, '9' },
    { 58, ':' },
    { 59, ';' },
    { 60, '<' },
    { 61, '=' },
    { 62, '>' },
    { 63, '?' },
    { 64, '≅' },
    { 65, 'Α' },
    { 66, 'Β' },
    { 67, 'Χ' },
    { 68, 'Δ' },
    { 69, 'Ε' },
    { 70, 'Φ' },
    { 71, 'Γ' },
    { 72, 'Η' },
    { 73, 'Ι' },
    { 74, 'ϑ' },
    { 75, 'Κ' },
    { 76, 'Λ' },
    { 77, 'Μ' },
    { 78, 'Ν' },
    { 79, 'Ο' },
    { 80, 'Π' },
    { 81, 'Θ' },
    { 82, 'Ρ' },
    { 83, 'Σ' },
    { 84, 'Τ' },
    { 85, 'Υ' },
    { 86, 'ς' },
    { 87, 'Ω' },
    { 88, 'Ξ' },
    { 89, 'Ψ' },
    { 90, 'Ζ' },
    { 91, '[' },
    { 92, '∴' },
    { 93, ']' },
    { 94, '⊥' },
    { 95, '_' },
    { 97, 'α' },
    { 98, 'β' },
    { 99, 'χ' },
    { 100, 'δ' },
    { 101, 'ε' },
    { 102, 'φ' },
    { 103, 'γ' },
    { 104, 'η' },
    { 105, 'ι' },
    { 106, 'ϕ' },
    { 107, 'κ' },
    { 108, 'λ' },
    { 109, 'μ' },
    { 110, 'ν' },
    { 111, 'ο' },
    { 112, 'π' },
    { 113, 'θ' },
    { 114, 'ρ' },
    { 115, 'σ' },
    { 116, 'τ' },
    { 117, 'υ' },
    { 118, 'Ϣ' },
    { 119, 'ω' },
    { 120, 'ξ' },
    { 121, 'ψ' },
    { 122, 'ζ' },
    { 123, '{' },
    { 124, '|' },
    { 125, '}' },
    { 126, '~' },
    { 160, '€' },
    { 161, 'ϒ' },
    { 162, '′' },
    { 163, '≤' },
    { 164, '⁄' },
    { 165, '∞' },
    { 166, 'ƒ' },
    { 167, '♣' },
    { 168, '♦' },
    { 169, '♥' },
    { 170, '♠' },
    { 171, '↔' },
    { 172, '←' },
    { 173, '↑' },
    { 174, '→' },
    { 175, '↓' },
    { 176, '°' },
    { 177, '±' },
    { 178, '″' },
    { 179, '≥' },
    { 180, '×' },
    { 181, '∝' },
    { 182, '∂' },
    { 183, '•' },
    { 184, '÷' },
    { 185, '≠' },
    { 186, '≡' },
    { 187, '≈' },
    { 188, '…' },
    { 189, '∣' },
    { 190, '⎯' },
    { 191, '↵' },
    { 192, 'ℵ' },
    { 193, 'ℑ' },
    { 194, 'ℜ' },
    { 195, '℘' },
    { 196, '⊗' },
    { 197, '⊕' },
    { 198, '∅' },
    { 199, '∩' },
    { 200, '∪' },
    { 201, '⊃' },
    { 202, '⊇' },
    { 203, '⊄' },
    { 204, '⊂' },
    { 205, '⊈' },
    { 206, '∈' },
    { 207, '∉' },
    { 208, '∠' },
    { 209, '∇' },
    { 210, '®' },
    { 211, '©' },
    { 212, '™' },
    { 213, '∏' },
    { 214, '√' },
    { 215, '⋅' },
    { 216, '¬' },
    { 217, '∧' },
    { 218, '∨' },
    { 219, '⇔' },
    { 220, '⇐' },
    { 221, '⇑' },
    { 222, '⇒' },
    { 223, '⇓' },
    { 224, '◊' },
    { 225, '〈' },
    { 226, '®' },
    { 227, '©' },
    { 228, '™' },
    { 229, '∑' },
    { 230, '⎛' },
    { 231, '⎜' },
    { 232, '⎝' },
    { 233, '⎡' },
    { 234, '⎢' },
    { 235, '⎣' },
    { 236, '⎧' },
    { 237, '⎨' },
    { 238, '⎩' },
    { 239, '⎪' },
    { 241, '〉' },
    { 242, '∫' },
    { 243, '⌠' },
    { 244, '⎮' },
    { 245, '⌡' },
    { 246, '⎞' },
    { 247, '⎟' },
    { 248, '⎠' },
    { 249, '⎤' },
    { 250, '⎥' },
    { 251, '⎦' },
    { 252, '⎫' },
    { 253, '⎬' },
    { 254, '⎭' },
  };
}