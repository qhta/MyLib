using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A bunch of string utility methods
/// </summary>
public static class OpenXmlStringUtils
{
  private static readonly (char open, char close)[] DefaultBraces =
  {
    ('(', ')'),
    ('[', ']'),
    ('{', '}')
  };

  /// <summary>
  /// Changes a number to literal text using whole part and a rational fraction part.
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  public static string NumberToText(this double number)
  {
    var intPart = (Int64)number;
    var frac = number - intPart;

    var result = intPart.NumberToText();
    if (frac != 0)
    {
      var cents = (int)frac * 100;
      result += " " + cents + "/100";
    }
    return result;
  }

  /// <summary>
  /// Changes a number to literal text.
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  public static string NumberToText(this int number) => ((Int64)number).NumberToText();

  /// <summary>
  /// Changes a number to literal text.
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  public static string NumberToText(this Int64 number)
  {
    if (number == 0)
      return "zero";

    if (number < 0)
      return "minus " + Math.Abs(number).NumberToText();

    var words = "";
    // 9223372036854775808
    if (number / 1000000000000000000 > 0)
    {
      words += (number / 1000000000000000000).NumberToText() + " quintillion ";
      number %= 1000000000000000000;
    }

    if (number / 1000000000000000 > 0)
    {
      words += (number / 1000000000000000).NumberToText() + " quadrillion ";
      number %= 1000000000000000;
    }

    if (number / 1000000000000 > 0)
    {
      words += (number / 1000000000000).NumberToText() + " trillion ";
      number %= 1000000000000;
    }

    if (number / 1000000000 > 0)
    {
      words += (number / 1000000000).NumberToText() + " billion ";
      number %= 1000000000;
    }

    if (number / 1000000 > 0)
    {
      words += (number / 1000000).NumberToText() + " million ";
      number %= 1000000;
    }

    if (number / 1000 > 0)
    {
      words += (number / 1000).NumberToText() + " thousand ";
      number %= 1000;
    }

    if (number / 100 > 0)
    {
      words += (number / 100).NumberToText() + " hundred ";
      number %= 100;
    }

    if (number > 0)
    {
      if (words != "")
        words += "and ";

      var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
      var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

      if (number < 20)
      {
        words += unitsMap[number];
      }
      else
      {
        words += tensMap[number / 10];
        if (number % 10 > 0)
          words += "-" + unitsMap[number % 10];
      }
    }

    return words;
  }

