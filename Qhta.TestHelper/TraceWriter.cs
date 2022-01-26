using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Qhta.TestHelper;

namespace Qhta.WikiNET.TextParser
{
  public class TraceWriter: ITraceWriter
  {
    public char LastChar { get; private set; } = '\n';

    public bool Enabled {get; set; }

    public void Indent()
    {
      if (!Enabled) return;
      Trace.Indent();
      Trace.Flush();
    }

    public void Unindent()
    {
      if (!Enabled) return;
      Trace.Unindent();
    }

    public void WriteLine() => WriteLine("\\n");


    public void WriteLine(string str)
    {
      if (!Enabled) return;
      Write(str);
      Trace.WriteLine("");
      Trace.Flush();
      LastChar = '\n';
    }

    public void Write(string str)
    {
      if (!Enabled) return;
      if (!String.IsNullOrEmpty(str))
      {
        if (str.StartsWith("\\n"))
        {
          if (LastChar != '\n')
          {
            Trace.WriteLine("");
            LastChar = '\n';
          }
          if (str.Length > 2)
            str = str.Substring(2);
          else
            str = "";
        }
        if (str.StartsWith("\\s"))
        {
          if (LastChar != ' ' && LastChar != '\n' && Char.GetUnicodeCategory(LastChar) != UnicodeCategory.OpenPunctuation)
          {
            Trace.Write(" ");
            LastChar = ' ';
          }
          if (str.Length > 2)
            str = str.Substring(2);
          else
            str = "";
        }
        if (str != "")
        {
          Trace.Write(str);
          LastChar = str.Last();
        }
        Trace.Flush();
      }
    }


    public void WriteLine(object obj)
    {
      if (!Enabled) return;
      Write(obj);
      WriteLine();
    }

    public void Write(object obj)
    {
      if (!Enabled) return;
      if (obj is string str)
        Write(str);
      else if (obj!=null)
        Write(obj.ToString() ?? "");
    }

    public ConsoleColor ForegroundColor { get; set; }
    public ConsoleColor BackgroundColor { get; set; }

    public void ResetColor()
    {
    }

    public void Flush()
    {
      Trace.Flush();
    }
  }
}
