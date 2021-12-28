using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
#nullable enable

namespace Qhta.Erratum
{
  public class PatternCountMonitor// : IPatternMonitor
  {

    public PatternCountMonitor(string filename)
    {
      Filename = filename;
    }

    private string Filename { get; init; }

    public string CreatePattern(string str)
    {
      if (str.Length == 0)
        return str;
      var pattern = str.ToList();
      for (int i = pattern.Count - 1; i >= 0; i--)
      {
        if (Char.IsLetterOrDigit(pattern[i]))
        {
          if (i < pattern.Count - 1 && pattern[i + 1] == 'A')
            pattern.RemoveAt(i);
          else
            pattern[i] = 'A';
        }
        else if (pattern[i] == ' ')
        {
          if (i < pattern.Count - 1 && pattern[i + 1] == ' ')
            pattern.RemoveAt(i);
        }
      }
      var result = new string(pattern.ToArray());
      return result;
    }

    private PatternSetDictionary patternSets = new();

    public void Notify(string? prePattern, string mainPattern, string? postPattern)
    {
      //Notify(prePattern + mainPattern);
      //Notify(prePattern + mainPattern + postPattern);
      //Notify(mainPattern + postPattern);
      lock (this)
      {
        patternSets.Add(prePattern, mainPattern, postPattern);
      }
    }

    //public void Notify(string pattern)
    //{
      //lock (this)
      //{
      //  if (!patterns.TryGetValue(pattern, out var delimiterCounter))
      //    patterns.Add(pattern, 1);
      //  else
      //    patterns[pattern] += 1;
      //}
    //}

    public void Flush()
    {
      lock (this)
      {
        patternSets.Store(Filename);
      }
    }

    //public void Clear()
    //{
      //lock (this)
      //  patterns.Clear();
    //}

  }
}
