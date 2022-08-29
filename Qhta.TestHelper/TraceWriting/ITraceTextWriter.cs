using System.IO;

namespace Qhta.TestHelper;

/// <summary>
/// This trace writer can write text to console, to trace or to a stream.
/// It allows to write simultaneously to the three targets.
/// </summary>
public interface ITraceTextWriter: IConsoleWriter
{
  /// <summary>
  /// Controls if text is sent to console
  /// </summary>
  bool ConsoleOutputEnabled { get; set; }

  /// <summary>
  /// Controls if text is sent to trace
  /// </summary>
  bool TraceOutputEnabled { get; set; }

  /// <summary>
  /// Controls if text is sent to stream
  /// </summary>
  bool StreamOutputEnabled { get; set; }

  /// <summary>
  /// Output stream
  /// </summary>
  Stream? OutputStream { get; set; }

  /// <summary>
  /// Enables lock to the same object
  /// </summary>
  object LockObject { get; set; }
}