  /// <param name="key"></param>
  extension(string key)
  {
    /// <summary>
    ///   Checks the similarity of key to pattern. Pattern can contain '*' as wildcard replacing any sequence of remaining
    ///   characters.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public bool IsLike(string pattern, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      var wildcardCount = pattern.Count(c => c == '*');
      if (wildcardCount == 1 && pattern.EndsWith("*"))
      {
        pattern = pattern.Substring(0, pattern.Length - 1);
        return key.StartsWith(pattern, stringComparison);
      }
      if (wildcardCount == 1 && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 1);
        return key.EndsWith(pattern, stringComparison);
      }
      if (wildcardCount == 2 && pattern.EndsWith("*") && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        return key.IndexOf(pattern, stringComparison) >= 0;
      }
      if (wildcardCount > 0)
      {
        var patternParts = pattern.Split('*').ToList();
        return MatchPatternParts(key, patternParts, stringComparison);
      }
      return key.Equals(pattern, stringComparison);
    }

    /// <summary>
    /// Checks the similarity of key to pattern parts. Pattern can contain '*' as wildcard replacing any sequence of remaining  
    /// characters.
    /// </summary>
    /// <param name="patternParts">The list of pattern parts to match.</param>
    /// <param name="stringComparison">The string comparison method to use.</param>
    /// <returns>True if the key matches the pattern parts; otherwise, false.</returns>
    private bool MatchPatternParts(List<string> patternParts, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      if (patternParts.Count == 0)
        return true;
      if (!key.StartsWith(patternParts.First(), stringComparison))
        return false;
      if (patternParts.Count == 1)
        return true;
      key = key.Substring(patternParts.First().Length);
      patternParts.RemoveAt(0);
      if (!key.EndsWith(patternParts.Last(), stringComparison))
        return false;
      if (patternParts.Count == 1)
        return true;
      key = key.Substring(key.Length - patternParts.Last().Length);
      patternParts.RemoveAt(patternParts.Count - 1);
      return key.MatchPatternParts(patternParts, stringComparison);
    }

    /// <summary>
    ///   Checks the similiarity of key to pattern. Pattern can contain '*' as wildchar replacing any sequence of remaining
    ///   characters.
    ///   Output <paramref name="wildKey" /> is set to found wildchar replacement in the key.
    ///   If there are multiple wildchars in the pattern then returned <paramref name="wildKey" /> contains multiple
    ///   replacements joined with '*' separator.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="wildKey"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public bool IsLike(string pattern, out string? wildKey, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      wildKey = null;
      var wildcardCount = pattern.Count(c => c == '*');
      if (wildcardCount == 1 && pattern.EndsWith("*"))
      {
        pattern = pattern.Substring(0, pattern.Length - 1);
        if (key.StartsWith(pattern, stringComparison))
        {
          wildKey = key.Substring(pattern.Length);
          return true;
        }
        return false;
      }
      if (wildcardCount == 1 && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 1);
        if (key.EndsWith(pattern, stringComparison))
        {
          wildKey = key.Substring(0, key.Length - pattern.Length);
          return true;
        }
        return false;
      }
      if (wildcardCount == 2 && pattern.EndsWith("*") && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        var patternPos = key.IndexOf(pattern, stringComparison);
        if (patternPos >= 0)
        {
          var wKeys = new string[2];
          wKeys[0] = key.Substring(0, patternPos);
          wKeys[1] = key.Substring(patternPos);
          wKeys[1] = wKeys[1].Substring(0, wKeys[1].Length - pattern.Length);
          wildKey = String.Join("*", wKeys);
          return true;
        }
      }
      if (wildcardCount > 0)
      {
        var patternParts = pattern.Split('*').ToList();
        if (key.MatchPatternParts(patternParts, out var wildKeyParts, stringComparison))
        {
          wildKey = String.Join("*", wildKeyParts);
          return true;
        }
        return false;
      }
      return key.Equals(pattern, stringComparison);
    }

    /// <summary>
    /// Checks the similarity of key to pattern parts. Pattern can contain '*' as wildcard replacing any sequence of remaining characters.
    /// </summary>
    /// <param name="patternParts">List of pattern parts to match against the key.</param>
    /// <param name="wildKeyParts">List of wildcard parts found in the key.</param>
    /// <param name="stringComparison">String comparison type.</param>
    /// <returns>True if the key matches the pattern parts, otherwise false.</returns>
    private bool MatchPatternParts(List<string> patternParts, out List<string> wildKeyParts, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      wildKeyParts = new List<string>();
      if (patternParts.Count == 0)
        return true;
      if (!key.StartsWith(patternParts.First(), stringComparison))
        return false;
      wildKeyParts.Add(key.Substring(patternParts.First().Length));
      if (patternParts.Count == 1)
        return true;
      key = key.Substring(patternParts.First().Length);
      patternParts.RemoveAt(0);
      if (!key.EndsWith(patternParts.Last(), stringComparison))
        return false;
      wildKeyParts.Add(key.Substring(key.Length - patternParts.Last().Length));
      if (patternParts.Count == 1)
        return true;
      key = key.Substring(key.Length - patternParts.Last().Length);
      patternParts.RemoveAt(patternParts.Count - 1);
      if (key.MatchPatternParts(patternParts, out var internWildParts, stringComparison))
      {
        if (internWildParts.Count > 0)
          wildKeyParts.InsertRange(1, internWildParts);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Checks if a key text starts or ends with a number..
    /// </summary>
    /// <param name="pattern">Pattern text containing '#' ad the beginning or end. The rest is a literal pattern</param>
    /// <param name="wildKey">Literal text detected by pattern</param>
    /// <param name="wildNum">Number parsed from the wildKey</param>
    /// <returns></returns>
    public bool IsLikeNumber(string pattern, out string? wildKey, out int wildNum)
    {
      if (pattern.EndsWith("#") && pattern.StartsWith("#"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        var patternPos = key.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase);
        if (patternPos >= 0)
        {
          wildKey = key.Remove(patternPos, pattern.Length);
          if (int.TryParse(wildKey, out wildNum))
            return true;
        }
      }
      else if (pattern.EndsWith("#"))
      {
        pattern = pattern.Substring(0, pattern.Length - 1);
        if (key.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
        {
          wildKey = key.Substring(pattern.Length);
          if (int.TryParse(wildKey, out wildNum))
            return true;
        }
      }
      if (pattern.StartsWith("#"))
      {
        pattern = pattern.Substring(1, pattern.Length - 1);
        if (key.EndsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
        {
          wildKey = key.Substring(0, pattern.Length);
          if (int.TryParse(wildKey, out wildNum))
            return true;
        }
      }
      wildKey = null;
      wildNum = 0;
      return false;
    }

    /// <summary>
    /// Check if URL is valid. Uses Regex.
    /// </summary>
    /// <returns></returns>
    public bool IsValidUrl()
    {
      var rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
      return rx.IsMatch(key);
    }

    /// <summary>
    /// Changes the first letter to uppercase. All others unchanged.
    /// </summary>
    /// <returns></returns>
    [DebuggerStepThrough]
    public string ToUpperFirstLetter()
    {
      if (string.IsNullOrEmpty(key))
        return key;
      var ss = key.ToCharArray();
      ss[0] = char.ToUpper(ss[0]);
      return new string(ss);
    }

    /// <summary>
    /// Changes the first letter to lowercase. All others unchanged.
    /// </summary>
    /// <returns></returns>
    [DebuggerStepThrough]
    public string ToLowerFirstLetter()
    {
      if (string.IsNullOrEmpty(key))
        return key;
      var ss = key.ToCharArray();
      ss[0] = char.ToLower(ss[0]);
      return new string(ss);
    }

    /// <summary>
    /// Trims parentheses enclosing the text.
    /// </summary>
    /// <param name="enclosings"></param>
    /// <returns></returns>
    public string TrimParens((char open, char close)[]? enclosings = null)
    {
      return key.TrimEnclosings('(', ')', enclosings);
    }

    /// <summary>
    /// Changes enclosing parens. Omits included enclosings.
    /// </summary>
    /// <param name="openParen"></param>
    /// <param name="closeParen"></param>
    /// <param name="enclosings"></param>
    /// <returns></returns>
    public string TrimEnclosings(char openParen, char closeParen, (char open, char close)[]? enclosings = null)
    {
      if (key.StartsWith(new String(openParen, 1)) && key.EndsWith(new String(closeParen, 1)))
      {
        if (enclosings == null)
          return key.Substring(1, key.Length - 2).Trim();

        int i;
        string? openParens = null;
        for (i = 0; i < key.Length; i++)
        {
          var ch = key[i];
          var (ch1, ch2) = enclosings.FirstOrDefault(item => item.open == ch);
          if (ch1 != '\0')
          {
            if (ch1 == openParen)
              openParens += ch1;
            i = key.FindMatch(i, ch1, enclosings);
            if (openParens == new string(openParen, 1) && i == key.Length - 1) return key.Substring(1, key.Length - 2).Trim();
          }
          else if (ch == closeParen && i == key.Length - 1)
          {
            return key.Substring(1, key.Length - 2).Trim();
          }
        }
      }
      return key.Trim();
    }

    /// <summary>
    /// Trim double quote characters enclosing the text.
    /// </summary>
    /// <returns></returns>
    [DebuggerStepThrough]
    public string TrimDblQuotes()
    {
      if (key.Length >= 2 && key.StartsWith("\"") && key.EndsWith("\""))
        return key.Substring(1, key.Length - 2);
      return key;
    }

    /// <summary>
    ///   Splits a text by <paramref name="sep" /> character.
    ///   Fragments enclosed by <paramref name="enclosings" /> are not splitted.
    /// </summary>
    /// <param name="sep"></param>
    /// <param name="enclosings"></param>
    /// <returns></returns>

    //[DebuggerStepThrough]
    public string[] SplitBy(char sep, (char open, char close)[] enclosings)
    {
      var result = new List<string>();
      var priorStart = 0;
      int i;
      for (i = 0; i < key.Length; i++)
      {
        var ch = key[i];
        var (ch1, ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0')
        {
          i = key.FindMatch(i, ch1, enclosings);
          if (i == key.Length)
            break;
        }
        else if (ch == sep)
        {
          result.Add(key.Substring(priorStart, i - priorStart));
          priorStart = i + 1;
        }
      }
      if (i > priorStart) result.Add(key.Substring(priorStart, i - priorStart));
      return result.ToArray();
    }

    /// <summary>
    ///   Searches a text from <paramref name="startNdx" /> position for a <paramref name="sep" /> character.
    ///   If not found then length of text is returned;
    /// </summary>
    /// <param name="startNdx"></param>
    /// <param name="sep"></param>
    /// <param name="enclosings"></param>
    /// <returns></returns>

    //[DebuggerStepThrough]
    public int Find(int startNdx, char sep, (char open, char close)[] enclosings)
    {
      for (var i = startNdx + 1; i < key.Length; i++)
      {
        var ch = key[i];
        if (ch == sep)
          return i;
        var (ch1, ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0') i = key.FindMatch(i, ch1, enclosings);
      }
      return key.Length;
    }

    /// <summary>
    ///   Searches a text from <paramref name="startNdx" /> position for a pair of <paramref name="openingSep" /> character.
    ///   If not found then length of text is returned;
    /// </summary>
    /// <param name="startNdx"></param>
    /// <param name="openingSep"></param>
    /// <param name="enclosings"></param>
    /// <returns></returns>

    //[DebuggerStepThrough]
    public int FindMatch(int startNdx, char openingSep, (char open, char close)[] enclosings)
    {
      var (c1, c2) = enclosings.FirstOrDefault(item => item.open == openingSep);
      var closingSep = c2;

      for (var i = startNdx + 1; i < key.Length; i++)
      {
        var ch = key[i];
        if (ch == closingSep)
          return i;
        var (ch1, ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0') i = key.FindMatch(i, ch1, enclosings);
      }
      return key.Length;
    }

    /// <summary>
    /// Gets a substring at a specified position until a specified delimiter is found.
    /// </summary>
    /// <param name="ch"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    [DebuggerStepThrough]
    public string SubstringUntil(char ch, int index)
    {
      var result = key.Substring(index);
      index = result.IndexOf(ch);
      if (index > 0)
        result = result.Substring(0, index);
      return result;
    }

    /// <summary>
    ///   Split text with delimiter omitting quotes
    /// </summary>
    [DebuggerStepThrough]
    public string[] SplitSpecial(char delimiter, (char Open, char Close)[]? bracesTuples = null)
    {
      var result = new List<string>();
      var sb = new StringBuilder();
      var inQuotes = false;
      if (bracesTuples == null)
        bracesTuples = DefaultBraces;
      for (var i = 0; i < key.Length; i++)
      {
        var ch = key[i];
        if (ch == '"')
          inQuotes = !inQuotes;
        if (inQuotes)
        {
          sb.Append(ch);
        }
        else
        {
          var foundTuple = bracesTuples.FirstOrDefault(tuple => tuple.Open == ch);
          if (foundTuple != default)
          {
            var endBrace = foundTuple.Close;
            sb.Append(ch);
            i++;
            var embedStr = key.SubstringUntil(endBrace, i, out var endPos);
            sb.Append(embedStr);
            if (endPos < key.Length)
              sb.Append(endBrace);
            i = endPos;
          }
          else
          {
            if (ch == delimiter)
            {
              result.Add(sb.ToString());
              sb.Clear();
            }
            else
            {
              sb.Append(ch);
            }
          }
        }
      }
      if (sb.Length > 0)
        result.Add(sb.ToString());
      return result.ToArray();
    }

    /// <summary>
    /// Gets a substring between start and end positions, but untils a specified delimiter is found.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    [DebuggerStepThrough]
    public string SubstringUntil(char delimiter, int startPos, out int endPos)
    {
      for (var i = startPos; i < key.Length; i++)
      {
        var ch = key[i];
        if (ch == delimiter)
        {
          endPos = i;
          return key.Substring(startPos, i - startPos);
        }
      }
      endPos = key.Length;
      return key.Substring(startPos);
    }

    /// <summary>
    /// Find a position of the end of the sentence using dot position which does not ends amy known abbreviation.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="abbreviations"></param>
    /// <returns></returns>
    public int FindEndOfSentence(int startPos, string[] abbreviations)
    {
      var ndx = key.IndexOf('.', startPos);
      while (ndx >= 0)
      {
        var foundAbbr = false;
        var ndx2 = -1;
        foreach (var abbr in abbreviations)
        {
          var dotPos = abbr.IndexOf('.');
          if (dotPos < 0)
            return ndx;
          var abbrPart = " " + abbr.Substring(0, dotPos);
          if (key.ContainsBefore(abbrPart, ndx))
          {
            if (dotPos == abbr.Length - 1)
            {
              foundAbbr = true;
              ndx2 = key.IndexOf('.', ndx + 1);
              if (ndx2 >= 0)
                break;
            }
            else
            {
              abbrPart = abbr.Substring(dotPos);
              if (key.ContainsAt(abbrPart, ndx))
              {
                foundAbbr = true;
                ndx2 = key.IndexOf('.', ndx + abbrPart.Length);
                if (ndx2 >= 0)
                  break;
              }
            }
          }
          if (foundAbbr)
            break;
          //if (ndx2 > ndx)
          //  ndx = ndx2;
        }
        if (!foundAbbr)
          return ndx;
        if (ndx2 > ndx)
          ndx = ndx2;
        else
          return ndx;
      }
      return -1;
    }

    /// <summary>
    /// Returns a string shortended by a specified character count.
    /// </summary>
    public string Shorten(int charCount)
    {
      return key.Substring(0, key.Length - charCount);
    }

    /// <summary>
    /// Makes Englush plural form of the noun.
    /// </summary>
    public string Pluralize()
    {
      if (key.EndsWith("y"))
        return key.Shorten(1) + "ies";
      if (key.EndsWith("s"))
        return key + "es";
      if (key.EndsWith("ss"))
        return key;
      return key + "s";
    }

    /// <summary>
    /// Makes Englush singular form of the noun.
    /// </summary>
    public string Singularize()
    {
      if (key.EndsWith("ies"))
        return key.Shorten(3) + "y";
      if (key.EndsWith("ses"))
        return key.Shorten(2);
      if (key.EndsWith("ss"))
        return key;
      if (key.EndsWith("s"))
        return key.Shorten(1);
      return key;
    }

    /// <summary>
    /// Decodes escapes sequences: \\, \t, \r, \n, \s, \u
    /// </summary>
    /// <returns></returns>
    public string DecodeEscapeSeq()
    {
      int index = 0;
      var sb = new StringBuilder();
      while (index < key.Length)
      {
        char c = key[index];
        if (c == '\\')
          sb.Append(DecodeEscapeSeq_(key, ref index));
        else
          sb.Append(c);
        index++;
      }
      return sb.ToString();
    }

    private string DecodeEscapeSeq_(ref int index)
    {
      char c = key[index + 1];
      switch (c)
      {
        case '\\':
          index++;
          return "\\";
        case 't':
          index++;
          return "\t";
        case 'r':
          index++;
          return "\r";
        case 'n':
          index++;
          return "\n";
        case 's':
          index++;
          return "\u00a0";
        case 'u':
        {
          index++;
          ushort num = 0;
          for (int i = 1; i <= 4; i++)
          {
            char c2 = key[index + i];
            if (c2 >= '0' && c2 <= '9')
            {
              num = (ushort)(num * 16 + (ushort)(c2 - 48));
            }
            else if (c2 >= 'A' && c2 <= 'F')
            {
              num = (ushort)(num * 16 + (ushort)(c2 - 65) + 10);
            }
            else if (c2 >= 'a' && c2 <= 'f')
            {
              num = (ushort)(num * 16 + (ushort)(c2 - 97) + 10);
            }
          }

          index += 4;
          return new string((char)num, 1);
        }
        default:
          index++;
          return new string(c, 1);
      }
    }

    /// <summary>
    /// Checks if all letters in the string are lower-case.
    /// </summary>
    /// <returns></returns>
    public bool IsLowercase()
    {
      return key.All(ch => !Char.IsLetter(ch) || Char.IsLower(ch));
    }

    /// <summary>
    /// Checks if all letters in the string are upper-case.
    /// </summary>
    /// <returns></returns>
    public bool IsUppercase()
    {
      return key.All(ch => !Char.IsLetter(ch) || Char.IsUpper(ch));
    }

    /// <summary>
    /// Checks if the first char is upper-case letter and all other letters are lower-case.
    /// </summary>
    /// <returns></returns>
    public bool IsTitlecase()
    {
      for (int i = 0; i < key.Length; i++)
      {
        var ch = key[i];
        if (i == 0)
        {
          if (!Char.IsUpper(ch))
            return false;
        }
        else
        if (!Char.IsLetter(ch) || Char.IsUpper(ch))
          return false;
      }
      return true;
    }
  }

  /// <summary>
  /// Creates a string by repeating the original string a specified number of times.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="count"></param>
  /// <returns></returns>
  public static string? Duplicate(this string? str, int count)
  {
    if (str == null)
      return null;
    var sb = new StringBuilder(str.Length * count);
    for (int i = 0; i < count; i++)
      sb.Append(str);
    return sb.ToString();
  }


  ///// <summary>
  ///// Concatenates two strings with a separator between them.
  ///// If any of both is empty of null - the other is returned.
  ///// </summary>
  //public static string? Concat2(this string? text1, string separator, string? text2)
  //{
  //  if (String.IsNullOrEmpty(text1))
  //    return text2;
  //  if (String.IsNullOrEmpty(text2))
  //    return text1;
  //  return text1 + separator + text2;
  //}
}
