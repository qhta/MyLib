using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Qhta.TextUtils;

/// <summary>
/// A bunch of string utility methods
/// </summary>
public static class StringUtils
{
  private static readonly (char open, char close)[] DefaultBraces =
  {
    ('(', ')'),
    ('[', ']'),
    ('{', '}')
  };

  /// <summary>
  /// Get a substring from the first characters of the text words.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  public static string Acronym(this string text)
  {
    return new string(text.Split([' '], StringSplitOptions.RemoveEmptyEntries).Select(word => word.First()).ToArray());
  }

  /// <summary>
  /// Change string to title case. First letter tu upper, rest to lower case.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="allWords">Determines whether all words should be treated separately.</param>
  /// <returns></returns>
  public static string TitleCase(this string str, bool allWords = false)
  {
    var chars = str.ToCharArray();
    if (allWords)
    {
      var wasLetter = false;
      for (var i = 0; i < chars.Length; i++)
      {
        var isLetter = char.IsLetter(chars[i]);
        if (isLetter)
        {
          if (!wasLetter)
            chars[i] = Char.ToUpper(chars[i]);
          else
            chars[i] = Char.ToLower(chars[i]);
          wasLetter = true;
        }
        else if (chars[i] != '\'')
          wasLetter = false;
      }
    }
    else
      for (var i = 0; i < chars.Length; i++)
      {
        if (i == 0)
          chars[i] = Char.ToUpper(chars[i]);
        else
          chars[i] = Char.ToLower(chars[i]);
      }
    return new string(chars);
  }

