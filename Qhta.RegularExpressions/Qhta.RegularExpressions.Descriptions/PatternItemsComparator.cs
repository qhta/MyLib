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
      "mark", "sign", "symbol",
      "character", 
      "string", "substring",
      "occurrence", "occurrenc",
      "literal",
      //"match", "begin", "start", "end",
      "input",
    };

    static BiDiDictionary<string, string> Synonyms = new BiDiDictionary<string, string>
    {
      { "match", "begin" },
      { "match", "start" },
      { "match", "end" },
      { "begin", "start" },
      { "beginning", "start" },
      { "whitespace", "space" },
      { "one more", "least one" },
      { "dot", "period" },
      { "full stop", "dot" },
      { "begin match", "start" },
      { "end match", "end" },
      { "begin match", "match" },
      { "start match", "start" },
      { "start match", "match" },
      { "end match", "match" },
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

      var description1 = Prepare(first.Description);
      var description2 = Prepare(second.Description);
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
      int i;
      int j;
      for (i = 0, j = 0; i < w1.Count() && j < w2.Count(); i++, j++)
      {
        string p1 = w1[i];
        string q1 = w2[j];
        string p12 = null;
        string q12 = null;
        result = AreEqual(p1, q1);
        if (i< w1.Count() - 1)
          p12 = p1 + " " + w1[i + 1];
        if (j < w2.Count() - 1)
           q12 = q1 + " " + w2[j + 1];
        if (p12!=null && q12!=null && AreEqual(p12, q12))
        {
          result = true;
          i++;
          j++;
        }
        else
        if (p12 != null && AreEqual(p12, q1))
        {
          result = true;
          i++;
        }
        else
        if (q12 != null && AreEqual(p1, q12))
        {
          result = true;
          j++;
        }
        if (!result)
        {
          Debug.WriteLine($"The following words are not equal: \"{p1}\" <=> \"{q1}\"");
          return false;
        }
      }
      if (w1.Count()-i != w2.Count()-j)
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

    //private static string TakeFirstSentence(string description)
    //{
    //  int k = description.IndexOf('.');
    //  if (k > 0)
    //    description = description.Substring(0, k+1);
    //  return description;
    //}

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
