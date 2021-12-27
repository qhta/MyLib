using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
#nullable enable

namespace Qhta.Erratum
{
  public class SourceChangeMonitor : ISourceChangeMonitor
  {

    public SourceChangeMonitor(string filename)
    {
      Filename = filename;
    }

    private string Filename { get; init; }

    private Errata Errata { get; init; } = new Errata();


    public void NotifyReplacement(string? filename, int lineNum, string text, string replacement, string comment, [CallerMemberName] string? callerName = null)
    {
      var line = new ErrLine{ Number = lineNum, Text = text.Replace("\n", "\\n"), Repl = replacement.Replace("\n", "\\n"), Comment = comment};
      Errata.Add(filename ?? "", line);
    }

    public void Flush()
    {
      lock (this)
      {
        var filename = Filename;
        using (var writer = File.CreateText(filename))
        {
          Errata.WriteXml(new XmlTextWriter(writer){ Formatting  = Formatting.Indented });
        }
      }
    }

    public void Clear()
    {
      lock (this)
        Errata.Clear();
    }

  }
}
