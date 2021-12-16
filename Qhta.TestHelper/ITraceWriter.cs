using System;
using System.Runtime.CompilerServices;
#nullable enable

namespace Qhta.TestHelper
{
  public interface ITraceWriter
  {
    ConsoleColor ForegroundColor {get; set; }
    ConsoleColor BackgroundColor { get; set; }
    void ResetColor();

    void Write(string str);

    void WriteLine(string line);

    void Flush();
  }
}
