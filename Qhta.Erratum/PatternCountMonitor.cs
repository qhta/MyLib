using Qhta.TestHelper;

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
  public class PatternCountMonitor : ITraceMonitor
  {

    public PatternCountMonitor(string filename)
    {
      Filename = filename;
    }

    private string Filename { get; init; }

    private PatternSetDictionary patternSets = new();

    public void Flush()
    {
      lock (this)
      {
        patternSets.Store(Filename);
      }
    }

    public void Notify(string message, [CallerMemberName] string? callerName = null)
    {
      var ss = message.Split('\t');
      if (ss.Length==3)
        lock (this)
        {
          patternSets.Add(ss[0], ss[1], ss[2]);
        }
    }

    public bool WasNotified(string message, [CallerMemberName] string? callerName = null)
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      patternSets.Clear();
    }
  }
}
