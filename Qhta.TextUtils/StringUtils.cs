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
  /// Extension methods for string. All methods return null if the input string is null.
  /// </summary>
  /// <param name="inputString">String to process.</param>
  extension(string inputString)
  {
    /// <summary>
    /// Get a substring from the first characters of the string's words.
    /// </summary>
    /// <returns>The acronym of the string.</returns>
    public string Acronym()
    {
      return new string(inputString.Split([' '], StringSplitOptions.RemoveEmptyEntries).Select(word => word.First())
        .ToArray());
    }

    /// <summary>
    /// Change string to title case. First letter tu upper, rest to lower case.
    /// </summary>
    /// <param name="allWords">Determines whether all words should be treated separately.</param>
    /// <returns>The string in title case.</returns>
    public string TitleCase(bool allWords = false)
    {
      var chars = inputString.ToCharArray();
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
    /// <returns>String with each word in camel case.</returns>
    public string CamelCase()
    {
      var chars = inputString.ToList();
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
    /// <returns>Array of words from the camel-case string.</returns>
    public string[] SplitCamelCase()
    {
      if (inputString == null)
        return Array.Empty<string>();

      var ss = new List<string>();
      var chars = new List<char>();
      for (var i = 0; i < inputString.Length; i++)
      {
        if (Char.IsUpper(inputString[i]))
          if (chars.Count > 0 && (!Char.IsUpper(chars.Last()) ||
                                  (i < inputString.Length - 1 && Char.IsLower(inputString[i + 1]))))
          {
            var s = new String(chars.ToArray());
            ss.Add(s);
            chars.Clear();
          }
        chars.Add(inputString[i]);
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
    /// Split camel-case string to words and return them as a string with words separated by spaces. Words in all upper-case are not changed, but other words are changed to title case.
    /// </summary>
    /// <returns>String with words separated by spaces.</returns>
    public string DeCamelCase()
    {
      var ss = inputString.SplitCamelCase();
      for (var i = 0; i < ss.Length; i++)
        if (ss[i] != ss[i].ToUpper())
        {
          if (i == 0)
            ss[i] = ss[i].TitleCase() ?? "";
          else
            ss[i] = ss[i].ToLower();
        }
      return string.Join(" ", ss);
    }

    /// <summary>
    /// Precedes a string with a prefix when string is not null.
    /// </summary>
    /// <param name="prefix">Prefix to add to the string.</param>
    /// <returns>String with the prefix added, or empty string if input is null.</returns>
    public string Precede(string prefix)
    {
      return inputString != null ? prefix + inputString : String.Empty;
    }

    /// <summary>
    /// Attach a suffix to a string if string is not null.
    /// </summary>
    /// <param name="suffix">Suffix to add to the string.</param>
    /// <returns>String with the suffix added, or empty string if input is null.</returns>
    public string Attach(string suffix)
    {
      return inputString != null ? inputString + suffix : String.Empty;
    }

    /// <summary>
    /// Checks if a string contains a substring at a specified position.
    /// </summary>
    /// <param name="substring">Substring to check for.</param>
    /// <param name="atIndex">Index at which to check for the substring.</param>
    /// <returns>True if the substring is found at the specified index, false otherwise.</returns>
    public bool ContainsAt(string substring, int atIndex)
    {
      if (inputString == null)
        return false;

      if (atIndex + substring.Length > inputString.Length)
        return false;

      return inputString.Substring(atIndex, substring.Length).Equals(substring);
    }

    /// <summary>
    /// Checks if a string contains a substring at a specified position using a specified comparison.
    /// </summary>
    /// <param name="substring">Substring to check for.</param>
    /// <param name="atIndex">Index at which to check for the substring.</param>
    /// <param name="comparison">String comparison type to use.</param>
    /// <returns>True if the substring is found at the specified index using the specified comparison, false otherwise.</returns>
    public bool ContainsAt(string substring, int atIndex, StringComparison comparison)
    {
      if (inputString == null)
        return false;

      if (atIndex + substring.Length > inputString.Length)
        return false;

      return inputString.Substring(atIndex, substring.Length).Equals(substring, comparison);
    }

    /// <summary>
    /// Checks if a string ends with a substring at a specified position
    /// </summary>
    /// <param name="substring">Substring to check for.</param>
    /// <param name="atIndex">Index at which to check for the substring.</param>
    /// <returns>True if the substring is found before the specified index, false otherwise.</returns>
    public bool ContainsBefore(string substring, int atIndex)
    {
      if (inputString == null)
        return false;

      return inputString.Substring(atIndex - substring.Length, substring.Length).Equals(substring);
    }

    /// <summary>
    /// Checks if a string contains a substring at a specified position using explicit comparison using a specified comparison.
    /// </summary>
    /// <param name="substring">Substring to check for.</param>
    /// <param name="atIndex">Index at which to check for the substring.</param>
    /// <param name="comparison">String comparison type to use.</param>
    /// <returns>True if the substring is found before the specified index using the specified comparison, false otherwise.</returns>
    public bool ContainsBefore(string substring, int atIndex, StringComparison comparison)
    {
      if (inputString == null)
        return false;

      if (atIndex - substring.Length < 0)
        return false;

      return inputString.Substring(atIndex - substring.Length, substring.Length).Equals(substring, comparison);
    }

    /// <summary>
    /// Checks if a string contains only upper-case letters.
    /// </summary>
    /// <returns>True if the string contains only upper-case letters, false otherwise.</returns>
    [DebuggerStepThrough]
    public bool IsUpper()
    {
      if (inputString == null)
        return false;
      if (string.IsNullOrEmpty(inputString))
        return false;

      foreach (var ch in inputString)
        if (!Char.IsUpper(ch))
          return false;

      return true;
    }

    /// <summary>
    /// Checks if a string contains characters from space to ~.
    /// </summary>
    /// <returns>True if the string contains only ASCII characters, false otherwise.</returns>
    [DebuggerStepThrough]
    public bool IsAscii()
    {
      if (inputString == null)
        return false;
      if (string.IsNullOrEmpty(inputString))
        return false;

      foreach (var ch in inputString)
        if (ch < ' ' || ch > '~')
          return false;

      return true;
    }

    /// <summary>
    /// Checks if a string contains Unicode characters (i.e. with codes greater than 127)
    /// </summary>
    /// <returns>True if the string contains Unicode characters, false otherwise.</returns>
    [DebuggerStepThrough]
    public bool IsUnicode()
    {
      if (inputString == null)
        return false;
      if (string.IsNullOrEmpty(inputString))
        return false;

      foreach (var ch in inputString)
        if (ch > '\x7F')
          return true;

      return false;
    }

    /// <summary>
    ///   Checks the similarity of key to pattern. Pattern can contain '*' as wildcard replacing any sequence of remaining
    ///   characters.
    /// </summary>
    /// <param name="pattern">Pattern to match against.</param>
    /// <param name="stringComparison">String comparison type to use.</param>
    /// <returns>True if the key matches the pattern, false otherwise.</returns>
    public bool IsLike(string pattern, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      if (inputString == null)
        return false;

      var wildcardCount = pattern.Count(c => c == '*');
      if (wildcardCount == 1 && pattern.EndsWith("*"))
      {
        pattern = pattern.Substring(0, pattern.Length - 1);
        return inputString.StartsWith(pattern, stringComparison);
      }
      if (wildcardCount == 1 && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 1);
        return inputString.EndsWith(pattern, stringComparison);
      }
      if (wildcardCount == 2 && pattern.EndsWith("*") && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        return inputString.IndexOf(pattern, stringComparison) >= 0;
      }
      if (wildcardCount > 0)
      {
        var patternParts = pattern.Split(['*'], StringSplitOptions.RemoveEmptyEntries).ToList();
        return inputString.MatchPatternParts_(patternParts, pattern.StartsWith("*"), pattern.EndsWith("*"), null,
          stringComparison);
      }
      return inputString.Equals(pattern, stringComparison);
    }

    /// <summary>
    ///   Checks the similarity of key to pattern. Pattern can contain '*' as wildcard replacing any sequence of remaining
    ///   characters.
    ///   Output <paramref name="wildKey" /> is set to found wildcard replacement in the key.
    ///   If there are multiple wildcards in the pattern then returned <paramref name="wildKey" /> contains multiple
    ///   replacements joined with '*' separator.
    /// </summary>
    /// <param name="pattern">Pattern to match against.</param>
    /// <param name="wildKey">Output parameter that will contain the found wildcard replacement(s) in the key.</param>
    /// <param name="stringComparison">String comparison type to use.</param>
    /// <returns>True if the key matches the pattern, false otherwise.</returns>
    public bool IsLike
    (string pattern, out string? wildKey,
      StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      wildKey = null;
      if (inputString == null)
        return false;

      var wildcardCount = pattern.Count(c => c == '*');
      if (wildcardCount == 1 && pattern.EndsWith("*"))
      {
        pattern = pattern.Substring(0, pattern.Length - 1);
        if (inputString.StartsWith(pattern, stringComparison))
        {
          wildKey = inputString.Substring(pattern.Length);
          return true;
        }
        return false;
      }
      if (wildcardCount == 1 && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 1);
        if (inputString.EndsWith(pattern, stringComparison))
        {
          wildKey = inputString.Substring(0, inputString.Length - pattern.Length);
          return true;
        }
        return false;
      }
      if (wildcardCount == 2 && pattern.EndsWith("*") && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        var patternPos = inputString.IndexOf(pattern, stringComparison);
        if (patternPos >= 0)
        {
          if (patternPos == 0 && inputString.Length == pattern.Length)
          {
            wildKey = null;
            return true;
          }
          if (patternPos == 0 && inputString.Length > pattern.Length)
          {
            wildKey = inputString.Substring(pattern.Length);
            return true;
          }
          if (patternPos + pattern.Length == inputString.Length)
          {
            wildKey = inputString.Substring(0, patternPos);
            return true;
          }
          var wKeys = new string[2];
          wKeys[0] = inputString.Substring(0, patternPos);
          wKeys[1] = inputString.Substring(patternPos + pattern.Length);
          wildKey = String.Join("*", wKeys);
          return true;
        }
      }
      if (wildcardCount > 0)
      {
        var patternParts = pattern.Split(['*'], StringSplitOptions.RemoveEmptyEntries).ToList();
        var wildKeyParts = new List<string>();
        if (inputString.MatchPatternParts_(patternParts, pattern.StartsWith("*"), pattern.EndsWith("*"), wildKeyParts,
              stringComparison))
        {
          wildKey = String.Join("*", wildKeyParts);
          return true;
        }
        return false;
      }
      return inputString.Equals(pattern, stringComparison);
    }

    /// <summary>
    /// Helper method to match pattern parts with wildcards. Pattern parts are separated by wildcards. Wildcards can be at the beginning and end of the pattern. If there are wildcards then found wildcard replacements are added to the <paramref name="wildKeyParts" /> list.
    /// </summary>
    /// <param name="patternParts">List of pattern parts separated by wildcards.</param>
    /// <param name="wildStart">Indicates if the pattern starts with a wildcard.</param>
    /// <param name="wildEnd">Indicates if the pattern ends with a wildcard.</param>
    /// <param name="wildKeyParts">List to store found wildcard replacements. </param>
    /// <param name="stringComparison">String comparison type to use.</param>
    /// <returns>True if the pattern parts match the input string, false otherwise.</returns>
    private bool MatchPatternParts_
    (List<string> patternParts, bool wildStart, bool wildEnd, List<string>? wildKeyParts,
      StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      if (inputString == null)
        return false;

      if (patternParts.Count == 0)
        return true;

      int k = 0;
      for (int i = 0; i < patternParts.Count; i++)
      {
        var part = patternParts[i];
        int l = inputString.IndexOf(part, k, stringComparison);
        if (l < 0)
          return false;

        if (i == 0 && !wildStart)
        {
          if (l > 0)
            return false;
        }
        if (wildKeyParts != null && l > k)
          wildKeyParts.Add(inputString.Substring(k, l - k));
        k = l + part.Length;
        if (i == patternParts.Count - 1)
        {
          if (!wildEnd)
          {
            if (k < inputString.Length)
              return false;
          }
          else
          {
            if (wildKeyParts != null && k < inputString.Length)
              wildKeyParts.Add(inputString.Substring(k));
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Checks if a key text starts or ends with a number..
    /// </summary>
    /// <param name="pattern">Pattern text containing '#' ad the beginning or end. The rest is a literal pattern</param>
    /// <param name="wildKey">Literal text detected by pattern</param>
    /// <param name="wildNum">Number parsed from the wildKey</param>
    /// <returns>True if the key matches the pattern, false otherwise.  </returns>
    public bool IsLikeNumber(string pattern, out string? wildKey, out int wildNum)
    {
      wildKey = null;
      wildNum = 0;
      if (inputString == null) return false;

      if (pattern.EndsWith("#") && pattern.StartsWith("#"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        var patternPos = inputString.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase);
        if (patternPos >= 0)
        {
          wildKey = inputString.Remove(patternPos, pattern.Length);
          if (int.TryParse(wildKey, out wildNum))
            return true;
        }
      }
      else if (pattern.EndsWith("#"))
      {
        pattern = pattern.Substring(0, pattern.Length - 1);
        if (inputString.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
        {
          wildKey = inputString.Substring(pattern.Length);
          if (int.TryParse(wildKey, out wildNum))
            return true;
        }
      }
      if (pattern.StartsWith("#"))
      {
        pattern = pattern.Substring(1, pattern.Length - 1);
        if (inputString.EndsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
        {
          wildKey = inputString.Substring(0, pattern.Length);
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
    /// <returns>True if the URL is valid, false otherwise.</returns>
    public bool IsValidUrl()
    {
      if (inputString == null)
        return false;

      var rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
      return rx.IsMatch(inputString);
    }

    /// <summary>
    /// Changes the first letter to uppercase. All others unchanged.
    /// </summary>
    /// <returns>String with the first letter in uppercase.</returns>
    [DebuggerStepThrough]
    public string ToUpperFirst()
    {
      if (string.IsNullOrEmpty(inputString))
        return string.Empty;

      var ss = inputString.ToCharArray();
      ss[0] = char.ToUpper(ss[0]);
      return new string(ss);
    }

    /// <summary>
    /// Changes the first letter to lowercase. All others unchanged.
    /// </summary>
    /// <returns>String with the first letter in lowercase.</returns>
    [DebuggerStepThrough]
    public string ToLowerFirst()
    {
      if (string.IsNullOrEmpty(inputString))
        return string.Empty;

      var ss = inputString.ToCharArray();
      ss[0] = char.ToLower(ss[0]);
      return new string(ss);
    }

    /// <summary>
    /// Trims parentheses enclosing the text.
    /// </summary>
    /// <param name="enclosings">Array of character pairs representing enclosings.</param>
    /// <returns>String with parentheses trimmed.</returns>
    public string TrimParens((char open, char close)[]? enclosings = null)
    {
      return inputString.TrimEnclosings('(', ')', enclosings);
    }

    /// <summary>
    /// Changes enclosing parens. Omits included enclosings.
    /// </summary>
    /// <param name="openParen">The opening parenthesis character.</param>
    /// <param name="closeParen">The closing parenthesis character.</param>
    /// <param name="enclosings">Array of character pairs representing enclosings.</param>
    /// <returns>String with specified enclosings trimmed.</returns>
    public string TrimEnclosings(char openParen, char closeParen, (char open, char close)[]? enclosings = null)
    {
      if (inputString.StartsWith(new String(openParen, 1)) && inputString.EndsWith(new String(closeParen, 1)))
      {
        if (enclosings == null)
          return inputString.Substring(1, inputString.Length - 2).Trim();

        int i;
        string? openParens = null;
        for (i = 0; i < inputString.Length; i++)
        {
          var ch = inputString[i];
          (var ch1, var ch2) = enclosings.FirstOrDefault(item => item.open == ch);
          if (ch1 != '\0')
          {
            if (ch1 == openParen)
              openParens += ch1;
            i = inputString.FindMatch(i, ch1, enclosings);
            if (openParens == new string(openParen, 1) && i == inputString.Length - 1)
              return inputString.Substring(1, inputString.Length - 2).Trim();
          }
          else if (ch == closeParen && i == inputString.Length - 1)
          {
            return inputString.Substring(1, inputString.Length - 2).Trim();
          }
        }
      }
      return inputString.Trim();
    }

    /// <summary>
    /// Trim double quote characters enclosing the text.
    /// </summary>
    /// <returns>String with double quotes trimmed.</returns>
    [DebuggerStepThrough]
    public string TrimDblQuotes()
    {
      if (inputString.Length >= 2 && inputString.StartsWith("\"") && inputString.EndsWith("\""))
        return inputString.Substring(1, inputString.Length - 2);

      return inputString;
    }

    /// <summary>
    ///   Splits a text by <paramref name="sep" /> character.
    ///   Fragments enclosed by <paramref name="enclosings" /> are not split.
    /// </summary>
    /// <param name="sep">The separator character.</param>
    /// <param name="enclosings">Array of character pairs representing enclosings.</param>
    /// <returns>Array of strings split by the separator, excluding enclosed fragments.</returns>

    //[DebuggerStepThrough]
    public string[] SplitBy(char sep, (char open, char close)[] enclosings)
    {
      var result = new List<string>();
      var priorStart = 0;
      int i;
      for (i = 0; i < inputString.Length; i++)
      {
        var ch = inputString[i];
        var (ch1, ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0')
        {
          i = inputString.FindMatch(i, ch1, enclosings);
          if (i == inputString.Length)
            break;
        }
        else if (ch == sep)
        {
          result.Add(inputString.Substring(priorStart, i - priorStart));
          priorStart = i + 1;
        }
      }
      if (i > priorStart) result.Add(inputString.Substring(priorStart, i - priorStart));
      return result.ToArray();
    }

    /// <summary>
    ///   Searches a text from <paramref name="startNdx" /> position for a <paramref name="sep" /> character.
    ///   If not found then length of text is returned;
    /// </summary>
    /// <param name="startNdx">The starting index for the search.</param>
    /// <param name="sep">The separator character to search for.</param>
    /// <param name="enclosings">Array of character pairs representing enclosings.</param>
    /// <returns>The index of the separator character or the length of the text if not found.</returns>

    //[DebuggerStepThrough]
    public int Find(int startNdx, char sep, (char open, char close)[] enclosings)
    {
      for (var i = startNdx + 1; i < inputString.Length; i++)
      {
        var ch = inputString[i];
        if (ch == sep)
          return i;

        var (ch1, ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0') i = inputString.FindMatch(i, ch1, enclosings);
      }
      return inputString.Length;
    }

    /// <summary>
    ///   Searches a text from <paramref name="startNdx" /> position for a pair of <paramref name="openingSep" /> character.
    ///   If not found then length of text is returned;
    /// </summary>
    /// <param name="startNdx">The starting index for the search.</param>
    /// <param name="openingSep">The opening separator character.</param>
    /// <param name="enclosings">Array of character pairs representing enclosings.</param>
    /// <returns>The index of the matching closing separator or the length of the text if not found.</returns>
    public int FindMatch(int startNdx, char openingSep, (char open, char close)[] enclosings)
    {
      var (c1, c2) = enclosings.FirstOrDefault(item => item.open == openingSep);
      var closingSep = c2;

      for (var i = startNdx + 1; i < inputString.Length; i++)
      {
        var ch = inputString[i];
        if (ch == closingSep)
          return i;

        var (ch1, ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0') i = inputString.FindMatch(i, ch1, enclosings);
      }
      return inputString.Length;
    }

    /// <summary>
    /// Gets a substring at a specified position until a specified delimiter is found.
    /// </summary>
    /// <param name="ch">The delimiter character.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <returns>The substring from the starting index until the delimiter is found.</returns>
    [DebuggerStepThrough]
    public string SubstringUntil(char ch, int index)
    {
      var result = inputString.Substring(index);
      index = result.IndexOf(ch);
      if (index > 0)
        result = result.Substring(0, index);
      return result;
    }

    /// <summary>
    ///   Split text with delimiter omitting quotes
    /// </summary>
    /// <param name="delimiter">The delimiter character.</param>
    /// <param name="bracesTuples">Array of character pairs representing enclosings.</param>
    /// <returns>Array of strings split by the delimiter, excluding enclosed fragments.</returns>
    [DebuggerStepThrough]
    public string[] SplitSpecial(char delimiter, (char Open, char Close)[]? bracesTuples = null)
    {
      var result = new List<string>();
      var sb = new StringBuilder();
      var inQuotes = false;
      if (bracesTuples == null)
        bracesTuples = DefaultBraces;
      for (var i = 0; i < inputString.Length; i++)
      {
        var ch = inputString[i];
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
            var embedStr = inputString.SubstringUntil(endbrace, i, out var endPos);
            sb.Append(embedStr);
            if (endPos < inputString.Length)
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
    /// <param name="delimiter">The delimiter character.</param>
    /// <param name="startPos">The starting position for the search.</param>
    /// <param name="endPos">The ending position of the found substring.</param>
    /// <returns>The substring from the starting position until the delimiter is found.</returns>
    [DebuggerStepThrough]
    public string SubstringUntil(char delimiter, int startPos, out int endPos)
    {
      for (var i = startPos; i < inputString.Length; i++)
      {
        var ch = inputString[i];
        if (ch == delimiter)
        {
          endPos = i;
          return inputString.Substring(startPos, i - startPos);
        }
      }
      endPos = inputString.Length;
      return inputString.Substring(startPos);
    }

    /// <summary>
    /// Find a position of the end of the sentence using dot position which does not ends any known abbreviation.
    /// </summary>
    /// <param name="startPos">The starting position for the search.</param>
    /// <param name="abbreviations">Array of known abbreviations.</param>
    /// <returns>The position of the end of the sentence or -1 if not found.</returns>
    public int FindEndOfSentence(int startPos, string[] abbreviations)
    {
      var ndx = inputString.IndexOf('.', startPos);
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
          if (inputString.ContainsBefore(abbrPart, ndx))
          {
            if (dotPos == abbr.Length - 1)
            {
              foundAbbr = true;
              ndx2 = inputString.IndexOf('.', ndx + 1);
              if (ndx2 >= 0)
                break;
            }
            else
            {
              abbrPart = abbr.Substring(dotPos);
              if (inputString.ContainsAt(abbrPart, ndx))
              {
                foundAbbr = true;
                ndx2 = inputString.IndexOf('.', ndx + abbrPart.Length);
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
    /// Replaces the beginning of the string when it starts with a specified text.
    /// </summary>
    /// <param name="startString">The string to be replaced at the start.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <returns>The modified string if the start matches; otherwise, the original string.</returns>
    public string ReplaceStart(string startString, string replaceString)
    {
      if (inputString.StartsWith(startString))
        return replaceString + inputString.Substring(startString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the beginning of the string when it starts with a specified text. Uses a specific string comparison.
    /// </summary>
    /// <param name="startString">The string to be replaced at the start.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the start matches; otherwise, the original string.</returns>
    public string ReplaceStart(string startString, string replaceString, StringComparison comparisonType)
    {
      if (inputString.StartsWith(startString, comparisonType))
        return replaceString + inputString.Substring(startString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the end of the string when it ends with a specified text.
    /// </summary>
    /// <param name="endString">The string to be replaced at the end.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <returns>The modified string if the end matches; otherwise, the original string.</returns>  
    public string ReplaceEnd(string endString, string replaceString)
    {
      if (inputString.EndsWith(endString))
        return inputString.Substring(0, inputString.Length - endString.Length) + replaceString;

      return inputString;
    }

    /// <summary>
    /// Replaces the end of the string when it ends with a specified text. Uses a specified string comparison.
    /// </summary>
    /// <param name="endString">The string to be replaced at the end.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the end matches; otherwise, the original string.</returns>
    public string ReplaceEnd(string endString, string replaceString, StringComparison comparisonType)
    {
      if (inputString.EndsWith(endString, comparisonType))
        return inputString.Substring(0, inputString.Length - endString.Length) + replaceString;

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string searching in a specified text.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceFirst(string searchString, string replaceString)
    {

      var k = inputString.IndexOf(searchString);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string searching in a specified text starting from the specified index.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>   
    public string ReplaceFirst(string searchString, string replaceString, int index)
    {
      var k = inputString.IndexOf(searchString, index);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string searching in a specified text starting from the specified index for the specified characters count.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <param name="count">The number of characters to search within.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceFirst(string searchString, string replaceString, int index, int count)
    {
      var k = inputString.IndexOf(searchString, index, count, StringComparison.Ordinal);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string in a specified text. Uses a specific string comparison.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceFirst(string searchString, string replaceString, StringComparison comparisonType)
    {
      var k = inputString.IndexOf(searchString, comparisonType);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string searching in a specified text starting from the specified index. Uses a specific string comparison.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceFirst(string searchString, string replaceString, int index, StringComparison comparisonType)
    {
      var k = inputString.IndexOf(searchString, index, comparisonType);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string searching in a specified text starting from the specified index for the specified characters count.
    /// Uses a specific string comparison.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <param name="count">The number of characters to search within.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceFirst
      (string searchString, string replaceString, int index, int count, StringComparison comparisonType)
    {
      var k = inputString.IndexOf(searchString, index, count, comparisonType);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the last occurence of the string searching in a specified text.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceLast(string searchString, string replaceString)
    {
      var k = inputString.LastIndexOf(searchString, StringComparison.Ordinal);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the last occurence of the string searching in a specified text starting from the specified index
    /// and continuing towards the begining of the text.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceLast(string searchString, string replaceString, int index)
    {
      var k = inputString.LastIndexOf(searchString, index, StringComparison.Ordinal);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the last occurence of the string searching in a specified text starting from the specified index
    /// and continuing towards the begining of the text for the specified characters count.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <param name="count">The number of characters to search within.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceLast(string searchString, string replaceString, int index, int count)
    {
      var k = inputString.LastIndexOf(searchString, index, count, StringComparison.Ordinal);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the last occurence of the string in a specified text. Uses a specific string comparison.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceLast(string searchString, string replaceString, StringComparison comparisonType)
    {
      var k = inputString.LastIndexOf(searchString, comparisonType);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string searching in a specified text starting from the specified index
    /// and continuing towards the begining of the text. Uses a specific string comparison.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceLast(string searchString, string replaceString, int index, StringComparison comparisonType)
    {
      var k = inputString.LastIndexOf(searchString, index, comparisonType);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Replaces the first occurence of the string searching in a specified text starting from the specified index
    /// and continuing towards the begining of the text for the specified characters count.
    /// Uses a specific string comparison.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="replaceString">The string to replace with.</param>
    /// <param name="index">The starting index for the search.</param>
    /// <param name="count">The number of characters to search within.</param>
    /// <param name="comparisonType">The string comparison type to use.</param>
    /// <returns>The modified string if the search string is found; otherwise, the original string.</returns>
    public string ReplaceLast
      (string searchString, string replaceString, int index, int count, StringComparison comparisonType)
    {
      var k = inputString.LastIndexOf(searchString, index, count, comparisonType);
      if (k >= 0)
        return inputString.Substring(0, k) + replaceString + inputString.Substring(k + searchString.Length);

      return inputString;
    }

    /// <summary>
    /// Returns a string shortened by a specified character count.
    /// </summary>
    /// <param name="charCount">The number of characters to remove from the end of the string.</param>
    /// <returns>The shortened string if the input string is not null; otherwise, null.</returns>
    public string Shorten(int charCount)
    {
      return inputString.Substring(0, inputString.Length - charCount);
    }

    /// <summary>
    /// Makes English plural form of the noun.
    /// </summary>
    /// <returns>The plural form of the noun if the input string is not null; otherwise, null.</returns>
    public string Pluralize()
    {
      if (inputString.EndsWith("y"))
        return inputString.Shorten(1) + "ies";
      if (inputString.EndsWith("s"))
        return inputString + "es";
      if (inputString.EndsWith("ss"))
        return inputString;

      return inputString + "s";
    }

    /// <summary>
    /// Makes English singular form of the noun.
    /// </summary>
    /// <returns>The singular form of the noun if the input string is not null; otherwise, null.</returns>
    public string Singularize()
    {
      if (inputString.EndsWith("ies"))
        return inputString.Shorten(3) + "y";
      if (inputString.EndsWith("ses"))
        return inputString.Shorten(2);
      if (inputString.EndsWith("ss"))
        return inputString;
      if (inputString.EndsWith("s"))
        return inputString.Shorten(1);

      return inputString;
    }

    /// <summary>
    /// Decodes escapes sequences: \\, \t, \r, \n, \s, \u
    /// </summary>
    /// <returns>The decoded string if the input string is not null; otherwise, null.</returns>
    public string DecodeEscapeSeq()
    {
      int index = 0;
      var sb = new StringBuilder();
      while (index < inputString.Length)
      {
        char c = inputString[index];
        if (c == '\\')
          sb.Append(inputString.DecodeEscapeSeq_(ref index));
        else
          sb.Append(c);
        index++;
      }
      return sb.ToString();
    }

    /// <summary>
    /// Helper method for <see cref="DecodeEscapeSeq"/>. Decodes escape sequence at the specified position and moves the position to the end of the sequence.
    /// </summary>
    /// <param name="index">The current position in the string. This will be updated to the end of the escape sequence.</param>
    /// <returns>The decoded escape sequence if the input string is not null; otherwise, null.</returns>
    private string DecodeEscapeSeq_(ref int index)
    {
      char c = inputString[index + 1];
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
              char c2 = inputString[index + i];
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
    /// <param name="count">The number of times to repeat the string.</param>
    /// <returns>The repeated string if the input string is not null; otherwise, null.</returns>
    public string Duplicate(int count)
    {
      var sb = new StringBuilder(inputString.Length * count);
      for (int i = 0; i < count; i++)
        sb.Append(inputString);
      return sb.ToString();
    }

    /// <summary>
    /// Check if a string ends with a specified substring and if so - returns the string with this substring removed.
    /// </summary>
    /// <param name="trimString">Substring to remove from the end of the string</param>
    /// <returns>Trimmed string if the substring is found at the end, otherwise the original string</returns>
    public string TrimEnd(string trimString)
    {
      if (inputString.EndsWith(trimString))
        return inputString.Substring(0, inputString.Length - trimString.Length);

      return inputString;
    }

    /// <summary>
    /// Check if a string starts with a specified substring and if so - returns the string with this substring removed.
    /// </summary>
    /// <param name="trimString">Substring to remove from the start of the string</param>
    /// <returns>Trimmed string if the substring is found at the start, otherwise the original string</returns>
    public string TrimStart(string trimString)
    {
      if (inputString.StartsWith(trimString))
        return inputString.Substring(trimString.Length);

      return inputString;
    }
  }

  /// <summary>
  /// Concatenates two strings with a separator between them.
  /// If any of both is empty of null - the other is returned.
  /// </summary>
  /// <param name="str1">The first string to concatenate.</param>
  /// <param name="separator">The string to use as a separator.</param>
  /// <param name="str2">The second string to concatenate.</param>
  /// <returns>The concatenated string if both strings are not null or empty; otherwise, the non-null string.</returns>
  public static string? Concat2(string? str1, string? separator, string? str2)
  {
    if (String.IsNullOrEmpty(str1))
      return str2;
    if (String.IsNullOrEmpty(str2))
      return str1;

    return str1 + separator + str2;
  }


  /// <summary>
  /// Changes a number to literal text using whole part and a rational fraction part.
  /// </summary>
  /// <param name="number">The number to convert to text.</param>
  /// <returns>The textual representation of the number.</returns>
  public static string? NumberToText(this double number)
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
  /// <param name="number">The number to convert to text.</param>
  /// <returns>The textual representation of the number.</returns>
  public static string? NumberToText(this int number)
  {
    return ((Int64)number).NumberToText();
  }

  /// <summary>
  /// Changes a number to literal text.
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  public static string? NumberToText(this Int64 number)
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

      var unitsMap = new[]
      {
        "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve",
        "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
      };
      var tensMap = new[]
        { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

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

}
