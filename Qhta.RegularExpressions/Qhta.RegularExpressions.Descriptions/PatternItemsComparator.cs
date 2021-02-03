using System;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.RegularExpressions.Descriptions
{
  public static class PatternItemsComparator
  {

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

    public static bool AreEqual(string[] first, string[] second)
    {
      bool result = true;
      if (first.Count() != second.Count())
        return false;
      int n = first.Count();
      if (second.Count() < n)
        n = second.Count();
      for (int i = 0; i < n; i++)
      {
        var item1 = first[i];
        var item2 = second[i];
        if (!AreEqual(item1, item2))
          result = false;
      }
      return result;
    }

    public static bool AreEqual(string first, string second)
    {
      first = Singularize(first);
      second = Singularize(second);
      bool result = first.Equals(second, StringComparison.CurrentCultureIgnoreCase);
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

    private static readonly SortedSet<string> NonComparedWords = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase)
    {
      "a", "an", "the",
      "marks", "mark",
      "character", "characters",
      "either", "all", "any",
      "or", "and",
      "occurrences", "occurrence",
      "occurences", "occurence",
      "of", "at",
    };

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
          chars[i] = ' ';
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
        if (NonComparedWords.Contains(word))
          words.RemoveAt(i);
      }
      return words.ToArray();
    }
  }
}
