using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Qhta.TestHelper
{
  public class TraceWriter : ITraceWriter, IDisposable
  {
    protected MemoryStream? _buffer { get; init; }

    protected int _bufferPos { get; set; }

    protected TextWriter _writer { get; init; }

    public TraceWriter()
    {
      _buffer = new MemoryStream();
      _writer = new StreamWriter(_buffer);
    }

    public TraceWriter(string filename)
    {
      _writer = new StreamWriter(filename);
    }


    public virtual void Flush()
    {
      if (!Enabled) return;
      _writer.Flush();
      if (_buffer != null)
      {
        int _bufSize = (int)(_buffer.Position - _bufferPos);
        if (_bufSize > 0)
        {
          var bytes = _buffer.GetBuffer()[_bufferPos..^0];
          _bufferPos += _bufSize;
          var bufStr = Encoding.UTF8.GetString(bytes);
          Trace.Write(bufStr);
          Trace.Flush();
        }
      }
    }

    public int IndentLevel { get; set; }

    public int IndentSize { get; set; } = 2;

    public bool AutoFlush { get; set; } = true;

    public char LastChar { get; private set; } = '\n';

    public bool Enabled { get; set; } = true;

    public virtual void Indent()
    {
      if (!Enabled) return;
      IndentLevel++;
    }

    public virtual void Unindent()
    {
      if (!Enabled) return;
      IndentLevel--;
    }

    public virtual void WriteLine(string str)
    {
      if (!Enabled) return;
      Write(str);
      WriteLine();
    }

    public virtual void WriteLine()
    {
      if (!Enabled) return;
      _writer.WriteLine();
      LastChar = '\n';
      if (AutoFlush)
        Flush();
    }



    public virtual void Write(string str)
    {
      if (!Enabled) return;
      if (!String.IsNullOrEmpty(str))
      {
        #region write new line if str starts with "\\n" and lastChar was "\n"
        if (str.StartsWith("\\n"))
        {
          if (LastChar != '\n')
          {
            WriteLine();
          }
          if (str.Length > 2)
            str = str.Substring(2);
          else
            str = "";
        }
        #endregion

        #region write indent chars if lastChar was "\n"
        if (LastChar == '\n')
          _writer.Write(new String(' ', IndentLevel * IndentSize));
        #endregion

        #region write space if str starts with "\\s" and lastChar was not "\n" and not space and not opening punctuation
        if (str.StartsWith("\\s"))
        {
          if (LastChar != '\n' && LastChar != ' ' && Char.GetUnicodeCategory(LastChar) != UnicodeCategory.OpenPunctuation)
          {
            _writer.Write(" ");
            LastChar = ' ';
          }
          if (str.Length > 2)
            str = str.Substring(2);
          else
            str = "";
        }
        #endregion

        #region write str interior content
        if (str != "")
        {
          _writer.Write(str);
          LastChar = str.Last();
        }
        #endregion

        if (AutoFlush)
          Flush();
      }
    }


    public virtual void WriteLine(object obj)
    {
      if (!Enabled) return;
      Write(obj);
      WriteLine();
    }

    public virtual void Write(object obj)
    {
      if (!Enabled) return;
      if (obj is string str)
        Write(str);
      else if (obj != null)
        Write(obj.ToString() ?? "");
    }


    #region ignored color management
    public ConsoleColor ForegroundColor { get; set; }

    public ConsoleColor BackgroundColor { get; set; }

    public void ResetColor() { }
    #endregion

    #region IDisposable implementation
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          Flush();
        }
        disposedValue = true;
      }
    }

    public void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}
