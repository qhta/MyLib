using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Qhta.TestHelper.TraceWriting;

namespace Qhta.TestHelper;

/// <summary>
/// Abstract implementation of a text writer
/// </summary>
public abstract class AbstractWriter: ITextWriter, IDisposable
{
  /// <summary>
  /// Buffer must not be a collection of string, because it may contain color tags.
  /// </summary>
  public List<object> Buffer { get; init; } = new List<object>();

  /// <summary>
  /// Last written character
  /// </summary>
  public char LastChar { get; private set; } = '\n';

  /// <summary>
  /// Flush invokes abstract <see cref="FlushBuffer"/>.
  /// This construction is needed to avoid premature cleaning
  /// when subclasses will invoke base method.
  /// </summary>
  public virtual void Flush() 
  {
    FlushBuffer();
    Buffer.Clear();
  }

  /// <summary>
  /// This method is abstract and must be implemented in a subclass
  /// </summary>
  protected abstract void FlushBuffer();

  /// <summary>
  /// AutoFlush means that flush is invoked after each operation
  /// </summary>
  public bool AutoFlush { get; set; } = true;

  /// <summary>
  /// Indent unit consists of two spaces
  /// </summary>
  public int IndentSize { get; set; } = 2;

  /// <summary>
  /// Indent level used at the beginning of each line
  /// </summary>
  public int IndentLevel { get; set; }

  /// <summary>
  /// Unlimited indent level increment
  /// </summary>
  public void Indent() { IndentLevel++; }

  /// <summary>
  /// Indent level decrement imited to zero
  /// </summary>
  public void Unindent() { if (IndentLevel>0) IndentLevel--; }

  /// <summary>
  /// If set to false then disables operations.
  /// </summary>
  public bool Enabled { get; set; } = true;

  /// <summary>
  /// If set then new lines are written visually, as "\n" sequence;
  /// To so, to write a "real" new line use <see cref="WriteLine()"/> method.
  /// </summary>
  public bool WriteVisualNewLine { get; set; }

  /// <summary>
  /// If set then "\n" and "\s" sequences at start of the string to write are treated specially.
  /// "\n" forces to write a new line if last char is not a new line character.
  /// "\s" forces to write a space if last char is not a space and is not an open punctuation character.
  /// </summary>
  public bool AllowIntelligentSpacing { get; set; } = true;

  /// <summary>
  /// Basic write implementation. New lines and spaces interpreted according to controlling properties.
  /// </summary>
  /// <param name="str"></param>
  public virtual void Write(string str)
  {
    if (!Enabled) return;
    if (!String.IsNullOrEmpty(str))
    {
      if (AllowIntelligentSpacing)
      {
        #region write new line if str starts with "\\n" and lastChar was "\n"
        if (str.StartsWith("\\n"))
        {
          if (LastChar != '\n')
          {
            Buffer.Add(new NewLineTag());
            LastChar = '\n';
          }
          if (str.Length > 2)
            str = str.Substring(2);
          else
            str = "";
        }
        #endregion

        #region write indent chars if lastChar was "\n"
        if (LastChar == '\n')
          Buffer.Add(new String(' ', IndentLevel * IndentSize));
        #endregion

        #region write space if str starts with "\\s" and lastChar was not "\n" and not space and not opening punctuation
        if (str.StartsWith("\\s"))
        {
          if (LastChar != '\n' && LastChar != ' ' && Char.GetUnicodeCategory(LastChar) != UnicodeCategory.OpenPunctuation)
          {
            Buffer.Add(" ");
            LastChar = ' ';
          }
          if (str.Length > 2)
            str = str.Substring(2);
          else
            str = "";
        }
        #endregion
      }

      #region write str content
      if (str != "")
      {
        string[] lines = str.Split('\n');
        for (int i=0; i<lines.Length; i++)
        { 
          if (i>0)
          {
            if (WriteVisualNewLine)
              Buffer.Add("\\n");
            else
              Buffer.Add(new NewLineTag());
          }
          Buffer.Add(lines[i]);
        }

        LastChar = str.Last();
      }
      #endregion

      if (AutoFlush)
        Flush();
    }
  }

  /// <summary>
  /// After WriteLine last char is '\n'.
  /// </summary>
  public virtual void WriteLine()
  {
    if (!Enabled) return;
    Buffer.Add(new NewLineTag());
    LastChar = '\n';
    if (AutoFlush)
      Flush();
  }

  /// <summary>
  /// Simply Write(str) and WriteLine();
  /// </summary>
  /// <param name="str"></param>
  public virtual void WriteLine(string str)
  {
    if (!Enabled) return;
    Write(str);
    WriteLine();
  }

  /// <summary>
  /// TraceListener needed for VisualStudio TraceListeners collections
  /// </summary>
  public TraceListener TraceListener
  {
    get
    {
      if (_traceListener == null)
        _traceListener = new WriterTraceListener(this);
      return _traceListener;
    }
  }
  private TraceListener? _traceListener;


  private bool disposedValue;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
}