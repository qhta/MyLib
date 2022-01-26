using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TestHelper
{
  public class DebugConsoleWriter : TextWriter, ITraceWriter
  {
    enum TagType
    {
      Reset,
      Foreground,
      Background,
    }
    struct ColorTag
    {
      public TagType Type;
      public ConsoleColor? Color;
      public ColorTag(TagType type, ConsoleColor? color=null) { Type = type; Color = color; }
    }

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public List<object> Buffer { get; init; } = new List<object>();

    public object LockObject { get; init; }

    static DebugConsoleWriter()
    {
      Debug.IndentSize=2;
      Console.ResetColor();
      initialForegroundColor = Console.ForegroundColor;
      initialBackgroundColor = Console.BackgroundColor;

    }

    static ConsoleColor initialForegroundColor;
    static ConsoleColor initialBackgroundColor;

    public DebugConsoleWriter(object lockObject) : base()
    {
      LockObject = lockObject;
    }

    public ConsoleColor ForegroundColor
    {
      get => _ForegroundColor;
      set
      {
        if (_ForegroundColor != value)
        {
          _ForegroundColor = value;
          Buffer.Add(new ColorTag(TagType.Foreground, _ForegroundColor));
        }
      }
    }
    private ConsoleColor _ForegroundColor = initialForegroundColor;

    public ConsoleColor BackgroundColor
    {
      get => _BackgroundColor;
      set
      {
        if (_BackgroundColor != value)
        {
          _BackgroundColor = value;
          Buffer.Add(new ColorTag(TagType.Background, _ForegroundColor));
        }
      }
    }
    private ConsoleColor _BackgroundColor = initialBackgroundColor;

    public void ResetColor()
    {
      _ForegroundColor = initialForegroundColor;
      _BackgroundColor = initialBackgroundColor;
      Buffer.Add(new ColorTag(TagType.Reset));
    }

    public override void Write(string? str)
    {
      if (str == null) return;
      str = str.Replace("\n", "\\n");
      Buffer.Add(str);
    }

    public override void WriteLine(string? line)
    {
      if (line == null) return;
      line = line.Replace("\n", "\\n");
      Buffer.Add(line + "\n");
    }

    public override void Flush()
    {
      lock (LockObject)
      {
        foreach (var item in Buffer)
        {
          if (item is string str)
          {
            Debug.Write(str);
            Console.Write(str);
          }
          else if (item is ColorTag tag)
          {
            switch (tag.Type)
            {
              case TagType.Foreground:
                Console.ForegroundColor = tag.Color ?? _ForegroundColor;
                break;
              case TagType.Background:
                Console.BackgroundColor = tag.Color ?? _BackgroundColor;
                break;
              default:
                Console.ResetColor();
                _ForegroundColor = Console.ForegroundColor;
                _BackgroundColor = Console.BackgroundColor;
                break;
            }
          }
        }
        Debug.Flush();
        Buffer.Clear();
      }
    }

    public void Indent() { }
    public void Unindent() { }
  }
}