  /// <summary>
  /// Change first letter of each word to upper, rest to lower case.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string CamelCase(this string str)
  {
    var chars = str.ToList();
    var wasLetter = false;
    for (var i = 0; i < chars.Count; i++)
    {
      var isLetter = char.IsLetter(chars[i]);
      if (isLetter)
      {
        if (!wasLetter)
          chars[i] = Char.ToUpper(chars[i]);
        else
          chars[i] = Char.ToLower(chars[i]);
        wasLetter = true;
      }
      else
      {
        wasLetter = false;
        chars.RemoveAt(i);
        i--;
      }
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Split camel-case string to array of words.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string[] SplitCamelCase(this string str)
  {
    var ss = new List<string>();
    var chars = new List<char>();
    for (var i = 0; i < str.Length; i++)
    {
      if (Char.IsUpper(str[i]))
        if (chars.Count > 0 && (!Char.IsUpper(chars.Last()) || (i < str.Length - 1 && Char.IsLower(str[i + 1]))))
        {
          var s = new String(chars.ToArray());
          ss.Add(s);
          chars.Clear();
        }
      chars.Add(str[i]);
    }
    if (chars.Count > 0)
    {
      var s = new String(chars.ToArray());
      ss.Add(s);
      chars.Clear();
    }
    return ss.ToArray();
  }

  /// <summary>
  /// Split camel-case string to words.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string DeCamelCase(this string str)
  {
    var ss = str.SplitCamelCase();
    for (var i = 0; i < ss.Length; i++)
      if (ss[i] != ss[i].ToUpper())
      {
        if (i == 0)
          ss[i] = ss[i].TitleCase();
        else
          ss[i] = ss[i].ToLower();
      }
    return string.Join(" ", ss);
  }

  /// <summary>
  /// Changes a number to literal text using whole part and a rational fraction part.
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  public static string NumberToText(this double number)
  {
    var intPart = (Int64)number;
    var frac = number - intPart;

    var result = NumberToText(intPart);
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
  public static string NumberToText(this int number)
  {
    return NumberToText((Int64)number);
  }

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
      return "minus " + NumberToText(Math.Abs(number));

    var words = "";
    // 9223372036854775808
    if (number / 1000000000000000000 > 0)
    {
      words += NumberToText(number / 1000000000000000000) + " quintillion ";
      number %= 1000000000000000000;
    }

    if (number / 1000000000000000 > 0)
    {
      words += NumberToText(number / 1000000000000000) + " quadrillion ";
      number %= 1000000000000000;
    }

    if (number / 1000000000000 > 0)
    {
      words += NumberToText(number / 1000000000000) + " trillion ";
      number %= 1000000000000;
    }

    if (number / 1000000000 > 0)
    {
      words += NumberToText(number / 1000000000) + " billion ";
      number %= 1000000000;
    }

    if (number / 1000000 > 0)
    {
      words += NumberToText(number / 1000000) + " million ";
      number %= 1000000;
    }

    if (number / 1000 > 0)
    {
      words += NumberToText(number / 1000) + " thousand ";
      number %= 1000;
    }

    if (number / 100 > 0)
    {
      words += NumberToText(number / 100) + " hundred ";
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

  /// <summary>
  /// Precedes a string with a prefix when string is not null.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="prefix"></param>
  /// <returns></returns>
  public static string? Precede(this string str, string prefix)
  {
    return str != null ? prefix + str : null;
  }

  /// <summary>
  /// Attach a suffix to a string if string is not null.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="suffix"></param>
  /// <returns></returns>
  public static string? Attach(this string str, string suffix)
  {
    return str != null ? str + suffix : null;
  }

  /// <summary>
  /// Checks if a string contains a substring at a specified position.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="substring"></param>
  /// <param name="atIndex"></param>
  /// <returns></returns>
  public static bool ContainsAt(this string str, string substring, int atIndex)
  {
    if (atIndex + substring.Length > str.Length)
      return false;
    return str.Substring(atIndex, substring.Length).Equals(substring);
  }

  /// <summary>
  /// Checks if a string contains a substring at a specified position using a specified comparison.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="substring"></param>
  /// <param name="atIndex"></param>
  /// <param name="comparison"></param>
  /// <returns></returns>
  public static bool ContainsAt(this string str, string substring, int atIndex, StringComparison comparison)
  {
    if (atIndex + substring.Length > str.Length)
      return false;
    return str.Substring(atIndex, substring.Length).Equals(substring, comparison);
  }

  /// <summary>
  /// Checks if a string ends with a substring at a specified position
  /// </summary>
  /// <param name="str"></param>
  /// <param name="substring"></param>
  /// <param name="atIndex"></param>
  /// <returns></returns>
  public static bool ContainsBefore(this string str, string substring, int atIndex)
  {
    if (atIndex - substring.Length < 0)
      return false;
    return str.Substring(atIndex - substring.Length, substring.Length).Equals(substring);
  }

  /// <summary>
  /// Checks if a string contains a substring at a specified position using explicit comparison using a specified comparison.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="substring"></param>
  /// <param name="atIndex"></param>
  /// <param name="comparison"></param>
  /// <returns></returns>
  public static bool ContainsBefore(this string str, string substring, int atIndex, StringComparison comparison)
  {
    if (atIndex - substring.Length < 0)
      return false;
    return str.Substring(atIndex - substring.Length, substring.Length).Equals(substring, comparison);
  }

  /// <summary>
  /// Checks if a string contains only upper-case letters.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static bool IsUpper(this string str)
  {
    if (string.IsNullOrEmpty(str))
      return false;
    foreach (var ch in str)
      if (!Char.IsUpper(ch))
        return false;
    return true;
  }

  /// <summary>
  /// Checks if a string contains characters from space to ~.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static bool IsAscii(this string str)
  {
    if (string.IsNullOrEmpty(str))
      return false;
    foreach (var ch in str)
      if (ch < ' ' || ch > '~')
        return false;
    return true;
  }

  /// <summary>
  /// Checks if a string contains Unicode characters (i.e. with codes greater than 127)
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static bool IsUnicode(this string str)
  {
    if (string.IsNullOrEmpty(str))
      return false;
    foreach (var ch in str)
      if (ch > '\x7F')
        return true;
    return false;
  }

  /// <summary>
  ///   Checks the similarity of key to pattern. Pattern can contain '*' as wildcard replacing any sequence of remaining
  ///   characters.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="pattern"></param>
  /// <param name="stringComparison"></param>
  /// <returns></returns>
  public static bool IsLike(this string key, string pattern, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
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
      var patternParts = pattern.Split(['*'], StringSplitOptions.RemoveEmptyEntries).ToList();
      return MatchPatternParts(key, patternParts, pattern.StartsWith("*"), pattern.EndsWith("*"), null, stringComparison);
    }
    return key.Equals(pattern, stringComparison);
  }

  /// <summary>
  ///   Checks the similarity of key to pattern. Pattern can contain '*' as wildcard replacing any sequence of remaining
  ///   characters.
  ///   Output <paramref name="wildKey" /> is set to found wildcard replacement in the key.
  ///   If there are multiple wildcards in the pattern then returned <paramref name="wildKey" /> contains multiple
  ///   replacements joined with '*' separator.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="pattern"></param>
  /// <param name="wildKey"></param>
  /// <param name="stringComparison"></param>
  /// <returns></returns>
  public static bool IsLike(this string key, string pattern, out string? wildKey, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
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
      var patternParts = pattern.Split(['*'], StringSplitOptions.RemoveEmptyEntries).ToList();
      var wildKeyParts = new List<string>();
      if (MatchPatternParts(key, patternParts, pattern.StartsWith("*"), pattern.EndsWith("*"), wildKeyParts, stringComparison))
      {
        wildKey = String.Join("*", wildKeyParts);
        return true;
      }
      return false;
    }
    return key.Equals(pattern, stringComparison);
  }

  private static bool MatchPatternParts(string key, List<string> patternParts, bool wildStart, bool wildEnd, List<string>? wildKeyParts, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
  {
    if (patternParts.Count == 0)
      return true;
    int k = 0;
    for (int i = 0; i < patternParts.Count; i++)
    {
      var part = patternParts[i];
      int l = key.IndexOf(part, k, stringComparison);
      if (l < 0)
        return false;
      if (i == 0 && !wildStart)
      {
        if (l > 0)
          return false;
      }
      if (wildKeyParts != null && l > k)
        wildKeyParts.Add(key.Substring(k, l - k));
      k = l + part.Length;
      if (i == patternParts.Count - 1)
      {
        if (!wildEnd)
        {
          if (k < key.Length)
            return false;
        }
        else
        {
          if (wildKeyParts != null && k < key.Length)
            wildKeyParts.Add(key.Substring(k));
        }
      }
    }
    return true;
  }

  /// <summary>
  /// Checks if a key text starts or ends with a number..
  /// </summary>
  /// <param name="key">Checked text</param>
  /// <param name="pattern">Pattern text containing '#' ad the beginning or end. The rest is a literal pattern</param>
  /// <param name="wildKey">Literal text detected by pattern</param>
  /// <param name="wildNum">Number parsed from the wildKey</param>
  /// <returns></returns>
  public static bool IsLikeNumber(this string key, string pattern, out string? wildKey, out int wildNum)
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
  /// <param name="text"></param>
  /// <returns></returns>
  public static bool IsValidUrl(this string text)
  {
    if (text == null)
      return false;

    var rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
    return rx.IsMatch(text);
  }

  /// <summary>
  /// Changes the first letter to uppercase. All others unchanged.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static string ToUpperFirst(this string text)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    var ss = text.ToCharArray();
    ss[0] = char.ToUpper(ss[0]);
    return new string(ss);
  }

  /// <summary>
  /// Changes the first letter to lowercase. All others unchanged.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static string ToLowerFirst(this string text)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    var ss = text.ToCharArray();
    ss[0] = char.ToLower(ss[0]);
    return new string(ss);
  }

  /// <summary>
  /// Trims parentheses enclosing the text.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="enclosings"></param>
  /// <returns></returns>
  public static string TrimParens(this string text, (char open, char close)[]? enclosings = null)
  {
    return TrimEnclosings(text, '(', ')', enclosings);
  }

  /// <summary>
  /// Changes enclosing parens. Omits included enclosings.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="openParen"></param>
  /// <param name="closeParen"></param>
  /// <param name="enclosings"></param>
  /// <returns></returns>
  public static string TrimEnclosings(this string text, char openParen, char closeParen, (char open, char close)[]? enclosings = null)
  {
    if (text.StartsWith(new String(openParen, 1)) && text.EndsWith(new String(closeParen, 1)))
    {
      if (enclosings == null)
        return text.Substring(1, text.Length - 2).Trim();

      int i;
      string? openParens = null;
      for (i = 0; i < text.Length; i++)
      {
        var ch = text[i];
        (var ch1, var ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0')
        {
          if (ch1 == openParen)
            openParens += ch1;
          i = FindMatch(text, i, ch1, enclosings);
          if (openParens == new string(openParen, 1) && i == text.Length - 1) return text.Substring(1, text.Length - 2).Trim();
        }
        else if (ch == closeParen && i == text.Length - 1)
        {
          return text.Substring(1, text.Length - 2).Trim();
        }
      }
    }
    return text.Trim();
  }

  /// <summary>
  /// Trim double quote characters enclosing the text.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static string TrimDblQuotes(this string text)
  {
    if (text.Length >= 2 && text.StartsWith("\"") && text.EndsWith("\""))
      return text.Substring(1, text.Length - 2);
    return text;
  }

  /// <summary>
  ///   Splits a text by <paramref name="sep" /> character.
  ///   Fragments enclosed by <paramref name="enclosings" /> are not splitted.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="sep"></param>
  /// <param name="enclosings"></param>
  /// <returns></returns>
  //[DebuggerStepThrough]
  public static string[] SplitBy(this string text, char sep, (char open, char close)[] enclosings)
  {
    var result = new List<string>();
    var priorStart = 0;
    int i;
    for (i = 0; i < text.Length; i++)
    {
      var ch = text[i];
      (var ch1, var ch2) = enclosings.FirstOrDefault(item => item.open == ch);
      if (ch1 != '\0')
      {
        i = FindMatch(text, i, ch1, enclosings);
        if (i == text.Length)
          break;
      }
      else if (ch == sep)
      {
        result.Add(text.Substring(priorStart, i - priorStart));
        priorStart = i + 1;
      }
    }
    if (i > priorStart) result.Add(text.Substring(priorStart, i - priorStart));
    return result.ToArray();
  }

  /// <summary>
  ///   Searches a text from <paramref name="startNdx" /> position for a <paramref name="sep" /> character.
  ///   If not found then length of text is returned;
  /// </summary>
  /// <param name="text"></param>
  /// <param name="startNdx"></param>
  /// <param name="sep"></param>
  /// <param name="enclosings"></param>
  /// <returns></returns>
  //[DebuggerStepThrough]
  public static int Find(this string text, int startNdx, char sep, (char open, char close)[] enclosings)
  {
    for (var i = startNdx + 1; i < text.Length; i++)
    {
      var ch = text[i];
      if (ch == sep)
        return i;
      (var ch1, var ch2) = enclosings.FirstOrDefault(item => item.open == ch);
      if (ch1 != '\0') i = FindMatch(text, i, ch1, enclosings);
    }
    return text.Length;
  }

  /// <summary>
  ///   Searches a text from <paramref name="startNdx" /> position for a pair of <paramref name="openingSep" /> character.
  ///   If not found then length of text is returned;
  /// </summary>
  /// <param name="text"></param>
  /// <param name="startNdx"></param>
  /// <param name="openingSep"></param>
  /// <param name="enclosings"></param>
  /// <returns></returns>
  //[DebuggerStepThrough]
  public static int FindMatch(this string text, int startNdx, char openingSep, (char open, char close)[] enclosings)
  {
    (var c1, var c2) = enclosings.FirstOrDefault(item => item.open == openingSep);
    var closingSep = c2;

    for (var i = startNdx + 1; i < text.Length; i++)
    {
      var ch = text[i];
      if (ch == closingSep)
        return i;
      (var ch1, var ch2) = enclosings.FirstOrDefault(item => item.open == ch);
      if (ch1 != '\0') i = FindMatch(text, i, ch1, enclosings);
    }
    return text.Length;
  }

  /// <summary>
  /// Gets a substring at a specified position until a specified delimiter is found.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="ch"></param>
  /// <param name="index"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static string SubstringUntil(this string text, char ch, int index)
  {
    var result = text.Substring(index);
    index = result.IndexOf(ch);
    if (index > 0)
      result = result.Substring(0, index);
    return result;
  }

  /// <summary>
  ///   Split text with delimiter omitting quotes
  /// </summary>
  [DebuggerStepThrough]
  public static string[] SplitSpecial(this string text, char delimiter, (char Open, char Close)[] bracesTuples = null!)
  {
    var result = new List<string>();
    var sb = new StringBuilder();
    var inQuotes = false;
    if (bracesTuples == null)
      bracesTuples = DefaultBraces;
    for (var i = 0; i < text.Length; i++)
    {
      var ch = text[i];
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
          var endbrace = foundTuple.Close;
          sb.Append(ch);
          i++;
          var embedStr = text.SubstringUntil(endbrace, i, out var endPos);
          sb.Append(embedStr);
          if (endPos < text.Length)
            sb.Append(endbrace);
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
  /// <param name="text"></param>
  /// <param name="delimiter"></param>
  /// <param name="startPos"></param>
  /// <param name="endPos"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static string SubstringUntil(this string text, char delimiter, int startPos, out int endPos)
  {
    for (var i = startPos; i < text.Length; i++)
    {
      var ch = text[i];
      if (ch == delimiter)
      {
        endPos = i;
        return text.Substring(startPos, i - startPos);
      }
    }
    endPos = text.Length;
    return text.Substring(startPos);
  }

  /// <summary>
  /// Find a position of the end of the sentence using dot position which does not ends amy known abbreviation.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="startPos"></param>
  /// <param name="abbreviations"></param>
  /// <returns></returns>
  public static int FindEndOfSentence(this string text, int startPos, string[] abbreviations)
  {
    var ndx = text.IndexOf('.', startPos);
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
        if (text.ContainsBefore(abbrPart, ndx))
        {
          if (dotPos == abbr.Length - 1)
          {
            foundAbbr = true;
            ndx2 = text.IndexOf('.', ndx + 1);
            if (ndx2 >= 0)
              break;
          }
          else
          {
            abbrPart = abbr.Substring(dotPos);
            if (text.ContainsAt(abbrPart, ndx))
            {
              foundAbbr = true;
              ndx2 = text.IndexOf('.', ndx + abbrPart.Length);
              if (ndx2 >= 0)
                break;
            }
          }
        }
        if (foundAbbr)
          break;
        if (ndx2 > ndx)
          ndx = ndx2;
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
  /// Replaces the beginning of the string when it starts with a specified text.
  /// </summary>
  public static string ReplaceStart(this string text, string startString, string replaceString)
  {
    if (text.StartsWith(startString))
      return replaceString + text.Substring(startString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the beginning of the string when it starts with a specified text. Uses a specific string comparison.
  /// </summary>
  public static string ReplaceStart(this string text, string startString, string replaceString, StringComparison comparisonType)
  {
    if (text.StartsWith(startString, comparisonType))
      return replaceString + text.Substring(startString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the end of the string when it ends with a specified text.
  /// </summary>
  public static string ReplaceEnd(this string text, string endString, string replaceString)
  {
    if (text.EndsWith(endString))
      return text.Substring(0, text.Length - endString.Length) + replaceString;
    return text;
  }

  /// <summary>
  /// Replaces the end of the string when it ends with a specified text. Uses a specified string comparison.
  /// </summary>
  public static string ReplaceEnd(this string text, string endString, string replaceString, StringComparison comparisonType)
  {
    if (text.EndsWith(endString, comparisonType))
      return text.Substring(0, text.Length - endString.Length) + replaceString;
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string searching in a specified text.
  /// </summary>
  public static string ReplaceFirst(this string text, string searchString, string replaceString)
  {
    var k = text.IndexOf(searchString);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string searching in a specified text starting from the specified index.
  /// </summary>
  public static string ReplaceFirst(this string text, string searchString, string replaceString, int index)
  {
    var k = text.IndexOf(searchString, index);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string searching in a specified text starting from the specified index for the specified characters count.
  /// </summary>
  public static string ReplaceFirst(this string text, string searchString, string replaceString, int index, int count)
  {
    var k = text.IndexOf(searchString, index, count);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string in a specified text. Uses a specific string comparison.
  /// </summary>
  public static string ReplaceFirst(this string text, string searchString, string replaceString, StringComparison comparisonType)
  {
    var k = text.IndexOf(searchString, comparisonType);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string searching in a specified text starting from the specified index. Uses a specific string comparison.
  /// </summary>
  public static string ReplaceFirst(this string text, string searchString, string replaceString, int index, StringComparison comparisonType)
  {
    var k = text.IndexOf(searchString, index, comparisonType);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string searching in a specified text starting from the specified index for the specified characters count.
  /// Uses a specific string comparison.
  /// </summary>
  public static string ReplaceFirst(this string text, string searchString, string replaceString, int index, int count, StringComparison comparisonType)
  {
    var k = text.IndexOf(searchString, index, count, comparisonType);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the last occurence of the string searching in a specified text.
  /// </summary>
  public static string ReplaceLast(this string text, string searchString, string replaceString)
  {
    var k = text.LastIndexOf(searchString);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the last occurence of the string searching in a specified text starting from the specified index
  /// and continuing towards the begining of the text.
  /// </summary>
  public static string ReplaceLast(this string text, string searchString, string replaceString, int index)
  {
    var k = text.LastIndexOf(searchString, index);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the last occurence of the string searching in a specified text starting from the specified index
  /// and continuing towards the begining of the text for the specified characters count.
  /// </summary>
  public static string ReplaceLast(this string text, string searchString, string replaceString, int index, int count)
  {
    var k = text.LastIndexOf(searchString, index, count);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the last occurence of the string in a specified text. Uses a specific string comparison.
  /// </summary>
  public static string ReplaceLast(this string text, string searchString, string replaceString, StringComparison comparisonType)
  {
    var k = text.LastIndexOf(searchString, comparisonType);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string searching in a specified text starting from the specified index
  /// and continuing towards the begining of the text. Uses a specific string comparison.
  /// </summary>
  public static string ReplaceLast(this string text, string searchString, string replaceString, int index, StringComparison comparisonType)
  {
    var k = text.LastIndexOf(searchString, index, comparisonType);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Replaces the first occurence of the string searching in a specified text starting from the specified index
  /// and continuing towards the begining of the text for the specified characters count.
  /// Uses a specific string comparison.
  /// </summary>
  public static string ReplaceLast(this string text, string searchString, string replaceString, int index, int count, StringComparison comparisonType)
  {
    var k = text.LastIndexOf(searchString, index, count, comparisonType);
    if (k >= 0)
      return text.Substring(0, k) + replaceString + text.Substring(k + searchString.Length);
    return text;
  }

  /// <summary>
  /// Concatenates two strings with a separator between them.
  /// If any of both is empty of null - the other is returned.
  /// </summary>
  public static string? Concat2(this string? text1, string separator, string? text2)
  {
    if (String.IsNullOrEmpty(text1))
      return text2;
    if (String.IsNullOrEmpty(text2))
      return text1;
    return text1 + separator + text2;
  }

  /// <summary>
  /// Returns a string shortended by a specified character count.
  /// </summary>
  public static string Shorten(this string text, int charCount)
  {
    return text.Substring(0, text.Length - charCount);
  }

  /// <summary>
  /// Makes Englush plural form of the noun.
  /// </summary>
  public static string Pluralize(this string text)
  {
    if (text.EndsWith("y"))
      return text.Shorten(1) + "ies";
    if (text.EndsWith("s"))
      return text + "es";
    if (text.EndsWith("ss"))
      return text;
    return text + "s";
  }

  /// <summary>
  /// Makes Englush singular form of the noun.
  /// </summary>
  public static string Singularize(this string text)
  {
    if (text.EndsWith("ies"))
      return text.Shorten(3) + "y";
    if (text.EndsWith("ses"))
      return text.Shorten(2);
    if (text.EndsWith("ss"))
      return text;
    if (text.EndsWith("s"))
      return text.Shorten(1);
    return text;
  }

  /// <summary>
  /// Decodes escapes sequences: \\, \t, \r, \n, \s, \u
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string DecodeEscapeSeq(this string str)
  {
    int index = 0;
    var sb = new StringBuilder();
    while (index < str.Length)
    {
      char c = str[index];
      if (c == '\\')
        sb.Append(DecodeEscapeSeq_(str, ref index));
      else
        sb.Append(c);
      index++;
    }
    return sb.ToString();
  }


  private static string DecodeEscapeSeq_(this string str, ref int index)
  {
    char c = str[index + 1];
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
            char c2 = str[index + i];
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
}
