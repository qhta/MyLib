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


    public void NotifyReplacement(string? fileName, int lineNum, string text, string replacement, string comment, [CallerMemberName] string? callerName = null)
    {
      var line = new ErrataLine{ Number = lineNum, Op = new Replace{ FindText = text.Replace("\n", "\\n"), ReplText = replacement.Replace("\n", "\\n") }, Comment = comment};
      var filename = fileName ?? string.Empty;
      if (!Errata.TryGetValue(filename, out var info))
      {
        info = new ErrataFileInfo{ Filename = filename };
        Errata.Add(filename, info);
      }
      info.Add(line);
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
