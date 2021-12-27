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
  public class PatternCountMonitor : IPatternMonitor
  {

    public PatternCountMonitor(string filename)
    {
      Filename = filename;
    }

    private string Filename { get; init; }

    private SortedDictionary<String, int> patterns = new();

    public bool WasNotified(string msg, [CallerMemberName] string? callerName = null)
    {
      lock (this)
      {
        if (patterns.TryGetValue(msg, out var delimiterCounter))
          return true;
      }
      return false;
    }

    public void Notify(string msg, [CallerMemberName] string? callerName = null)
    {
      lock (this)
      {
        if (!patterns.TryGetValue(msg, out var delimiterCounter))
          patterns.Add(msg, 1);
        else
          patterns[msg] += 1;
      }
    }

    public void Flush()
    {
      lock (this)
      {
        var filename = Filename;
        using (var writer = new StreamWriter(File.Create(filename), new UTF8Encoding(false, false)))
        {
          foreach (var item in patterns.OrderByDescending(item => item.Value))
          {
            writer.WriteLine($"{item.Key}\t{item.Value}");
          }
        }
      }
    }

    public void Clear()
    {
      lock (this)
        patterns.Clear();
    }

    public string CreatePattern(in string str)
    {
      if (str.Length == 0)
        return str;
      var pattern = str.ToList();
      for (int i = pattern.Count - 1; i >= 0; i--)
      {
        if (Char.IsLetterOrDigit(pattern[i]))
        {
          if (i < pattern.Count - 1 && pattern[i + 1] == '*')
            pattern.RemoveAt(i);
          else
            pattern[i] = '*';
        }
        else if (pattern[i] == '*')
        {
          pattern.Insert(i, '\\');
        }
        else if (pattern[i] == ' ')
        {
          if (i < pattern.Count - 1 && pattern[i + 1] == ' ')
            pattern.RemoveAt(i);
        }
      }
      var result = new string(pattern.ToArray());
      result = result.Replace(" ", "\\s").Replace("\n", "\\n").Replace("\t", "\\t");
      return result;
    }

  }
}
