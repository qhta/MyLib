using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#nullable enable

namespace Qhta.Erratum
{
  public class ErrataFileInfo : ICollection<ErrataEntry>, IXmlSerializable
  {
    public string? Filename { get; set; }

    private List<ErrataEntry> list = new();

    public ErrataFileInfo() { }

    public XmlSchema? GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      Filename = reader.GetAttribute("name");
      list = new();
      reader.ReadStartElement();
      reader.MoveToContent();
      while (reader.IsStartElement("line") || reader.IsStartElement("lines"))
      {
        ErrataEntry entry;
        if (reader.IsStartElement("line"))
        {
          var lineEntry = new ErrataLine();
          entry = lineEntry;
          var numberStr = GetStringAttribute(reader, "number");
          if (numberStr != null)
          {
            var ss = numberStr.Split('-');
            if (ss.Length > 0 && int.TryParse(ss[0].Trim(), out var n))
            {
              lineEntry.Number = n;
              if (ss.Length > 1 && int.TryParse(ss[1].Trim(), out var m))
                lineEntry.Count = m - n + 1;
            }
          }
          lineEntry.Comment = GetStringAttribute(reader, "comment");
        }
        else if (reader.IsStartElement("lines"))
        {
          var linesEntry = new ErrataLines();
          entry = linesEntry;
          linesEntry.From = GetIntAttribute(reader, "from");
          linesEntry.To = GetIntAttribute(reader, "to");
          linesEntry.Comment = GetStringAttribute(reader, "comment");
        }
        else
          continue;

        reader.ReadStartElement();
        reader.MoveToContent();
        if (!reader.IsStartElement("text"))
          throw new InvalidDataException($"Text element missing when reading errata file for \"{Filename}\"");

        reader.ReadStartElement();
        reader.MoveToContent();
        var text = reader.ReadContentAsString();
        reader.ReadEndElement(); // "text"
        reader.MoveToElement();
        if (reader.IsStartElement("repl"))
        {
          var replOp = new Replace { FindText = text };
          reader.ReadStartElement();
          reader.MoveToContent();
          replOp.ReplText = reader.ReadContentAsString();
          reader.ReadEndElement(); // "repl"
          entry.Op = replOp;
        }
        else if (reader.IsStartElement("moveUp"))
        {
          var moveOp = new Move { FindText = text };
          reader.ReadStartElement();
          reader.MoveToContent();
          moveOp.Distance = -reader.ReadContentAsInt();
          reader.ReadEndElement(); // "moveUp"
          entry.Op = moveOp;
        }
        else if (reader.IsStartElement("moveDown"))
        {
          var moveOp = new Move { FindText = text };
          reader.ReadStartElement();
          reader.MoveToContent();
          moveOp.Distance = reader.ReadContentAsInt();
          reader.ReadEndElement(); // "moveDown"
          entry.Op = moveOp;
        }
        else if (reader.IsStartElement("delete"))
        {
          var delOp = new Remove { FindText = text };
          reader.ReadStartElement();
          reader.ReadEndElement(); // "delete"
          entry.Op = delOp;
        }
        else if (reader.IsStartElement("insert"))
        {
          var insOp = new Insert { FindText = text };
          reader.ReadStartElement();
          reader.MoveToContent();
          insOp.InsText = reader.ReadContentAsString();
          reader.ReadEndElement(); // "insert"
          entry.Op = insOp;
        }
        else
          throw new InvalidDataException($"Operation element missing when reading errata file for \"{Filename}\"");
        reader.ReadEndElement(); // "line" or "lines"
        list.Add(entry);
        reader.MoveToElement();
      }
      reader.ReadEndElement(); // "file"
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
      writer.WriteStartElement("file");
      if (Filename != null)
        writer.WriteAttributeString("name", Filename);
      foreach (var item in this)
      {
        ErrataEntry? entry = null;
        if (item is ErrataLine lineEntry)
        {
          writer.WriteStartElement("line");
          var numberStr = lineEntry.Number.ToString();
          if (lineEntry.Count > 1)
            numberStr += "-" + (lineEntry.Number + lineEntry.Count).ToString();
          writer.WriteAttributeString("number", numberStr);
          entry = lineEntry;
        }
        else
        if (item is ErrataLines linesEntry)
        {
          writer.WriteStartElement("lines");
          writer.WriteAttributeString("from", linesEntry.From.ToString());
          if (linesEntry.To != null)
            writer.WriteAttributeString("to", linesEntry.To.ToString());
        }
        else
          continue;
        if (entry != null)
        {
          if (entry.Comment != null)
            writer.WriteAttributeString("comment", entry.Comment);
          var entryOp = entry.Op;
          writer.WriteElementString("text", entryOp.FindText);
          if (entryOp is Replace replOp)
          {
            writer.WriteElementString("repl", replOp.ReplText);
          }
          else
          if (entryOp is Move moveOp)
          {
            if (moveOp.Distance < 0)
              writer.WriteElementString("moveUp", (-moveOp.Distance).ToString());
            else
              writer.WriteElementString("moveDown", (moveOp.Distance).ToString());
          }
          else
          if (entryOp is Remove removeOp)
          {
            writer.WriteStartElement("remove");
            writer.WriteEndElement();
          }
          else
          if (entryOp is Insert insOp)
          {
            writer.WriteElementString("insert", insOp.InsText);
          }
        }

        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }

    public IEnumerator<object> GetEnumerator()
    {
      return ((IEnumerable<ErrataEntry>)list).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<ErrataEntry>)list).GetEnumerator();
    }

    IEnumerator<ErrataEntry> IEnumerable<ErrataEntry>.GetEnumerator()
    {
      return ((IEnumerable<ErrataEntry>)list).GetEnumerator();
    }

    public void Add(ErrataEntry item)
    {
      ((ICollection<ErrataEntry>)list).Add(item);
    }

    public void Clear()
    {
      ((ICollection<ErrataEntry>)list).Clear();
    }

    public bool Contains(ErrataEntry item)
    {
      return ((ICollection<ErrataEntry>)list).Contains(item);
    }

    public void CopyTo(ErrataEntry[] array, int arrayIndex)
    {
      ((ICollection<ErrataEntry>)list).CopyTo(array, arrayIndex);
    }

    public bool Remove(ErrataEntry item)
    {
      return ((ICollection<ErrataEntry>)list).Remove(item);
    }

    public int Count => ((ICollection<ErrataEntry>)list).Count;

    public bool IsReadOnly => ((ICollection<ErrataEntry>)list).IsReadOnly;
  }
}
