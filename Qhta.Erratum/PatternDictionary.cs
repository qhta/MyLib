using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace Qhta.Erratum
{

  public static class PatternUtils
  {
    public static string EncodePattern(this string str)
    {
      var result = str.Replace("*", "\\*").Replace("A", "*").Replace(" ", "\\s").Replace("\n", "\\n").Replace("\t", "\\t");
      return result;
    }
  }

  public class PatternDictionary: SortedDictionary<String, int> 
  {
  }

  public class PatternSet
  {
    public PatternDictionary PrePatterns { get; init; } = new();
    public PatternDictionary PostPatterns { get; init; } = new();
  }

  public class PatternSetDictionary
  {
    private SortedDictionary<string, PatternSet> patternSets = new();

    public void Add(string? prePattern, string mainPattern, string? postPattern)
    {
      if (!patternSets.TryGetValue(mainPattern, out var patternSet))
      {
        patternSet = new PatternSet();
        patternSets.Add(mainPattern, patternSet);
      }
      if (!String.IsNullOrEmpty(prePattern))
      {
        if (!patternSet.PrePatterns.ContainsKey(prePattern))
          patternSet.PrePatterns.Add(prePattern, 1);
        else
          patternSet.PrePatterns[prePattern] += 1;
      }
      if (!String.IsNullOrEmpty(postPattern))
      {
        if (!patternSet.PostPatterns.ContainsKey(postPattern))
          patternSet.PostPatterns.Add(postPattern, 1);
        else
          patternSet.PostPatterns[postPattern] += 1;
      }
    }

    public void Store(string filename)
    {
      using (var writer = new StreamWriter(File.Create(filename), new UTF8Encoding(false, false)))
      {
        foreach (var item in patternSets)
        {
          var patternSet = item.Value;
          foreach (var item2 in patternSet.PrePatterns.OrderByDescending(item => item.Value))
          {
            var key2 = PatternUtils.EncodePattern(item2.Key);
            writer.WriteLine($"{key2}{item.Key}\t{item2.Value}");
          }
          foreach (var item2 in patternSet.PostPatterns.OrderByDescending(item => item.Value))
          {
            var key2 = PatternUtils.EncodePattern(item2.Key);
            writer.WriteLine($"{item.Key}{key2}\t{item2.Value}");
          }
        }
      }

    }
  }

}
