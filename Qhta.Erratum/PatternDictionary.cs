using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#nullable enable

namespace Qhta.Erratum
{

  public static class PatternUtils
  {

    public static string CreatePattern(this string str)
    {
      if (str.Length == 0)
        return str;
      var pattern = str.ToList();
      for (int i = pattern.Count - 1; i >= 0; i--)
      {
        char ch=pattern[i];
        var cat = Char.GetUnicodeCategory(ch);
        if (Char.IsLetterOrDigit(ch))
        {
          if (i < pattern.Count - 1 && pattern[i + 1] == 'A')
            pattern.RemoveAt(i);
          else
            pattern[i] = 'A';
        }
        else if (ch == ' ' || ch == '\t' ||cat == UnicodeCategory.SpaceSeparator)
        {
          if (i < pattern.Count - 1 && pattern[i + 1] == ' ')
            pattern.RemoveAt(i);
          else
            pattern[i] = ' ';
        }
        else if (ch == '\n' || cat == UnicodeCategory.LineSeparator || cat == UnicodeCategory.ParagraphSeparator)
        {
          pattern[i] = '\n';
        }
        else if (cat==UnicodeCategory.Control || cat == UnicodeCategory.Format || cat == UnicodeCategory.Surrogate)
        {
          pattern.RemoveAt(i);
        }
      }
      var result = new string(pattern.ToArray());
      return result;
    }

    public static bool IsSpace(char ch)
    {
      return Char.IsWhiteSpace(ch) && ch != '\n';
    }

    public static string EncodePattern(this string str)
    {
      if (str.Length == 0)
        return str;
      var pattern = str.ToList();
      for (int i=pattern.Count - 1; i >= 0; i--)
      {
        var ch = pattern[i];
        if (ch == ' ')
        {
          pattern[i]='s';
          pattern.Insert(i,'\\');
        }
        else
        if (ch == '\n')
        {
          pattern[i] = 'n';
          pattern.Insert(i, '\\');
        }
        else
        if (ch == '*' || ch == '\\' )
        {
          pattern.Insert(i, '\\');
        }
        else
        if (ch == 'A')
          pattern[i]='*';
      }
      var result = new string(pattern.ToArray());
      return result;
    }

    public static string DecodePattern(this string str)
    {
      if (str.Length == 0)
        return str;
      var pattern = str.ToList();
      for (int i = 0; i < pattern.Count; i++)
      {
        var ch = pattern[i];
        var ch2 = i<pattern.Count-1 ? pattern[i+1] : '\0';
        if (ch == '\\' && ch2 == 's')
        {
          pattern.RemoveAt(i);
          pattern[i]=' ';
        }
        else
        if (ch == '\\' && ch2 == 'n')
        {
          pattern.RemoveAt(i);
          pattern[i] = '\n';
        }
        else
        if (ch == '\\' && (ch2 == '*' || ch2 =='\\'))
        {
          pattern.RemoveAt(i);
        }
        else
        if (ch == '*')
          pattern[i] = 'A';
      }
      var result = new string(pattern.ToArray());
      return result;
    }
  }

  public class PatternDictionary : SortedDictionary<String, int>
  {
  }

  public class PatternSet
  {
    public PatternDictionary PrePatterns { get; init; } = new();
    public PatternDictionary PostPatterns { get; init; } = new();
  }

  public class PatternSetDictionary: SortedDictionary<string, PatternSet>
  {
    public void Add(string? prePattern, string mainPattern, string? postPattern, int count=1)
    {
      if (!this.TryGetValue(mainPattern, out var patternSet))
      {
        patternSet = new PatternSet();
        this.Add(mainPattern, patternSet);
      }
      if (!String.IsNullOrEmpty(prePattern))
      {
        if (!patternSet.PrePatterns.ContainsKey(prePattern))
          patternSet.PrePatterns.Add(prePattern, count);
        else
        {
          if (count != 1)
            Debug.Assert(true);
          patternSet.PrePatterns[prePattern] += 1;
        }
      }
      if (!String.IsNullOrEmpty(postPattern))
      {
        if (!patternSet.PostPatterns.ContainsKey(postPattern))
          patternSet.PostPatterns.Add(postPattern, count);
        else
        {
          if (count != 1)
            Debug.Assert(true);
          patternSet.PostPatterns[postPattern] += 1;
        }
      }
    }

    public void Store(string filename)
    {
      using (var writer = new StreamWriter(File.Create(filename), new UTF8Encoding(false, false)))
      {
        foreach (var item in this)
        {
          var patternSet = item.Value;
          foreach (var item2 in patternSet.PrePatterns.OrderByDescending(item => item.Value))
          {
            if (item2.Value==10)
             break;
            var key1 = PatternUtils.EncodePattern(item2.Key);
            writer.WriteLine($"{key1}\t{item.Key}\t\t{item2.Value}");
          }
          foreach (var item2 in patternSet.PostPatterns.OrderByDescending(item => item.Value))
          {
            if (item2.Value == 10)
              break;
            var key2 = PatternUtils.EncodePattern(item2.Key);
            writer.WriteLine($"\t{item.Key}\t{key2}\t{item2.Value}");
          }
        }
      }
    }

    public void Load(string filename)
    {
      this.Clear();
      using (var reader = File.OpenText(filename))
      {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
          string[] ss = line.Split('\t');
          if (ss.Length == 4)
          {
            var prePattern = PatternUtils.DecodePattern(ss[0]);
            var pattern = ss[1];
            var postPattern = PatternUtils.DecodePattern(ss[2]);
            if (Int32.TryParse (ss[3], out var count))
              Add(prePattern, pattern, postPattern, count); 
          }
        }
      }
    }
  }

}
