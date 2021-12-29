using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#nullable enable

namespace Qhta.Erratum
{
  public class Errata : Dictionary<string, ErrataFileInfo>, IXmlSerializable
  {
    public Errata() { }

    public XmlSchema? GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      reader.ReadStartElement("Errata");
      reader.MoveToContent();
      while (reader.IsStartElement("file"))
      {
        var fileInfo = new ErrataFileInfo();
        fileInfo.ReadXml(reader);
        this.Add(fileInfo.Filename ?? "", fileInfo);
      }
      reader.ReadEndElement(); // "Errata"
    }

    private string? GetStringAttribute(XmlReader reader, string attrName)
    {
      string? str = reader.GetAttribute(attrName);
      return str;
    }

    private int? GetIntAttribute(XmlReader reader, string attrName)
    {
      string? numberStr = reader.GetAttribute(attrName);
      if (numberStr != null)
      {
        if (!int.TryParse(numberStr, out var number))
          throw new InvalidOperationException($"Line number expected but \"{numberStr}\" when reading errata file");
        return number;
      }
      return null;
    }

    public void WriteXml(XmlWriter writer)
    {
      writer.WriteStartElement("Errata");
      foreach (var item in this)
      {
        item.Value.WriteXml(writer);
      }
      writer.WriteEndElement();
    }

    public bool TryRepairAndSave(string filename, string articleTitle)
    {
      if (TryGetValue(articleTitle, out var errList))
      {
        var fileText = File.ReadAllText(filename);
        if (TryRepair(articleTitle, fileText, out var replText))
        {
          var backfilename = filename + (".bak");
          File.Copy(filename, backfilename, true);
          File.WriteAllText(filename, replText);
          return true;
        }
      };
      return false;
    }

    public bool TryRepair(string filename, string origText, out string replText)
    {
      replText = origText.Replace("\r\n", "\n");
      if (!TryGetValue(filename, out var fileInfo))
        return false;
      var lines = replText.Split('\n').ToList();
      foreach (ErrataEntry entry in fileInfo)
      {
        if (entry is ErrataLine lineEntry)
          entry.Op.ExecuteAt(lines, lineEntry.Number, lineEntry.Count);
        else
        if (entry is ErrataLines linesEntry)
        {
          entry.Op.ExecuteFor(lines, linesEntry.From ?? 1, linesEntry.To);
        }

        //string find = entry.Op.FindText.Replace("\\n", "\n");
        //if (entry.Op is Replace replOp)
        //{
        //  string? repl = entry.Repl?.Replace("\\n", "\n");
        //}
        //int move = entry.Move;
        //int lineNum = entry.Number - 1;
        //if (lineNum >= 0 && lineNum < lines.Count())
        //{
        //  int count = 1;
        //  if (entry.Count == int.MaxValue)
        //    count = lines.Count - lineNum;
        //  else
        //    count = entry.Count;
        //  for (int i = 0; i < count; i++)
        //  {
        //    int lineNdx = lineNum + i;
        //    if (entry.Repl == null)
        //      lineNdx = lineNum + Count - i - 1;
        //    var aLine = lines[lineNdx] + "\n";
        //    bool found = aLine.IndexOf(find) >= 0;
        //    if (move != 0)
        //    {
        //      if (found)
        //      {
        //        lines.RemoveAt(lineNdx);
        //        lines.Insert(lineNdx + move, TrimLastNL(aLine));
        //      }
        //    }
        //    else
        //    if (entry.Repl == null)
        //    {
        //      if (found)
        //      {
        //        aLine = aLine.Replace(find, repl);
        //        if (aLine == "")
        //          lines.RemoveAt(lineNdx);
        //        else
        //          lines[lineNdx] = TrimLastNL(aLine);
        //      }
        //    }
        //    else
        //    {
        //      bool endsWithNL = aLine.EndsWith('\n');
        //      aLine = aLine.Replace(find, repl);
        //      if (endsWithNL && !aLine.EndsWith("\n") && lineNdx < lines.Count - 1)
        //      {
        //        lines[lineNdx] = aLine + lines[lineNdx + 1];
        //        lines.RemoveAt(lineNdx + 1);
        //      }
        //      else
        //        lines[lineNdx] = TrimLastNL(aLine);
        //    }
        //  }
        //}
      }
      replText = String.Join("\n", lines);
      var done = replText != origText;
      return done;
    }




  }
}
