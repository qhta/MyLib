using System.Diagnostics;

namespace Qhta.TestHelper;

/// <summary>
/// This implementation of <see cref="ITextWriter"/> uses <see cref="ConsoleWriter"/>.
/// It enables to write simultaneously to console and to trace output.
/// You can control the flush with <see cref="ConsoleWriter.ConsoleOutputEnabled"/>
/// and <see cref="TraceOutputEnabled"/> operations.
/// </summary>
public class TraceWriter : ConsoleWriter, ITextWriter
{
  /// <summary>
  /// Controls if output strings are sent to traces
  /// </summary>
  public bool TraceOutputEnabled { get; set; } = true;

  /// <summary>
  /// This implementation uses <see cref="ConsoleWriter.FlushString"/> operation first
  /// and then a string is sent to trace (if <see cref="TraceOutputEnabled"/> is set).
  /// </summary>
  /// <param name="str"></param>
  protected override void FlushString(string str)
  {
    base.FlushString(str);
    if (TraceOutputEnabled)
      Trace.Write(str);
  }


  /// <summary>
  /// This implementation uses <see cref="ConsoleWriter.FlushNewLineTag"/> operation first
  /// and then a string is sent to trace (if <see cref="TraceOutputEnabled"/> is set).
  /// </summary>
  protected override void FlushNewLineTag()
  {
    base.FlushNewLineTag();
    if (TraceOutputEnabled)
      Trace.WriteLine("");
  }

  /// <summary>
  /// This flush is needed due to trace output.
  /// </summary>
  protected override void FlushBuffer()
  {
    base.FlushBuffer();
    if (TraceOutputEnabled)
    {
      Trace.Flush();
    }
  }
}