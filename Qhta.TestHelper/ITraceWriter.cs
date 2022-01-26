using System;
using System.Runtime.CompilerServices;
#nullable enable

namespace Qhta.TestHelper
{
  public interface ITraceWriter: IConsoleWriter
  {
    //void Write(string str);

    //void WriteLine(string line);

    //void WriteLine();

    //void Flush();

    void Indent();

    void Unindent();

    int IndentLevel { get; set; }

    int IndentSize { get; set; }

    bool AutoFlush { get; set; }

  }
}
