using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Qhta.Erratum
{
  public class Errata : Dictionary<string, List<ErrLine>>, IXmlSerializable
  {
    public Errata() { }

    public XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      reader.ReadStartElement("Errata");
      reader.MoveToContent();
      while (reader.IsStartElement("file"))
      {
        string filename = reader.GetAttribute("name");
        var list = new List<ErrLine>();
        reader.ReadStartElement();
        reader.MoveToContent();
        while (reader.IsStartElement("line") || reader.IsStartElement("lines"))
        {
          bool multi = reader.IsStartElement("lines");
          int? number = null;
          int? from = null;
          int? to = null;
          if (!multi)
            number = GetIntAttribute(reader, "number");
          else
          {
            from = GetIntAttribute(reader, "from");
            to = GetIntAttribute(reader, "to");
          }


          reader.ReadStartElement();
          reader.MoveToContent();
          if (!reader.IsStartElement("text"))
            throw new InvalidOperationException($"Text element missing when reading errata file");
          reader.ReadStartElement();
          reader.MoveToContent();
          var text = reader.ReadContentAsString();
          reader.ReadEndElement(); // "text"
          reader.MoveToElement();
          string repl = null;
          int move = 0;
          //bool delete = false;
          if (reader.IsStartElement("repl"))
          {
            reader.ReadStartElement();
            reader.MoveToContent();
            repl = reader.ReadContentAsString();
            reader.ReadEndElement(); // "repl"
          }
          else if (reader.IsStartElement("moveUp"))
          {
            reader.ReadStartElement();
            reader.MoveToContent();
            move = -reader.ReadContentAsInt();
            reader.ReadEndElement(); // "moveUp"
          }
          else if (reader.IsStartElement("moveDown"))
          {
            reader.ReadStartElement();
            reader.MoveToContent();
            move = reader.ReadContentAsInt();
            reader.ReadEndElement(); // "moveDown"
          }
          else if (reader.IsStartElement("delete"))
          {
            reader.ReadStartElement();
            //delete = true;
            reader.ReadEndElement(); // "delete"
          }
          reader.ReadEndElement(); // "line"
          ErrLine errLine;
          if (number != null)
          {
            if (move != 0)
              errLine = new ErrLine((int)number, text, move);
            else
              errLine = new ErrLine((int)number, text, repl);
          }
          else
          {
            int count = int.MaxValue;
            if (to != null)
              count = (int)to - (int)from;
            if (move != 0)
              errLine = new ErrLine((int)from, count, text, move);
            else
              errLine = new ErrLine((int)from, count, text, repl);

          }
          list.Add(errLine);
          reader.MoveToElement();
        }
        reader.ReadEndElement(); // "file"
        this.Add(filename, list);
      }
      reader.ReadEndElement(); // "Errata"
    }

    private int? GetIntAttribute(XmlReader reader, string attrName)
    {
      string numberStr = reader.GetAttribute(attrName);
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
        writer.WriteStartElement("file");
        writer.WriteAttributeString("name", item.Key);
        foreach (var lineItem in item.Value)
        {
          if (lineItem.Count == 1)
          {
            writer.WriteStartElement("line");
            writer.WriteAttributeString("number", lineItem.Number.ToString());
          }
          else
          {
            writer.WriteStartElement("lines");
            writer.WriteAttributeString("from", lineItem.Number.ToString());
            if (lineItem.Count != int.MaxValue)
              writer.WriteAttributeString("to", (lineItem.Number + lineItem.Count).ToString());
          }
          writer.WriteElementString("text", lineItem.Text);
          if (lineItem.Move != 0)
          {
            if (lineItem.Move < 0)
              writer.WriteElementString("moveUp", (-lineItem.Move).ToString());
            else
              writer.WriteElementString("moveDown", (lineItem.Move).ToString());
          }
          else
            writer.WriteElementString("repl", lineItem.Repl);
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
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
      if (!TryGetValue(filename, out var list))
        return false;
      var lines = replText.Split('\n').ToList();
      foreach (var lineItem in list.OrderByDescending(item => item.Number))
      {
        string find = lineItem.Text.Replace("\\n", "\n");
        string repl = lineItem.Repl?.Replace("\\n", "\n");
        int move = lineItem.Move;
        int lineNum = lineItem.Number - 1;
        if (lineNum >= 0 && lineNum < lines.Count())
        {
          int count = 1;
          if (lineItem.Count == int.MaxValue)
            count = lines.Count - lineNum;
          else
            count = lineItem.Count;
          for (int i = 0; i<count; i++)
          {
            int lineNdx = lineNum+i;
            if (lineItem.Repl==null)
              lineNdx = lineNum + Count - i - 1;
            var aLine = lines[lineNdx] + "\n";
            bool found = aLine.IndexOf(find) >= 0;
            if (move != 0)
            {
              if (found)
              {
                lines.RemoveAt(lineNdx);
                lines.Insert(lineNdx + move, TrimLastNL(aLine));
              }
            }
            else
            if (lineItem.Repl == null)
            {
              if (found)
              {
                aLine = aLine.Replace(find, repl);
                if (aLine == "")
                  lines.RemoveAt(lineNdx);
                else
                  lines[lineNdx] = TrimLastNL(aLine);
              }
            }
            else
            {
              aLine = aLine.Replace(find, repl);
              lines[lineNdx] = TrimLastNL(aLine);
            }
          }
        }
      }
      replText = String.Join("\n", lines);
      var done = replText != origText;
      return done;
    }



    private string TrimLastNL(string str)
    {
      if (str.LastOrDefault() == '\n')
        str = str.Substring(0, str.Length - 1);
      return str;
    }
  }
}
