using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Qhta.TextUtils
{
  public static class StringUtils
  {

    public static string TitleCase(this string str)
    {
      var chars = str.ToCharArray();
      for (int i = 0; i < chars.Length; i++)
        if (i == 0)
          chars[i] = Char.ToUpper(chars[i]);
        else
          chars[i] = Char.ToLower(chars[i]);
      return new string(chars);
    }

    public static string CamelCase(this string str)
    {
      var ss = str.Split(' ');
      for (int i = 0; i < ss.Length; i++)
        ss[i] = ss[i].TitleCase();
      return string.Join("", ss);
    }

    public static string[] SplitCamelCase(this string str)
    {
      var ss = new List<string>();
      var chars = new List<char>();
      for (int i = 0; i < str.Length; i++)
      {
        if (Char.IsUpper(str[i]))
        {
          if (chars.Count > 0 && (!Char.IsUpper(chars.Last()) || (i < str.Length - 1 && Char.IsLower(str[i + 1]))))
          {
            var s = new String(chars.ToArray());
            ss.Add(s);
            chars.Clear();
          }
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

    public static string DeCamelCase(this string str)
    {
      var ss = str.SplitCamelCase();
      for (int i = 0; i < ss.Length; i++)
      {
        if (ss[i] != ss[i].ToUpper())
        {
          if (i == 0)
            ss[i] = ss[i].TitleCase();
          else
            ss[i] = ss[i].ToLower();
        }
      }
      return string.Join(" ", ss);
    }

    public static string NumberToText(this double number)
    {
      Int64 intPart = (Int64)number;
      double frac = number - intPart;

      string result = NumberToText(intPart);
      if (frac != 0)
      {
        int cents = (int)frac * 100;
        result += " " + cents.ToString() + "/100";
      }
      return result;
    }

    public static string NumberToText(this int number)
    {
      return NumberToText((Int64)number);
    }

    public static string NumberToText(this Int64 number)
    {
      if (number == 0)
        return "zero";

      if (number < 0)
        return "minus " + NumberToText(Math.Abs(number));

      string words = "";
      // 9223372036854775808
      if ((number / 1000000000000000000) > 0)
      {
        words += NumberToText(number / 1000000000000000000) + " quintillion ";
        number %= 1000000000000000000;
      }

      if ((number / 1000000000000000) > 0)
      {
        words += NumberToText(number / 1000000000000000) + " quadrillion ";
        number %= 1000000000000000;
      }

      if ((number / 1000000000000) > 0)
      {
        words += NumberToText(number / 1000000000000) + " trillion ";
        number %= 1000000000000;
      }

      if ((number / 1000000000) > 0)
      {
        words += NumberToText(number / 1000000000) + " billion ";
        number %= 1000000000;
      }

      if ((number / 1000000) > 0)
      {
        words += NumberToText(number / 1000000) + " million ";
        number %= 1000000;
      }

      if ((number / 1000) > 0)
      {
        words += NumberToText(number / 1000) + " thousand ";
        number %= 1000;
      }

      if ((number / 100) > 0)
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
          words += unitsMap[number];
        else
        {
          words += tensMap[number / 10];
          if ((number % 10) > 0)
            words += "-" + unitsMap[number % 10];
        }
      }

      return words;
    }

    public static string? Precede(this string str, string prefix) => str != null ? prefix + str : null;

    public static string? Attach(this string str, string suffix) => str != null ? str + suffix : null;

    public static bool ContainsAt(this string str, string substring, int atIndex)
    {
      if (atIndex + substring.Length > str.Length)
        return false;
      return str.Substring(atIndex, substring.Length).Equals(substring);
    }

    public static bool ContainsAt(this string str, string substring, int atIndex, StringComparison comparison)
    {
      if (atIndex + substring.Length > str.Length)
        return false;
      return str.Substring(atIndex, substring.Length).Equals(substring, comparison);
    }

    public static bool ContainsBefore(this string str, string substring, int atIndex)
    {
      if (atIndex - substring.Length < 0)
        return false;
      return str.Substring(atIndex - substring.Length, substring.Length).Equals(substring);
    }

    public static bool ContainsBefore(this string str, string substring, int atIndex, StringComparison comparison)
    {
      if (atIndex - substring.Length < 0)
        return false;
      return str.Substring(atIndex - substring.Length, substring.Length).Equals(substring, comparison);
    }

    [DebuggerStepThrough]
    public static bool IsUpper(this string str)
    {
      if (string.IsNullOrEmpty(str))
        return false;
      foreach (var ch in str)
        if (!(Char.IsUpper(ch)))
          return false;
      return true;
    }

    /// <summary>
    /// Checks the similiarity of key to pattern. Pattern can contain '*' as wildchar replacing any sequence of remaining characters.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="pattern"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public static bool IsLike(this string key, string pattern, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      var wildcardCount = pattern.Where(c => c == '*').Count();
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
        return key.IndexOf(pattern,stringComparison)>=0;
      }
      if (wildcardCount > 0)
      {
        var patternParts = pattern.Split('*').ToList();
        return MatchPatternParts(key, patternParts, stringComparison);
      }
      return key.Equals(pattern, stringComparison);
    }

    private static bool MatchPatternParts(string key, List<string> patternParts, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
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
      return MatchPatternParts(key, patternParts, stringComparison);
    }

    /// <summary>
    /// Checks the similiarity of key to pattern. Pattern can contain '*' as wildchar replacing any sequence of remaining characters.
    /// Output <paramref name="wildKey"/> is set to found wildchar replacement in the key.
    /// If there are multiple wildchars in the pattern then returned <paramref name="wildKey"/> contains multiple replacements joined with '*' separator.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="pattern"></param>
    /// <param name="wildKey"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public static bool IsLike(this string key, string pattern, out string? wildKey, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
      wildKey = null;
      var wildcardCount = pattern.Where(c => c == '*').Count();
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
          wildKey = key.Substring(0, pattern.Length);
          return true;
        }
        return false;
      }
      if (wildcardCount == 2 && pattern.EndsWith("*") && pattern.StartsWith("*"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        int patternPos = key.IndexOf(pattern, stringComparison);
        if (patternPos >= 0)
        {
          var wKeys = new string[2];
          wKeys[0] = key.Substring(0, patternPos);
          wKeys[1] = key.Substring(patternPos + pattern.Length);
          wildKey = String.Join("*", wKeys);
          return true;
        }
      }
      if (wildcardCount > 0)
      {
        var patternParts = pattern.Split('*').ToList();
        if (MatchPatternParts(key, patternParts, out var wildKeyParts, stringComparison))
        {
          wildKey = String.Join("*", wildKeyParts);
          return true;
        }
        return false;
      }
      return key.Equals(pattern, stringComparison);
    }

    private static bool MatchPatternParts(string key, List<string> patternParts, out List<string> wildKeyParts, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
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
      if (MatchPatternParts(key, patternParts, out var internWildParts, stringComparison))
      {
        if (internWildParts.Count > 0)
          wildKeyParts.InsertRange(1, internWildParts);
        return true;
      }
      return false;
    }

    public static bool IsLikeNumber(this string key, string pattern, out string? wildKey, out int wildNum)
    {
      if (pattern.EndsWith("#") && pattern.StartsWith("#"))
      {
        pattern = pattern.Substring(1, pattern.Length - 2);
        int patternPos = key.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase);
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

    public static bool IsValidUrl(this string text)
    {
      if (text == null)
        return false;

      Regex rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
      return rx.IsMatch(text);
    }

    [DebuggerStepThrough]
    public static string ToUpperFirst(this string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      char[] ss = text.ToCharArray();
      ss[0] = char.ToUpper(ss[0]);
      return new string(ss);
    }

    [DebuggerStepThrough]
    public static string ToLowerFirst(this string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      char[] ss = text.ToCharArray();
      ss[0] = char.ToLower(ss[0]);
      return new string(ss);
    }

    public static string TrimParens(this string text, (char open, char close)[]? enclosings = null)
      => TrimEnclosings(text, '(', ')', enclosings);

    public static string TrimEnclosings(this string text, char openParen, char closeParen, (char open, char close)[]? enclosings = null)
    {
      if (text.StartsWith(new String(openParen,1)) && text.EndsWith(new String(closeParen,1)))
      {
        if (enclosings == null)
          return text.Substring(1, text.Length - 2).Trim();

        int i;
        string? openParens = null;
        for (i = 0; i < text.Length; i++)
        {
          char ch = text[i];
          (char ch1, char ch2) = enclosings.FirstOrDefault(item => item.open == ch);
          if (ch1 != '\0')
          {
            if (ch1 == openParen)
              openParens += ch1;
            i = FindMatch(text, i, ch1, enclosings);
            if (openParens == new string(openParen, 1) && i == text.Length - 1)
            {
              return text.Substring(1, text.Length - 2).Trim();
            }
          }
          else
          if (ch == closeParen && i == text.Length - 1)
          {
            return text.Substring(1, text.Length - 2).Trim();
          }
        }
      }
      return text.Trim();
    }

    [DebuggerStepThrough]
    public static string TrimDblQuotes(this string text)
    {
      if (text.Length >= 2 && text.StartsWith("\"") && text.EndsWith("\""))
        return text.Substring(1, text.Length - 2);
      return text;
    }

    /// <summary>
    /// Splits a text by <paramref name="sep"/> character.
    /// Fragments enclosed by <paramref name="enclosings"/> are not splitted.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="enclosings"></param>
    /// <returns></returns>
    //[DebuggerStepThrough]
    public static string[] SplitBy(this string text, char sep, (char open, char close)[] enclosings)
    {
      List<string> result = new List<string>();
      int priorStart = 0;
      int i;
      for (i = 0; i < text.Length; i++)
      {
        char ch = text[i];
        (char ch1, char ch2) = enclosings.FirstOrDefault(item => item.open == ch);
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
      if (i > priorStart)
      {
        result.Add(text.Substring(priorStart, i - priorStart));
      }
      return result.ToArray();
    }

    /// <summary>
    /// Searches a text from <paramref name="startNdx"/> position for a <paramref name="sep"/> character.
    /// If not found then length of text is returned;
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startNdx"></param>
    /// <param name="sep"></param>
    /// <param name="enclosings"></param>
    /// <returns></returns>
    //[DebuggerStepThrough]
    public static int Find(this string text, int startNdx, char sep, (char open, char close)[] enclosings)
    {
      for (int i = startNdx + 1; i < text.Length; i++)
      {
        char ch = text[i];
        if (ch == sep)
          return i;
        (char ch1, char ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0')
        {
          i = FindMatch(text, i, ch1, enclosings);
        }
      }
      return text.Length;
    }

    /// <summary>
    /// Searches a text from <paramref name="startNdx"/> position for a pair of <paramref name="openingSep"/> character.
    /// If not found then length of text is returned;
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startNdx"></param>
    /// <param name="openingSep"></param>
    /// <param name="enclosings"></param>
    /// <returns></returns>
    //[DebuggerStepThrough]
    public static int FindMatch(this string text, int startNdx, char openingSep, (char open, char close)[] enclosings)
    {
      (char c1, char c2) = enclosings.FirstOrDefault(item => item.open == openingSep);
      var closingSep = c2;

      for (int i = startNdx + 1; i < text.Length; i++)
      {
        char ch = text[i];
        if (ch == closingSep)
          return i;
        (char ch1, char ch2) = enclosings.FirstOrDefault(item => item.open == ch);
        if (ch1 != '\0')
        {
          i = FindMatch(text, i, ch1, enclosings);
        }
      }
      return text.Length;
    }

    [DebuggerStepThrough]
    public static string SubstringUntil(this string text, char ch, int index)
    {
      var result = text.Substring(index);
      index = result.IndexOf(ch);
      if (index > 0)
        result = result.Substring(0, index);
      return result;
    }

    private static (char open, char close)[] DefaultBraces = new (char open, char close)[]
    {
      ( '(', ')' ),
      ( '[', ']' ),
      ( '{', '}' ),
    };

    /// <summary>
    /// Split text with delimiter omitting quotes
    /// </summary>
    [DebuggerStepThrough]
    public static string[] SplitSpecial(this string text, char delimiter, (char Open,char Close)[] bracesTuples = null!)
    {
      var result = new List<string>();
      StringBuilder sb = new StringBuilder();
      bool inQuotes = false;
      if (bracesTuples==null)
        bracesTuples = DefaultBraces;
      for (int i = 0; i < text.Length; i++)
      {
        char ch = text[i];
        if (ch == '"')
          inQuotes = !inQuotes;
        if (inQuotes)
          sb.Append(ch);
        else
        {
          (char Open, char Close) foundTuple = bracesTuples.FirstOrDefault(tuple => tuple.Open == ch);
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
              sb.Append(ch);
          }
        }
      }
      if (sb.Length > 0)
        result.Add(sb.ToString());
      return result.ToArray();
    }

    [DebuggerStepThrough]
    public static string SubstringUntil(this string text, char delimiter, int startPos, out int endPos)
    {
      for (int i = startPos; i < text.Length; i++)
      {
        char ch = text[i];
        if (ch == delimiter)
        {
          endPos = i;
          return text.Substring(startPos, i - startPos);
        }
      }
      endPos = text.Length;
      return text.Substring(startPos);
    }

    public static int FindEndOfSentence(this string text, int startPos, string[] abbreviations)
    {
      int ndx = text.IndexOf('.', startPos);
      while (ndx >= 0)
      {
        bool foundAbbr = false;
        var ndx2 = -1;
        foreach (var abbr in abbreviations)
        {
          var dotPos = abbr.IndexOf('.');
          if (dotPos<0)
            return ndx;
          var abbrPart = " " + abbr.Substring(0, dotPos);
          if (text.ContainsBefore(abbrPart, ndx))
          {
            if (dotPos == abbr.Length-1)
            {
              foundAbbr = true;
              ndx2 = text.IndexOf('.', ndx+1);
              if (ndx2 >= 0)
                break;
            }
            else
            {
              abbrPart = abbr.Substring(dotPos);
              if (text.ContainsAt(abbrPart, ndx))
              {
                foundAbbr = true;
                ndx2 = text.IndexOf('.', ndx+abbrPart.Length);
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
        else
        if (ndx2>ndx)
          ndx = ndx2;
        else
          return ndx;
          
      }
      return -1;
    }

  }
}
