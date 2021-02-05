using Qhta.Collections;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qhta.RegularExpressions.Descriptions
{
  public static class PatternItemsComparator
  {

    private static readonly SortedSet<string> NonComparedWords = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase)
    {
      "a", "an", "the",
      "either", "all", "any",
      "of", "at", "on", "in",
      "or", "and",
      "mark",
      "character", 
      "string", "substring",
      "occurrence", "occurrenc",
      "literal",
      "match",
      "input",
    };

    static BiDiDictionary<string, string> Synonyms = new BiDiDictionary<string, string>
    {
      { "begin", "start" },
      { "beginning", "start" },
      { "whitespace", "space" },
      { "one more", "least one" },
      { "dot", "period" },
      { "full stop", "dot" },
    }; 

    public static bool AreEqual(PatternItems first, PatternItems second)
    {
      bool result = true;
      int n = first.Count;
      if (second.Count < n)
        n = second.Count;
      for (int i = 0; i < n; i++)
      {
        var item1 = first[i];
        var item2 = second[i];
        if (!AreEqual(item1, item2))
          result = false;
      }
      if (first.Count != second.Count)
        return false;
      return result;
    }

    public static bool AreEqual(PatternItem first, PatternItem second)
    {
      bool result = true;

      if (first.Str != second.Str)
      {
        result = false;
      }

      var description1 = Prepare(TakeFirstSentence(first.Description));
      var description2 = Prepare(TakeFirstSentence(second.Description));
      if (!AreEqual(description1, description2))
      {
        result = false;
      }
      first.IsOK = result;
      return result;
    }

    public static bool AreEqual(string[] w1, string[] w2)
    {
      bool result = true;

      int n = w1.Count();
      if (w2.Count() < n)
        n = w2.Count();
      for (int i = 0; i < n; i++)
      {
        var s1 = w1[i];
        var s2 = w2[i];
        if (!AreEqual(s1, s2))
        {
          if (i<n-1)
          {
            s1 += " " + w1[i + 1];
            s2 += " " + w2[i + 1];
            if (!AreEqual(s1, s2))
              result = false;
            else 
              i++;
          }
          else
            result = false;
        }
      }
      if (w1.Count() != w2.Count())
      {
        Debug.WriteLine($"The first string has {w1.Count()} words to compare and the second has {w2.Count()} words");
        return false;
      }
      return result;
    }

    public static bool AreEqual(string s1, string s2)
    {
      string synonym;
      bool result = s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
      if (!result)
      {
        if (Synonyms.TryGetValue1(s1, out  synonym))
          result = synonym.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
        if (!result && Synonyms.TryGetValue2(s1, out synonym))
          result = synonym.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
        if (!result && Synonyms.TryGetValue1(s2, out synonym))
          result = synonym.Equals(s1, StringComparison.CurrentCultureIgnoreCase);
        if (!result && Synonyms.TryGetValue2(s2, out synonym))
          result = synonym.Equals(s1, StringComparison.CurrentCultureIgnoreCase);
        if (!result)
          Debug.WriteLine($"The following words are not equal: \"{s1}\" <=> \"{s2}\"");
      }
      return result;
    }

    public static string Singularize(string str)
    {
      if (str.Length>=5 && str.EndsWith("es"))
        str = str.Substring(0, str.Length - 2);
      else
      if (str.Length >= 4 && str.EndsWith("s"))
        str = str.Substring(0, str.Length - 1);
      return str;
    }

    private static string TakeFirstSentence(string description)
    {
      int k = description.IndexOf('.');
      if (k > 0)
        description = description.Substring(0, k+1);
      return description;
    }

    private static string[] Prepare(string description)
    {
      var chars = description.ToCharArray().ToList();
      for (int i = chars.Count - 1; i >= 0; i--)
      {
        var ch = chars[i];
        if (ch == '-')
          chars.RemoveAt(i);
        else if (ch == '"')
          chars.RemoveAt(i);
        else
        if (Char.IsPunctuation(ch))
          chars.RemoveAt(i);
      }
      description = new string(chars.ToArray());
      var words = description.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
      for (int i = words.Count - 1; i >= 0; i--)
      {
        var word = words[i];
        word = Singularize(word).ToLower();
        if (NonComparedWords.Contains(word))
          words.RemoveAt(i);
        else
          words[i] = word;
      }
      return words.ToArray();
    }
  }
}
