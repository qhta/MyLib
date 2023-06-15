using System.Diagnostics;

namespace Qhta.TestHelper.TraceWriting;

/// <summary>
/// Class used as a listener for <see cref="System.Diagnostics.Trace"/> output.
/// </summary>
public class WriterTraceListener : TraceListener
{
  /// <summary>
  /// Connects listener to <see cref="BufferedTextWriter"/>
  /// </summary>
  /// <param name="writer"></param>
  public WriterTraceListener(BufferedTextWriter writer)
  { _writer = writer; }

  private BufferedTextWriter _writer;

  /// <summary>
  /// Implemented method to transfer message to base writer.
  /// </summary>
  /// <param name="message"></param>
  public override void Write(string? message)
  {
    _writer.Write(message ?? "");
  }

  /// <summary>
  /// Implemented method to transfer message line to base writer.
  /// </summary>
  /// <param name="message"></param>
  public override void WriteLine(string? message)
  {
    _writer.WriteLine(message ?? "");
  }
}
